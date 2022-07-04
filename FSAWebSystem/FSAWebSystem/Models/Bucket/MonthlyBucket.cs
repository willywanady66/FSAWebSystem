namespace FSAWebSystem.Models.Bucket
{
    public class MonthlyBucket
    {
        public Guid Id { get; set; }
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
    }
}
