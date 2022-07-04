using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using System.ComponentModel;

namespace FSAWebSystem.Services
{
    public class UploadDocumentService
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



        public static List<string> GetProductColumn()
        {
            return new List<string>
            {
                "PC Map",
                "Description Map",
                "Category"
            };
        }

        public static List<string> GetMonthlyBucketColumn()
        {
            return new List<string>
            {
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

    }



    public enum DocumentUpload
    {
        [Description("Product")]
        Product,
        [Description("Monthly Bucket")]
        MonthlyBucket
    }
}
