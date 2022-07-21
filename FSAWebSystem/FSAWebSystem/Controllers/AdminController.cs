using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
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
            //List<Banner> listBanners = await _bannerService.GetAllBanner().ToListAsync();
            var listDocumentUpload = Enum.GetValues(typeof(DocumentUpload)).Cast<DocumentUpload>().Select(x => new SelectListItem { Text = UploadDocumentService.GetEnumDesc(x), Value = ((int) x).ToString() }).ToList();
            //List<UserUnilever> listUsers = await _userService.GetAllUsers();
    //         foreach(var user in listUsers)
    //        {
    //            if(user.Banners.Count == listBanners.Where(x => x.IsActive).Count())
				//{
    //                user.BannerName = "All Banners";
				//}
				//else
    //            {
    //                user.BannerName = String.Join(", ", user.Banners.Select(x => x.BannerName));
    //            }
               
    //        }
            
            List<RoleUnilever> listRoles = await _roleServices.GetAllRoles().ToListAsync();
            //List<ProductCategory> listCategory = await _skuService.GetAllProductCategories().ToListAsync();
            //List<SKU> listSKU = await _skuService.GetAllProducts().ToListAsync();
            FSACalendarHeader fsaCalendar = await _calendarService.GetFSACalendarHeader(currentDate.Month, currentDate.Year);
            AdminModel model = new AdminModel
            {
                //Users = listUsers,
                //Banners = listBanners,
                Roles = listRoles,
                //SKUs = listSKU,
                //Categories = listCategory,
                DocumentUploads = listDocumentUpload,
                LoggedUser = User.Identity.Name,
                FSACalendar = fsaCalendar
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserPagination(DataTableParam param)
        {
            List<Banner> listBanners = await _bannerService.GetAllBanner().ToListAsync();
            var data = await _userService.GetAllUsersPagination(param);

            foreach(var user in data.userUnilevers)
            {
                if (user.Banners.Count == listBanners.Where(x => x.IsActive).Count())
                {
                    user.BannerName = "All Banners";
                }
                else
                {
                    user.BannerName = String.Join(", ", user.Banners.Select(x => x.BannerName));
                }
            }
            var listData = Json(new 
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.userUnilevers
            });

            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> GetBannerPagination(DataTableParam param)
        {
      
            var data = await _bannerService.GetBannerPagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.banners
            });
            return listData;   
        }

        [HttpPost]
        public async Task<IActionResult> GetCategoryPagination(DataTableParam param)
        {
            var data = await _skuService.GetCategoryPagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.categories
            });
            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> GetSKUPagination(DataTableParam param)
        {
            var data = await _skuService.GetSKUPagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.skus
            });
            return listData;
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
