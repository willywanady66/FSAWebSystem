using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FSAWebSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<FSAWebSystemUser> _userManager;
        private readonly IRoleService _roleService;

        public HomeController(ILogger<HomeController> logger,UserManager<FSAWebSystemUser> userManager, IRoleService roleService)
        {
            _logger = logger;
            _userManager = userManager;
            _roleService = roleService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            //var menus = (await _roleService.GetRoleByName(user.Role)).Menus.Select(x => x.Name).ToList();
            //User.
            //ViewData["Menus"] = menus;
            return View(user);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Report()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}