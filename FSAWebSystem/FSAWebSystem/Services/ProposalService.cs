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
        public async Task<ProposalData> GetProposalForView(int month, int year, int week, DataTableParamProposal param, UserUnilever userUnilever)
        {
            _db.ChangeTracker.AutoDetectChangesEnabled = false;

            var proposals = (from weeklyBucket in _db.WeeklyBuckets.Include(x => x.BannerPlant)
                             join bannerPlant in _db.BannerPlants.Include(x => x.Plant) on weeklyBucket.BannerPlant.Id equals bannerPlant.Id
                             join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                             join proposal in _db.Proposals.Include(x => x.ProposalDetails).Include(x => x.Banner).Where(x => x.IsWaitingApproval) on new { BannerId = weeklyBucket.BannerPlant.Banner.Id, SKUId = weeklyBucket.SKUId } equals new { BannerId = proposal.Banner.Id, SKUId = proposal.Sku.Id } into proposalGroups
                             from p in proposalGroups.DefaultIfEmpty()
                             where weeklyBucket.Month == month && weeklyBucket.Year == year
                             select new Proposal
                             {
                                 Id = p != null ? p.Id : Guid.Empty,
                                 KAM = bannerPlant.KAM,
                                 CDM = bannerPlant.CDM,
                                 Banner = bannerPlant.Banner,
                                 //WeeklyBucketId = weeklyBucket.Id,
                                 //BannerName = bannerPlant.Banner.BannerName,
                                 //BannerId = bannerPlant.Banner.Id,
                                 Month = month,
                                 Week = week,
                                 Year = year,
                                 //PlantCode = bannerPlant.Plant.PlantCode,
                                 //PlantName = bannerPlant.Plant.PlantName,
                                 Sku = sku,
                                 DescriptionMap = sku.DescriptionMap,
                                 RatingRate = weeklyBucket.RatingRate,
                                 MonthlyBucket = weeklyBucket.MonthlyBucket,
                                 ValidBJ = weeklyBucket.ValidBJ,
                                 RemFSA = weeklyBucket.MonthlyBucket - weeklyBucket.ValidBJ,
                                 CurrentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + week.ToString()).GetValue(weeklyBucket, null)),
                                 NextBucket = week <= 4 ? Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).GetValue(weeklyBucket, null)) : 0,
                                 Remark = p != null ? p.Remark : string.Empty,
                                 Rephase = p != null ? p.Rephase : decimal.Zero,
                                 ProposeAdditional = p != null ? p.ProposeAdditional : decimal.Zero,
                                 IsWaitingApproval = p != null ? p.IsWaitingApproval : false,
                                 SubmittedBy = p == null ? Guid.Empty : p.SubmittedBy.Value,
                             }).AsEnumerable().GroupBy(x => new { x.KAM, x.CDM, x.Banner.Id, SKUId = x.Sku.Id }).Select(y => new Proposal
                             {
                                 Id = y.First().Id,
                                 Banner = y.First().Banner,
                                 Month = y.First().Month,
                                 Week = y.First().Week,
                                 Year = y.First().Year,
                                 KAM = y.First().KAM,
                                 CDM = y.First().CDM,
                                 //PlantCode = y.First().PlantCode,
                                 //PlantName = y.First().PlantName,
                                 Sku = y.First().Sku,
                                 DescriptionMap = y.First().DescriptionMap,
                                 RatingRate = y.Sum(z => z.RatingRate),
                                 MonthlyBucket = y.Sum(z => z.MonthlyBucket),
                                 ValidBJ = y.Sum(z => z.ValidBJ),
                                 RemFSA = y.Sum(z => z.RemFSA),
                                 CurrentBucket = y.Sum(z => z.CurrentBucket),
                                 NextBucket = y.Sum(z => z.NextBucket),
                                 Remark = y.First().Remark,
                                 Rephase = y.First().Rephase,
                                 ProposeAdditional = y.First().ProposeAdditional,
                                 IsWaitingApproval = y.Any(z => z.IsWaitingApproval),
                                 SubmittedBy = y.First().SubmittedBy,
                             });



            if (userUnilever.RoleUnilever.RoleName != "Administrator")
            {
                proposals = proposals.Where(x => userUnilever.BannerPlants.Select(y => y.Id).Contains(x.Banner.Id));
                proposals = proposals.Where(x => x.SubmittedBy == userUnilever.Id || x.SubmittedBy == Guid.Empty || x.SubmittedBy == null);
            }


            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                proposals = proposals.Where(x => x.Banner.BannerName.ToLower().Contains(search.ToLower()) ||
                                            x.Sku.PCMap.ToLower().Contains(search.ToLower()) ||
                                            x.Sku.DescriptionMap.ToLower().Contains(search.ToLower()));
            }


            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.IsWaitingApproval).ThenByDescending(x => x.Banner.BannerName) : proposals.OrderByDescending(x => x.IsWaitingApproval).ThenBy(x => x.Banner.BannerName);
                        break;
                    case 1:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.PCMap) : proposals.OrderBy(x => x.PCMap);
                        break;
                    case 2:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.DescriptionMap) : proposals.OrderBy(x => x.DescriptionMap);
                        break;
                    case 3:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.RatingRate) : proposals.OrderBy(x => x.RatingRate);
                        break;
                    case 4:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.MonthlyBucket) : proposals.OrderBy(x => x.MonthlyBucket);
                        break;
                    //case 6:
                    //    proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.CurrentBucket) : proposal2.OrderBy(x => x.CurrentBucket);
                    //    break;
                    //case 7:
                    //    proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.NextBucket) : proposal2.OrderBy(x => x.NextBucket);
                    //    break;
                    case 7:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.ValidBJ) : proposals.OrderBy(x => x.ValidBJ);
                        break;
                    case 8:
                        proposals = order.dir == "desc" ? proposals.OrderByDescending(x => x.RemFSA) : proposals.OrderBy(x => x.RemFSA);
                        break;
                    default:
                        break;

                }
            }


            var totalCount = proposals.Count();
            var listProposal = proposals.Skip(param.start).Take(param.length).ToList();
            _db.ChangeTracker.AutoDetectChangesEnabled = true;
            return new ProposalData
            {
                proposalInputs = param.proposalInputs,
                totalRecord = totalCount,
                proposals = listProposal
            };
        }


        public ProposalHistoryPagingData GetProposalHistoryPagination(DataTableParam param, UserUnilever userUnilever, int month, int year)
        {

            var proposalHistories = (from proposalHistory in _db.ProposalHistories
                                     join proposal in (from proposal in _db.Proposals
                                                       join approval in _db.Approvals on proposal.Id equals approval.Proposal.Id
                                                       select new Proposal
                                                       {
                                                           Id = proposal.Id,
                                                           ApprovedBy = approval.ApprovedBy,
                                                           ApprovalStatus = approval.ApprovalStatus,
                                                           ApprovalNote = approval.ApprovalNote,
                                                           ApproverWL = approval.ApproverWL,
                                                       }) on proposalHistory.ProposalId equals proposal.Id
                                     join sku in _db.SKUs.Where(x => x.IsActive) on proposalHistory.SKUId equals sku.Id
                                     join banner in _db.Banners on proposalHistory.BannerId equals banner.Id
                                     where proposalHistory.Month == month && proposalHistory.Year == year
                                     select new ProposalHistory
                                     {
                                         Week = proposalHistory.Week,
                                         BannerName = banner.BannerName,
                                         PCMap = sku.PCMap,
                                         DescriptionMap = sku.DescriptionMap,
                                         Month = proposalHistory.Month,
                                         SubmittedAt = proposalHistory.SubmittedAt,
                                         Remark = proposalHistory.Remark,
                                         ProposeAdditional = proposalHistory.ProposeAdditional,
                                         Rephase = proposalHistory.Rephase,
                                         ApprovedBy = proposal.ApprovedBy,
                                         ApprovalNote = proposal.ApprovalNote,
                                         ApprovalStatus = proposal.ApprovalStatus == ApprovalStatus.WaitingNextLevel || proposal.ApprovalStatus == ApprovalStatus.Pending ? proposal.ApprovalStatus.ToString() + '(' + proposal.ApproverWL + ')' : proposal.ApprovalStatus.ToString(),
                                         SubmittedBy = proposalHistory.SubmittedBy
                                     });

            var zz = proposalHistories.ToList();
            if (userUnilever.RoleUnilever.RoleName != "Administrator")
            {
                //proposalHistories = proposalHistories.Where(x => userUnilever.Banners.Select(y => y.Id).Contains(x.BannerId));
                proposalHistories = proposalHistories.Where(x => x.SubmittedBy == userUnilever.Id);
            }


            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                proposalHistories = proposalHistories.Where(x => x.BannerName.ToLower().Contains(search.ToLower()) ||
                x.PlantName.ToLower().Contains(search.ToLower()) || x.PCMap.ToLower().Contains(search.ToLower()) ||
                x.DescriptionMap.ToLower().Contains(search.ToLower()) ||
                x.Remark.ToLower().Contains(search.ToLower())
                );
            }


            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.SubmittedAt) : proposalHistories.OrderByDescending(x => x.SubmittedAt);
                        break;
                    case 1:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.Week) : proposalHistories.OrderBy(x => x.Week);
                        break;
                    case 3:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.BannerName) : proposalHistories.OrderBy(x => x.BannerName);
                        break;
                    case 4:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.PlantName) : proposalHistories.OrderBy(x => x.PlantName);
                        break;
                    case 5:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.PCMap) : proposalHistories.OrderBy(x => x.PCMap);
                        break;
                    case 6:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.DescriptionMap) : proposalHistories.OrderBy(x => x.DescriptionMap);
                        break;
                    case 7:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.Rephase) : proposalHistories.OrderBy(x => x.Rephase);
                        break;
                    case 8:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.ProposeAdditional) : proposalHistories.OrderBy(x => x.ProposeAdditional);
                        break;
                    case 9:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.Remark) : proposalHistories.OrderBy(x => x.Remark);
                        break;
                    case 11:
                        proposalHistories = order.dir == "desc" ? proposalHistories.OrderByDescending(x => x.ApprovedBy) : proposalHistories.OrderBy(x => x.ApprovedBy);
                        break;
                    default:
                        proposalHistories = proposalHistories.OrderByDescending(x => x.SubmittedAt).ThenBy(x => x.ApprovalId);
                        break;

                }
            }

            var totalCount = proposalHistories.Count();
            var listProposalHistory = proposalHistories.Skip(param.start).Take(param.length).ToList();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("id-ID");
            foreach (var proposalHistory in listProposalHistory)
            {
                var submitDate = DateTime.Parse(proposalHistory.SubmittedAt);
                var uliCalendarDetail = _db.ULICalendarDetails.SingleOrDefault(x => submitDate.Date >= x.StartDate.Value.Date && submitDate.Date <= x.EndDate);
                proposalHistory.ULIWeek = uliCalendarDetail != null ? uliCalendarDetail.Week.ToString() : string.Empty;
            }
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
            await _db.Proposals.AddRangeAsync(listProposal);
        }

        public IQueryable<Proposal> GetPendingProposals(FSACalendarDetail fsaDetail, Guid userId)
        {
            var listProposals = _db.Proposals.Include(x => x.Banner).Where(x => !x.IsWaitingApproval);
            return listProposals;
        }

        public async Task<Proposal> GetProposalById(Guid proposalId)
        {
            var proposal = await _db.Proposals.SingleOrDefaultAsync(x => x.Id == proposalId);
            return proposal;
        }

        //public async Task<Proposal> GetProposalByApprovalId(Guid approvalId)
        //{
        //    var proposal = await _db.Proposals.Include(x => x.ProposalDetails).SingleOrDefaultAsync(x => x.ApprovalId == approvalId);
        //    return proposal;
        //}

        public async Task SaveProposalHistories(List<ProposalHistory> listProposalHistory)
        {
            await _db.ProposalHistories.AddRangeAsync(listProposalHistory);

        }

        public List<ProposalExcelModel> GetProposalExcelData(int month, int year, UserUnilever user)
        {

            var data = (from weeklyBucket in _db.WeeklyBuckets.Include(x => x.BannerPlant)
                        join bannerPlant in _db.BannerPlants.Include(x => x.UserUnilevers).Include(x => x.Plant).Where(y => y.UserUnilevers.Any(z => z.Id == user.Id)) on weeklyBucket.BannerPlant.Id equals bannerPlant.Id
                        join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                        join proposal in _db.Proposals.Include(x => x.Banner) on weeklyBucket.BannerPlant.Banner.Id equals proposal.Banner.Id into proposalGroups
                        from p in proposalGroups.DefaultIfEmpty()
                        where weeklyBucket.Month == month && weeklyBucket.Year == year
                        select new ProposalExcelModel
                        {
                            Month = month,
                            KAM = bannerPlant.KAM,
                            CDM = bannerPlant.CDM,
                            BannerName = bannerPlant.Banner.BannerName,
                            PlantCode = bannerPlant.Plant.PlantCode,
                            PlantName = bannerPlant.Plant.PlantName,
                            PCMap = sku.PCMap,
                            DescriptionMap = sku.DescriptionMap,
                            MonthlyBucket = weeklyBucket.MonthlyBucket,
                            PlantContribution = weeklyBucket.PlantContribution,
                            ValidBJ = weeklyBucket.ValidBJ,
                            RemFSA = weeklyBucket.RemFSA,
                            DispatchConsume = weeklyBucket.DispatchConsume,
                            BucketWeek1 = weeklyBucket.BucketWeek1,
                            BucketWeek2 = weeklyBucket.BucketWeek2,
                            BucketWeek3 = weeklyBucket.BucketWeek3,
                            BucketWeek4 = weeklyBucket.BucketWeek4,
                            BucketWeek5 = weeklyBucket.BucketWeek5,
                            RatingRate = weeklyBucket.RatingRate,
                            SubmittedBy = p != null ? p.SubmittedBy.Value : Guid.Empty
                        }).ToList();

            data = data.Where(x => x.SubmittedBy == user.Id || x.SubmittedBy == Guid.Empty).ToList();
            return data;
        }

        public async Task<ProposalHistory> GetProposalHistory(Guid approvalId)
        {
            var proposalHistory = await _db.ProposalHistories.SingleAsync(x => x.ApprovalId == approvalId && x.ProposeAdditional < 0);
            return proposalHistory;
        }
    }
}
