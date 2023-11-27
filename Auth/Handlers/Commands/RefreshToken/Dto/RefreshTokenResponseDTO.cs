namespace Auth.Handlers.Commands.RefreshToken.Dto
{
    public class RefreshTokenResponseDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
