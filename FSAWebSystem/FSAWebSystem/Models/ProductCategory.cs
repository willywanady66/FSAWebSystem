using System.ComponentModel;

namespace FSAWebSystem.Models
{
    public class ProductCategory
    {
        public Guid Id { get; set; }
        public string CategoryProduct { get; set; }
        
        //[DefaultValue(true)]
        //public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        //public List<SKU> SKUs { get; set; }
    }
}
