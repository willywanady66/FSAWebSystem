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
        public Task<List<Tuple<string, Guid, Approval, string>>> GetRecipientEmail(Approval approval);
        public string GetWLApprover(Approval approval);
        public Task<List<EmailApproval>> GenerateEmailProposal(Approval approval, string url, string requestor);
        public Task<EmailApproval> GenerateEmailApproval(Approval approval, string userApproverEmail, string requesterEmail, string approvalNote, BannerPlant bannerPlant, SKU sku);

        public Task<List<EmailApproval>> GenerateCombinedEmailApproval(List<Approval> approvals, string userApproverEmail, string approvalNote); 

        public Task<List<EmailApproval>> GenerateCombinedEmailProposal(List<Approval> approvals, string baseUrl, string requestor = "");
        
    }
}
