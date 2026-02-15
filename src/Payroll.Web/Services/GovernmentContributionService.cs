using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public interface IGovernmentContributionService
{
    Task<List<SssTableDto>> GetSssTableAsync();
    Task<SssTableDto?> CreateSssAsync(SssTableDto entry);
    Task<bool> UpdateSssAsync(int id, SssTableDto entry);
    Task<bool> DeleteSssAsync(int id);

    Task<List<PhilhealthTableDto>> GetPhilhealthTableAsync();
    Task<PhilhealthTableDto?> CreatePhilhealthAsync(PhilhealthTableDto entry);
    Task<bool> UpdatePhilhealthAsync(int id, PhilhealthTableDto entry);
    Task<bool> DeletePhilhealthAsync(int id);

    Task<List<PagibigTableDto>> GetPagibigTableAsync();
    Task<PagibigTableDto?> CreatePagibigAsync(PagibigTableDto entry);
    Task<bool> UpdatePagibigAsync(int id, PagibigTableDto entry);
    Task<bool> DeletePagibigAsync(int id);

    Task<List<TaxTableDto>> GetTaxTableAsync();
    Task<TaxTableDto?> CreateTaxAsync(TaxTableDto entry);
    Task<bool> UpdateTaxAsync(int id, TaxTableDto entry);
    Task<bool> DeleteTaxAsync(int id);
}

public class GovernmentContributionService : IGovernmentContributionService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public GovernmentContributionService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    // SSS
    public async Task<List<SssTableDto>> GetSssTableAsync()
    {
        await SetAuthHeaderAsync();
        try { return await _httpClient.GetFromJsonAsync<List<SssTableDto>>("api/government/sss") ?? new(); }
        catch { return new(); }
    }
    public async Task<SssTableDto?> CreateSssAsync(SssTableDto entry)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PostAsJsonAsync("api/government/sss", entry);
        return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<SssTableDto>() : null;
    }
    public async Task<bool> UpdateSssAsync(int id, SssTableDto entry)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PutAsJsonAsync($"api/government/sss/{id}", entry);
        return r.IsSuccessStatusCode;
    }
    public async Task<bool> DeleteSssAsync(int id)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.DeleteAsync($"api/government/sss/{id}");
        return r.IsSuccessStatusCode;
    }

    // PhilHealth
    public async Task<List<PhilhealthTableDto>> GetPhilhealthTableAsync()
    {
        await SetAuthHeaderAsync();
        try { return await _httpClient.GetFromJsonAsync<List<PhilhealthTableDto>>("api/government/philhealth") ?? new(); }
        catch { return new(); }
    }
    public async Task<PhilhealthTableDto?> CreatePhilhealthAsync(PhilhealthTableDto entry)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PostAsJsonAsync("api/government/philhealth", entry);
        return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<PhilhealthTableDto>() : null;
    }
    public async Task<bool> UpdatePhilhealthAsync(int id, PhilhealthTableDto entry)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PutAsJsonAsync($"api/government/philhealth/{id}", entry);
        return r.IsSuccessStatusCode;
    }
    public async Task<bool> DeletePhilhealthAsync(int id)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.DeleteAsync($"api/government/philhealth/{id}");
        return r.IsSuccessStatusCode;
    }

    // Pag-IBIG
    public async Task<List<PagibigTableDto>> GetPagibigTableAsync()
    {
        await SetAuthHeaderAsync();
        try { return await _httpClient.GetFromJsonAsync<List<PagibigTableDto>>("api/government/pagibig") ?? new(); }
        catch { return new(); }
    }
    public async Task<PagibigTableDto?> CreatePagibigAsync(PagibigTableDto entry)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PostAsJsonAsync("api/government/pagibig", entry);
        return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<PagibigTableDto>() : null;
    }
    public async Task<bool> UpdatePagibigAsync(int id, PagibigTableDto entry)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PutAsJsonAsync($"api/government/pagibig/{id}", entry);
        return r.IsSuccessStatusCode;
    }
    public async Task<bool> DeletePagibigAsync(int id)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.DeleteAsync($"api/government/pagibig/{id}");
        return r.IsSuccessStatusCode;
    }

    // Tax
    public async Task<List<TaxTableDto>> GetTaxTableAsync()
    {
        await SetAuthHeaderAsync();
        try { return await _httpClient.GetFromJsonAsync<List<TaxTableDto>>("api/government/tax") ?? new(); }
        catch { return new(); }
    }
    public async Task<TaxTableDto?> CreateTaxAsync(TaxTableDto entry)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PostAsJsonAsync("api/government/tax", entry);
        return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<TaxTableDto>() : null;
    }
    public async Task<bool> UpdateTaxAsync(int id, TaxTableDto entry)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PutAsJsonAsync($"api/government/tax/{id}", entry);
        return r.IsSuccessStatusCode;
    }
    public async Task<bool> DeleteTaxAsync(int id)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.DeleteAsync($"api/government/tax/{id}");
        return r.IsSuccessStatusCode;
    }
}
