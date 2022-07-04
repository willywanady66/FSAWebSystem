using FSAWebSystem.Models;

namespace FSAWebSystem.Services.Interface
{
    public interface ICalendarService
    {
        public Task<FSACalendarHeader> GetFSACalendarHeader(int month, int year);
    }
}
