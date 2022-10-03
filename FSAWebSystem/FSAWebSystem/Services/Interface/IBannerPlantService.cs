using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FSAWebSystem.Services.Interface
{
    public interface IBannerPlantService
    {
        public IQueryable<BannerPlant> GetAllBannerPlant();

        public Task<List<BannerPlant>> GetUserBannerPlants(Guid userId);
        public Task<BannerPlantPagingData> GetBannerPlantPagination(DataTableParam param);
        public IQueryable<BannerPlant> GetAllActiveBannerPlant();
        public Task<BannerPlant> GetBannerPlant(Guid id);
        public Task<BannerPlant> SaveBannerPlant(BannerPlant banner, string user);
        public Task SaveBannerPlants(List<BannerPlant> banners);
        public Task<BannerPlant> GetBannerPlantByName(string name);
        public Task<bool> IsBannerPlantExist(string name);

        public Task<BannerPlant> UpdateBannerPlant(BannerPlant banner, string loggedUser);

        public Task FillBannerPlantDropdown(ViewDataDictionary viewData);

        public Task<bool> IsBannerPlantUsed(string name, Guid plantId);
        public Task<bool> DeleteBannerPlant(Guid id);
        //public Task DeleteUserBanner(List<Banner> userBanners);
    }
}
