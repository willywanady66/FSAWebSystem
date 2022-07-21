namespace FSAWebSystem.Models.ViewModels
{
    public class BannerPagingData : PagingData
    {
        public BannerPagingData()
        {
            banners = new List<Banner>();
        }

        public List<Banner> banners { get; set; }
    }
}
