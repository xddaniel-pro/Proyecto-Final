using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using proyec_pag.Models;

namespace proyec_pag.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel();

            // Llamadas a tu API
            var ordenes = await _httpClient.GetFromJsonAsync<List<OrdenDto>>("/api/ordenes");
            var paquetes = await _httpClient.GetFromJsonAsync<List<PaqueteDto>>("/api/paquetes");
            var productos = await _httpClient.GetFromJsonAsync<List<ProductoDto>>("/api/productos");
            var topVendidos = await _httpClient.GetFromJsonAsync<List<ProductoDto>>("/api/productos/topvendidos");

            // Asignar valores
            dashboard.TotalOrdenes = ordenes?.Count ?? 0;
            dashboard.TotalPaquetes = paquetes?.Count ?? 0;
            dashboard.TotalProductos = productos?.Count ?? 0;
            dashboard.TopVendidosMes = topVendidos?.Count ?? 0;

            return View(dashboard);
        }
    }
}
