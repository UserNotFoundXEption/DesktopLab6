using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WpfApp.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5018/api/")
        };
    }

    public async Task<List<Product>> GetProductsAsync(string? filterByName = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(filterByName))
            query.Add($"filterByName={filterByName}");
        if (!string.IsNullOrWhiteSpace(sortBy))
            query.Add($"sortBy={sortBy}&sortOrder={sortOrder}");

        var url = "Products" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
        return await _httpClient.GetFromJsonAsync<List<Product>>(url) ?? new List<Product>();
    }

    public async Task AddProductAsync(Product product) =>
        await _httpClient.PostAsJsonAsync("Products", product);

    public async Task UpdateProductAsync(string name, Product product) =>
        await _httpClient.PutAsJsonAsync($"Products/{name}", product);

    public async Task DeleteProductAsync(string name) =>
        await _httpClient.DeleteAsync($"Products/{name}");
}
