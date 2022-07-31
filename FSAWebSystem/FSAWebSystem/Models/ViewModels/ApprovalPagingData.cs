namespace FSAWebSystem.Models.ViewModels
{
    public class ApprovalPagingData : PagingData
    {
        public ApprovalPagingData()
        {
            approvals = new List<Approval>();
        }

        public List<Approval> approvals { get; set; }
    }
}
