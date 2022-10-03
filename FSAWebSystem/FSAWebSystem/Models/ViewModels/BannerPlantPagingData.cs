namespace FSAWebSystem.Models.ViewModels
{
    public class BannerPlantPagingData : PagingData
    {
        public BannerPlantPagingData()
        {
            bannerPlants = new List<BannerPlant>();
        }

        public List<BannerPlant> bannerPlants { get; set; }
    }
}
