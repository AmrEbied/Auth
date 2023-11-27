using System.ComponentModel.DataAnnotations;

namespace Auth.Infrastructure.Models
{
    public class AddRoleModel
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
