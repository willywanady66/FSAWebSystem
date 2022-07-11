using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Services.Interface
{
    public interface IProposalService
    {

        public Task<ProposalData> GetProposalForView(int month, int year, int week, DataTableParam param, string bannerName ="");
    }
}
