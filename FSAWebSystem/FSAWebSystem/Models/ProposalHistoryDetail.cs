namespace FSAWebSystem.Models
{
    public class ProposalHistoryDetail
    {
        public Guid Id { get; set; }
        public Guid ProposalHistoryId { get; set; }
        public Guid WeeklyBucketId { get; set; }
        public decimal ProposeAdditional { get; set; }
        public decimal Rephase { get; set; }
    }
}
