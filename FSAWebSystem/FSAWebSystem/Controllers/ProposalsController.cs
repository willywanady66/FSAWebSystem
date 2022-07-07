﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Controllers
{
    public class ProposalsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IProposalService _proposalService;
        private readonly ICalendarService _calendarService;

        public ProposalsController(FSAWebSystemDbContext context, IProposalService proposalService, ICalendarService calendarService)
        {
            _context = context;
            _proposalService = proposalService;
            _calendarService = calendarService;
        }

        // GET: Proposals
        public async Task<IActionResult> Index()
        {
            var currentDate = DateTime.Now.Date;
            var fsaDetail = await _calendarService.GetCurrentWeek(DateTime.Now.Date);
            var listProposal = await _proposalService.GetProposalForView(currentDate.Month, currentDate.Year, fsaDetail.Week);

            return View(listProposal);
        }

        // GET: Proposals/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Proposals == null)
            {
                return NotFound();
            }

            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proposal == null)
            {
                return NotFound();
            }

            return View(proposal);
        }

        // GET: Proposals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proposals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MonthlyBucketId,Rephase,ProposeAddional,Remark,SubmittedAt,SubmittedBy")] Proposal proposal)
        {
            if (ModelState.IsValid)
            {
                proposal.Id = Guid.NewGuid();
                _context.Add(proposal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(proposal);
        }

        // GET: Proposals/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Proposals == null)
            {
                return NotFound();
            }

            var proposal = await _context.Proposals.FindAsync(id);
            if (proposal == null)
            {
                return NotFound();
            }
            return View(proposal);
        }

        // POST: Proposals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,MonthlyBucketId,Rephase,ProposeAddional,Remark,SubmittedAt,SubmittedBy")] Proposal proposal)
        {
            if (id != proposal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proposal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProposalExists(proposal.Id))
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
            return View(proposal);
        }

        // GET: Proposals/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Proposals == null)
            {
                return NotFound();
            }

            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proposal == null)
            {
                return NotFound();
            }

            return View(proposal);
        }

        // POST: Proposals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Proposals == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.Proposals'  is null.");
            }
            var proposal = await _context.Proposals.FindAsync(id);
            if (proposal != null)
            {
                _context.Proposals.Remove(proposal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProposalExists(Guid id)
        {
          return (_context.Proposals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}