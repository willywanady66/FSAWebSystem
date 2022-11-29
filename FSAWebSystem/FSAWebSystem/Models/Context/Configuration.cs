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

            if (!_db.Menus.Any())
            {
                var listMenus = new List<Menu>
                {
                    new Menu
                    {
                        Id = Guid.NewGuid(),
                        Name = "Dashboard"
                    },
                    new Menu
                    {
                        Id = Guid.NewGuid(),
                        Name = "Report"
                    },
                    new Menu
                    {
                        Id = Guid.NewGuid(),
                        Name = "Proposal"
                    },
                    new Menu
                    {
                        Id = Guid.NewGuid(),
                        Name = "Approval"
                    },
                    new Menu
                    {
                        Id = Guid.NewGuid(),
                        Name = "Admin"
                    }
                };
                _db.AddRange(listMenus);
                _db.SaveChanges();
            }


            if (!_db.RoleUnilevers.Any())
            {
                var listRole = new List<RoleUnilever>
                {
                    new RoleUnilever
                    {
                        RoleUnileverId = Guid.NewGuid(),
                        RoleName = "Administrator",
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System",
                        Menus = _db.Menus.ToList()

                    },
                    new RoleUnilever
                    {
                        RoleUnileverId = Guid.NewGuid(),
                        RoleName = "Approver",
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System",
                        Menus = _db.Menus.Where(x => x.Name == "Approval").ToList()
                    },
                    new RoleUnilever
                    {
                        RoleUnileverId = Guid.NewGuid(),
                        RoleName = "Requestor",
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System",
                        Menus = _db.Menus.Where(x => x.Name == "Proposal").ToList()
                    },
                    new RoleUnilever
                    {
                        RoleUnileverId = Guid.NewGuid(),
                        RoleName = "Support",
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System"
                    }
                };
                _db.RoleUnilevers.AddRange(listRole);
                _db.SaveChanges();
            }

            var listWL = new List<WorkLevel>();
            var wls = new List<string>()
            {
                "KAM WL 1",
                "KAM WL 2",
                "SOM MT WL 1",
                "SOM MT WL 2",
                "CDM WL 3",
                "VP MTDA",
                "CCD",
                "CORE VP",
                "CD DIRECTOR"
            };
            if(_db.WorkLevels.Any())
            {
                var wlInDb = _db.WorkLevels.Where(x => wls.Contains(x.WL)).Select(x => x.WL).ToList();
                var wlToInsert = wls.Except(wlInDb);
                foreach(var wl in wlToInsert)
                {
                    var worklevel = new WorkLevel();
                    worklevel.IsActive = true;
                    worklevel.Id = Guid.NewGuid();
                    worklevel.WL = wl;
                    worklevel.CreatedAt = DateTime.Now;
                    worklevel.CreatedBy = "System";
                    listWL.Add(worklevel);
                }

            }
            else
            {
                var workLevels = wls.Select(x => new WorkLevel
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    WL = x,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "System"
                });

                listWL.AddRange(workLevels);
            }

            _db.WorkLevels.AddRange(listWL);
            _db.SaveChanges();

            

            var user = Activator.CreateInstance<FSAWebSystemUser>();
            user.Email = "admin@unilever.co.id";
            var savedUser = await userManager.FindByEmailAsync(user.Email);
            if(savedUser == null)
            {
                user.UserName = user.Email;
                user.UserUnileverId = Guid.NewGuid();
                user.Role = "Administrator";
                var res = await userManager.CreateAsync(user, "Administrator1*");
                if (res.Succeeded)
                {
                    var userUnilever = new UserUnilever
                    {
                        Id = (Guid)user.UserUnileverId,
                        Password = "Administrator1*",
                        Name = user.UserName,
                        Email = user.Email,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System",
                        RoleUnilever = await _roleService.GetRoleByName("Administrator"),
                        WLId = _db.WorkLevels.Single(x => x.WL == "CD DIRECTOR").Id
                    };

                    await userManager.AddClaimAsync(user, new Claim("Menu", "Admin"));
                    await userManager.AddClaimAsync(user, new Claim("Menu", "Approval"));
                    await userManager.AddClaimAsync(user, new Claim("Menu", "Proposal"));
                    await userManager.AddClaimAsync(user, new Claim("Menu", "Report"));
                    if (!_db.UsersUnilever.Any(x => x.Email == userUnilever.Email))
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
}
