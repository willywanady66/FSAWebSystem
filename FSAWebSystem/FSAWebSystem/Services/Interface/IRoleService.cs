using FSAWebSystem.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FSAWebSystem.Services.Interface
{
    public interface IRoleService
    {
        public IQueryable<RoleUnilever> GetAllRoles();

        public Task<RoleUnilever> GetRole(Guid id);

        public Task<RoleUnilever> GetRoleByName (string roleName);

        public Task FillRoleDropdown(ViewDataDictionary viewData);
    }
}
