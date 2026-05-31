using BeautySaloon_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await SeedUsersAsync(context);
        await SeedServicesAsync(context);
        await SeedWorkingHoursAsync(context);
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        context.Users.AddRange(
            new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@beautysalon.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin"
            },
            new User
            {
                FirstName = "Customer",
                LastName = "User",
                Email = "customer@beautysalon.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                Role = "Customer"
            }
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedServicesAsync(AppDbContext context)
    {
        if (await context.Services.AnyAsync()) return;

        context.Services.AddRange(
            new Service { Name = "Haircut",            Description = "Professional haircut and styling",       Price = 15.00m, DurationMinutes = 30,  IsActive = true },
            new Service { Name = "Hair Coloring",      Description = "Full hair coloring service",             Price = 45.00m, DurationMinutes = 120, IsActive = true },
            new Service { Name = "Manicure",           Description = "Classic manicure with nail polish",      Price = 20.00m, DurationMinutes = 45,  IsActive = true },
            new Service { Name = "Pedicure",           Description = "Classic pedicure with nail polish",      Price = 25.00m, DurationMinutes = 60,  IsActive = true },
            new Service { Name = "Facial Treatment",   Description = "Deep cleansing facial treatment",        Price = 35.00m, DurationMinutes = 60,  IsActive = true },
            new Service { Name = "Eyebrow Threading",  Description = "Precise eyebrow shaping with thread",   Price = 10.00m, DurationMinutes = 15,  IsActive = true }
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedWorkingHoursAsync(AppDbContext context)
    {
        if (await context.WorkingHours.AnyAsync()) return;

        context.WorkingHours.AddRange(
            new WorkingHours { DayOfWeek = DayOfWeek.Monday,    OpenTime = new TimeOnly(9, 0),  CloseTime = new TimeOnly(18, 0), IsOpen = true  },
            new WorkingHours { DayOfWeek = DayOfWeek.Tuesday,   OpenTime = new TimeOnly(9, 0),  CloseTime = new TimeOnly(18, 0), IsOpen = true  },
            new WorkingHours { DayOfWeek = DayOfWeek.Wednesday, OpenTime = new TimeOnly(9, 0),  CloseTime = new TimeOnly(18, 0), IsOpen = true  },
            new WorkingHours { DayOfWeek = DayOfWeek.Thursday,  OpenTime = new TimeOnly(9, 0),  CloseTime = new TimeOnly(18, 0), IsOpen = true  },
            new WorkingHours { DayOfWeek = DayOfWeek.Friday,    OpenTime = new TimeOnly(9, 0),  CloseTime = new TimeOnly(18, 0), IsOpen = true  },
            new WorkingHours { DayOfWeek = DayOfWeek.Saturday,  OpenTime = new TimeOnly(9, 0),  CloseTime = new TimeOnly(15, 0), IsOpen = true  },
            new WorkingHours { DayOfWeek = DayOfWeek.Sunday,    OpenTime = new TimeOnly(0, 0),  CloseTime = new TimeOnly(0, 0),  IsOpen = false }
        );

        await context.SaveChangesAsync();
    }
}
