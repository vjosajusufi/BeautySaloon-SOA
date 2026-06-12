using BeautySaloon_API.Models;

namespace BeautySaloon_API.Repositories.Interfaces;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAll(CancellationToken ct = default);
    Task<Service?> GetById(int id, CancellationToken ct = default);
    Task<Service> Create(Service service, CancellationToken ct = default);
    Task<Service> Update(Service service, CancellationToken ct = default);
    Task<bool> Delete(int id, CancellationToken ct = default);
}
