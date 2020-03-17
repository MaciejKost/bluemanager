using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlueManagerPlatform.Models
{
    public class Tool
    {
        public int Id { get; set; }
 //       [Required(ErrorMessage = "Please enter the tool name.")]

        public string Name { get; set; }
        [Display(Name = "Nazwa narzędzia:")]
        public string ObjName { get; set; } // Objekt przypisany do iBeacon'a
        [Required(ErrorMessage = "Wprowadź adres MAC urzadzenia.")]
      //  [StringLength(19, MinimumLength = 18, ErrorMessage = "The Last Name must be less than {1} characters.")]
        [Display(Name = "Adres MAC")]
        public string MacAddress { get; set; }
        public string Location { get; set; }
        public string Time { get; set; }

    }
}