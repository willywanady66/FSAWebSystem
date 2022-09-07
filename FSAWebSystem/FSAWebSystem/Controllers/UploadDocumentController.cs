using AspNetCoreHero.ToastNotification.Abstractions;
using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace FSAWebSystem.Controllers
{
    public class UploadDocumentController : Controller
    {

        public ISKUService _skuService;
        public IBannerService _bannerService;
        public IUploadDocumentService _uploadDocService;
        public IRoleService _roleService;
        public IUserService _userService;
        public IBucketService _bucketService;
        private readonly UserManager<FSAWebSystemUser> _userManager;
        private readonly FSAWebSystemDbContext _db;
        private readonly INotyfService _notyfService;
        private readonly ICalendarService _calendarService;
        public UploadDocumentController(ISKUService skuService, IBannerService bannerService, IUploadDocumentService uploadDocService, IRoleService roleService, IUserService userService, IBucketService bucketService,
            UserManager<FSAWebSystemUser> userManager, FSAWebSystemDbContext db, INotyfService notyfService, ICalendarService calendarService)
        {
            _db = db;
            _skuService = skuService;
            _bannerService = bannerService;
            _uploadDocService = uploadDocService;
            _notyfService = notyfService;
            _roleService = roleService;
            _userService = userService;
            _userManager = userManager;
            _bucketService = bucketService;
            _calendarService = calendarService;
        }


        [Authorize]
        public async Task<IActionResult> GetFileFormat(string documentType)
        {
            var doc = (DocumentUpload)(Convert.ToInt32(documentType));
            var workbook = new HSSFWorkbook();
            ISheet worksheet = workbook.CreateSheet(doc.ToString());
            var headerRow = worksheet.CreateRow(0).CreateCell(0);
            List<string> errorMessages = new List<string>();

            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;

            var columns = new List<string>();

            switch (doc)
            {
                case DocumentUpload.User:
                    columns = _uploadDocService.GetUserColumns();
                    break;
                case DocumentUpload.Banner:
                    columns = _uploadDocService.GetBannerColumns();
                    break;
                case DocumentUpload.SKU:
                    columns = _uploadDocService.GetSKUColumns();
                    break;
                case DocumentUpload.MonthlyBucket:
                    columns = _uploadDocService.GetMonthlyBucketColumns();
                    break;
                case DocumentUpload.WeeklyDispatch:
                    columns = _uploadDocService.GetWeeklyDispatchColumns();
                    break;
                case DocumentUpload.DailyOrder:
                    columns = _uploadDocService.GetDailyOrderColumns();
                    break;
                case DocumentUpload.Andromeda:
                    columns = _uploadDocService.GetAndromedaColumns();
                    break;
                case DocumentUpload.BottomPrice:
                    columns = _uploadDocService.GetBottomPriceColumns();
                    break;
                case DocumentUpload.ITRUST:
                    columns = _uploadDocService.GetITrustColumns();
                    break;
            }

            CellRangeAddress range = new CellRangeAddress(0, 0, 0, columns.Count - 1);
            worksheet.AddMergedRegion(range);

            headerRow.CellStyle = style;
            headerRow.SetCellValue("Import " + doc.ToString());




            var row = worksheet.CreateRow(1);

            for (var i = 0; i < columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(columns[i]);
            }

            for (int col = 0; col <= worksheet.GetRow(1).LastCellNum; col++)
            {
                worksheet.AutoSizeColumn(col);
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Position = 0;


            try
            {
                FileStreamResult file = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Upload" + doc.ToString() + "Format.xls");

                return file;
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                TempData["ErrorMessages"] = errorMessages;
            }
            finally
            {

            }
            return RedirectToAction("Index", "Admin", errorMessages);
            //return Ok();
        }

        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile excelDocument, string documentType, string loggedUser, string? uploadMonth)
        {
            var currDate = DateTime.Now;
            var month = currDate.Month;
            if (!string.IsNullOrEmpty(uploadMonth))
            {
                month = Convert.ToInt32(uploadMonth);

            }
            var doc = (DocumentUpload)(Convert.ToInt32(documentType));
            
            List<string> errorMessages = new List<string>();
           

            if (excelDocument != null)
            {
                if (!excelDocument.FileName.Contains(doc.ToString()))
                {
                    errorMessages.Add("Wrong File Format!");
                    TempData["ErrorMessages"] = errorMessages;
                    TempData["Tab"] = "UploadDoc";
                    return RedirectToAction("Index", "Admin");
                }

                if (doc == DocumentUpload.MonthlyBucket)
                {
                    var isCalendarExist = await _db.FSACalendarHeaders.AnyAsync(x => x.Month == month && x.Year == currDate.Year);
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                    if (!isCalendarExist)
                    {
                        errorMessages.Add("Please Input FSACalendar for " + monthName + "-" + currDate.Year.ToString());
                        TempData["ErrorMessages"] = errorMessages;
                        TempData["Tab"] = "UploadDoc";
                        return RedirectToAction("Index", "Admin");
                    }
                }
                else if(doc == DocumentUpload.WeeklyDispatch)
                {
                    var calendarDetail = await _calendarService.GetCalendarDetail(DateTime.Now);
                    var weeklyDispatch = await _bucketService.WeeklyBucketExist(calendarDetail.Month, calendarDetail.Week, calendarDetail.Year);
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarDetail.Month);
                    if (weeklyDispatch)
                    {
                        errorMessages.Add("Cannot upload! Weekly Bucket for Month: " + monthName + ", Week: " + calendarDetail.Week.ToString() + " already exist!");
                        TempData["ErrorMessages"] = errorMessages;
                        TempData["Tab"] = "UploadDoc";
                        return RedirectToAction("Index", "Admin");
                    }
                }
                

                MemoryStream stream = new MemoryStream();
                //var stream = new FileStream(excelDocument.FileName, FileMode.Open, FileAccess.Read);
                await excelDocument.CopyToAsync(stream);
                stream.Position = 0;
                var workbook = new HSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0);

                var columns = new List<string>();
                DataTable dt = new DataTable();
                try
                {
                    switch (doc)
                    {
                        case DocumentUpload.User:
                            columns = _uploadDocService.GetUserColumns();
                            dt = CreateDataTable(sheet, columns, errorMessages);
                            await SaveUsers(dt, excelDocument.Name, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.Banner:
                            columns = _uploadDocService.GetBannerColumns();
                            dt = CreateDataTable(sheet, columns, errorMessages);
                            await SaveBanners(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.SKU:
                            columns = _uploadDocService.GetSKUColumns();
                            dt = CreateDataTable(sheet, columns, errorMessages);
                            await SaveSKUs(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.MonthlyBucket:
                            columns = _uploadDocService.GetMonthlyBucketColumns();
                            dt = CreateDataTable(sheet, columns, errorMessages);
                            await SaveMonthlyBuckets(dt, excelDocument.FileName, month, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.WeeklyDispatch:
                            columns = _uploadDocService.GetWeeklyDispatchColumns();
                            dt = CreateDataTable(sheet, columns, errorMessages);
                            await SaveWeeklyBucket(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.DailyOrder:
                            columns = _uploadDocService.GetDailyOrderColumns();
                            dt = CreateDataTable(sheet, columns, errorMessages);
                            await SaveDailyOrder(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                    }
                    if (!errorMessages.Any())
                    {
                        await _db.SaveChangesAsync();
                        _notyfService.Success(doc.ToString() + " successfully uploaded");
                    }
                    else
                    {
                        TempData["ErrorMessages"] = errorMessages;
                        TempData["Tab"] = "UploadDoc";
                        _notyfService.Error("Upload Failed.");
                    }

                }
                catch (Exception ex)
                {
                    errorMessages.Add(ex.Message);
                    TempData["ErrorMessages"] = errorMessages;
                }
            }
            else
            {
                errorMessages.Add("Please Select File.");
                TempData["ErrorMessages"] = errorMessages;
                _notyfService.Error("Please Select File.");
            }

            return RedirectToAction("Index", "Admin");
        }


        private DataTable CreateDataTable(ISheet sheet, List<string> columns, List<string> errorMessage)
        {

            var dt = new DataTable();

            foreach (var column in columns)
            {
                dt.Columns.Add(column);
            }
            var currentRow = 0;
            var currentCol = 0;
            for (int row = 2; row <= sheet.LastRowNum; row++)
            {
                currentRow = row + 1;
                if (sheet.GetRow(row) == null || sheet.GetRow(row).Cells.All(cell => cell.CellType == CellType.Blank))
                {
                    continue;
                }
                try
                {

                    DataRow dr = dt.NewRow();
                    var sheetRow = sheet.GetRow(row);
                    for (int col = 0; col < sheetRow.LastCellNum; col++)
                    {
                        currentCol = col + 1;
                        var cell = sheetRow.GetCell(col);
                        if (cell != null)
                        {
                            switch (cell.CellType)
                            {
                                case CellType.String:
                                    dr[col] = cell.StringCellValue;
                                    break;
                                case CellType.Boolean:
                                    dr[col] = cell.BooleanCellValue;
                                    break;
                                case CellType.Numeric:
                                    dr[col] = cell.NumericCellValue;
                                    break;
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                    dt.AcceptChanges();

                }
                catch (Exception ex)
                {
                    errorMessage.Add(ex.Message + " on Column: " + currentCol + " and Row: " + currentRow);
                }
            }
            return dt;
        }

        private async Task SaveUsers(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessage)
        {
            List<UserUnilever> listUser = new List<UserUnilever>();
            FSADocument fSADocument = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            foreach (DataRow row in dt.Rows)
            {
                var user = new UserUnilever
                {
                    Email = row["Email"].ToString(),
                    Name = row["Name"].ToString(),
                    BannerName = row["Banner Name"].ToString(),
                    Password = row["Password"].ToString(),
                    Role = row["Role"].ToString(),
                    WLName = row["Work Level"].ToString(),
                    PlantCode = row["Plant Code"].ToString()
                };
                listUser.Add(user);
            }

            ValidateUserExcel(listUser, errorMessage);

            if (!errorMessage.Any())
            {
                var usersToAdd = new List<UserUnilever>();

                var savedWorkLevels = _userService.GetAllWorkLevel() as IEnumerable<WorkLevel>;

                var savedBanners = _bannerService.GetAllActiveBanner();
                var groupUser = listUser.GroupBy(x => x.Email);
                foreach (var group in groupUser)
                {
                    var savedUser = await _userService.GetUserByEmail(group.Key);
                    List<Banner> banners = new List<Banner>();
                    var savedUserLogin = await _userManager.FindByEmailAsync(group.First().Email);
                    foreach (var user in group)
                    {
                        var userBanner = savedBanners.Single(x => x.BannerName == user.BannerName && x.PlantCode == user.PlantCode);
                        banners.Add(userBanner);
                    }
                    var userRole = await _roleService.GetRoleByName(group.First().Role);
                    var workLevelId = savedWorkLevels.Single(x => x.WL == group.First().WLName).Id;
                    if (savedUser == null)
                    {

                        var newUser = new UserUnilever();
                        newUser.Id = Guid.NewGuid();
                        newUser.Email = group.Key;
                        newUser.Name = group.First().Name;
                        newUser.RoleUnilever = userRole;
                        newUser.WLId = workLevelId;
                        newUser.Banners = banners;
                        newUser.IsActive = true;
                        newUser.FSADocumentId = fSADocument.Id;
                        newUser.CreatedAt = DateTime.Now;
                        newUser.CreatedBy = loggedUser;
                        newUser.Password = group.First().Password;
                        usersToAdd.Add(newUser);
                    }
                    else
                    {
                        savedUser.RoleUnilever = userRole;
                        savedUser.Name = group.First().Name;
                        savedUser.ModifiedAt = DateTime.Now;
                        savedUser.ModifiedBy = loggedUser;
                        savedUser.FSADocumentId = fSADocument.Id;
                        savedUser.WLId = workLevelId;
                        savedUser.Banners = banners;
                        savedUser.Password = group.First().Password;
                        savedUserLogin.Role = userRole.RoleName;

                        var claims = await _userManager.GetClaimsAsync(savedUserLogin);

                        foreach (var claim in claims)
                        {
                            await _userManager.RemoveClaimAsync(savedUserLogin, claim);
                        }

                        foreach (var menu in savedUser.RoleUnilever.Menus)
                        {
                            await _userManager.AddClaimAsync(savedUserLogin, new Claim("Menu", menu.Name));
                        }

                        await _userManager.RemovePasswordAsync(savedUserLogin);
                        await _userManager.AddPasswordAsync(savedUserLogin, group.First().Password);
                        await _userManager.UpdateAsync(savedUserLogin);
                    }

                }


                foreach (var user in usersToAdd)
                {
                    var userLogin = new FSAWebSystemUser
                    {
                        Email = user.Email,
                        UserName = user.Email,
                        UserUnileverId = user.Id,
                        Role = user.Role
                    };
                    var res = await _userManager.CreateAsync(userLogin, user.Password);
                    var savedUser = await _userManager.FindByEmailAsync(userLogin.Email);
                    var claims = await _userManager.GetClaimsAsync(savedUser);
                    await _userManager.RemoveClaimsAsync(savedUser, claims);

                    foreach(var menu in user.RoleUnilever.Menus)
                    {
                        await _userManager.AddClaimAsync(savedUser, new Claim("Menu", menu.Name));
                    }
                }

                if (errorMessage.Any())
                {
                    return;
                }

                await _uploadDocService.SaveDocument(fSADocument);
                await _userService.SaveUsers(usersToAdd);
            }

        }

        private async Task SaveSKUs(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<SKU> listSKU = new List<SKU>();
            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            //List<ProductCategory> listCategory = new List<ProductCategory>();
            foreach (DataRow row in dt.Rows)
            {
                var sku = new SKU
                {
                    PCMap = row["PC Map"].ToString(),
                    DescriptionMap = row["Description Map"].ToString(),
                    Category = row["Category"].ToString()
                };
                listSKU.Add(sku);
            }

            ValidateExcel(listSKU, errorMessages);

            if (!errorMessages.Any())
            {
                var savedCategory = _skuService.GetAllProductCategories() as IEnumerable<ProductCategory>;

                var categoryFromExcel = listSKU.Select(x => x.Category).Distinct();

                var categoryToAdd = categoryFromExcel.Where(x => !savedCategory.Select(y => y.CategoryProduct).ToList().Contains(x)).ToList();

                IEnumerable<ProductCategory> listCategory = (from category in categoryToAdd
                                                             select category).Select(x => new ProductCategory { Id = Guid.NewGuid(), CategoryProduct = x, CreatedAt = DateTime.Now, CreatedBy = loggedUser, FSADocumentId = fsaDoc.Id }).AsEnumerable();

                listCategory = !listCategory.Any() ? savedCategory : listCategory;

                if (categoryToAdd.Any())
                {
                    await _skuService.SaveProductCategories(listCategory.ToList());
                }
                _db.SaveChanges();

                savedCategory = _skuService.GetAllProductCategories() as IEnumerable<ProductCategory>;

                List<SKU> skuToAdd = new List<SKU>();
                foreach (var sku in listSKU)
                {
                    var savedSKU = await _skuService.GetSKU(sku.PCMap);

                    if (savedSKU != null)
                    {
                        if (savedSKU.DescriptionMap != sku.DescriptionMap || savedSKU.ProductCategory.CategoryProduct != sku.Category)
                        {
                            savedSKU.DescriptionMap = sku.DescriptionMap;
                            savedSKU.ModifiedBy = loggedUser;
                            savedSKU.ModifiedAt = DateTime.Now;
                            savedSKU.FSADocumentId = fsaDoc.Id;
                            savedSKU.ProductCategory = savedCategory.Single(x => x.CategoryProduct == sku.Category);
                        }
                    }
                    else
                    {
                        sku.Id = Guid.NewGuid();
                        sku.CreatedAt = DateTime.Now;
                        sku.CreatedBy = loggedUser;
                        var category = savedCategory.Single(x => x.CategoryProduct == sku.Category);
                        sku.ProductCategory = category;
                        sku.FSADocumentId = fsaDoc.Id;

                        skuToAdd.Add(sku);
                    }
                }

                await _uploadDocService.SaveDocument(fsaDoc);


                await _skuService.SaveSKUs(skuToAdd);



            }


        }

        private async Task SaveMonthlyBuckets(DataTable dt, string fileName, int month, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<MonthlyBucket> listMonthlyBucket = new List<MonthlyBucket>();

            IQueryable<Banner> banners = _bannerService.GetAllBanner();
            IQueryable<SKU> skus = _skuService.GetAllProducts();
            //List<ProductCategory> categories = _skuService.GetAllProductCategories().ToList();

            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var currentDate = DateTime.Now;
            //var fsaDetail = await _calendarService.GetCalendarDetail(currentDate.Date);
            var row = 0;
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    row += 1;
                    //var sku = skus.Single(x => x.PCMap == dr["PC Map"].ToString());
                    //var banner = banners.Single(x => x.BannerName == dr["Banner"].ToString());
                   
                    var monthlyBucket = new MonthlyBucket
                    {
                        Id = Guid.NewGuid(),
                        BannerName = dr["Banner"].ToString(),
                        PlantCode = dr["Plant Code"].ToString(),
                        PCMap = dr["PC Map"].ToString(),
                        Price = Decimal.Parse(ConvertNumber(dr["Price"].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        PlantContribution = Decimal.Parse(ConvertNumber(dr["Plant Contribution"].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }) * 100,
                        RatingRate = Decimal.Parse(ConvertNumber(dr["Rating Rate"].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        TCT = Decimal.Parse(ConvertNumber(dr["TCT"].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }) * 100,
                        MonthlyTarget = Decimal.Parse(ConvertNumber(dr["Monthly Target"].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }) * 100,
                        Month = month,
                        Year = currentDate.Year,
                        CreatedAt = DateTime.Now,
                        CreatedBy = loggedUser,
                        FSADocument = fsaDoc
                    };

                    listMonthlyBucket.Add(monthlyBucket);
                }
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
            }

            var savedMonthlyBucket = _bucketService.GetMonthlyBuckets().Where(x => x.Month == month && x.Year == currentDate.Year);
            ValidateMonthlyBucketExcel(listMonthlyBucket, savedMonthlyBucket, errorMessages);
            if (!errorMessages.Any())
            {
                foreach (var monthlyBucket in listMonthlyBucket)
                {
                    var sku = skus.Single(x => x.PCMap == monthlyBucket.PCMap);
                    var banner = banners.Single(x => x.BannerName == monthlyBucket.BannerName && x.PlantCode == monthlyBucket.PlantCode);

                    monthlyBucket.SKUId = sku.Id;
                    monthlyBucket.BannerId = banner.Id;
                }
                await _uploadDocService.SaveMonthlyBuckets(listMonthlyBucket);
                await _uploadDocService.SaveDocument(fsaDoc);
                await CreateWeeklyBucket(listMonthlyBucket);
            }
        }

        private async Task SaveBanners(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<Banner> banners = new List<Banner>();
            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            foreach (DataRow dr in dt.Rows)
            {
                var banner = new Banner
                {
                    Id = Guid.NewGuid(),
                    Trade = dr["Trade"].ToString(),
                    CDM = dr["CDM"].ToString(),
                    KAM = dr["KAM"].ToString(),
                    BannerName = dr["Banner Name"].ToString(),
                    PlantCode = dr["Plant Code"].ToString(),
                    PlantName = dr["Plant Name"].ToString(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = loggedUser,
                    FSADocumentId = fsaDoc.Id
                };

                banners.Add(banner);
            }
            ValidateBannerExcel(banners, errorMessages);

            if (!errorMessages.Any())
            {
                var savedBanners = _bannerService.GetAllBanner();
                var bannersToAdd = new List<Banner>();

                foreach (var banner in banners)
                {
                    var savedBanner = await savedBanners.SingleOrDefaultAsync(x => x.BannerName == banner.BannerName && x.PlantCode == banner.PlantCode);
                    if (savedBanner != null)
                    {
                        if (savedBanner.PlantName != banner.PlantName || savedBanner.Trade != banner.Trade)
                        {
                            savedBanner.Trade = banner.Trade;
                            savedBanner.PlantName = banner.PlantName;
                            savedBanner.ModifiedAt = DateTime.Now;
                            savedBanner.ModifiedBy = loggedUser;
                            savedBanner.FSADocumentId = fsaDoc.Id;
                        }
                    }
                    else
                    {
                        banner.Id = Guid.NewGuid();
                        banner.CreatedAt = DateTime.Now;
                        banner.CreatedBy = loggedUser;
                        banner.FSADocumentId = fsaDoc.Id;
                        bannersToAdd.Add(banner);
                    }
                }

                await _uploadDocService.SaveDocument(fsaDoc);
                await _bannerService.SaveBanners(bannersToAdd);


            }
        }

        private async Task SaveWeeklyBucket(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<WeeklyBucket> weeklyBuckets = new List<WeeklyBucket>();

            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var currentDate = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                var weeklyBucket = new WeeklyBucket
                {
                    BannerName = dr["Banner Name"].ToString(),
                    PlantCode = dr["Plant Code"].ToString(),
                    PCMap = dr["Material"].ToString(),
                    DispatchConsume = Decimal.Parse(ConvertNumber(dr["Dispatch / Consume"].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                    Month = currentDate.Month,
                    Year = currentDate.Year
                };

                weeklyBuckets.Add(weeklyBucket);
            }
           
            ValidateWeeklyBucketExcel(weeklyBuckets, errorMessages);

            if (!errorMessages.Any())
            {
                var calendarDetail = await _calendarService.GetCalendarDetail(currentDate);
                var skus = _skuService.GetAllProducts().Where(x => x.IsActive);
                var banners = _bannerService.GetAllActiveBanner();
                var savedWeeklyBuckets = _bucketService.GetWeeklyBuckets().Where(x => x.Year == currentDate.Year && x.Month == currentDate.Month);
                List<WeeklyBucketHistory> weeklyBucketHistories = new List<WeeklyBucketHistory>();
                foreach (var weeklyBucket in weeklyBuckets)
                {
                    var bannerId = banners.Single(x => x.BannerName == weeklyBucket.BannerName && x.PlantCode == weeklyBucket.PlantCode).Id;
                    var skuId = skus.Single(x => x.PCMap == weeklyBucket.PCMap).Id;
                    var weeklyBucketHistory = new WeeklyBucketHistory();

                    weeklyBucketHistory.Id = Guid.NewGuid();
                    weeklyBucketHistory.Month = weeklyBucket.Month;
                    weeklyBucketHistory.Year = weeklyBucket.Year;
                    weeklyBucketHistory.BannerId = bannerId;
                    weeklyBucketHistory.SKUId = skuId;
                    weeklyBucketHistory.DispatchConsume = weeklyBucket.DispatchConsume;
                    weeklyBucketHistory.CreatedAt = DateTime.Now;
                    weeklyBucketHistory.CreatedBy = loggedUser;
                    weeklyBucketHistory.Week = calendarDetail.Week;

                    weeklyBucketHistories.Add(weeklyBucketHistory);
                    var savedWeeklyBucket = savedWeeklyBuckets.Single(x => x.BannerId == bannerId && x.SKUId == skuId);
                    var currentWeekBucket = decimal.Zero;
                    var remainingBucket = decimal.Zero;
                    var totalDispatch = decimal.Zero;
                    if (calendarDetail.Week == 2)
                    {
                        remainingBucket = savedWeeklyBucket.BucketWeek1;
                        //remainingBucket = savedWeeklyBucket.ValidBJ - savedWeeklyBucket.BucketWeek1;
                    }
                    else if (calendarDetail.Week == 3)
                    {
                        remainingBucket = savedWeeklyBucket.BucketWeek2;
                        //remainingBucket = savedWeeklyBucket.ValidBJ - savedWeeklyBucket.BucketWeek2;
                    }
                    else if (calendarDetail.Week == 4)
                    {
                        remainingBucket =  savedWeeklyBucket.BucketWeek3;
                        //remainingBucket = /*savedWeeklyBucket.ValidBJ*/ - savedWeeklyBucket.BucketWeek3;
                    }
                    else
                    {
                        errorMessages.Add("Cannot Upload Weekly Dispatch on Week: " + calendarDetail.Week);
                        return;
                    }
                    totalDispatch = weeklyBucket.DispatchConsume * (savedWeeklyBucket.PlantContribution / 100);

                    currentWeekBucket = remainingBucket - totalDispatch;
                    savedWeeklyBucket.GetType().GetProperty("BucketWeek" + (calendarDetail.Week + 1).ToString()).SetValue(savedWeeklyBucket, currentWeekBucket);
                    savedWeeklyBucket.DispatchConsume += weeklyBucket.DispatchConsume;
                }

                await _uploadDocService.SaveWeeklyBucketHistories(weeklyBucketHistories);
            }
        }

        private async Task SaveDailyOrder(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<DailyOrder> dailyOrders = new List<DailyOrder>();

            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var currentDate = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {

                //var sku = skus.Single(x => x.PCMap == dr["PC Map"].ToString());
                //var banner = banners.Single(x => x.BannerName == dr["Banner"].ToString());
                var dailyOrder = new DailyOrder
                {
                    BannerName = dr["Banner Name"].ToString(),
                    PlantCode = dr["Plant Code"].ToString(),
                    PCMap = dr["PC Map"].ToString(),
                    ValidBJ = Decimal.Parse(ConvertNumber(dr["Valid Order + BJ"].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                    Month = currentDate.Month,
                    Year = currentDate.Year
                };

                dailyOrders.Add(dailyOrder);
            }

            ValidateDailyOrderExcel(dailyOrders, errorMessages, currentDate);

            if (!errorMessages.Any())
            {
                var banners = _bannerService.GetAllActiveBanner();
                var weeklyBuckets = _bucketService.GetWeeklyBuckets();
                var skus = _skuService.GetAllProducts();
                foreach (var dailyOrder in dailyOrders)
                {
                    var bannerId = (await banners.SingleAsync(x => x.BannerName == dailyOrder.BannerName && x.PlantCode == dailyOrder.PlantCode)).Id;
                    var skuId = (await skus.SingleAsync(x => x.PCMap == dailyOrder.PCMap)).Id;
                    var weeklyBucket = await weeklyBuckets.SingleAsync(x => x.BannerId == bannerId && x.SKUId == skuId && x.Year == dailyOrder.Year && x.Month == dailyOrder.Month);
                    weeklyBucket.ValidBJ += dailyOrder.ValidBJ;
                    weeklyBucket.RemFSA = weeklyBucket.MonthlyBucket - dailyOrder.ValidBJ;
                }
            }
        }

        private async Task CreateWeeklyBucket(List<MonthlyBucket> monthlyBuckets)
        {
            List<WeeklyBucket> weeklyBuckets = new List<WeeklyBucket>();
            foreach (var monthlyBucket in monthlyBuckets)
            {
                var weeklyBucket = new WeeklyBucket();

                weeklyBucket.Id = Guid.NewGuid();
                weeklyBucket.Month = monthlyBucket.Month;
                weeklyBucket.Year = monthlyBucket.Year;
                weeklyBucket.BannerId = monthlyBucket.BannerId;
                weeklyBucket.SKUId = monthlyBucket.SKUId;
                weeklyBucket.RatingRate = monthlyBucket.RatingRate;
                weeklyBucket.PlantContribution = monthlyBucket.PlantContribution;
                var mBucket = monthlyBucket.RatingRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100);
                weeklyBucket.MonthlyBucket = mBucket;
                weeklyBucket.BucketWeek1 = mBucket * ((decimal)50 / (decimal)100);
                weeklyBucket.BucketWeek2 = mBucket * ((decimal)50 / (decimal)100);
                weeklyBuckets.Add(weeklyBucket);

   

                
            }

            await _uploadDocService.SaveWeeklyBuckets(weeklyBuckets);
 
        }

        private void ValidateMonthlyBucketExcel(List<MonthlyBucket> listMonthlyBucket, IQueryable<MonthlyBucket> savedMonthlyBuckets, List<string> errorMessages)
        {
            var banners = _bannerService.GetAllActiveBanner();
            var skus = _skuService.GetAllProducts();
            if (listMonthlyBucket.Any(x => string.IsNullOrEmpty(x.PCMap) || string.IsNullOrEmpty(x.BannerName)))
            {
                errorMessages.Add("Please fill all PCMap and BannerName column");
            }

            

            var bannerPlantCodesFromExcel = listMonthlyBucket.Select(x => new { x.BannerName, x.PlantCode }).Distinct().ToList();

            var bannersNameNotInDb = (from monthlyBucket in bannerPlantCodesFromExcel
                                      where !banners.Any(x => x.BannerName == monthlyBucket.BannerName && x.PlantCode == monthlyBucket.PlantCode)
                                      select monthlyBucket).ToList();
            foreach (var bannerNotInDb in bannersNameNotInDb)
            {
                errorMessages.Add("Banner Name: " + bannerNotInDb.BannerName + " and Plant Code: " + bannerNotInDb.PlantCode + " doesn't exist in database");
            }

            var pcMapsNotInDb = (from monthlyBucket in listMonthlyBucket.DistinctBy(x => x.PCMap)
                                 where !(from sku in skus
                                         select sku.PCMap).Contains(monthlyBucket.PCMap)
                                 select monthlyBucket.PCMap).Distinct().ToList();
            foreach (var pcMapNotInDb in pcMapsNotInDb)
            {
                errorMessages.Add("PCMap: " + pcMapNotInDb + " doesn't exist in database");
            }

            var skuGroups = listMonthlyBucket.GroupBy(x => new { x.BannerName, x.PCMap, x.PlantCode }).ToList();
            foreach (var skuGrp in skuGroups)
            {
                if (skuGrp.Count() > 1)
                {
                    errorMessages.Add("Unable to add duplicate record on Banner Name: " + skuGrp.Key.BannerName + ", PCMap: " + skuGrp.Key.PCMap + " and Plant Code : " + skuGrp.Key.PlantCode + ". Please remove one record.");
                }

                var sku = skus.Single(x => x.PCMap == skuGrp.Key.PCMap);
                var banner = banners.SingleOrDefault(x => x.BannerName == skuGrp.Key.BannerName && x.PlantCode == skuGrp.Key.PlantCode);
                if (banner != null)
                {
                    if (savedMonthlyBuckets.Any(x => x.SKUId == sku.Id && x.BannerId == banner.Id))
                    {
                        errorMessages.Add("Monthly Bucket for Banner: " + skuGrp.Key.BannerName + ", PCMap: " + skuGrp.Key.PCMap + " and Plant Code : " + skuGrp.Key.PlantCode + " already exist in database");
                    }
                }
   
            }
        }

        private void ValidateWeeklyBucketExcel(List<WeeklyBucket> listWeeklyBucket,  List<string> errorMessages)
        {
            if (listWeeklyBucket.Any(x => string.IsNullOrEmpty(x.PCMap) || string.IsNullOrEmpty(x.BannerName)))
            {
                errorMessages.Add("Please fill all PCMap and BannerName column");
            }
            var z = _bucketService.GetMonthlyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerId, x.SKUId }).ToList();

            var bannersNotExist = (from weeklyBucket in listWeeklyBucket
                                   where !(
                                   from monthlyBucket in _bucketService.GetMonthlyBuckets().AsEnumerable().DistinctBy(x => x.BannerId)
                                   join banner in _bannerService.GetAllActiveBanner().AsEnumerable() on monthlyBucket.BannerId equals banner.Id
                                   select new
                                   {
                                       banner.BannerName,
                                       banner.PlantCode,
                                   }).Any(x => x.BannerName == weeklyBucket.BannerName && x.PlantCode == weeklyBucket.PlantCode)
                                   select new
                                   {
                                       weeklyBucket.BannerName,
                                       weeklyBucket.PlantCode
                                   }).ToList();

            foreach (var banner in bannersNotExist)
            {
                errorMessages.Add("Banner Name: " + banner.BannerName + " and Plant Code: " + banner.PlantCode + " doesn't exist on Monthly Bucket");
            }

            var skusNotExist = (from weeklyBucket in listWeeklyBucket
                                where !(
                                from monthlyBucket in _bucketService.GetMonthlyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerId, x.SKUId })
                                join sku in _skuService.GetAllProducts().Where(x => x.IsActive).AsEnumerable() on monthlyBucket.SKUId equals sku.Id
                                select new
                                {
                                    sku.PCMap
                                }).Any(x => x.PCMap == weeklyBucket.PCMap)
                                select new
                                {
                                    weeklyBucket.PCMap
                                }).ToList();
            foreach (var sku in skusNotExist)
            {
                errorMessages.Add("PC Map: " + sku.PCMap + " doesn't exist on Monthly Bucket");
            }

        }

        private void ValidateDailyOrderExcel(List<DailyOrder> listDailyOrder, List<string> errorMessages, DateTime currentDate)
        {
            if (listDailyOrder.Any(x => string.IsNullOrEmpty(x.PCMap) || string.IsNullOrEmpty(x.BannerName)))
            {
                errorMessages.Add("Please fill all PCMap and BannerName column");
            }

            var bannersNotExist = (from dailyOrder in listDailyOrder
                                   where !(
                                   from weeklyBucket in _bucketService.GetWeeklyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerId, x.Month, x.Year })
                                   join banner in _bannerService.GetAllActiveBanner().AsEnumerable() on weeklyBucket.BannerId equals banner.Id
                                   where weeklyBucket.Month == currentDate.Month && weeklyBucket.Year == currentDate.Year
                                   select new
                                   {
                                       banner.BannerName,
                                       banner.PlantCode,
                                   }).Any(x => x.BannerName == dailyOrder.BannerName && x.PlantCode == dailyOrder.PlantCode)
                                   select new
                                   {
                                       dailyOrder.BannerName,
                                       dailyOrder.PlantCode
                                   }).ToList();

            foreach (var banner in bannersNotExist)
            {
                errorMessages.Add("Banner Name: " + banner.BannerName + " and Plant Code: " + banner.PlantCode + " doesn't exist on Weekly Bucket");
            }

            var skusNotExist = (from dailyOrder in listDailyOrder
                                where !(
                                from weeklyBucket in _bucketService.GetWeeklyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerId, x.SKUId, x.Month, x.Year})
                                join sku in _skuService.GetAllProducts().AsEnumerable() on weeklyBucket.SKUId equals sku.Id
                                where weeklyBucket.Month == currentDate.Month && weeklyBucket.Year == currentDate.Year
                                select new
                                {
                                    sku.PCMap
                                }).Any(x => x.PCMap == dailyOrder.PCMap)
                                select new
                                {
                                    dailyOrder.PCMap
                                }).ToList();
            foreach (var sku in skusNotExist)
            {
                errorMessages.Add("PC Map: " + sku.PCMap + " doesn't exist on Weekly Bucket");
            }
        }

        private static void ValidateBannerExcel(List<Banner> listBanner, List<string> errorMessages)
        {
            if (listBanner.Any(x => string.IsNullOrEmpty(x.BannerName) || string.IsNullOrEmpty(x.PlantCode)))
            {
                errorMessages.Add("Please fill all BannerName and PlantCode column");
            }

            var bannerGroups = listBanner.GroupBy(x => new { x.BannerName, x.PlantCode }).ToList();
            foreach (var bannerGrp in bannerGroups)
            {
                if (bannerGrp.Count() > 1)
                {
                    errorMessages.Add("There is duplicate record of Banner Name: " + bannerGrp.Key.BannerName + " and Plant Code : " + bannerGrp.Key.PlantCode + ". Please remove one record.");
                }
            }
        }


        private void ValidateUserExcel(List<UserUnilever> listUser, List<string> errorMessages)
        {
            List<string> columnToCheck = new List<string>();
            var savedBanners = _bannerService.GetAllActiveBanner();
            var savedRoles = _roleService.GetAllRoles();
            var savedWls = _userService.GetAllWorkLevel();
            columnToCheck.Add("Name");
            columnToCheck.Add("WLName");
            columnToCheck.Add("Role");

            var groups = listUser.GroupBy(x => x.Email).ToList();
            foreach (var grp in groups)
            {
                if (string.IsNullOrEmpty(grp.Key))
                {
                    errorMessages.Add("Please fill all Email column");
                }


                var groupPassword = grp.GroupBy(x => x.Password);
                if (groupPassword.Any(x => string.IsNullOrEmpty(x.Key)))
                {
                    errorMessages.Add("Password cannot empty on Email: " + grp.Key);
                }
                if (groupPassword.Count() > 1)
                {
                    errorMessages.Add("There are more than 1 Password on Email: " + grp.Key);
                }

                var groupName = grp.GroupBy(x => x.Name);
                if (groupName.Any(x => string.IsNullOrEmpty(x.Key)))
                {
                    errorMessages.Add("Name cannot empty on Email: " + grp.Key);
                }
                if(groupName.Count() > 1)
                {
                    errorMessages.Add("There are more than 1 Name on Email: " + grp.Key);
                }

                var groupWl = grp.GroupBy(x => x.WLName);
                if(groupWl.Any(x => string.IsNullOrEmpty(x.Key)))
                {
                    errorMessages.Add("WL Name cannot empty on Email: " + grp.Key);
                }
                if (groupWl.Count() > 1)
                {
                    errorMessages.Add("There are more than 1 WL Name on Email: " + grp.Key);
                }
                if (!savedWls.Any(x => x.WL == groupWl.First().Key))
                {
                    errorMessages.Add("Work Level: " + groupWl.First().Key + " on Email: " + grp.Key + " doesnt exist in database!");
                }


                var groupRole = grp.GroupBy(x => x.Role);
                if (groupRole.Any(x => string.IsNullOrEmpty(x.Key)))
                {
                    errorMessages.Add("Role cannot empty on Email: " + grp.Key);
                }
                if (groupRole.Count() > 1)
                {
                    errorMessages.Add("There are more than 1 Role on Email: " + grp.Key);
                }
                if(!savedRoles.Any(x => x.RoleName == groupRole.First().Key))
                {
                    errorMessages.Add("Role: " + groupRole.First().Key + " on Email: " + grp.Key + " doesnt exist in database!");
                }

                foreach (var user in grp)
                {
                    var bannerExist = savedBanners.Any(x => x.BannerName == user.BannerName && x.PlantCode == user.PlantCode);
                    if (!bannerExist)
                    {
                        errorMessages.Add("Banner Name: " + user.BannerName + ", Plant Code: " + user.PlantCode + "on Email: " + grp.Key  + " doesn't exist in database!");
                    }
                }

                string rx = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$";
                var match = Regex.Match(grp.First().Password, rx, RegexOptions.None);

                if (!match.Success)
                {
                    errorMessages.Add("Password on Email: " + grp.Key + " must contain at least 6 characters, at least one number, at least one lowercase letter, at least one uppercase letter, and at least one special characters");
                }
            }


        }

        private static void ValidateExcel<T>(List<T> list, List<string> errorMessages)
        {
            string mainColumn = string.Empty;
            var className = typeof(T).Name;
            IEnumerable<Banner> banners = new List<Banner>().AsEnumerable();
            IEnumerable<RoleUnilever> roles = new List<RoleUnilever>().AsEnumerable();
            IEnumerable<UserUnilever> users = new List<UserUnilever>().AsEnumerable();
            List<string> columnToCheck = new List<string>();

            var classType = typeof(T);
            switch (className)
            {
                case nameof(SKU):
                    mainColumn = "PCMap";
                    break;
                case nameof(UserUnilever):
                    mainColumn = "Email";
                    //banners = _bannerService.GetAllBanner().AsEnumerable();
                    //roles = _roleService.GetAllRoles().AsEnumerable();
                    columnToCheck.Add("Email");
                    columnToCheck.Add("Name");
                    columnToCheck.Add("WLName");
                    columnToCheck.Add("Role");
                    break;
            }

            if (list.Any(x => string.IsNullOrEmpty(GetColStringValue(x, mainColumn))))
            {
                errorMessages.Add("Please fill all " + mainColumn + " column");
            }

            foreach (var col in columnToCheck)
            {
                var emptyColumnValues = list.Where(x => string.IsNullOrEmpty(GetColStringValue(x, col))).Select(x => GetColStringValue(x, mainColumn)).ToList();
                if (emptyColumnValues.Any())
                {
                    Parallel.ForEach(emptyColumnValues, x =>
                    {
                        errorMessages.Add(col + " cannot by empty on " + mainColumn + ": " + x);
                    });
                }
            }


            var groups = list.GroupBy(x => x.GetType().GetProperty(mainColumn).GetValue(x, null)).ToList();
            foreach (var grp in groups)
            {
                if (grp.Count() > 1)
                {
                    errorMessages.Add("There is duplicate record of " + mainColumn + " : " + grp.Key + ". Please remove one record.");
                }
            }
        }

        private static void CheckExistInDb<T>(IEnumerable<T> list, List<string> errorMessages, string column, string colValue)
        {
            if (list.Any(x => x.GetType().GetProperty(column).GetValue(x, null).ToString() == colValue))
            {
                errorMessages.Add(column + ": " + colValue + " already exist in database");
            }
        }

        private static string GetColStringValue<T>(T obj, string column)
        {
            var value = obj.GetType().GetProperty(column).GetValue(obj, null)?.ToString();
            return value;
        }

        private static string ConvertNumber(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? "0" : input;
        }

    }
}
