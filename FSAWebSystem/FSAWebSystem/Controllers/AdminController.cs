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
        private IBannerPlantService _bannerPlantService;
        private IRoleService _roleServices;
        private ISKUService _skuService;
        private ICalendarService _calendarService;
        private IBannerService _bannerService;
        public AdminController(FSAWebSystemDbContext db, IUserService userSvc, IBannerPlantService bannerSvc, IRoleService roleSvc, ISKUService skuService, ICalendarService calendarService, IBannerService bannerService)
        {
            _db = db;
            _userService = userSvc;
            _bannerPlantService = bannerSvc;
            _roleServices = roleSvc;
            _skuService = skuService;
            _calendarService = calendarService;
            _bannerService = bannerService;
        }

        [Authorize(Policy = "AdminPage")]

        public async Task<IActionResult> Index()
        {
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            var currentDate = DateTime.Now; 
            var listDocumentUpload = Enum.GetValues(typeof(DocumentUpload)).Cast<DocumentUpload>().Select(x => new SelectListItem { Text = UploadDocumentService.GetEnumDesc(x), Value = ((int)x).ToString() }).ToList();


            List<RoleUnilever> listRoles = await _roleServices.GetAllRoles().ToListAsync();
            var savedMenus = await _roleServices.GetAllMenu().ToListAsync();
            foreach (var role in listRoles)
            {
                if (role.Menus.Count == savedMenus.Count)
                {
                    role.Menu = "All Menu";
                }
                else
                {
                    foreach (var menu in role.Menus)
                    {
                        role.Menu += menu.Name + "; ";
                    }
                }
            }
         
            AdminModel model = new AdminModel
            {
    
                Roles = listRoles,
                DocumentUploads = listDocumentUpload,
                LoggedUser = User.Identity.Name,
            };
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> GetFSACalendar(string month, string year)
        {

            FSACalendarHeader fsaCalendar = await _calendarService.GetFSACalendarHeader(Convert.ToInt32(month), Convert.ToInt32(year));
            if (fsaCalendar != null)
            {
                var fsaDetails = (from calendar in fsaCalendar.FSACalendarDetails.Where(x => x.StartDate.HasValue && x.EndDate.HasValue)
                                  select new
                                  {
                                      StartDate = calendar.StartDate.Value.ToString("dd/MM/yyyy"),
                                      EndDate = calendar.EndDate.Value.ToString("dd/MM/yyyy"),
                                      Week = calendar.Week,
                                      Year = calendar.Year,
                                      Month = calendar.Month,
                                      Id = fsaCalendar.Id
                                  }).ToList();

                return Json(new
                {
                    data = fsaDetails
                });
            }
            else
            {
                var listData = new List<FSACalendarDetail>();
                return Json(new
                {
                    data = listData
                }) ;
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetULICalendar(string month, string year)
        {
            try
            {
                ULICalendar uliCalendar = await _calendarService.GetULICalendar(Convert.ToInt32(month), Convert.ToInt32(year));
                if (uliCalendar != null)
                {
                    var uliCalendarDetail = (from calendar in uliCalendar.ULICalendarDetails.Where(x => x.StartDate.HasValue && x.EndDate.HasValue && x.Week != 0)
                                             select new
                                             {
                                                 StartDate = calendar.StartDate.Value.ToString("dd/MM/yyyy"),
                                                 EndDate = calendar.EndDate.Value.ToString("dd/MM/yyyy"),
                                                 Week = calendar.Week,
                                                 Year = year,
                                                 Month = month,
                                                 Id = uliCalendar.Id
                                             }).ToList();

                    return Json(new
                    {
                        data = uliCalendarDetail
                    });
                }
            }
            catch (Exception ex)
            {

            }
            var listData = new List<FSACalendarDetail>();
            return Json(new
            {
                data = listData
            });
        }


        [HttpPost]
        public async Task<IActionResult> GetUserPagination(DataTableParam param)
        {
            List<BannerPlant> listBanners = await _bannerPlantService.GetAllBannerPlant().ToListAsync();
            var data = await _userService.GetAllUsersPagination(param);

            foreach (var user in data.userUnilevers)
            {
                if (user.BannerPlants.Where(x => x.IsActive).Count() == listBanners.Where(x => x.IsActive).Count() && user.BannerPlants.Count>1)
                {
                    user.BannerName = "All Banners";
                }
                else
                {
                    user.BannerName = String.Join(", ", user.BannerPlants.Select(x => x.Banner.BannerName));
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
        public async Task<IActionResult> GetBannerPlantPagination(DataTableParam param)
        {

            var data = await _bannerPlantService.GetBannerPlantPagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.bannerPlants
            });
            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> GetBannersPagination(DataTableParam param)
        {
            var data = await _bannerService.GetBannersPagination(param);
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
        public async Task<IActionResult> GetPlantsPagination(DataTableParam param)
        {
            var data = await _bannerService.GetPlantsPagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.plants
            });
            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> GetAndromedaPagination(DataTableParam param)
        {
            var data = await _skuService.GetAndromedaPagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.andromedas
            });
            return listData;
        }


        [HttpPost]
        public async Task<IActionResult> GetBottomPricePagination(DataTableParam param)
        {
            var data = await _skuService.GetBottomPricePagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.bottomPrices
            });
            return listData;
        }


        [HttpPost]
        public async Task<IActionResult> GetITrustPagination(DataTableParam param)
        {
            var data = await _skuService.GetITrustPagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.iTrusts
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

        [HttpPost]
        public async Task<IActionResult> GetWorkLevelPagination(DataTableParam param)
        {
            var data = await _userService.GetAllWorkLevelPagination(param);
            var listData = Json(new
            {
                draw = param.draw,
                recordsTotal = data.totalRecord,
                recordsFiltered = data.totalRecord,
                data = data.workLevels
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
            //public List<Banner> Banners { get; set; }
            public List<BannerPlant> BannerPlants { get; set; }
            public List<SKU> SKUs { get; set; }
            public List<RoleUnilever> Roles { get; set; }
            public List<ProductCategory> Categories { get; set; }

            public List<SelectListItem> DocumentUploads { get; set; }
            public FSACalendarHeader FSACalendar { get; set; }
            public string LoggedUser { get; set; }
        }


    }



}
