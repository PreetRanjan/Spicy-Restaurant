using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class SubCatagoryViewModel
    {
        public IEnumerable<Catagory> CatagoryList { get; set; }
        public List<string> SubCatagoryList { get; set; }
        public SubCatagory SubCatagory { get; set; }
        public string StatusMessage { get; set; }
    }
}
