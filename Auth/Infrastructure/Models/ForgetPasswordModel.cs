namespace Auth.Infrastructure.Models
{
    public class ForgetPasswordModel
    {
        public string Email { get; set; } = null!;
        public int Code { get; set; } 
        public string NewPassword { get; set; } = null!;
    }
}
