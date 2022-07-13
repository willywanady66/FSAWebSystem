using FSAWebSystem.Models.Bucket;

namespace FSAWebSystem.Services.Interface
{
    public interface IBucketService
    {
        public IQueryable<MonthlyBucket> GetMonthlyBucket();
        public IQueryable<WeeklyBucket> GetWeeklyBucket();
    }
}
