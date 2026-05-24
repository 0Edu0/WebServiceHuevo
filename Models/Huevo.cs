using System.ComponentModel.DataAnnotations;

namespace WebServiceHuevo.Models;

public class Huevo
{
    public int Id { get; set; }

    public decimal? Peso { get; set; }

    [Required]
    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Color { get; set; } = string.Empty;

    public DateTime FechaIngreso { get; set; }
}
