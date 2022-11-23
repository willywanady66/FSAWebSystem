using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
	public class ReportService : IReportService
	{
		public FSAWebSystemDbContext _db;
		private readonly ICalendarService _calendarService;

		public ReportService(FSAWebSystemDbContext db, ICalendarService calendarService)
		{
			_db = db;
			_calendarService = calendarService;
		}

        public void GenerateWeeklyReport()
		{
			var report = new Report();
			var currDate = DateTime.Now;
			var fsaCalendar = _calendarService.GetULICalendarDetails().SingleOrDefault(x => currDate >= x.StartDate.Value.Date && currDate <= x.EndDate);

		

			report.Id = Guid.NewGuid();
			report.Name = "FSA_WEEK_" + (fsaCalendar != null ? fsaCalendar.Week.ToString() : "") + "_" + currDate.Year ;
			report.PublishedDate = DateTime.Now;
			report.Type = ReportType.Weekly;
			report.Status = "Published";
			_db.Reports.Add(report);
			_db.SaveChanges();
		}

		public void GenerateDailyReport()
		{
            var report = new Report();
            var currDate = DateTime.Now;
            report.Id = Guid.NewGuid();
            report.Name = "FSA_" + currDate.Month.ToString() + "_" + currDate.Day.ToString() + "_" + currDate.Year.ToString();
            report.PublishedDate = DateTime.Now;
            report.Type = ReportType.Daily;
            report.Status = "Published";
            _db.Reports.Add(report);
            _db.SaveChanges();
        }

		public async Task<ReportPagingData> GetDailyReports(DataTableParam param)
		{
			var reports = _db.Reports.Where(x => x.Type == ReportType.Daily);
			var listReports = await reports.Skip(param.start).Take(param.length).ToListAsync();
			return new ReportPagingData
			{
				totalRecord = reports.Count(),
				reports = reports.ToList()
			};
		}

		public async Task<ReportPagingData> GetWeeklyReports(DataTableParam param)
		{
            var reports = _db.Reports.Where(x => x.Type == ReportType.Weekly);
            var listReports = await reports.Skip(param.start).Take(param.length).ToListAsync();
            return new ReportPagingData
            {
                totalRecord = reports.Count(),
                reports = reports.ToList()
            };
        }
	}
}
