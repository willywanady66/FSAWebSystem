using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models.Bucket
{
    public class WeeklyBucketHistory
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public Guid BannerId { get; set; }
        public Guid SKUId { get; set; }
        public decimal DispatchConsume { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        [NotMapped]
        public string UploadedDate { get; set; }
        public string ULIWeek { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }
        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
    }
}
