namespace FSAWebSystem.Models.ViewModels
{
    public class ProposalViewModel
    {
        public class ProposalData
        {
            public ProposalData()
            {
                proposals = new List<Proposal>();
            }

            public int draw { get; set; }
            public int totalRecord { get; set; }
            public int pageSize { get; set; }
            public int pageNo { get; set; }
            public List<Proposal> proposals { get; set; }
            public List<ProposalInput> proposalInputs { get; set; }
        }

        public class ProposalInput
        {
            public string weeklyBucketId { get; set; }
            public string bannerName { get; set; }
            public string pcMap { get; set; }
            public decimal nextWeekBucket { get; set; }
            public decimal rephase { get; set; }
            public decimal proposeAdditional { get; set; }
            public string remark { get; set; }
            //public string Remark { get; set; }
        }
    }
}
