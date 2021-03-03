using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CatagoryController : Controller
    {
        private readonly ApplicationDbContext db;

        public CatagoryController(ApplicationDbContext _db)
        {
            db = _db;
        }
        //GET : Get All Catagories
        public async Task<IActionResult> Index()
        {
            return View(await db.Catagories.ToListAsync());
        }
        //GET:Create
        public IActionResult Create()
        {
            return View();
        }
        //POST:Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Catagory category)
        {
            if (ModelState.IsValid)
            {
                await db.Catagories.AddAsync(category);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //GET:Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await db.Catagories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //POST:Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Catagory category)
        {
            if (ModelState.IsValid)
            {
                db.Update(category);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //GET:Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await db.Catagories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest();
                }
                var category = await db.Catagories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                db.Catagories.Remove(category);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                string exce = @"The category you are trying to delete contains
                                references in subcategory and menuitems.
                                If you wish to delete the same remove the all items related to 
                                    this category.";
                return RedirectToActionPermanent("HandleException", "Home", new { area="Customer",msg = exce });
            }
            

        }
    }
}
