using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Services
{
    public class BucketService : IBucketService
    {
        private FSAWebSystemDbContext _db;
        public BucketService(FSAWebSystemDbContext db)
        {
            _db = db;
        }
        public IQueryable<MonthlyBucket> GetMonthlyBucket()
        {
            return _db.MonthlyBuckets;
        }

        public IQueryable<WeeklyBucket> GetWeeklyBucket()
        {
            return _db.WeeklyBuckets;
        }
    }
}
