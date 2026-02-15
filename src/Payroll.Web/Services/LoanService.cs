using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public interface ILoanService
{
    Task<List<LoanTypeDto>> GetLoanTypesAsync();
    Task<LoanTypeDto?> CreateLoanTypeAsync(CreateLoanTypeDto dto);
    Task<bool> UpdateLoanTypeAsync(int id, CreateLoanTypeDto dto);
    Task<List<EmployeeLoanDto>> GetLoansAsync(Guid? employeeId = null);
    Task<EmployeeLoanDto?> CreateLoanAsync(CreateLoanDto dto);
    Task<bool> CancelLoanAsync(int id);
    Task<List<LoanPaymentDto>> GetPaymentsAsync(int loanId);
}

public class LoanService : ILoanService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public LoanService(HttpClient httpClient, ILocalStorageService localStorage)
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

    public async Task<List<LoanTypeDto>> GetLoanTypesAsync()
    {
        await SetAuthHeaderAsync();
        try { return await _httpClient.GetFromJsonAsync<List<LoanTypeDto>>("api/loans/types") ?? new(); }
        catch { return new(); }
    }

    public async Task<LoanTypeDto?> CreateLoanTypeAsync(CreateLoanTypeDto dto)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PostAsJsonAsync("api/loans/types", dto);
        return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<LoanTypeDto>() : null;
    }

    public async Task<bool> UpdateLoanTypeAsync(int id, CreateLoanTypeDto dto)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PutAsJsonAsync($"api/loans/types/{id}", dto);
        return r.IsSuccessStatusCode;
    }

    public async Task<List<EmployeeLoanDto>> GetLoansAsync(Guid? employeeId = null)
    {
        await SetAuthHeaderAsync();
        var url = employeeId.HasValue ? $"api/loans?employeeId={employeeId}" : "api/loans";
        try { return await _httpClient.GetFromJsonAsync<List<EmployeeLoanDto>>(url) ?? new(); }
        catch { return new(); }
    }

    public async Task<EmployeeLoanDto?> CreateLoanAsync(CreateLoanDto dto)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PostAsJsonAsync("api/loans", dto);
        return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<EmployeeLoanDto>() : null;
    }

    public async Task<bool> CancelLoanAsync(int id)
    {
        await SetAuthHeaderAsync();
        var r = await _httpClient.PutAsync($"api/loans/{id}/cancel", null);
        return r.IsSuccessStatusCode;
    }

    public async Task<List<LoanPaymentDto>> GetPaymentsAsync(int loanId)
    {
        await SetAuthHeaderAsync();
        try { return await _httpClient.GetFromJsonAsync<List<LoanPaymentDto>>($"api/loans/{loanId}/payments") ?? new(); }
        catch { return new(); }
    }
}
