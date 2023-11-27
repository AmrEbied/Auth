using Auth.Handlers.Commands.Login.Dto;
using Auth.Handlers.Commands.RefreshToken.Dto;
using Auth.Infrastructure.Models;
using Auth.Infrastructure.Services.Auth;
using AutoMapper;
using MediatR;

namespace Auth.Handlers.Commands.RefreshToken
{
    public class CommendHandler : IRequestHandler<Commend, RefreshTokenResponseDTO>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public CommendHandler(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }
        public async Task<RefreshTokenResponseDTO> Handle(Commend request, CancellationToken cancellationToken)
        {

            return _mapper.Map<RefreshTokenResponseDTO>(await _authService.RefreshTokenAsync(request.Token));


        }
    }
}
