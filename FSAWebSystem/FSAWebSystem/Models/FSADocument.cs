namespace FSAWebSystem.Models
{
    public class FSADocument
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
