using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FSAWebSystem.Services
{
    public class CalendarService : ICalendarService
    {

        public FSAWebSystemDbContext _db;

        public CalendarService(FSAWebSystemDbContext db)
        {
            _db = db;
        }

        public async Task AddCalendar(FSACalendarHeader fsaCalendar)
        {
            await _db.AddAsync(fsaCalendar);
        }

        public void UpdateCalendar(FSACalendarHeader fSACalendar)
        {
            _db.Update(fSACalendar);
        }

        public async Task<FSACalendarDetail> GetCalendarDetail(DateTime date)
        {
            date = date.Date;
            var calendarDetail = await _db.FSACalendarDetails.SingleOrDefaultAsync(x => date >= x.StartDate.Value.Date && date <= x.EndDate.Value.Date && x.Month == date.Month);
            return calendarDetail;
        }

        public async Task<FSACalendarHeader> GetFSACalendarHeader(int month, int year)
        {
            var fsaCalendar = await _db.FSACalendarHeaders.Include(x => x.FSACalendarDetails.OrderBy(x => x.Week)).SingleOrDefaultAsync(x => x.Month == month && x.Year == year);
            return fsaCalendar;
        }

        public async Task<ULICalendar> GetULICalendar(int month, int year)
        {
            var uliCalendar = await _db.ULICalendars.Include(x => x.ULICalendarDetails.OrderBy(x => x.Week)).SingleOrDefaultAsync(x => x.Month == month && x.Year == year);
            return uliCalendar;
        }

        public IQueryable<ULICalendar> GetULICalendars()
        {
            return _db.ULICalendars.AsQueryable();
        }



        public IQueryable<ULICalendarDetail> GetULICalendarDetails()
        {
            return _db.ULICalendarDetails.AsQueryable();
        }


        public async Task<FSACalendarHeader> GetFSACalendarById(Guid id)
        {
            var fsaCalendar = await _db.FSACalendarHeaders.Include(x => x.FSACalendarDetails.OrderBy(x => x.Week)).SingleOrDefaultAsync(x => x.Id == id);
            return fsaCalendar;
        }

        public async Task<ULICalendar> GetULICalendarById(Guid id)
        {
            var uliCalendar = await _db.ULICalendars.Include(x => x.ULICalendarDetails.OrderBy(x => x.Week)).SingleOrDefaultAsync(x => x.Id == id);
            return uliCalendar;
        }

        public List<SelectListItem> GetListMonth()
        {

            var months = DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames.Where(x => !string.IsNullOrEmpty(x))
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = monthName
                       });
            List<SelectListItem> listMonths = months.ToList();
            listMonths.Single(x => x.Value == DateTime.Now.Month.ToString()).Selected = true;
            return listMonths;
        }

		public List<SelectListItem> GetListYear()
		{

            var thisYear = DateTime.Now.Year;
            var years = Enumerable.Range(thisYear - 2, 5).ToList();
            List<SelectListItem> listYears = years.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            listYears.Single(x => x.Value == DateTime.Now.Year.ToString()).Selected = true;
            return listYears;
		}   

        public List<int> GetListYear2()
        {
            var thisYear = DateTime.Now.Year;
            var years = Enumerable.Range(thisYear - 2, 5).ToList();
            return years;
        }
	}
}
