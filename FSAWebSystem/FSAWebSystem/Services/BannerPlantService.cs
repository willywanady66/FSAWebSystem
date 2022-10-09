using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class BannerPlantService : IBannerPlantService
    {
        public FSAWebSystemDbContext _db;

        public BannerPlantService(FSAWebSystemDbContext db)
        {
            _db = db;
        }

        public Task<bool> DeleteBannerPlant(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BannerPlant>> GetUserBannerPlants(Guid userId)
		{
            var bannerUsers = await _db.BannerPlants.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)).ToListAsync();
            return bannerUsers;
        }

        public async Task FillBannerPlantDropdown(ViewDataDictionary viewData)
        {
            var banners = GetAllBannerPlant().ToList();
             
            var listBanner = banners.Select(x => new SelectListItem { Text = x.Banner.BannerName + " (" + x.Plant.PlantName + ") " + "(" + x.Plant.PlantCode + ")", Value = x.Id.ToString() }).ToList();
            viewData["ListBanner"] = listBanner;
        }

        public IQueryable<BannerPlant> GetAllActiveBannerPlant()
        {
            var bannerPlants = _db.BannerPlants.Include(x => x.Plant).Include(x => x.Banner).Where(x => x.IsActive).AsQueryable();
            return bannerPlants;
        }

        public async Task<BannerPlantPagingData> GetBannerPlantPagination(DataTableParam param)
        {
            var banners = _db.BannerPlants.Include(x => x.Plant).Include(x => x.Banner).AsQueryable();
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                banners = banners.Where(x => x.Banner.BannerName.ToLower().Contains(search.ToLower()) || x.Banner.Trade.ToLower().Contains(search.ToLower()) || x.Plant.PlantCode.ToLower().Contains(search.ToLower()) || x.Plant.PlantName.ToLower().Contains(search.ToLower())
                                        || x.CDM.ToLower().Contains(search.ToLower()) || x.KAM.ToLower().Contains(search.ToLower()));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.Banner.Trade) : banners.OrderByDescending(x => x.IsActive).ThenBy(x => x.Banner.Trade);
                        break;
                    case 1:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.CDM) : banners.OrderBy(x => x.CDM);
                        break;
                    case 2:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.KAM) : banners.OrderBy(x => x.KAM);
                        break;
                    case 3:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.Banner.BannerName) : banners.OrderBy(x => x.Banner.BannerName);
                        break;
                    case 4:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.Plant.PlantCode) : banners.OrderBy(x => x.Plant.PlantCode);
                        break;
                    case 5:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.Plant.PlantName) : banners.OrderBy(x => x.Plant.PlantName);
                        break;
                    
                }
            }


            var totalCount = banners.Count();
            var listBanner = await banners.Skip(param.start).Take(param.length).ToListAsync();
            return new BannerPlantPagingData
            {
                totalRecord = totalCount,
                bannerPlants = listBanner
            };
        }

        public IQueryable<BannerPlant> GetAllBannerPlant()
        {
            return _db.BannerPlants.Include(x => x.Plant).Include(x => x.Banner);
        }

        public async Task<BannerPlant> GetBannerPlant(Guid id)
        {
            return await _db.BannerPlants.FindAsync(id);
        }

        public async Task<BannerPlant> GetBannerPlantByName(string name)
        {

            return await _db.BannerPlants.SingleOrDefaultAsync(x => x.Banner.BannerName == name);
        }

        public async Task<bool> IsBannerPlantExist(string name)
        {
            return await _db.BannerPlants.Where(x => x.IsActive).AnyAsync(x => x.Banner.BannerName.ToUpper() == name.ToUpper());
        }

        public async Task<bool> IsBannerPlantUsed(string name, Guid plantId)
        {
            var bannerUsed = false;
            var usedBanner = await _db.BannerPlants.Where(x => x.IsActive).Include(x => x.UserUnilevers).Include(x => x.Plant).Include(x => x.Banner).SingleOrDefaultAsync(x => x.Banner.BannerName.ToUpper() == name.ToUpper() && x.Plant.Id == plantId);

            
            if(usedBanner != null)
            {
                var usedBannerBucket = _db.MonthlyBuckets.Where(x => x.BannerId == usedBanner.Id);
                bannerUsed = usedBanner.UserUnilevers.Any() || usedBannerBucket.Any();
            }
            
            return bannerUsed;

        }

        public async Task<BannerPlant> SaveBannerPlant(BannerPlant banner, string user)
        {
            banner.Id = Guid.NewGuid();
            banner.CreatedAt = DateTime.Now;
            banner.CreatedBy = user;

             _db.BannerPlants.Add(banner);
            await _db.SaveChangesAsync();
            return banner;
        }

        public async Task SaveBannerPlants(List<BannerPlant> banners)
        {
            await _db.BannerPlants.AddRangeAsync(banners);
        }

        public async Task<BannerPlant> UpdateBannerPlant(BannerPlant banner, string loggedUser)
        {
            banner.ModifiedAt = DateTime.Now;
            banner.ModifiedBy = loggedUser;
            _db.Update(banner);
            await _db.SaveChangesAsync();
            return banner;
        }
    }
}
