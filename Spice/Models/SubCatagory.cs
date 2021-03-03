using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class SubCatagory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Subcategory Name")]
        public string Name { get; set; }
        //[Required]
        //public string Description { get; set; }
        [Required]
        public int CatagoryId { get; set; }
        [ForeignKey("CatagoryId")]
        public virtual Catagory Catagory { get; set; }
    }
}
