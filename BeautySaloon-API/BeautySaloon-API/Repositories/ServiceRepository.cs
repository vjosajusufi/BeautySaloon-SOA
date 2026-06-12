using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Repositories;

public class ServiceRepository(AppDbContext context) : IServiceRepository
{
    public async Task<IEnumerable<Service>> GetAll(CancellationToken ct = default) =>
        await context.Services.AsNoTracking().ToListAsync(ct);

    public async Task<Service?> GetById(int id, CancellationToken ct = default) =>
        await context.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Service> Create(Service service, CancellationToken ct = default)
    {
        context.Services.Add(service);
        await context.SaveChangesAsync(ct);
        return service;
    }

    public async Task<Service> Update(Service service, CancellationToken ct = default)
    {
        context.Entry(service).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
        return service;
    }

    public async Task<bool> Delete(int id, CancellationToken ct = default)
    {
        var service = await context.Services.FindAsync(new object[] { id }, ct);
        if (service is null) return false;
        context.Services.Remove(service);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
