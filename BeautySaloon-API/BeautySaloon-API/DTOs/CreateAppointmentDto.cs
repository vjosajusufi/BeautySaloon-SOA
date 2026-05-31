namespace BeautySaloon_API.DTOs;

public class CreateAppointmentDto
{
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public string? Notes { get; set; }
}
