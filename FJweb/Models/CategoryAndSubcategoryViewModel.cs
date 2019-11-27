using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FJweb.Models
{
    public class CategoryAndSubcategoryViewModel
    {
        public int ID { get; set; }

        public int IDCategoryWork { get; set; }

        public int idRegion { get; set; }

        public int idCity { get; set; }

        public int IDTypePosition { get; set; }
    }
}