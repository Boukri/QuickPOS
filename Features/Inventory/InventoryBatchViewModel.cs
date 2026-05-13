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
    private ProductSummary? _selectedProduct;


    //  Batch list  
    public ObservableCollection<StockBatchRowViewModel> Batches { get; } = [];

    //Dialog events
    public event EventHandler? ReceiveStockRequested;
    public event EventHandler? ReceiveStockCompleted;

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
        }
        finally { IsBusy = false; }
    }

    // Receive stock 

    [RelayCommand]
    private void ShowReceiveForm()
    {
        LoadProducts().ConfigureAwait(false);
        ReceiveQuantity = 1;
        ReceiveUnitCost = 0;
        ReceiveSellPrice = SelectedProduct?.ActualPrice ?? 0;
        ReceiveCostingMethod = SelectedProduct?.CostingMethod ?? CostingMethod.Fifo;
        ReceiveReference = string.Empty;
        ReceiveStockRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ConfirmReceiveStock()
    {
        if (SelectedProduct is null || ReceiveQuantity <= 0 || ReceiveUnitCost <= 0)
            return;

        // Persist the new stock batch.
        await _stockService.ReceiveStockAsync(
            productId: SelectedProduct.Id,
            quantityReceived: ReceiveQuantity,
            unitCost: ReceiveUnitCost,
            receivedAt: DateTime.UtcNow,
            reference: string.IsNullOrWhiteSpace(ReceiveReference) ? null : ReceiveReference);

        if (ReceiveCostingMethod == CostingMethod.WeightedAverage)
        {
            ReceiveSellPrice = await _stockService.GetWeightedAverageCostAsync(SelectedProduct.Id);
        }

        // Update the product's retail price and costing method so the POS reflects the change.
        var product = await _productRepo.GetByIdAsync(SelectedProduct.Id);
        if (product is not null)
        {
            product.ActualPrice = Math.Round(ReceiveSellPrice, 0, MidpointRounding.AwayFromZero);
            product.CostingMethod = ReceiveCostingMethod;
            await _productRepo.UpdateAsync(product);
        }

        // Refresh the Products list so ProductSummary reflects the updated values.
        await LoadProducts();

        ReceiveStockCompleted?.Invoke(this, EventArgs.Empty);
        await LoadBatches();
    }
    }

// ?? Supporting view-models ????????????????????????????????????????????????

public class ProductSummary(Product p)
{
    public int Id { get; } = p.Id;
    public string Name { get; } = p.Name; 
    public decimal ActualPrice { get; } = p.ActualPrice;
    public CostingMethod CostingMethod { get; } = p.CostingMethod;
    public override string ToString() => $"{Name}  [{ActualPrice}]";
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
