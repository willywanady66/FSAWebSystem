﻿using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace FSAWebSystem.Models.Context
{
    public static class Configuration
    {
        public static async void Initialize(FSAWebSystemDbContext _db, UserManager<FSAWebSystemUser> userManager, IRoleService _roleService)
        {
            if (!_db.RoleUnilevers.Any())
            {
                var listRole = new List<RoleUnilever>
                {
                    new RoleUnilever
                    {
                        RoleUnileverId = Guid.NewGuid(),
                        RoleName = "Administrator"
                    },
                    new RoleUnilever
                    {
                        RoleUnileverId = Guid.NewGuid(),
                        RoleName = "Approver"
                    },
                    new RoleUnilever
                    {
                        RoleUnileverId = Guid.NewGuid(),
                        RoleName = "Requestor",
                    },
                    new RoleUnilever
                    {
                        RoleUnileverId = Guid.NewGuid(),
                        RoleName = "Support"
                    }
                };
                _db.RoleUnilevers.AddRange(listRole);
                
            }



            var user = Activator.CreateInstance<FSAWebSystemUser>();
            user.Email = "ww66696@gmail.com";
            user.UserName = user.Email;
            var res = await userManager.CreateAsync(user, "Unilever1*");
            if(res.Succeeded)
            {
                var userUnilever = new UserUnilever
                {
                    Name = user.UserName,
                    Email = user.Email,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "System",
                    RoleUnilever = await _roleService.GetRoleByName("administrator")
                };
                if (!_db.UsersUnilever.Any())
                {
                    _db.UsersUnilever.Add(userUnilever);
                }
            }

            _db.SaveChanges();
        }

        
    }
}
