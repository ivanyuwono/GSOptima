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

namespace GSOptima.Controllers
{
    [Authorize]
    //[SessionTimeout]
    public class GSProWatchListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public GSProWatchListController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StockWatchLists
        public async Task<IActionResult> Index(string filter, int? page, string sort, string nowsort)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["CurrentUser"] = currentUser;

            if (Request?.Headers != null &&
                Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return ViewComponent("GSProWatchList", new { currentUser = currentUser, page = page, sort = sort, nowsort = nowsort, filter=filter });

            }

            
            //var stockWatchList = new StockWatchListViewModel()
            //{
            //    CurrentUser = currentUser
            //    //WatchList = pagingModel
            //};
            //return View(stockWatchList);
            return View();
        }
        
      
        public IActionResult SearchStock(int? page, string filter)
        {
            
            return ViewComponent("StockList", new { page = page, filter = filter });
        }
        public IActionResult Create(string search)
        {

            return PartialView("Create");
        }
        public IActionResult ShowModal(string header, string body)
        {

            return ViewComponent("Modal", new { header = header, body = body });
        }


        [HttpPost]
        public async Task<IActionResult> InquiryWatchlist([FromBody]string stockID)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            return Json(_context.StockWatchList.Any(m => m.StockID == stockID && m.ApplicationUserId == currentUser.Id));
        }
               

       
        [HttpGet]
        public async Task<IActionResult> DisplayChart(string stockId, int numberOfDays)
        {


            //if (Request?.Headers != null &&
            //  Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return ViewComponent("StockChart", new { parameter = new ChartViewModel() { StockID = stockId, NumberOfDays = numberOfDays } });

            //}
            //return View();

            //return ViewComponent("StockChart", new { parameter = new ChartViewModel() { StockID = stockId, NumberOfDays = numberOfDays } });
            return PartialView("_StockChart", new  ChartViewModel() { StockID = stockId, NumberOfDays = numberOfDays } );

            //return RedirectToAction("Index");
        }

        private bool StockWatchListExists(string id)
        {
            return _context.StockWatchList.Any(e => e.ApplicationUserId == id);
        }
    }
}
