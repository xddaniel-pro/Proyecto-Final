using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using APP_Proyec.Models;
using System.Net.Http.Headers;

namespace APP_Proyec.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient("Api");

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<IEnumerable<Product>>("api/products");
        }

        public async Task<IEnumerable<Product>> GetTopSellingAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<IEnumerable<Product>>("api/products/top-selling");
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<IEnumerable<Order>>("api/orders");
        }

        public async Task<IEnumerable<Account>> GetPendingAccountsAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<IEnumerable<Account>>("api/accounts/pending");
        }
    }
}
