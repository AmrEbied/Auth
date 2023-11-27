using Auth.Handlers.Commands.Login.Dto;
using Auth.Handlers.Commands.RefreshToken.Dto;
using Auth.Infrastructure.Mappings;
using Auth.Infrastructure.Models;
using AutoMapper;
using MediatR;

namespace Auth.Handlers.Commands.RefreshToken
{
    public class Commend : IRequest<RefreshTokenResponseDTO>, IMapFrom<TokenRequestModel>
    {
        public string Token { get; set; } = null!;

        public void Mapping(Profile profile)
        {
             
            profile.CreateMap<RefreshTokenResponseDTO, AuthModel>().ReverseMap()
                .IgnoreAllNonExisting();
        }
    }
}
