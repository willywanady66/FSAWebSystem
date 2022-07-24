using System;
using System.Collections.Generic;
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
        public Guid FSADocumentId { get; set; }
    }
}
