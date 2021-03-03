using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = @"Manager")]
    public class MenuItemsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment env;
        private readonly IMapper mapper;

        public MenuItemsController(ApplicationDbContext _db, IWebHostEnvironment _env,IMapper mapper)
        {
            db = _db;
            env = _env;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var menuItems = await db.MenuItems
                .Include(c => c.Catagory)
                .Include(c => c.SubCatagory)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
            return View(menuItems);
        }

        public IActionResult Create()
        {
            var MenuItemVM = new MenuItemViewModel()
            {
                Catagories = db.Catagories.ToList(),
                MenuItem = new Models.MenuItem()
            };
            return View(MenuItemVM);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuItem(MenuItemViewModel MenuVM)
        {
            MenuVM.MenuItem.SubCatagorId = Convert.ToInt32(Request.Form["SubCatagoryId"].ToString());
            if (ModelState.IsValid)
            {
                MenuVM.MenuItem.Slug = MenuVM.MenuItem.Name.ToSlug();
                
                await db.MenuItems.AddAsync(MenuVM.MenuItem);
                await db.SaveChangesAsync();
                var menuItemInDb = await db.MenuItems.FindAsync(MenuVM.MenuItem.Id);
                //ImageSaving Section
                string webRootPath = env.WebRootPath;
                if (MenuVM.Thumbnail != null)
                {
                    var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);
                    var fileExt = Path.GetExtension(MenuVM.Thumbnail.FileName);
                    var fileName = Guid.NewGuid().ToString() + "." + fileExt;
                    var finalPath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await MenuVM.Thumbnail.CopyToAsync(stream);
                    }

                    menuItemInDb.Image = @"\images\" + fileName;
                    await db.SaveChangesAsync();

                }
                else
                {
                    menuItemInDb.Image = SD.DefaultImage;
                }
                return RedirectToAction(nameof(Index));
            }
            MenuVM.Catagories = db.Catagories.ToList();
            MenuVM.MenuItem = new Models.MenuItem();
            return View(MenuVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var MenuItemVM = new MenuItemViewModel()
            {
                Catagories = db.Catagories.ToList(),

            };
            MenuItemVM.MenuItem = await db.MenuItems
                .Include(c => c.Catagory)
                .Include(m => m.SubCatagory)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            MenuItemVM.SubCatagories = await db.SubCatagories
                .Where(m => m.CatagoryId == MenuItemVM.MenuItem.Id)
                .ToListAsync();
            return View(MenuItemVM);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuItem(int? id, MenuItemViewModel MenuVM)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuVM.MenuItem.SubCatagorId = Convert.ToInt32(Request.Form["SubCatagoryId"].ToString());
            if (ModelState.IsValid)
            {

                //New ImageSaving Section
                string webRootPath = env.WebRootPath;
                var menuItemInDb = await db.MenuItems.FindAsync(MenuVM.MenuItem.Id);
                if (MenuVM.Thumbnail != null)
                {
                    var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);
                    var fileExt = Path.GetExtension(MenuVM.Thumbnail.FileName);
                    var fileName = Guid.NewGuid().ToString() + "." + fileExt;
                    var finalPath = Path.Combine(uploadPath, fileName);



                    //Delete the original file
                    var imagePath = Path.Combine(webRootPath, menuItemInDb.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await MenuVM.Thumbnail.CopyToAsync(stream);
                    }
                    menuItemInDb.Image = @"\images\" + fileName;
                }

                mapper.Map<MenuItem>(MenuVM.MenuItem);
                //menuItemInDb.Name = MenuVM.MenuItem.Name;
                //menuItemInDb.Price = MenuVM.MenuItem.Price;
                //menuItemInDb.CatagorId = MenuVM.MenuItem.CatagorId;
                //menuItemInDb.SubCatagorId = MenuVM.MenuItem.SubCatagorId;
                //menuItemInDb.Spicyness = MenuVM.MenuItem.Spicyness;
                //menuItemInDb.Description = MenuVM.MenuItem.Description;
                //menuItemInDb.IsFeatured = MenuVM.MenuItem.IsFeatured;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            MenuVM.Catagories = await db.Catagories.ToListAsync();
            MenuVM.SubCatagories = await db.SubCatagories.ToListAsync();
            return View(MenuVM);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var MenuItemVM = new MenuItemViewModel()
            {
                Catagories = db.Catagories.ToList(),
               
            };
            
            MenuItemVM.MenuItem = await db.MenuItems
                .Include(c => c.Catagory)
                .Include(m => m.SubCatagory)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            MenuItemVM.SubCatagories = await db.SubCatagories
                .Where(m => m.CatagoryId == MenuItemVM.MenuItem.Id)
                .ToListAsync();
            return View(MenuItemVM);
        }
        //[HttpGet("menuitems/{slug}")]
        public async Task<IActionResult> DetailsBySlug(string slug)
        {
            if (slug == null)
            {
                return NotFound();
            }

            var MenuItemVM = new MenuItemViewModel()
            {
                Catagories = db.Catagories.ToList(),

            };

            MenuItemVM.MenuItem = await db.MenuItems
                .Include(c => c.Catagory)
                .Include(m => m.SubCatagory)
                .SingleOrDefaultAsync(m => m.Slug == slug);
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            MenuItemVM.SubCatagories = await db.SubCatagories
                .Where(m => m.CatagoryId == MenuItemVM.MenuItem.Id)
                .ToListAsync();
            return View("Details",MenuItemVM);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var MenuItemVM = new MenuItemViewModel()
            {
                Catagories = db.Catagories.ToList(),

            };

            MenuItemVM.MenuItem = await db.MenuItems
                .Include(c => c.Catagory)
                .Include(m => m.SubCatagory)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            MenuItemVM.SubCatagories = await db.SubCatagories
                .Where(m => m.CatagoryId == MenuItemVM.MenuItem.Id)
                .ToListAsync();
            return View(MenuItemVM);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var menuItem = await db.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            db.MenuItems.Remove(menuItem);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
