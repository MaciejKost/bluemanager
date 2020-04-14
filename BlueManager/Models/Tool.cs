using System.ComponentModel.DataAnnotations;

namespace BlueManager.Models
{
    public class Tool
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "required")]
        [Display(Name = "Numer głowicy")]
        public string ToolName { get; set; }

        [Required(ErrorMessage = "required")]
        [RegularExpression("^([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})$",
            ErrorMessage = "Wprowadź prawidłowy adres MAC")]
        [Display(Name = "Adres MAC lokalizatora", Prompt = "AA:BB:CC:DD:EE:FF")]
        public string MacAddress { get; set; }



    }
}