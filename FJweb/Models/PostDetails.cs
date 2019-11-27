using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FJweb.Models
{
    public class PostDetails
    {
        [Key]
        [Required]
        public string Post_Title { get; set; }

        public string Name_of_Publisher { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string Address { get; set; }

        public string Salary { get; set; }
        [Required]
        public string Typ_of_job { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Experience { get; set; }
        [Required]
        public string CategoryPosition { get; set; }
        [Required]
        public string Subcategory { get; set; }
        [Required]
        public string NumberOfPosition { get; set; } 
        [Required]
        public string dateTime { get { return DateTime.Now.ToString();} }
       
    }
}