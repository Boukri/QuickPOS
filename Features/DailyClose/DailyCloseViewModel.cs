using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Ardalis.Specification;
using QuickPOS.Core;
using QuickPOS.Data;
using System.Collections.ObjectModel;
using DailyCloseModel = QuickPOS.Models.Entities.DailyClose;

namespace QuickPOS.Features.DailyClose;

public partial class DailyCloseViewModel : ViewModelBase
{
    private readonly QuickPosDbContext _db;
    private readonly IRepositoryBase<DailyCloseModel> _dailyCloseRepo;

    [ObservableProperty] private decimal _systemCashTotal;
    [ObservableProperty] private decimal _systemCardTotal;
    [ObservableProperty] private decimal _systemPointsTotal;
    [ObservableProperty] private decimal _systemGrandTotal;

    [ObservableProperty] private decimal _manualCashCount;
    [ObservableProperty] private decimal _cashDiscrepancy;

    [ObservableProperty] private bool _isCashOutModalVisible;

    // Cash denomination counts
    [ObservableProperty] private int _count100;
    [ObservableProperty] private int _count50;
    [ObservableProperty] private int _count20;
    [ObservableProperty] private int _count10;
    [ObservableProperty] private int _count5;
    [ObservableProperty] private int _count1;
    [ObservableProperty] private int _countQuarters;
    [ObservableProperty] private int _countDimes;
    [ObservableProperty] private int _countNickels;
    [ObservableProperty] private int _countPennies;

    public ObservableCollection<TransactionSummaryItem> TodayTransactions { get; } = [];

    public DailyCloseViewModel(QuickPosDbContext db, IRepositoryBase<DailyCloseModel> dailyCloseRepo)
    {
        _db = db;
        _dailyCloseRepo = dailyCloseRepo;
        LoadTodayDataCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadTodayData()
    {
        IsBusy = true;
        try
        {
            var today = DateTime.UtcNow.Date;
            var transactions = await _db.Transactions
                .Where(t => t.CreatedAt.Date == today)
                .ToListAsync();

            SystemCashTotal = transactions.Where(t => t.PaymentMethod == "Cash").Sum(t => t.TotalAmount);
            SystemCardTotal = transactions.Where(t => t.PaymentMethod == "Card").Sum(t => t.TotalAmount);
            SystemPointsTotal = transactions.Where(t => t.PaymentMethod == "Points").Sum(t => t.TotalAmount);
            SystemGrandTotal = SystemCashTotal + SystemCardTotal + SystemPointsTotal;

            TodayTransactions.Clear();
            foreach (var t in transactions.OrderByDescending(t => t.CreatedAt))
            {
                TodayTransactions.Add(new TransactionSummaryItem
                {
                    TransactionId = t.Id,
                    Amount = t.TotalAmount,
                    PaymentMethod = t.PaymentMethod,
                    Time = t.CreatedAt
                });
            }
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void ShowCashOutModal() => IsCashOutModalVisible = true;

    [RelayCommand]
    private void CloseCashOutModal() => IsCashOutModalVisible = false;

    [RelayCommand]
    private async Task SubmitCashOut()
    {
        var dailyClose = new DailyCloseModel
        {
            CreatedAt = DateTime.UtcNow,
            SystemCashTotal = SystemCashTotal,
            SystemCardTotal = SystemCardTotal,
            SystemPointsTotal = SystemPointsTotal,
            ManualCashTotal = ManualCashCount,
            ClosedBy = "Current User"
        };

        await _dailyCloseRepo.AddAsync(dailyClose);
        IsCashOutModalVisible = false;
    }

    private void RecalculateManualCount()
    {
        ManualCashCount = (Count100 * 100m) + (Count50 * 50m) + (Count20 * 20m) + 
                          (Count10 * 10m) + (Count5 * 5m) + (Count1 * 1m) +
                          (CountQuarters * 0.25m) + (CountDimes * 0.10m) + 
                          (CountNickels * 0.05m) + (CountPennies * 0.01m);
        CashDiscrepancy = ManualCashCount - SystemCashTotal;
    }

    partial void OnCount100Changed(int value) => RecalculateManualCount();
    partial void OnCount50Changed(int value) => RecalculateManualCount();
    partial void OnCount20Changed(int value) => RecalculateManualCount();
    partial void OnCount10Changed(int value) => RecalculateManualCount();
    partial void OnCount5Changed(int value) => RecalculateManualCount();
    partial void OnCount1Changed(int value) => RecalculateManualCount();
    partial void OnCountQuartersChanged(int value) => RecalculateManualCount();
    partial void OnCountDimesChanged(int value) => RecalculateManualCount();
    partial void OnCountNickelsChanged(int value) => RecalculateManualCount();
    partial void OnCountPenniesChanged(int value) => RecalculateManualCount();
}

public class TransactionSummaryItem
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}
