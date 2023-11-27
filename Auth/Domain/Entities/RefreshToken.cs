using Microsoft.EntityFrameworkCore;

namespace Auth.Domain.Entities
{
    [Owned]
    public class UserDevice
    {
        public long Id { get; set; }
        public string DeviceId { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
