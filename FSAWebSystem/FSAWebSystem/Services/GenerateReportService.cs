using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Services
{
    public class GenerateReportService : IGenerateReportService
    {
        public FSAWebSystemDbContext _db;

        public GenerateReportService(FSAWebSystemDbContext db)
        {
            _db = db;
        }
        public void GenerateWeeklyReport()
        {
            var report = new Report();
            var currDate = DateTime.Now;
            report.Id = Guid.NewGuid();
            report.Name = "FSA";
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
            report.Name = "Test";
            report.PublishedDate = DateTime.Now;
            report.Type = ReportType.Daily;
            report.Status = "Published";
            _db.Reports.Add(report);
            _db.SaveChanges();
        }

    }
}
