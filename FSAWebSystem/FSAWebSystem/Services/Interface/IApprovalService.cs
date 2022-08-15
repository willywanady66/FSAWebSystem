using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Services.Interface
{
    public interface IApprovalService
    {
        public  Task SaveApprovals(List<Approval> listApproval);

        public IQueryable<Approval> GetPendingApprovals();
        public Task<ApprovalPagingData> GetApprovalPagination(DataTableParam param, int month, int year, UserUnilever user);
        //public Task<ApprovalPagingData> GetApprovalReallocatePagination(DataTableParam param, int month, int year);
        public Task<Approval> GetApprovalById(Guid approvalId);
        public Task<Approval> GetApprovalDetails(Guid approvalId);
    }
}
