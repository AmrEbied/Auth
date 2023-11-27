using MediatR;

namespace Auth.Handlers.Commands.RevokeToken
{
    public class Commend : IRequest<bool>
    {
        public string Token { get; set; } = null!;
 
    }
}
