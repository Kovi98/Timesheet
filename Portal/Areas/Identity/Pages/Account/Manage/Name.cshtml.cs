using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Portal.Areas.Identity.Pages.Account.Manage
{
    public class NameModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public NameModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public string Name { get; set; }
        public string Surname { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Jméno je povinné")]
            [Display(Name = "Jméno")]
            [StringLength(50, ErrorMessage = "{0} mùže být maximálnì 50 znakù.")]
            public string NewName { get; set; }
            [Required(ErrorMessage = "Pøíjmení je povinné")]
            [Display(Name = "Pøíjmení")]
            [StringLength(50, ErrorMessage = "{0} mùže být maximálnì 50 znakù.")]
            public string NewSurname { get; set; }
        }
        private async Task LoadAsync(ApplicationUser user)
        {
            Name = user.Name;
            Surname = user.Surname;

            Input = new InputModel
            {
                NewName = Name,
                NewSurname = Surname
            };
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
        public async Task<IActionResult> OnPostChangeNameAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var name = user.Name;
            var surname = user.Surname;
            if (Input.NewName != name || Input.NewSurname != surname)
            {
                if (Input.NewName != name)
                    user.Name = Input.NewName;
                if (Input.NewSurname != surname)
                    user.Surname = Input.NewSurname;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    StatusMessage = "Jméno bylo úspìšnì zmìnìno.";
                    return RedirectToPage("/Account/Manage/Index", new { Area = "Identity" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
            }

            StatusMessage = "Jméno zùstalo nezmìnìno.";
            return RedirectToPage("/Account/Manage/Index", new { Area = "Identity" });
        }
    }
}
