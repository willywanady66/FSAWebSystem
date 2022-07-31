using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FSAWebSystem.Models
{
    public class WorkLevel
    {
        public Guid Id { get; set; }
        public string WL { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public Guid? FSADocumentId { get; set; }

        [NotMapped]
        public string Status { get; set; }
    }
}
