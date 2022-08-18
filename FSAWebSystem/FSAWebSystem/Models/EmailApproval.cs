namespace FSAWebSystem.Models
{
    public class EmailApproval { 
        public string RecipientEmail { get; set; }
        public string Name { get; set; }
        public string ApprovalUrl { get; set; }
        public string Requestor { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
