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
using ExcelDataReader;
using FSAWebSystem.Models.ApprovalSystemCheckModel;
using Newtonsoft.Json;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Controllers
{
    public class UploadDocumentController : Controller
    {

        public ISKUService _skuService;
        public IBannerPlantService _bannerPlantService;
        public IBannerService _bannerService;
        public IUploadDocumentService _uploadDocService;
        public IRoleService _roleService;
        public IUserService _userService;
        public IBucketService _bucketService;
        private readonly UserManager<FSAWebSystemUser> _userManager;
        private readonly FSAWebSystemDbContext _db;
        private readonly INotyfService _notyfService;
        private readonly ICalendarService _calendarService;
        public UploadDocumentController(ISKUService skuService, IBannerPlantService bannerPlantService, IUploadDocumentService uploadDocService, IRoleService roleService, IUserService userService, IBucketService bucketService,
            UserManager<FSAWebSystemUser> userManager, FSAWebSystemDbContext db, INotyfService notyfService, ICalendarService calendarService, IBannerService bannerService)
        {
            _db = db;
            _skuService = skuService;
            _bannerPlantService = bannerPlantService;
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
            string fileContent = string.Empty;
            var currDate = DateTime.Now;
            var month = currDate.Month;

            if (!string.IsNullOrEmpty(uploadMonth))
            {
                month = Convert.ToInt32(uploadMonth);

            }
            var desc = string.Empty;
            var doc = (DocumentUpload)(Convert.ToInt32(documentType));
            var fieldInfo = doc.GetType().GetField(doc.ToString());
            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                desc = ((DescriptionAttribute)attrs[0]).Description;
            }
            List<string> errorMessages = new List<string>();
            var currentDate = DateTime.Now;

            if (excelDocument != null)
            {
                //if (!excelDocument.FileName.Contains(doc.ToString()) && !excelDocument.FileName.Contains(desc))
                //{
                //    errorMessages.Add("Wrong File!");

                //    _notyfService.Error("Wrong File!");
                //    return Ok(Json(new { errorMessages }));
                //}

                if (doc == DocumentUpload.MonthlyBucket)
                {
                    var isCalendarExist = await _db.FSACalendarHeaders.AnyAsync(x => x.Month == month && x.Year == currDate.Year);
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                    if (!isCalendarExist)
                    {
                        errorMessages.Add("Please Input FSACalendar for " + monthName + "-" + currDate.Year.ToString());
                        return Ok(Json(new { errorMessages }));
                    }
                }
                else if (doc == DocumentUpload.WeeklyDispatch)
                {
                    var calendarDetail = await _calendarService.GetCalendarDetail(DateTime.Now);
                    var weeklyDispatch = await _bucketService.WeeklyBucketExist(calendarDetail.Month, calendarDetail.Week, calendarDetail.Year);
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarDetail.Month);
                    if (weeklyDispatch)
                    {
                        errorMessages.Add("Cannot upload! Weekly Bucket for Month: " + monthName + ", Week: " + calendarDetail.Week.ToString() + " already exist!");
                        return Ok(Json(new { errorMessages }));
                    }
                }
                var workbook = new HSSFWorkbook();
                MemoryStream stream = new MemoryStream();
                await excelDocument.CopyToAsync(stream);
                stream.Position = 0;
                ISheet sheet = new HSSFSheet(workbook);
                DataTable dt = new DataTable();
                if (doc != DocumentUpload.Andromeda && doc != DocumentUpload.ITRUST && doc != DocumentUpload.BottomPrice)
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    DataSet dataSet = excelReader.AsDataSet();
                    dt = dataSet.Tables[0];
                }
                else
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    DataSet dataSet = excelReader.AsDataSet();
                    dt = dataSet.Tables[0];
                }

                var columns = new List<string>();

                var listColumn = new List<string>();
                //Get Col Header
                for(var i = 0; i < dt.Columns.Count ; i++)
                {
                    var row = dt.Rows[1];
                    if (!string.IsNullOrEmpty(row[i].ToString())){
                        listColumn.Add(row[i].ToString());
                    }
             
                }

                try
                {
                    switch (doc)
                    {
                        case DocumentUpload.User:
                            columns = _uploadDocService.GetUserColumns();
                            if(!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File!");
                                return Ok(Json(new { errorMessages }));
                            }
                            await SaveUsers(dt, excelDocument.Name, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.Banner:
                            columns = _uploadDocService.GetBannerColumns();
                            if (!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File!");
                                return Ok(Json(new { errorMessages }));
                            }
                            await SaveBanners(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.SKU:
                            columns = _uploadDocService.GetSKUColumns();
                            if (!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File!");
                                return Ok(Json(new { errorMessages }));
                            }
                            await SaveSKUs(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.MonthlyBucket:
                            columns = _uploadDocService.GetMonthlyBucketColumns();
                            if (!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File! File format must be same with provided format");
                                return Ok(Json(new { errorMessages }));
                            }
                            await SaveMonthlyBuckets(dt, excelDocument.FileName, month, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.WeeklyDispatch:
                            columns = _uploadDocService.GetWeeklyDispatchColumns();
                            if (!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File!");
                                return Ok(Json(new { errorMessages }));
                            }
                            var calendarDetail = await _calendarService.GetCalendarDetail(currentDate);
                            if (calendarDetail.Week == 1)
                            {
                                errorMessages.Add("Cannot Upload Weekly Dispatch on Week: " + calendarDetail.Week);

                            }
                            else
                            {
                                await SaveWeeklyBucket(dt, excelDocument.FileName, loggedUser, doc, errorMessages);

                            }


                            break;
                        case DocumentUpload.DailyOrder:
                            columns = _uploadDocService.GetDailyOrderColumns();
                            if (!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File!");
                                return Ok(Json(new { errorMessages }));
                            }
                            await SaveDailyOrder(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.Andromeda:
                            columns = _uploadDocService.GetAndromedaColumns();
                            if (!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File!");
                                return Ok(Json(new { errorMessages }));
                            }
                            fileContent = await SaveAndromeda(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.ITRUST:
                            columns = _uploadDocService.GetITrustColumns();
                            if (!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File!");
                                return Ok(Json(new { errorMessages }));
                            }
                            fileContent = await SaveITrust(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.BottomPrice:
                            columns = _uploadDocService.GetBottomPriceColumns();
                            if (!columns.SequenceEqual(listColumn))
                            {
                                errorMessages.Add("Wrong File! File format must be same with provided format");

                                _notyfService.Error("Wrong File!");
                                return Ok(Json(new { errorMessages }));
                            }
                            fileContent = await SaveBottomPrice(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                    }

                    if (!errorMessages.Any())
                    {
                        await _db.SaveChangesAsync();
                        _notyfService.Success(doc.ToString() + " successfully uploaded");
                        if (!string.IsNullOrEmpty(fileContent))
                        {
                            _notyfService.Warning("Some SKUs Not Found, Please Refer to ListError file");
                            return Ok(Json(new { data = fileContent }));
                        }
                    }
                    else
                    {
                        _notyfService.Error("Upload Failed.");
                    }
                }
                catch (Exception ex)
                {
                    errorMessages.Add(ex.Message);
                }
            }
            else
            {
                errorMessages.Add("Please Select File.");
                _notyfService.Error("Please Select File.");
            }
            return Ok(Json(new { errorMessages }));
        }

        [Authorize]
        public async Task<IActionResult> UploadFileCheck(IFormFile excelDocument, string documentType, string loggedUser)
        {
            string fileContent = string.Empty;
            var fileByte = string.Empty; ;

            var doc = (DocumentUpload)(Convert.ToInt32(documentType));

            List<string> errorMessages = new List<string>();
            if (excelDocument != null)
            {
                if (!excelDocument.FileName.Contains(doc.ToString()))
                {
                    errorMessages.Add("Wrong File!");
                    _notyfService.Error("Wrong File!");
                    if (doc == DocumentUpload.Andromeda || doc == DocumentUpload.ITRUST || doc == DocumentUpload.BottomPrice)
                    {
                        return Ok(Json(new { errorMessages }));
                    }
                }
                var workbook = new HSSFWorkbook();
                MemoryStream stream = new MemoryStream();
                await excelDocument.CopyToAsync(stream);
                stream.Position = 0;
                ISheet sheet = new HSSFSheet(workbook);
                DataTable dt = new DataTable();
                if (doc != DocumentUpload.Andromeda && doc != DocumentUpload.ITRUST && doc != DocumentUpload.BottomPrice)
                {
                    workbook = new HSSFWorkbook(stream);
                    sheet = workbook.GetSheetAt(0);
                }
                else
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    DataSet dataSet = excelReader.AsDataSet();
                    dt = dataSet.Tables[0];
                }

                var columns = new List<string>();
                try
                {
                    switch (doc)
                    {
                        case DocumentUpload.Andromeda:
                            columns = _uploadDocService.GetAndromedaColumns();
                            fileContent = await SaveAndromeda(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.ITRUST:
                            columns = _uploadDocService.GetITrustColumns();
                            fileContent = await SaveITrust(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                        case DocumentUpload.BottomPrice:
                            columns = _uploadDocService.GetBottomPriceColumns();
                            fileContent = await SaveBottomPrice(dt, excelDocument.FileName, loggedUser, doc, errorMessages);
                            break;
                    }



                    if (!errorMessages.Any())
                    {
                        await _db.SaveChangesAsync();
                        _notyfService.Success(doc.ToString() + " successfully uploaded");
                    }
                    else
                    {
                        _notyfService.Error("Upload Failed.");
                        return Ok(Json(new { errorMessages }));
                    }


                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        _notyfService.Warning("Some SKUs Not Found, Please Refer to ListError file");
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
                _notyfService.Error("Please Select File.");
            }

            return Ok(Json(new { errorMessages }));
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
            for (var i = 2; i < dt.Rows.Count; i++)
            {

                var row = dt.Rows[i];
                var user = new UserUnilever
                {
                    Name = row[0].ToString(),
                    Email = row[1].ToString(),
                    Password = row[2].ToString(),
                    Role = row[3].ToString(),
                    BannerName = row[4].ToString(),

                    PlantCode = row[5].ToString(),
                    WLName = row[6].ToString()

                };
                listUser.Add(user);
            }

            ValidateUserExcel(listUser, errorMessage);

            if (!errorMessage.Any())
            {
                var usersToAdd = new List<UserUnilever>();

                var savedWorkLevels = _userService.GetAllWorkLevel() as IEnumerable<WorkLevel>;

                var savedBanners = _bannerPlantService.GetAllActiveBannerPlant();
                var groupUser = listUser.GroupBy(x => x.Email);
                foreach (var group in groupUser)
                {
                    var savedUser = await _userService.GetUserByEmail(group.Key);
                    List<BannerPlant> bannerPlants = new List<BannerPlant>();
                    var savedUserLogin = await _userManager.FindByEmailAsync(group.First().Email);
                    foreach (var user in group)
                    {
                        var userBanner = savedBanners.Single(x => x.Banner.BannerName == user.BannerName && x.Plant.PlantCode == user.PlantCode);
                        bannerPlants.Add(userBanner);
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
                        newUser.BannerPlants = bannerPlants;
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
                        savedUser.BannerPlants = bannerPlants;
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

                    foreach (var menu in user.RoleUnilever.Menus)
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
            for (var i = 2; i < dt.Rows.Count; i++)
            {

                var row = dt.Rows[i];
                var sku = new SKU
                {
                    PCMap = row[0].ToString(),
                    DescriptionMap = row[1].ToString(),
                    Category = row[2].ToString()
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

            IQueryable<BannerPlant> bannerPlants = _bannerPlantService.GetAllBannerPlant();
            IQueryable<SKU> skus = _skuService.GetAllProducts();
            //List<ProductCategory> categories = _skuService.GetAllProductCategories().ToList();

            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var currentDate = DateTime.Now;

            try
            {
                for (var i = 2; i < dt.Rows.Count; i++)
                {
                    var dr = dt.Rows[i];
                    //var sku = skus.Single(x => x.PCMap == dr["PC Map"].ToString());
                    //var banner = banners.Single(x => x.BannerName == dr["Banner"].ToString());

                    var monthlyBucket = new MonthlyBucket
                    {
                        Id = Guid.NewGuid(),
                        BannerName = dr[2].ToString(),
                        PCMap = dr[3].ToString(),
                        PlantCode = dr[6].ToString(),

                        Price = Decimal.Parse(ConvertNumber(dr[8].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        PlantContribution = Decimal.Parse(ConvertNumber(dr[9].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }) * 100,
                        RunningRate = Decimal.Parse(ConvertNumber(dr[10].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        TCT = Decimal.Parse(ConvertNumber(dr[11].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }) * 100,
                        MonthlyTarget = Decimal.Parse(ConvertNumber(dr[12].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }) * 100,
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
                    var bannerPlant = bannerPlants.Single(x => x.Banner.BannerName == monthlyBucket.BannerName && x.Plant.PlantCode == monthlyBucket.PlantCode);

                    monthlyBucket.SKUId = sku.Id;
                    monthlyBucket.BannerPlant = bannerPlant;
                }
                await _uploadDocService.SaveMonthlyBuckets(listMonthlyBucket);
                await _uploadDocService.SaveDocument(fsaDoc);
                await CreateWeeklyBucket(listMonthlyBucket);
            }
        }

        private async Task SaveBanners(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<BannerPlant> bannerPlants = new List<BannerPlant>();
            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            for (var i = 2; i < dt.Rows.Count; i++)
            {

                var dr = dt.Rows[i];
                var bannerPlant = new BannerPlant
                {
                    Id = Guid.NewGuid(),
                    Trade = dr[0].ToString(),
                    CDM = dr[1].ToString(),
                    KAM = dr[2].ToString(),
                    BannerName = dr[3].ToString(),
                    PlantName = dr[4].ToString(),
                    PlantCode = dr[5].ToString(),

                    CreatedAt = DateTime.Now,
                    CreatedBy = loggedUser,
                    FSADocumentId = fsaDoc.Id
                };

                bannerPlants.Add(bannerPlant);
            }
            ValidateBannerExcel(bannerPlants, errorMessages);

            if (!errorMessages.Any())
            {
                var savedBanners = _bannerService.GetAllBanner();
                var savedPlants = _bannerService.GetAllPlant();

                var bannersFromExcel = bannerPlants.Select(x => new Banner { Id = Guid.NewGuid(), BannerName = x.BannerName, Trade = x.Trade }).DistinctBy(x => x.BannerName).ToList();
                var plantsFromExcel = bannerPlants.Select(x => new Plant { Id = Guid.NewGuid(), PlantCode = x.PlantCode, PlantName = x.PlantName }).DistinctBy(x => x.PlantCode).ToList();
                var bannersToAdd = bannersFromExcel.Where(x => !savedBanners.Select(y => y.BannerName).ToList().Contains(x.BannerName)).ToList();
                var plantsToAdd = plantsFromExcel.Where(x => !savedPlants.Select(y => y.PlantCode).ToList().Contains(x.PlantCode)).ToList();


                await _bannerService.SaveBanners(bannersToAdd);
                await _bannerService.SavePlants(plantsToAdd);
                _db.SaveChanges();


                savedBanners = _bannerService.GetAllBanner();
                savedPlants = _bannerService.GetAllPlant();

                var savedBannerPlants = _bannerPlantService.GetAllBannerPlant().Where(x => x.IsActive);
                var bannerPlantsToAdd = new List<BannerPlant>();

                foreach (var bannerPlant in bannerPlants)
                {
                    var savedBannerPlant = await savedBannerPlants.SingleOrDefaultAsync(x => x.Banner.BannerName == bannerPlant.BannerName && x.Plant.PlantCode == bannerPlant.PlantCode);
                    if (savedBannerPlant == null)
                    {
                        var bannerPlantToAdd = new BannerPlant();
                        bannerPlantToAdd.Banner = savedBanners.Single(x => x.BannerName == bannerPlant.BannerName);
                        bannerPlantToAdd.Plant = savedPlants.Single(x => x.PlantCode == bannerPlant.PlantCode);
                        bannerPlantToAdd.CreatedAt = DateTime.Now;
                        bannerPlantToAdd.CreatedBy = loggedUser;
                        bannerPlantToAdd.FSADocumentId = fsaDoc.Id;
                        bannerPlantToAdd.KAM = bannerPlant.KAM;
                        bannerPlantToAdd.CDM = bannerPlant.CDM;
                        bannerPlantToAdd.Id = Guid.NewGuid();
                        bannerPlantsToAdd.Add(bannerPlantToAdd);

                        //if (savedBannerPlant.PlantName != banner.PlantName || savedBannerPlant.Trade != banner.Trade)
                        //{
                        //    savedBanner.PlantName = banner.PlantName;
                        //    savedBanner.ModifiedAt = DateTime.Now;
                        //    savedBanner.ModifiedBy = loggedUser;
                        //    savedBanner.FSADocumentId = fsaDoc.Id;
                        //}
                    }
                    else
                    {
                        //banner.Id = Guid.NewGuid();
                        //banner.CreatedAt = DateTime.Now;
                        //banner.CreatedBy = loggedUser;
                        //banner.FSADocumentId = fsaDoc.Id;
                        //bannersToAdd.Add(banner);
                    }
                }

                await _uploadDocService.SaveDocument(fsaDoc);
                await _bannerPlantService.SaveBannerPlants(bannerPlantsToAdd);


            }
        }

        private async Task SaveWeeklyBucket(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<WeeklyBucket> weeklyBuckets = new List<WeeklyBucket>();

            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var currentDate = DateTime.Now;
            for (var i = 2; i < dt.Rows.Count; i++)
            {
                var dr = dt.Rows[i];
                var weeklyBucket = new WeeklyBucket
                {
                    BannerName = dr[2].ToString(),
                    PCMap = dr[3].ToString(),
                    DispatchConsume = Decimal.Parse(ConvertNumber(dr[6].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
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
                var bannerPlants = _bannerPlantService.GetAllActiveBannerPlant();
                var savedWeeklyBuckets = _bucketService.GetWeeklyBuckets().Include(x => x.BannerPlant).Where(x => x.Year == currentDate.Year && x.Month == currentDate.Month);
                List<WeeklyBucketHistory> weeklyBucketHistories = new List<WeeklyBucketHistory>();
                foreach (var weeklyBucket in weeklyBuckets)
                {
                    var bannerPlantIds = bannerPlants.Where(x => x.Banner.BannerName == weeklyBucket.BannerName).Select(x => x.Id);
                    var skuId = skus.Single(x => x.PCMap == weeklyBucket.PCMap).Id;
                    var weeklyBucketHistory = new WeeklyBucketHistory();

                    foreach (var bannerPlantId in bannerPlantIds)
                    {
                        weeklyBucketHistory.Id = Guid.NewGuid();
                        weeklyBucketHistory.Month = weeklyBucket.Month;
                        weeklyBucketHistory.Year = weeklyBucket.Year;
                        weeklyBucketHistory.BannerPlantId = bannerPlantId;
                        weeklyBucketHistory.SKUId = skuId;
                        weeklyBucketHistory.DispatchConsume = weeklyBucket.DispatchConsume;
                        weeklyBucketHistory.CreatedAt = DateTime.Now;
                        weeklyBucketHistory.CreatedBy = loggedUser;
                        weeklyBucketHistory.Week = calendarDetail.Week;

                        weeklyBucketHistories.Add(weeklyBucketHistory);
                        var savedWeeklyBucket = savedWeeklyBuckets.SingleOrDefault(x => x.BannerPlant.Id == bannerPlantId && x.SKUId == skuId);
                        if(savedWeeklyBucket != null)
                        {
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
                                remainingBucket = savedWeeklyBucket.BucketWeek3;
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
                       
                    }
                }

                await _uploadDocService.SaveWeeklyBucketHistories(weeklyBucketHistories);
            }
        }

        private async Task SaveDailyOrder(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<DailyOrder> dailyOrders = new List<DailyOrder>();

            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var currentDate = DateTime.Now;
            for (var i = 2; i < dt.Rows.Count; i++)
            {

                var dr = dt.Rows[i];
                var dailyOrder = new DailyOrder
                {
                    BannerName = dr[1].ToString(),
                    PCMap = dr[5].ToString(),
                    ValidBJ = Decimal.Parse(ConvertNumber(dr[9].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                    Month = currentDate.Month,
                    Year = currentDate.Year
                };

                dailyOrders.Add(dailyOrder);
            }

            ValidateDailyOrderExcel(dailyOrders, errorMessages, currentDate);

            if (!errorMessages.Any())
            {
                var bannerPlants = _bannerPlantService.GetAllActiveBannerPlant();
                var weeklyBuckets = _bucketService.GetWeeklyBuckets();
                var skus = _skuService.GetAllProducts();
                foreach (var dailyOrder in dailyOrders)
                {
                    //var bannerPlantId = (await bannerPlants.SingleAsync(x => x.Banner.BannerName == dailyOrder.BannerName)).Id;
                    var bannerPlantIds = bannerPlants.Where(x => x.Banner.BannerName == dailyOrder.BannerName).Select(x => x.Id);
                    var skuId = (await skus.SingleAsync(x => x.PCMap == dailyOrder.PCMap)).Id;
                    foreach(var bannerPlantId in bannerPlantIds)
                    {
                        var weeklyBucket = await weeklyBuckets.SingleOrDefaultAsync(x => x.BannerPlant.Id == bannerPlantId && x.SKUId == skuId && x.Year == dailyOrder.Year && x.Month == dailyOrder.Month);
                        if(weeklyBucket != null)
                        {
                            weeklyBucket.ValidBJ = dailyOrder.ValidBJ;
                            weeklyBucket.RemFSA = weeklyBucket.MonthlyBucket - dailyOrder.ValidBJ;
                        }
           
                    }
                   
                }
            }
        }

        private async Task CreateWeeklyBucket(List<MonthlyBucket> monthlyBuckets)
        {
            List<WeeklyBucket> weeklyBuckets = new List<WeeklyBucket>();
            var bannerPlants = _bannerPlantService.GetAllActiveBannerPlant();
            foreach (var monthlyBucket in monthlyBuckets)
            {
                var weeklyBucket = new WeeklyBucket();


                weeklyBucket.Id = Guid.NewGuid();
                weeklyBucket.Month = monthlyBucket.Month;
                weeklyBucket.Year = monthlyBucket.Year;
                weeklyBucket.BannerPlant = monthlyBucket.BannerPlant;
                weeklyBucket.SKUId = monthlyBucket.SKUId;
                weeklyBucket.RunningRate = decimal.Round(monthlyBucket.RunningRate);
                weeklyBucket.PlantContribution = monthlyBucket.PlantContribution;
                var mBucket = monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100);
                weeklyBucket.MonthlyBucket = decimal.Round(mBucket);
                weeklyBucket.BucketWeek1 = decimal.Round(mBucket * ((decimal)50 / (decimal)100));
                weeklyBucket.BucketWeek2 = decimal.Round(mBucket * ((decimal)50 / (decimal)100));
                weeklyBuckets.Add(weeklyBucket);
            }

            await _uploadDocService.SaveWeeklyBuckets(weeklyBuckets);

        }

        private void ValidateMonthlyBucketExcel(List<MonthlyBucket> listMonthlyBucket, IQueryable<MonthlyBucket> savedMonthlyBuckets, List<string> errorMessages)
        {
            var bannerPlants = _bannerPlantService.GetAllActiveBannerPlant().AsEnumerable();
            var skus = _skuService.GetAllProducts();
            if (listMonthlyBucket.Any(x => string.IsNullOrEmpty(x.PCMap) || string.IsNullOrEmpty(x.BannerName)))
            {
                errorMessages.Add("Please fill all PCMap and BannerName column");
            }



            var bannerPlantCodesFromExcel = listMonthlyBucket.Select(x => new { x.BannerName, x.PlantCode }).Distinct().ToList();

            var bannersNameNotInDb = (from monthlyBucket in bannerPlantCodesFromExcel
                                      where !bannerPlants.Any(x => x.Banner.BannerName == monthlyBucket.BannerName && x.Plant.PlantCode == monthlyBucket.PlantCode)
                                      select monthlyBucket).ToList();

            Parallel.ForEach(bannersNameNotInDb, bannerNotInDb =>
            {
                errorMessages.Add("Banner Name: " + bannerNotInDb.BannerName + " and Plant Code: " + bannerNotInDb.PlantCode + " doesn't exist in database");
            });


            var pcMapsNotInDb = (from monthlyBucket in listMonthlyBucket.DistinctBy(x => x.PCMap)
                                 where !(from sku in skus
                                         select sku.PCMap).Contains(monthlyBucket.PCMap)
                                 select monthlyBucket.PCMap).Distinct().ToList();
            Parallel.ForEach(pcMapsNotInDb, pcMapNotInDb =>
            {
                errorMessages.Add("PCMap: " + pcMapNotInDb + " doesn't exist in database");
            });

            var skuGroups = listMonthlyBucket.GroupBy(x => new { x.BannerName, x.PCMap, x.PlantCode }).ToList();
            foreach (var skuGrp in skuGroups)
            {
                if (skuGrp.Count() > 1)
                {
                    errorMessages.Add("Unable to add duplicate record on Banner Name: " + skuGrp.Key.BannerName + ", PCMap: " + skuGrp.Key.PCMap + " and Plant Code : " + skuGrp.Key.PlantCode + ". Please remove one record.");
                }

                var sku = skus.Single(x => x.PCMap == skuGrp.Key.PCMap);
                var bannerPlant = bannerPlants.SingleOrDefault(x => x.Banner.BannerName == skuGrp.Key.BannerName && x.Plant.PlantCode == skuGrp.Key.PlantCode);
                if (bannerPlant != null)
                {
                    if (savedMonthlyBuckets.Any(x => x.SKUId == sku.Id && x.BannerPlant.Id == bannerPlant.Id))
                    {
                        errorMessages.Add("Monthly Bucket for Banner: " + skuGrp.Key.BannerName + ", PCMap: " + skuGrp.Key.PCMap + " and Plant Code : " + skuGrp.Key.PlantCode + " already exist in database");
                    }
                }
            };
            //foreach (var skuGrp in skuGroups)
            //{


            //}
        }

        private void ValidateWeeklyBucketExcel(List<WeeklyBucket> listWeeklyBucket, List<string> errorMessages)
        {
            var currDate = DateTime.Now;
            if (listWeeklyBucket.Any(x => string.IsNullOrEmpty(x.PCMap) || string.IsNullOrEmpty(x.BannerName)))
            {
                errorMessages.Add("Please fill all PCMap and BannerName column");
            }

            var bucketPlants = (from monthlyBucket in _bucketService.GetMonthlyBuckets().Where(x => x.Month == currDate.Month && x.Year == currDate.Year).Select(x => x.BannerPlant).AsEnumerable().DistinctBy(x => x.Id)
                                join bannerPlant in _bannerPlantService.GetAllActiveBannerPlant() on monthlyBucket.Id equals bannerPlant.Id
                                select new BucketPlant
                                {
                                    BannerName = bannerPlant.Banner.BannerName,
                                }).ToList();

            var bannerBuckets = listWeeklyBucket.Select(x => new BucketPlant
            {
                BannerName = x.BannerName,
            }).ToList();

            var equalityComparer = new BucketPlantEqualityComparer();
            var bannersNotExist = bannerBuckets.Except(bucketPlants, equalityComparer).ToList();

            foreach (var banner in bannersNotExist)
            {
                errorMessages.Add("Banner Name: " + banner.BannerName + " doesn't exist on Monthly Bucket");
            }

            var skuMonthlyBuckets = (from monthlyBucket in _bucketService.GetMonthlyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerPlant.Id, x.SKUId })
                                     join sku in _skuService.GetAllProducts().Where(x => x.IsActive).AsEnumerable() on monthlyBucket.SKUId equals sku.Id
                                     select sku.PCMap).ToList();

            var skuWeeklys = listWeeklyBucket.Select(x => x.PCMap).ToList();

            var skusNotExist = skuWeeklys.Except(skuMonthlyBuckets);

            foreach (var sku in skusNotExist)
            {
                errorMessages.Add("PC Map: " + sku + " doesn't exist on Monthly Bucket");
            }

        }

        private void ValidateDailyOrderExcel(List<DailyOrder> listDailyOrder, List<string> errorMessages, DateTime currentDate)
        {
            if (listDailyOrder.Any(x => string.IsNullOrEmpty(x.PCMap) || string.IsNullOrEmpty(x.BannerName)))
            {
                errorMessages.Add("Please fill all PCMap and BannerName column");
            }

            var bucketPlants = (from weeklyBucket in _bucketService.GetWeeklyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerPlant.Id, x.Month, x.Year })
                                join bannerPlant in _bannerPlantService.GetAllActiveBannerPlant().AsEnumerable() on weeklyBucket.BannerPlant.Id equals bannerPlant.Id
                                select new BucketPlant
                                {
                                    BannerName = bannerPlant.Banner.BannerName,
                                }).ToList();

            var bannerDailyOrders = listDailyOrder.Select(x => new BucketPlant
            {
                BannerName = x.BannerName,
            }).ToList();



            var equalityComparer = new BucketPlantEqualityComparer();
            var bannersNotExist = bannerDailyOrders.Except(bucketPlants, equalityComparer).ToList();


            foreach (var banner in bannersNotExist)
            {
                errorMessages.Add("Banner Name: " + banner.BannerName + " doesn't exist on Monthly Bucket");
            }

            //var bannersNotExist = (from dailyOrder in listDailyOrder
            //                       where !(
            //                       from weeklyBucket in _bucketService.GetWeeklyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerPlant.Id, x.Month, x.Year })
            //                       join banner in _bannerPlantService.GetAllActiveBannerPlant().AsEnumerable() on weeklyBucket.BannerPlant.Id equals banner.Id
            //                       where weeklyBucket.Month == currentDate.Month && weeklyBucket.Year == currentDate.Year
            //                       select new
            //                       {
            //                           banner.Banner.BannerName,
            //                           banner.Plant.PlantCode,
            //                       }).Any(x => x.BannerName == dailyOrder.BannerName && x.PlantCode == dailyOrder.PlantCode)
            //                       select new
            //                       {
            //                           dailyOrder.BannerName,
            //                           dailyOrder.PlantCode
            //                       }).ToList();

            var skuWeeklyBuckets = (from weeklyBucket in _bucketService.GetWeeklyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerPlant.Id, x.SKUId, x.Month, x.Year })
                                    join sku in _skuService.GetAllProducts().Where(x => x.IsActive).AsEnumerable() on weeklyBucket.SKUId equals sku.Id
                                    select sku.PCMap).ToList();


            var skuDailys = listDailyOrder.Select(x => x.PCMap).ToList();

            var skusNotExist = skuDailys.Except(skuWeeklyBuckets);

            foreach (var sku in skusNotExist)
            {
                errorMessages.Add("PC Map: " + sku + " doesn't exist on Monthly Bucket");
            }

            //var skusNotExist = (from dailyOrder in listDailyOrder
            //                    where !(
            //                    from weeklyBucket in _bucketService.GetWeeklyBuckets().AsEnumerable().DistinctBy(x => new { x.BannerPlant.Id, x.SKUId, x.Month, x.Year })
            //                    join sku in _skuService.GetAllProducts().AsEnumerable() on weeklyBucket.SKUId equals sku.Id
            //                    where weeklyBucket.Month == currentDate.Month && weeklyBucket.Year == currentDate.Year
            //                    select new
            //                    {
            //                        sku.PCMap
            //                    }).Any(x => x.PCMap == dailyOrder.PCMap)
            //                    select new
            //                    {
            //                        dailyOrder.PCMap
            //                    }).ToList();
            //foreach (var sku in skusNotExist)
            //{
            //    errorMessages.Add("PC Map: " + sku.PCMap + " doesn't exist on Weekly Bucket");
            //}
        }

        private static void ValidateBannerExcel(List<BannerPlant> listBannerPlant, List<string> errorMessages)
        {
            if (listBannerPlant.Any(x => string.IsNullOrEmpty(x.BannerName) || string.IsNullOrEmpty(x.PlantCode) || string.IsNullOrEmpty(x.PlantName)))
            {
                errorMessages.Add("Please fill all BannerName, PlantCode, and PlantName column");
            }

            var bannerGroups = listBannerPlant.GroupBy(x => new { x.BannerName, x.PlantCode }).ToList();
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
            var savedBanners = _bannerPlantService.GetAllActiveBannerPlant();
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
                if (groupName.Count() > 1)
                {
                    errorMessages.Add("There are more than 1 Name on Email: " + grp.Key);
                }

                var groupWl = grp.GroupBy(x => x.WLName);
                if (groupWl.Any(x => string.IsNullOrEmpty(x.Key)))
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
                if (!savedRoles.Any(x => x.RoleName == groupRole.First().Key))
                {
                    errorMessages.Add("Role: " + groupRole.First().Key + " on Email: " + grp.Key + " doesnt exist in database!");
                }

                foreach (var user in grp)
                {
                    var bannerExist = savedBanners.Any(x => x.Banner.BannerName == user.BannerName && x.Plant.PlantCode == user.PlantCode);
                    if (!bannerExist)
                    {
                        errorMessages.Add("Banner Name: " + user.BannerName + ", Plant Code: " + user.PlantCode + "on Email: " + grp.Key + " doesn't exist in database!");
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
            IEnumerable<BannerPlant> bannerPlants = new List<BannerPlant>().AsEnumerable();
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
                    //banners = _bannerPlantService.GetAllBannerPlant().AsEnumerable();
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

        private async Task<string> SaveAndromeda(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<AndromedaModel> listAndromeda = new List<AndromedaModel>();
            FSADocument fSADocument = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var fileContent = string.Empty;
            try
            {
                var skus = _skuService.GetAllProducts().Where(x => x.IsActive);

                //var pcMaps = await skus.Select(x => x.PCMap).ToListAsync();

                for (var i = 2; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    var andromedaMdl = new AndromedaModel
                    {
                        PCMap = row[0].ToString(),
                        Description = row[1].ToString(),
                        UUStock = Decimal.Parse(ConvertNumber(row[2].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        ITThisWeek = Decimal.Parse(ConvertNumber(row[3].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        RRACT13Wk = Decimal.Parse(ConvertNumber(row[4].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                    };

                    listAndromeda.Add(andromedaMdl);
                }
                var pcMapAndromeda = listAndromeda.Select(x => x.PCMap).Distinct().ToList();
                var pcMapInDb = skus.Where(x => pcMapAndromeda.Contains(x.PCMap)).Select(x => x.PCMap).ToList();
                var pcMapsNotInDb = pcMapAndromeda.Except(pcMapInDb).ToList();

                var skusNotInDb = listAndromeda.Where(x => pcMapsNotInDb.Contains(x.PCMap)).Select(x => new SKU { PCMap = x.PCMap, DescriptionMap = x.Description }).ToList();

                var andromedasToSave = listAndromeda.Where(x => pcMapInDb.Contains(x.PCMap));
                var andromedaGroupSku = andromedasToSave.GroupBy(x => x.PCMap).ToList();
                List<AndromedaModel> listAndromedaToSave = new List<AndromedaModel>();
                foreach (var andromedaGroup in andromedaGroupSku)
                {
                    var andromeda = new AndromedaModel();
                    var sku = skus.Single(x => x.PCMap == andromedaGroup.Key);
                    andromeda.Id = Guid.NewGuid();
                    andromeda.SKUId = sku.Id;
                    andromeda.PCMap = andromedaGroup.First().PCMap;
                    andromeda.Description = andromedaGroup.First().Description;
                    andromeda.UUStock = andromedaGroup.Sum(x => x.UUStock);
                    andromeda.ITThisWeek = andromedaGroup.Sum(x => x.ITThisWeek);
                    andromeda.RRACT13Wk = andromedaGroup.Sum(x => x.RRACT13Wk);
                    andromeda.WeekCover = (andromeda.UUStock + andromeda.ITThisWeek) / andromeda.RRACT13Wk;
                    listAndromedaToSave.Add(andromeda);
                }

                var pcMap = string.Empty;

                _uploadDocService.DeleteAndromeda();
                await _uploadDocService.SaveDocument(fSADocument);
                await _uploadDocService.SaveAndromeda(listAndromedaToSave);
                fileContent = GenerateErrorFile(skusNotInDb);
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return string.Empty;
            }

            return fileContent;
        }

        private async Task<string> SaveITrust(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<ITrustModel> listITrust = new List<ITrustModel>();
            FSADocument fSADocument = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var fileContent = string.Empty;


            try
            {
                var skus = _skuService.GetAllProducts().Where(x => x.IsActive);
                for (var i = 2; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    var iTrust = new ITrustModel
                    {
                        PCMap = row[0].ToString(),
                        Description = row[1].ToString(),
                        SumIntransit = Decimal.Parse(ConvertNumber(row[2].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        SumStock = Decimal.Parse(ConvertNumber(row[3].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        SumFinalRpp = Decimal.Parse(ConvertNumber(row[4].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," })
                    };

                    listITrust.Add(iTrust);
                }

                var pcMapITrust = listITrust.Select(x => x.PCMap).Distinct().ToList();
                var pcMapInDb = skus.Where(x => pcMapITrust.Contains(x.PCMap)).Select(x => x.PCMap).ToList();
                var pcMapsNotInDb = pcMapITrust.Except(pcMapInDb).ToList();

                var skusNotInDb = listITrust.Where(x => pcMapsNotInDb.Contains(x.PCMap)).Select(x => new SKU { PCMap = x.PCMap, DescriptionMap = x.Description }).ToList();

                var iTrustsToSave = listITrust.Where(x => pcMapInDb.Contains(x.PCMap));
                foreach (var iTrustToSave in iTrustsToSave)
                {
                    iTrustToSave.Id = Guid.NewGuid();
                    var sku = skus.Single(x => x.PCMap == iTrustToSave.PCMap);
                    iTrustToSave.SKUId = sku.Id;
                    iTrustToSave.DistStock = (iTrustToSave.SumStock + iTrustToSave.SumIntransit) / iTrustToSave.SumFinalRpp;
                }
                fileContent = GenerateErrorFile(skusNotInDb);

                await _uploadDocService.SaveDocument(fSADocument);
                await _uploadDocService.DeleteITrust();
                await _uploadDocService.SaveITrust(iTrustsToSave.ToList());
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return string.Empty;
            }
            return fileContent;
        }

        private async Task<string> SaveBottomPrice(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType, List<string> errorMessages)
        {
            List<BottomPriceModel> listBottomPrice = new List<BottomPriceModel>();
            FSADocument fSADocument = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            var fileContent = string.Empty;

            try
            {
                var skus = _skuService.GetAllProducts().Where(x => x.IsActive);
                for (var i = 2; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    var iTrust = new BottomPriceModel
                    {
                        PCMap = row[0].ToString(),
                        Description = row[1].ToString(),
                        AvgNormalPrice = Decimal.Parse(ConvertNumber(row[2].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        AvgBottomPrice = Decimal.Parse(ConvertNumber(row[3].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        AvgActualPrice = Decimal.Parse(ConvertNumber(row[4].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        MinActualPrice = Decimal.Parse(ConvertNumber(row[5].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        Gap = Decimal.Parse(ConvertNumber(row[6].ToString()), NumberStyles.Any, new NumberFormatInfo { CurrencyDecimalSeparator = "," }),
                        Remarks = row[7].ToString()
                    };

                    listBottomPrice.Add(iTrust);
                }

                var pcMapBPrice = listBottomPrice.Select(x => x.PCMap).Distinct().ToList();
                var pcMapInDb = skus.Where(x => pcMapBPrice.Contains(x.PCMap)).Select(x => x.PCMap).ToList();
                var pcMapsNotInDb = pcMapBPrice.Except(pcMapInDb).ToList();

                var skusNotInDb = listBottomPrice.Where(x => pcMapsNotInDb.Contains(x.PCMap)).Select(x => new SKU { PCMap = x.PCMap, DescriptionMap = x.Description }).ToList();

                var bPricesToSave = listBottomPrice.Where(x => pcMapInDb.Contains(x.PCMap));
                foreach (var bPrice in bPricesToSave)
                {
                    bPrice.Id = Guid.NewGuid();
                    var sku = skus.Single(x => x.PCMap == bPrice.PCMap);
                    bPrice.SKUId = sku.Id;
                }
                fileContent = GenerateErrorFile(skusNotInDb);
                await _uploadDocService.SaveDocument(fSADocument);
                await _uploadDocService.DeleteBottomPrice();
                await _uploadDocService.SaveBottomPrice(bPricesToSave.ToList());
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return string.Empty;
            }

            return fileContent;
        }
        public string GenerateErrorFile(List<SKU> skusNotInDb)
        {
            MemoryStream ms = new MemoryStream();
            var message = "SKU does not exist in database: ";
            StreamWriter writer = new StreamWriter(ms);
            writer.WriteLine(message);
            var skus = skusNotInDb.DistinctBy(x => x.PCMap);
            foreach (var skuNotInDb in skus)
            {
                writer.WriteLine(skuNotInDb.PCMap + " - " + skuNotInDb.DescriptionMap);
            }
            writer.Flush();
            ms.Position = 0;

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
