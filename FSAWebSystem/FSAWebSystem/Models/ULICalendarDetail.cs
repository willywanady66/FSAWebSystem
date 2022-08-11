using System.ComponentModel.DataAnnotations;

namespace FSAWebSystem.Models
{
    public class ULICalendarDetail
    {
        public Guid Id { get; set; }

        public int Week { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
}
