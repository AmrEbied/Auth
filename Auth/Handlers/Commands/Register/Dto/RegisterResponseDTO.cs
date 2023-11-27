namespace Auth.Handlers.Commands.Register.Dto
{
    public class RegisterResponseDTO
    {
        public string? Message { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
        public int Code { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
