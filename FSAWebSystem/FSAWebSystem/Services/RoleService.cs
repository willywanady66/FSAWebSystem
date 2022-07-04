using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly FSAWebSystemDbContext _db;
     
        public RoleService(FSAWebSystemDbContext db)
        {
            _db = db;
        }

        public async Task FillRoleDropdown(ViewDataDictionary viewData)
        {
            var roles = await GetAllRoles().ToListAsync();
            MultiDropDownListViewModel listRole = new MultiDropDownListViewModel();
            viewData["ListRole"] = roles.Select(x => new SelectListItem { Text = x.RoleName, Value = x.RoleUnileverId.ToString() }).ToList();
        }

        public IQueryable<RoleUnilever> GetAllRoles()
        {
            return _db.RoleUnilevers;
        }

        public async Task<RoleUnilever> GetRole(Guid id)
        {
            var role = await _db.RoleUnilevers.FindAsync(id);
            return role;
        }

        public async Task<RoleUnilever> GetRoleByName(string roleName)
        {
            return await _db.RoleUnilevers.SingleOrDefaultAsync(x => x.RoleName.ToUpper() == roleName.ToUpper());
        }
    }
}
