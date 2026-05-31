using BeautySaloon_API.Models;

namespace BeautySaloon_API.Repositories.Interfaces;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAll();
    Task<Service?> GetById(int id);
    Task<Service> Create(Service service);
    Task<Service> Update(Service service);
    Task<bool> Delete(int id);
}
