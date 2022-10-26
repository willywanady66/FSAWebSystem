 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using AspNetCoreHero.ToastNotification.Abstractions;
using FSAWebSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FSAWebSystem.Services;
using System.Text.Encodings.Web;
using FSAWebSystem.Models.Bucket;

namespace FSAWebSystem.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IApprovalService _approvalService;
        private readonly IProposalService _proposalService;
        private readonly IBucketService _bucketService;
        private readonly UserManager<FSAWebSystemUser> _userManager;
        private readonly INotyfService _notyfService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService; 
        private readonly IBannerPlantService _bannerPlantService;
        private readonly ISKUService _skuService;
        
        public ApprovalsController(FSAWebSystemDbContext context, IApprovalService approvalService, IProposalService proposalService, IBucketService bucketService, INotyfService notyfService, UserManager<FSAWebSystemUser> userManager, IUserService userService, IEmailService emailService, IBannerPlantService bannerService, ISKUService skuService)
        {
            _context = context;
            _approvalService = approvalService;
            _proposalService = proposalService;
            _bucketService = bucketService;
            _notyfService = notyfService;
            _userManager = userManager;
            _userService = userService;
            _emailService = emailService;
            _bannerPlantService = bannerService;
            _skuService = skuService;
        }

        [Authorize(Policy = ("ApprovalPage"))]
        // GET: Approvals
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize(Policy = ("ApprovalPage"))]
        // GET: Approvals/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var canApprove = true;
            ViewData["CanApprove"] = false;
            ViewData["IsApproved"] = false;
            var isApproved = false;
            

            try
            {
                var workLevel = string.Empty;
                var approval = await _approvalService.GetApprovalDetails((Guid)id);
                if (approval != null)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
                    var wl = (await _userService.GetAllWorkLevel().SingleOrDefaultAsync(x => x.Id == userUnilever.WLId));
                    if(wl != null)
                    {
                        workLevel = wl.WL;
                    }
                    
                    if (approval.ProposalType != ProposalType.Rephase)
                    {
                        var appovaldetailTarget = approval.ApprovalDetails.SingleOrDefault(x => x.ProposeAdditional > 0);
                        if (appovaldetailTarget != null)
                        {
                            ViewData["BannerTarget"] = appovaldetailTarget.BannerName;
                        }

                        var appovaldetailSource = approval.ApprovalDetails.SingleOrDefault(x => x.ProposeAdditional < 0);
                        if (appovaldetailSource != null)
                        {
                            ViewData["BannerSource"] = appovaldetailSource.BannerName;
                        }

                       

                    

                        isApproved = approval.ApprovedBy.Split(';').Last().Contains(userUnilever.Email);


                        if (approval.ProposalType == ProposalType.ReallocateAcrossKAM)
                        {
                            ViewData["Type"] = "Reallocate Across KAM";
                        }
                        else if (approval.ProposalType == ProposalType.ReallocateAcrossCDM)
                        {
                            ViewData["Type"] = "Reallocate Across CDM";
                        }
                        else if (approval.ProposalType == ProposalType.ReallocateAcrossMT)
                        {
                            ViewData["Type"] = "Reallocate Across MT";
                        }
                        else
                        {
                            ViewData["Type"] = "Propose Additional";
                        }
                    }

                    if (userUnilever.RoleUnilever.RoleName != "Administrator")
                    {
                        canApprove = approval.ApproverWL == workLevel;
                    }

                    var approvalNoteList = new List<Tuple<string, string, string>>();

                    if(!string.IsNullOrEmpty(approval.ApprovalNote))
                    {
                        var approvalNotes = approval.ApprovalNote.Split(';').ToList();
                        var approvers = approval.ApprovedBy.Split(';').ToList();

                        for (int i = 0; i < approvalNotes.Count; i++)
                        {
                            var approver = await _userService.GetUserByEmail(approvers[i]);
                            var worklevel = _userService.GetAllWorkLevel().Single(x => x.Id == approver.WLId);
                            var note = new Tuple<string, string, string>(approvers[i].ToString(), worklevel.WL, approvalNotes[i].ToString());
                            approvalNoteList.Add(note);
                        }
                    }
           
                 

                    ViewData["ApprovalNotes"] = approvalNoteList;

                    ViewData["CanApprove"] = canApprove;
                    ViewData["IsApproved"] = isApproved;
                    if(approval.ProposalType != ProposalType.Rephase && approval.ProposalType != ProposalType.ProposeAdditional)
                    {
                        if(approval.ProposalType == ProposalType.ReallocateAcrossKAM)
                        {
                            approval.NextProposalType = ProposalType.ReallocateAcrossCDM;
                        }
						else if(approval.ProposalType == ProposalType.ReallocateAcrossCDM)
						{
                            approval.NextProposalType = ProposalType.ReallocateAcrossMT;
						}
						else
						{
                            approval.NextProposalType = ProposalType.ProposeAdditional;
						}
                    }
                    return View(approval);
                }
            }
            catch (Exception ex)
            {

            }
            return View();
                
        }


 
   

        [Authorize(Policy = ("ApprovalPage"))]
        [HttpPost]
        public async Task<IActionResult> GetApprovalPagination(DataTableParam param)
        {
            var listData = Json(new { });
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
                var wl = (await _userService.GetAllWorkLevel().SingleOrDefaultAsync(x => x.Id == userUnilever.WLId));
                if (wl != null)
                {
                    userUnilever.WLName = wl.WL;
                }
                   
                var currentDate = DateTime.Now;
            
                var data = await _approvalService.GetApprovalPagination(param, currentDate.Month, currentDate.Year, userUnilever);
                listData = Json(new
                {
                    draw = param.draw,
                    recordsTotal = data.totalRecord,
                    recordsFiltered = data.totalRecord,
                    data = data.approvals
                });
            }
            catch (Exception ex)
            {
                listData = Json(new
                {
                    error = ex.Message
                });
            }
            return listData;
        }


        // GET: Approvals/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Approvals == null)
            {
                return NotFound();
            }

            var approval = await _context.Approvals.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }
            return View(approval);
        }

      

        [Authorize(Policy = ("ApprovalPage"))]
        [HttpPost]
        public async Task<IActionResult> ApproveProposal(Guid approvalId, string approvalNote)
        {
            try
            {
                
                var listEmail = new List<EmailApproval>();
                var approval = await _approvalService.GetApprovalById(approvalId);
             
                var errorMessages = new List<string>();
                var approvalIds = new List<Guid>();
                approvalIds.Add(approvalId);
                if (approval.ApprovedBy.Split(';').Last().Contains(User.Identity.Name))
                {
                    errorMessages.Add("You already approve this proposal");
                    _notyfService.Warning("Proposal Already Approved");
                    TempData["ErrorMessages"] = errorMessages;
                    return Ok(Json( new{ errorMessages}));
                }
                else
                {

                    await Approve(approvalIds, approvalNote);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _notyfService.Error("Approve Proposal Failed");
                return Ok(Json(new { ex.Message }));
            }
            _notyfService.Success("Proposal Approved");
            return Ok();
        }

        public async Task UpdateWeeklyBuckets(Proposal proposal, ProposalType type, int week)
        {
            foreach (var detail in proposal.ProposalDetails)
            {
                var weeklyBucket = await _bucketService.GetWeeklyBucket(detail.WeeklyBucketId);
                if (type == ProposalType.Rephase)
                {

                    var nextBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).GetValue(weeklyBucket, null));
                    //NEXTWEEK
                    weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).SetValue(weeklyBucket, nextBucket - detail.ActualRephase);

                    var currentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week).ToString()).GetValue(weeklyBucket, null));
                    weeklyBucket.GetType().GetProperty("BucketWeek" + (week).ToString()).SetValue(weeklyBucket, currentBucket + detail.ActualRephase);
                }
                else
                {
                    weeklyBucket.MonthlyBucket = weeklyBucket.MonthlyBucket + detail.ActualProposeAdditional;
                    var currentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week).ToString()).GetValue(weeklyBucket, null));
                    weeklyBucket.GetType().GetProperty("BucketWeek" + (week).ToString()).SetValue(weeklyBucket, currentBucket + detail.ActualProposeAdditional);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectProposal(Guid approvalId, string approvalNote)
        {
            try
            {
                var approvalIds = new List<Guid>
                {
                    approvalId
                };
                var approval = await _approvalService.GetApprovalById(approvalId);
                var errorMessages = new List<string>();
                if (approval.ApprovedBy.Split(';').Last().Contains(User.Identity.Name))
                {
                    var type = string.Empty;
                    if (approval.ApprovalStatus ==ApprovalStatus.Rejected)
                    {
                        type = "rejected";
                    }
                    else if(approval.ApprovalStatus != ApprovalStatus.Pending)
                    {
                        type = "approved";
                    }
                    errorMessages.Add("You already " + type + " this proposal");
                    _notyfService.Warning("Proposal already " + type);
                    TempData["ErrorMessages"] = errorMessages;
                    return Ok(Json(new { errorMessages }));
                }
                else
                {
                    await Reject(approvalIds, approvalNote);
                }
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return BadRequest();
            }
            _notyfService.Warning("Proposal Rejected");
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> RejectProposals(List<Guid> approvalIds, string approvalNote)
        {
            try
            {
                await Reject(approvalIds, approvalNote);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessages"] = ex.Message;
                _notyfService.Error("Reject Proposals Failed");
                return BadRequest();
            }
            _notyfService.Warning("Proposals Rejected");
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> ApproveProposals(List<Guid> approvalIds, string approvalNote)
        {
            try
            {
                await Approve(approvalIds, approvalNote);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessages"] = ex.Message;
                _notyfService.Error("Approve Proposals Failed");
                return BadRequest();
            }
            _notyfService.Success("Proposals Approved");
            return Ok();
        }
        public async Task Approve(List<Guid> approvalIds, string approvalNote)
        {
            var user = await _userManager.GetUserAsync(User);
            //var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
            var currDate = DateTime.Now;
            var listApproval = new List<Approval>();
            var listEmail = new List<EmailApproval>();

            try
            {
                foreach (var approvalId in approvalIds)
                {

                    var approval = await _approvalService.GetApprovalById(approvalId);
                    //var proposal = await _proposalService.GetProposalByApprovalId(approval.Id);
                    var proposal = approval.Proposal;
                    var weeklyBucket = new WeeklyBucket();
                    //var weeklyBucket = await _bucketService.GetWeeklyBucket(proposal.WeeklyBucketId);
                    //var banner = await _bannerPlantService.GetBannerPlant(weeklyBucket.BannerId);
                    //var sku = await _skuService.GetSKUById(weeklyBucket.SKUId);
                    var userRequestor = await _userService.GetUser(proposal.SubmittedBy.Value);
                    approval.ApprovedAt = currDate;
     
                    approval.ApprovalStatus = ApprovalStatus.WaitingNextLevel;
                    approval.RequestedBy = userRequestor.Email;
                    if (string.IsNullOrEmpty(approval.ApprovedBy))
                    {
                        approval.ApprovedBy = User.Identity.Name;
                    }
                    else
                    {
                        approval.ApprovedBy += ";" + User.Identity.Name;
                    }

                    if (string.IsNullOrEmpty(approval.ApprovalNote))
                    {
                        approval.ApprovalNote = approvalNote;
                    }
                    else
                    {
                        approval.ApprovalNote += ";" + approvalNote;
                    }


                    approval.Level -= 1;
                    //Update Weekly & Approval Done
                    if (approval.Level == 0)
                    {
                        approval.ApprovalStatus = ApprovalStatus.Approved;
                        proposal.IsWaitingApproval = false;
                        proposal.SubmittedBy = Guid.Empty;
                        await UpdateWeeklyBuckets(proposal, proposal.Type.Value, proposal.Week);
                    }
                    else
                    {
                        var nextApproverWL = _approvalService.GetWLApprover(approval);
                        approval.ApproverWL = nextApproverWL;

                        
                    }
                    listApproval.Add(approval);
                }

                await _context.SaveChangesAsync();
                var baseUrl = Request.Scheme + "://" + Request.Host + Url.Action("Details", "Approvals");
                var proposalEmails = await _approvalService.GenerateCombinedEmailProposal(listApproval, baseUrl);
                var approvalEmails = await _approvalService.GenerateCombinedEmailApproval(listApproval, User.Identity.Name, approvalNote);
                listEmail.AddRange(proposalEmails);
                listEmail.AddRange(approvalEmails);
                foreach (var email in listEmail)
                {
                    await _emailService.SendEmailAsync(email.RecipientEmail, email.Subject, email.Body);
                }
            }
            catch(Exception ex)
            {

            }
          
        }

        public async Task Reject(List<Guid> approvalIds, string approvalNote)
        {
            var listApproval = new List<Approval>();
            var listEmail = new List<EmailApproval>();
            foreach (var approvalId in approvalIds)
            {
                var approval = await _approvalService.GetApprovalById(approvalId);
                //var proposal = await _proposalService.GetProposalByApprovalId(approval.Id);
                var proposal = new Proposal();
                var weeklyBucket = new WeeklyBucket();
                //var weeklyBucket = await _bucketService.GetWeeklyBucket(proposal.WeeklyBucketId);
                approval.BannerPlantId = weeklyBucket.BannerPlant.Id;
                approval.SKUId = weeklyBucket.SKUId;
                var userRequestor = await _userService.GetUser(proposal.SubmittedBy.Value);
           

                approval.ApprovalStatus = ApprovalStatus.Rejected;
                if (string.IsNullOrEmpty(approval.ApprovalNote))
                {
                    approval.ApprovalNote = approvalNote;
                }
                else
                {
                    approval.ApprovalNote += ";" + approvalNote;
                }  

                if (string.IsNullOrEmpty(approval.ApprovedBy))
                {
                    approval.ApprovedBy = User.Identity.Name;
                }
                else
                {
                    approval.ApprovedBy += ";" + User.Identity.Name;
                }

                approval.ApprovedAt = DateTime.Now;
                approval.ApproverWL = string.Empty;
                proposal.IsWaitingApproval = false;
                proposal.SubmittedBy = Guid.Empty;
                approval.RequestedBy = userRequestor.Email;

                listApproval.Add(approval);
            }
            await _context.SaveChangesAsync();
            var approvalEmails = await _approvalService.GenerateCombinedEmailApproval(listApproval, User.Identity.Name, approvalNote);
            listEmail.AddRange(approvalEmails);
            foreach (var email in listEmail)
            {
                await _emailService.SendEmailAsync(email.RecipientEmail, email.Subject, email.Body);
            }

        }

		[HttpPost]
        public async Task<IActionResult> RequestApprovalNextType(Guid approvalId, string approvalNote)
		{
			try
			{
                var approval = await _approvalService.GetApprovalById(approvalId);
                //var proposal = await _proposalService.GetProposalByApprovalId(approvalId);
                var proposal = new Proposal();
                var proposalTypeInit = proposal.Type;
                var proposalHistory = await _proposalService.GetProposalHistory(approvalId);
                var weeklyBucket = new WeeklyBucket();
                //var weeklyBucket = await _bucketService.GetWeeklyBucket(proposal.WeeklyBucketId);
                var weekBucketTarget = await GetWeeklyBucketTarget(approval, proposal);
                proposalHistory.BannerId = weekBucketTarget.BannerPlant.Banner.Id;
                //var proposalDetailTarget = proposal.ProposalDetails.Single(x => x.ProposeAdditional < 0);
                var proposalDetailTarget = new ProposalDetail();

                if(((int)proposalTypeInit) + 1 != (int)proposal.Type)
                {
                    var message = string.Empty;
                    if(proposal.Type == ProposalType.ReallocateAcrossCDM)
                    {
                        message = "Across CDM";
                    }
                    else if(proposal.Type == ProposalType.ReallocateAcrossMT)
                    {
                        message = "Across MT";
                    }
                    else
                    {
                        message = "Propose Additional";
                    }
                    approvalNote += " (Skipped to " + message + ")";
                }

                if(weekBucketTarget.Id == Guid.Empty)
                {
                    proposal.ProposalDetails.Remove(proposalDetailTarget);
                }
                else {
                    //proposalDetailTarget.WeeklyBucketId = weekBucketTarget.Id;
                }
              
                var userRequestor = await _userService.GetUser(proposal.SubmittedBy.Value);
                approval.ProposalType = proposal.Type.Value;
                approval.Level = approval.ProposalType == ProposalType.ProposeAdditional ? 3 : 2;
                approval.ApproverWL = _approvalService.GetWLApprover(approval);
                approval.SKUId = weeklyBucket.SKUId;
                approval.BannerPlantId = weeklyBucket.BannerPlant.Id;

                if (string.IsNullOrEmpty(approval.ApprovedBy))
                {
                    approval.ApprovedBy = User.Identity.Name;
                }
                else
                {
                    approval.ApprovedBy += ";" + User.Identity.Name;
                }

                if (string.IsNullOrEmpty(approval.ApprovalNote))
                {
                    approval.ApprovalNote = approvalNote;
                }
                else
                {
                    approval.ApprovalNote += ";" + approvalNote;
                }

                

                await _context.SaveChangesAsync();
                
                var listEmail = new List<EmailApproval>();
                var baseUrl = Request.Scheme + "://" + Request.Host + Url.Action("Index", "Approvals");
                var proposalEmails = await _approvalService.GenerateEmailProposal(approval, baseUrl, userRequestor.Email);
                listEmail.AddRange(proposalEmails);
                foreach (var email in listEmail)
                {
                    await _emailService.SendEmailAsync(email.RecipientEmail, email.Subject, email.Body);
                }
            }
            catch(Exception ex)
			{

			}

            return Ok();
        }

        public async Task<WeeklyBucket> GetWeeklyBucketTarget(Approval approval, Proposal proposal)
		{
            var i = 0;
            var bucketTargetIds = new List<Guid>();
            var weeklyBucketTarget = new WeeklyBucket();
            var weeklyBucket = new WeeklyBucket();
            //var weeklyBucket = await _bucketService.GetWeeklyBucket(proposal.WeeklyBucketId);
            var sku = await _skuService.GetSKUById(weeklyBucket.SKUId);
            var bannerPlants = _bannerPlantService.GetAllActiveBannerPlant();
            var weeklyBuckets = _bucketService.GetWeeklyBuckets();
            var bannerPlant = await bannerPlants.SingleOrDefaultAsync(x => x.Id == weeklyBucket.BannerPlant.Id);
            //var currentBucketTargetId = proposal.ProposalDetails.Single(x=> x.ProposeAdditional < 0).WeeklyBucketId;
            var currentBucketTargetId = Guid.Empty;
            var currentBucketTarget = weeklyBuckets.Single(x => x.Id == currentBucketTargetId);
            var currentBannerTarget = bannerPlants.Single(x => x.Id == currentBucketTarget.BannerPlant.Id);
       
            if(approval.ProposalType == ProposalType.ReallocateAcrossKAM)
			{
                i = 0;
			}
            else if (approval.ProposalType == ProposalType.ReallocateAcrossCDM)
			{
                i = 1;
			}
			else
			{
                i = 2;
			}

            while (i != 3)
            {
                
                switch (i)
                {
                    //REALLOCATE ACROSS CDM
                    case 0:
                        var bannerTargetIdsCDM = bannerPlants.Where(x => x.KAM != bannerPlant.KAM && x.CDM == bannerPlant.CDM).Select(x => x.Id);
                        weeklyBuckets = _bucketService.GetWeeklyBuckets().Where(x => bucketTargetIds.Contains(x.BannerPlant.Id) && x.SKUId == sku.Id);
                        proposal.Type = ProposalType.ReallocateAcrossCDM;
                        break;
                    //REALLOCATE ACROSS MT
                    case 1:
                        var bannerTargetIds = bannerPlants.Where(x => x.KAM != bannerPlant.KAM && x.CDM != bannerPlant.CDM).Select(x => x.Id);
                        weeklyBuckets = _bucketService.GetWeeklyBuckets().Where(x => x.SKUId == sku.Id && bannerTargetIds.Contains(x.BannerPlant.Id));
                        var zz = weeklyBuckets.ToList();
                        proposal.Type = ProposalType.ReallocateAcrossMT;
                        break;
                    default:
                        proposal.Type = ProposalType.ProposeAdditional;
                        break;
                }

                if(proposal.Type != ProposalType.ProposeAdditional)
				{
                    approval.ProposalType = proposal.Type.Value;
                    //var proposeAdditional = proposal.ProposalDetails.Single(x => x.ProposeAdditional > 0).ProposeAdditional;
                    var proposeAdditional = 0;
                    var weeklyBucketTargetByMonthly = await weeklyBuckets.Where(x => x.MonthlyBucket > proposeAdditional && x.Id != weeklyBucket.Id && x.Month == proposal.Month).OrderByDescending(x => x.MonthlyBucket).FirstOrDefaultAsync();
                    var weeklyBucketTargetByRemFSA = await weeklyBuckets.Where(x => x.RemFSA > proposeAdditional && x.Id != weeklyBucket.Id && x.Month == proposal.Month).OrderByDescending(x => x.RemFSA).FirstOrDefaultAsync();
                    if (weeklyBucketTargetByMonthly == null && weeklyBucketTargetByRemFSA == null)
                    {
                        proposal.Type = ProposalType.ProposeAdditional;
                        i++;
                    }
                    else if (weeklyBucketTargetByMonthly != null && weeklyBucketTargetByRemFSA != null)
                    {
                        if (weeklyBucketTargetByMonthly.MonthlyBucket > weeklyBucketTargetByRemFSA.RemFSA)
                        {
                            weeklyBucketTarget = weeklyBucketTargetByMonthly;
                            break;
                        }
                        else
                        {
                            weeklyBucketTarget = weeklyBucketTargetByRemFSA;
                            break;
                        }
                    }
                    else
                    {
                        weeklyBucketTarget = weeklyBucketTargetByMonthly != null ? weeklyBucketTargetByMonthly : weeklyBucketTargetByRemFSA;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return weeklyBucketTarget;
        } 
    }
}
