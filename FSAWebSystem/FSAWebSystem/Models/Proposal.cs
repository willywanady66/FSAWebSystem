using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class Proposal
    {
        public Guid Id { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid WeeklyBucketId { get; set; }
        public decimal Rephase { get; set; }
        public decimal ProposeAdditional { get; set; }
        public string Remark { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectionReason { get; set; }


        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        
        [NotMapped]
        public string DescriptionMap { get; set; }
        [NotMapped]
        public decimal RatingRate { get; set; }
        [NotMapped]
        public decimal MonthlyBucket { get; set; }
        [NotMapped]
        public decimal CurrentBucket { get; set; }
        [NotMapped]
        public decimal NextBucket { get; set; }
        [NotMapped]
        public decimal ValidBJ { get; set; }
        [NotMapped]
        public decimal RemFSA { get; set; }
        [NotMapped]
        public Guid UserId { get; set; }
    }

    
}
