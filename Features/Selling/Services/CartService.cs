using System.Collections.ObjectModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickPOS.Models.Entities;

namespace QuickPOS.Features.Selling;

public partial class CartService : ObservableObject
{
    public ObservableCollection<CartItemViewModel> Items { get; } = [];

    [ObservableProperty]
    private decimal _totalAmount;

    [RelayCommand]
    public void AddItem(Product product)
    {
        var existing = Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing is not null)
        {
            existing.Quantity++;
        }
        else
        {
            Items.Add(new CartItemViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.RetailPrice,
                Quantity = 1,
                ImagePath = product.ImagePath
            });
        }
        RecalculateTotal();
    }

    [RelayCommand]
    public void RemoveItem(CartItemViewModel item)
    {
        Items.Remove(item);
        RecalculateTotal();
    }

    [RelayCommand]
    public void IncrementQuantity(CartItemViewModel item)
    {
        item.Quantity++;
        RecalculateTotal();
    }

    [RelayCommand]
    public void DecrementQuantity(CartItemViewModel item)
    {
        if (item.Quantity > 1)
            item.Quantity--;
        else
            Items.Remove(item);
        RecalculateTotal();
    }

    [RelayCommand]
    public void ClearCart()
    {
        Items.Clear();
        TotalAmount = 0;
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.Total);
    }
}

public partial class CartItemViewModel : ObservableObject
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public decimal UnitPrice { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Total))]
    private int _quantity;

    public decimal Total => _quantity * UnitPrice;
}
