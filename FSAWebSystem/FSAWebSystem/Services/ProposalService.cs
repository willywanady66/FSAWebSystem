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
                             join proposal in _db.Proposals on weeklyBucket.Id equals proposal.WeeklyBucketId into proposalGroups
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
                                 IsWaitingApproval = p != null ? p.IsWaitingApproval : false,
                                 SubmittedBy = p != null ? p.SubmittedBy : Guid.Empty
                             });

            var proposal2 = (from proposal in proposals.Where(x => x.SubmittedBy == userId || x.SubmittedBy == Guid.Empty)
                             join approval in _db.Approvals.Where(x => x.ApprovalStatus == ApprovalStatus.Pending) on proposal.Id equals approval.ProposalId into approvalGroup
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
                                 Remark = proposal.IsWaitingApproval ? proposal.Remark : string.Empty,
                                 Rephase = proposal.IsWaitingApproval ?  proposal.Rephase : decimal.Zero,
                                 ApprovedRephase = proposal.ApprovedRephase,
                                 ApprovalStatus = apprvl != null ? apprvl.ApprovalStatus : ApprovalStatus.Pending,
                                 ProposeAdditional = proposal.IsWaitingApproval ? proposal.ProposeAdditional : decimal.Zero,
                                 ApprovedProposeAdditional = proposal.ApprovedProposeAdditional,
                                 IsWaitingApproval = proposal.IsWaitingApproval
                             });

            //proposal2 = proposal2.Where(x => !x.IsWaitingApproval);
           
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

  //      public async Task<ProposalData> GetProposalReallocateForView(int month, int year, int week, DataTableParamProposal param, Guid userId)
		//{
  //          _db.ChangeTracker.AutoDetectChangesEnabled = false;

  //          var proposals = (from weeklyBucket in _db.WeeklyBuckets
  //                           join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
  //                           join sku in _db.SKUs.Include(x => x.ProductCategory) on weeklyBucket.SKUId equals sku.Id
  //                           join proposal in _db.Proposals.Where(x => x.Type == ProposalType.Reallocate) on weeklyBucket.Id equals proposal.WeeklyBucketId into proposalGroups
  //                           from p in proposalGroups.DefaultIfEmpty()
  //                           where weeklyBucket.Month == month && weeklyBucket.Year == year
  //                           select new Proposal
  //                           {
  //                               Id = p != null ? p.Id : Guid.Empty,
  //                               Type = p != null ? p.Type : null,
  //                               WeeklyBucketId = weeklyBucket.Id,
  //                               BannerName = banner.BannerName,
  //                               BannerId = banner.Id,
  //                               Month = month,
  //                               Week = week,
  //                               Year = year,
  //                               PlantCode = banner.PlantCode,
  //                               PlantName = banner.PlantName,
  //                               PCMap = sku.PCMap,
  //                               Category = sku.ProductCategory.CategoryProduct,
  //                               DescriptionMap = sku.DescriptionMap,
  //                               RatingRate = weeklyBucket.RatingRate,
  //                               MonthlyBucket = weeklyBucket.MonthlyBucket,
  //                               ValidBJ = weeklyBucket.ValidBJ,
  //                               RemFSA = weeklyBucket.MonthlyBucket - weeklyBucket.ValidBJ,
  //                               CurrentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + week.ToString()).GetValue(weeklyBucket, null)),
  //                               NextBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).GetValue(weeklyBucket, null)),
  //                               Remark = p != null ? p.Remark : string.Empty,
  //                               Rephase = p != null ? p.Rephase : decimal.Zero,
  //                               Reallocate = p != null ? p.Reallocate : decimal.Zero,
  //                               BannerTargetId = p != null ? p.BannerTargetId : Guid.Empty,
  //                               IsWaitingApproval = p != null ? p.IsWaitingApproval : false,
  //                           });

  //          var proposal2 = (from proposal in proposals.Where(x => x.SubmittedBy == userId)
  //                           join approval in _db.Approvals.Where(x => x.ApprovalStatus == ApprovalStatus.Pending) on proposal.Id equals approval.ProposalId into approvalGroup
  //                           from apprvl in approvalGroup.DefaultIfEmpty()
  //                           select new Proposal
  //                           {
  //                               Id = proposal.IsWaitingApproval ? proposal.Id : Guid.Empty,
  //                               WeeklyBucketId = proposal.WeeklyBucketId,
  //                               BannerName = proposal.BannerName,
  //                               BannerId = proposal.BannerId,
  //                               Month = month,
  //                               Week = week,
  //                               Year = year,
  //                               PlantCode = proposal.PlantCode,
  //                               PlantName = proposal.PlantName,
  //                               PCMap = proposal.PCMap,
  //                               Category = proposal.Category,
  //                               CurrentBucket = proposal.CurrentBucket,
  //                               DescriptionMap = proposal.DescriptionMap,
  //                               Remark = proposal.IsWaitingApproval ? proposal.Remark : string.Empty,
  //                               Reallocate = proposal.IsWaitingApproval ? proposal.Reallocate : decimal.Zero,
  //                               BannerTargetId = proposal.IsWaitingApproval ? proposal.BannerTargetId : Guid.Empty,
  //                               Type = proposal.Type,
  //                               ApprovalStatus = apprvl != null ? apprvl.ApprovalStatus : ApprovalStatus.Pending,
  //                               ProposeAdditional = proposal.ProposeAdditional,
  //                               IsWaitingApproval = proposal.IsWaitingApproval
  //                           });



  //          //proposal2 = proposal2.Where(x => !x.IsWaitingApproval);

  //          var totalCount = proposal2.Count();
  //          var listProposal = proposal2.Skip(param.start).Take(param.length).ToList();
  //          _db.ChangeTracker.AutoDetectChangesEnabled = true;
  //          return new ProposalData
  //          {
  //              proposalInputs = param.proposalInputs,
  //              totalRecord = totalCount,
  //              proposals = listProposal
  //          };

  //      }
        public ProposalHistoryPagingData GetProposalHistoryPagination(DataTableParam param, Guid userId, int month, int year)
        {

            var proposalHistories = (from proposalHistory in _db.ProposalHistories
                                    join approval in _db.Approvals on proposalHistory.ApprovalId equals approval.Id
                                    join proposal in (from proposal in _db.Proposals
                                                      join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
                                                                            join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                                                                            join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                                                                            select new Proposal
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
                                                          SubmittedBy = proposal.SubmittedBy,
                                                          SubmittedAt = proposal.SubmittedAt,
                                                          BannerName = weeklyBucket.BannerName,
                                                          PlantName = weeklyBucket.PlantName,
                                                          PCMap = weeklyBucket.PCMap,
                                                          DescriptionMap = weeklyBucket.DescriptionMap,
                                                          ProposeAdditional = proposal.ProposeAdditional,
                                                          Rephase = proposal.Rephase,
                                                          Reallocate = proposal.Reallocate,
                                                          Remark = proposal.Remark,
                                                          ApprovedRephase = 0,
                                                          ApprovedProposeAdditional = 0,
                                                          //Year = proposal.Year,
                                                          //Month = proposal.Month,
                                                          Week = proposal.Week
                                                      }) on proposalHistory.ProposalId equals proposal.Id
                                    select new ProposalHistory
                                    {
                                        Proposal = proposal,
                                        SubmittedAt = proposal.SubmittedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                        ApprovedBy = String.Join(',', approval.ApprovedBy.Select(x => x.Name)),
                                        RejectionReason = approval.RejectionReason,
                                        ApprovalStatus = approval.ApprovalStatus.ToString()
                                    });

            //var proposalsHistory = (from approval in _db.Approvals
            //                        join userUnilever in _db.UsersUnilever.Where(x => x.Id == userId) on proposal.SubmittedBy equals userUnilever.Id
            //                        join proposal in ( from proposal in _db.Proposals
            //                                            join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
            //                                                                join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
            //                                                                join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
            //                                                                  select new 
            //                                                                {
            //                                                                    Id = weeklyBucket.Id,
            //                                                                    BannerName = banner.BannerName,
            //                                                                    PlantName = banner.PlantName,
            //                                                                    PCMap = sku.PCMap,
            //                                                                    DescriptionMap = sku.DescriptionMap,
            //                                                                }) on proposal.WeeklyBucketId equals weeklyBucket.Id
            //                                           where proposal.Month == month && proposal.Year == year
            //                                           select new Proposal
            //                                           {
            //                                               Id = proposal.Id,
            //                                               SubmittedBy = proposal.SubmittedBy,
            //                                               SubmittedAt = proposal.SubmittedAt,
            //                                               BannerName = weeklyBucket.BannerName,
            //                                               PlantName = weeklyBucket.PlantName,
            //                                               PCMap = weeklyBucket.PCMap,
            //                                               DescriptionMap = weeklyBucket.DescriptionMap,
            //                                               ProposeAdditional = proposal.ProposeAdditional,
            //                                               Rephase = proposal.Rephase,
            //                                               Reallocate = proposal.Reallocate,
            //                                               Remark = proposal.Remark,
            //                                               //Year = proposal.Year,
            //                                               //Month = proposal.Month,
            //                                               Week = proposal.Week
            //                                           }) on approval.ProposalId equals proposal.Id

            //                        select new ProposalHistory
            //                        {
            //                            Proposal = proposal,
            //                            ApprovedBy = approval.ApprovedBy
            //                        });

            //if (!string.IsNullOrEmpty(param.search.value))
            //{
            //    var search = param.search.value.ToLower();
            //    proposalsHistory = proposalsHistory.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search));
            //}
            var totalCount = proposalHistories.Count();
            var listProposalHistory = proposalHistories.Skip(param.start).Take(param.length).ToList();
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
            var listProposals = _db.Proposals.Where(x => x.Week == fsaDetail.Week && x.Month == fsaDetail.Month && x.Year == fsaDetail.Year && x.SubmittedBy == userId);
            return listProposals;
        }
          
        public async Task<Proposal> GetProposalById(Guid proposalId)
        {
            var proposal = await _db.Proposals.SingleOrDefaultAsync(x => x.Id == proposalId);
            return proposal;
        }

        public async Task SaveProposalHistories(List<ProposalHistory> listProposalHistory)
        {
            await _db.ProposalHistories.AddRangeAsync(listProposalHistory);

        }
    }
}
