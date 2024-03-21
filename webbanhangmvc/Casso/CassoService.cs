using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace webbanhangmvc.Casso
{
    public class CassoService : ICassoService
    {
        private readonly HttpClient _httpClient; 
        private readonly string _apiKey = "AK_CS.04f895d039c711efbf32abb139db7a4d.KNMNldMgqGfa4aZ9rBAN4GwiAqyerZKr5I3GGcqPx9beAyu5xztHK6K1bpPyzle3nXtRebAW"; // Thay bằng API Key thực tế của bạn
        private readonly string _baseUrl = "https://oauth.casso.vn/v2";

        public CassoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Transaction>> GetRecentTransactionsAsync(string fromDate, string toDate)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/transactions?fromDate={fromDate}&toDate={toDate}");
            request.Headers.Add("Authorization", $"Apikey {_apiKey}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactionsResponse = JsonConvert.DeserializeObject<TransactionResponse>(content);

            return transactionsResponse.Data.Records;
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/transactions/{id}");
            request.Headers.Add("Authorization", $"Apikey {_apiKey}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactionResponse = JsonConvert.DeserializeObject<TransactionSingleResponse>(content);

            return transactionResponse.Data;
        }
    }
}
public class TransactionSingleResponse
{
    public int Error { get; set; }
    public string Message { get; set; }
    public Transaction Data { get; set; }
}

public class TransactionResponse
{
    public int Error { get; set; }
    public string Message { get; set; }
    public TransactionData Data { get; set; }
}

public class TransactionData
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int NextPage { get; set; }
    public int PrevPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public List<Transaction> Records { get; set; }
}

public class Transaction
{
    public int Id { get; set; }
    public string Tid { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public decimal CusumBalance { get; set; }
    public DateTime When { get; set; }
}
