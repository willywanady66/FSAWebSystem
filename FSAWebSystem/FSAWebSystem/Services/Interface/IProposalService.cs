using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using static FSAWebSystem.Models.ViewModels.ProposalViewModel;

namespace FSAWebSystem.Services.Interface
{
    public interface IProposalService
    {

        public Task<ProposalData> GetProposalForView(int month, int year, int week, DataTableParamProposal param, UserUnilever userUnilever);
        //public Task<ProposalData> GetProposalReallocateForView(int month, int year, int week, DataTableParamProposal param, Guid userId);
        //public Task<ProposalData> GetProposalForView(int month, int year, int week, int pageNo, Guid userId);
        public Task SaveProposals(List<Proposal> listProposal);
        public Task SaveProposalHistories(List<ProposalHistory> listProposalHistory);
        public Task<bool> IsProposalExist(FSACalendarDetail fsaDetail);
        public IQueryable<Proposal> GetPendingProposals(FSACalendarDetail fsaDetail, Guid userId);

        public ProposalHistoryPagingData GetProposalHistoryPagination(DataTableParam param, UserUnilever userUnilever, int month, int year);

        public Task<Proposal> GetProposalById(Guid proposalId);
        public Task<Proposal> GetProposalByApprovalId(Guid approvalId);
        public List<ProposalExcelModel> GetProposalExcelData(int month, int year, UserUnilever user);
        public Task<ProposalHistory> GetProposalHistory(Guid approvalId);
    }
}
