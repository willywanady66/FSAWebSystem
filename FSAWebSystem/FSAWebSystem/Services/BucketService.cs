using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class BucketService : IBucketService
    {
        private FSAWebSystemDbContext _db;
        public BucketService(FSAWebSystemDbContext db)
        {
            _db = db;
        }
        public IQueryable<MonthlyBucket> GetMonthlyBuckets()
        {
            return _db.MonthlyBuckets;
        }

        public async Task<MonthlyBucketHistoryPagingData> GetMonthlyBucketHistoryPagination(DataTableParam param, Guid userId)
        {
            var monthlyBucketHistories = (from monthlyBucket in _db.MonthlyBuckets
                                         join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on monthlyBucket.BannerId equals banner.Id
                                         join sku in _db.SKUs on monthlyBucket.SKUId equals sku.Id
                                         where monthlyBucket.Year == Convert.ToInt32(param.year) && monthlyBucket.Month == Convert.ToInt32(param.month)
                                         select new MonthlyBucket
                                         {
                                             UploadedDate = monthlyBucket.CreatedAt.Value.ToString("dd/MM/yyyy"),
                                             BannerName = banner.BannerName,
                                             PCMap = sku.PCMap,
                                             DescriptionMap = sku.DescriptionMap,
                                             PlantCode = banner.PlantCode,
                                             PlantName = banner.PlantName,
                                             Year = monthlyBucket.Year,
                                             Month = monthlyBucket.Month,
                                             Price = monthlyBucket.Price,
                                             PlantContribution = monthlyBucket.PlantContribution,
                                             RatingRate = monthlyBucket.RatingRate,
                                             TCT = monthlyBucket.TCT,
                                             MonthlyTarget = monthlyBucket.MonthlyTarget
                                         });

            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                monthlyBucketHistories = monthlyBucketHistories.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search) || x.PlantName.ToLower().Contains(search));
            }

            var totalCount = monthlyBucketHistories.Count();
            var listMonthlyBucketHistory = await monthlyBucketHistories.Skip(param.start).Take(param.length).ToListAsync();
            return new MonthlyBucketHistoryPagingData
            {
                totalRecord = totalCount,
                monthlyBuckets = listMonthlyBucketHistory
            };
        }


        public IQueryable<WeeklyBucket> GetWeeklyBuckets()
        {
            return _db.WeeklyBuckets;
        }

        public async Task<WeeklyBucketHistoryPagingData> GetWeeklyBucketHistoryPagination(DataTableParam param, Guid userId)
        {
            var weeklyBucketHistories = (from weeklyBucketHistory in _db.WeeklyBucketHistories
                                          join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucketHistory.BannerId equals banner.Id
                                          join sku in _db.SKUs on weeklyBucketHistory.SKUId equals sku.Id
                                          where weeklyBucketHistory.Year == Convert.ToInt32(param.year) && weeklyBucketHistory.Month == Convert.ToInt32(param.month)
                                          select new WeeklyBucketHistory
                                          {
                                              UploadedDate = weeklyBucketHistory.CreatedAt.Value.ToString("dd/MM/yyyy"),
                                              BannerName = banner.BannerName,
                                              PCMap = sku.PCMap,
                                              DescriptionMap = sku.DescriptionMap,
                                              PlantCode = banner.PlantCode,
                                              PlantName = banner.PlantName,
                                              Year = weeklyBucketHistory.Year,
                                              Month = weeklyBucketHistory.Month,
                                              Week = weeklyBucketHistory.Week,
                                              DispatchConsume = weeklyBucketHistory.DispatchConsume
                                          });
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                weeklyBucketHistories = weeklyBucketHistories.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search) || x.PlantName.ToLower().Contains(search));
            }

            var totalCount = weeklyBucketHistories.Count();
            var listWeeklyBucketHistory = await weeklyBucketHistories.Skip(param.start).Take(param.length).ToListAsync();
            return new WeeklyBucketHistoryPagingData
            {
                totalRecord = totalCount,
                weeklyBucketHistories = listWeeklyBucketHistory
            };
        }

        public async Task<WeeklyBucket> GetWeeklyBucket(Guid id)
        {
            var weeklyBucket = await _db.WeeklyBuckets.SingleOrDefaultAsync(x => x.Id == id);
            return weeklyBucket;
        }

        public async Task<List<Guid>> GetWeeklyBucketBanners()
		{
            var weeklyBucketBanners = await _db.WeeklyBuckets.Select(x => x.BannerId).Distinct().ToListAsync();
            return weeklyBucketBanners;
		}
		
        public async Task<WeeklyBucket> GetWeeklyBucketByBanner(Guid targetBanner, int year, int month)
		{
            var weeklyBucket = await _db.WeeklyBuckets.SingleAsync(x => x.BannerId == targetBanner && x.Month == month && x.Year == year);
            return weeklyBucket;
        }
    }
}
