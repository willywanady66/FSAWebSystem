namespace FSAWebSystem.Models.ApprovalSystemCheckModel
{
    public class AndromedaModel
    {
        public string Id { get; set; }

        public Guid SKUId { get; set; }
        public string PCMap { get; set; }
        public string Description { get; set; }

        public decimal UUStock { get; set; }
        public decimal ITThisWeek{ get; set; }
        public decimal RRACT13Wk{ get; set; }
        public decimal WeekCover { get; set; }

    }
}
