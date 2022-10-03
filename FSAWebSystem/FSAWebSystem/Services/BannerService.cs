using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class BannerService : IBannerService
    {
        private FSAWebSystemDbContext _db;

        public BannerService(FSAWebSystemDbContext db)
        {
            _db = db;
        }


        public async Task<BannerPagingData> GetBannersPagination(DataTableParam param)
        {
            var banners = _db.Banners.AsQueryable();
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                banners = banners.Where(x => x.BannerName.ToLower().Contains(search.ToLower()));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 1:
                        banners = order.dir == "desc" ? banners.OrderByDescending(x => x.BannerName) : banners.OrderBy(x => x.BannerName);
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

        public async Task<PlantPagingData> GetPlantsPagination(DataTableParam param)
        {
            var plants = _db.Plants.AsQueryable();
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                plants = plants.Where(x => x.PlantCode.ToLower().Contains(search.ToLower()) || x.PlantName.ToLower().Contains(search.ToLower()));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        plants = order.dir == "desc" ? plants.OrderByDescending(x => x.PlantCode) : plants.OrderBy(x => x.PlantCode);
                        break;
                    case 1:
                        plants = order.dir == "desc" ? plants.OrderByDescending(x => x.PlantName) : plants.OrderBy(x => x.PlantName);
                        break;
                }
            }

            var totalCount = plants.Count();
            var listPlant = await plants.Skip(param.start).Take(param.length).ToListAsync();
            return new PlantPagingData
            {
                totalRecord = totalCount,
                plants = listPlant
            };
        }
    }
}
