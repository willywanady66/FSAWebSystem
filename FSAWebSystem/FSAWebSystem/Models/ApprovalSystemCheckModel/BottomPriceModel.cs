namespace FSAWebSystem.Models.ApprovalSystemCheckModel
{
    public class BottomPriceModel
    {
        public Guid Id { get; set; }
        public Guid SKUId { get; set; }
        public string PCMap { get; set; }
        public string Description{ get; set; }
        public decimal AvgNormalPrice{ get; set; }
        public decimal AvgBottomPrice{ get; set; }
        public decimal AvgActualPrice{ get; set; }
        public decimal MinActualPrice{ get; set; }
        public decimal Gap{ get; set; }
        public decimal Remaks{ get; set; }
    }
}
