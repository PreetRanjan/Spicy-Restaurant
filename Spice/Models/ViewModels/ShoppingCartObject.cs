using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class ShoppingCartObject
    {
        [Required]
        [HiddenInput]
        public int MenuItemId { get; set; }
        [Required]
        [Range(1,10,ErrorMessage ="Item quantity must be 1-10")]
        public int Count { get; set; } = 1;
        //[Required]
        //[HiddenInput]
        //public string AppUserId { get; set; }
    }
}
