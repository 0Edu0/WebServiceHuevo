namespace WebServiceHuevo.Models;

public class HuevoStatsDto
{
    public int Total { get; set; }
    public decimal PesoPromedio { get; set; }
    public Dictionary<string, int> PorCategoria { get; set; } = new();
    public Dictionary<string, int> PorColor { get; set; } = new();
}
