using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext db;
        public CouponsController(ApplicationDbContext _db)
        {
            db = _db;
        }
        //GET : Get All Catagories
        public async Task<IActionResult> Index()
        {
            return View(await db.Coupons.ToListAsync());
        }
        //GET:Create
        public IActionResult Create()
        {
            return View();
        }
        //POST:Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var coupon = new Coupon
                {
                    Name = vm.Name,
                    IsActive = vm.IsActive,
                    MinimumAmount = vm.MinimumAmount,
                    CouponType = vm.CouponType,
                    Discount = vm.Discount
                };
                byte[] b1 = null;
                using (var fs1=vm.Picture.OpenReadStream())
                {
                    
                    using (var ms = new MemoryStream())
                    {
                        await fs1.CopyToAsync(ms);
                        b1 = ms.ToArray();
                    }
                }
                coupon.Picture = b1;
                await db.Coupons.AddAsync(coupon);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        //GET:Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await db.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            var vm = new CouponViewModel
            {
                IsActive = coupon.IsActive,
                MinimumAmount=coupon.MinimumAmount,
                Name=coupon.Name,
                CouponType=coupon.CouponType,
                Discount=coupon.Discount
            };
            return View(vm);
        }

        //POST:Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,CouponViewModel vm)
        {
            if (id==null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var coupon = await db.Coupons.SingleOrDefaultAsync(c => c.Id == id);
                coupon.IsActive = vm.IsActive;
                coupon.MinimumAmount = vm.MinimumAmount;
                coupon.CouponType = vm.CouponType;
                coupon.Discount = vm.Discount;

                byte[] b1 = null;
                using (var fs1 = vm.Picture.OpenReadStream())
                {

                    using (var ms = new MemoryStream())
                    {
                        await fs1.CopyToAsync(ms);
                        b1 = ms.ToArray();
                    }
                }
                coupon.Picture = b1;
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        //GET:Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var c = await db.Coupons.FindAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            return View(c);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var c = await db.Coupons.FindAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            db.Coupons.Remove(c);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
