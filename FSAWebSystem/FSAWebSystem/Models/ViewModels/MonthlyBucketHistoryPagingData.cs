using FSAWebSystem.Models.Bucket;

namespace FSAWebSystem.Models.ViewModels
{
    public class MonthlyBucketHistoryPagingData : PagingData
    {
        public MonthlyBucketHistoryPagingData()     
        {
            monthlyBuckets = new List<MonthlyBucket>();
        }

        public List<MonthlyBucket> monthlyBuckets { get; set; }
    }
}
