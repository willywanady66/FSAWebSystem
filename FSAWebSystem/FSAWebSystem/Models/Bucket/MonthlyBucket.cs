using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models.Bucket
{
    public class MonthlyBucket
    {
        public Guid Id { get; set; }
        public string? SWF2 { get; set; }
        public string? SWF3 { get; set; }
        public BannerPlant BannerPlant { get; set; }
        public Guid SKUId { get; set; }
        public decimal Price { get; set; }
        public decimal PlantContribution { get; set; }
        public decimal RunningRate { get; set; }
        public decimal TCT { get; set; }
        public decimal MonthlyTarget { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public FSADocument FSADocument { get; set; }

        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public Guid BPlantId { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string UploadedDate { get; set; }
        [NotMapped]
        public string KAM { get; set; }
        [NotMapped]
        public string CDM { get; set; }
        [NotMapped]
        public Guid BnrId { get; set; }
    }
}
