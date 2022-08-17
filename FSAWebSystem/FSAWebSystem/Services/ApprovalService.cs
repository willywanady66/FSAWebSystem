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

        public async Task<ApprovalPagingData> GetApprovalPagination(DataTableParam param, int month, int year, UserUnilever user)
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
                                                                         BannerId = banner.Id,
                                                                         PlantName = banner.PlantName,
                                                                         PCMap = sku.PCMap,
                                                                         DescriptionMap = sku.DescriptionMap,
                                                                     }) on proposal.WeeklyBucketId equals weeklyBucket.Id
                                               select new Proposal
                                               {
                                                   Id = proposal.Id,
                                                   ApprovalId = proposal.ApprovalId,
                                                   BannerId = weeklyBucket.BannerId,
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
                                                   Week = proposal.Week,
                                                   SubmittedAt = proposal.SubmittedAt
                                               }) on approval.Id equals proposal.ApprovalId
                                               where approval.ApprovalStatus == ApprovalStatus.Pending || approval.ApprovalStatus == ApprovalStatus.WaitingNextLevel
                             select new Approval
                             {
                                 ProposalId = proposal.Id,
                                 Proposal = proposal,
                                 ProposalType = proposal.Type.Value,
                                 Id = approval.Id,
                                 ProposalSubmitDate = proposal.SubmittedAt.ToString("dd/MM/yyyy"),
                                 BannerId = proposal.BannerId,
                                 BannerName = proposal.BannerName,
                                 PCMap = proposal.PCMap,
                                 DescriptionMap = proposal.DescriptionMap,
                                 ProposeAdditional = proposal.ProposeAdditional,
                                 Rephase = proposal.Rephase,
                                 Remark = proposal.Remark,
                                 Week = proposal.Week,
                                 Level1 = "",
                                 Level2 = "",
                                 Level = approval.Level
                             });
            var workLevel = user.WLName;
            var userBannerIds = user.Banners.Select(x => x.Id).ToList();
            if(workLevel == "KAM WL 2")
            {
                approvals = approvals.Where(x => x.ProposalType == ProposalType.Rephase || x.ProposalType == ProposalType.ReallocateAcrossKAM && x.Level == 2 && userBannerIds.Contains(x.BannerId));
            }
            else if(workLevel == "SOM MT WL 1")
            {
                approvals = approvals.Where(x => x.ProposalType == ProposalType.Rephase && x.Level == 1);
            }
            else if(workLevel == "CDM WL 3")
            {
                approvals = approvals.Where(x => x.ProposalType == ProposalType.ReallocateAcrossCDM && x.Level == 2);
            }
            else if(workLevel == "SOM MT WL 2")
            {
                approvals = approvals.Where(x => x.ProposalType == ProposalType.ReallocateAcrossKAM || x.ProposalType == ProposalType.ReallocateAcrossCDM || x.ProposalType == ProposalType.ReallocateAcrossMT && x.Level == 1);
            }
            else 
            {
                approvals = approvals.Where(x => x.ProposalType == ProposalType.ProposeAdditional);
                if(workLevel == "CCD")
                {
                    approvals = approvals.Where(x => x.Level == 3);
                }
                else if (workLevel == "CORE VP")
                {
                    approvals = approvals.Where(x => x.Level == 2);
                }
                else
                {
                    approvals = approvals.Where(x => x.Level == 1);
                }
            }

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
         
  //      public async Task<ApprovalPagingData> GetApprovalReallocatePagination(DataTableParam param, int month, int year)
		//{
  //          var approvals = (from approval in _db.Approvals
  //                           join proposal in (from proposal in _db.Proposals
  //                                             join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
  //                                                                   //join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
  //                                                                   join banner in _db.Banners on weeklyBucket.BannerId equals banner.Id
  //                                                                   join sku in _db.SKUs on weeklyBucket.SKUId equals sku.Id
  //                                                                   select new WeeklyBucket
  //                                                                   {
  //                                                                       Id = weeklyBucket.Id,
  //                                                                       BannerName = banner.BannerName,
  //                                                                       PlantName = banner.PlantName,
  //                                                                       PCMap = sku.PCMap,
  //                                                                       DescriptionMap = sku.DescriptionMap,
  //                                                                   }) on proposal.WeeklyBucketId equals weeklyBucket.Id
  //                                                                   join bannerTarget in _db.Banners on proposal.BannerTargetId equals bannerTarget.Id
  //                                             select new Proposal
  //                                             {
  //                                                 Id = proposal.Id,
  //                                                 BannerName = weeklyBucket.BannerName,
  //                                                 PlantName = weeklyBucket.PlantName,
  //                                                 PCMap = weeklyBucket.PCMap,
  //                                                 DescriptionMap = weeklyBucket.DescriptionMap,
  //                                                 ProposeAdditional = proposal.ProposeAdditional,
  //                                                 Rephase = proposal.Rephase,
  //                                                 Reallocate = proposal.Reallocate,
  //                                                 Remark = proposal.Remark,
  //                                                 BannerNameTarget = bannerTarget.BannerName,
  //                                                 PlantCodeTarget = bannerTarget.PlantCode,
  //                                                 PlantNameTarget = bannerTarget.PlantName,
  //                                                 Type = proposal.Type,
  //                                                 //Year = proposal.Year,
  //                                                 //Month = proposal.Month,
  //                                                 Week = proposal.Week
  //                                             }) on approval.Id equals proposal.ApprovalId
  //                                             where approval.ApprovalStatus == ApprovalStatus.Pending
  //                                             && approval.ProposalType == ProposalType.Reallocate
  //                           select new Approval
  //                           {
  //                               ProposalId = proposal.Id,
  //                               Proposal = proposal,
  //                               Id= approval.Id,
  //                               SubmitDate = proposal.SubmittedAt.ToString("dd/MM/yyyy"),
  //                               BannerName = proposal.BannerName,
  //                               PCMap = proposal.PCMap,
  //                               DescriptionMap = proposal.DescriptionMap,
  //                               Remark = proposal.Remark,
  //                               Week = proposal.Week,
  //                               Level1 = "",
  //                               Level2 = ""
  //                           });


  //          if (!string.IsNullOrEmpty(param.search.value))
  //          {
  //              var search = param.search.value.ToLower();
  //              approvals = approvals.Where(x => x.BannerName.ToLower().Contains(search) || x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search));
  //          }
  //          var totalCount = approvals.Count();
  //          var listApproval = approvals.Skip(param.start).Take(param.length).ToList();
  //          return new ApprovalPagingData
  //          {
  //              totalRecord = totalCount,
  //              approvals = listApproval
  //          };
		//}

        public async Task<Approval> GetApprovalDetails(Guid approvalId)
        {
            var apprvl = _db.Approvals.Where(x => x.ApprovalStatus == ApprovalStatus.Pending || x.ApprovalStatus == ApprovalStatus.WaitingNextLevel).Include(x => x.ApprovalDetails).SingleOrDefault(x => x.Id == approvalId);
            if(apprvl != null)
            {
                var proposalThisApproval = _db.Proposals.Include(x => x.ProposalDetails.Where(y => y.ApprovalId == approvalId)).SingleOrDefault(x => x.ApprovalId == apprvl.Id);

                apprvl.ProposalSubmitDate = proposalThisApproval.SubmittedAt.ToString("dd/MM/yyyy");
                apprvl.Week = proposalThisApproval.Week;
                apprvl.RequestedBy = (await _db.UsersUnilever.SingleAsync(x => x.Id == proposalThisApproval.SubmittedBy)).Email;
                apprvl.ProposalType = proposalThisApproval.Type.Value;
                apprvl.Remark = proposalThisApproval.Remark;
                foreach (var detail in proposalThisApproval.ProposalDetails)
                {
                    var approvalDetail = new ApprovalDetail();
                    var weeklyBucket = await _db.WeeklyBuckets.SingleAsync(x => x.Id == detail.WeeklyBucketId);
                    var banner = await _db.Banners.SingleAsync(x => x.Id == weeklyBucket.BannerId);
                    var sku = await _db.SKUs.SingleAsync(x => x.Id == weeklyBucket.SKUId);
                    approvalDetail.BannerName = banner.BannerName;
                    approvalDetail.CDM = banner.CDM;
                    approvalDetail.KAM = banner.KAM;
                    approvalDetail.PCMap = sku.PCMap;
                    approvalDetail.DescriptionMap = sku.DescriptionMap;
                    approvalDetail.MonthlyBucket = weeklyBucket.MonthlyBucket;
                    approvalDetail.RatingRate = weeklyBucket.RatingRate;
                    approvalDetail.CurrentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (apprvl.Week).ToString()).GetValue(weeklyBucket, null));
                    approvalDetail.NextBucket = apprvl.Week <= 4 ? Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (apprvl.Week + 1).ToString()).GetValue(weeklyBucket, null)) : 0;
                    approvalDetail.ValidBJ = weeklyBucket.ValidBJ;
                    approvalDetail.RemFSA = weeklyBucket.RemFSA;
                    approvalDetail.ProposeAdditional = detail.ProposeAdditional;
                    
                    apprvl.ApprovalDetails.Add(approvalDetail);
                }
                apprvl.ApprovalDetails = apprvl.ApprovalDetails.OrderByDescending(x => x.ProposeAdditional).ToList();
            }
            
            return apprvl;
        }
        public async Task<Approval> GetApprovalById(Guid approvalId)
        {
            var approval = await _db.Approvals.SingleOrDefaultAsync(x => x.Id == approvalId);
            return approval;
        }
    }
}
