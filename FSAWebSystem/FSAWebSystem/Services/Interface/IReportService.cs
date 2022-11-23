using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Services.Interface
{
	public interface IReportService
	{


		public Task<ReportPagingData> GetDailyReports(DataTableParam param);
		public Task<ReportPagingData> GetWeeklyReports(DataTableParam param);
    }
}
