using FSAWebSystem.Models.ApprovalSystemCheckModel;

namespace FSAWebSystem.Models.ViewModels
{
    public class BottomPricePagingData : PagingData
    {
        public BottomPricePagingData()
        {
            bottomPrices = new List<BottomPriceModel>();
        }
        public List<BottomPriceModel> bottomPrices { get; set; }
    }
}
