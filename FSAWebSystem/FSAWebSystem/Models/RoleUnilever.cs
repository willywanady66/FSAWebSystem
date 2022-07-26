using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class RoleUnilever
    {
        public RoleUnilever()
        {
            Menus = new List<Menu>();
        }
        [Key]
        public Guid RoleUnileverId { get; set; }
        public string RoleName { get; set; }
        public List<Menu> Menus { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [NotMapped]
        public string Menu { get; set; }
    }
}
