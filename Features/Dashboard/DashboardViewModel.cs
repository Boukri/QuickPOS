using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuickPOS.Core;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.Features.Dashboard;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly QuickPosDbContext _db;

    // ── Financial (Year-to-date) ──────────────────────────────────────────
    [ObservableProperty] private decimal _totalRevenue;
    [ObservableProperty] private decimal _totalExpenses;
    [ObservableProperty] private decimal _netProfit;
    [ObservableProperty] private decimal _totalCogs;
    [ObservableProperty] private decimal _grossMarginPct;

    // ── Today ─────────────────────────────────────────────────────────────
    [ObservableProperty] private decimal _todayRevenue;
    [ObservableProperty] private int     _todayTransactionCount;

    // ── Inventory ─────────────────────────────────────────────────────────
    [ObservableProperty] private decimal _totalStockValue;
    [ObservableProperty] private int     _totalStockUnits;
    [ObservableProperty] private decimal _weightedAverageCost;
    [ObservableProperty] private int     _lowStockCount;
    [ObservableProperty] private int     _outOfStockCount;

    // ── Catalogue ─────────────────────────────────────────────────────────
    [ObservableProperty] private int _totalProductCount;
    [ObservableProperty] private int _totalCategoryCount;
    [ObservableProperty] private int _serviceProductCount;

    // ── Date range filter ─────────────────────────────────────────────────
    [ObservableProperty] private DateTime _startDate = new DateTime(DateTime.Now.Year, 1, 1);
    [ObservableProperty] private DateTime _endDate   = DateTime.Now.Date;

     
    public ObservableCollection<RecentActivityItem> RecentActivity { get; } = [];
    public ObservableCollection<MonthlyData> MonthlyStats { get; set; } = [];

 
    public DashboardViewModel(QuickPosDbContext db)
    {
        _db = db;
        LoadData().ConfigureAwait(false);
       
    }

    partial void OnStartDateChanged(DateTime value) => LoadData().ConfigureAwait(false);
    partial void OnEndDateChanged(DateTime value)   => LoadData().ConfigureAwait(false);

    [RelayCommand]
    private void ApplyPreset(string preset)
    {
        var today = DateTime.Now.Date;
        (StartDate, EndDate) = preset switch
        {
            "today" => (today, today),
            "week"  => (today.AddDays(-(int)today.DayOfWeek), today),
            "month" => (new DateTime(today.Year, today.Month, 1), today),
            "year"  => (new DateTime(today.Year, 1, 1), today),
            _       => (StartDate, EndDate)
        };
    }

    [RelayCommand]
    private async Task LoadData()
    {
        IsBusy = true;
        try
        {
            var rangeStart = StartDate.Date.ToUniversalTime();
            var rangeEnd = EndDate.Date.AddDays(1).ToUniversalTime();   // exclusive upper bound
            var todayStart = DateTime.UtcNow.Date.ToUniversalTime();

            // ── Financial (range) ────────────────────────────────────────
            TotalRevenue = await _db.Transactions
                .Where(t => t.CreatedAt >= rangeStart && t.CreatedAt < rangeEnd)
                .SumAsync(t => (decimal?)t.TotalAmount) ?? 0m;

            TotalExpenses = await _db.Expenses
                .Where(e => e.CreatedAt >= rangeStart && e.CreatedAt < rangeEnd)
                .SumAsync(e => (decimal?)e.Amount) ?? 0m;

            NetProfit = TotalRevenue - TotalExpenses;

            TotalCogs = await _db.TransactionItems
                .Where(ti => ti.Transaction.CreatedAt >= rangeStart && ti.Transaction.CreatedAt < rangeEnd)
                .SumAsync(ti => (decimal?)ti.Cogs) ?? 0m;

            GrossMarginPct = TotalRevenue > 0
                ? Math.Round((TotalRevenue - TotalCogs) / TotalRevenue * 100, 1)
                : 0m;

            // ── Today ────────────────────────────────────────────────────
            TodayRevenue = await _db.Transactions
                .Where(t => t.CreatedAt >= todayStart)
                .SumAsync(t => (decimal?)t.TotalAmount) ?? 0m;

            TodayTransactionCount = await _db.Transactions
                .CountAsync(t => t.CreatedAt >= todayStart);

            // ── Inventory ────────────────────────────────────────────────
            TotalStockValue = await _db.StockBatches
                .SumAsync(b => (decimal?)(b.QuantityRemaining * b.UnitCost)) ?? 0m;

            TotalStockUnits = await _db.StockBatches
                .SumAsync(b => (int?)b.QuantityRemaining) ?? 0;

            WeightedAverageCost = TotalStockUnits > 0
                ? Math.Round(TotalStockValue / TotalStockUnits, 2)
                : 0m;

            // Low stock: physical products where total remaining < alert threshold
            var stockByProduct = await _db.StockBatches
                .GroupBy(b => b.ProductId)
                .Select(g => new { ProductId = g.Key, Remaining = g.Sum(b => b.QuantityRemaining) })
                .ToListAsync();

            var alertThresholds = await _db.Products
                .Where(p => !p.IsService && p.MinimumQuantityAlert > 0)
                .Select(p => new { p.Id, p.MinimumQuantityAlert })
                .ToListAsync();

            var remainingById = stockByProduct.ToDictionary(x => x.ProductId, x => x.Remaining);

            LowStockCount = alertThresholds.Count(p =>
            {
                var rem = remainingById.GetValueOrDefault(p.Id, 0);
                return rem > 0 && rem < p.MinimumQuantityAlert;
            });

            OutOfStockCount = await _db.Products
                .Where(p => !p.IsService &&
                            !p.Batches.Any(b => b.QuantityRemaining > 0))
                .CountAsync();

            // ── Catalogue ────────────────────────────────────────────────
            TotalProductCount = await _db.Products.CountAsync();
            TotalCategoryCount = await _db.Categories.CountAsync();
            ServiceProductCount = await _db.Products.CountAsync(p => p.IsService);

            // ── Monthly chart (months within range) ──────────────────────
            MonthlyStats.Clear();
            var chartStart = new DateTime(rangeStart.Year, rangeStart.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var chartEnd = new DateTime(rangeEnd.Year, rangeEnd.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var monthlyDataList = new List<MonthlyData>();

            for (var ms = chartStart; ms <= chartEnd; ms = ms.AddMonths(1))
            {
                var me = ms.AddMonths(1);
                var rev = await _db.Transactions
                    .Where(t => t.CreatedAt >= ms && t.CreatedAt < me)
                    .SumAsync(t => (decimal?)t.TotalAmount) ?? 0m;
                var exp = await _db.Expenses
                    .Where(e => e.CreatedAt >= ms && e.CreatedAt < me)
                    .SumAsync(e => (decimal?)e.Amount) ?? 0m;

                monthlyDataList.Add(new MonthlyData
                {
                    Month = ms.ToString("MMM"),
                    Revenue = rev,
                    Expenses = exp
                });
            }

            // ⭐ CRITICAL: Calculate global maximum across all months BEFORE adding to collection
            if (monthlyDataList.Any())
            {
                var allValues = monthlyDataList.SelectMany(m => new[] { m.Revenue, m.Expenses });
                MonthlyData.GlobalMax = allValues.Max();
            }

            // Now add to ObservableCollection
            foreach (var item in monthlyDataList)
            {
                MonthlyStats.Add(item);
            }

            // ── Recent activity ───────────────────────────────────────────
            RecentActivity.Clear();
            var recent = await _db.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(8)
                .ToListAsync();

            foreach (var t in recent)
                RecentActivity.Add(new RecentActivityItem
                {
                    Description = $"Transaction #{t.Id}",
                    Amount = t.TotalAmount,
                    Date = t.CreatedAt,
                    Type = "Sale"
                });
        }
        finally { IsBusy = false; }
    }
}

public class MonthlyData
{
    public string  Month    { get; set; } = string.Empty;
    public decimal Revenue  { get; set; } 
    public decimal Expenses { get; set; }
    // Static property for global maximum (set from ViewModel)
    public static decimal GlobalMax { get; set; } = 1;

    // Bar heights based on global maximum (max height = 140px)
    public double RevenueHeight => GlobalMax > 0
        ? Math.Max(4, (double)(Revenue / GlobalMax * 340))
        : 4;

    public double ExpensesHeight => GlobalMax > 0
        ? Math.Max(4, (double)(Expenses / GlobalMax * 340))
        : 4;

}

public class RecentActivityItem
{
    public string  Description { get; set; } = string.Empty;
    public decimal Amount      { get; set; }
    public DateTime Date       { get; set; }
    public string  Type        { get; set; } = string.Empty;
}
