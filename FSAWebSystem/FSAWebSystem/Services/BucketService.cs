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
            return _db.MonthlyBuckets.Include(x => x.BannerPlant);
        }

        public async Task<MonthlyBucketHistoryPagingData> GetMonthlyBucketHistoryPagination(DataTableParam param, UserUnilever userUnilever)
        {
            var monthlyBucketHistories = (from monthlyBucket in _db.MonthlyBuckets.Include(x => x.BannerPlant)
                                          join bannerPlant in _db.BannerPlants.Include(x => x.Plant) on monthlyBucket.BannerPlant.Id equals bannerPlant.Id
                                          join sku in _db.SKUs on monthlyBucket.SKUId equals sku.Id
                                          where monthlyBucket.Year == Convert.ToInt32(param.year) && monthlyBucket.Month == Convert.ToInt32(param.month)
                                          select new MonthlyBucket
                                          {
                                              UploadedDate = monthlyBucket.CreatedAt.Value.ToString("dd/MM/yyyy"),
                                              KAM = bannerPlant.KAM,
                                              CDM = bannerPlant.CDM,
                                              CreatedAt = monthlyBucket.CreatedAt,
                                              BannerName = bannerPlant.Banner.BannerName,
                                              BPlantId = bannerPlant.Id,
                                              SKUId = sku.Id,
                                              PCMap = sku.PCMap,
                                              DescriptionMap = sku.DescriptionMap,
                                              Year = monthlyBucket.Year,
                                              Month = monthlyBucket.Month,
                                              Price = monthlyBucket.Price,
                                              PlantContribution = monthlyBucket.PlantContribution,
                                              RatingRate = monthlyBucket.RatingRate,
                                              TCT = monthlyBucket.TCT,
                                              MonthlyTarget = monthlyBucket.MonthlyTarget
                                          }).AsEnumerable().GroupBy(x => new { x.KAM, x.CDM, x.BnrId, SKUId = x.SKUId }).Select(y => new MonthlyBucket
                                          {
                                              UploadedDate = y.First().UploadedDate,
                                              KAM = y.Key.KAM,
                                              CDM = y.Key.CDM,
                                              CreatedAt = y.First().CreatedAt,
                                              BannerName = y.First().BannerName,
                                              SKUId = y.Key.SKUId,
                                              PCMap = y.First().PCMap,
                                              DescriptionMap = y.First().DescriptionMap,
                                              Year = y.First().Year,
                                              Month = y.First().Month,
                                              Price = y.Sum(z => z.Price),
                                              PlantContribution = y.Sum(z => z.PlantContribution),
                                              RatingRate = y.Sum(z => z.RatingRate),
                                              TCT = y.Sum(z => z.TCT),
                                              MonthlyTarget = y.Sum(z => z.MonthlyTarget)
                                          });
                                         
            if (userUnilever.RoleUnilever.RoleName != "Administrator")
            {
                monthlyBucketHistories = monthlyBucketHistories.Where(x => userUnilever.BannerPlants.Select(y => y.Id).Contains(x.BPlantId));
            }


            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                monthlyBucketHistories = monthlyBucketHistories.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search));
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
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.PCMap) : monthlyBucketHistories.OrderBy(x => x.PCMap);
                        break;
                    case 5:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.DescriptionMap) : monthlyBucketHistories.OrderBy(x => x.DescriptionMap);
                        break;
                    case 6:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.Price) : monthlyBucketHistories.OrderBy(x => x.Price);
                        break;
                    case 7:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.PlantContribution) : monthlyBucketHistories.OrderBy(x => x.PlantContribution);
                        break;
                    case 8:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.RatingRate) : monthlyBucketHistories.OrderBy(x => x.RatingRate);
                        break;
                    case 9:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.TCT) : monthlyBucketHistories.OrderBy(x => x.TCT);
                        break;
                    case 10:
                        monthlyBucketHistories = order.dir == "desc" ? monthlyBucketHistories.OrderByDescending(x => x.MonthlyTarget) : monthlyBucketHistories.OrderBy(x => x.MonthlyTarget);
                        break;
                    default:
                        monthlyBucketHistories = monthlyBucketHistories.OrderByDescending(x => x.CreatedAt);
                        break;

                }
            }



            var totalCount = monthlyBucketHistories.Count();
            var listMonthlyBucketHistory = monthlyBucketHistories.Skip(param.start).Take(param.length).ToList();

            return new MonthlyBucketHistoryPagingData
            {
                totalRecord = totalCount,
                monthlyBuckets = listMonthlyBucketHistory
            };
        }


        public IQueryable<WeeklyBucket> GetWeeklyBuckets()
        {
            return _db.WeeklyBuckets.Include(x => x.BannerPlant).ThenInclude(x => x.Banner);
        }

        public async Task<WeeklyBucketHistoryPagingData> GetWeeklyBucketHistoryPagination(DataTableParam param, UserUnilever userUnilever)
        {
            var weeklyBucketHistories = (from weeklyBucketHistory in _db.WeeklyBucketHistories
                                          join bannerPlant in _db.BannerPlants.Include(x => x.Plant)  on weeklyBucketHistory.BannerPlantId equals bannerPlant.Id
                                          join sku in _db.SKUs on weeklyBucketHistory.SKUId equals sku.Id
                                          where weeklyBucketHistory.Year == Convert.ToInt32(param.year) && weeklyBucketHistory.Month == Convert.ToInt32(param.month)
                                          select new WeeklyBucketHistory
                                          {
                                              BannerPlantId = bannerPlant.Id,
                                              CreatedAt = weeklyBucketHistory.CreatedAt,
                                              UploadedDate = weeklyBucketHistory.CreatedAt.Value.ToShortDateString(),
                                              BannerName = bannerPlant.Banner.BannerName,
                                              PCMap = sku.PCMap,
                                              DescriptionMap = sku.DescriptionMap,
                                              PlantCode = bannerPlant.Plant.PlantCode,
                                              PlantName = bannerPlant.Plant.PlantName,
                                              Year = weeklyBucketHistory.Year,
                                              Month = weeklyBucketHistory.Month,
                                              Week = weeklyBucketHistory.Week,
                                              DispatchConsume = weeklyBucketHistory.DispatchConsume
                                          });

            if(userUnilever.RoleUnilever.RoleName != "Administrator")
            {
                weeklyBucketHistories = weeklyBucketHistories.Where(x => userUnilever.BannerPlants.Select(y => y.Id).Contains(x.BannerPlantId));
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
            var weeklyBucket = await _db.WeeklyBuckets.Include(x => x.BannerPlant).SingleOrDefaultAsync(x => x.Id == id);
            return weeklyBucket;
        }

        public async Task<IQueryable<WeeklyBucket>> GetWeeklyBucketsByBannerSKU(Guid bannerId, Guid skuId)
        {
            var weeklyBuckets = _db.WeeklyBuckets.Include(x => x.BannerPlant).Where(x => x.BannerPlant.Banner.Id == bannerId && x.SKUId == skuId);
            return weeklyBuckets;
        }

        public async Task<List<Guid>> GetWeeklyBucketBanners()
		{
            var weeklyBucketBanners = await _db.WeeklyBuckets.Include(x => x.BannerPlant).Select(x => x.BannerPlant.Id).Distinct().ToListAsync();
            return weeklyBucketBanners;
		}
		
        public async Task<WeeklyBucket> GetWeeklyBucketByBanner(Guid targetBanner, int year, int month)
		{
            var weeklyBucket = await _db.WeeklyBuckets.Include(x => x.BannerPlant).SingleAsync(x => x.BannerPlant.Id == targetBanner && x.Month == month && x.Year == year);
            return weeklyBucket;
        }

        public async Task<bool> WeeklyBucketExist(int month, int week, int year)
        {
            var isWeeklyBucketExist = await _db.WeeklyBucketHistories.AnyAsync(x => x.Month == month && x.Week == week && x.Year == year);
            return isWeeklyBucketExist;
        }


        public async Task<ProposeAddtionalBucket> GetWeeklyBucketSource(IQueryable<BannerPlant> bannerPlants, IQueryable<SKU> skus, decimal proposeAdditional, Proposal proposal, int month, int year, ProposalType? proposalType = null)
        {
            var i = 0;
            if(proposalType != null)
            {
                if (proposalType == ProposalType.ReallocateAcrossKAM)
                {
                    i = 1;
                }
                else if (proposalType == ProposalType.ReallocateAcrossCDM)
                {
                    i = 2;
                }
                else
                {
                    i = 3;
                    proposal.Type = ProposalType.ProposeAdditional;
                }
            }
          
            var bannerPlantSourceIds = new List<Guid>();
            var weeklyBucketSource = new WeeklyBucket();
            var weeklyBucketTargets = (await GetWeeklyBucketsByBannerSKU(proposal.Banner.Id, proposal.Sku.Id)).Where(x => x.Month == month && x.Year == year);
            var proposeAdditionalBucket = new ProposeAddtionalBucket();
            proposeAdditionalBucket.WeeklyBucketTargets = weeklyBucketTargets.ToList();

            var targetBannerPlants = weeklyBucketTargets.Select(y => y.BannerPlant.Id);
            var weeklyBuckets = GetWeeklyBuckets().Where(x => x.Month == month && x.Year == year);
            while (i != 3)
            {

                switch (i)
                {
                    //REALLOCATE ACROSS KAM
                    case 0:
                        bannerPlantSourceIds = await bannerPlants.Where(x => x.IsActive && x.CDM == proposal.CDM && x.KAM == proposal.KAM && !targetBannerPlants.Contains(x.Id) && x.Banner.Id != proposal.Banner.Id).Select(x => x.Id).ToListAsync();

                        proposal.Type = ProposalType.ReallocateAcrossKAM;

                        //bucketTargetIds = await bannerPlants.Where(x => x.KAM == bannerPlant.KAM && x.CDM == bannerPlant.CDM).Select(x => x.Id).ToListAsync();
                        //weeklyBucketTargets = _bucketService.GetWeeklyBuckets().Where(x => bucketTargetIds.Contains(x.BannerPlant.Id) && x.SKUId == sku.Id && x.Month == month && x.Year == year);
                        //proposal.Type = ProposalType.ReallocateAcrossKAM;
                        break;
                    //REALLOCATE ACROSS CDM
                    case 1:
                        bannerPlantSourceIds = await bannerPlants.Where(x => x.IsActive && x.CDM == proposal.CDM && !targetBannerPlants.Contains(x.Id) && x.Banner.Id != proposal.Banner.Id).Select(x => x.Id).ToListAsync();

                        proposal.Type = ProposalType.ReallocateAcrossCDM;
                        break;
                    //REALLOCATE ACROSS MT
                    case 2:
                        bannerPlantSourceIds = await bannerPlants.Where(x => x.IsActive && !targetBannerPlants.Contains(x.Id) && x.Banner.Id != proposal.Banner.Id).Select(x => x.Id).ToListAsync();
                        proposal.Type = ProposalType.ReallocateAcrossMT;
                        break;
                    default:
                        proposal.Type = ProposalType.ProposeAdditional;
                        break;
                }

                if (bannerPlantSourceIds.Any())
                {
                    var weeklyBucketSources = weeklyBuckets.AsEnumerable().Where(x => bannerPlantSourceIds.Contains(x.BannerPlant.Id) && x.SKUId == proposal.Sku.Id);

                    var weeklyBucketSouceGroups = weeklyBucketSources.GroupBy(x => new { x.BannerPlant.CDM, x.BannerPlant.KAM, x.BannerPlant.Banner.Id }).ToList();

                   
                    var groupedWeeklyBucketSources = new List<WeeklyBucket>();
                    foreach (var weeklyBucketSourceGroup in weeklyBucketSouceGroups)
                    {
                        var groupedWeeklyBucketSource = new WeeklyBucket();
                        groupedWeeklyBucketSource.CDM = weeklyBucketSourceGroup.Key.CDM;
                        groupedWeeklyBucketSource.KAM = weeklyBucketSourceGroup.Key.KAM;
                        groupedWeeklyBucketSource.Banner = weeklyBucketSourceGroup.First().BannerPlant.Banner;
                        groupedWeeklyBucketSource.MonthlyBucket = weeklyBucketSourceGroup.Sum(x => x.MonthlyBucket);
                        groupedWeeklyBucketSource.RemFSA = weeklyBucketSourceGroup.Sum(x => x.RemFSA);
                        groupedWeeklyBucketSources.Add(groupedWeeklyBucketSource);
                    }
                    if (!groupedWeeklyBucketSources.Any())
                    {
                        i++;
                    }
                    else
                    {
                        var weeklyBucketSourceByMonthly = groupedWeeklyBucketSources.Where(x => x.MonthlyBucket > proposeAdditional).OrderByDescending(x => x.MonthlyBucket).FirstOrDefault();
                        var weeklyBucketSourceByRemFSA = groupedWeeklyBucketSources.Where(x => x.RemFSA > proposeAdditional).OrderByDescending(x => x.RemFSA).FirstOrDefault();
                        if (weeklyBucketSourceByMonthly == null && weeklyBucketSourceByRemFSA == null)
                        {
                            proposal.Type = ProposalType.ProposeAdditional;
                            i++;
                        }
                        else if (weeklyBucketSourceByMonthly != null && weeklyBucketSourceByRemFSA != null)
                        {
                            if (weeklyBucketSourceByMonthly.MonthlyBucket > weeklyBucketSourceByRemFSA.RemFSA)
                            {
                                weeklyBucketSource = weeklyBucketSourceByMonthly;
                            }
                            else
                            {
                                weeklyBucketSource = weeklyBucketSourceByRemFSA;
                            }
                        }
                        else
                        {
                            weeklyBucketSource = weeklyBucketSourceByMonthly != null ? weeklyBucketSourceByMonthly : weeklyBucketSourceByRemFSA;

                        }
                        proposeAdditionalBucket.GroupedBucket = weeklyBucketSource;
                        proposeAdditionalBucket.WeeklyBucketSource = weeklyBucketSources.Where(x=> x.BannerPlant.CDM == weeklyBucketSource.CDM && x.BannerPlant.KAM == weeklyBucketSource.KAM && x.BannerPlant.Banner.Id == weeklyBucketSource.Banner.Id).ToList();
                        break;
                    }

                }
                else
                {
                    i++;
                }
            }
            return proposeAdditionalBucket;
        }
    }
}
