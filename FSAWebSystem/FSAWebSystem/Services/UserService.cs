using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FSAWebSystem.Services
{
    public class UserService : IUserService
    {
        public FSAWebSystemDbContext _db;
        private readonly UserManager<FSAWebSystemUser> _userManager;
        //private readonly IUserStore<FSAWebSystemUser> _userStore;
        //private readonly IUserEmailStore<FSAWebSystemUser> _emailStore;
        private readonly IBannerService _bannerService;
        private readonly IRoleService _roleService;
        private readonly ISKUService _skuService;


        public UserService(FSAWebSystemDbContext db, UserManager<FSAWebSystemUser> userManager, IBannerService bannerService, IRoleService roleService, ISKUService skuService)
        {
            _db = db;
            _userManager = userManager;
            _bannerService = bannerService;
            _roleService = roleService;
            _skuService = skuService;

        }

        public async Task<List<UserUnilever>> GetAllUsers()
        {
            var users = await (from user in _userManager.Users
                               join userUnilever in _db.UsersUnilever.Include(x => x.Banners) on user.UserUnileverId equals userUnilever.Id
                               select new UserUnilever
                               {
                                   UserId = user.Id,
                                   Id = userUnilever.Id,
                                   Banners = userUnilever.Banners,
                                   RoleUnilever = userUnilever.RoleUnilever,
                                   IsActive = userUnilever.IsActive,
                                   Status = userUnilever.IsActive ? "Active" : "Non-Active",
                                   Name = userUnilever.Name,
                                   Email = userUnilever.Email,
                               }).ToListAsync();
            return users;
        }

        public async Task<UserPagingData> GetAllUsersPagination(DataTableParam param)
        {
            var users = (from user in _userManager.Users
                         join userUnilever in _db.UsersUnilever.Include(x => x.Banners) on user.UserUnileverId equals userUnilever.Id
                         join worklevel in _db.WorkLevels on userUnilever.WLId equals worklevel.Id into workLevelGroups
                         from wl in workLevelGroups.DefaultIfEmpty()
                         select new UserUnilever
                         {
                             UserId = user.Id,
                             Id = userUnilever.Id,
                             Banners = userUnilever.Banners.Where(x => x.IsActive).ToList(),
                             RoleUnilever = userUnilever.RoleUnilever,
                             Role = userUnilever.RoleUnilever.RoleName,
                             IsActive = userUnilever.IsActive,
                             Status = userUnilever.IsActive ? "Active" : "Non-Active",
                             Name = userUnilever.Name,
                             Email = userUnilever.Email,
                             WLName = wl != null ? wl.WL : string.Empty
                         });

            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                users = users.Where(x => x.RoleUnilever.RoleName.ToLower().Contains(search) || x.Name.ToLower().Contains(search) || x.Email.ToLower().Contains(search));
            }


            var totalCount = users.Count();
            var listUser = await users.Skip(param.start).Take(param.length).ToListAsync();
            return new UserPagingData
            {
                totalRecord = totalCount,
                userUnilevers = listUser
            };
        }

        public async Task<UserUnilever> GetUser(Guid id)
        {
            var userUnilever = await _db.UsersUnilever.Include(x => x.Banners).Include(x => x.RoleUnilever.Menus).Include(x => x.SKUs).Include(x => x.ProductCategories).SingleOrDefaultAsync(x => x.Id == id);
            return userUnilever;
        }

        public async Task<UserUnilever> GetUserOnly(Guid id)
        {
            var userUnilever = await _db.UsersUnilever.SingleOrDefaultAsync(x => x.Id == id);
            return userUnilever;
        }

        public async Task<UserUnilever> GetUserByEmail(string email)
        {
            return await _db.UsersUnilever.Include(x => x.Banners).Include(x => x.RoleUnilever.Menus).SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task SaveUsers(List<UserUnilever> users)
        {
            _db.UsersUnilever.AddRange(users);
        }

        public async Task<List<Banner>> GetUserBanners(Guid id)
        {
            var user = await GetUser(id);
            var userBanners = user.Banners;
            return userBanners;
        }

        public async Task<UserUnilever> Update(UserUnilever user, string loggedUser)
        {
            user.ModifiedAt = DateTime.Now;
            user.ModifiedBy = loggedUser;
            _db.UsersUnilever.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }


        public async Task<UserUnilever> CreateUser(string name, string email, string password, string[] bannerIds, string roleId, string worklevelId, string loggedUser, IUserStore<FSAWebSystemUser> _userStore, IUserEmailStore<FSAWebSystemUser> _emailStore, string[] skuIds, string[] categoryIds)
        {
            var selectedBannerIds = bannerIds.Select(x => Guid.Parse(x)).ToList();
            var selectedBanners = (_bannerService.GetAllBanner().ToList()).Where(x => selectedBannerIds.Contains(x.Id)).ToList();
            var selectedWl = new WorkLevel();
            if (!string.IsNullOrEmpty(worklevelId))
            {
                selectedWl = _db.WorkLevels.Single(x => x.Id == Guid.Parse(worklevelId));
            }

            var selectedSkuIds = skuIds.Select(x => Guid.Parse(x)).ToList();
            var selectedSKUs = (_skuService.GetAllProducts().ToList()).Where(x => selectedSkuIds.Contains(x.Id)).ToList();

            var selectedCategoryIds = categoryIds.Select(x => Guid.Parse(x)).ToList();
            var selectedCategories = (_skuService.GetAllProductCategories().ToList()).Where(x => selectedCategoryIds.Contains(x.Id)).ToList();
           

            var userUnilever = new UserUnilever
            {
                Name = name,
                Email = email,
                Password = password,
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CreatedBy = loggedUser,
                RoleUnilever = await _roleService.GetRole(Guid.Parse(roleId)),
                Banners = selectedBanners,
                WLId = selectedWl.Id != Guid.Empty ? selectedWl.Id : null,
                SKUs = selectedSKUs,
                ProductCategories = selectedCategories
            };

            var user = Activator.CreateInstance<FSAWebSystemUser>();
            user.UserUnileverId = userUnilever.Id;
            user.Role = userUnilever.RoleUnilever.RoleName;
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, password);

           
          
            if (result.Succeeded)
            {
                foreach (var menu in userUnilever.RoleUnilever.Menus)
                {
                    await _userManager.AddClaimAsync(user, new Claim("Menu", menu.Name));
                }


                _db.UsersUnilever.Add(userUnilever);
                await _db.SaveChangesAsync();
            }
            else
            {
                userUnilever.Message = result.Errors;
            }
            return userUnilever;
        }

        public IQueryable<WorkLevel> GetAllWorkLevel()
        {
            return _db.WorkLevels.AsQueryable();
        }

        public void SaveWorkLevels(List<WorkLevel> workLevels)
        {
            _db.WorkLevels.AddRange(workLevels);
        }

        public async Task<WorkLevelPagingData> GetAllWorkLevelPagination(DataTableParam param)
        {
            var workLevels = _db.WorkLevels.Select(x => new WorkLevel { Id = x.Id, WL = x.WL, IsActive = x.IsActive, Status = x.IsActive ? "Active" : "Non-Active" }).AsQueryable();

            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                workLevels = workLevels.Where(x => x.WL.ToLower().Contains(search) || x.Status.ToLower().Contains(search));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 1:
                        workLevels = order.dir == "desc" ? workLevels.OrderBy(x => x.Status).ThenByDescending(x => x.WL) : workLevels.OrderBy(x => x.Status).ThenBy(x => x.WL);
                        break;
                }
            }


            var totalCount = workLevels.Count();
            var listWL = await workLevels.Skip(param.start).Take(param.length).ToListAsync();
            return new WorkLevelPagingData
            {
                totalRecord = totalCount,
                workLevels = listWL
            };
        }

        public async Task FillWorkLevelDropdown(ViewDataDictionary viewData)
        {
            var workLevels = GetAllWorkLevel().Where(x => x.IsActive).ToList();
            List<SelectListItem> listWorkLevel = new List<SelectListItem>();
            listWorkLevel = workLevels.Select(x => new SelectListItem { Text = x.WL, Value = x.Id.ToString() }).ToList();
            viewData["ListWorkLevel"] = listWorkLevel;
        }

        public async Task<List<UserUnilever>> GetUserByRole(Guid roleId)
        {
            var users = await _db.UsersUnilever.Include(x => x.RoleUnilever).Where(x => x.RoleUnilever.RoleUnileverId == roleId).ToListAsync();
            return users;
        }

        public async Task<List<UserUnilever>> GetUserByWL(string workLevelName)
        {
            var worklevel = await _db.WorkLevels.SingleAsync(x => x.WL == workLevelName);
            var users = await _db.UsersUnilever.Where(x => x.WLId == worklevel.Id).ToListAsync();
            return users;
            
        }
    }
}
