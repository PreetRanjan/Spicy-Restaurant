using Spice.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<MenuItemDTO> MenuItems { get; set; }
        public IEnumerable<CatagoryDTO> Catagories { get; set; }
        public IEnumerable<CouponDTO> Coupons { get; set; }
    }
}
