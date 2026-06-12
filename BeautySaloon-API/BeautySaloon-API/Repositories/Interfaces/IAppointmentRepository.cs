using BeautySaloon_API.Models;

namespace BeautySaloon_API.Repositories.Interfaces;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAll(CancellationToken ct = default);
    Task<Appointment?> GetById(int id, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetByUserId(int userId, CancellationToken ct = default);
    Task<Appointment> Create(Appointment appointment, CancellationToken ct = default);
    Task<Appointment> Update(Appointment appointment, CancellationToken ct = default);
    Task<bool> Delete(int id, CancellationToken ct = default);
}
