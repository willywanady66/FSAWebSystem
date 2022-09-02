using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FSAWebSystem.Services.Interface
{
    public interface IUserService
    {
        public Task<List<UserUnilever>> GetAllUsers();
        public Task<UserPagingData> GetAllUsersPagination(DataTableParam param);
        public Task<UserUnilever> GetUser(Guid id);
        public Task<UserUnilever> GetUserOnly(Guid id);
        public Task<UserUnilever> GetUserByEmail(string email);
        public Task SaveUsers(List<UserUnilever> users);    
        public Task<UserUnilever> Update(UserUnilever user, string loggedUser);

        public Task<List<Banner>> GetUserBanners (Guid id);

        public Task<UserUnilever> CreateUser(string name, string email, string password, string[] bannerIds, string roleId, string worklevelId, string loggedUser, IUserStore<FSAWebSystemUser> userStore, IUserEmailStore<FSAWebSystemUser> _emailStore, string[] skuIds, string[] categoryIds);
        public IQueryable<WorkLevel> GetAllWorkLevel();

        public void SaveWorkLevels(List<WorkLevel> workLevels);

        public Task<WorkLevelPagingData> GetAllWorkLevelPagination(DataTableParam param);
        public Task FillWorkLevelDropdown(ViewDataDictionary viewData);
        public Task<List<UserUnilever>> GetUserByRole(Guid roleId);

        public Task<List<UserUnilever>> GetUserByWL(string workLevelName);
    }
}
