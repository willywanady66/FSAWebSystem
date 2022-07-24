namespace FSAWebSystem.Models.ViewModels
{
    public class WorkLevelPagingData : PagingData
    {
        public WorkLevelPagingData()
        {
            workLevels = new List<WorkLevel>();
        }

        public List<WorkLevel> workLevels { get; set; }
    }
}
