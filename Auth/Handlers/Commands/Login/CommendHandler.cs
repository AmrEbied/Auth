using Auth.Handlers.Commands.Login.Dto;
using Auth.Handlers.Commands.Register.Dto;
using Auth.Infrastructure.Models;
using Auth.Infrastructure.Services.Auth;
using AutoMapper;
using MediatR;

namespace Auth.Handlers.Commands.Login
{
    public class CommendHandler : IRequestHandler<Commend, LoginResponseDTO>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public CommendHandler(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }
        public async Task<LoginResponseDTO> Handle(Commend request, CancellationToken cancellationToken)
        {

            return _mapper.Map<LoginResponseDTO>(await _authService.GetTokenAsync((_mapper.Map<TokenRequestModel>(request.Model))));


        }
    }
}
