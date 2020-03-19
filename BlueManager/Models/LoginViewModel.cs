using System.ComponentModel.DataAnnotations;

namespace BlueManager.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "required")]
        [Display(Name = "Użytkownik", Prompt = "Użytkownik")]
        public string Username { get; set; }

        [Required(ErrorMessage = "required")]
        [Display(Name="Hasło")]
        public string Password { get; set; }
    }
}