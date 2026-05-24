using System.ComponentModel.DataAnnotations;

namespace WebServiceHuevo.Models;

public class HuevoUpdateDto
{
    [Range(0, 9999.99)]
    public decimal? Peso { get; set; }

    [Required]
    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Color { get; set; } = string.Empty;
}
