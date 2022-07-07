using FSAWebSystem.Models;

namespace FSAWebSystem.Services.Interface
{
    public interface IProposalService
    {

        public Task<List<Proposal>> GetProposalForView(int month, int year, int week, string bannerName ="");
    }
}
