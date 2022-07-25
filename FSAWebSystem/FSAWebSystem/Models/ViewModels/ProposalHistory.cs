using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models.ViewModels
{
    public class ProposalHistory
    {
        public Guid Id { get; set; }
        public string SubmittedBy { get; set; }
        public string SubmittedAt { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectionReason { get; set; }

        public decimal ApprovedRephase { get; set; }
        public decimal ApprovedProposeAdditional { get; set; }

        public string Status { get; set; }

        [NotMapped]
        public decimal Rephase { get; set; }
        [NotMapped]
        public string Remark { get; set; }
        [NotMapped]
        public decimal ProposeAdditional { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }

        [NotMapped]
        public string BannerName { get; set; }
    }
}
