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

namespace FSAWebSystem.Controllers
{
    public class BannerPlantsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IBannerPlantService _bannerPlantService;
        private readonly IBannerService _bannerService;
        public BannerPlantsController(FSAWebSystemDbContext context, IBannerPlantService bannerPlantService, IBannerService bannerService)
        {
            _context = context;
            _bannerPlantService = bannerPlantService;
            _bannerService = bannerService;
         }


        // GET: Banners/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.BannerPlants == null)
            {
                return NotFound();
            }

            var banner = await _context.BannerPlants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // GET: Banners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Banners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Trade,BannerName,PlantName,PlantCode")] BannerPlant bannerPlant)
        {
            if (ModelState.IsValid)
            {
                var IsBannerPlantExist = await _bannerPlantService.IsBannerPlantExist(bannerPlant.Banner.BannerName);
                if(IsBannerPlantExist)
                {
                    ModelState.AddModelError("", "Banner Plant" + bannerPlant.Banner.BannerName + " already exist!");
                }
                else
                {
                    var savedBanner = await _bannerPlantService.SaveBannerPlant(bannerPlant, User.Identity.Name);
                    return RedirectToAction("Index", "Admin");
                }   
            }
            return View(bannerPlant);
            
        }

        // GET: Banners/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            await _bannerService.FillBannerDropdown(ViewData);
            await _bannerService.FillPlantDropdown(ViewData);

            var listBanner = (List<SelectListItem>)ViewData["ListBanner"];
            var listPlant = (List<SelectListItem>)ViewData["ListPlant"];
            if (id == null || _context.BannerPlants == null)
            {
                return NotFound();
            }

            var bannerPlant = await _context.BannerPlants.FindAsync(id);
            var selectedBanner = listBanner.SingleOrDefault(x => Guid.Parse(x.Value) == bannerPlant.Banner.Id);
            if(selectedBanner != null)
            {
                selectedBanner.Selected = true;
            }

            var selectedPlant = listPlant.SingleOrDefault(x => Guid.Parse(x.Value) == bannerPlant.Plant.Id);
            if(selectedPlant != null)
            {
                selectedPlant.Selected = true;
            }

            if (bannerPlant == null)
            {
                return NotFound();
            }
            return View(bannerPlant);
        }

        // POST: Banners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, CDM, Banner, Plant, KAM, IsActive")] BannerPlant bannerPlant, string plantId, string bannerId)
        {
            if (id != bannerPlant.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Banner");
            ModelState.Remove("Banner.Trade");
            ModelState.Remove("Banner.BannerName");
            ModelState.Remove("Plant");
            ModelState.Remove("Plant.PlantName");
            ModelState.Remove("Plant.PlantCode");
            ModelState.Remove("Trade");
            ModelState.Remove("PlantName");
            ModelState.Remove("PlantCode");
            ModelState.Remove("BannerName");

            await _bannerService.FillBannerDropdown(ViewData);
            await _bannerService.FillPlantDropdown(ViewData);

            if (ModelState.IsValid)
            {   
                try
                {
                    var savedBanner = await _bannerPlantService.GetBannerPlant(bannerPlant.Id);
                    var banner = _bannerService.GetAllBanner().Single(x => x.Id == Guid.Parse(bannerId));
                    var plant = _bannerService.GetAllPlant().Single(x => x.Id == Guid.Parse(plantId));
                    if (await _bannerPlantService.IsBannerPlantUsed(bannerPlant.Banner.BannerName, bannerPlant.Plant.Id))
                    {
                        ModelState.AddModelError("", "Cannot Edit, Banner is registered on User/ Bucket");
                        return View(bannerPlant);
                    }

                    //CHECKDUPLICATE
                 
                    savedBanner.Banner = banner;
                    savedBanner.Plant = plant;
                    savedBanner.KAM = bannerPlant.KAM;
                    savedBanner.CDM = bannerPlant.CDM;
                    savedBanner.IsActive = bannerPlant.IsActive;
                    await _bannerPlantService.UpdateBannerPlant(savedBanner, User.Identity.Name);
                    return RedirectToAction("Index", "Admin");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerExists(bannerPlant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(bannerPlant);
                }
            }
            return View(bannerPlant);
        }

        // GET: Banners/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            await ValidateDeleteBanner((Guid)id);
            return RedirectToAction("Index", "Admin");
        }

        // POST: Banners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.BannerPlants == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.Banners'  is null.");
            }
            var banner = await _context.BannerPlants.FindAsync(id);
            if (banner != null)
            {
                _context.BannerPlants.Remove(banner);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(Guid id)
        {
          return (_context.BannerPlants?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task ValidateDeleteBanner(Guid bannerId)
        {
            var banner = await _bannerPlantService.GetBannerPlant(bannerId);


        }
    }
}
