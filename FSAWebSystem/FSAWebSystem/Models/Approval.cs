using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class Approval
    {
        public Approval()
        {
            ApprovalDetails = new List<ApprovalDetail>();
        }

        public Guid Id { get; set; }
        public Proposal Proposal { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public ProposalType ProposalType { get; set; }
        public List<ApprovalDetail> ApprovalDetails { get; set; }
        public string? ApprovalNote { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int Level { get; set; }
        public string ApproverWL { get; set; }

        [NotMapped]
        public bool? IsAndromedaPassed { get; set; }
        [NotMapped]
        public bool? IsBottomPricePassed { get; set; }
        [NotMapped]
        public bool? IsITrustPassed { get; set; }

        [NotMapped]
        public string Level1 { get; set; }
        [NotMapped]
        public string Level2 { get; set; }
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
        public Guid BannerPlantId { get; set; }
        [NotMapped]
        public Guid SKUId { get; set; }
        [NotMapped]
        public Guid ProductCategoryId { get; set; }

        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }
        [NotMapped]
        public DateTime ProposalSubmitDate { get; set; }
        [NotMapped]
        public string RequestedBy { get; set; }

        [NotMapped]
        public decimal RatingRate { get; set; }
        [NotMapped]
        public decimal MonthlyBucket { get; set; }
        [NotMapped]
        public decimal CurrentBucket { get; set; }
        [NotMapped]
        public decimal NextWeekBucket { get; set; }
        [NotMapped]
        public decimal ValidBJ { get; set; }
        [NotMapped]
        public decimal RemFSA { get; set; }
		[NotMapped]
        public ProposalType NextProposalType { get; set; }
        [NotMapped]
        public List<string> EmailApprover { get; set; }
        [NotMapped]
        public decimal WeekCover { get; set; }
        [NotMapped]
        public decimal DistStock { get; set; }
        [NotMapped]
        public string BotPrcRemark { get; set; }
    }

    public enum ApprovalStatus
    {
        Pending,
        WaitingNextLevel,
        Approved,
        Rejected,
        RequestedToNextType
    }

}
