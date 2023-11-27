using Auth.Infrastructure.Services.Auth;
using MediatR;

namespace Auth.Handlers.Commands.RevokeToken
{
    public class CommendHandler : IRequestHandler<Commend, bool>
    {
        private readonly IAuthService _authService; 
        public CommendHandler(IAuthService authService )
        {
            _authService = authService; 
        }
        public async Task<bool> Handle(Commend request, CancellationToken cancellationToken)
        {

          return  await _authService.RevokeTokenAsync(request.Token);


        } 
    }
}
