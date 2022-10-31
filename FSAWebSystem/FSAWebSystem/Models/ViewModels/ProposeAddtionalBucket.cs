using FSAWebSystem.Models.Bucket;

namespace FSAWebSystem.Models.ViewModels
{
    public class ProposeAddtionalBucket
    {
        public ProposeAddtionalBucket()
        {
            WeeklyBucketTargets = new List<WeeklyBucket>();
            WeeklyBucketSource = new List<WeeklyBucket>();
        }

        public List<WeeklyBucket> WeeklyBucketTargets { get; set; }
        public List<WeeklyBucket> WeeklyBucketSource { get; set; }
        public WeeklyBucket GroupedBucket { get; set; }
    }
}
