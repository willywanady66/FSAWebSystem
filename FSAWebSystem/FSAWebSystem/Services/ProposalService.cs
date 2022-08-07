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
        public async Task<ProposalData> GetProposalForView(int month, int year, int week, DataTableParamProposal param, Guid userId)
        {
            _db.ChangeTracker.AutoDetectChangesEnabled = false;

            var proposals = (from weeklyBucket in _db.WeeklyBuckets
                             join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                             join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                             join proposal in _db.Proposals.Where(x => x.Type != ProposalType.Reallocate) on weeklyBucket.Id equals proposal.WeeklyBucketId into proposalGroups
                             from p in proposalGroups.DefaultIfEmpty()
                             where weeklyBucket.Month == month && weeklyBucket.Year == year
                             select new Proposal
                             {
                                 Id = p != null ? p.Id : Guid.Empty,
                                 WeeklyBucketId = weeklyBucket.Id,
                                 BannerName = banner.BannerName,
                                 BannerId = banner.Id,
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
                                 ApprovedRephase = p != null ? p.ApprovedRephase : decimal.Zero,
                                 ProposeAdditional = p != null ? p.ProposeAdditional : decimal.Zero,
                                 ApprovedProposeAdditional = p != null ? p.ApprovedProposeAdditional : decimal.Zero,
                                 IsWaitingApproval = p != null ? p.IsWaitingApproval : false
                             });

            var proposal2 = (from proposal in proposals
                             join approval in _db.Approvals.Where(x => x.SubmittedBy == userId) on proposal.Id equals approval.ProposalId into approvalGroup
                             from apprvl in approvalGroup.DefaultIfEmpty()
                             select new Proposal
                             {
                                 Id = proposal.Id,
                                 BannerId = proposal.BannerId,
                                 WeeklyBucketId = proposal.WeeklyBucketId,
                                 BannerName = proposal.BannerName,
                                 Month = month,
                                 Week = week,
                                 Year = year,
                                 PlantCode = proposal.PlantCode,
                                 PlantName = proposal.PlantName,
                                 PCMap = proposal.PCMap,
                                 DescriptionMap = proposal.DescriptionMap,
                                 RatingRate = proposal.RatingRate,
                                 MonthlyBucket = proposal.MonthlyBucket,
                                 ValidBJ = proposal.ValidBJ,
                                 RemFSA = proposal.MonthlyBucket - proposal.ValidBJ,
                                 CurrentBucket = proposal.CurrentBucket,
                                 NextBucket = proposal.NextBucket,
                                 Remark = proposal.Remark,
                                 Rephase =  proposal.Rephase,
                                 ApprovedRephase = proposal.ApprovedRephase,
                                 ApprovalStatus = apprvl != null ? apprvl.ApprovalStatus : ApprovalStatus.Pending,
                                 ProposeAdditional = proposal.ProposeAdditional,
                                 ApprovedProposeAdditional = proposal.ApprovedProposeAdditional,
                                 IsWaitingApproval = proposal.IsWaitingApproval
                             });

            proposal2 = proposal2.Where(x => !x.IsWaitingApproval);
           
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

        public async Task<ProposalData> GetProposalReallocateForView(int month, int year, int week, DataTableParamProposal param, Guid userId)
		{
            _db.ChangeTracker.AutoDetectChangesEnabled = false;

            var proposals = (from weeklyBucket in _db.WeeklyBuckets
                             join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                             join sku in _db.SKUs.Include(x => x.ProductCategory) on weeklyBucket.SKUId equals sku.Id
                             join proposal in _db.Proposals.Where(x => x.Type == ProposalType.Reallocate) on weeklyBucket.Id equals proposal.WeeklyBucketId into proposalGroups
                             from p in proposalGroups.DefaultIfEmpty()
                             where weeklyBucket.Month == month && weeklyBucket.Year == year
                             select new Proposal
                             {
                                 Id = p != null ? p.Id : Guid.Empty,
                                 Type = p != null ? p.Type : null,
                                 WeeklyBucketId = weeklyBucket.Id,
                                 BannerName = banner.BannerName,
                                 BannerId = banner.Id,
                                 Month = month,
                                 Week = week,
                                 Year = year,
                                 PlantCode = banner.PlantCode,
                                 PlantName = banner.PlantName,
                                 PCMap = sku.PCMap,
                                 Category = sku.ProductCategory.CategoryProduct,
                                 DescriptionMap = sku.DescriptionMap,
                                 RatingRate = weeklyBucket.RatingRate,
                                 MonthlyBucket = weeklyBucket.MonthlyBucket,
                                 ValidBJ = weeklyBucket.ValidBJ,
                                 RemFSA = weeklyBucket.MonthlyBucket - weeklyBucket.ValidBJ,
                                 CurrentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + week.ToString()).GetValue(weeklyBucket, null)),
                                 NextBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).GetValue(weeklyBucket, null)),
                                 Remark = p != null ? p.Remark : string.Empty,
                                 Rephase = p != null ? p.Rephase : decimal.Zero,
                                 Reallocate = p != null ? p.Reallocate : decimal.Zero,
                                 BannerTargetId = p != null ? p.BannerTargetId : Guid.Empty,
                                 IsWaitingApproval = p != null ? p.IsWaitingApproval : false,
                             });

            var proposal2 = (from proposal in proposals
                             join approval in _db.Approvals.Where(x => x.SubmittedBy == userId) on proposal.Id equals approval.ProposalId into approvalGroup
                             from apprvl in approvalGroup.DefaultIfEmpty()
                             select new Proposal
                             {
                                 Id = proposal.Id,
                                 WeeklyBucketId = proposal.WeeklyBucketId,
                                 BannerName = proposal.BannerName,
                                 BannerId = proposal.BannerId,
                                 Month = month,
                                 Week = week,
                                 Year = year,
                                 PlantCode = proposal.PlantCode,
                                 PlantName = proposal.PlantName,
                                 PCMap = proposal.PCMap,
                                 Category = proposal.Category,
                                 CurrentBucket = proposal.CurrentBucket,
                                 DescriptionMap = proposal.DescriptionMap,
                                 Remark = proposal.Remark,
                                 Reallocate = proposal.Reallocate,
                                 BannerTargetId = proposal.BannerTargetId,
                                 Type = proposal.Type,
                                 ApprovalStatus = apprvl != null ? apprvl.ApprovalStatus : ApprovalStatus.Pending,
                                 ProposeAdditional = proposal.ProposeAdditional,
                                 IsWaitingApproval = proposal.IsWaitingApproval
                             });



            proposal2 = proposal2.Where(x => !x.IsWaitingApproval);

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
        public ProposalHistoryPagingData GetProposalHistoryPagination(DataTableParam param, Guid userId, int month, int year)
        {
            var proposalsHistory = (from approval in _db.Approvals 
                                    join userUnilever in _db.UsersUnilever.Where(x => x.Id == userId) on approval.SubmittedBy equals userUnilever.Id
                                    join proposal in ( from proposal in _db.Proposals
                                                        join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
                                                                            join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                                                                            join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                                                                              select new WeeklyBucket
                                                                            {
                                                                                Id = weeklyBucket.Id,
                                                                                BannerName = banner.BannerName,
                                                                                PlantName = banner.PlantName,
                                                                                PCMap = sku.PCMap,
                                                                                DescriptionMap = sku.DescriptionMap,
                                                                            }) on proposal.WeeklyBucketId equals weeklyBucket.Id
                                                       where proposal.Month == month && proposal.Year == year
                                                       select new Proposal
                                                       {
                                                           Id = proposal.Id,
                                                           BannerName = weeklyBucket.BannerName,
                                                           PlantName = weeklyBucket.PlantName,
                                                           PCMap = weeklyBucket.PCMap,
                                                           DescriptionMap = weeklyBucket.DescriptionMap,
                                                           ProposeAdditional = proposal.ProposeAdditional,
                                                           Rephase = proposal.Rephase,
                                                           Reallocate = proposal.Reallocate,
                                                           Remark = proposal.Remark,
                                                           //Year = proposal.Year,
                                                           //Month = proposal.Month,
                                                           Week = proposal.Week
                                                       }) on approval.ProposalId equals proposal.Id
                                    select new ProposalHistory
                                    {
                                        SubmittedAt = approval.SubmittedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                        BannerName = proposal.BannerName,
                                        PlantName = proposal.PlantName,
                                        PCMap = proposal.PCMap,
                                        DescriptionMap = proposal.DescriptionMap,
                                        Rephase = proposal.Rephase,
                                        Remark = proposal.Remark,
                                        ApprovedRephase = 0,
                                        ProposeAdditional = proposal.ProposeAdditional,
                                        ApprovedProposeAdditional = 0,     
                                        Reallocate = proposal.Reallocate,
                                        Status = approval.ApprovalStatus.ToString(),
                                        ApprovedBy = userUnilever.Name,
                                        RejectionReason = approval.RejectionReason,
                                        Week = proposal.Week
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

        public async Task<Proposal> GetProposalById(Guid proposalId)
        {
            var proposal = await _db.Proposals.SingleOrDefaultAsync(x => x.Id == proposalId);
            return proposal;
        }
    }
}
