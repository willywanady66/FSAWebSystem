using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class Approval
    {
        public Approval()
        {
            ApprovedBy = new List<UserUnilever>();
        }
        public Guid Id { get; set; }
        public Guid ProposalId { get; set; }
        //public Guid SubmittedBy { get; set; }
        //public DateTime SubmittedAt { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }

        public ProposalType ProposalType { get; set; }

        //public decimal ApprovedRephase { get; set; }
        //public decimal ApprovedProposeAdditional { get; set; }
        [NotMapped]
        public string Level1 { get; set; }
        [NotMapped ]
        public string Level2 { get; set; }
        public string? RejectionReason { get; set; }
        public List<UserUnilever> ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }


		[NotMapped]
        public Proposal Proposal { get; set; }
        [NotMapped]
        public int Week { get; set; }
        [NotMapped]
        public Guid WeeklyBucketId { get; set; }

        [NotMapped]
        public decimal Rephase { get; set; }
        [NotMapped]
        public decimal ProposeAdditional { get; set; }

        [NotMapped]
        public string Remark { get; set; }

        [NotMapped]
        public string BannerName { get; set; }
		
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }
        [NotMapped]
        public string SubmitDate { get; set; }

    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }

}
