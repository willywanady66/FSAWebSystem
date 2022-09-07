namespace FSAWebSystem.Models.ApprovalSystemCheckModel
{
    public class ITrustModel
    {
        public Guid Id { get; set; }
        public Guid SKUId { get; set; }
        public string PCMap { get; set; }
        public string Description { get; set; }
        public decimal SumIntransit { get; set; }
        public decimal SumStock { get; set; }
        public decimal SumFinalRpp { get; set; }
        public decimal DistStock { get; set; }
    }
}
