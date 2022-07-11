using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class UserService : IUserService
    {
        public FSAWebSystemDbContext _db;

        public UserService(FSAWebSystemDbContext db)
        {
            _db = db;
        }

        public async Task<List<UserUnilever>> GetAllUsers()
        {
            return await _db.UsersUnilever.Include(x => x.Banners).ToListAsync();
        }

        public async Task<UserUnilever> GetUser(Guid id)
        {
            return await _db.UsersUnilever.Include(x => x.Banners).SingleOrDefaultAsync(x => x.Id == id);
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
