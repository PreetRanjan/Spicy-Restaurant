using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using Stripe;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;

        [BindProperty]
        public OrderDetailsCart DetailCart { get; set; }

        [BindProperty]
        public CouponStatus? CStatus { get; set; }
        public CartController(ApplicationDbContext _db)
        {
            db = _db;
            CStatus = CouponStatus.Removed;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            DetailCart = new OrderDetailsCart
            {
                OrderHeader = new Models.OrderHeader()
            };
            DetailCart.OrderHeader.OrderTotal = 0;
            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            DetailCart.OrderHeader.AppUserId = userId;
            var cart = db.ShoppingCarts.Where(c => c.AppUserId == userId);

            if (cart != null)
            {
                DetailCart.ShoppingCarts = cart.ToList();
            }
            foreach (var item in DetailCart.ShoppingCarts)
            {
                item.MenuItem = await db.MenuItems.FirstOrDefaultAsync(c => c.Id == item.MenuItemId);
                DetailCart.OrderHeader.OrderTotal += (item.MenuItem.Price * item.Count);
                item.MenuItem.Description = SD.ConvertToRawHtml(item.MenuItem.Description);
                if (item.MenuItem.Description.Length > 100)
                {
                    item.MenuItem.Description = item.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            DetailCart.OrderHeader.OrderTotalOriginal = DetailCart.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.CartCoupon) != null)
            {
                DetailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.CartCoupon);
                var coupon = await db.Coupons.SingleOrDefaultAsync(c => c.Name.ToLower() == DetailCart.OrderHeader.CouponCode.ToLower());
                if (coupon == null)
                {
                    ViewBag.InvalidCoupon = "Coupon is expired or Not valid";
                }
                DetailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(coupon, DetailCart.OrderHeader.OrderTotalOriginal);
            }
            var discount = DetailCart.OrderHeader.OrderTotalOriginal - DetailCart.OrderHeader.OrderTotal;
            if (discount > 0)
            {
                ViewBag.CStatus = SD.CouponApplied;
            }
            return View(DetailCart);
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cart.Count == 1)
            {
                db.ShoppingCarts.Remove(cart);
                await db.SaveChangesAsync();

                var cnt = db.ShoppingCarts.Where(u => u.AppUserId == cart.AppUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.CartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);

            db.ShoppingCarts.Remove(cart);
            await db.SaveChangesAsync();

            var cnt = db.ShoppingCarts.Where(u => u.AppUserId == cart.AppUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.CartCount, cnt);


            return RedirectToAction(nameof(Index));
        }

        public IActionResult ApplyCoupon()
        {
            if (DetailCart.OrderHeader.CouponCode == null)
            {
                DetailCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.CartCoupon, DetailCart.OrderHeader.CouponCode);
            ViewBag.CStatus = SD.CouponApplied;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {

            HttpContext.Session.SetString(SD.CartCoupon, string.Empty);
            ViewBag.CStatus = SD.CouponRemoved;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderSummary()
        {
            DetailCart = new OrderDetailsCart
            {
                OrderHeader = new Models.OrderHeader()
            };
            DetailCart.OrderHeader.OrderTotal = 0;
            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var appUser = await db.AppUsers.SingleOrDefaultAsync(m => m.Id == userId);
            var cart = db.ShoppingCarts.Where(c => c.AppUserId == userId);

            if (cart != null)
            {
                DetailCart.ShoppingCarts = cart.ToList();
            }
            foreach (var item in DetailCart.ShoppingCarts)
            {
                item.MenuItem = await db.MenuItems.FirstOrDefaultAsync(c => c.Id == item.MenuItemId);
                DetailCart.OrderHeader.OrderTotal += (item.MenuItem.Price * item.Count);

            }
            DetailCart.OrderHeader.OrderTotalOriginal = DetailCart.OrderHeader.OrderTotal;
            DetailCart.OrderHeader.PickerName = appUser.Name;
            DetailCart.OrderHeader.PickerPhoneNumber = appUser.PhoneNumber;
            DetailCart.OrderHeader.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(SD.CartCoupon) != null)
            {
                DetailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.CartCoupon);
                var coupon = await db.Coupons.SingleOrDefaultAsync(c => c.Name.ToLower() == DetailCart.OrderHeader.CouponCode.ToLower());
                if (coupon == null)
                {
                    ViewBag.InvalidCoupon = "Coupon is expired or Not valid";
                }
                DetailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(coupon, DetailCart.OrderHeader.OrderTotalOriginal);
            }
            var discount = DetailCart.OrderHeader.OrderTotalOriginal - DetailCart.OrderHeader.OrderTotal;
            if (discount > 0)
            {
                ViewBag.CStatus = SD.CouponApplied;
            }
            DetailCart.OrderHeader.AppUserId = userId;
            DetailCart.OrderHeader.Comments = string.Empty;
            return View(DetailCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("OrderSummary")]
        public async Task<IActionResult> OrderSummaryPost(string stripeToken)
        {
            //Get signed in user
            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;

            //Get all shopping cart items
            var cart = db.ShoppingCarts.Where(c => c.AppUserId == userId);
            if (cart != null)
            {
                DetailCart.ShoppingCarts = cart.ToList();
            }
            DetailCart.OrderHeader.PaymentStatus = PaymentStatus.Pending;
            DetailCart.OrderHeader.OrderDate = DateTime.Now;
            DetailCart.OrderHeader.Status = OrderStatus.Pending;
            DetailCart.OrderHeader.PickUpTime = Convert.ToDateTime(DetailCart.OrderHeader.PickUpDate.ToShortDateString() + " "
                + DetailCart.OrderHeader.PickUpTime.ToShortTimeString());

            var orderList = new List<OrderDetail>();
            await db.OrderHeaders.AddAsync(DetailCart.OrderHeader);
            await db.SaveChangesAsync();

            DetailCart.OrderHeader.OrderTotalOriginal = 0;
            //Calculate order total
            foreach (var item in DetailCart.ShoppingCarts)
            {
                item.MenuItem = await db.MenuItems.FirstOrDefaultAsync(c => c.Id == item.MenuItemId);
                var orderDetail = new OrderDetail
                {
                    MenuItemId = item.MenuItemId,
                    OrderHeaderId = DetailCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count

                };
                DetailCart.OrderHeader.OrderTotalOriginal += orderDetail.Count * orderDetail.Price;
                await db.OrderDetails.AddAsync(orderDetail);
            }
            //Applycoupon from session
            if (HttpContext.Session.GetString(SD.CartCoupon) != null)
            {
                DetailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.CartCoupon);
                var coupon = await db.Coupons.SingleOrDefaultAsync(c => c.Name.ToLower() == DetailCart.OrderHeader.CouponCode.ToLower());
                if (coupon == null || coupon.MinimumAmount <= DetailCart.OrderHeader.OrderTotal)
                {
                    ViewBag.InvalidCoupon = "Coupon is Not valid/Expired.";
                }
                DetailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(coupon, DetailCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                DetailCart.OrderHeader.CouponCode = null;
                DetailCart.OrderHeader.OrderTotal = DetailCart.OrderHeader.OrderTotalOriginal;
            }
            //Calculate discount
            var discount = DetailCart.OrderHeader.OrderTotalOriginal - DetailCart.OrderHeader.OrderTotal;
            DetailCart.OrderHeader.CouponCodeDiscount = discount;
            await db.SaveChangesAsync();
            DetailCart.OrderHeader.TransactionId = new Random().Next().ToString();
            DetailCart.OrderHeader.PaymentStatus = PaymentStatus.Successful;
            DetailCart.OrderHeader.Status = OrderStatus.Submitted;
            //    DetailCart.OrderHeader.Status = OrderStatus.Submitted;
            //Charge from payment gateway
            //var options = new ChargeCreateOptions
            //{
            //    Amount = Convert.ToInt32(DetailCart.OrderHeader.OrderTotal * 100),
            //    Currency = "inr",
            //    Description = "Order ID " + DetailCart.OrderHeader.Id.ToString(),
            //    Source = stripeToken
            //};
            //var service = new ChargeService();
            //Charge charge = await service.CreateAsync(options);
            //if (charge.BalanceTransactionId == null)
            //{
            //    DetailCart.OrderHeader.PaymentStatus = PaymentStatus.Failed;
            //}
            //else
            //{
            //    DetailCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            //}
            //if (charge.Status.ToLower() == "succeeded")
            //{
            //    DetailCart.OrderHeader.PaymentStatus = PaymentStatus.Successful;
            //    DetailCart.OrderHeader.Status = OrderStatus.Submitted;
            //}
            //else
            //{
            //    DetailCart.OrderHeader.PaymentStatus = PaymentStatus.Failed;
            //}
            await db.SaveChangesAsync();
            db.ShoppingCarts.RemoveRange(DetailCart.ShoppingCarts);
            HttpContext.Session.SetString(SD.CartCoupon, string.Empty);
            await db.SaveChangesAsync();
            return RedirectToAction("Confirm", "Orders", new { id = DetailCart.OrderHeader.Id });
        }

        
    }
}
