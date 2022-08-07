using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Services.Interface
{
    public interface IBucketService
    {

        public Task<MonthlyBucketHistoryPagingData> GetMonthlyBucketHistoryPagination(DataTableParam param, Guid userId);
        public Task<WeeklyBucketHistoryPagingData> GetWeeklyBucketHistoryPagination(DataTableParam param, Guid userId);
        public IQueryable<MonthlyBucket> GetMonthlyBuckets();
        public IQueryable<WeeklyBucket> GetWeeklyBuckets();

        public Task<WeeklyBucket> GetWeeklyBucket(Guid id);

        public Task<List<Guid>> GetWeeklyBucketBanners();

        public Task<WeeklyBucket> GetWeeklyBucketByBanner(Guid targetBanner, int year, int month);
    }
}
