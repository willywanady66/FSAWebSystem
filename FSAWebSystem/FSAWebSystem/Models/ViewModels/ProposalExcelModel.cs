namespace FSAWebSystem.Models.ViewModels
{
    public class ProposalExcelModel
    {
        public int Month { get; set; }
        public string KAM { get; set; }
        public string CDM { get; set; }
        public string BannerName { get; set; }
        public string PlantCode{ get; set; }
        public string PlantName{ get; set; }
        public string PCMap{ get; set; }
        public string DescriptionMap{ get; set; }
        public decimal MonthlyBucket{ get; set; }
        public decimal BucketWeek1{ get; set; }
        public decimal BucketWeek2{ get; set; }
        public decimal BucketWeek3{ get; set; }
        public decimal BucketWeek4{ get; set; }
        public decimal BucketWeek5{ get; set; }
        public decimal RatingRate{ get; set; }
        public decimal Tct{ get; set; }
        public decimal MonthlyTarget{ get; set; }
        public decimal PlantContribution{ get; set; }
        public decimal DispatchConsume{ get; set; }
        public decimal ValidBJ{ get; set; }
        public decimal RemFSA{ get; set; }
    }
}
