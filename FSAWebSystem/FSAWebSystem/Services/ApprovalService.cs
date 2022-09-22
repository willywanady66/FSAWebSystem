using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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


            var banners = _db.Banners.Include(x => x.UserUnilevers).AsQueryable();
            var skus = _db.SKUs.Include(x => x.ProductCategory).AsQueryable();
            var workLevel = user.WLName;

            
            if (workLevel == "KAM WL 2")
            {
                banners = banners.Where(x => x.UserUnilevers.Any(y => y.Id == user.Id)).AsQueryable();
            }

            var approvals = _db.Approvals.AsQueryable();

            if(workLevel == "CCD")
            {
                approvals = (from approval in _db.Approvals
                             join proposal in (from proposal in _db.Proposals
                                               join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
                                                                     join banner in banners on weeklyBucket.BannerId equals banner.Id
                                                                     join sku in skus.Include(x => x.ProductCategory).ThenInclude(cat => cat.UserUnilevers)
                                                                                     .Include(x => x.UserUnilevers).Where(y => y.UserUnilevers.Any(z => z.Id == user.Id) || y.ProductCategory.UserUnilevers.Any(u => u.Id == user.Id)) on weeklyBucket.SKUId equals sku.Id
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
            }
            else
            {
                approvals = (from approval in _db.Approvals
                             join proposal in (from proposal in _db.Proposals
                                               join weeklyBucket in (from weeklyBucket in _db.WeeklyBuckets
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
            }

            if (user.RoleUnilever.RoleName != "Administrator")
            {
                approvals = approvals.Where(x => x.ApproverWL == workLevel);
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


        public async Task<Approval> GetApprovalDetails(Guid approvalId)
        {
            var apprvl = _db.Approvals.Where(x => x.ApprovalStatus == ApprovalStatus.Pending || x.ApprovalStatus == ApprovalStatus.WaitingNextLevel).Include(x => x.ApprovalDetails).SingleOrDefault(x => x.Id == approvalId);
            if (apprvl != null)
            {
                var proposalThisApproval = _db.Proposals.Include(x => x.ProposalDetails.Where(y => y.ApprovalId == approvalId)).SingleOrDefault(x => x.ApprovalId == apprvl.Id);
                var weeklyBucketProposal = _db.WeeklyBuckets.Single(x => x.Id == proposalThisApproval.WeeklyBucketId);
                apprvl.ProposalSubmitDate = proposalThisApproval.SubmittedAt.ToString("dd/MM/yyyy");
                apprvl.Week = proposalThisApproval.Week;
                apprvl.RequestedBy = (await _db.UsersUnilever.SingleAsync(x => x.Id == proposalThisApproval.SubmittedBy)).Email;
                apprvl.ProposalType = proposalThisApproval.Type.Value;
                apprvl.Remark = proposalThisApproval.Remark;

                var andromeda = await _db.Andromedas.SingleOrDefaultAsync(x => x.SKUId == weeklyBucketProposal.SKUId);
                if(andromeda != null)
                {
                    apprvl.IsAndromedaPassed = andromeda.WeekCover > 2; 
                }
                var bottomPrice = await _db.BottomPrices.SingleOrDefaultAsync(x => x.SKUId == weeklyBucketProposal.SKUId);
                if (bottomPrice != null)
                {
                    apprvl.IsBottomPricePassed = bottomPrice.Remarks.ToUpper() == "ABOVE" || bottomPrice.Remarks.ToUpper() == "NORMAL";
                }
                var iTrust = await _db.ITrusts.SingleOrDefaultAsync(x => x.SKUId == weeklyBucketProposal.SKUId);
                if(iTrust != null)
                {
                    apprvl.IsITrustPassed = iTrust.DistStock > 3;
                }

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

        public async Task<List<Tuple<string, Guid, Approval, string>>> GetRecipientEmail(Approval approval)
        {
            var worklevel = await _db.WorkLevels.SingleAsync(x => x.WL == approval.ApproverWL);
            var users = await _db.UsersUnilever.Include(x => x.Banners).Where(x => x.WLId == worklevel.Id).ToListAsync();
            if (approval.ApproverWL == "KAM WL 2")
            {
                users = users.Where(x => x.Banners.Select(x => x.Id).Contains(approval.BannerId)).ToList();
            }


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

            var usersEmail = users.Select(x => new Tuple<string, Guid, Approval, string>(x.Email, approval.SKUId, approval, type)).ToList();
            //var usersEmail = users.Select(x => x.Email).ToList();
            return usersEmail;
        }

        public async Task<List<EmailApproval>> GenerateCombinedEmailProposal(List<Approval> approvals, string baseUrl, string? requestor = "")
        {
            var listEmail = new List<EmailApproval>();
            
            var approvalIds = approvals.Select(x => x.Id);
            var approvalGroupsSku = approvals.GroupBy(x => x.SKUId).ToList();

            var emails2 = new List<Tuple<string, Guid, Approval, string>>();
            foreach (var approval in approvals)
            {
                emails2.AddRange(await GetRecipientEmail(approval));
            }

            var emailGrps = emails2.GroupBy(x => new { x.Item1, x.Item2 }).ToList();
            foreach(var emailGroup in emailGrps)
            {
                var emailApproval = new EmailApproval();
                emailApproval.RecipientEmail = emailGroup.Key.Item1;
                emailApproval.Requestor = requestor;
                emailApproval.Subject = $"FSA Approval Request";
                emailApproval.Body = $"Hi, {emailGroup.Key.Item1}, " +
                                     $"<br> Please approve Proposal Request: ";
                                        
                foreach (var email in emailGroup)
                { 
                    var approval = email.Item3;
                    emailApproval.ApprovalUrl = baseUrl + "/" + approval.Id;
                    var banner = await _db.Banners.SingleOrDefaultAsync(x => x.Id == approval.BannerId);
                    var sku = await _db.SKUs.SingleOrDefaultAsync(x => x.Id == approval.SKUId);
                    emailApproval.Body +=  $"<br><br> {email.Item4} <br> <br> Banner: {banner.BannerName} " +
                                 $"<br> " +
                                 $"Plant Code: {banner.PlantCode} " +
                                 $"<br> " +
                                 $"Plant Name: {banner.PlantName} " +
                                 $"<br> " +
                                 $"PC Code: {sku.PCMap}" +
                                 $"<br>" +
                                 $"Description Map: {sku.DescriptionMap} " +
                                 $"<br>" +
                                 $"Link: <a href='{HtmlEncoder.Default.Encode(emailApproval.ApprovalUrl)}'>Detail</a> <br> <br>";
                }
                listEmail.Add(emailApproval);
            }
           
            return listEmail;
        }

        public async Task<List<EmailApproval>> GenerateEmailProposal(Approval approval, string url, string requestor)
        {
            var listEmail = new List<EmailApproval>();
            var type = string.Empty;
            var emails = await GetRecipientEmail(approval);
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
                var banner = _db.Banners.Single(x => x.Id == approval.BannerId);
                var sku = _db.SKUs.Single(x => x.Id == approval.SKUId);
                var emailApproval = new EmailApproval();
                emailApproval.RecipientEmail = email.Item1;
                emailApproval.ApprovalUrl = url;
                emailApproval.Name = email.Item1;
                emailApproval.Requestor = requestor;
                emailApproval.Subject = $"FSA {type} Approval Request";
                emailApproval.Body = $"Hi, {emailApproval.RecipientEmail}, " +
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

        public async Task<List<EmailApproval>> GenerateCombinedEmailApproval(List<Approval> approvals, string userApproverEmail, string approvalNote){
            var listEmailApproval = new List<EmailApproval>();
            var banners = _db.Banners;
            var skus = _db.SKUs;
            var proposals = _db.Proposals;
            var approvalGroups = approvals.GroupBy(x => new { x.RequestedBy, x.SKUId }).ToList();
            foreach(var approvalGroup in approvalGroups)
            {
                var emailApproval = new EmailApproval();
                emailApproval.Requestor = approvalGroup.Key.RequestedBy;
                emailApproval.RecipientEmail = approvalGroup.Key.RequestedBy;
                emailApproval.Subject = "FSA Proposal Request Status";
                emailApproval.Body = $"Hi, {approvalGroup.Key.RequestedBy}, <br><br>";
                foreach (var approval in approvalGroup)
                {
                    var type = string.Empty;
                    var sku = skus.SingleOrDefault(x => x.Id == approval.SKUId);
                    var banner = banners.SingleOrDefault(x => x.Id == approval.BannerId);
                    var emailBody = GenerateApprovalEmailBody(approval, approval.ProposalType, userApproverEmail, approvalNote, banner, sku);
                    emailApproval.Body += emailBody;
                }
                emailApproval.Body += "<br> Thank You.";
                listEmailApproval.Add(emailApproval);
            }
            return listEmailApproval;
        }

        public string GenerateApprovalEmailBody(Approval approval, ProposalType proposalType, string userApproverEmail, string approvalNote, Banner banner, SKU sku)
        {
            var body = string.Empty;
            var type = string.Empty;
            if (proposalType == ProposalType.ReallocateAcrossKAM)
            {
                type = "Reallocate Across KAM";
            }
            else if (proposalType == ProposalType.ReallocateAcrossCDM)
            {
                type = "Reallocate Across CDM";
            }
            else if (proposalType == ProposalType.ReallocateAcrossMT)
            {
                type = "Reallocate Across MT";
            }
            else if (proposalType == ProposalType.Rephase)
            {
                type = "Rephase";
            }
            else
            {
                type = "Propose Additional";
            }

            if (approval.ApprovalStatus == ApprovalStatus.WaitingNextLevel)
            {
                body = $"Your Proposal Request on " +
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
                                     $"has been approved by {userApproverEmail} and now waiting for next level approval. <br><br>";
            }
            else if (approval.ApprovalStatus == ApprovalStatus.Rejected)
            {
                body = $"Your Proposal Request on " +
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
                                    $"has been rejected by {userApproverEmail} because of {approvalNote}. <br><br>";
            }
            else
            {
                var note = !string.IsNullOrEmpty(approvalNote) ? $"Approval Note: {approvalNote} <br>" : string.Empty;
                body = $"Your Proposal Request on " +
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
                                    $"has been approved by {userApproverEmail}. <br><br>";
            }
            return body;
        }
    }

 
}

