using FluentScheduler;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Services.Job
{
	public class ReportWeeklyJob : IJob
	{

		private readonly FSAWebSystemDbContext _db;
		private readonly ICalendarService _calendarService;
		public ReportWeeklyJob(FSAWebSystemDbContext db, ICalendarService calendarService)
		{
            _db = db;
			_calendarService = calendarService;
		}

		public void Execute()
		{
			var generateReportService = new ReportService(_db, _calendarService);
            generateReportService.GenerateWeeklyReport();
            //generateReportService.GenerateDailyReport();
		}
	}
}
