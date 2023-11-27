using AzureBlobManager.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureBlobManager.Infrastructure.Persistence.DataContext
{
    public interface IDocumentDbContext
    {
        DbSet<Document>? Documents { get; set; }
        DbSet<LookupMediaType>? LookupMediaTypes { get; set; }
        DbSet<Configuration>? Configurations { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> UpdateAsync<T>(T obj, CancellationToken cancellationToken) where T : class;
        DbContext CurrentDbContext { get; }
    }
}
