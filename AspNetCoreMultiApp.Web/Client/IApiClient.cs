using AspNetCoreMultiApp.Web.DTO;

namespace AspNetCoreMultiApp.Web.Client
{
    public interface IApiClient
    {
        Task<List<ProductDTO>> GetProductsAsync();
        Task<ProductDTO?> GetProductAsync(int productId);

    }
}
