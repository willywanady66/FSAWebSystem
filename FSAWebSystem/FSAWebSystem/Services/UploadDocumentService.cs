using FSAWebSystem.Models;
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
                "Rating Rate",
                "TCT",
                "Monthly Target"
            };
        }


        public List<string> GetBannerColumns()
        {
            return new List<string>
            {
                "Trade",
                "Banner Name",
                "Plant Name",
                "Plant Code"
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

        public List<string> GetWeeklyDispatchColumn()
        {
            return new List<string>
            {
                "Unik",
                "Account",
                "Material",
                "Material Description",
                "Description",
                "Dispatch / Consume",
            };
        }

        public async Task SaveDocument(FSADocument fsaDoc)
        {
            await _db.FSADocuments.AddAsync(fsaDoc);
        }

        public List<string> GetWeeklyDispatchColumns()
        {
            throw new NotImplementedException();
        }

        public List<string> GetDailyOrderColumns()
        {
            throw new NotImplementedException();
        }

		public async Task SaveWeeklyBuckets(List<WeeklyBucket> weeklyBuckets)
		{
            await _db.WeeklyBuckets.AddRangeAsync(weeklyBuckets);
		}

        public List<string> GetUserColumns()
        {
            return new List<string>
            {
                "Name",
                "Email",
                "Password",
                "Role",
                "BannerName"
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
        DailyOrder
    }
}
