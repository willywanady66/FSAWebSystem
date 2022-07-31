using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Services.Interface
{
    public interface IApprovalService
    {
        public  Task SaveApprovals(List<Approval> listApproval);

        public IQueryable<Approval> GetPendingApprovals();
        public Task<ApprovalPagingData> GetApprovalPagination(DataTableParam param, int month, int year);
    }
}
