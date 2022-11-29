using FSAWebSystem.Models;
using FSAWebSystem.Models.ApprovalSystemCheckModel;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using System.ComponentModel;

namespace FSAWebSystem.Services
{
    public class UploadDocumentService : IUploadDocumentService
    {
        public FSAWebSystemDbContext _db;
        public UploadDocumentService(FSAWebSystemDbContext db)
        {
            _db = db;
        }

        public static string GetEnumDesc(Enum value)
        {
            DescriptionAttribute attribute = value.GetType()
            .GetField(value.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public List<string> GetSKUColumns()
        {
            return new List<string>
            {
                "PC Map",
                "Description Map",
                "Category"
            };
        }

        public List<string> GetMonthlyBucketColumns()
        {
            return new List<string>
            {
                "SWF 2",
                "SWF 3",
                "Banner",
                "PC Map",
                "Description",
                "Category",
                "Plant Code",
                "Plant",    
                "Price",
                "Plant Contribution",
                "Running Rate",
                "TCT",
                "Monthly Target"
            };
        }


        public List<string> GetBannerColumns()
        {
            return new List<string>
            {
                "Trade",
                "CDM",
                "KAM",
                "Banner Name",
                "Plant Name",
                "Plant Code"
            };
        }

        public List<string> GetAndromedaColumns()
        {
            return new List<string>
            {
                "Material",
                "Description",
                "UU Stock",
                "IT This Week",
                "RR_ACT 13 WK"
            };
        }

        public List<string> GetBottomPriceColumns()
        {
            return new List<string>
            {
                "PC MAP",
                "Description",
                "Average of Normal Price",
                "Average of Bottom Price",
                "Average of Actual Price",
                "Min of Actual Price",
                "Gap",
                "Remark"
            };
        }

        public List<string> GetITrustColumns()
        {
            return new List<string>
            {
                "PC MAP",
                "Description",
                "Sum of Intransit",
                "Sum of Stock",
                "Sum of Final RPP"
            };
        }

        public async Task SaveMonthlyBuckets(List<MonthlyBucket> monthlyBuckets)
        {
            await _db.MonthlyBuckets.AddRangeAsync(monthlyBuckets);
        }

        public FSADocument CreateFSADoc(string fileName, string loggedUser, DocumentUpload docType)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            FSADocument fsaDoc = new FSADocument
            {
                Id = Guid.NewGuid(),
                DocumentName = fileInfo.Name,
                DocumentType = docType.ToString(),
                UploadedAt = DateTime.Now,
                UploadedBy = loggedUser
            };

            return fsaDoc;
        }

        public async Task SaveDocument(FSADocument fsaDoc)
        {
            await _db.FSADocuments.AddAsync(fsaDoc);
        }

        public List<string> GetWeeklyDispatchColumns()
        {
            return new List<string>
            {
                "Unik",
                "Account",
                "Banner Name",
                "Material",
                "Material Description",
                "Description",
                "Dispatch / Consume",
            };
        }

        public List<string> GetDailyOrderColumns()
        {
            return new List<string>
            {
                "Account",
                "Banner Name",
                "Category Name",
                "Market Name",
                "Corp Brand Name",
                "PC Map",
                "PC Description",
                "Flag SKU FSA",
                "Original Order",
                "Valid Order + BJ"
            };
        }

		public async Task SaveWeeklyBuckets(List<WeeklyBucket> weeklyBuckets)
		{
            await _db.WeeklyBuckets.AddRangeAsync(weeklyBuckets);
		}

        public async Task SaveWeeklyBucketHistories(List<WeeklyBucketHistory> weeklyBucketHistories)
        {
            await _db.WeeklyBucketHistories.AddRangeAsync(weeklyBucketHistories);
        }

        public async Task SaveAndromeda(List<AndromedaModel> listAndromeda)
        {
            await _db.Andromedas.AddRangeAsync(listAndromeda);
        }

        public async Task SaveITrust(List<ITrustModel> listITrust)
        {
            await _db.ITrusts.AddRangeAsync(listITrust);
        }

        public async Task SaveBottomPrice(List<BottomPriceModel> listBottomPrice)
        {
            await _db.BottomPrices.AddRangeAsync(listBottomPrice);
        }

        public async Task DeleteAndromeda()
        {
            var andromedas = _db.Andromedas;
            _db.Andromedas.RemoveRange(andromedas);
        }

        public async Task DeleteBottomPrice()
        {
            var bottomPrices = _db.BottomPrices;
            _db.BottomPrices.RemoveRange(bottomPrices);
        }

        public async Task DeleteITrust()
        {
            var iTrust= _db.ITrusts;
            _db.ITrusts.RemoveRange(iTrust);
        }
        public List<string> GetUserColumns()
        {
            return new List<string>
            {
                "Name",
                "Email",
                "Password",
                "Role",
                "Banner Name",
                "Plant Code",
                "Work Level"
            };
        }
    }



    public enum DocumentUpload
    {
        [Description("User")]
        User,
        [Description("Banner")]
        Banner,
        [Description("SKU")]
        SKU,
        [Description("Monthly Bucket")]
        MonthlyBucket,
        [Description("Weekly Dispatch")]
        WeeklyDispatch,
        [Description("Daily Order")]
        DailyOrder,
        [Description("Andromeda")]
        Andromeda,
        [Description("Bottom Price")]
        BottomPrice,
        [Description("I-TRUST")]
        ITRUST
    }
}
