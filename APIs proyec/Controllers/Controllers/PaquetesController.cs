using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class PaquetesController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PaquetesController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public IActionResult CrearPaquete([FromBody] Paquete paquete)
    {
        if (paquete == null || string.IsNullOrWhiteSpace(paquete.nombre_paquete))
            return BadRequest(new { mensaje = "Datos inválidos" });

        string connectionString = _configuration.GetConnectionString("OracleConnection");
        int nuevoId = 0;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string insert = @"INSERT INTO paquetes (id_paquete, nombre_paquete, descripcion, precio_paquete, fecha_caducidad)
                              VALUES (seq_paquete.NEXTVAL, :nombre, :descripcion, :precio, :fecha)
                              RETURNING id_paquete INTO :id_paquete";

            using (OracleCommand cmd = new OracleCommand(insert, conn))
            {
                cmd.Parameters.Add(new OracleParameter("nombre", paquete.nombre_paquete));
                cmd.Parameters.Add(new OracleParameter("descripcion", paquete.descripcion));
                cmd.Parameters.Add(new OracleParameter("precio", paquete.precio_paquete));
                cmd.Parameters.Add(new OracleParameter("fecha", paquete.fecha_caducidad));
                OracleParameter outId = new OracleParameter("id_paquete", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, ParameterDirection.Output);
                cmd.Parameters.Add(outId);

                try
                {
                    cmd.ExecuteNonQuery();
                    nuevoId = Convert.ToInt32(outId.Value.ToString());
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { mensaje = "Error al insertar paquete", detalle = ex.Message });
                }
            }
            conn.Close();
        }

        paquete.id_paquete = nuevoId;
        return CreatedAtAction(nameof(GetPaquetes), new { id = nuevoId }, paquete);
    }

    [HttpGet]
    public IActionResult GetPaquetes()
    {
        List<Paquete> lista = new List<Paquete>();
        string connectionString = _configuration.GetConnectionString("OracleConnection");

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT * FROM paquetes";
            OracleCommand cmd = new OracleCommand(query, conn);
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Paquete
                {
                    id_paquete = Convert.ToInt32(reader["id_paquete"]),
                    nombre_paquete = reader["nombre_paquete"].ToString(),
                    descripcion = reader["descripcion"].ToString(),
                    precio_paquete = Convert.ToDecimal(reader["precio_paquete"]),
                    fecha_caducidad = Convert.ToDateTime(reader["fecha_caducidad"])
                });
            }
            conn.Close();
        }
        return Ok(lista);
    }

    [HttpGet("vencer")]
    public IActionResult PaquetesPorVencer()
    {
        List<Paquete> lista = new List<Paquete>();
        string connectionString = _configuration.GetConnectionString("OracleConnection");

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = @"
                SELECT id_paquete, nombre_paquete, descripcion, precio_paquete, fecha_caducidad
                FROM paquetes
                WHERE fecha_caducidad <= SYSDATE + 15
                ORDER BY fecha_caducidad ASC";

            OracleCommand cmd = new OracleCommand(query, conn);
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Paquete
                {
                    id_paquete = Convert.ToInt32(reader["id_paquete"]),
                    nombre_paquete = reader["nombre_paquete"].ToString(),
                    descripcion = reader["descripcion"].ToString(),
                    precio_paquete = Convert.ToDecimal(reader["precio_paquete"]),
                    fecha_caducidad = Convert.ToDateTime(reader["fecha_caducidad"])
                });
            }
            conn.Close();
        }
        return Ok(lista);
    }

    [HttpGet("ejecutar-vencimiento")]
    public IActionResult EjecutarProcedimiento()
    {
        List<Paquete> lista = new List<Paquete>();
        string connectionString = _configuration.GetConnectionString("OracleConnection");

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            OracleCommand cmd = new OracleCommand("paquetes_vencer", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            OracleParameter cursor = new OracleParameter();
            cursor.OracleDbType = OracleDbType.RefCursor;
            cursor.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(cursor);

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Paquete
                {
                    id_paquete = Convert.ToInt32(reader["id_paquete"]),
                    nombre_paquete = reader["nombre_paquete"].ToString(),
                    descripcion = reader["descripcion"].ToString(),
                    precio_paquete = Convert.ToDecimal(reader["precio_paquete"]),
                    fecha_caducidad = Convert.ToDateTime(reader["fecha_caducidad"])
                });
            }
            conn.Close();
        }

        return Ok(lista);
    }
}
