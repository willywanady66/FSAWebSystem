using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class ProposalHistory
    {
        public Guid Id { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid ProposalId { get; set; }
        public Guid ApprovalId { get; set; }
        public decimal Rephase { get; set; }
        public decimal ProposeAdditional { get; set; }
        public string Remark { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedAt { get; set; }


        [NotMapped]
        public Proposal Proposal { get; set; }
        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }
        [NotMapped]
        public string ApprovedBy { get; set; }
        [NotMapped]
        public string RejectionReason { get; set; }
        [NotMapped]
        public string ApprovalStatus { get; set; }
    }
}
