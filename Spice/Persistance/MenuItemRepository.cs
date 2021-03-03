using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Spice.Data;
using Spice.Models;
using Spice.Models.DTOs;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Threading.Tasks;
using AutoMapper;
using Spice.Utility;
using AutoMapper.QueryableExtensions;
using X.PagedList;

namespace Spice.Persistance
{
    public class MenuItemRepository
    {
        private ApplicationDbContext _db;
        private string conn;
        private MapperConfiguration mappingConfig;
        public MenuItemRepository(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            conn = config.GetConnectionString("DefaultConnection");
            mappingConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile(typeof(MappingProfile)));
        }

        public async Task<IndexViewModel> GetItemsAsync()
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile(typeof(MappingProfile)));
            var vm = new IndexViewModel();
            vm.MenuItems = await _db.MenuItems
                .Include(c => c.Catagory)
                .Include(c => c.SubCatagory)
                .ProjectTo<MenuItemDTO>(mappingConfig).ToListAsync();

            vm.Catagories = await vm.MenuItems
                .GroupBy(c=>c.CatagoryName)
                .Select(c => new CatagoryDTO { Name = c.Key })
                .ToListAsync();
            vm.Coupons = await _db.Coupons
                .Where(c => c.IsActive == true)
                .ProjectTo<CouponDTO>(mappingConfig)
                .ToListAsync();
            return vm;
        }
        public async Task<List<MenuItemDTO>> GetFeaturedItems()
        {
           
            var items = await _db.MenuItems
                .Include(c => c.Catagory)
                .Include(c => c.SubCatagory)
                .Where(c=>c.IsFeatured == true)
                .ProjectTo<MenuItemDTO>(mappingConfig).ToListAsync();
            return items;
        }
        public async Task DeleteAsync(int? id)
        {
            var item = await _db.MenuItems.FindAsync(id);
            _db.MenuItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
