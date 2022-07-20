﻿using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Identity;
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


        public UserService(FSAWebSystemDbContext db, UserManager<FSAWebSystemUser> userManager, IBannerService bannerService, IRoleService roleService)
		{
			_db = db;
			_userManager = userManager;
            _bannerService = bannerService;
            _roleService = roleService;

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
                             Name = userUnilever.Name,
                             Email = userUnilever.Email,
                         }).ToListAsync();
            return users;
        }

        public async Task<UserUnilever> GetUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var userUnilever = await _db.UsersUnilever.Include(x => x.Banners).SingleOrDefaultAsync(x => x.Id == user.UserUnileverId);
            return userUnilever;
        }

        public async Task<UserUnilever> GetUserByEmail(string email)
        {
            return await _db.UsersUnilever.Include(x => x.Banners).SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task SaveUsers (List<UserUnilever> users)
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


        public async Task<UserUnilever> CreateUser(string name, string email, string password, string[] bannerIds, string roleId, string loggedUser, IUserStore<FSAWebSystemUser> _userStore, IUserEmailStore<FSAWebSystemUser> _emailStore)
        {
            var selectedBannerIds = bannerIds.Select(x => Guid.Parse(x)).ToList();
            var selectedBanners = (_bannerService.GetAllBanner().ToList()).Where(x => selectedBannerIds.Contains(x.Id)).ToList();
            var userUnilever = new UserUnilever
            {
                Name = name,
                Email = email,
                //Password = Input.Password,
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CreatedBy = loggedUser,
                RoleUnilever = await _roleService.GetRole(Guid.Parse(roleId)),
                Banners = selectedBanners
            };

            var user = Activator.CreateInstance<FSAWebSystemUser>();
            user.UserUnileverId = userUnilever.Id;
            user.Role = userUnilever.RoleUnilever.RoleName;
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, password);
            await _userManager.AddClaimAsync(user, new Claim("Role", userUnilever.RoleUnilever.RoleName));
            if (result.Succeeded)
            {
                _db.UsersUnilever.Add(userUnilever);
                await _db.SaveChangesAsync();
            }
            else
            {
                userUnilever.Message = result.Errors;
            }
            return userUnilever;
        }
    }
}
