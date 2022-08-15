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
        
        public ApprovalsController(FSAWebSystemDbContext context, IApprovalService approvalService, IProposalService proposalService, IBucketService bucketService, INotyfService notyfService, UserManager<FSAWebSystemUser> userManager, IUserService userService)
        {
            _context = context;
            _approvalService = approvalService;
            _proposalService = proposalService;
            _bucketService = bucketService;
            _notyfService = notyfService;
            _userManager = userManager;
            _userService = userService;
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
            if (id == null || _context.ApprovalDetail == null)
            {
                return NotFound();
            }

            var approval = await _approvalService.GetApprovalDetails((Guid)id);

            if(approval != null)
            {
                var bannerTarget = approval.ApprovalDetails.SingleOrDefault(x => x.ProposeAdditional > 0).BannerName;
                ViewData["BannerSource"] = approval.ApprovalDetails.SingleOrDefault(x => x.ProposeAdditional < 0).BannerName;


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
                return View(approval);
            }

            return NotFound();
            
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

            }
            return listData;
        }

		//[HttpPost]
  //      public async Task<IActionResult> GetApprovalReallocatePagination(DataTableParam param)
		//{
  //          var listData = Json(new { });
  //          try
  //          {

  //              var currentDate = DateTime.Now;
  //              var data = await _approvalService.GetApprovalReallocatePagination(param, currentDate.Month, currentDate.Year);
  //              listData = Json(new
  //              {
  //                  draw = param.draw,
  //                  recordsTotal = data.totalRecord,
  //                  recordsFiltered = data.totalRecord,
  //                  data = data.approvals
  //              });
  //          }
  //          catch (Exception ex)
  //          {

  //          }
  //          return listData;
  //      }
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

        // POST: Approvals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ProposalId,SubmittedBy,SubmittedAt,ApprovalStatus,RejectionReason,ApprovedBy,ApprovedAt")] Approval approval)
        {
            if (id != approval.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(approval);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApprovalExists(approval.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(approval);
        }

        [Authorize(Policy = ("ApprovalPage"))]

        [HttpPost]
        public async Task<IActionResult> ApproveProposal(Guid approvalId)
        {
            try
            {
                

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
                }

                var proposal = await _proposalService.GetProposalByApprovalId(approval.Id);

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

                approval.Level -= 1;

                //Update Weekly & Approval Done
                if(approval.Level == 0)
                {
                    approval.ApprovalStatus = ApprovalStatus.Approved;
                    proposal.IsWaitingApproval = false;
                    await UpdateWeeklyBuckets(proposal.ProposalDetails);
                }

               
                await _context.SaveChangesAsync();
                _notyfService.Success("Proposal Approved");
                
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        public async Task UpdateWeeklyBuckets(List<ProposalDetail> proposalDetails)
        {
            foreach(var detail in proposalDetails)
            {
                var weeklyBucket = await _bucketService.GetWeeklyBucket(detail.WeeklyBucketId);
                weeklyBucket.MonthlyBucket = weeklyBucket.MonthlyBucket + detail.ProposeAdditional;  
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectProposal(Guid approvalId, string rejectionReason)
        {
            try
            {
                var approval = await _approvalService.GetApprovalById(approvalId);
                var proposal = await _proposalService.GetProposalByApprovalId(approval.Id);

                approval.ApprovalStatus = ApprovalStatus.Rejected;
                approval.RejectionReason = rejectionReason;
                approval.ApprovedAt = DateTime.Now;
                approval.ApprovedBy = User.Identity.Name;

                proposal.IsWaitingApproval = false;

                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
            _notyfService.Warning("Proposal Rejected");
            return Ok();
        }

        private bool ApprovalExists(Guid id)
        {
          return (_context.Approvals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
