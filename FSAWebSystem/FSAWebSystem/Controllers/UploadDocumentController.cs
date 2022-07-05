using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.ComponentModel;
using System.Data;
using System.Net;

namespace FSAWebSystem.Controllers
{
    public class UploadDocumentController : Controller
    {

        public ISKUService _skuService;
        public IBannerService _bannerService;
        public IUploadDocumentService _uploadDocService;
        private readonly FSAWebSystemDbContext _db;
        public UploadDocumentController(ISKUService skuService, IBannerService bannerService, IUploadDocumentService uploadDocService, FSAWebSystemDbContext db)
        {
            _db = db;
            _skuService = skuService;
            _bannerService = bannerService;
            _uploadDocService = uploadDocService;
        }


        [Authorize]
        public async Task<IActionResult> GetFileFormat(string documentType)
        {
            var doc = (DocumentUpload)(Convert.ToInt32(documentType));
            var workbook = new HSSFWorkbook();
            ISheet worksheet = workbook.CreateSheet(doc.ToString());
            var headerRow = worksheet.CreateRow(0).CreateCell(0);


            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;

            var columns = new List<string>();

            switch (doc)
            {
                case DocumentUpload.Banner:
                    columns = _uploadDocService.GetBannerColumns();
                    break;
                case DocumentUpload.SKU:
                    columns = _uploadDocService.GetSKUColumns();
                    break;
                case DocumentUpload.MonthlyBucket:
                    columns = _uploadDocService.GetMonthlyBucketColumns();
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
                return BadRequest();
            }
            finally
            {

            }

            //return Ok();
        }

        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile excelDocument, string documentType, string loggedUser)
        {
            var doc = (DocumentUpload)(Convert.ToInt32(documentType));
            List<string> errorMessages = new List<string>();
            var currDate = DateTime.Now;
           
            if (excelDocument != null)
            {
                if (!excelDocument.FileName.Contains(doc.ToString()))
                {
                    errorMessages.Add("Wrong File Format!");
                    TempData["ErrorMessages"] = errorMessages;
                    TempData["Tab"] = "UploadDoc";
                    return RedirectToAction("Index", "Admin");
                }

                if(doc == DocumentUpload.MonthlyBucket)
				{
                    var isCalendarExist = await _db.FSACalendarHeader.AnyAsync(x => x.Month == currDate.Month && x.Year == currDate.Year);
                    var monthName = currDate.ToString("MMMM");
                    if (!isCalendarExist)
					{
                        errorMessages.Add("Please Input FSACalendar for " + monthName + "-" + currDate.Year.ToString());
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
                        case DocumentUpload.Banner:
                            columns = _uploadDocService.GetBannerColumns();
                            dt = CreateDataTable(sheet, columns);
                            await SaveBanners(dt, excelDocument.FileName, loggedUser, doc);
                            break;
                        case DocumentUpload.SKU:
                            columns = _uploadDocService.GetSKUColumns();
                            dt = CreateDataTable(sheet, columns);
                            await SaveSKUs(dt, excelDocument.FileName, loggedUser, doc);
                            break;
                        case DocumentUpload.MonthlyBucket:
                            columns = _uploadDocService.GetMonthlyBucketColumns();
                            dt = CreateDataTable(sheet, columns);
                            await SaveMonthlyBuckets(dt, excelDocument.FileName, loggedUser, doc);
                            break;
                    }
                }
                catch (Exception ex)
                {

                }
            }


            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Admin", errorMessages);
        }


        private DataTable CreateDataTable(ISheet sheet, List<string> columns)
        {

            var dt = new DataTable();

            foreach (var column in columns)
            {
                dt.Columns.Add(column);
            }

            for (int row = 2; row <= sheet.LastRowNum; row++)
            {
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
                        var cell = sheetRow.GetCell(col);
                        if (sheetRow != null)
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

                }
            }
            return dt;
        }

        private async Task SaveSKUs(DataTable dt, string fileName,  string loggedUser, DocumentUpload documentType)
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
            var categoriesExist = await _db.ProductCategories.AnyAsync();

            if (categoriesExist)
            {
                var count = await _db.Database.ExecuteSqlRawAsync("DELETE FROM ProductCategories");
            }


            var categoryToAdd = listSKU.Select(x => x.Category).Distinct().ToList();

            List<ProductCategory> listCategory = (from category in categoryToAdd
                                                  select category).Select(x => new ProductCategory { Id = Guid.NewGuid(), CategoryProduct = x, CreatedAt = DateTime.Now, CreatedBy = loggedUser, FSADocumentId = fsaDoc.Id }).ToList();

            foreach (var sku in listSKU)
            {
                sku.Id = Guid.NewGuid();
                sku.ProductCategory = listCategory.Single(x => x.CategoryProduct == sku.Category);
                sku.CreatedAt = DateTime.Now;
                sku.CreatedBy = loggedUser;
                sku.FSADocumentId = fsaDoc.Id;
            }

            await _uploadDocService.SaveDocument(fsaDoc);
            await _skuService.SaveProductCategories(listCategory);
            await _skuService.SaveSKUs(listSKU);
        }

        private async Task SaveMonthlyBuckets(DataTable dt, string fileName, string loggedUser, DocumentUpload documentType)
        {
            List<MonthlyBucket> listMonthlyBucket = new List<MonthlyBucket>();

            IQueryable<Banner> banners = _bannerService.GetAllBanner();
            IQueryable<SKU> skus = _skuService.GetAllProducts();
            //List<ProductCategory> categories = _skuService.GetAllProductCategories().ToList();

            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);

            foreach (DataRow dr in dt.Rows)
            {

                var sku = skus.Single(x => x.PCMap == dr["PC Map"].ToString());
                var banner = banners.Single(x => x.BannerName == dr["Banner"].ToString());
                var currentDate = DateTime.Now;
                var monthlyBucket = new MonthlyBucket
                {
                    Id = Guid.NewGuid(),
                    BannerId = banner.Id,
                    SKUId = sku.Id,
                    Price = Convert.ToDecimal(ConvertNumber(dr["Price"].ToString())),
                    PlantContribution = Convert.ToDecimal(ConvertNumber(dr["Plant Contribution"].ToString())) * 100,
                    RatingRate = Convert.ToDecimal(ConvertNumber(dr["Rating Rate"].ToString())) ,
                    TCT = Convert.ToDecimal(ConvertNumber(dr["TCT"].ToString())) * 100,
                    MonthlyTarget = Convert.ToDecimal(ConvertNumber(dr["Monthly Target"].ToString())) * 100,
                    Month = currentDate.Month,
                    Year = currentDate.Year,
                    FSADocument = fsaDoc
                };
                listMonthlyBucket.Add(monthlyBucket);
            }

            await _uploadDocService.SaveMonthlyBuckets(listMonthlyBucket);
            await _uploadDocService.SaveDocument(fsaDoc);
            await CreateWeeklyBucket(listMonthlyBucket);
        }

        private async Task SaveBanners (DataTable dt, string fileName, string loggedUser, DocumentUpload documentType)
        {
            List<Banner> banners = new List<Banner>();
            FSADocument fsaDoc = _uploadDocService.CreateFSADoc(fileName, loggedUser, documentType);
            foreach (DataRow dr in dt.Rows)
            {
                var banner = new Banner
                {
                    Id = Guid.NewGuid(),
                    Trade = dr["Trade"].ToString(),
                    BannerName = dr["Banner Name"].ToString(),
                    PlantCode = dr["Plant Code"].ToString(),
                    PlantName = dr["Plant Name"].ToString(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = loggedUser,
                    FSADocumentId = fsaDoc.Id
                };

                banners.Add(banner);
            }
            await _uploadDocService.SaveDocument(fsaDoc);
            await _bannerService.SaveBanners(banners);
           
        }

		private async Task CreateWeeklyBucket(List<MonthlyBucket> monthlyBuckets)
		{
            List<WeeklyBucket> weeklyBuckets = new List<WeeklyBucket>();
            foreach(var monthlyBucket in monthlyBuckets)
			{
                var weeklyBucket = new WeeklyBucket();

                weeklyBucket.Id = Guid.NewGuid();
                weeklyBucket.Month = monthlyBucket.Month;
                weeklyBucket.Year = monthlyBucket.Year;
                weeklyBucket.BannerId = monthlyBucket.BannerId;
                weeklyBucket.SKUId = monthlyBucket.SKUId;
                weeklyBucket.RatingRate = monthlyBucket.RatingRate;
                var mBucket = monthlyBucket.RatingRate * (monthlyBucket.TCT /100) * (monthlyBucket.MonthlyTarget/100);
                weeklyBucket.MonthlyBucket = mBucket;
                weeklyBucket.BucketWeek1 = mBucket * ((decimal)50 / (decimal)100);
                weeklyBucket.BucketWeek2 = mBucket * ((decimal)50 / (decimal)100);

                weeklyBuckets.Add(weeklyBucket);
			}

            await _uploadDocService.SaveWeeklyBuckets(weeklyBuckets);
		}

        private static string ConvertNumber(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? "0" : input;
        }

    }




}
