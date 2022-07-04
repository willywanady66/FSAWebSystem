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
        private readonly FSAWebSystemDbContext _db;
        public UploadDocumentController(ISKUService skuService, IBannerService bannerService, FSAWebSystemDbContext db)
        {
            _db = db;
            _skuService = skuService;
            _bannerService = bannerService;
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
                case DocumentUpload.Product:
                    columns = UploadDocumentService.GetProductColumn();
                    break;
                case DocumentUpload.MonthlyBucket:
                    columns = UploadDocumentService.GetMonthlyBucketColumn();
                    break;
            }

            CellRangeAddress range = new CellRangeAddress(0, 0, 0, columns.Count);  
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

            if (excelDocument != null)
            {
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
                        case DocumentUpload.Product:
                            columns = UploadDocumentService.GetProductColumn();
                             dt = CreateDataTable(sheet, columns);
                            await SaveSKUs(dt, loggedUser);
                            break;
                        case DocumentUpload.MonthlyBucket:
                            columns = UploadDocumentService.GetMonthlyBucketColumn();
                             dt = CreateDataTable(sheet, columns);
                            await SaveMonthlyBucket(dt);
                            break;
                    }
                }
                catch(Exception ex)
                {

                }
            }

            FSADocument fsaDoc = CreateFSADoc(excelDocument.FileName, loggedUser);
            await _db.AddAsync(fsaDoc);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");
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

        private async Task SaveSKUs(DataTable dt, string loggedUser)
        {
            List<SKU> listSKU = new List<SKU>();
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
                                                  select category).Select(x => new ProductCategory { Id = Guid.NewGuid(), CategoryProduct = x, CreatedAt = DateTime.Now, CreatedBy = loggedUser }).ToList();

            foreach (var sku in listSKU)
            {
                sku.Id = Guid.NewGuid();
                sku.ProductCategory = listCategory.Single(x => x.CategoryProduct == sku.Category);
                sku.CreatedAt = DateTime.Now;
                sku.CreatedBy = loggedUser;
            }
           
           
            await _skuService.SaveProductCategories(listCategory);
            await _skuService.SaveSKUs(listSKU);
        }

        private async Task SaveMonthlyBucket(DataTable dt)
        {
            List<MonthlyBucket> listMonthlyBucket = new List<MonthlyBucket>();
            

            List<Banner> banners = _bannerService.GetAllBanner().ToList();
            List<SKU> skus = _skuService.GetAllProducts().ToList();
            //List<ProductCategory> categories = _skuService.GetAllProductCategories().ToList();

            foreach(DataRow dr in dt.Rows)
            {

                var sku = skus.Single(x => x.PCMap == dr["PC Map"].ToString());
                var monthlyBucket = new MonthlyBucket
                {
                    Id = Guid.NewGuid(),
                    BannerId = banners.Single(x => x.BannerName == dr["Banner"].ToString()).Id,
                    SKUId = sku.Id,
                    Price = Convert.ToDecimal(ConvertNumber(dr["Price"].ToString())),
                    PlantContribution = Convert.ToDecimal(ConvertNumber(dr["Plant Contribution"].ToString())),
                    RatingRate = Convert.ToDecimal(ConvertNumber(dr["Rating Rate"].ToString())),
                    TCT = Convert.ToDecimal(ConvertNumber(dr["TCT"].ToString())),
                    MonthlyTarget = Convert.ToDecimal(ConvertNumber(dr["MonthlyTarget"].ToString())),
                   
                };
            }
        }


        public FSADocument CreateFSADoc(string fileName, string loggedUser)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            FSADocument fsaDoc = new FSADocument
            {
                Id = Guid.NewGuid(),
                DocumentName = fileInfo.Name,
                DocumentType = fileInfo.Extension,
                UploadedAt = DateTime.Now,
                UploadedBy = loggedUser
            };

            return fsaDoc;
        }


        private static string ConvertNumber(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? "0" : input;
        }

    }



    
}
