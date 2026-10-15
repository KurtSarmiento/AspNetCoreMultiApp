using AspNetCoreMultiApp.Web.DTO;

namespace AspNetCoreMultiApp.Web.Client
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        public ApiClient(HttpClient client)
        {
            _httpClient = client;
        }
        public Task<ProductDTO?> GetProductAsync(int productId)
        {
            var res = _httpClient.GetFromJsonAsync<ProductDTO>($"api/products/{productId}");
            return res;
        }

        public async Task<List<ProductDTO>> GetProductsAsync()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductDTO>>("api/products");
            return products ?? new List<ProductDTO>();
            //return products ?? [];
        }
    }
}
