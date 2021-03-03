using System;
using System.Collections.Generic;
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
    public class SubCatagoryController : Controller
    {
        private readonly ApplicationDbContext db;
        public SubCatagoryController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public async Task<IActionResult> Index()
        {
            var subCatagories = await db.SubCatagories.Include(c => c.Catagory).ToListAsync();
            return View(subCatagories);
        }

        //GET-Create
        public async Task<IActionResult> Create()
        {
            SubCatagoryViewModel model = new SubCatagoryViewModel()
            {
                CatagoryList = await db.Catagories.ToListAsync(),
                SubCatagory = new Models.SubCatagory(),
                SubCatagoryList = await db.SubCatagories.OrderBy(p => p.Name).Select(c => c.Name).Distinct().ToListAsync()
            };
            return View(model);
        }
        public async Task<IActionResult> GetSubCatagory(int id)
        {
            var list = new List<SubCatagory>();
            list = await db.SubCatagories.Where(m => m.CatagoryId == id).ToListAsync();
            return Json(list);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCatagoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doessubCatagoryExists = db.SubCatagories.Any(m => m.Name == model.SubCatagory.Name);
                if (doessubCatagoryExists)
                {
                    //Error
                }
                else
                {
                    await db.SubCatagories.AddAsync(model.SubCatagory);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            model.CatagoryList = await db.Catagories.ToListAsync();
            model.SubCatagoryList = await db.SubCatagories.OrderBy(m => m.Name).Select(c=>c.Name).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subcategory = await db.SubCatagories.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }
            var model = new SubCatagoryViewModel()
            {
                CatagoryList = await db.Catagories.ToListAsync(),
                SubCatagory = subcategory,
                SubCatagoryList = await db.SubCatagories.OrderBy(p => p.Name).Select(c => c.Name).Distinct().ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SubCatagoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doessubCatagoryExists = db.SubCatagories.Any(m => m.Name == model.SubCatagory.Name
                && m.Catagory.Id == model.SubCatagory.Id
                );
                if (doessubCatagoryExists)
                {
                    //Error
                }
                else
                {
                    db.SubCatagories.Update(model.SubCatagory);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            model.CatagoryList = await db.Catagories.ToListAsync();
            model.SubCatagoryList = await db.SubCatagories.OrderBy(m => m.Name).Select(c => c.Name).ToListAsync();
            return View(model);
        }


        //GET:Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await db.SubCatagories.Include(c => c.Catagory).SingleOrDefaultAsync(c => c.Id == id);
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
            if (id == null)
            {
                return BadRequest();
            }
            
            var category = await db.SubCatagories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            db.SubCatagories.Remove(category);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
