using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GSOptima.Data;
using GSOptima.Models;
using Microsoft.AspNetCore.Identity;
using GSOptima.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace GSOptima.Controllers
{
    
    public class GSProAdminWatchListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public GSProAdminWatchListController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GSProStockWatchLists
        public async Task<IActionResult> Index(string filter, int? page, string sort, string nowsort)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["CurrentUser"] = currentUser;

            if (Request?.Headers != null &&
                Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return ViewComponent("GSProAdminWatchList", new { currentUser = currentUser, page = page, sort = sort, nowsort = nowsort, filter = filter });

            }

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GSProAdminWatchListViewModel model)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["CurrentUser"] = currentUser;

            var stockId = model.StockID.Trim().ToUpper();

            if (!_context.Stock.Any(m => m.StockID == stockId))
            {
                return Json(stockId + " Is Not Valid");
            }
            else if (_context.GSProAdminWatchList.Any(m => m.StockID == stockId))
            {
                return Json(stockId + " Is Already Exist On Gs Pro Watchlist");
            }
            GSProAdminWatchList ent = new GSProAdminWatchList();
            ent.StockID = model.StockID.Trim().ToUpper();
            ent.Target1 = model.Target1;
            ent.Target2 = model.Target2;

            _context.GSProAdminWatchList.Add(ent);
            await _context.SaveChangesAsync();
            return Json(null);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string stockId)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["CurrentUser"] = currentUser;

            stockId = stockId.Trim().ToUpper();

            if (!_context.Stock.Any(m => m.StockID == stockId))
            {
                return Json(stockId + " Is Not Valid");
            }
            else if (!_context.GSProAdminWatchList.Any(m => m.StockID == stockId))
            {
                return Json(stockId + " Is Not Exist On Gs Pro Watchlist");
            }
 
            var ent = _context.GSProAdminWatchList.SingleOrDefault(s => s.StockID == stockId);
            _context.GSProAdminWatchList.Remove(ent);
            await _context.SaveChangesAsync();

            return ViewComponent("GSProAdminWatchList", new { currentUser = currentUser });
        }
    }
}
