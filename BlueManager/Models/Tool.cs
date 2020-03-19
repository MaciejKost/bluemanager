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

        //[Required] // setting this field as required causes errors on reading tools from DB - separate ViewModel?
        public string Name { get; set; }

        [Display(Name = "Nazwa narzędzia")]
        public string ObjName { get; set; } // Obiekt przypisany do iBeacon'a

        [Required(ErrorMessage = "required")]
        [RegularExpression("^([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})$",
            ErrorMessage = "Wprowadź prawidłowy adres MAC")]
        [Display(Name = "Adres MAC", Prompt = "AA:BB:CC:DD:EE:FF")]
        public string MacAddress { get; set; }

        public string Location { get; set; }

        public string Time { get; set; }

    }
}