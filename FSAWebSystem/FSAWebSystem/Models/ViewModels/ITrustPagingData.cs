using FSAWebSystem.Models.ApprovalSystemCheckModel;

namespace FSAWebSystem.Models.ViewModels
{
    public class ITrustPagingData : PagingData
    {
        public ITrustPagingData()
        {
            iTrusts = new List<ITrustModel>();
        }        
        public List<ITrustModel> iTrusts { get; set; }
    }
}
