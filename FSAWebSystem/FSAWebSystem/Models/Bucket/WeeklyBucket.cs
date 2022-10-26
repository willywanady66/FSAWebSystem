using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models.Bucket
{
    public class WeeklyBucket
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public BannerPlant BannerPlant { get; set; }
        public Guid SKUId { get; set; }
        public decimal PlantContribution { get; set; }
        public decimal MonthlyBucket { get; set; }
        public decimal RatingRate { get; set; }
        public decimal BucketWeek1 { get; set; }
        public decimal BucketWeek2 { get; set; }
        public decimal BucketWeek3 { get; set; }
        public decimal BucketWeek4 { get; set; }
        public decimal BucketWeek5 { get; set; }
        public decimal ValidBJ { get; set; }
        public decimal RemFSA { get; set; }
        public decimal DispatchConsume {get;set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        //public Guid MonthlyBucketId { get; set; }

        [NotMapped]
        public string CDM { get; set; }
        [NotMapped]
        public string KAM { get; set; }
        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }
        [NotMapped]
        public decimal CurrentBucket { get; set; }
        [NotMapped]
        public decimal NextBucket { get; set; }
        [NotMapped]
        public Guid ProductCategoryId { get; set; }
    }
}
