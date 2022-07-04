﻿using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class BannerService : IBannerService
    {
        public FSAWebSystemDbContext _db;

        public BannerService(FSAWebSystemDbContext db)
        {
            _db = db;
        }

        //public async Task DeleteUserBanners(List<Banner> userBanners)
        //{
        //    _db.Banners.RemoveRange(userBanners);
        //    await _db.SaveChangesAsync();
           
        //}

        public async Task FillBannerDropdown(ViewDataDictionary viewData)
        {
            var banners = GetAllBanner().ToList();
            MultiDropDownListViewModel listBanner = new MultiDropDownListViewModel();
            listBanner.ItemList = banners.Select(x => new SelectListItem { Text = x.BannerName, Value = x.Id.ToString() }).ToList();
            viewData["ListBanner"] = listBanner.ItemList;
        }

        public IQueryable<Banner> GetAllBanner()
        {
            return _db.Banners.Where(x => x.IsActive);
        }

        public async Task<Banner> GetBanner(Guid id)
        {
            return await _db.Banners.FindAsync(id);
        }

        public Banner GetBannerByName(string name)
        {

            throw new NotImplementedException();
        }

        public async Task<bool> IsBannerExist(string name)
        {
            return await _db.Banners.AnyAsync(x => x.BannerName.ToUpper() == name.ToUpper());
        }

        public async Task<bool> IsBannerUsed(string name)
        {
            var usedBanner = await _db.Banners.Include(x => x.UserUnilevers).SingleOrDefaultAsync(x => x.BannerName.ToUpper() == name.ToUpper());
            return usedBanner.UserUnilevers.Any();

        }

        public async Task<Banner> SaveBanner(Banner banner, string user)
        {
            banner.Id = Guid.NewGuid();
            banner.CreatedAt = DateTime.Now;
            banner.CreatedBy = user;

             _db.Banners.Add(banner);
            await _db.SaveChangesAsync();
            return banner;
        }

        public async Task<Banner> UpdateBanner(Banner banner, string loggedUser)
        {
            banner.ModifiedAt = DateTime.Now;
            banner.ModifiedBy = loggedUser;
            _db.Update(banner);
            await _db.SaveChangesAsync();
            return banner;
        }
    }
}
