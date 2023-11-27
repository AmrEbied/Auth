using AzureBlobManager.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureBlobManager.Infrastructure.Persistence.DataContext
{
    public partial class DocumentDbContext : DbContext
    {
        public DocumentDbContext()
        {
        }
        
        public DbContext CurrentDbContext { get { return this; } }
        public DocumentDbContext(DbContextOptions<DocumentDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Document>? Documents { get; set; }
        public virtual DbSet<LookupMediaType>? LookupMediaTypes { get; set; }
        public virtual DbSet<Configuration>? Configurations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Extension).HasMaxLength(10);

                entity.Property(e => e.FileName).HasMaxLength(250);

                entity.Property(e => e.ParentId).HasMaxLength(50);

                entity.Property(e => e.FileUrl).HasMaxLength(500);
            });

            modelBuilder.Entity<LookupMediaType>(entity =>
            {
                entity.HasIndex(e => e.Alias, "IDX_Alias")
                    .IsUnique();

                entity.Property(e => e.Alias).HasMaxLength(50);

                entity.Property(e => e.NameArabic).HasMaxLength(100);

                entity.Property(e => e.NameEnglish).HasMaxLength(100);

                entity.Property(e => e.MediaPath).HasMaxLength(100);
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(250);
            });
        }
    }
}
