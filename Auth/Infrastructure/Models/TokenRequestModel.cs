using System.ComponentModel.DataAnnotations;

namespace Auth.Infrastructure.Models
{
    public class TokenRequestModel
    {
        
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
