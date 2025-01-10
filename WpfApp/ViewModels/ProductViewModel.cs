using GalaSoft.MvvmLight.Command;
using Shared;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfApp.Services;

namespace WpfApp.ViewModels;

public class ProductViewModel : INotifyPropertyChanged
{
    private readonly ProductService _productService;

    public ObservableCollection<Product> Products { get; set; } = new();
    public Product FormProduct { get; set; } = new();
    public bool ShowForm { get; set; } = false;
    public bool IsEditing { get; set; }

    public string? FilterByName { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "asc";
    public string? EditedProductName { get; set; }

    public ICommand LoadProductsCommand { get; }
    public ICommand AddProductCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand EditProductCommand { get; }
    public ICommand DeleteProductCommand { get; }
    public ICommand CancelCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ProductViewModel()
    {
        _productService = new ProductService();
        LoadProductsCommand = new RelayCommand(async () => await LoadProductsAsync());
        AddProductCommand = new RelayCommand(() => ShowAddForm());
        EditProductCommand = new RelayCommand<Product>(ShowEditForm);
        SaveCommand = new RelayCommand<Product>(async product => await SaveAsync(product));
        DeleteProductCommand = new RelayCommand<Product>(async product => await DeleteProductAsync(product));
        CancelCommand = new RelayCommand(() => CancelForm());
        Application.Current.Dispatcher.Invoke(() =>LoadProductsAsync());
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var products = await _productService.GetProductsAsync(FilterByName, SortBy, SortOrder);
            Products.Clear();
            foreach (var product in products)
                Products.Add(product);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading products: {ex.Message}");
        }
    }

    private void ShowAddForm()
    {
        FormProduct = new Product();
        IsEditing = false;
        ShowForm = true;
        OnPropertyChanged(nameof(FormProduct));
        OnPropertyChanged(nameof(ShowForm));
    }

    private void ShowEditForm(Product product)
    {
        FormProduct = new Product
        {
            Name = product.Name,
            Price = product.Price,
            UnitsAvailable = product.UnitsAvailable
        };
        EditedProductName = product.Name;
        IsEditing = true;
        ShowForm = true;
        OnPropertyChanged(nameof(FormProduct));
        OnPropertyChanged(nameof(ShowForm));
    }

    private async Task SaveAsync(Product product)
    {
        if (IsEditing)
        {
            await SaveEditAsync(product);
        }
        else
        {
            await SaveProductAsync();
        }
    }

    private async Task SaveProductAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FormProduct.Name) || FormProduct.Price <= 0 || FormProduct.UnitsAvailable < 0)
                throw new Exception("Invalid data");

            if (Products.Contains(FormProduct))
                await _productService.UpdateProductAsync(FormProduct.Name, FormProduct);
            else
                await _productService.AddProductAsync(FormProduct);

            await LoadProductsAsync();
            CancelForm();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving product: {ex.Message}");
        }
    }

    private async Task SaveEditAsync(Product product)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FormProduct.Name) || FormProduct.Price <= 0 || FormProduct.UnitsAvailable < 0)
                throw new Exception("Invalid data");

            if(EditedProductName != null)
            {
                await _productService.DeleteProductAsync(EditedProductName);
            }
            await _productService.AddProductAsync(FormProduct);
            await LoadProductsAsync();
            CancelForm();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving product: {ex.Message}");
        }
    }

    private async Task DeleteProductAsync(Product product)
    {
        try
        {
            await _productService.DeleteProductAsync(product.Name);
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting product: {ex.Message}");
        }
    }

    private void CancelForm()
    {
        ShowForm = false;
        OnPropertyChanged(nameof(ShowForm));
    }

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
