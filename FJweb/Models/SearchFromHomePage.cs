using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FJweb.Models
{
    public class SearchFromHomePage
    {
        [Required]
        public string CategoryPosition { get; set; }

    }
}