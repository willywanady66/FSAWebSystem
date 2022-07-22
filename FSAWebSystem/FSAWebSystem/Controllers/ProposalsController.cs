using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using FSAWebSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using FSAWebSystem.Areas.Identity.Data;
using static FSAWebSystem.Models.ViewModels.ProposalViewModel;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace FSAWebSystem.Controllers
{
    public class ProposalsController : Controller
    {
        private readonly FSAWebSystemDbContext _db;
        private readonly IProposalService _proposalService;
        private readonly ICalendarService _calendarService;
        private readonly IUserService _userService;
        private readonly INotyfService _notyfService;
        private readonly UserManager<FSAWebSystemUser> _userManager;

        public ProposalsController(FSAWebSystemDbContext db, IProposalService proposalService, ICalendarService calendarService, UserManager<FSAWebSystemUser> userManager, IUserService userService, INotyfService notyfService)
        {
            _db = db;
            _proposalService = proposalService;
            _calendarService = calendarService;
            _userManager = userManager;
            _userService = userService;
            _notyfService = notyfService;
        }

        // GET: Proposals


        [Authorize(Policy = ("ReqOnly"))]
        public async Task<IActionResult> Index(string message)
        {              
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetProposalPagination(DataTableParamProposal param)
        {
            var user = await _userManager.GetUserAsync(User);
            var userUnilever = await _userService.GetUser(Guid.Parse(user.Id));
            var bannersThisUser = userUnilever.Banners;
            List<Proposal> listProposal = new List<Proposal>();
            var listData = Json(new { });
            var data = new ProposalData();
            var fsaDetail = await _calendarService.GetCalendarDetail(DateTime.Now.Date);

            try
            {
                if (fsaDetail != null)
                {
                    if (!await _proposalService.IsProposalExist(fsaDetail))
                    {
                        data = await _proposalService.GetProposalForView(fsaDetail.Month, fsaDetail.Year, fsaDetail.Week, param, userUnilever.Id);
                    }
                }
                if (param.proposalInputs != null)
                {
                    var weeklyBucketIds = data.proposals.Select(x => x.WeeklyBucketId).ToList();
                    var proposalForUpdate = param.proposalInputs.Where(x => (!string.IsNullOrEmpty(x.remark) || x.proposeAdditional > 0 || x.rephase > 0) && weeklyBucketIds.Contains(Guid.Parse(x.weeklyBucketId))).ToList();
                    foreach (var proposalInput in proposalForUpdate)
                    {
                        var proposal = data.proposals.SingleOrDefault(x => x.WeeklyBucketId == Guid.Parse(proposalInput.weeklyBucketId));
                        proposal.Rephase = proposalInput.rephase;
                        proposal.ProposeAdditional = proposalInput.proposeAdditional;
                        proposal.Remark = proposalInput.remark;
                    }
                }
                listData = Json(new
                {

                    draw = param.draw,
                    recordsTotal = data.totalRecord,
                    recordsFiltered = data.totalRecord,
                    data = data.proposals
                });
            }
            catch(Exception ex)
            {

            }
          

            

            return listData;

        }


        [HttpPost]
        public async Task<IActionResult> SaveProposal(List<ProposalInput> proposals)
        {
            List<string> errorMessages = new List<string>();
            var message = string.Empty;
            ValidateProposalInput(proposals, errorMessages);
            List<Proposal> listProposal = new List<Proposal>();
            if (!errorMessages.Any() && !proposals.Any(x => string.IsNullOrEmpty(x.weeklyBucketId)))
            {
                var fsaDetail = await _calendarService.GetCalendarDetail(DateTime.Now.Date);
                foreach (var proposalInput in proposals)
                {
                    var proposal = new Proposal
                    {
                        Id = Guid.NewGuid(),
                        Week = fsaDetail.Week,
                        Year = fsaDetail.Year,
                        Month = fsaDetail.Month,
                        WeeklyBucketId = Guid.Parse(proposalInput.weeklyBucketId),
                        Rephase = proposalInput.rephase,
                        ProposeAdditional = proposalInput.proposeAdditional,
                        SubmittedAt = DateTime.Now,
                        SubmittedBy = User.Identity.Name,
                        Remark = proposalInput.remark,
                        ApprovalStatus = ApprovalStatus.Pending,
                    };
                    listProposal.Add(proposal);
                }
                await _proposalService.SaveProposals(listProposal);
                try
                {
                    await _db.SaveChangesAsync();
                    
                    message = "Your Proposal has been saved!";
                    _notyfService.Success(message);
                    return Ok(proposals);
                    
                }
                catch (Exception ex)
                {
                    message = "Submit Proposal Failed";
                    _notyfService.Warning(message);
                }
            }
            else
            {
                message = "Submit Proposal Failed";
            }

            TempData["ErrorMessages"] = errorMessages;
            return BadRequest();
        }
       
        public void ValidateProposalInput(List<ProposalInput> proposalInputs, List<string> errorMessages)
        {
            foreach (var proposal in proposalInputs)
            {
                if (proposal.rephase > proposal.nextWeekBucket)
                {
                    errorMessages.Add(string.Format("Cannot request rephase more than next week bucket value on Bucket Name: {0} and PCMap: {1}", proposal.bannerName, proposal.pcMap));
                }
            }
        }
    }
}
