using FSAWebSystem.Models.Bucket;

namespace FSAWebSystem.Models.ViewModels
{
    public class WeeklyBucketHistoryPagingData : PagingData
    {
        public WeeklyBucketHistoryPagingData()
        {
            weeklyBucketHistories = new List<WeeklyBucketHistory>();
        }

        public List<WeeklyBucketHistory> weeklyBucketHistories { get; set; }
    }
}
