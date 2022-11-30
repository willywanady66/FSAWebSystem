namespace FSAWebSystem.Models
{
	public class ReportWeekVersion
	{
		public Guid Id { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }
		public int Week { get; set; }
		public int MaxVersion { get; set; }
		
	}
}
