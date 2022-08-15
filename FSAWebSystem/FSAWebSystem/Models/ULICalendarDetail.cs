using System.ComponentModel.DataAnnotations;

namespace FSAWebSystem.Models
{
    public class ULICalendarDetail
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year{ get; set; }
        public int Week { get; set; }
        public Guid ULICalendarId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
}
