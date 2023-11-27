using Auth.Handlers.Commands.ForgetPassword.Dto;
using Auth.Infrastructure.Mappings;
using Auth.Infrastructure.Models;
using AutoMapper;
using MediatR;

namespace Auth.Handlers.Commands.ForgetPassword
{
    public class Commend : IRequest<AuthModel>, IMapFrom<ForgetPasswordDto>
    {
        public ForgetPasswordDto? Model { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ForgetPasswordDto, ForgetPasswordModel>().ReverseMap()
                 .IgnoreAllNonExisting(); 
        }
    }
}
