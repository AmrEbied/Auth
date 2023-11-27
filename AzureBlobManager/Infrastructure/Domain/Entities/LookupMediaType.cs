using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobManager.Infrastructure.Domain.Entities
{
    public partial class LookupMediaType
    {
        public int Id { get; set; }
        public string? Alias { get; set; }
        public string? NameEnglish { get; set; }
        public string? NameArabic { get; set; }
        public string? MediaPath { get; set; }
        public bool? AllowThumb { get; set; }
        public bool? IsActive { get; set; }
    }
}
