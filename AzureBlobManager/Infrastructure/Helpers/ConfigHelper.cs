using AzureBlobManager.Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobManager.Infrastructure.Helpers
{

    public interface IConfigHelper
    {
        Task<string> GetConfig(string name);
    }
    public class ConfigHelper : IConfigHelper
    {
        private readonly IDocumentDbContext _context;
        public ConfigHelper(IDocumentDbContext context)
        {
            _context = context;
        }
        public async Task<string> GetConfig(string name)
        {
            var config = await _context.Configurations.FirstOrDefaultAsync(x => x.Name == name);
            if(config != null)
            {
                return config.Value;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
