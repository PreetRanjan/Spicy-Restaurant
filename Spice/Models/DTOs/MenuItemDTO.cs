using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.DTOs
{
    public class MenuItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ESpicy Spicyness { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }
        public string CatagoryName { get; set; }
        public string SubCatagoryName { get; set; }
        //public CatagoryDTO Catagory { get; set; }
        //public SubCatagoryDTO SubCatagory { get; set; }
        public bool IsFeatured { get; set; }
        public double Price { get; set; }
    }
    public class CatagoryDTO
    {
        public string Name { get; set; }
    }
    public class SubCatagoryDTO
    {
        
        public string Name { get; set; }
        public CatagoryDTO CatagoryDTO { get; set; }
    }
    public class CouponDTO
    {
        public string Name { get; set; }
        public CouponType CouponType { get; set; }
        public double Discount { get; set; }
        public byte[] Picture { get; set; }
    }
}
