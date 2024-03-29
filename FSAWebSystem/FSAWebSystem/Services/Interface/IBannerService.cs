﻿using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FSAWebSystem.Services.Interface
{
    public interface IBannerService
    {
        public IQueryable<Banner> GetAllBanner();

        public Task<List<Banner>> GetUserBanners(Guid userId);
        public Task<BannerPagingData> GetBannerPagination(DataTableParam param);
        public IQueryable<Banner> GetAllActiveBanner();
        public Task<Banner> GetBanner(Guid id);
        public Task<Banner> SaveBanner(Banner banner, string user);
        public Task SaveBanners(List<Banner> banners);
        public Task<Banner> GetBannerByName (string name);
        public Task<bool> IsBannerExist (string name);

        public Task<Banner> UpdateBanner(Banner banner, string loggedUser);

        public Task FillBannerDropdown(ViewDataDictionary viewData);

        public Task<bool> IsBannerUsed(string name, string plantCode);
        public Task<bool> DeleteBanner(Guid id);
        //public Task DeleteUserBanner(List<Banner> userBanners);
    }
}
