using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class Proposal
    {
        public Proposal()
        {
            ProposalDetails = new List<ProposalDetail>();
        }
        public Guid Id { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        //public Guid WeeklyBucketId { get; set; }    
        public Banner Banner { get; set; }
        public string KAM { get; set; }
        public string CDM { get; set; }
        public SKU Sku { get; set; }
        public decimal Rephase { get; set; }
        public decimal ProposeAdditional { get; set; }
        public string? Remark { get; set; }
        public ProposalType? Type { get; set; }
        //public Guid ApprovalId { get; set; }
        public List<ProposalDetail>? ProposalDetails { get; set; }
        public bool IsWaitingApproval { get; set; }

        public DateTime SubmittedAt { get; set; }
   
        public Guid? SubmittedBy { get; set; }


        [NotMapped]
        public decimal ApprovedRephase { get; set; }
        [NotMapped]
        public decimal ApprovedProposeAdditional { get; set; }
        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string Category { get; set; }

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
        public string UserName { get; set; }

        [NotMapped]
        public ApprovalStatus ApprovalStatus { get; set; }
        [NotMapped]
        public string ApprovedBy { get; set; }
        [NotMapped]
        public string ApprovalNote { get; set; }
        [NotMapped]
        public string ApproverWL { get; set; }

		[NotMapped]
        public string BannerNameTarget { get; set; }
		[NotMapped]
        public string PlantNameTarget { get; set; }
		[NotMapped]
        public string PlantCodeTarget { get; set; }
        [NotMapped]
        public Guid ProductCategoryId { get; set; }
        [NotMapped]
        public Guid SKUId { get; set; }

    }


    public enum ProposalType
	{
        Rephase,
        ProposeAdditional,
        ReallocateAcrossKAM,
        ReallocateAcrossCDM,
        ReallocateAcrossMT,
        
    }

 }
