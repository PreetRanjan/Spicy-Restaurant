using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
            MenuItem = new MenuItem();
        }
        [Key]
        public int Id { get; set; }

        public string AppUserId { get; set; }

        [NotMapped]
        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; }

        [NotMapped]
        [ForeignKey("AppUserId")]
        public virtual MenuItem MenuItem { get; set; }

        public int MenuItemId { get; set; }
        [Range(1,100,ErrorMessage ="Min. Value is 1 and Max. Value is 100")]
        public int Count { get; set; }
    }
}
