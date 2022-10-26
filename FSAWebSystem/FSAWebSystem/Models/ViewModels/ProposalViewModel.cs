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
            public string id { get; set; }
            public string approvalId { get; set; }
            public string weeklyBucketId { get; set; }
            public string bannerName { get; set; }
            public string plantName { get; set; }
            public string pcMap { get; set; }
            public decimal currentBucket { get; set; }
            public decimal nextWeekBucket { get; set; }
            public decimal rephase { get; set; }
            public decimal proposeAdditional { get; set; }
            public decimal reallocate { get; set; }
            public string remark { get; set; }
            public string bannerId { get; set; }
            public string skuId { get; set; }
            public string bannerTargetId { get; set; }
            public bool isWaitingApproval { get; set; }
            public ProposalType Type { get; set; }
            public string KAM { get; set; }
            public string CDM { get; set; }
            //public string Remark { get; set; }
        }
    }
}
