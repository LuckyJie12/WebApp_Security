using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApp_Security.Authorization;

namespace WebApp_Security.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Simple example authentication - replace with real authentication logic
            if (Credential is { Username: "admin", Password: "123" })
            {
                // On success redirect to home page
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Credential.Username),
                    new Claim(ClaimTypes.Email, "snow.hero@qq.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("DepartmentDate","2025-3-13")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe,
                };

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);

                // Redirect to return URL if local, otherwise to home
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "登录失败，用户或者密码输入错误！");
            return Page();
        }
    }
}
