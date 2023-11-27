namespace AzureBlobManager.Infrastructure.Domain.Entities
{
    public partial class Document
    {
        public Guid Id { get; set; }
        public int MediaTypeId { get; set; }
        public string? ParentId { get; set; }
        public string? ParentAlias { get; set; }
        public string? FileName { get; set; }
        public Guid? DocGuid { get; set; }
        public string? Extension { get; set; }
        public long? SizeInByte { get; set; }
        public string? FileUrl { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
