using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public CouponType CouponType { get; set; }
        [Required]
        public double Discount { get; set; }
        [Required]
        public double MinimumAmount { get; set; }
        
        public byte[] Picture { get; set; }

        public bool IsActive { get; set; }
    }
    public enum CouponType
    {
        Percent=1,Dollar,Rupee
    }
}
