using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

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


            var banners = _db.Banners.AsQueryable();
            var skus = _db.SKUs.Include(x => x.ProductCategory).AsQueryable();
            var workLevel = user.WLName;
            var userBannerIds = user.Banners.Select(x => x.Id);
            var userSkuIds = user.SKUs.Select(x => x.Id);
            var userCategIds = user.ProductCategories.Select(x => x.Id);
            if (workLevel == "KAM WL 2")
            {
                banners = banners.Where(x => userBannerIds.Contains(x.Id));
            }
            else if (workLevel == "CCD")
            {
                skus = skus.Where(x => userSkuIds.Contains(x.Id) || userCategIds.Contains(x.ProductCategory.Id));
            }

            var approvals = (from approval in _db.Approvals
                             join proposal in (from proposal in _db.Proposals
                                               join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
                                                                         //join banner in _db.Banners.Include(x => x.UserUnilevers).Where(x => x.UserUnilevers.Any(x => x.Id == userId)) on weeklyBucket.BannerId equals banner.Id
                                                                     join banner in banners on weeklyBucket.BannerId equals banner.Id
                                                                     join sku in skus on weeklyBucket.SKUId equals sku.Id
                                                                     select new WeeklyBucket
                                                                     {
                                                                         Id = weeklyBucket.Id,
                                                                         ProductCategoryId = sku.ProductCategory.Id,
                                                                         BannerName = banner.BannerName,
                                                                         PlantName = banner.PlantName,
                                                                         PCMap = sku.PCMap,
                                                                         DescriptionMap = sku.DescriptionMap,
                                                                     }) on proposal.WeeklyBucketId equals weeklyBucket.Id
                                               select new Proposal
                                               {
                                                   Id = proposal.Id,
                                                   ApprovalId = proposal.ApprovalId,
                                                   BannerName = weeklyBucket.BannerName,
                                                   PlantName = weeklyBucket.PlantName,
                                                   PCMap = weeklyBucket.PCMap,
                                                   DescriptionMap = weeklyBucket.DescriptionMap,
                                                   ProposeAdditional = proposal.ProposeAdditional,
                                                   Rephase = proposal.Rephase,
                                                   Remark = proposal.Remark,
                                                   Type = proposal.Type,
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
                                 BannerName = proposal.BannerName,
                                 PCMap = proposal.PCMap,
                                 DescriptionMap = proposal.DescriptionMap,
                                 ProposeAdditional = proposal.ProposeAdditional,
                                 Rephase = proposal.Rephase,
                                 Remark = proposal.Remark,
                                 Week = proposal.Week,
                                 Level = approval.Level,
                                 ApproverWL = approval.ApproverWL,
                             });

            //var workLevel = user.WLName;
            //var userBannerIds = user.Banners.Select(x => x.Id).ToList();
            //var userSkuIds = user.SKUs.Select(x => x.Id).ToList();
            //var userCategIds = user.ProductCategories.Select(x => x.Id).ToList();
            //approvals = approvals.Where(x => x.ApproverWL == workLevel);
            //if (workLevel == "KAM WL 2")
            //{
            //    approvals = approvals.Where(x => userBannerIds.Contains(x.BannerId));
            //}
            //else if (workLevel == "CCD")
            //{
            //    approvals = approvals.Where(x => userSkuIds.Contains(x.SKUId) || userCategIds.Contains(x.ProductCategoryId));
            //}
            //if (workLevel == "KAM WL 2")
            //{
            //    approvals = approvals.Where(x => (x.ProposalType == ProposalType.Rephase || x.ProposalType == ProposalType.ReallocateAcrossKAM) && x.Level == 2 && userBannerIds.Contains(x.BannerId));
            //}
            //else if(workLevel == "SOM MT WL 1")
            //{
            //    approvals = approvals.Where(x => x.ProposalType == ProposalType.Rephase && x.Level == 1);
            //}
            //else if(workLevel == "CDM WL 3")
            //{
            //    approvals = approvals.Where(x => x.ProposalType == ProposalType.ReallocateAcrossCDM && x.Level == 2);
            //}
            //else if(workLevel == "SOM MT WL 2")
            //{
            //    approvals = approvals.Where(x => x.ProposalType == ProposalType.ReallocateAcrossKAM || x.ProposalType == ProposalType.ReallocateAcrossCDM || x.ProposalType == ProposalType.ReallocateAcrossMT && x.Level == 1);
            //}
            //else 
            //{
            //    approvals = approvals.Where(x => x.ProposalType == ProposalType.ProposeAdditional);
            //    if(workLevel == "CCD")
            //    {
            //        approvals = approvals.Where(x => x.Level == 3);
            //    }
            //    else if (workLevel == "CORE VP")
            //    {
            //        approvals = approvals.Where(x => x.Level == 2);
            //    }
            //    else
            //    {
            //        approvals = approvals.Where(x => x.Level == 1);
            //    }
            //}

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


        public async Task<Approval> GetApprovalDetails(Guid approvalId)
        {
            var apprvl = _db.Approvals.Where(x => x.ApprovalStatus == ApprovalStatus.Pending || x.ApprovalStatus == ApprovalStatus.WaitingNextLevel).Include(x => x.ApprovalDetails).SingleOrDefault(x => x.Id == approvalId);
            if (apprvl != null)
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
                    if (proposalThisApproval.Type == ProposalType.Rephase)
                    {
                        approvalDetail.Rephase = detail.Rephase;
                    }
                    else
                    {
                        approvalDetail.ProposeAdditional = detail.ProposeAdditional;
                    }

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

        public string GetWLApprover(Approval approval)
        {
            var wlApprover = string.Empty;
            if (approval.Level == 3)
            {
                wlApprover = "CCD";
            }
            else if (approval.Level == 2)
            {
                if (approval.ProposalType == ProposalType.Rephase || approval.ProposalType == ProposalType.ReallocateAcrossKAM)
                {
                    wlApprover = "KAM WL 2";
                }
                else if (approval.ProposalType == ProposalType.ReallocateAcrossCDM)
                {
                    wlApprover = "CDM WL 3";
                }
                else if (approval.ProposalType == ProposalType.ReallocateAcrossMT)
                {
                    wlApprover = "VP MTDA";
                }
                else
                {
                    wlApprover = "CORE VP";
                }
            }
            else
            {
                if (approval.ProposalType == ProposalType.Rephase)
                {
                    wlApprover = "SOM MT WL 1";
                }
                else if (approval.ProposalType == ProposalType.ProposeAdditional)
                {
                    wlApprover = "CD DIRECTOR";
                }
                else
                {
                    wlApprover = "SOM MT WL 2";
                }
            }

            return wlApprover;
        }

        public async Task<List<string>> GetRecipientEmail(string wlApprover, Guid bannerId = new Guid())
        {
            var worklevel = await _db.WorkLevels.SingleAsync(x => x.WL == wlApprover);
            var users = await _db.UsersUnilever.Include(x => x.Banners).Where(x => x.WLId == worklevel.Id).ToListAsync();
            if (wlApprover == "KAM WL 2")
            {
                users = users.Where(x => x.Banners.Select(x => x.Id).Contains(bannerId)).ToList();
            }

            var usersEmail = users.Select(x => x.Email).ToList();
            return usersEmail;
        }

        public async Task<List<EmailApproval>> GenerateEmailProposal(Approval approval, string url, string requestor, Banner banner, SKU sku)
        {
            var listEmail = new List<EmailApproval>();
            var type = string.Empty;
            var emails = await GetRecipientEmail(approval.ApproverWL, banner.Id);
            if (approval.ProposalType == ProposalType.ReallocateAcrossKAM)
            {
                type = "Reallocate Across KAM";
            }
            else if (approval.ProposalType == ProposalType.ReallocateAcrossCDM)
            {
                type = "Reallocate Across CDM";
            }
            else if (approval.ProposalType == ProposalType.ReallocateAcrossMT)
            {
                type = "Reallocate Across MT";
            }
            else if (approval.ProposalType == ProposalType.Rephase)
            {
                type = "Rephase";
            }
            else
            {
                type = "Propose Additional";
            }
            foreach (var email in emails)
            {
                var emailApproval = new EmailApproval();
                emailApproval.RecipientEmail = email;
                emailApproval.ApprovalUrl = url;
                emailApproval.Name = email;
                emailApproval.Requestor = requestor;
                emailApproval.Subject = $"FSA {type} Approval Request";
                emailApproval.Body = $"Hi, {email}, " +
                                     $"<br> Please approve {type} Proposal Request: " +
                                     $"<br> " +
                                     $"Banner: {banner.BannerName} " +
                                     $"<br> " +
                                     $"Plant Code: {banner.PlantCode} " +
                                     $"<br> " +
                                     $"Plant Name: {banner.PlantName} " +
                                     $"<br> " +
                                     $"PC Code: {sku.PCMap}" +
                                     $"<br>" +
                                     $"Description Map: {sku.DescriptionMap} <br>" +
                                     $" from {requestor} by <a href='{HtmlEncoder.Default.Encode(url)}'>clicking here</a>. <br><br><br> Thank You.";
                listEmail.Add(emailApproval);
            }

            return listEmail;
        }

        public async Task<EmailApproval> GenerateEmailApproval(Approval approval, string userApproverEmail, string requestorEmail, string approvalNote, Banner banner, SKU sku)
        {
            var emailApproval = new EmailApproval();
            var type = string.Empty;

            if (approval.ProposalType == ProposalType.ReallocateAcrossKAM)
            {
                type = "Reallocate Across KAM";
            }
            else if (approval.ProposalType == ProposalType.ReallocateAcrossCDM)
            {
                type = "Reallocate Across CDM";
            }
            else if (approval.ProposalType == ProposalType.ReallocateAcrossMT)
            {
                type = "Reallocate Across MT";
            }
            else if (approval.ProposalType == ProposalType.Rephase)
            {
                type = "Rephase";
            }
            else
            {
                type = "Propose Additional";
            }

            emailApproval.RecipientEmail = requestorEmail;
            emailApproval.Name = requestorEmail;
            emailApproval.Subject = $"FSA {type} Proposal Request";

            if (approval.ApprovalStatus == ApprovalStatus.WaitingNextLevel)
            {
                emailApproval.Body = $"Hi, {requestorEmail}, " +
                                     $"<br> " +
                                     $"Your Proposal Request on " +
                                     $"<br> " +
                                     $"Banner: {banner.BannerName} " +
                                     $"<br> " +
                                     $"Plant Code: {banner.PlantCode} " +
                                        $"<br> " +
                                     $"Plant Name: {banner.PlantName} " +
                                        $"<br> " +
                                        $"PC Code: {sku.PCMap}" +
                                        $"<br>" +
                                        $"Description Map: {sku.DescriptionMap}" +
                                        $"<br>" +
                                     $"has been approved by {userApproverEmail} and now waiting for next level approval. <br><br><br> Thank You";
            }
            else if (approval.ApprovalStatus == ApprovalStatus.Rejected)
            {
                emailApproval.Body = $"Hi, {requestorEmail}, " +
                                    $"<br><br> " +
                                    $"Your Proposal Request on " +
                                    $"<br> " +
                                    $"Banner: {banner.BannerName} " +
                                    $"<br> " +
                                    $"Plant Code: {banner.PlantCode} " +
                                    $"<br> " +
                                    $"Plant Name: {banner.PlantName} " +
                                    $"<br> " +
                                    $"PC Code: {sku.PCMap} " +
                                    $"<br>" +
                                    $"Description Map: {sku.DescriptionMap} " +
                                    $"<br>" +
                                    $"has been rejected by {userApproverEmail} because of {approvalNote}. <br><br><br> Thank You";
            }
            else
            {
                var note = !string.IsNullOrEmpty(approvalNote) ? $"Approval Note: {approvalNote} <br>" : string.Empty;
                emailApproval.Body = $"Hi, {requestorEmail}, " +
                                    $"<br> " +
                                    $"Your Proposal Request on " +
                                    $"<br> " +
                                    $"Banner: {banner.BannerName} " +
                                    $"<br> " +
                                    $"Plant Code: {banner.PlantCode} " +
                                    $"<br> " +
                                    $"Plant Name: {banner.PlantName} " +
                                    $"<br> " +
                                    $"PC Code: {sku.PCMap} " +
                                    $"<br>" +
                                    $"Description Map: {sku.DescriptionMap} " +
                                    $"<br>" +
                                    note +
                                    $"has been approved by {userApproverEmail}. <br><br><br> Thank You";
            }


            return emailApproval;
        }

    }
}

