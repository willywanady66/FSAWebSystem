using System.ComponentModel.DataAnnotations;

namespace FSAWebSystem.Models
{
    public class ULICalendar
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<ULICalendarDetail> ULICalendarDetails { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
