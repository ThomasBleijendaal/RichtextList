using System.ComponentModel.DataAnnotations;

namespace RichText.Models
{
    public class SignInViewModel
    {
        [Required]
        [RegularExpression("^https://.*$")]
        public string Url { get; set; } = "https://jira.wearetriple.com";

        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}
