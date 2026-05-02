using System.Collections.ObjectModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ardalis.Specification;
using QuickPOS.Core;
using QuickPOS.Models.Entities;
using QuickPOS.Models.Enums;

namespace QuickPOS.Features.Products;

public partial class ProductsViewModel : ViewModelBase
{
    private readonly IRepositoryBase<Product> _productRepo;
    private readonly IRepositoryBase<CategoryModel> _categoryRepo;

    // Tab: 0 = Products, 1 = Categories
    [ObservableProperty]
    private int _activeTab;

    // --- Products ---
    public ObservableCollection<ProductRowViewModel> Products { get; } = [];
    public ObservableCollection<CategoryModel> ProductCategories { get; } = [];

    [ObservableProperty]
    private bool _isFormVisible;

    [ObservableProperty]
    private bool _isDeleteConfirmVisible;

    [ObservableProperty]
    private string _formName = string.Empty;
    [ObservableProperty]
    private CategoryModel? _formCategory;
    [ObservableProperty]
    private string _formSku = string.Empty;
    [ObservableProperty]
    private decimal _formWholesalePrice;
    [ObservableProperty]
    private decimal _formRetailPrice;
    [ObservableProperty]
    private bool _formIsService;

    [ObservableProperty]
    private CostingMethod _formCostingMethod = CostingMethod.Fifo;

    [ObservableProperty]
    private string? _formImagePath;

    private int? _editingProductId;
    private ProductRowViewModel? _pendingDeleteProduct;

    // --- Categories ---
    public ObservableCollection<CategoryRowViewModel> CategoryRows { get; } = [];

    [ObservableProperty]
    private bool _isCategoryFormVisible;

    [ObservableProperty]
    private bool _isCategoryDeleteConfirmVisible;

    [ObservableProperty]
    private string _catFormName = string.Empty;

    [ObservableProperty]
    private bool _catFormIsService;

    private int? _editingCategoryId;
    private CategoryRowViewModel? _pendingDeleteCategory;

    public ProductsViewModel(IRepositoryBase<Product> productRepo, IRepositoryBase<CategoryModel> categoryRepo)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
        LoadDataCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private void SwitchToProductsTab() => ActiveTab = 0;

    [RelayCommand]
    private void SwitchToCategoriesTab() => ActiveTab = 1;

    [RelayCommand]
    private async Task LoadData()
    {
        await LoadCategories();
        await LoadProducts();
        await LoadCategoryRows();
    }

    private async Task LoadCategories()
    {
        var cats = await _categoryRepo.ListAsync(new Data.Specifications.AllCategoriesSpec());
        ProductCategories.Clear();
        foreach (var c in cats)
            ProductCategories.Add(c);
    }

    private async Task LoadCategoryRows()
    {
        var cats = await _categoryRepo.ListAsync(new Data.Specifications.AllCategoriesSpec());
        CategoryRows.Clear();
        foreach (var c in cats)
        {
            var productCount = Products.Count(p => p.CategoryId == c.Id);
            CategoryRows.Add(new CategoryRowViewModel(c, productCount));
        }
    }

    // ===================== PRODUCTS =====================

    [RelayCommand]
    private async Task LoadProducts()
    {
        IsBusy = true;
        try
        {
            var products = await _productRepo.ListAsync(new Data.Specifications.ProductsByCategorySpec());
            Products.Clear();
            foreach (var p in products)
                Products.Add(new ProductRowViewModel(p));
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void ShowAddForm()
    {
        _editingProductId = null;
        FormName = string.Empty;
        FormCategory = ProductCategories.FirstOrDefault(c => !c.IsService);
        FormSku = string.Empty;
        FormWholesalePrice = 0;
        FormRetailPrice = 0;
        FormIsService = false;
        FormCostingMethod = CostingMethod.Fifo;
        FormImagePath = null;
        IsFormVisible = true;
    }

    partial void OnFormCategoryChanged(CategoryModel? value)
    {
        FormIsService = value?.IsService ?? false;
    }

    [RelayCommand]
    private void EditProduct(ProductRowViewModel row)
    {
        _editingProductId = row.Id;
        FormName = row.Name;
        FormCategory = ProductCategories.FirstOrDefault(c => c.Id == row.CategoryId);
        FormSku = row.Sku;
        FormWholesalePrice = row.WholesalePrice;
        FormRetailPrice = row.RetailPrice;
        FormIsService = row.IsService;
        FormCostingMethod = row.CostingMethod;
        FormImagePath = row.ImagePath;
        IsFormVisible = true;
    }

    [RelayCommand]
    private void PickImage()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Select Product Image",
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp",
            Multiselect = false
        };
        if (dlg.ShowDialog() == true)
            FormImagePath = dlg.FileName;
    }

    [RelayCommand]
    private void ClearImage() => FormImagePath = null;

    [RelayCommand]
    private async Task SaveProduct()
    {
        if (_editingProductId.HasValue)
        {
            var product = await _productRepo.GetByIdAsync(_editingProductId.Value);
            if (product is not null)
            {
                product.Name = FormName;
                product.CategoryId = FormCategory?.Id ?? 0;
                product.Sku = FormSku;
                product.WholesalePrice = FormWholesalePrice;
                product.RetailPrice = FormRetailPrice;
                product.IsService = FormIsService;
                product.CostingMethod = FormCostingMethod;
                product.ImagePath = FormImagePath;
                await _productRepo.UpdateAsync(product);
            }
        }
        else
        {
            await _productRepo.AddAsync(new Product
            {
                Name = FormName,
                CategoryId = FormCategory?.Id ?? 0,
                Sku = FormSku,
                WholesalePrice = FormWholesalePrice,
                RetailPrice = FormRetailPrice,
                IsService = FormIsService,
                CostingMethod = FormCostingMethod,
                ImagePath = FormImagePath
            });
        }

        IsFormVisible = false;
        await LoadProducts();
    }

    [RelayCommand]
    private void RequestDeleteProduct(ProductRowViewModel row)
    {
        _pendingDeleteProduct = row;
        IsDeleteConfirmVisible = true;
    }

    [RelayCommand]
    private async Task ConfirmDeleteProduct()
    {
        if (_pendingDeleteProduct is not null)
        {
            var product = await _productRepo.GetByIdAsync(_pendingDeleteProduct.Id);
            if (product is not null)
            {
                await _productRepo.DeleteAsync(product);
                await LoadProducts();
            }
        }
        _pendingDeleteProduct = null;
        _isDeleteConfirmVisible = false;
    }

    [RelayCommand]
    private void CancelDelete()
    {
        _pendingDeleteProduct = null;
        IsDeleteConfirmVisible = false;
    }

    [RelayCommand]
    private void CancelForm() => IsFormVisible = false;

    // ===================== CATEGORIES =====================

    [RelayCommand]
    private void ShowAddCategoryForm()
    {
        _editingCategoryId = null;
        CatFormName = string.Empty;
        CatFormIsService = false;
        IsCategoryFormVisible = true;
    }

    [RelayCommand]
    private void EditCategory(CategoryRowViewModel row)
    {
        _editingCategoryId = row.Id;
        CatFormName = row.Name;
        CatFormIsService = row.IsService;
        IsCategoryFormVisible = true;
    }

    [RelayCommand]
    private async Task SaveCategory()
    {
        if (_editingCategoryId.HasValue)
        {
            var cat = await _categoryRepo.GetByIdAsync(_editingCategoryId.Value);
            if (cat is not null)
            {
                cat.Name = CatFormName;
                cat.IsService = CatFormIsService;
                await _categoryRepo.UpdateAsync(cat);
            }
        }
        else
        {
            await _categoryRepo.AddAsync(new CategoryModel
            {
                Name = CatFormName,
                IsService = CatFormIsService
            });
        }

        IsCategoryFormVisible = false;
        await LoadCategories();
        await LoadCategoryRows();
    }

    [RelayCommand]
    private void CancelCategoryForm() => IsCategoryFormVisible = false;

    [RelayCommand]
    private void RequestDeleteCategory(CategoryRowViewModel row)
    {
        _pendingDeleteCategory = row;
        IsCategoryDeleteConfirmVisible = true;
    }

    [RelayCommand]
    private async Task ConfirmDeleteCategory()
    {
        if (_pendingDeleteCategory is not null)
        {
            var cat = await _categoryRepo.GetByIdAsync(_pendingDeleteCategory.Id);
            if (cat is not null)
            {
                await _categoryRepo.DeleteAsync(cat);
                await LoadCategories();
                await LoadCategoryRows();
            }
        }
        _pendingDeleteCategory = null;
        IsCategoryDeleteConfirmVisible = false;
    }

    [RelayCommand]
    private void CancelCategoryDelete()
    {
        _pendingDeleteCategory = null;
        IsCategoryDeleteConfirmVisible = false;
    }
}

public class ProductRowViewModel
{
    public int Id { get; }
    public string Name { get; }
    public int CategoryId { get; }
    public string CategoryName { get; }
    public string Sku { get; }
    public decimal WholesalePrice { get; }
    public decimal RetailPrice { get; }
    public bool IsService { get; }
    public string? ImagePath { get; }
    public CostingMethod CostingMethod { get; }
    public decimal ProfitMargin => RetailPrice > 0 ? Math.Round((RetailPrice - WholesalePrice) / RetailPrice * 100, 1) : 0;

    public ProductRowViewModel(Product p)
    {
        Id = p.Id;
        Name = p.Name;
        CategoryId = p.CategoryId;
        CategoryName = p.Category?.Name ?? "Unknown";
        Sku = p.Sku;
        WholesalePrice = p.WholesalePrice;
        RetailPrice = p.RetailPrice;
        IsService = p.IsService;
        CostingMethod = p.CostingMethod;
        ImagePath = p.ImagePath;
    }
}

public class CategoryRowViewModel
{
    public int Id { get; }
    public string Name { get; }
    public bool IsService { get; }
    public int ProductCount { get; }

    public CategoryRowViewModel(CategoryModel c, int productCount)
    {
        Id = c.Id;
        Name = c.Name;
        IsService = c.IsService;
        ProductCount = productCount;
    }
}
