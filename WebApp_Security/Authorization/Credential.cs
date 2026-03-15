using System.ComponentModel.DataAnnotations;

namespace WebApp_Security.Authorization
{
    public class Credential
    {
        [Required]
        [Display(Name = "用户名称")]
        public string Username { get; set; } = string.Empty;
        [Required]
        [Display(Name = "用户密码")]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }
    }
}
