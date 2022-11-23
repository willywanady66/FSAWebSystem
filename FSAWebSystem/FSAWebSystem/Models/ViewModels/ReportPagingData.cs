namespace FSAWebSystem.Models.ViewModels
{
	public class ReportPagingData : PagingData
	{

        public ReportPagingData()
        {
            reports = new List<Report>();
        }

        public List<Report> reports { get; set; }
    }
}
