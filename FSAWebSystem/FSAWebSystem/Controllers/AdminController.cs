using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static FSAWebSystem.Controllers.UploadDocumentController;

namespace FSAWebSystem.Controllers
{


    public class AdminController : Controller
    {
        private readonly FSAWebSystemDbContext _db;
        private IUserService _userService;
        private IBannerService _bannerService;
        private IRoleService _roleServices;
        private ISKUService _skuService;
        private ICalendarService _calendarService;
        public AdminController(FSAWebSystemDbContext db, IUserService userSvc, IBannerService bannerSvc, IRoleService roleSvc, ISKUService skuService, ICalendarService calendarService)
        {
            _db = db;
            _userService = userSvc;
            _bannerService = bannerSvc;
            _roleServices = roleSvc;
            _skuService = skuService;
            _calendarService = calendarService;
        }

        [Authorize(Policy ="AdminOnly")]
		
        public async Task<IActionResult> Index()
        {
            var currentDate = DateTime.Now;
            List<Banner> listBanners = await _bannerService.GetAllBanner().ToListAsync();
            var listDocumentUpload = Enum.GetValues(typeof(DocumentUpload)).Cast<DocumentUpload>().Select(x => new SelectListItem { Text = UploadDocumentService.GetEnumDesc(x), Value = ((int) x).ToString() }).ToList();
            List<UserUnilever> listUsers = await _userService.GetAllUsers();
             foreach(var user in listUsers)
            {
                if(user.Banners.Count == listBanners.Where(x => x.IsActive).Count())
				{
                    user.BannerName = "All Banners";
				}
				else
                {
                    user.BannerName = String.Join(", ", user.Banners.Select(x => x.BannerName));
                }
               
            }
            
            List<RoleUnilever> listRoles = await _roleServices.GetAllRoles().ToListAsync();
            List<ProductCategory> listCategory = await _skuService.GetAllProductCategories().ToListAsync();
            List<SKU> listSKU = await _skuService.GetAllProducts().ToListAsync();
            FSACalendarHeader fsaCalendar = await _calendarService.GetFSACalendarHeader(currentDate.Month, currentDate.Year);
            AdminModel model = new AdminModel
            {
                Users = listUsers,
                Banners = listBanners,
                Roles = listRoles,
                SKUs = listSKU,
                Categories = listCategory,
                DocumentUploads = listDocumentUpload,
                LoggedUser = User.Identity.Name,
                FSACalendar = fsaCalendar
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> UploadDocument(IFormFile excelDocument, string document)
		{
            return View();
		}

        public class AdminModel
        {
            public List<UserUnilever> Users { get; set; }
            public List<Banner> Banners { get; set; }
            public List<SKU> SKUs { get; set; }
            public List<RoleUnilever> Roles { get; set; }
            public List<ProductCategory> Categories{ get; set; }

            public List<SelectListItem> DocumentUploads { get; set; }
            public FSACalendarHeader FSACalendar { get; set; }
            public string LoggedUser { get; set; }
        }

        
    }


   
}
