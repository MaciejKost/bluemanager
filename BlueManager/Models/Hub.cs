using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlueManagerPlatform.Models
{
    public class Hub
    {
        public int Id { get; set; }
        [Display(Name = "Adres IP")]
        [Required(ErrorMessage = "required")]
        [RegularExpression("^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$",
            ErrorMessage = "Wprowadź prawidłowy adres IP")]
        public string IpAddress { get; set; }
        [Display(Name = "Obszar")]
        public string LocationName { get; set;}
        [Display(Name = "Aktywny")]
        public bool IsActive { get; set; }

        public string GetUrl() => $"http://{IpAddress}:8000/";
    }
}