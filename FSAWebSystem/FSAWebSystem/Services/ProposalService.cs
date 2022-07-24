using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using static FSAWebSystem.Models.ViewModels.ProposalViewModel;

namespace FSAWebSystem.Services
{
    public class ProposalService : IProposalService
    {
        private readonly FSAWebSystemDbContext _db;
        public ProposalService(FSAWebSystemDbContext db)
        {
            _db = db;
        }

        //public async Task<ProposalData> GetProposalForView(int month, int year, int week, int pageNo, Guid userId)
        //{
        //    var proposals = (from weeklyBucket in _db.WeeklyBuckets
        //                     join banner in _db.Banners.Include(x => x.UserUnilevers) on weeklyBucket.BannerId equals banner.Id
        //                     join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
        //                     where weeklyBucket.Month == month && weeklyBucket.Year == year
        //                     && banner.UserUnilevers.Any(x => x.Id == userId)
        //                     select new Proposal
        //                     {
        //                         Id = Guid.NewGuid(),
        //                         WeeklyBucketId = weeklyBucket.Id,
        //                         BannerName = banner.BannerName,
        //                         PlantCode = banner.PlantCode,
        //                         PlantName = banner.PlantName,
        //                         PCMap = sku.PCMap,
        //                         DescriptionMap = sku.DescriptionMap,
        //                         RatingRate = weeklyBucket.RatingRate,
        //                         MonthlyBucket = weeklyBucket.MonthlyBucket,
        //                         CurrentBucket = (decimal)weeklyBucket.GetType().GetProperty("BucketWeek" + week.ToString()).GetValue(weeklyBucket, null),
        //                         NextBucket = (decimal)weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).GetValue(weeklyBucket, null)
        //                     });


        //    var totalCount = proposals.Count();
        //    var listProposal = proposals.Skip((pageNo - 1) * 20).Take(20).ToList();
        //    return new ProposalData
        //    {
        //        PageNo = pageNo,
        //        PageSize = 20,
        //        TotalRecord = totalCount,
        //        Proposals = listProposal
        //    };
        //}

        public async Task<ProposalData> GetProposalForView(int month, int year, int week, DataTableParamProposal param, Guid userId)
        {

            var proposals = (from weeklyBucket in _db.WeeklyBuckets
                             join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                             join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                             join proposal in _db.Proposals.Where(x => x.SubmittedBy == userId) on weeklyBucket.Id equals proposal.WeeklyBucketId into proposalGroups
                             from p in proposalGroups.DefaultIfEmpty()
                             where weeklyBucket.Month == month && weeklyBucket.Year == year
                             select new Proposal
                             {
                                 Id = p != null ? p.Id : Guid.Empty,
                                 WeeklyBucketId = weeklyBucket.Id,
                                 BannerName = banner.BannerName,
                                 Month = month,
                                 Week = week,
                                 Year = year,
                                 PlantCode = banner.PlantCode,
                                 PlantName = banner.PlantName,
                                 PCMap = sku.PCMap,
                                 DescriptionMap = sku.DescriptionMap,
                                 RatingRate = weeklyBucket.RatingRate,
                                 MonthlyBucket = weeklyBucket.MonthlyBucket,
                                 ValidBJ = weeklyBucket.ValidBJ,
                                 RemFSA = weeklyBucket.MonthlyBucket - weeklyBucket.ValidBJ,
                                 CurrentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + week.ToString()).GetValue(weeklyBucket, null)),
                                 NextBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).GetValue(weeklyBucket, null)),
                                 Remark = p != null ? p.Remark : string.Empty,
                                 Rephase = p != null ? p.Rephase : decimal.Zero,
                                 RejectionReason = p != null ? p.RejectionReason : string.Empty,
                                 ProposeAdditional = p != null ? p.ProposeAdditional : decimal.Zero,
                                 ApprovalStatus = p != null ? p.ApprovalStatus : ApprovalStatus.Pending
                             });

            proposals = proposals.Where(x => x.ApprovalStatus == ApprovalStatus.Pending);
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                proposals = proposals.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search));
            }
            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.BannerName) : proposals.OrderBy(x => x.BannerName);
                        break;
                    case 1:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.PlantName) : proposals.OrderBy(x => x.PlantName);
                        break;
                    case 2:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.PCMap) : proposals.OrderBy(x => x.PCMap);
                        break;
                    case 3:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.DescriptionMap) : proposals.OrderBy(x => x.DescriptionMap);
                        break;
                    case 4:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.RatingRate) : proposals.OrderBy(x => x.RatingRate);
                        break;
                    case 5:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.MonthlyBucket) : proposals.OrderBy(x => x.MonthlyBucket);
                        break;
                    case 6:
                        proposals = (order.dir == "desc" ? proposals.AsEnumerable().OrderByDescending(x => x.CurrentBucket).AsQueryable() : proposals.AsEnumerable().OrderBy(x => x.CurrentBucket).AsQueryable());
                        break;
                    case 7:
                        proposals = (order.dir == "desc" ? proposals.AsEnumerable().OrderByDescending(x => x.NextBucket).AsQueryable() : proposals.AsEnumerable().OrderBy(x => x.NextBucket).AsQueryable());
                        break;
                    case 8:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.ValidBJ) : proposals.OrderBy(x => x.ValidBJ);
                        break;
                    case 9:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.RemFSA) : proposals.OrderBy(x => x.RemFSA);
                        break;
                }
            }


            var totalCount = proposals.Count();
            var listProposal = proposals.Skip(param.start).Take(param.length).ToList();
            return new ProposalData
            {
                proposalInputs = param.proposalInputs,
                totalRecord = totalCount,
                proposals = listProposal
            };
        }

        public ProposalHistoryPagingData GetProposalHistoryPagination(DataTableParam param, Guid userId)
        {
            var proposalsHistory = (from proposal in _db.Proposals
                                    join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
                                                          join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                                                          join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                                                          select new WeeklyBucket
                                                          {
                                                              Id = weeklyBucket.Id,
                                                              BannerName = banner.BannerName,
                                                              PlantName = banner.PlantName,
                                                              PCMap = sku.PCMap,
                                                              DescriptionMap = sku.DescriptionMap
                                                          }) on proposal.WeeklyBucketId equals weeklyBucket.Id
                                    where proposal.ApprovalStatus != ApprovalStatus.Pending
                                    && proposal.SubmittedBy == userId
                                    select new ProposalHistory
                                    {
                                        SubmitDate = proposal.SubmittedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                        BannerName = weeklyBucket.BannerName,
                                        PlantName = weeklyBucket.PlantName,
                                        PCMap = weeklyBucket.PCMap,
                                        DescriptionMap = weeklyBucket.DescriptionMap,
                                        Rephase = proposal.Rephase,
                                        ApprovedRephase = 0,
                                        ProposeAdditional = proposal.ProposeAdditional,
                                        ApprovedProposeAdditional = 0,
                                        //Remark = proposal.Remark,
                                        Status = proposal.ApprovalStatus.ToString(),
                                        ApprovedBy = proposal.ApprovedBy,
                                        RejectionReason = proposal.RejectionReason
                                    });

            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                proposalsHistory = proposalsHistory.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search) || x.Status.ToLower().Contains(search));
            }
            var totalCount = proposalsHistory.Count();
            var listProposalHistory = proposalsHistory.Skip(param.start).Take(param.length).ToList();
            return new ProposalHistoryPagingData
            {
                totalRecord = totalCount,
                proposalsHistory = listProposalHistory
            };


        }

        public async Task<bool> IsProposalExist(FSACalendarDetail fsaDetail)
        {
            return await _db.Proposals.AnyAsync(x => x.Week == fsaDetail.Week && x.Month == fsaDetail.Month && x.Year == fsaDetail.Year);
        }

        public async Task SaveProposals(List<Proposal> listProposal)
        {
            await _db.AddRangeAsync(listProposal);
        }

        public IQueryable<Proposal> GetPendingProposals(FSACalendarDetail fsaDetail, Guid userId)
        {
            var listProposals = _db.Proposals.Where(x => x.Week == fsaDetail.Week && x.Month == fsaDetail.Month && x.Year == fsaDetail.Year && x.SubmittedBy == userId && x.ApprovalStatus == ApprovalStatus.Pending);
            return listProposals;
        }
    }
}
