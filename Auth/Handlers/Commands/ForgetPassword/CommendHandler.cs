using Auth.Infrastructure.Models;
using Auth.Infrastructure.Services.Auth;
using AutoMapper;
using MediatR;

namespace Auth.Handlers.Commands.ForgetPassword
{
    public class CommendHandler : IRequestHandler<Commend, AuthModel>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public CommendHandler(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }
        public async Task<AuthModel> Handle(Commend request, CancellationToken cancellationToken)
        {

            return await _authService.ForgetPasswordAsync((_mapper.Map<ForgetPasswordModel>(request.Model)));


        }
    }
}
