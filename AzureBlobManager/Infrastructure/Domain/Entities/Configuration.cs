using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobManager.Infrastructure.Domain.Entities
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}
