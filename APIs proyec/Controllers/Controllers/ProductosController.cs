using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace proyec_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        // Simulación temporal de datos (puedes reemplazar con BD/servicio real)
        private static readonly List<ProductoDto> _productos = new()
        {
            new ProductoDto { Id = 1, Nombre = "Laptop", Precio = 8500, CantidadVendida = 12 },
            new ProductoDto { Id = 2, Nombre = "Mouse", Precio = 150, CantidadVendida = 50 },
            new ProductoDto { Id = 3, Nombre = "Teclado", Precio = 300, CantidadVendida = 30 },
            new ProductoDto { Id = 4, Nombre = "Monitor", Precio = 2200, CantidadVendida = 20 }
        };

        // ✅ GET: api/productos
        [HttpGet]
        public ActionResult<IEnumerable<ProductoDto>> GetAll()
        {
            return Ok(_productos);
        }

        // ✅ GET: api/productos/{id}
        [HttpGet("{id}")]
        public ActionResult<ProductoDto> GetById(int id)
        {
            var producto = _productos.Find(p => p.Id == id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        // ✅ GET: api/productos/topvendidos
        [HttpGet("topvendidos")]
        public ActionResult<IEnumerable<ProductoDto>> GetTopVendidos()
        {
            var top = _productos
                .OrderByDescending(p => p.CantidadVendida)
                .Take(10);
            return Ok(top);
        }

        // ✅ POST: api/productos
        [HttpPost]
        public ActionResult<ProductoDto> Create(ProductoDto nuevo)
        {
            nuevo.Id = _productos.Max(p => p.Id) + 1;
            _productos.Add(nuevo);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        // ✅ PUT: api/productos/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductoDto actualizado)
        {
            var producto = _productos.Find(p => p.Id == id);
            if (producto == null) return NotFound();

            producto.Nombre = actualizado.Nombre;
            producto.Precio = actualizado.Precio;
            producto.CantidadVendida = actualizado.CantidadVendida;

            return NoContent();
        }

        // ✅ DELETE: api/productos/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var producto = _productos.Find(p => p.Id == id);
            if (producto == null) return NotFound();

            _productos.Remove(producto);
            return NoContent();
        }
    }

    // DTO
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int CantidadVendida { get; set; }
    }
}
