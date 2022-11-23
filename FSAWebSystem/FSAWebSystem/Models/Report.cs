namespace FSAWebSystem.Models
{
	public class Report
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTime PublishedDate { get; set; }
		public string Status { get; set; }
		public ReportType Type { get; set; }
	}


	public enum ReportType { 
		Daily = 1,
		Weekly
	}
}
