using Microsoft.AspNetCore.Mvc;
using WebServiceHuevo.Data;
using WebServiceHuevo.Models;

namespace WebServiceHuevo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HuevosController : ControllerBase
{
    private readonly IHuevosRepository _repo;
    private readonly ILogger<HuevosController> _logger;

    public HuevosController(IHuevosRepository repo, ILogger<HuevosController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    /// <summary>Obtiene todos los huevos registrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Huevo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Huevo>>> GetAll(CancellationToken ct)
    {
        var lista = await _repo.GetAllAsync(ct);
        return Ok(lista);
    }

    /// <summary>
    /// Reporte agregado: total de huevos y conteos por tamanio, color y tipo.
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(HuevoStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HuevoStatsDto>> GetStats(CancellationToken ct)
    {
        var stats = await _repo.GetStatsAsync(ct);
        return Ok(stats);
    }

    /// <summary>Obtiene un huevo por su Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Huevo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Huevo>> GetById(int id, CancellationToken ct)
    {
        var huevo = await _repo.GetByIdAsync(id, ct);
        if (huevo is null) return NotFound();
        return Ok(huevo);
    }

    /// <summary>Crea un nuevo huevo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Huevo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Huevo>> Create([FromBody] HuevoCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var creado = await _repo.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    /// <summary>Actualiza un huevo existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] HuevoUpdateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _repo.UpdateAsync(id, dto, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>Elimina un huevo por su Id.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _repo.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}
