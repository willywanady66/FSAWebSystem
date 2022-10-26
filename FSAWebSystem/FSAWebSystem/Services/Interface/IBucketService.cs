using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Services.Interface
{
    public interface IBucketService
    {

        public Task<MonthlyBucketHistoryPagingData> GetMonthlyBucketHistoryPagination(DataTableParam param, UserUnilever userUnilever);
        public Task<WeeklyBucketHistoryPagingData> GetWeeklyBucketHistoryPagination(DataTableParam param, UserUnilever userUnilever);
        public IQueryable<MonthlyBucket> GetMonthlyBuckets();
        public IQueryable<WeeklyBucket> GetWeeklyBuckets();

        public Task<WeeklyBucket> GetWeeklyBucket(Guid id);

        public Task<List<Guid>> GetWeeklyBucketBanners();

        public Task<WeeklyBucket> GetWeeklyBucketByBanner(Guid targetBanner, int year, int month);

        public Task<bool> WeeklyBucketExist(int month, int week, int year);

        public Task<IQueryable<WeeklyBucket>> GetWeeklyBucketsByBannerSKU(Guid bannerId, Guid skuId);
    }
}
