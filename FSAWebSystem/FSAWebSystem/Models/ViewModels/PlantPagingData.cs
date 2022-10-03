namespace FSAWebSystem.Models.ViewModels
{
    public class PlantPagingData : PagingData
    {
        public PlantPagingData()
        {
            plants = new List<Plant>();
        }

        public List<Plant> plants { get; set; }
    }
}
