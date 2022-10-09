using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FSAWebSystem.Services.Interface
{
    public interface IBannerService
    {
        public Task<BannerPagingData> GetBannersPagination(DataTableParam param);
        public Task<PlantPagingData> GetPlantsPagination(DataTableParam param);
        public IQueryable<Banner> GetAllBanner();
        public IQueryable<Plant> GetAllPlant();

        public Task SaveBanners(List<Banner> banners);
        public Task SavePlants(List<Plant> plants);
        public Task FillBannerDropdown(ViewDataDictionary viewData);
        public Task FillPlantDropdown(ViewDataDictionary viewData);
    }
}
