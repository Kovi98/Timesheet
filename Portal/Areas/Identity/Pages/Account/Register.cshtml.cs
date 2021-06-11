using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Portal.Validation;

namespace Portal.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger/*,
            IEmailSender emailSender*/)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            /*_emailSender = emailSender;*/
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "E-mail je povinný")]
            [EmailAddress(ErrorMessage = "E-mail musí mít správný formát.")]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Heslo je povinné")]
            [StringLength(100, ErrorMessage = "{0} musí být alespoň {2} a nejvíce {1} znaků dlouhé.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Heslo")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$", ErrorMessage = "Heslo musí obsahovat velké písmeno, malé písmeno, číslo, speciální znak a musí být dlouhé alespoň 6 znaků. ")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potvrdit heslo")]
            [Compare("Password", ErrorMessage = "Hesla se neshodují.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Jméno je povinné")]
            [Display(Name = "Jméno")]
            [StringLength(50, ErrorMessage = "{0} může být maximálně 50 znaků.")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Příjmení je povinné")]
            [Display(Name = "Příjmení")]
            [StringLength(50, ErrorMessage = "{0} může být maximálně 50 znaků.")]
            public string Surname { get; set; }
        }

        public async Task OnGetAsync()
        {
            return;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, Name = Input.Name, Surname = Input.Surname };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    ModelState.AddModelError("Success", "Registrace byla úspěšná! Je potřeba aktivovat účet správcem aplikace.");
                    return Page();
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
