using FSAWebSystem.Models;
using FSAWebSystem.Models.ApprovalSystemCheckModel;
using FSAWebSystem.Models.Bucket;

namespace FSAWebSystem.Services.Interface
{
    public interface IUploadDocumentService
    {
        public Task SaveMonthlyBuckets(List<MonthlyBucket> monthlyBuckets);
        public Task SaveDocument(FSADocument fsaDoc);
        public Task SaveWeeklyBuckets(List<WeeklyBucket> weeklyBuckets);
        public Task SaveWeeklyBucketHistories(List<WeeklyBucketHistory> weeklyBucketHistories);
        public Task SaveAndromeda(List<AndromedaModel> listAndromeda);
        public Task SaveITrust(List<ITrustModel> listITrust);
        public Task SaveBottomPrice(List<BottomPriceModel> listBottomPrice);

        public FSADocument CreateFSADoc(string fileName, string loggedUser, DocumentUpload docType);

        public List<string> GetBannerColumns();
        public List<string> GetMonthlyBucketColumns();
        public List<string> GetSKUColumns();
        public List<string> GetWeeklyDispatchColumns();
        public List<string> GetDailyOrderColumns();
        public List<string> GetUserColumns();
        public List<string> GetAndromedaColumns();
        public List<string> GetBottomPriceColumns();
        public List<string> GetITrustColumns();
        public Task DeleteAndromeda();
        public Task DeleteITrust();
        public Task DeleteBottomPrice();

    }
}
