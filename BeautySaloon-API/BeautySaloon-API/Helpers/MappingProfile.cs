using AutoMapper;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Models;

namespace BeautySaloon_API.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.UserFullName,
                opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.ServiceName,
                opt => opt.MapFrom(src => src.Service.Name));

        CreateMap<CreateAppointmentDto, Appointment>();

        CreateMap<Service, ServiceDto>();
        CreateMap<CreateServiceDto, Service>();

        CreateMap<WorkingHours, WorkingHoursDto>();
        CreateMap<CreateWorkingHoursDto, WorkingHours>();

        CreateMap<UpdateUserDto, User>();
    }
}
