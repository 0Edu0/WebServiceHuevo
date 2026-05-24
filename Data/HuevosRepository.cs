using MySqlConnector;
using WebServiceHuevo.Models;

namespace WebServiceHuevo.Data;

public class HuevosRepository : IHuevosRepository
{
    private readonly string _connectionString;

    public HuevosRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Falta la cadena de conexion 'MySql' en appsettings.json");
    }

    public async Task<IEnumerable<Huevo>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"SELECT Id, Peso, Categoria, Color, FechaIngreso
                             FROM Huevos
                             ORDER BY Id DESC;";

        var lista = new List<Huevo>();
        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            lista.Add(Map(reader));
        }
        return lista;
    }

    public async Task<Huevo?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        const string sql = @"SELECT Id, Peso, Categoria, Color, FechaIngreso
                             FROM Huevos
                             WHERE Id = @Id;";

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            return Map(reader);
        }
        return null;
    }

    public async Task<Huevo> CreateAsync(HuevoCreateDto dto, CancellationToken ct = default)
    {
        const string sql = @"INSERT INTO Huevos (Peso, Categoria, Color)
                             VALUES (@Peso, @Categoria, @Color);
                             SELECT LAST_INSERT_ID();";

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Peso", (object?)dto.Peso ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Categoria", dto.Categoria);
        cmd.Parameters.AddWithValue("@Color", dto.Color);

        var nuevoId = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        return (await GetByIdAsync(nuevoId, ct))!;
    }

    public async Task<bool> UpdateAsync(int id, HuevoUpdateDto dto, CancellationToken ct = default)
    {
        const string sql = @"UPDATE Huevos
                             SET Peso = @Peso,
                                 Categoria = @Categoria,
                                 Color = @Color
                             WHERE Id = @Id;";

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Peso", (object?)dto.Peso ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Categoria", dto.Categoria);
        cmd.Parameters.AddWithValue("@Color", dto.Color);

        var filas = await cmd.ExecuteNonQueryAsync(ct);
        return filas > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        const string sql = @"DELETE FROM Huevos WHERE Id = @Id;";

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        var filas = await cmd.ExecuteNonQueryAsync(ct);
        return filas > 0;
    }

    public async Task<HuevoStatsDto> GetStatsAsync(CancellationToken ct = default)
    {
        const string sqlResumen = @"SELECT COUNT(*) AS Total,
                                           COALESCE(AVG(Peso), 0) AS PesoPromedio
                                    FROM Huevos;";
        const string sqlCategoria = @"SELECT Categoria AS Clave, COUNT(*) AS Cantidad
                                      FROM Huevos GROUP BY Categoria;";
        const string sqlColor = @"SELECT Color AS Clave, COUNT(*) AS Cantidad
                                  FROM Huevos GROUP BY Color;";

        var stats = new HuevoStatsDto();

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        await using (var cmd = new MySqlCommand(sqlResumen, conn))
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            if (await reader.ReadAsync(ct))
            {
                stats.Total = reader.GetInt32("Total");
                stats.PesoPromedio = Math.Round(reader.GetDecimal("PesoPromedio"), 2);
            }
        }

        stats.PorCategoria = await ReadGroupAsync(conn, sqlCategoria, ct);
        stats.PorColor = await ReadGroupAsync(conn, sqlColor, ct);

        return stats;
    }

    private static async Task<Dictionary<string, int>> ReadGroupAsync(MySqlConnection conn, string sql, CancellationToken ct)
    {
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var clave = reader.IsDBNull(0) ? "(sin dato)" : reader.GetString(0);
            var cantidad = reader.GetInt32(1);
            dict[clave] = cantidad;
        }
        return dict;
    }

    private static Huevo Map(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("Id"),
        Peso = reader.IsDBNull(reader.GetOrdinal("Peso")) ? null : reader.GetDecimal("Peso"),
        Categoria = reader.GetString("Categoria"),
        Color = reader.GetString("Color"),
        FechaIngreso = reader.GetDateTime("FechaIngreso"),
    };
}
