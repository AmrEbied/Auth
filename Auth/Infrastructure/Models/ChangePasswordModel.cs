namespace Auth.Infrastructure.Models
{
    public class ChangePasswordModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; 
    }
}
