using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class OrdenesController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public OrdenesController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // =========================================
    // CREAR ORDEN (Cliente)
    // =========================================
    [HttpPost]
    public IActionResult CrearOrden([FromBody] Orden orden)
    {
        string connectionString = _configuration.GetConnectionString("OracleConnection");

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();

            string queryFactura = @"
                INSERT INTO factura (id_factura, id_usuario, fecha_factura, total, estado)
                VALUES (seq_factura.NEXTVAL, :usuario, SYSDATE, :total, 'Pendiente')
                RETURNING id_factura INTO :id_factura";

            using (OracleCommand cmdFactura = new OracleCommand(queryFactura, conn))
            {
                cmdFactura.Parameters.Add(new OracleParameter("usuario", orden.id_usuario));
                cmdFactura.Parameters.Add(new OracleParameter("total", orden.total));
                OracleParameter idFacturaParam = new OracleParameter("id_factura", OracleDbType.Int32, ParameterDirection.Output);
                cmdFactura.Parameters.Add(idFacturaParam);
                cmdFactura.ExecuteNonQuery();

                int idFactura = Convert.ToInt32(idFacturaParam.Value);

                foreach (var detalle in orden.detalles)
                {
                    string queryDetalle = @"
                        INSERT INTO factura_detalle (id_detalle, id_factura, id_producto, cantidad, subtotal)
                        VALUES (seq_detalle.NEXTVAL, :factura, :producto, :cantidad, :subtotal)";

                    using (OracleCommand cmdDetalle = new OracleCommand(queryDetalle, conn))
                    {
                        cmdDetalle.Parameters.Add(new OracleParameter("factura", idFactura));
                        cmdDetalle.Parameters.Add(new OracleParameter("producto", detalle.id_producto));
                        cmdDetalle.Parameters.Add(new OracleParameter("cantidad", detalle.cantidad));
                        cmdDetalle.Parameters.Add(new OracleParameter("subtotal", detalle.subtotal));
                        cmdDetalle.ExecuteNonQuery();
                    }
                }
            }
        }

        return Ok(new { mensaje = "Orden registrada correctamente" });
    }

    // =========================================
    // LISTAR ORDENES (Empleados)
    // =========================================
    [HttpGet]
    public IActionResult GetOrdenes()
    {
        var lista = new List<object>();
        string connectionString = _configuration.GetConnectionString("OracleConnection");

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = @"
                SELECT f.id_factura, f.id_usuario, f.fecha_factura, f.total,
                       NVL(f.estado, 'Pendiente') AS estado,
                       NVL(u.nombre, 'Sin cliente') AS cliente,
                       NVL(u.telefono, 'N/A') AS telefono,
                       NVL(u.correo, 'N/A') AS correo
                FROM factura f
                LEFT JOIN usuarios u ON f.id_usuario = u.id_usuario
                ORDER BY f.fecha_factura DESC";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new
                        {
                            id_factura = reader["id_factura"],
                            id_usuario = reader["id_usuario"],
                            fecha_factura = reader["fecha_factura"],
                            total = reader["total"],
                            estado = reader["estado"],
                            cliente = reader["cliente"],
                            telefono = reader["telefono"],
                            correo = reader["correo"]
                        });
                    }
                }
            }
        }

        return Ok(lista);
    }

    // =========================================
    // VER ORDEN POR ID (Cliente)
    // =========================================
    [HttpGet("{id}")]
    public IActionResult GetOrdenPorId(int id)
    {
        object orden = null;
        string connectionString = _configuration.GetConnectionString("OracleConnection");

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = @"
                SELECT f.id_factura, f.id_usuario, f.fecha_factura, f.total,
                       NVL(f.estado, 'Pendiente') AS estado
                FROM factura f
                WHERE f.id_factura = :id";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new OracleParameter("id", id));

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        orden = new
                        {
                            id_factura = reader["id_factura"],
                            id_usuario = reader["id_usuario"],
                            fecha_factura = reader["fecha_factura"],
                            total = reader["total"],
                            estado = reader["estado"]
                        };
                    }
                }
            }
        }

        if (orden == null)
            return NotFound(new { mensaje = "Orden no encontrada" });

        return Ok(orden);
    }

    // =========================================
    // ACTUALIZAR ESTADO DE ORDEN (Empleados)
    // =========================================
    [HttpPut("{id}")]
    public IActionResult ActualizarOrden(int id, [FromBody] string estado)
    {
        string connectionString = _configuration.GetConnectionString("OracleConnection");

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "UPDATE factura SET estado = :estado WHERE id_factura = :id";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(new OracleParameter("estado", estado));
                cmd.Parameters.Add(new OracleParameter("id", id));
                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    return Ok(new { mensaje = "Orden actualizada correctamente" });
                else
                    return NotFound(new { mensaje = "Orden no encontrada" });
            }
        }
    }
}
