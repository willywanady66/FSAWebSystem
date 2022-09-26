using FSAWebSystem.Models;
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

        public async Task<MonthlyBucketHistoryPagingData> GetMonthlyBucketHistoryPagination(DataTableParam param, UserUnilever userUnilever)
        {
            var monthlyBucketHistories = (from monthlyBucket in _db.MonthlyBuckets
                                         join banner in _db.Banners on monthlyBucket.BannerId equals banner.Id
                                         join sku in _db.SKUs on monthlyBucket.SKUId equals sku.Id
                                         where monthlyBucket.Year == Convert.ToInt32(param.year) && monthlyBucket.Month == Convert.ToInt32(param.month)
                                         select new MonthlyBucket
                                         {
                                             BannerId = banner.Id,
                                             UploadedDate = monthlyBucket.CreatedAt.Value.ToString("dd/MM/yyyy"),
                                             CreatedAt = monthlyBucket.CreatedAt,
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
            if (userUnilever.RoleUnilever.RoleName != "Administrator")
            {
                monthlyBucketHistories = monthlyBucketHistories.Where(x => userUnilever.Banners.Select(y => y.Id).Contains(x.BannerId));
            }


            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                monthlyBucketHistories = monthlyBucketHistories.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search) || x.PlantName.ToLower().Contains(search));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.CreatedAt) : monthlyBucketHistories.OrderByDescending(x => x.CreatedAt);
                        break;
                    case 1:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.Year) : monthlyBucketHistories.OrderBy(x => x.Year);
                        break;
                    case 2:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.Month) : monthlyBucketHistories.OrderBy(x => x.Month);
                        break;
                    case 3:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.BannerName) : monthlyBucketHistories.OrderBy(x => x.BannerName);
                        break;
                    case 4:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.PlantName) : monthlyBucketHistories.OrderBy(x => x.PlantName);
                        break;
                    case 5:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.PCMap) : monthlyBucketHistories.OrderBy(x => x.PCMap);
                        break;
                    case 6:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.DescriptionMap) : monthlyBucketHistories.OrderBy(x => x.DescriptionMap);
                        break;
                    case 7:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.Price) : monthlyBucketHistories.OrderBy(x => x.Price);
                        break;
                    case 8:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.PlantContribution) : monthlyBucketHistories.OrderBy(x => x.PlantContribution);
                        break;
                    case 9:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.RatingRate) : monthlyBucketHistories.OrderBy(x => x.RatingRate);
                        break;
                    case 10:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.TCT) : monthlyBucketHistories.OrderBy(x => x.TCT);
                        break;
                    case 11:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.MonthlyTarget) : monthlyBucketHistories.OrderBy(x => x.MonthlyTarget);
                        break;
                    default:
                        monthlyBucketHistories = monthlyBucketHistories.OrderByDescending(x => x.CreatedAt);
                        break;

                }
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

        public async Task<WeeklyBucketHistoryPagingData> GetWeeklyBucketHistoryPagination(DataTableParam param, UserUnilever userUnilever)
        {
            var weeklyBucketHistories = (from weeklyBucketHistory in _db.WeeklyBucketHistories
                                          join banner in _db.Banners  on weeklyBucketHistory.BannerId equals banner.Id
                                          join sku in _db.SKUs on weeklyBucketHistory.SKUId equals sku.Id
                                          where weeklyBucketHistory.Year == Convert.ToInt32(param.year) && weeklyBucketHistory.Month == Convert.ToInt32(param.month)
                                          select new WeeklyBucketHistory
                                          {
                                              BannerId = banner.Id,
                                              CreatedAt = weeklyBucketHistory.CreatedAt,
                                              UploadedDate = weeklyBucketHistory.CreatedAt.Value.ToShortDateString(),
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

            if(userUnilever.RoleUnilever.RoleName != "Administrator")
            {
                weeklyBucketHistories = weeklyBucketHistories.Where(x => userUnilever.Banners.Select(y => y.Id).Contains(x.BannerId));
            }


            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        weeklyBucketHistories = order.dir == "desc" ? weeklyBucketHistories.OrderByDescending(x => x.CreatedAt) : weeklyBucketHistories.OrderByDescending(x => x.CreatedAt);
                        break;
                    case 1:
                        weeklyBucketHistories = order.dir == "desc" ? weeklyBucketHistories.OrderByDescending(x => x.Year) : weeklyBucketHistories.OrderBy(x => x.Year);
                        break;
                    case 2:
                        weeklyBucketHistories = order.dir == "desc" ? weeklyBucketHistories.OrderByDescending(x => x.Month) : weeklyBucketHistories.OrderBy(x => x.Month);
                        break;
                    case 3:
                        weeklyBucketHistories = order.dir == "desc" ? weeklyBucketHistories.OrderByDescending(x => x.Week) : weeklyBucketHistories.OrderBy(x => x.Week);
                        break;
                    case 5:
                        weeklyBucketHistories = order.dir == "desc" ? weeklyBucketHistories.OrderByDescending(x => x.BannerName) : weeklyBucketHistories.OrderBy(x => x.BannerName);
                        break;
                    case 6:
                        weeklyBucketHistories = order.dir == "desc" ? weeklyBucketHistories.OrderByDescending(x => x.PCMap) : weeklyBucketHistories.OrderBy(x => x.PCMap);
                        break;
                    case 7:
                        weeklyBucketHistories = order.dir == "desc" ? weeklyBucketHistories.OrderByDescending(x => x.DescriptionMap) : weeklyBucketHistories.OrderBy(x => x.DescriptionMap);
                        break;
                    case 8:
                        weeklyBucketHistories = order.dir == "desc" ? weeklyBucketHistories.OrderByDescending(x => x.DispatchConsume) : weeklyBucketHistories.OrderBy(x => x.DispatchConsume);
                        break;
                    default:
                        weeklyBucketHistories = weeklyBucketHistories.OrderByDescending(x => x.CreatedAt);
                        break;

                }
            }


            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                weeklyBucketHistories = weeklyBucketHistories.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search) || x.PlantName.ToLower().Contains(search));
            }

            var totalCount = weeklyBucketHistories.Count();
            var listWeeklyBucketHistory = await weeklyBucketHistories.Skip(param.start).Take(param.length).ToListAsync();
           
            foreach (var weeklyBucketHistory in listWeeklyBucketHistory)
            {
                var uliCalendarDetail = _db.ULICalendarDetails.SingleOrDefault(x => weeklyBucketHistory.CreatedAt >= x.StartDate.Value.Date && weeklyBucketHistory.CreatedAt <= x.EndDate);
                weeklyBucketHistory.ULIWeek = uliCalendarDetail != null ? uliCalendarDetail.Week.ToString() : string.Empty;
            }
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

        public async Task<bool> WeeklyBucketExist(int month, int week, int year)
        {
            var isWeeklyBucketExist = await _db.WeeklyBucketHistories.AnyAsync(x => x.Month == month && x.Week == week && x.Year == year);
            return isWeeklyBucketExist;
        }
    }
}
