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

        public AdminController(FSAWebSystemDbContext db, IUserService userSvc, IBannerService bannerSvc, IRoleService roleSvc, ISKUService skuService)
        {
            _db = db;
            _userService = userSvc;
            _bannerService = bannerSvc;
            _roleServices = roleSvc;
            _skuService = skuService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var listDocumentUpload = Enum.GetValues(typeof(DocumentUpload)).Cast<DocumentUpload>().Select(x => new SelectListItem { Text = UploadDocumentService.GetEnumDesc(x), Value = ((int) x).ToString() }).ToList();
            List<UserUnilever> listUsers = await _userService.GetAllUsers();
             foreach(var user in listUsers)
            {
                user.BannerName = String.Join(',', user.Banners.Select(x => x.BannerName));
            }
            List<Banner> listBanners = await _bannerService.GetAllBanner().ToListAsync();
            List<RoleUnilever> listRoles = await _roleServices.GetAllRoles().ToListAsync();
            List<ProductCategory> listCategory = await _skuService.GetAllProductCategories().ToListAsync();
            List<SKU> listSKU = await _skuService.GetAllProducts().ToListAsync();
            AdminModel model = new AdminModel
            {
                Users = listUsers,
                Banners = listBanners,
                Roles = listRoles,
                SKUs = listSKU,
                Categories = listCategory,
                DocumentUploads = listDocumentUpload,
                LoggedUser = User.Identity.Name
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
            public string LoggedUser { get; set; }
        }

        
    }


   
}
