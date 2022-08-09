using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
	public class UserUnilever
	{

		public Guid Id { get; set; }
		public Guid? WLId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public List<Banner>? Banners { get; set; }
		public List<Approval>? Approvals { get; set; }
        public RoleUnilever RoleUnilever { get; set; }
        public DateTime? CreatedAt { get; set; }
		public string? CreatedBy { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? ModifiedBy { get; set; }
		
		public bool IsActive { get; set; }
		public Guid? FSADocumentId { get; set; }

		//[NotMapped] public Banner Banner { get; set; }
		[NotMapped]
		public string BannerName { get; set; }
        [NotMapped]
		public string PlantCode { get; set; }
        [NotMapped]
		public string Role { get; set; }
		[NotMapped]
		public string UserId { get; set; }
		[NotMapped]
		public string Status { get; set; }
		[NotMapped]
        public IEnumerable<IdentityError> Message { get; set; }
        [NotMapped]
		public string WLName { get; set; }
	}
}
