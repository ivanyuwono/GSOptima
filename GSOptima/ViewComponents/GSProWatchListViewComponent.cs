using GSOptima.Data;
using GSOptima.Models;
using GSOptima.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSOptima.ViewComponents
{
    public class GSProWatchListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser _userID;

        
        private Decimal GetAverageVolume(StockPrice stock, int days, out string lastTrendBar)
        {
            //*Fungsi ini menghitung rata2 volume selama (days) terakhir sekaligus mencari last trend bar yang muncul
            var avgdata = _context.StockPrice.Where(m => m.Date <= stock.Date && m.Date >= stock.Date.AddDays(-days) && m.StockID == stock.StockID).OrderByDescending(m => m.Date).AsNoTracking();
            long temp = 0; int count = 0;
            var lastTrend = "";

            foreach (var item in avgdata)
            {
                temp += item.Volume;
                count++;

                if (lastTrend == "")
                {
                    if (item.TrendHigh > 0)
                    {
                        lastTrend = "H;" + item.TrendHigh;

                        if (item.TrendLow > 0)
                        {
                            lastTrend += ";L" + item.TrendLow;
                        }
                    }
                    else if (item.TrendLow > 0)
                        lastTrend = "L;" + item.TrendLow;
                }
            }
            lastTrendBar = lastTrend;

            var avg = (Decimal)temp / count;
            //var avg = (Decimal) avgdata.Average(s => s.Volume);
            return avg;
        }


        public GSProWatchListViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

           
        }

        // GET: StockWatchLists
        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser currentUser, int? page, string filter, string sort, string nowsort)
        {
            _userID = currentUser;
            //var watchList = await _context.StockWatchList.Where(s => s.ApplicationUserId == _userID.Id).Include(s => s.Stock).ThenInclude(p=>p.Prices).ToListAsync();

            var watchList = await _context.GSProAdminWatchList.Include(s => s.Stock).ThenInclude(p => p.Prices).ToListAsync();
            var model = watchList.Select(x => 
            
            {
                var lastPrice = x.Stock.Prices.OrderBy(m=>m.Date).LastOrDefault();
                var lastTrendBar = "";
                GetAverageVolume(lastPrice, 50, out lastTrendBar);
                return new GSProWatchListViewModel()
                {


                    StockID = x.StockID,
                    Open = lastPrice == null ? 0 : lastPrice.Open,
                    Close = lastPrice == null ? 0 : lastPrice.Close,
                    High = lastPrice == null ? 0 : lastPrice.High,
                    Low = lastPrice == null ? 0 : lastPrice.Low,
                    Volume = lastPrice == null ? 0 : lastPrice.Volume,
                    TradingPlan = Screening.DetermineTradingPlan(lastPrice),
                    Trend = Screening.DetermineTrend(lastPrice),
                    RiskProfile = Screening.DetermineRiskProfile(lastPrice),
                    BigWave = Screening.DetermineBigWave(lastPrice),
                    Support = lastPrice.Support,
                    Resistance = lastPrice.Resistance,
                    CloseToSupport = (lastPrice.Close - lastPrice.Support) / lastPrice.Support * 100,
                    CloseToResistance = (lastPrice.Close - lastPrice.Resistance) / lastPrice.Resistance * 100,
                    GSLineDirection = lastPrice.GSLineDirection,
                    NormalRange = Screening.DetermineNormalRange(lastPrice),
                    BuyLimit = Screening.DetermineBuyLimit(lastPrice),
                    LastTrendBar = lastTrendBar,
                    Target1 = x.Target1 == null ? 0: x.Target1,
                    Target2 = x.Target2 == null ? 0: x.Target2



                };
            }
            ).ToList();

            //var watchList = _context.StockWatchList.Where(s => s.ApplicationUserId == currentUser.Id).Include(s => s.Stock);


            if (string.IsNullOrEmpty(sort))
                sort = "StockID";

            var current_sort_field = sort.Replace("_DESC", "");

            if (!string.IsNullOrEmpty(nowsort))
            {


                if (nowsort == current_sort_field)
                {
                    if (sort.Contains("_DESC"))
                        sort = sort.Replace("_DESC", "");
                    else
                        sort = sort + "_DESC";
                }
                else
                    sort = nowsort;
            }
            

            Paging<GSProWatchListViewModel> pagingModel = new Paging<GSProWatchListViewModel>();
            pagingModel.data = (IQueryable<GSProWatchListViewModel>)model.AsQueryable();
            pagingModel.attribute.recordPerPage = 25;
            pagingModel.attribute.recordsTotal = watchList.Count();
            pagingModel.attribute.totalPage = Convert.ToInt32(Math.Ceiling((decimal)pagingModel.attribute.recordsTotal / pagingModel.attribute.recordPerPage));
            pagingModel.attribute.url = @"/GSProWatchList/Index";
            pagingModel.attribute.divName = "watchList";
            pagingModel.attribute.sorting = sort;
            pagingModel.attribute.filter = filter;
            pagingModel.OrderBy(sort);

            if (page == null)
                page = 1;

            
            pagingModel.DoPaging((int)page);

            //var stockWatchList = new StockWatchListViewModel()
            //{
            //    CurrentUser = currentUser,
            //    ModifiedStockId = "",
            //    WatchList = pagingModel
            //};
            return View(pagingModel);


            //return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IViewComponentResult> DeleteAsync([FromBody] string stockId)
        {
            var watchList = await _context.StockWatchList.SingleOrDefaultAsync(s => s.ApplicationUserId == _userID.Id && s.StockID == stockId);
            _context.StockWatchList.Remove(watchList);
            await _context.SaveChangesAsync();
            return View();
        }
    }
}
