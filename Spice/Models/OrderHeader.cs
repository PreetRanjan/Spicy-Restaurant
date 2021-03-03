using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        [Required]
        
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public double OrderTotalOriginal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString ="{0:C}")]
        [Display(Name ="Order Total")]
        public double OrderTotal { get; set; }

        [Required]
        public DateTime PickUpTime { get; set; }

        
        [NotMapped]
        public DateTime PickUpDate { get; set; }

        public string CouponCode { get; set; }
        public double CouponCodeDiscount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string Comments { get; set; }
        [Required(ErrorMessage ="Name is required.")]
        [MaxLength(40)]
        public string PickerName { get; set; }
        [Required(ErrorMessage ="Phone number is required.")]
        [MinLength(10)]
        public string PickerPhoneNumber { get; set; }
        public string TransactionId { get; set; }

    }
    public enum PaymentStatus
    {
        Pending,Successful,Failed
    }
    public enum OrderStatus
    {
        Pending,BeingPrepared,Ready,Submitted,PickedUp
    }

}


