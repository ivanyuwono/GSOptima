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
    public class StockWatchListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public StockWatchListController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
                return ViewComponent("WatchList", new { currentUser = currentUser, page = page, sort = sort, nowsort = nowsort, filter=filter });

            }

            

            //var applicationDbContext = _context.StockWatchList.Where(s=>s.StockID==currentUser.Id).Include(s => s.Stock).Include(s => s.User);
            //return View(await applicationDbContext.ToListAsync());

            //var watchList = _context.StockWatchList.Where(s => s.ApplicationUserId == currentUser.Id).Include(s => s.Stock);

            //Paging<Stock> pagingModel = new Paging<Stock>();
            //pagingModel.data = watchList.Select(x => new Stock() { StockID = x.StockID, Name = x.Stock.Name }).ToList();
            //pagingModel.attribute.recordPerPage = 5;
            //pagingModel.attribute.recordsTotal = watchList.Count();
            //pagingModel.attribute.totalPage = Convert.ToInt32(Math.Ceiling((decimal)pagingModel.attribute.recordsTotal / pagingModel.attribute.recordPerPage));

            //if (page == null)
            //    page = 1;

            //pagingModel.DoPaging((int)page);

            var stockWatchList = new StockWatchListViewModel()
            {
                CurrentUser = currentUser
                //WatchList = pagingModel
            };
            return View(stockWatchList);

        }
        
        // GET: StockWatchLists/Create
        //public IActionResult Create()
        //{
        //    ViewData["StockID"] = new SelectList(_context.Stock, "StockID", "StockID");
        //    ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");

        //    //if (Request?.Headers != null &&
        //    //    Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    //{
        //    //    return ViewComponent("StockList", paging);

        //    //}

        //}
        
        public IActionResult SearchStock(int? page, string filter)
        {
            
            return ViewComponent("StockList", new { page = page, filter = filter });
        }
        public IActionResult Create(string search)
        {

            
            //if (Request?.Headers != null &&
            //    Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return ViewComponent("StockList", new { search = search });

            //}
            return PartialView("Create");
        }
        public IActionResult ShowModal(string header, string body)
        {


            //if (Request?.Headers != null &&
            //    Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return ViewComponent("StockList", new { search = search });

            //}
            return ViewComponent("Modal", new { header = header, body = body });
        }


        [HttpPost]
        public async Task<IActionResult> InquiryWatchlist([FromBody]string stockID)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            return Json(_context.StockWatchList.Any(m => m.StockID == stockID && m.ApplicationUserId == currentUser.Id));
        }

        [HttpPost]
        //public async Task<IActionResult> AddToStock([FromBody]string stockId, string action)
        public async Task<IActionResult> AddToStock([FromBody]ToogleWatchListViewModel setting)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["CurrentUser"] = currentUser;
            //*IVY 5 Nov 2019
            var stockId = setting.StockID.Trim();
            var action = setting.Action;

            stockId = stockId.ToUpper();

            if(!_context.Stock.Any(m => m.StockID == stockId))
            {
                return Json(stockId + " Is Not Valid");

            }
            else if(!_context.StockWatchList.Any(m => m.StockID == stockId && m.ApplicationUserId == currentUser.Id))  //if not exist in watchlist
            {
                if (action == "A")  //add to watchlist
                {
                    _context.StockWatchList.Add(new StockWatchList() { StockID = stockId, ApplicationUserId = currentUser.Id });
                    await _context.SaveChangesAsync();
                    return Json(null);
                }
                else
                    return Json(stockId + " is not in watchlist");
                //return RedirectToAction("Index", "StockWatchList");
                //return Json(Url.Action("Index", "StockWatchList"));

            }
            else
            {
                if(action == "R")
                {
                    var watchlist = _context.StockWatchList.SingleOrDefault(m => m.StockID == stockId && m.ApplicationUserId == currentUser.Id);
                    _context.StockWatchList.Remove(watchlist);
                    await _context.SaveChangesAsync();
                    return Json(null);
                }
                //ModelState.AddModelError(String.Empty, "Stock is already exist in watch list");
                else
                    return Json(stockId  + " is already in watchlist");
            }

        }


        // POST: StockWatchLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.



        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string stockId)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var watchList = await _context.StockWatchList.SingleOrDefaultAsync(s => s.ApplicationUserId == currentUser.Id && s.StockID == stockId);
            _context.StockWatchList.Remove(watchList);
            await _context.SaveChangesAsync();
            return ViewComponent("WatchList", new { currentUser = currentUser });

            //return RedirectToAction("Index");
        }

        //public async Task<IActionResult> Search(string search)
        //{
        //    ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
        //    ViewData["CurrentUser"] = currentUser;
        //    IQueryable<Stock> result = null;

        //    if (String.IsNullOrEmpty(search))
        //    {
        //        result = _context.Stock;
        //    }
        //    else
        //    {
        //        //var watchList = _context.StockWatchList.Where(s => s.ApplicationUserId == currentUser.Id).Include(s => s.Stock);
        //        result = _context.Stock.Where(s => s.Name.Contains(search) || s.StockID.Contains(search));
        //    }
        //    return View(result);
        //}



        //// GET: StockWatchLists/Delete/5
        //public async Task<IActionResult> Delete(string? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var stockWatchList = await _context.StockWatchList.SingleOrDefaultAsync(m => m.ApplicationUserId == id);
        //    if (stockWatchList == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(stockWatchList);
        //}

        //// POST: StockWatchLists/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var stockWatchList = await _context.StockWatchList.SingleOrDefaultAsync(m => m.ApplicationUserId == id);
        //    _context.StockWatchList.Remove(stockWatchList);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}
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
