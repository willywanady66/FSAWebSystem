namespace FSAWebSystem.Models
{
    public class MonthlyBucketHistory
    {
        public Guid Id { get; set; }
        public Guid WeeklyBucketId { get; set; }
        public decimal MonthlyBucket { get; set; }
        public int Version { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
  
    }
}
