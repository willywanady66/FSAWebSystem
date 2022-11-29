namespace FSAWebSystem.Models.Bucket
{
    public class DailyOrder
    {
        public string BannerName {get;set;}
        public string PCMap {get;set;}
        public decimal OriginalOrder {get;set;}
        public decimal ValidBJ {get;set;}
        public int Month { get; set; }
        public int Year { get; set; }

        public Guid BannerId { get; set; }
    }
}
