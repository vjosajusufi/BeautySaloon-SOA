using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Repositories;

public class ServiceRepository(AppDbContext context) : IServiceRepository
{
    public async Task<IEnumerable<Service>> GetAll() =>
        await context.Services.ToListAsync();

    public async Task<Service?> GetById(int id) =>
        await context.Services.FindAsync(id);

    public async Task<Service> Create(Service service)
    {
        context.Services.Add(service);
        await context.SaveChangesAsync();
        return service;
    }

    public async Task<Service> Update(Service service)
    {
        context.Services.Update(service);
        await context.SaveChangesAsync();
        return service;
    }

    public async Task<bool> Delete(int id)
    {
        var service = await context.Services.FindAsync(id);
        if (service is null) return false;
        context.Services.Remove(service);
        await context.SaveChangesAsync();
        return true;
    }
}
