using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Services.Job
{
    public class ReportDailyJob
    {

        private readonly FSAWebSystemDbContext _db;
        private readonly ICalendarService _calendarService;
        public ReportDailyJob(FSAWebSystemDbContext db, ICalendarService calendarService)
        {
            _db = db;
            _calendarService = calendarService;
        }

        public void Execute()
        {
            //var generateReportService = new ReportService(_db, _calendarService);
            ////generateReportService.GenerateWeeklyReport();
            //generateReportService.GenerateDailyReport();
        }
    }
}
