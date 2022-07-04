using System.ComponentModel.DataAnnotations;

namespace FSAWebSystem.Models
{
    public class RoleUnilever
    {
        [Key]
        public Guid RoleUnileverId { get; set; }
        public string RoleName { get; set; }
    }
}
