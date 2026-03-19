using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebAPP.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;

        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        [BindProperty]
        public RegisterViewModel registerViewModel { get; set; } = new RegisterViewModel();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(registerViewModel.ConfirmPassword), "两次输入的密码不一致");
                return Page();
            }

            var user = new IdentityUser
            {
                UserName = registerViewModel.Email,
                Email = registerViewModel.Email
            };

            var result = await userManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
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
    }
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "邮件地址无效")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
