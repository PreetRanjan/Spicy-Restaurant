using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.DTOs;
using Spice.Models.ViewModels;
using Spice.Persistance;
using Spice.Utility;
using Spice.Utility.Filters;
using X.PagedList;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;
        private readonly MenuItemRepository menu;
        

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _db,
            MenuItemRepository menu)
        {
            _logger = logger;
            db = _db;
            this.menu = menu;
            
        }
        public string MakeSlugs()
        {
            var menuItems = db.MenuItems.ToList();
            foreach (var item in menuItems)
            {
                item.Slug = item.Name.ToSlug();
            }
            db.SaveChanges();
            return "Slugs are Updated";
        }
        public async Task<IActionResult> Index()
        {
            var vm = await menu.GetItemsAsync();
            if (User.Identity.IsAuthenticated)
            {
                var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
                var cartCount = db.ShoppingCarts.Where(c => c.AppUserId == userId).Count();
                HttpContext.Session.SetInt32(SD.CartCount, cartCount);
            }
            return View(vm);
        }

        [AllowAnonymous]
        [HttpGet("/menuitems/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (slug==null)
            {
                return NotFound();
            }
            var mappingConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile(typeof(MappingProfile)));
            var menuItemdt = await db.MenuItems
                .Include(c => c.Catagory)
                .Include(c => c.SubCatagory)
                .ProjectTo<MenuItemDTO>(mappingConfig)
                .FirstOrDefaultAsync(c => c.Slug == slug);
            var menuItem = await db.MenuItems
                .Include(c => c.Catagory)
                .Include(c => c.SubCatagory).FirstOrDefaultAsync(c => c.Slug == slug);
            if (menuItem == null)
            {
                return NotFound();
            }
            var cart = new ShoppingCart
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };
            return View(cart);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind("MenuItemId,Count")] ShoppingCartObject cartObj)
        {
            
            if (ModelState.IsValid)
            {
                var itemsCount = Convert.ToInt32(Request.Form["quantity"].ToString());
                var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
                cartObj.Count = itemsCount;
                ShoppingCart cart = await db.ShoppingCarts.Where(c => c.AppUserId == userId && c.MenuItemId == cartObj.MenuItemId).FirstOrDefaultAsync();
                if (cart == null)
                {
                    var newCart = new ShoppingCart
                    {
                        Id=0,
                        AppUserId = userId,
                        Count = itemsCount
                    };

                    await db.ShoppingCarts.AddAsync(newCart);
                }
                else
                {
                    cart.Count += itemsCount;
                }
                await db.SaveChangesAsync();
                var itemscount = db.ShoppingCarts.Where(c => c.AppUserId == userId).ToList().Count();
                HttpContext.Session.SetInt32(SD.CartCount, itemscount);
                //return RedirectToAction(nameof(Index));
            }
            var menuItem = await db.MenuItems
                .Include(c => c.Catagory)
                .Include(c => c.SubCatagory)
                .FirstOrDefaultAsync(c => c.Id == cartObj.MenuItemId);
            var cartobj = new ShoppingCart
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };
            return View(cartobj);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddtoCart([Bind("MenuItemId,Count")] ShoppingCartObject cartObj)
        {

            if (ModelState.IsValid)
            {
                //var itemsCount = Convert.ToInt32(Request.Form["quantity"].ToString());
                var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
                //cartObj.Count = itemsCount;
                ShoppingCart cart = await db.ShoppingCarts.Where(c => c.AppUserId == userId && c.MenuItemId == cartObj.MenuItemId).FirstOrDefaultAsync();
                if (cart == null)
                {
                    var newCart = new ShoppingCart
                    {
                        Id = 0,
                        AppUserId = userId,
                        Count = cartObj.Count,
                        MenuItemId = cartObj.MenuItemId
                    };

                    await db.ShoppingCarts.AddAsync(newCart);
                }
                else
                {
                    cart.Count += cartObj.Count;
                }
                await db.SaveChangesAsync();
                var itemscount = db.ShoppingCarts.Where(c => c.AppUserId == userId).ToList().Count();
                HttpContext.Session.SetInt32(SD.CartCount, itemscount);
                return Json(cartObj);
            }
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult HIndex()
        {
            return View("_Host");
        }

        public async Task<IActionResult> HandleException(string msg)
        {
            return View(msg);
        }
        //[TypeFilter(typeof(HandleException))]
        //public IActionResult Test()
        //{
        //    throw new Exception("Heeeyaaaa...");
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
