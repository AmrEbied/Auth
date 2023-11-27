using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace AzureBlobManager.Infrastructure.Persistence.DataContext
{
    public partial class DocumentDbContext : IDocumentDbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public DocumentDbContext(DbContextOptions<DocumentDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> UpdateAsync<T>(T obj, CancellationToken cancellationToken) where T : class
        {
            Entry<T>(obj).State = EntityState.Modified;
            return await SaveChangesAsync(obj, cancellationToken);
        }

        private async Task<int> SaveChangesAsync<T>(T obj, CancellationToken cancellationToken) where T : class
        {
            int result = await SaveChangesAsync(cancellationToken);
            Entry(obj).State = EntityState.Detached;
            return await Task.FromResult(result);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public void DetachEntity<T>(T obj) where T : class
        {
            Entry(obj).State = EntityState.Detached;
        }
    }
}
