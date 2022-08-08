using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly FSAWebSystemDbContext _db;

        public ApprovalService(FSAWebSystemDbContext db)
        {
            _db = db;
        }
        public async Task SaveApprovals(List<Approval> listApproval)
        {
            await _db.AddRangeAsync(listApproval);
        }

        public IQueryable<Approval> GetPendingApprovals()
        {
            return _db.Approvals.Where(x => x.ApprovalStatus == ApprovalStatus.Pending);
        }

        public async Task<ApprovalPagingData> GetApprovalPagination(DataTableParam param, int month, int year)
        {
            var approvals = (from approval in _db.Approvals
                             join proposal in (from proposal in _db.Proposals
                                               join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
                                                                     //join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                                                                     join banner in _db.Banners on weeklyBucket.BannerId equals banner.Id
                                                                     join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                                                                     select new WeeklyBucket
                                                                     {
                                                                         Id = weeklyBucket.Id,
                                                                         BannerName = banner.BannerName,
                                                                         PlantName = banner.PlantName,
                                                                         PCMap = sku.PCMap,
                                                                         DescriptionMap = sku.DescriptionMap,
                                                                     }) on proposal.WeeklyBucketId equals weeklyBucket.Id
                                               select new Proposal
                                               {
                                                   Id = proposal.Id,
                                                   BannerName = weeklyBucket.BannerName,
                                                   PlantName = weeklyBucket.PlantName,
                                                   PCMap = weeklyBucket.PCMap,
                                                   DescriptionMap = weeklyBucket.DescriptionMap,
                                                   ProposeAdditional = proposal.ProposeAdditional,
                                                   Rephase = proposal.Rephase,
                                                   Remark = proposal.Remark,
                                                   Type = proposal.Type,
                                                   //Year = proposal.Year,
                                                   //Month = proposal.Month,
                                                   Week = proposal.Week
                                               }) on approval.ProposalId equals proposal.Id
                                               where approval.ApprovalStatus == ApprovalStatus.Pending && (approval.ProposalType == ProposalType.Rephase || approval.ProposalType == ProposalType.ProposeAdditional)
                             select new Approval
                             {
                                 ProposalId = approval.ProposalId,
                                 Proposal = proposal,
                                 Id = approval.Id,
                                 SubmitDate = approval.SubmittedAt.ToString("dd/MM/yyyy"),
                                 BannerName = proposal.BannerName,
                                 PCMap = proposal.PCMap,
                                 DescriptionMap = proposal.DescriptionMap,
                                 ProposeAdditional = proposal.ProposeAdditional,
                                 Rephase = proposal.Rephase,
                                 Remark = proposal.Remark,
                                 Week = proposal.Week,
                                 Level1 = "",
                                 Level2 = ""
                             });


            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                approvals = approvals.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search));
            }
            var totalCount = approvals.Count();
            var listApproval = approvals.Skip(param.start).Take(param.length).ToList();
            return new ApprovalPagingData
            {
                totalRecord = totalCount,
                approvals = listApproval
            };

        }
         
        public async Task<ApprovalPagingData> GetApprovalReallocatePagination(DataTableParam param, int month, int year)
		{
            var approvals = (from approval in _db.Approvals
                             join proposal in (from proposal in _db.Proposals
                                               join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
                                                                     //join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                                                                     join banner in _db.Banners on weeklyBucket.BannerId equals banner.Id
                                                                     join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
                                                                     select new WeeklyBucket
                                                                     {
                                                                         Id = weeklyBucket.Id,
                                                                         BannerName = banner.BannerName,
                                                                         PlantName = banner.PlantName,
                                                                         PlantCode = banner.PlantCode,
                                                                         PCMap = sku.PCMap,
                                                                         DescriptionMap = sku.DescriptionMap,
                                                                     }) on proposal.WeeklyBucketId equals weeklyBucket.Id
                                                                     join bannerTarget in _db.Banners on proposal.BannerTargetId equals bannerTarget.Id
                                               select new Proposal
                                               {
                                                   Id = proposal.Id,
                                                   BannerName = weeklyBucket.BannerName,
                                                   PlantName = weeklyBucket.PlantName,
                                                   PlantCode = weeklyBucket.PlantCode,
                                                   PCMap = weeklyBucket.PCMap,
                                                   DescriptionMap = weeklyBucket.DescriptionMap,
                                                   ProposeAdditional = proposal.ProposeAdditional,
                                                   Rephase = proposal.Rephase,
                                                   Reallocate = proposal.Reallocate,
                                                   Remark = proposal.Remark,
                                                   BannerNameTarget = bannerTarget.BannerName,
                                                   PlantCodeTarget = bannerTarget.PlantCode,
                                                   PlantNameTarget = bannerTarget.PlantName,
                                                   Type = proposal.Type,
                                                   //Year = proposal.Year,
                                                   //Month = proposal.Month,
                                                   Week = proposal.Week
                                               }) on approval.ProposalId equals proposal.Id
                                               where approval.ApprovalStatus == ApprovalStatus.Pending
                                               && approval.ProposalType == ProposalType.Reallocate
                             select new Approval
                             {
                                 ProposalId = approval.ProposalId,
                                 Proposal = proposal,
                                 Id= approval.Id,
                                 SubmitDate = approval.SubmittedAt.ToString("dd/MM/yyyy"),
                                 BannerName = proposal.BannerName,
                                 PCMap = proposal.PCMap,
                                 DescriptionMap = proposal.DescriptionMap,
                                 Remark = proposal.Remark,
                                 Week = proposal.Week,
                                 Level1 = "",
                                 Level2 = ""
                             });


            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                approvals = approvals.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search));
            }
            var totalCount = approvals.Count();
            var listApproval = approvals.Skip(param.start).Take(param.length).ToList();
            return new ApprovalPagingData
            {
                totalRecord = totalCount,
                approvals = listApproval
            };
		}

        public async Task<Approval> GetApprovalById(Guid approvalId)
        {
            var approval = await _db.Approvals.SingleOrDefaultAsync(x => x.Id == approvalId);
            return approval;
        }
    }
}
