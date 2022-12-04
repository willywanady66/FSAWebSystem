namespace FSAWebSystem.Models.ViewModels
{
    public class BaseReportModel
    {
        public Guid WeeklyBucketId { get; set; }
        public string BannerName { get; set; }
        public string PCMap { get; set; }
        public string Description { get; set; }
        public string Category{ get; set; }
        public string PlantCode { get; set; }
        public string PlantName{get; set;}
        public decimal Price { get; set; }
        public decimal PlantContribution { get; set; }
        public decimal RR { get; set; }
        public decimal TCT { get; set; }
        public decimal Target { get; set; }
        public decimal MonthlyBucket { get; set; }
        public decimal Week1 { get; set; }
        public decimal Week2 { get; set; }
        public decimal ValidBJ { get; set; }
        public decimal RemFSA { get; set; }
        public Guid BannerPlantId { get; set; }
        public Guid SKUId { get; set; }
    }

    public class ReportRephaseData
    {
        public int Week { get; set; }
        public decimal Rephase { get; set; }
    }

    public class ReportProposeData
    {
        public int Week { get; set; }
        public decimal Propose { get; set; }
    }
}
