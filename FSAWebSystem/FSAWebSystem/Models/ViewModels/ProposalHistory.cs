namespace FSAWebSystem.Models.ViewModels
{
    public class ProposalHistory : Proposal
    {
        public decimal ApprovedRephase { get; set; }
        public decimal ApprovedProposeAdditional { get; set; }

        public string Status { get; set; }
        public string SubmitDate { get; set; }
    }
}
