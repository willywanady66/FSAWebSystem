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
                             join proposal in _db.Proposals.Include(x => x.Banner).Where(x => x.IsWaitingApproval) on weeklyBucket.BannerPlant.Banner.Id equals proposal.Banner.Id into proposalGroups
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
                                 ApprovedRephase = p != null ? p.ApprovedRephase : decimal.Zero,
                                 ProposeAdditional = p != null ? p.ProposeAdditional : decimal.Zero,
                                 ApprovedProposeAdditional = p != null ? p.ApprovedProposeAdditional : decimal.Zero,
                                 IsWaitingApproval = p != null ? p.IsWaitingApproval : false,
                                 SubmittedBy = p == null ? Guid.Empty : p.SubmittedBy.Value,
                                 ApprovalId = p != null ? p.ApprovalId : Guid.Empty,
                             }).AsEnumerable().GroupBy(x => new {x.KAM, x.CDM, x.Banner.Id, SKUId = x.Sku.Id}).Select(y => new Proposal
                             {
                                 Id = y.First().Id,
                                 Banner = y.First().Banner,
                                 Month = y.First().Month,
                                 Week = y.First().Week,
                                 Year = y.First().Year,
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
                                 Remark = y.SingleOrDefault(z => !string.IsNullOrEmpty(z.Remark)) != null ? y.SingleOrDefault(z => !string.IsNullOrEmpty(z.Remark)).Remark : string.Empty,
                                 Rephase = y.First().Rephase,
                                 IsWaitingApproval = y.Any(z => z.IsWaitingApproval),
                                 SubmittedBy = y.SingleOrDefault(z => z.SubmittedBy != Guid.Empty) != null ? y.SingleOrDefault(z => z.SubmittedBy != Guid.Empty).SubmittedBy : Guid.Empty,
                                 ApprovalId = y.SingleOrDefault(z => z.ApprovalId != Guid.Empty) != null ? y.SingleOrDefault(z => z.ApprovalId != Guid.Empty).ApprovalId : Guid.Empty,
                             });

            var proposal2 = proposals;
            //var zz = proposals.ToList();
            //var proposal2 = (from proposal in proposals
            //                 join approval in _db.Approvals on proposal.ApprovalId equals approval.Id into approvalGroup
            //                 from apprvl in approvalGroup.DefaultIfEmpty()

            //                 select new Proposal
            //                 {
            //                     Id = proposal.Id,
            //                     BannerId = proposal.BannerId,
            //                     WeeklyBucketId = proposal.WeeklyBucketId,
            //                     BannerName = proposal.BannerName,
            //                     Month = month,
            //                     Week = week,
            //                     Year = year,
            //                     PlantCode = proposal.PlantCode,
            //                     PlantName = proposal.PlantName,
            //                     PCMap = proposal.PCMap,
            //                     DescriptionMap = proposal.DescriptionMap,
            //                     RatingRate = proposal.RatingRate,
            //                     MonthlyBucket = proposal.MonthlyBucket,
            //                     ValidBJ = proposal.ValidBJ,
            //                     RemFSA = proposal.MonthlyBucket - proposal.ValidBJ,
            //                     CurrentBucket = proposal.CurrentBucket,
            //                     NextBucket = proposal.NextBucket,
            //                     Remark = proposal.IsWaitingApproval ? proposal.Remark : string.Empty,
            //                     Rephase = proposal.IsWaitingApproval ? proposal.Rephase : decimal.Zero,
            //                     ApprovedRephase = proposal.ApprovedRephase,
            //                     ApprovalStatus = apprvl != null ? apprvl.ApprovalStatus : ApprovalStatus.Pending,
            //                     ProposeAdditional = proposal.IsWaitingApproval ? proposal.ProposeAdditional : decimal.Zero,
            //                     ApprovedProposeAdditional = proposal.ApprovedProposeAdditional,
            //                     IsWaitingApproval = proposal.IsWaitingApproval,
            //                     SubmittedBy = proposal.SubmittedBy
            //                 });

            if (userUnilever.RoleUnilever.RoleName != "Administrator")
            {
                proposal2 = proposal2.Where(x => userUnilever.BannerPlants.Select(y => y.Id).Contains(x.Banner.Id));
                proposal2 = proposal2.Where(x => x.SubmittedBy == userUnilever.Id || x.SubmittedBy == Guid.Empty || x.SubmittedBy == null);
            }


            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                proposal2 = proposal2.Where(x => x.BannerName.ToLower().Contains(search.ToLower()) ||
                                            x.PlantCode.ToLower().Contains(search.ToLower()) ||
                                            x.PlantName.ToLower().Contains(search.ToLower()) ||
                                            x.PCMap.ToLower().Contains(search.ToLower()) ||
                                            x.DescriptionMap.ToLower().Contains(search.ToLower()));
            }


            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.Banner.BannerName) : proposal2.OrderBy(x => x.Banner.BannerName);
                        break;
                    case 1:
                        proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.PCMap) : proposal2.OrderBy(x => x.PCMap);
                        break;
                    case 2:
                        proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.DescriptionMap) : proposal2.OrderBy(x => x.DescriptionMap);
                        break;
                    case 3:
                        proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.RatingRate) : proposal2.OrderBy(x => x.RatingRate);
                        break;
                    case 4:
                        proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.MonthlyBucket) : proposal2.OrderBy(x => x.MonthlyBucket);
                        break;
                    //case 6:
                    //    proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.CurrentBucket) : proposal2.OrderBy(x => x.CurrentBucket);
                    //    break;
                    //case 7:
                    //    proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.NextBucket) : proposal2.OrderBy(x => x.NextBucket);
                    //    break;
                    case 7:
                        proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.ValidBJ) : proposal2.OrderBy(x => x.ValidBJ);
                        break;
                    case 8:
                        proposal2 = order.dir == "desc" ? proposal2.OrderByDescending(x => x.RemFSA) : proposal2.OrderBy(x => x.RemFSA);
                        break;
                    default:
                        break;

                }
            }


            var totalCount = proposal2.Count();
            var listProposal = proposal2.Skip(param.start).Take(param.length).ToList();
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
                                     join sku in _db.SKUs.Where(x => x.IsActive) on proposalHistory.SKUId equals sku.Id
                                     //join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userUnilever.Id) && x.IsActive) on proposalHistory.BannerId equals banner.Id
                                     join bannerPlant in _db.BannerPlants.Include(x => x.Plant).Include(x => x.Banner) on proposalHistory.BannerId equals bannerPlant.Banner.Id
                                     join approval in _db.Approvals on proposalHistory.ApprovalId equals approval.Id
                                     where proposalHistory.Month == month && proposalHistory.Year == year
                                     select new ProposalHistory
                                     {
                                         BannerId = bannerPlant.Banner.Id,
                                         Week = proposalHistory.Week,
                                         BannerName = bannerPlant.Banner.BannerName,
                                         PlantName = bannerPlant.PlantName,
                                         PCMap = sku.PCMap,
                                         DescriptionMap = sku.DescriptionMap,
                                         Month = proposalHistory.Month,
                                         SubmittedAt = proposalHistory.SubmittedAt,
                                         Remark = proposalHistory.Remark,
                                         ProposeAdditional = proposalHistory.ProposeAdditional,
                                         Rephase = proposalHistory.Rephase,
                                         ApprovedBy = approval.ApprovedBy,
                                         ApprovalNote = approval.ApprovalNote,
                                         ApprovalStatus = approval.ApprovalStatus == ApprovalStatus.WaitingNextLevel || approval.ApprovalStatus == ApprovalStatus.Pending ? approval.ApprovalStatus.ToString() + '(' + approval.ApproverWL + ')' : approval.ApprovalStatus.ToString(),
                                         ApprovalId = approval.Id,
                                         SubmittedBy = proposalHistory.SubmittedBy
                                     });

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

        public async Task<Proposal> GetProposalByApprovalId(Guid approvalId)
        {
            var proposal = await _db.Proposals.Include(x => x.ProposalDetails).SingleOrDefaultAsync(x => x.ApprovalId == approvalId);
            return proposal;
        }

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
