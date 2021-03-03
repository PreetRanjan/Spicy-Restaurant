using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug format not valid.")]
        [StringLength(160)]
        public string Slug { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public ESpicy Spicyness { get; set; }
        
        public string Image { get; set; }
        [Required]
        public int CatagorId { get; set; }
        [ForeignKey("CatagorId")]
        public Catagory Catagory { get; set; }
        [Required]
        public int SubCatagorId { get; set; }
        [ForeignKey("SubCatagorId")]
        public SubCatagory SubCatagory { get; set; }
        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="The price must be ₹10 or Higher")]
        public double Price { get; set; }

        public bool IsFeatured { get; set; } = false;
    }
    public enum ESpicy
    {
        NA=0,NotSpicy,Spicy,VerySpicy
    }
}
