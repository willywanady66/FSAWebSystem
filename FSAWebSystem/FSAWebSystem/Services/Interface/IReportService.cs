using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Services.Interface
{
	public interface IReportService
	{


		public Task<ReportPagingData> GetDailyReports(DataTableParam param);
		public Task<ReportPagingData> GetWeeklyReports(DataTableParam param);
		public Task GenerateDailyReport();
		public Task GenerateWeeklyReport();
		public Task<IEnumerable<object>> GenerateFirstReportOfMonth(int month, int year);	
    }
}
