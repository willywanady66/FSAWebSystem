using FSAWebSystem.Models;
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

        public Task<bool> DeleteBanner(Guid id)
        {
            throw new NotImplementedException();
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
            listBanner.ItemList = banners.Select(x => new SelectListItem { Text = x.BannerName + " (" + x.PlantName + ')', Value = x.Id.ToString() }).ToList();
            viewData["ListBanner"] = listBanner.ItemList;
        }

        public IQueryable<Banner> GetAllActiveBanner()
        {
            return _db.Banners.Where(x => x.IsActive);
        }

        public IQueryable<Banner> GetAllBanner()
        {
            return _db.Banners;
        }

        public async Task<Banner> GetBanner(Guid id)
        {
            return await _db.Banners.FindAsync(id);
        }

        public async Task<Banner> GetBannerByName(string name)
        {

            return await _db.Banners.SingleOrDefaultAsync(x => x.BannerName == name);
        }

        public async Task<bool> IsBannerExist(string name)
        {
            return await _db.Banners.Where(x => x.IsActive).AnyAsync(x => x.BannerName.ToUpper() == name.ToUpper());
        }

        public async Task<bool> IsBannerUsed(string name)
        {
            var usedBanner = await _db.Banners.Where(x => x.IsActive).Include(x => x.UserUnilevers).SingleOrDefaultAsync(x => x.BannerName.ToUpper() == name.ToUpper());
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

        public async Task SaveBanners(List<Banner> banners)
        {
            await _db.Banners.AddRangeAsync(banners);
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
