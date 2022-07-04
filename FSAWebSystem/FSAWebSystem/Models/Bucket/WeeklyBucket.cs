namespace FSAWebSystem.Models.Bucket
{
    public class WeeklyBucket
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Banner Banner { get; set; }
        public SKU SKU { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public MonthlyBucket MonthlyBucket { get; set; }
        public decimal RatingRate { get; set; }
        public decimal BucketWeek1 { get; set; }
        public decimal BucketWeek2 { get; set; }
        public decimal BucketWeek3 { get; set; }
        public decimal BucketWeek4 { get; set; }
        public decimal BucketWeek5 { get; set; }
        public decimal ValidBJ { get; set; }
        public decimal RemFSA { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
