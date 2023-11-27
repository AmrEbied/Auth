using Auth.Handlers.Commands.ChangePassword.Dto;
using Auth.Infrastructure.Mappings;
using Auth.Infrastructure.Models;
using AutoMapper;
using MediatR;

namespace Auth.Handlers.Commands.ChangePassword
{
    public class Commend : IRequest<AuthModel>, IMapFrom<ChangePasswordDto>
    {
        public ChangePasswordDto? Model { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ChangePasswordDto, ChangePasswordModel>().ReverseMap()
                 .IgnoreAllNonExisting();
        }
    }
}
