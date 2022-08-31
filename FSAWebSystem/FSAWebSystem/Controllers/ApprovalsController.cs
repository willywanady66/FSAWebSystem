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
        private readonly IBannerService _bannerService;
        private readonly ISKUService _skuService;
        
        public ApprovalsController(FSAWebSystemDbContext context, IApprovalService approvalService, IProposalService proposalService, IBucketService bucketService, INotyfService notyfService, UserManager<FSAWebSystemUser> userManager, IUserService userService, IEmailService emailService, IBannerService bannerService, ISKUService skuService)
        {
            _context = context;
            _approvalService = approvalService;
            _proposalService = proposalService;
            _bucketService = bucketService;
            _notyfService = notyfService;
            _userManager = userManager;
            _userService = userService;
            _emailService = emailService;
            _bannerService = bannerService;
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
            if (id == null || _context.ApprovalDetail == null)
            {
                return NotFound();
            }

            try
            {
                var approval = await _approvalService.GetApprovalDetails((Guid)id);
                if (approval != null)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);

                    if(approval.ProposalType != ProposalType.Rephase)
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

                        var workLevel = (await _userService.GetAllWorkLevel().SingleAsync(x => x.Id == userUnilever.WLId)).WL;

                 
                        canApprove = approval.ApproverWL == workLevel;

                        isApproved = approval.ApprovedBy.Contains(userUnilever.Email);

                        //if (workLevel == "KAM WL 2" || workLevel == "CDM WL 3" || workLevel == "VP MTDA" || workLevel == "CORE VP")
                        //{
                        //    if (approval.Level != 2)
                        //    {
                        //        canApprove = false;
                        //    }
                        //}
                        //else if (workLevel == "SOM MT WL 1" || workLevel == "SOM MT WL 2" || workLevel == "CD DIRECTOR")
                        //{
                        //    if (approval.Level != 1)
                        //    {
                        //        canApprove = false;
                        //    }
                        //}
                        //else if (workLevel == "CCD")
                        //{
                        //    if (approval.Level != 3)
                        //    {
                        //        canApprove = false;
                        //    }
                        //}

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
                    else
                    {

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
                userUnilever.WLName = (await _userService.GetAllWorkLevel().SingleAsync(x => x.Id == userUnilever.WLId)).WL;
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
                var user = await _userManager.GetUserAsync(User);
                var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
                var currDate = DateTime.Now;
                var approval = await _approvalService.GetApprovalById(approvalId);
             
                var errorMessages = new List<string>();
                if(approval.ApprovedBy.Contains(User.Identity.Name))
                {
                    errorMessages.Add("You already approve this proposal");
                    _notyfService.Warning("Proposal Already Approved");
                    TempData["ErrorMessages"] = errorMessages;
                    return Ok();
                }

                var proposal = await _proposalService.GetProposalByApprovalId(approval.Id);
                var weeklyBucket = await _bucketService.GetWeeklyBucket(proposal.WeeklyBucketId);
                var banner = await _bannerService.GetBanner(weeklyBucket.BannerId);
                var sku = await _skuService.GetSKUById(weeklyBucket.SKUId);
                var userRequestor = await _userService.GetUser(proposal.SubmittedBy.Value);
                approval.ApprovedAt = currDate;
                approval.ApprovalStatus = ApprovalStatus.WaitingNextLevel;
                if(string.IsNullOrEmpty(approval.ApprovedBy))
                {
                    approval.ApprovedBy = User.Identity.Name;
                }
                else
                {
                    approval.ApprovedBy += ";" + User.Identity.Name;
                }

                if(string.IsNullOrEmpty(approval.ApprovalNote))
                {
                    approval.ApprovalNote = approvalNote;
                }
                else
                {
                    approval.ApprovalNote += ";" + approvalNote;
                }

                approval.Level -= 1;
                
                var page = Request.Scheme + "://" + Request.Host + Url.Action("Details", "Approvals", new { id = approval.Id });

                var emails = await _approvalService.GenerateEmailProposal(approval, page, userRequestor.Email, banner, sku);
                listEmail.AddRange(emails);

               
                //Update Weekly & Approval Done
                if (approval.Level == 0)
                {
                    approval.ApprovalStatus = ApprovalStatus.Approved;
                    proposal.IsWaitingApproval = false;
                    await UpdateWeeklyBuckets(proposal.ProposalDetails, proposal.Type.Value, proposal.Week, approval.Id);
                }
                else
                {
                    var nextApproverWL = _approvalService.GetWLApprover(approval);
                    approval.ApproverWL = nextApproverWL;
                }

                var approvalEmail = await _approvalService.GenerateEmailApproval(approval, User.Identity.Name, userRequestor.Email, approvalNote, banner, sku);
                await _emailService.SendEmailAsync(approvalEmail.RecipientEmail, approvalEmail.Subject, approvalEmail.Body);

                await _context.SaveChangesAsync();
                _notyfService.Success("Proposal Approved");

                if(approval.Level != 0)
                {
                    foreach (var email in listEmail)
                    {
                        await _emailService.SendEmailAsync(email.RecipientEmail, email.Subject, email.Body);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return BadRequest();
            }
            return Ok();
        }

        public async Task UpdateWeeklyBuckets(List<ProposalDetail> proposalDetails, ProposalType type, int week, Guid approvalId)
        {
            foreach(var detail in proposalDetails.Where(x => x.ApprovalId == approvalId))
            {
                var weeklyBucket = await _bucketService.GetWeeklyBucket(detail.WeeklyBucketId);
                if (type == ProposalType.Rephase)
                {
       
                    var nextBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).GetValue(weeklyBucket, null));
                    //NEXTWEEK
                    weeklyBucket.GetType().GetProperty("BucketWeek" + (week + 1).ToString()).SetValue(weeklyBucket, nextBucket - detail.Rephase);

                    var currentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week).ToString()).GetValue(weeklyBucket, null));
                    weeklyBucket.GetType().GetProperty("BucketWeek" + (week).ToString()).SetValue(weeklyBucket, currentBucket + detail.Rephase);
                }
                else
                {
                    weeklyBucket.MonthlyBucket = weeklyBucket.MonthlyBucket + detail.ProposeAdditional;
                    var currentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (week).ToString()).GetValue(weeklyBucket, null));
                    weeklyBucket.GetType().GetProperty("BucketWeek" + (week).ToString()).SetValue(weeklyBucket, currentBucket + detail.ProposeAdditional);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectProposal(Guid approvalId, string approvalNote)
        {
            try
            {
                var approval = await _approvalService.GetApprovalById(approvalId);
                var proposal = await _proposalService.GetProposalByApprovalId(approval.Id);
                var weeklyBucket = await _bucketService.GetWeeklyBucket(proposal.WeeklyBucketId);
                var banner = await _bannerService.GetBanner(weeklyBucket.BannerId);
                var sku = await _skuService.GetSKUById(weeklyBucket.SKUId);
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

                approval.ApprovedAt = DateTime.Now;
                approval.ApprovedBy = User.Identity.Name;
                approval.ApproverWL = string.Empty;
                proposal.IsWaitingApproval = false;

                await _context.SaveChangesAsync();

                var email = await _approvalService.GenerateEmailApproval(approval, User.Identity.Name, userRequestor.Email, approvalNote, banner, sku);
                await _emailService.SendEmailAsync(email.RecipientEmail, email.Subject, email.Body);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return BadRequest();
            }
            _notyfService.Warning("Proposal Rejected");
            return Ok();
        }

    }
}
