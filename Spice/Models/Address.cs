using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public int PINCode { get; set; }
        [Required]
        [MaxLength(50)]
        public string Locality { get; set; }
        [Required]
        [MaxLength(255)]
        public string StreetAddress { get; set; }
        [Required]
        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        [MaxLength(50)]
        public string State { get; set; }
        [MaxLength(100)]
        public string Landmark { get; set; }
        
        public string AlternateMobileNumber { get; set; }
        [Required]
        public AddressType AddressType { get; set; }
        public AppUser AppUser { get; set; }
        [Required]
        public string AppUserId { get; set; }
        

    }
    public enum AddressType
    {
        Home,Work
    }
}
