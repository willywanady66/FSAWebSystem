namespace FSAWebSystem.Models.ViewModels
{
    public class ProposalHistoryPagingData : PagingData
    {
        public ProposalHistoryPagingData()
        {
            proposalsHistory = new List<ProposalHistory>();
        }

        public List<ProposalHistory> proposalsHistory { get; set; }
    }
}
