using FSAWebSystem.Models;

namespace FSAWebSystem.Services.Interface
{
    public interface IUserService
    {
        public Task<List<UserUnilever>> GetAllUsers();
        public Task<UserUnilever> GetUser(Guid id);
        public Task<UserUnilever> GetUserByEmail(string email);
        public Task SaveUsers(List<UserUnilever> users);    
        public Task<UserUnilever> Update(UserUnilever user, string loggedUser);

        public Task<List<Banner>> GetUserBanners (Guid id);
    }
}
