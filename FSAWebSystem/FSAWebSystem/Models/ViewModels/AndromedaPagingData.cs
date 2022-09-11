using FSAWebSystem.Models.ApprovalSystemCheckModel;

namespace FSAWebSystem.Models.ViewModels
{
    public class AndromedaPagingData : PagingData
    {
        public AndromedaPagingData()
        {
            andromedas = new List<AndromedaModel>();
        }
        public List<AndromedaModel> andromedas { get; set; }
    }
}
