using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.DTOs;
using Spice.Persistance;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.ViewComponents
{
    public class FeaturedViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly MenuItemRepository repo;
        public FeaturedViewComponent(ApplicationDbContext db,MenuItemRepository repo)
        {
            this._db = db;
            this.repo = repo;
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await repo.GetFeaturedItems();
            return View(items);
        }
    }
}
