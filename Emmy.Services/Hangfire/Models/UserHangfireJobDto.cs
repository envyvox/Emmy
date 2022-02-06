using System;
using AutoMapper;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;

namespace Emmy.Services.Hangfire.Models
{
    public record UserHangfireJobDto(
        HangfireJobType Type,
        string JobId,
        DateTimeOffset Expiration);

    public class UserHangfireJobProfile : Profile
    {
        public UserHangfireJobProfile() => CreateMap<UserHangfireJob, UserHangfireJobDto>();
    }
}
