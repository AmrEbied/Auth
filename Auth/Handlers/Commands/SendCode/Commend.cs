using Auth.Infrastructure.Models;
using MediatR;

namespace Auth.Handlers.Commands.SendCode
{
    public class Commend : IRequest<AuthModel>
    {
        public string Email { get; set; } = null!;
    }
}
