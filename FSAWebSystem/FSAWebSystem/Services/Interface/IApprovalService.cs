using FSAWebSystem.Models;

namespace FSAWebSystem.Services.Interface
{
    public interface IApprovalService
    {
        public  Task SaveApprovals(List<Approval> listApproval);

        public IQueryable<Approval> GetPendingApprovals();
    }
}
