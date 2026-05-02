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

    [ObservableProperty] private decimal _totalRevenue;
    [ObservableProperty] private decimal _totalExpenses;
    [ObservableProperty] private decimal _netProfit;

    public ObservableCollection<MonthlyData> MonthlyStats { get; } = [];
    public ObservableCollection<RecentActivityItem> RecentActivity { get; } = [];

    public DashboardViewModel(QuickPosDbContext db)
    {
        _db = db;
        LoadDataCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        IsBusy = true;
        try
        {
            var now = DateTime.UtcNow;
            var startOfYear = new DateTime(now.Year, 1, 1);

            _totalRevenue = await _db.Transactions
                .Where(t => t.CreatedAt >= startOfYear)
                .SumAsync(t => t.TotalAmount);

            _totalExpenses = await _db.Expenses
                .Where(e => e.CreatedAt >= startOfYear)
                .SumAsync(e => e.Amount);

            _netProfit = _totalRevenue - _totalExpenses;
            // Monthly stats
            MonthlyStats.Clear();
            for (int m = 1; m <= now.Month; m++)
            {
                var monthStart = new DateTime(now.Year, m, 1);
                var monthEnd = monthStart.AddMonths(1);
                var rev = await _db.Transactions
                    .Where(t => t.CreatedAt >= monthStart && t.CreatedAt < monthEnd)
                    .SumAsync(t => t.TotalAmount);
                var exp = await _db.Expenses
                    .Where(e => e.CreatedAt >= monthStart && e.CreatedAt < monthEnd)
                    .SumAsync(e => e.Amount);
                MonthlyStats.Add(new MonthlyData
                {
                    Month = monthStart.ToString("MMM"),
                    Revenue = rev,
                    Expenses = exp
                });
            }

            // Recent activity
            RecentActivity.Clear();
            var recent = await _db.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(8)
                .ToListAsync();
            foreach (var t in recent)
            {
                RecentActivity.Add(new RecentActivityItem
                {
                    Description = $"Transaction #{t.Id}",
                    Amount = t.TotalAmount,
                    Date = t.CreatedAt,
                    Type = "Sale"
                });
            }
        }
        finally { IsBusy = false; }
    }
}

public class MonthlyData
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal MaxValue => Math.Max(Revenue, Expenses);
}

public class RecentActivityItem
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
}
