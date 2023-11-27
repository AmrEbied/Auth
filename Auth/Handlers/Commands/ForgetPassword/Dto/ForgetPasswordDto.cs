namespace Auth.Handlers.Commands.ForgetPassword.Dto
{
    public class ForgetPasswordDto
    {
        public string Email { get; set; } = null!;
        public int Code { get; set; } 
        public string NewPassword { get; set; } = null!;
    }
}
