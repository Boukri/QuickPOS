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
    private readonly AuthenticationService _authService;
    private readonly CurrentUserProvider _currentUser;

    // ?? Today KPI totals ?????????????????????????????????????????????????????
    [ObservableProperty] private decimal _systemCashTotal;
    [ObservableProperty] private decimal _systemCardTotal;
    [ObservableProperty] private decimal _systemPointsTotal;
    [ObservableProperty] private decimal _systemGrandTotal;

    // ?? Cash-out modal state ?????????????????????????????????????????????????
    [ObservableProperty] private string _manualCashCount = string.Empty;
    [ObservableProperty] private decimal _cashDiscrepancy;
    [ObservableProperty] private bool _isCashOutModalVisible;

    partial void OnManualCashCountChanged(string value)
    {
        CashDiscrepancy = decimal.TryParse(value, out var entered)
            ? entered - SystemCashTotal
            : 0;
    }

    // ?? Tab navigation ???????????????????????????????????????????????????????
    [ObservableProperty] private int _activeTab;

    partial void OnActiveTabChanged(int value)
    {
        if (value == 1 && IsAdmin)
            LoadHistory().ConfigureAwait(false);
    }

    // ?? Admin gate ???????????????????????????????????????????????????????????
    public bool IsAdmin => _authService.IsAdmin;

    // ?? Collections ??????????????????????????????????????????????????????????
    public ObservableCollection<TransactionSummaryItem> TodayTransactions { get; } = [];
    public ObservableCollection<DailyCloseHistoryRow>   ClosureHistory    { get; } = [];

    // ?? Constructor ??????????????????????????????????????????????????????????
    public DailyCloseViewModel(
        QuickPosDbContext db,
        CurrentUserProvider currentUser,
        IRepositoryBase<DailyCloseModel> dailyCloseRepo,
        AuthenticationService authService)
    {
        _db             = db;
        _dailyCloseRepo = dailyCloseRepo;
        _currentUser    = currentUser;
        _authService    = authService;
        LoadTodayData().ConfigureAwait(false);
    }

    // ?? Today data ???????????????????????????????????????????????????????????
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

            SystemCashTotal   = transactions.Where(t => t.PaymentMethod == "Cash").Sum(t => t.TotalAmount);
            SystemCardTotal   = transactions.Where(t => t.PaymentMethod == "Card").Sum(t => t.TotalAmount);
            SystemPointsTotal = transactions.Where(t => t.PaymentMethod == "Points").Sum(t => t.TotalAmount);
            SystemGrandTotal  = SystemCashTotal + SystemCardTotal + SystemPointsTotal;

            TodayTransactions.Clear();
            foreach (var t in transactions.OrderByDescending(t => t.CreatedAt))
            {
                TodayTransactions.Add(new TransactionSummaryItem
                {
                    TransactionId = t.Id,
                    Amount        = t.TotalAmount,
                    PaymentMethod = t.PaymentMethod,
                    Time          = t.CreatedAt
                });
            }
        }
        finally { IsBusy = false; }
    }

    // ?? Tab commands ?????????????????????????????????????????????????????????
    [RelayCommand] private void SwitchToTodayTab()   => ActiveTab = 0;
    [RelayCommand] private void SwitchToHistoryTab() { if (IsAdmin) ActiveTab = 1; }

    // ?? History data (admin only) ????????????????????????????????????????????
    [RelayCommand]
    private async Task LoadHistory()
    {
        IsBusy = true;
        try
        {
            var records = await _db.DailyCloses
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            ClosureHistory.Clear();
            foreach (var r in records)
            {
                ClosureHistory.Add(new DailyCloseHistoryRow
                {
                    Id           = r.Id,
                    ClosedAt     = r.CreatedAt,
                    SystemCash   = r.SystemCashTotal,
                    SystemCard   = r.SystemCardTotal,
                    SystemPoints = r.SystemPointsTotal,
                    SystemTotal  = r.SystemCashTotal + r.SystemCardTotal + r.SystemPointsTotal,
                    ManualCash   = r.ManualCashTotal,
                    Discrepancy  = r.CashDiscrepancy,
                    ClosedBy     = r.ClosedBy
                });
            }
        }
        finally { IsBusy = false; }
    }

    // ?? Cash-out modal commands ???????????????????????????????????????????????
    [RelayCommand] private void ShowCashOutModal()  => IsCashOutModalVisible = true;
    [RelayCommand] private void CloseCashOutModal() => IsCashOutModalVisible = false;

    [RelayCommand]
    private async Task SubmitCashOut()
    {
        _ = decimal.TryParse(ManualCashCount, out var manualTotal);

        var dailyClose = new DailyCloseModel
        {
            CreatedAt        = DateTime.Now,
            SystemCashTotal  = SystemCashTotal,
            SystemCardTotal  = SystemCardTotal,
            SystemPointsTotal = SystemPointsTotal,
            ManualCashTotal  = manualTotal,
            ClosedBy         = _currentUser.Username ?? "System"
        };

        await _dailyCloseRepo.AddAsync(dailyClose);
        IsCashOutModalVisible = false;
        ManualCashCount       = string.Empty;

        // refresh history so the new record appears immediately
        if (IsAdmin)
            await LoadHistory();
    }
}

// ?? Supporting row models ?????????????????????????????????????????????????????

public class TransactionSummaryItem
{
    public int    TransactionId { get; set; }
    public decimal Amount       { get; set; }
    public string  PaymentMethod { get; set; } = string.Empty;
    public DateTime Time         { get; set; }
}

public class DailyCloseHistoryRow
{
    public int     Id           { get; set; }
    public DateTime ClosedAt    { get; set; }
    public decimal SystemCash   { get; set; }
    public decimal SystemCard   { get; set; }
    public decimal SystemPoints { get; set; }
    public decimal SystemTotal  { get; set; }
    public decimal ManualCash   { get; set; }
    public decimal Discrepancy  { get; set; }
    public string  ClosedBy     { get; set; } = string.Empty;
}
