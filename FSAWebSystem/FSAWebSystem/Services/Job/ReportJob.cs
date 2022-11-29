using FluentScheduler;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Services.Job
{
	public class ReportJob : IJob
	{

		private readonly FSAWebSystemDbContext _db;
		private readonly ICalendarService _calendarService;
		public ReportJob(FSAWebSystemDbContext db, ICalendarService calendarService)
		{
            _db = db;
		}

		public void Execute()
		{
			//var reportService = new ReportService(_db, _calendarService);
			//reportService.GenerateWeeklyReport();
		}
	}
}
