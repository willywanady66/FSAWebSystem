using FluentScheduler;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Job;

namespace FSAWebSystem.Services
{
	public class ReportRegistry : Registry
	{
        public ReportRegistry(IServiceProvider sp)
		{
            Schedule(() => sp.CreateScope().ServiceProvider.GetService<ReportDailyJob>()).ToRunEvery(1).Days().At(0,0);
            Schedule(() => sp.CreateScope().ServiceProvider.GetService<ReportWeeklyJob>()).ToRunEvery(1).Weeks().On(DayOfWeek.Saturday).At(0,0);
		}
	}


}
