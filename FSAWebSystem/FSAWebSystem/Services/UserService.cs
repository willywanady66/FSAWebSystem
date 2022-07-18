using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class UserService : IUserService
    {
        public FSAWebSystemDbContext _db;
        private readonly UserManager<FSAWebSystemUser> _userManager;

        public UserService(FSAWebSystemDbContext db, UserManager<FSAWebSystemUser> userManager)
		{
			_db = db;
			_userManager = userManager;
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
    }
}
