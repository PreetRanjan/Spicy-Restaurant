using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using X.PagedList;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var orderDetail = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeaders
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(o => o.Id == id && o.AppUserId == userId),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderHeaderId == id).ToListAsync()
            };
            return View(orderDetail);
        }
        [Authorize]
        public async Task<IActionResult> OrderHistory(int page = 1)
        {

            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var orders = _db.OrderHeaders;
            var data = orders.Include(c => c.AppUser)
            .Where(c => c.AppUserId == userId)
            .OrderByDescending(c => c.PickUpTime);
            var ordersdt = await data.ToPagedListAsync(page, 6);
            ViewBag.OrderAction = "OrderHistory";
            return base.View(ordersdt);

        }
        [Authorize]
        public async Task<IActionResult> OrderSearch(int page = 1, int? orderId = null)
        {
            if (orderId == null)
            {
                return RedirectToAction(nameof(OrderHistory));
            }
            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var data = await _db.OrderHeaders.Include(c => c.AppUser)
            .Where(c => c.AppUserId == userId && c.Id == orderId)
            .OrderByDescending(c => c.PickUpTime).ToPagedListAsync(page, 6);
            ViewBag.OrderAction = "OrderSearch";
            return base.View("OrderHistory", data);

        }
        [Authorize]
        public async Task<IActionResult> OpenOrderHistory(int page = 1)
        {

            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var orders = _db.OrderHeaders;
            var data = await orders.Include(c => c.AppUser)
            .Where(c => c.AppUserId == userId && c.Status != OrderStatus.Ready && c.Status != OrderStatus.PickedUp)
            .OrderByDescending(c => c.PickUpTime)
            .ToPagedListAsync(page, 6);
            ViewBag.OrderAction = "OpenOrders";
            return base.View("OrderHistory", data);

        }
        [Authorize]
        public async Task<IActionResult> OrderPickup(int? page = 1)
        {

            var list = new List<OrderDetailsViewModel>();
            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var ordersHeaders = await _db.OrderHeaders
                .Where(c => c.PaymentStatus == PaymentStatus.Successful && c.Status == OrderStatus.Ready)
                .OrderByDescending(c => c.PickUpTime)
                .Take(20)
                .ToListAsync();
            foreach (var item in ordersHeaders)
            {
                var model = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(m => m.OrderHeaderId == item.Id).ToListAsync()
                };
                list.Add(model);
            }
            return View(list);

        }
        [Authorize]
        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeaders.Include(el => el.AppUser).FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetails = await _db.OrderDetails.Where(m => m.OrderHeaderId == Id).ToListAsync()
            };
            //orderDetailsViewModel.OrderHeader.ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);

            return PartialView("_IndividualOrderPartial", orderDetailsViewModel);
        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> ManageOrders()
        {
            var list = new List<OrderDetailsViewModel>();
            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var ordersHeaders = await _db.OrderHeaders
                .Where(c => c.PaymentStatus == PaymentStatus.Successful && c.Status != OrderStatus.PickedUp)
                .OrderByDescending(c => c.PickUpTime)
                .Take(20)
                .ToListAsync();
            foreach (var item in ordersHeaders)
            {
                var model = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(m => m.OrderHeaderId == item.Id).ToListAsync()
                };
                list.Add(model);
            }
            return View(list);

        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> StartCooking(int id)
        {
            var order = await _db.OrderHeaders.FindAsync(id);
            if (order == null)
            {
                return Json(false);
            }
            order.Status = OrderStatus.BeingPrepared;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrders));
        }
        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> BeingPrepared(int id)
        {
            var order = await _db.OrderHeaders.FindAsync(id);
            if (order == null)
            {
                return Json(false);
            }
            order.Status = OrderStatus.Ready;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrders));
        }
        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderComplete(int id)
        {
            var order = await _db.OrderHeaders.FindAsync(id);
            if (order == null)
            {
                return Json(false);
            }
            order.Status = OrderStatus.PickedUp;
            await _db.SaveChangesAsync();
            return Json(id);
        }
    }
}
