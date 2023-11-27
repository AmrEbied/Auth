using Auth.Handlers.Commands.Register.Dto;
using Auth.Infrastructure.Mappings;
using Auth.Infrastructure.Models;
using AutoMapper;
using MediatR;

namespace Auth.Handlers.Commands.Register
{
    public class Commend : IRequest<RegisterResponseDTO>, IMapFrom<RegisterModel>
    {
        public RegisterDto? Model { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<RegisterDto, RegisterModel>().ReverseMap()
                 .IgnoreAllNonExisting();
            profile.CreateMap<AuthModel, RegisterResponseDTO>().ReverseMap()
                 .IgnoreAllNonExisting();
        }
    }
}
