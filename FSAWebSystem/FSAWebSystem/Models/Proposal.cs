using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class Proposal
    {
        public Guid Id { get; set; }
        public Guid WeeklyBucketId { get; set; }
        public decimal Rephase { get; set; }
        public decimal ProposeAddional { get; set; }
        public string Remark { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }


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
    }

    public class ProposalData
    {
        public int TotalRecord { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public List<Proposal> Proposals { get; set; }
    }
}
