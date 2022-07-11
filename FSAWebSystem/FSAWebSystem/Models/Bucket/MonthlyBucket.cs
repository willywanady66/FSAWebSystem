using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models.Bucket
{
    public class MonthlyBucket
    {
        public Guid Id { get; set; }
        public string? SWF2 { get; set; }
        public string? SWF3 { get; set; }
        public Guid BannerId { get; set; }
        public Guid SKUId { get; set; }
        public decimal Price { get; set; }
        public decimal PlantContribution { get; set; }
        public decimal RatingRate { get; set; }
        public decimal TCT { get; set; }
        public decimal MonthlyTarget { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public FSADocument FSADocument { get; set; }

        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
    }
}
