using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using AspNetCoreHero.ToastNotification.Abstractions;
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Controllers
{
    public class SKUsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ISKUService _skuService;
        public SKUsController(FSAWebSystemDbContext context, INotyfService notyfService, ISKUService sKUService)
        {
            _context = context;
            _notyfService = notyfService;
            _skuService = sKUService;
        }

        // GET: SKUs
        public async Task<IActionResult> Index()
        {
              return _context.SKUs != null ? 
                          View(await _context.SKUs.ToListAsync()) :
                          Problem("Entity set 'FSAWebSystemDbContext.SKUs'  is null.");
        }

        // GET: SKUs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.SKUs == null)
            {
                return NotFound();
            }

            var sKU = await _context.SKUs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sKU == null)
            {
                return NotFound();
            }

            return View(sKU);
        }

        // GET: SKUs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SKUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PCMap,DescriptionMap,IsActive,CreatedAt,CreatedBy,ModifiedAt,ModifiedBy")] SKU sKU)
        {
            if (ModelState.IsValid)
            {
                sKU.Id = Guid.NewGuid();
                _context.Add(sKU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sKU);
        }

        // GET: SKUs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.SKUs == null)
            {
                return NotFound();
            }

            var sKU = await _context.SKUs.FindAsync(id);
            if (sKU == null)
            {
                return NotFound();
            }
            return View(sKU);
        }

        // POST: SKUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,PCMap,DescriptionMap,IsActive,CreatedAt,CreatedBy,ModifiedAt,ModifiedBy")] SKU sKU)
        {
            //ModelState.Remove("IsActive");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("ModifiedAt");
            ModelState.Remove("ModifiedBy");
            ModelState.Remove("Status");
            ModelState.Remove("ProductCategory");
            ModelState.Remove("Category");
            if (id != sKU.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var savedSKU = await _skuService.GetSKU(sKU.PCMap);
                    if(savedSKU != null)
                    {
                        if (savedSKU.Id != sKU.Id)
                        {
                            ModelState.AddModelError("", "PCMap: " + sKU.PCMap + " already exist");
                            _notyfService.Error("Update SKU Failed!");
                            return View(sKU);
                        }
                    }
                   
                    _context.Update(sKU);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("SKU Updated!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SKUExists(sKU.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Admin");
            }
            return View(sKU);
        }



        private bool SKUExists(Guid id)
        {
          return (_context.SKUs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
