using Auth.Handlers.Commands.Login.Dto;
using Auth.Infrastructure.Mappings;
using Auth.Infrastructure.Models;
using AutoMapper;
using MediatR;

namespace Auth.Handlers.Commands.Login
{
    public class Commend : IRequest<LoginResponseDTO>, IMapFrom<TokenRequestModel>
    {
        public LoginDto? Model { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<LoginDto, TokenRequestModel>().ReverseMap()
                 .IgnoreAllNonExisting();
            profile.CreateMap<LoginResponseDTO, AuthModel>().ReverseMap()
                .IgnoreAllNonExisting();
        }
    }
}
