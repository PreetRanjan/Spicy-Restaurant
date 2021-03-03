using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.ViewComponents
{
    public class UserNameViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            this._db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = (this.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var userName = await _db.AppUsers.FirstOrDefaultAsync(c => c.Id == userId);
            return View(userName);
        }
    }
}
