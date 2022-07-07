using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace FSAWebSystem.Areas.Identity.Data
{
    public class FSAWebSystemUser : IdentityUser
    {
        public Guid? UserUnileverId { get; set; }

        public string? Role { get; set; }

        [NotMapped]
        public List<string> BannerName { get; set; }
    }
}

// Add profile data for application users by adding properties to the FSAWebSystemUser class


