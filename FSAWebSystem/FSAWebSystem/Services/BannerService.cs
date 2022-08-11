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

        public async Task<List<Banner>> GetUserBanners(Guid userId)
		{
            var bannerUsers = await _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)).ToListAsync();
            return bannerUsers;
        }

        public async Task FillBannerDropdown(ViewDataDictionary viewData)
        {
            var banners = GetAllBanner().ToList();
             
            var listBanner = banners.Select(x => new SelectListItem { Text = x.BannerName + " (" + x.PlantName + ") " + "(" + x.PlantCode + ")", Value = x.Id.ToString() }).ToList();
            viewData["ListBanner"] = listBanner;
        }

        public IQueryable<Banner> GetAllActiveBanner()
        {
            return _db.Banners.Where(x => x.IsActive);
        }

        public async Task<BannerPagingData> GetBannerPagination(DataTableParam param)
        {
            var banners = _db.Banners.AsQueryable();
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                banners = banners.Where(x => x.BannerName.ToLower().Contains(search) || x.Trade.ToLower().Contains(search) || x.PlantCode.ToLower().Contains(search) || x.PlantName.ToLower().Contains(search));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.Trade) : banners.OrderBy(x => x.Trade);
                        break;
                    case 1:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.CDM) : banners.OrderBy(x => x.CDM);
                        break;
                    case 2:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.KAM) : banners.OrderBy(x => x.KAM);
                        break;
                    case 3:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.BannerName) : banners.OrderBy(x => x.BannerName);
                        break;
                    case 4:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.PlantCode) : banners.OrderBy(x => x.PlantCode);
                        break;
                    case 5:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.PlantName) : banners.OrderBy(x => x.PlantName);
                        break;
                    
                }
            }


            var totalCount = banners.Count();
            var listBanner = await banners.Skip(param.start).Take(param.length).ToListAsync();
            return new BannerPagingData
            {
                totalRecord = totalCount,
                banners = listBanner
            };
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

        public async Task<bool> IsBannerUsed(string name, string plantCode)
        {
            var usedBanner = await _db.Banners.Where(x => x.IsActive).Include(x => x.UserUnilevers).SingleOrDefaultAsync(x => x.BannerName.ToUpper() == name.ToUpper() && x.PlantCode == plantCode);

            var usedBannerBucket =  _db.MonthlyBuckets.Where(x => x.BannerId == usedBanner.Id);
            var bannerUsed = usedBanner.UserUnilevers.Any() && usedBannerBucket.Any();
            return bannerUsed;

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
