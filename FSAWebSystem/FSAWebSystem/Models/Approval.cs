using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class Approval
    {
        public Guid Id { get; set; }
        public Guid ProposalId { get; set; }
        public Guid SubmittedBy { get; set; }
        public DateTime SubmittedAt { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        
        public string? RejectionReason { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }


        [NotMapped]
        public Guid WeeklyBucketId { get; set; }
    }
}
