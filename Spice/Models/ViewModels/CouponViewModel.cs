using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class CouponViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name="Coupon Type")]
        public CouponType CouponType { get; set; }

        [Required][Range(0,100)]
        public double Discount { get; set; }

        [Required]
        [Display(Name ="Minimum Amount")]
        public double MinimumAmount { get; set; }
        [Required]
        public IFormFile Picture { get; set; }
        [Required]
        [Display(Name = "Is-Coupon Active??")]
        public bool IsActive { get; set; } = true;
    }
}
