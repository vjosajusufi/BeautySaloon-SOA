namespace BeautySaloon_API.Models;

public class WorkingHours
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsOpen { get; set; } = true;
}
