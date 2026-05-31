using BeautySaloon_API.Models;

namespace BeautySaloon_API.Repositories.Interfaces;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAll();
    Task<Appointment?> GetById(int id);
    Task<IEnumerable<Appointment>> GetByUserId(int userId);
    Task<Appointment> Create(Appointment appointment);
    Task<Appointment> Update(Appointment appointment);
    Task<bool> Delete(int id);
}
