using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp_Security.Pages
{
    [Authorize("AdminOnly")]
    public class SettingsModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            // Load current settings - in real app load from config or database
            Input.SiteTitle = "我的网站";
            Input.SupportEmail = "support@example.com";
            Input.AllowRegistration = true;
            Input.DefaultUserRole = "User";
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Simulate saving settings - replace with persistent storage
            StatusMessage = "设置已保存。";
            return Page();
        }
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "站点标题")]
        public string SiteTitle { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "支持邮箱")]
        public string SupportEmail { get; set; } = string.Empty;

        [Display(Name = "允许注册")]
        public bool AllowRegistration { get; set; }

        [Display(Name = "默认用户角色")]
        public string DefaultUserRole { get; set; } = string.Empty;
    }
}
