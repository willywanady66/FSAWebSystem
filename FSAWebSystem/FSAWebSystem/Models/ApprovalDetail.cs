using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class ApprovalDetail
    {
        public Guid Id { get; set; }
        public Guid ApprovalId { get; set; }
        public decimal ProposeAdditional { get; set; }
        public decimal Rephase { get; set; }
        public decimal PlantContribution { get; set; }
        public decimal ActualProposeAdditional { get; set; }

        public Approval Approval { get; set; }
        [NotMapped]
        public string CDM { get; set; }
        [NotMapped]
        public string KAM { get; set; }

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
        public decimal RunningRate { get; set; }
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
        public ProposalType? ProposalType { get; set; }
    }
}
