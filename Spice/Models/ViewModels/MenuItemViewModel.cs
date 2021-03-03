using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }
        public IFormFile Thumbnail { get; set; }
        public IEnumerable<Catagory> Catagories { get; set; }
        public IEnumerable<SubCatagory> SubCatagories { get; set; }
    }
}
