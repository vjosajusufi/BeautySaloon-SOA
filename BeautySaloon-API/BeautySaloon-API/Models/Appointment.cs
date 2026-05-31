namespace BeautySaloon_API.Models;

public class Appointment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public Service Service { get; set; } = null!;
}
