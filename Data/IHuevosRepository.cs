using WebServiceHuevo.Models;

namespace WebServiceHuevo.Data;

public interface IHuevosRepository
{
    Task<IEnumerable<Huevo>> GetAllAsync(CancellationToken ct = default);
    Task<Huevo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Huevo> CreateAsync(HuevoCreateDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, HuevoUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<HuevoStatsDto> GetStatsAsync(CancellationToken ct = default);
}
