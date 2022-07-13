using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class ProposalService : IProposalService
    {
        private readonly FSAWebSystemDbContext _db;
        public ProposalService(FSAWebSystemDbContext db)
        {
            _db = db;
        }
        public async Task<ProposalData> GetProposalForView(int month, int year, int week, DataTableParam param, Guid userId)
        {
            var proposals = (from weeklyBucket in _db.WeeklyBuckets
                                join banner in _db.Banners.Include(x => x.UserUnilevers) on weeklyBucket.BannerId equals banner.Id
                                join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                                where weeklyBucket.Month == month && weeklyBucket.Year == year
                                && banner.UserUnilevers.Any(x => x.Id == userId)
                                select new Proposal
                                {
                                    Id = Guid.NewGuid(),
                                    WeeklyBucketId = weeklyBucket.Id,
                                    BannerName = banner.BannerName,
                                    PlantCode = banner.PlantCode,
                                    PlantName = banner.PlantName,
                                    PCMap = sku.PCMap,
                                    DescriptionMap = sku.DescriptionMap,
                                    RatingRate = weeklyBucket.RatingRate,
                                    MonthlyBucket = weeklyBucket.MonthlyBucket,
                                    CurrentBucket = (decimal)weeklyBucket.GetType().GetProperty("BucketWeek" + week.ToString()).GetValue(weeklyBucket, null),
                                    NextBucket = (decimal)weeklyBucket.GetType().GetProperty("BucketWeek" + (week+1).ToString()).GetValue(weeklyBucket, null)
                                });

           
            var totalCount = proposals.Count();
            var listProposal = proposals.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            return new ProposalData
            {
                TotalRecord = totalCount,
                Proposals = listProposal
            };
        }
    }
}
