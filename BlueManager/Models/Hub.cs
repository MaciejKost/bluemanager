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
        public string IpAddress { get; set; }
        [Display(Name = "Lokalizacja")]
        public string LocationName { get; set;}

        public string GetUrl() => $"http://{IpAddress}:8000/";
    }
}