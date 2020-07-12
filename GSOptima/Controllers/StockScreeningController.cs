
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using GSOptima.Data;
using GSOptima.Models;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Authorization;



namespace GSOptima.Controllers
{
    [Authorize]
    //[SessionTimeout]
    public class StockScreeningController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StockScreeningController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string filter, int time, int? page, string sort, string nowsort)
        {
            if (Request?.Headers != null &&
              Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return ViewComponent("StockScreening", new { filter = filter, time=time, page= page, sort = sort, nowsort = nowsort });

            }
            return View();
        }

        //public async Task<IActionResult> TradingBuy()
        //{

        //    //_context.Stock.Include(s=>s.Prices).
        //    //var price = _context.StockPrice.OrderBy(m => m.Date).Last();
        //    //price.Close
        //    //_userID = currentUser;
        //    var watchList = await _context.Stock.Include(p => p.Prices).ToListAsync();


        //    var model = watchList.Select(x =>

        //    {
        //        var lastPrice = x.Prices.OrderBy(m => m.Date).LastOrDefault();
        //        return lastPrice;

        //        //return new WatchList()
        //        //{


        //        //    StockID = x.StockID,
        //        //    Open = lastPrice == null ? 0 : lastPrice.Open,
        //        //    Close = lastPrice == null ? 0 : lastPrice.Close,
        //        //    High = lastPrice == null ? 0 : lastPrice.High,
        //        //    Low = lastPrice == null ? 0 : lastPrice.Low,
        //        //    Volume = lastPrice == null ? 0 : lastPrice.Volume
        //        //};
        //    }
        //    ).ToList();
        //    return View(model);
        //}

    }
}
