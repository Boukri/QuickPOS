using Ardalis.Specification;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickPOS.Core;
using QuickPOS.Features.Inventory;
using QuickPOS.Models.Entities;
using QuickPOS.Models.Enums;
using System.Collections.ObjectModel;

namespace QuickPOS.Features.Selling;

public partial class SellingViewModel : ViewModelBase
{
    private readonly IRepositoryBase<Product> _productRepo;
    private readonly IRepositoryBase<Transaction> _transactionRepo;
    private readonly IRepositoryBase<CategoryModel> _categoryRepo;
    private readonly CartService _cartService;
    private readonly UserSettingsService _settings;
    private readonly IStockService _stockService;
    public LocalizationService Loc => LocalizationService.Instance;
    public CartService Cart => _cartService;

    public ObservableCollection<SellingProductViewModel> Products { get; } = [];
    public ObservableCollection<CategoryFilterItem> Categories { get; } = [];

    [ObservableProperty]
    private CategoryFilterItem? _selectedCategory;

    [ObservableProperty]
    private double _cartPanelWidth;

    partial void OnCartPanelWidthChanged(double value)
    {
        _settings.Settings.CartPanelWidth = value;
        _settings.Save();
    }

    public SellingViewModel(IRepositoryBase<Product> productRepo, IRepositoryBase<Transaction> transactionRepo, IRepositoryBase<CategoryModel> categoryRepo, CartService cartService, UserSettingsService settings, IStockService stockService)
    {
        _productRepo = productRepo;
        _transactionRepo = transactionRepo;
        _categoryRepo = categoryRepo;
        _cartService = cartService;
        _settings = settings;
        _stockService = stockService;
        _cartPanelWidth = settings.Settings.CartPanelWidth;
        LoadCategoriesCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadCategories()
    {
        var cats = await _categoryRepo.ListAsync(new Data.Specifications.AllCategoriesSpec());
        Categories.Clear();
        Categories.Add(new CategoryFilterItem(null, "All"));
        foreach (var c in cats)
            Categories.Add(new CategoryFilterItem(c.Id, c.Name));
        SelectedCategory = Categories[0];
    }

    [RelayCommand]
    private async Task LoadProducts()
    {
        IsBusy = true;
        try
        {
            var spec = new Data.Specifications.ProductsForSellingSpec(
                SelectedCategory?.CategoryId);
            var products = await _productRepo.ListAsync(spec);
            Products.Clear();
            foreach (var p in products)
                Products.Add(new SellingProductViewModel(p));
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void AddToCart(SellingProductViewModel vm)
    {
        _cartService.AddItem(vm.Product);
    }

    [RelayCommand]
    private async Task Checkout()
    {
        if (_cartService.Items.Count == 0) return;

        var dialogVm = new PaymentDialogViewModel(_cartService.TotalAmount);
        var dialog = new PaymentDialog(dialogVm)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };

        if (dialog.ShowDialog() != true) return;

        // Load the products so we know their costing method.
        var productIds = _cartService.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepo.ListAsync(new Data.Specifications.ProductsByIdsSpec(productIds));
        var productMap = products.ToDictionary(p => p.Id);

        var items = new List<TransactionItem>();
        foreach (var i in _cartService.Items)
        {
            decimal cogs = 0m;
            if (productMap.TryGetValue(i.ProductId, out var product) && !product.IsService)
            {
                var result = await _stockService.ProcessSaleAsync(
                    i.ProductId, i.Quantity, product.CostingMethod);
                cogs = result?.Cogs ?? 0m;
            }

            items.Add(new TransactionItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Cogs = cogs
            });

            var p = Products.FirstOrDefault(p => p.Id == i.ProductId);
            if(p is not null)
            {
                p.UnitsAvailable = p.UnitsAvailable - i.Quantity;
            }
           
        }

        var transaction = new Transaction
        {
            TotalAmount = _cartService.TotalAmount,
            Items = items
        };

        await _transactionRepo.AddAsync(transaction);

        _cartService.ClearCart();
    }

    partial void OnSelectedCategoryChanged(CategoryFilterItem? value)
    {
        LoadProductsCommand.ExecuteAsync(null);
    }
}

public record CategoryFilterItem(int? CategoryId, string Name)
{
    public override string ToString() => Name;
}

/// <summary>
/// Wraps a Product for the POS view, enriching it with live stock data
/// and a costing-method-aware unit cost so the card can show alerts.
/// </summary>
public class SellingProductViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    private const int LowStockThreshold = 5;

    public Product Product { get; }

    // Pass-through for existing XAML bindings
    public int Id               => Product.Id;
    public string Name          => Product.Name;
    public string? ImagePath    => Product.ImagePath;
    public decimal RetailPrice  => Product.RetailPrice;
    public bool IsService       => Product.IsService;
    public CategoryModel Category   => Product.Category;
    public CostingMethod CostingMethod => Product.CostingMethod;

    // Observable stock — changing this updates badge + overlay automatically.
    private int _unitsAvailable;
    public int UnitsAvailable
    {
        get => _unitsAvailable;
        set
        {
            if (SetProperty(ref _unitsAvailable, value))
            {
                OnPropertyChanged(nameof(IsOutOfStock));
                OnPropertyChanged(nameof(IsLowStock));
                OnPropertyChanged(nameof(CanSell));
            }
        }
    }

    public bool CanSell      => IsService || UnitsAvailable > 0;
    public bool IsOutOfStock => !IsService && UnitsAvailable == 0;
    public bool IsLowStock   => !IsService && UnitsAvailable > 0 && UnitsAvailable <= LowStockThreshold;

    // Current unit cost based on the active costing method
    public decimal CurrentUnitCost { get; }

    public SellingProductViewModel(Product p)
    {
        Product = p;

        var active = p.Batches.Where(b => b.QuantityRemaining > 0).ToList();
        _unitsAvailable = active.Sum(b => b.QuantityRemaining);

        CurrentUnitCost = p.CostingMethod switch
        {
            CostingMethod.Fifo => active
                .OrderBy(b => b.ReceivedAt)
                .FirstOrDefault()?.UnitCost ?? 0m,
            CostingMethod.Lifo => active
                .OrderByDescending(b => b.ReceivedAt)
                .FirstOrDefault()?.UnitCost ?? 0m,
            CostingMethod.WeightedAverage when _unitsAvailable > 0 =>
                Math.Round(active.Sum(b => b.QuantityRemaining * b.UnitCost) / _unitsAvailable, 4),
            _ => 0m
        };
    }
}
