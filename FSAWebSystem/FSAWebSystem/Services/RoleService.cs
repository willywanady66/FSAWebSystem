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
            return _db.RoleUnilevers.Include(x => x.Menus);
        }

        public async Task<RoleUnilever> GetRole(Guid id)
        {
            var role = await _db.RoleUnilevers.Include(x => x.Menus).SingleOrDefaultAsync(x => x.RoleUnileverId == id);
            return role;
        }

        public async Task AddRole(RoleUnilever role)
        {
            await _db.AddAsync(role);
            await _db.SaveChangesAsync();
        }

        public async Task<RoleUnilever> GetRoleByName(string roleName)
        {
            return await _db.RoleUnilevers.Include(x => x.Menus).SingleOrDefaultAsync(x => x.RoleName.ToUpper() == roleName.ToUpper());
        }

        public List<SelectListItem> GetMenuDropdown()
        {
            var menus = _db.Menus.ToList();
            List<SelectListItem> listMenus = menus.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return listMenus;
        }

        public IQueryable<Menu> GetAllMenu()
        {
            return _db.Menus;
       
        }
        public async Task<RoleUnilever> Update(RoleUnilever role)
        {
            _db.RoleUnilevers.Update(role);
            await _db.SaveChangesAsync();
            return role;
        }
    }
}
