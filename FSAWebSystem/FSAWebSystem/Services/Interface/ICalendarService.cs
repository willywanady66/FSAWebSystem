using FSAWebSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FSAWebSystem.Services.Interface
{
    public interface ICalendarService
    {
        public Task<FSACalendarHeader> GetFSACalendarHeader(int month, int year);
        public Task<ULICalendar> GetULICalendar(int month, int year);
        public Task<FSACalendarHeader> GetFSACalendarById(Guid id);
        public Task<FSACalendarDetail> GetCalendarDetail(DateTime date); 
        public List<SelectListItem> GetListMonth();
		public List<SelectListItem> GetListYear();
		public List<int> GetListYear2();

        public Task AddCalendar(FSACalendarHeader fsaCalendar);
        public void UpdateCalendar(FSACalendarHeader fsaCalendar);
    }
}
