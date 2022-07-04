namespace FSAWebSystem.Models
{
    public class FSACalendarHeader
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<FSACalendarDetail> FSACalendarDetails { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy{ get; set; }
        public DateTime? ModifiedAt{ get; set; }
        public string? ModifiedBy{ get; set; }
    }
}
