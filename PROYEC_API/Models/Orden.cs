using System.Collections.Generic;

namespace PROYEC_API.Models
{
    public class Orden
    {
        public int id_usuario { get; set; }
        public decimal total { get; set; }
        public List<Detalle> detalles { get; set; } = new List<Detalle>();
    }

    public class Detalle
    {
        public int id_producto { get; set; }
        public int cantidad { get; set; }
        public decimal subtotal { get; set; }
    }
}   