using AspNetCoreMultiApp.Web.Client;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMultiApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IApiClient _client;
        public ProductsController(IApiClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _client.GetProductsAsync();
            return View(data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _client.GetProductAsync(id);

            //if (product == null)
            //{
            //    return NotFound();
            //}
            //return View(product);

            return product == null ? NotFound() : View(product);
        }
    }
}
