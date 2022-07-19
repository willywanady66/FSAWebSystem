using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
                _db.SaveChanges();
            }


           
            var user = Activator.CreateInstance<FSAWebSystemUser>();
            user.Email = "admin@gmail.com";
            user.UserName = user.Email;
            user.UserUnileverId = Guid.NewGuid();
            var res = await userManager.CreateAsync(user, "Administrator1*");
            if(res.Succeeded)
            {
               
                var userUnilever = new UserUnilever
                {
                    Id = (Guid)user.UserUnileverId,
                    Name = user.UserName,
                    Email = user.Email,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "System",
                    RoleUnilever = await _roleService.GetRoleByName("Administrator")
                };

                await userManager.AddClaimAsync(user, new Claim("Role", userUnilever.RoleUnilever.RoleName));
                if (!_db.UsersUnilever.Any())
                {
                    _db.UsersUnilever.Add(userUnilever);
                }
                try
                {
                    _db.SaveChanges();
                    Console.WriteLine("Success");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
               
            }
        }

        
    }
}
