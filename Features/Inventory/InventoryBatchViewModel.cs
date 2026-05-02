using System.Collections.ObjectModel;
using Ardalis.Specification;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickPOS.Core;
using QuickPOS.Data.Specifications;
using QuickPOS.Models.Entities;
using QuickPOS.Models.Enums;

namespace QuickPOS.Features.Inventory;

public partial class InventoryBatchViewModel : ViewModelBase
{
    private readonly IRepositoryBase<Product> _productRepo;
    private readonly IRepositoryBase<StockBatch> _batchRepo;
    private readonly IStockService _stockService;

    // Product selector 
    public ObservableCollection<ProductSummary> Products { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WeightedAverageCost))]
    [NotifyPropertyChangedFor(nameof(TotalUnits))]
    [NotifyPropertyChangedFor(nameof(TotalStockValue))]
    private ProductSummary? _selectedProduct;

    //partial void OnSelectedProductChanged(ProductSummary? value)
    //{
    //    if (value is not null)
    //    {
    //        // Seed the receive-form prices from the currently saved product values.
    //        _receiveSellPrice     = value.RetailPrice;
    //        _receiveCostingMethod = value.CostingMethod;
    //    }
    //}

    //  Batch list  
    public ObservableCollection<StockBatchRowViewModel> Batches { get; } = [];

    // ?? Computed stats ????????????????????????????????????????????????????
    /// <summary>Sum of all remaining units across active batches.</summary>
    public int TotalUnits => Batches.Sum(b => b.QuantityRemaining);

    /// <summary>Total inventory value (? remaining × unit cost).</summary>
    public decimal TotalStockValue => Batches.Sum(b => b.RemainingValue);

    /// <summary>Weighted Average Cost = total value / total units.</summary>
    public decimal WeightedAverageCost =>
        TotalUnits > 0 ? Math.Round(TotalStockValue / TotalUnits, 4) : 0m;

    // ?? COGS tracker ??????????????????????????????????????????????????????
    [ObservableProperty]
    private decimal _sessionCOGS;

    [ObservableProperty]
    private string _cogsMessage = string.Empty;

    // ?? Dialog events ?????????????????????????????????????????????????????
    public event EventHandler? ReceiveStockRequested;
    public event EventHandler? ReceiveStockCompleted;
    public event EventHandler? SellRequested;
    public event EventHandler? SellCompleted;

    // Receive-stock form
    [ObservableProperty]
    private int _receiveQuantity = 1;

    [ObservableProperty]
    private decimal _receiveUnitCost;

    [ObservableProperty]
    private decimal _receiveSellPrice;

    [ObservableProperty]
    private CostingMethod _receiveCostingMethod = CostingMethod.WeightedAverage;

    [ObservableProperty]
    private string _receiveReference = string.Empty;

    // Sell form  
    [ObservableProperty]
    private int _sellQuantity = 1;

    [ObservableProperty]
    private string _sellFormError = string.Empty;

    // Constructor 
    public InventoryBatchViewModel(
        IRepositoryBase<Product> productRepo,
        IRepositoryBase<StockBatch> batchRepo,
        IStockService stockService)
    {
        _productRepo = productRepo;
        _batchRepo = batchRepo;
        _stockService = stockService;
        LoadBatches().ConfigureAwait(false);
    }

    // Commands  

    [RelayCommand]
    private async Task LoadProducts()
    {
        IsBusy = true;
        try
        {
            var list = await _productRepo.ListAsync(new ProductsWithBatchesSpec());
            Products.Clear();
            foreach (var p in list)
                Products.Add(new ProductSummary(p));
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task LoadBatches()
    {
        IsBusy = true;
        try
        {
            var batches = await _batchRepo.ListAsync(new AllStockBatchesSpec());

            Batches.Clear();
            foreach (var b in batches)
                Batches.Add(new StockBatchRowViewModel(b));

            RefreshStats();
        }
        finally { IsBusy = false; }
    }

    // Receive stock 

    [RelayCommand]
    private void ShowReceiveForm()
    {
        LoadProducts().ConfigureAwait(false);
        _receiveQuantity = 1;
        _receiveUnitCost = 0;
        _receiveSellPrice = _selectedProduct?.RetailPrice ?? 0;
        _receiveCostingMethod = _selectedProduct?.CostingMethod ?? CostingMethod.Fifo;
        _receiveReference = string.Empty;
        ReceiveStockRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ConfirmReceiveStock()
    {
        if (_selectedProduct is null || _receiveQuantity <= 0 || _receiveUnitCost <= 0)
            return;

        // Persist the new stock batch.
        await _stockService.ReceiveStockAsync(
            productId: _selectedProduct.Id,
            quantityReceived: _receiveQuantity,
            unitCost: _receiveUnitCost,
            receivedAt: DateTime.UtcNow,
            reference: string.IsNullOrWhiteSpace(_receiveReference) ? null : _receiveReference);

        if (_receiveCostingMethod == CostingMethod.WeightedAverage)
        {
            _receiveSellPrice = await _stockService.GetWeightedAverageCostAsync(_selectedProduct.Id);
        }

        // Update the product's retail price and costing method so the POS reflects the change.
        var product = await _productRepo.GetByIdAsync(_selectedProduct.Id);
        if (product is not null)
        {
            product.RetailPrice = Math.Round(_receiveSellPrice, 0, MidpointRounding.AwayFromZero);
            product.CostingMethod = _receiveCostingMethod;
            await _productRepo.UpdateAsync(product);
        }

        // Refresh the Products list so ProductSummary reflects the updated values.
        await LoadProducts();

        ReceiveStockCompleted?.Invoke(this, EventArgs.Empty);
        await LoadBatches();
    }
    // Helpers  

    private void RefreshStats()
    {
        OnPropertyChanged(nameof(TotalUnits));
        OnPropertyChanged(nameof(TotalStockValue));
        OnPropertyChanged(nameof(WeightedAverageCost));
    }
}

// ?? Supporting view-models ????????????????????????????????????????????????

public class ProductSummary(Product p)
{
    public int Id { get; } = p.Id;
    public string Name { get; } = p.Name;
    public string Sku { get; } = p.Sku;
    public decimal RetailPrice { get; } = p.RetailPrice;
    public CostingMethod CostingMethod { get; } = p.CostingMethod;
    public override string ToString() => $"{Name}  [{Sku}]";
}

public class StockBatchRowViewModel
{
    public int Id { get; }
    public string ProductName { get; }
    public DateTime ReceivedAt { get; }
    public int QuantityReceived { get; }
    public int QuantityRemaining { get; }
    public int QuantitySold => QuantityReceived - QuantityRemaining;
    public decimal UnitCost { get; }
    public decimal BatchValue => QuantityReceived * UnitCost;
    public decimal RemainingValue => QuantityRemaining * UnitCost;
    public string? Reference { get; }
    public bool IsExhausted => QuantityRemaining == 0;

    public StockBatchRowViewModel(StockBatch b)
    {
        Id = b.Id;
        ProductName = b.Product?.Name ?? "—";
        ReceivedAt = b.ReceivedAt.ToLocalTime();
        QuantityReceived = b.QuantityReceived;
        QuantityRemaining = b.QuantityRemaining;
        UnitCost = b.UnitCost;
        Reference = b.Reference;
    }
}
