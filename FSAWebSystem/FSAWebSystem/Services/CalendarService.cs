using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class CalendarService : ICalendarService
    {

        public FSAWebSystemDbContext _db;

        public CalendarService(FSAWebSystemDbContext db)
        {
            _db = db;
        }

        public async Task<FSACalendarHeader> GetFSACalendarHeader(int month, int year)
        {
            var fsaCalendar = await _db.FSACalendarHeader.Include(x => x.FSACalendarDetails.Where(x => x.StartDate.HasValue && x.EndDate.HasValue).OrderBy(x => x.Week)).SingleAsync(x => x.Month == month && x.Year == year);
            return fsaCalendar;
        }
    }
}
