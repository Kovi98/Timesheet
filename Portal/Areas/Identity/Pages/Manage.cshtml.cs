using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Portal.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal.Areas.Identity.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class ManageModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public List<ApplicationUser> Users;

        public ManageModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.AsNoTracking().ToListAsync();

        }
        public async Task<IActionResult> OnPostActivateAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return Page();
                await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
            }
            catch
            {
                ModelState.AddModelError("Error", "Nastala neoèekávaná chyba!");
            }
            Users = await _userManager.Users.AsNoTracking().ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostDeactivateAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return Page();

                var numberOfAdmins = (await _userManager.GetUsersInRoleAsync(Roles.Member.ToString())).Count;
                if (numberOfAdmins <= 1)
                {
                    ModelState.AddModelError("Error", "Není možné odebrat posledního správce!");
                    Users = await _userManager.Users.AsNoTracking().ToListAsync();
                    return Page();
                }

                await _userManager.RemoveFromRoleAsync(user, Roles.Member.ToString());
            }
            catch
            {
                ModelState.AddModelError("Error", "Nastala neoèekávaná chyba!");
            }
            Users = await _userManager.Users.AsNoTracking().ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostPromoteAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return Page();
                if (await IsActiveAsync(user))
                    await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
            catch
            {
                ModelState.AddModelError("Error", "Nastala neoèekávaná chyba!");
            }
            Users = await _userManager.Users.AsNoTracking().ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostDemoteAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    return Page();

                var numberOfAdmins = (await _userManager.GetUsersInRoleAsync(Roles.Member.ToString())).Count;
                if (numberOfAdmins <= 1)
                {
                    ModelState.AddModelError("Error", "Není možné odebrat posledního správce!");
                    Users = await _userManager.Users.AsNoTracking().ToListAsync();
                    return Page();
                }

                await _userManager.RemoveFromRoleAsync(user, Roles.Admin.ToString());
            }
            catch
            {
                ModelState.AddModelError("Error", "Nastala neoèekávaná chyba!");
            }
            Users = await _userManager.Users.AsNoTracking().ToListAsync();
            return Page();
        }

        public async Task<bool> IsActiveAsync(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, Roles.Member.ToString());
        }
        public async Task<bool> IsAdminAsync(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, Roles.Admin.ToString());
        }
    }
}
