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
    public class WatchListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser _userID;

        //private string DetermineTradingPlan(StockPrice x)
        //{

        //    if (x.High > x.Resistance && x.High <= (decimal)1.03 * x.Resistance)   //If Highest Price > Resist (0% < X <= +3%) | X = Closing price
        //    {
        //        return "Buy";
        //    }
        //    else if (x.Close > (decimal)1.03 * x.Resistance)  //If Closing Price > Resist (X > +3%) | X = Closing price
        //    {
        //        return "Hold";
        //    }
        //    else if (x.Close <= x.Resistance && x.Close >= (decimal)0.97 * x.Resistance)  //If Closing Price < Resist (-3% <= X <= 0% ) | X = Closing price
        //    {

        //        return "Watch";
        //    }
        //    else if (x.Low < x.Support && x.Low >= (decimal)0.925 * x.Support)  //If Lowest Price < Support ( -7.5% <= X < 0%)
        //    {

        //        return "Sell";
        //    }
        //    else
        //        return "";
        //}
        //private string DetermineRiskProfile(StockPrice x)
        //{
        //    if (x.Support != null && x.Resistance != null)
        //    {
        //        //if (x.Close >= x.Support)  //bila closing di atas support
        //        //{
        //        var closeToS = (decimal)(x.Close - x.Support) / x.Support * 100;
        //        var closeToR = (decimal)(x.Close - x.Resistance) / x.Resistance * 100;
        //        var risk = (Math.Abs((decimal)closeToS) + Math.Abs((decimal)closeToR)) / 2;
        //        if (risk >= 0 && risk <= (decimal)2.0)   //0% <= X <= 2%
        //            return "Low";
        //        else if (risk > 2 && risk <= (decimal)3.5)    //2% < X <= 3.5%
        //        {
        //            return "Medium";
        //        }
        //        else
        //            return "High";    //  3.5% < X
        //        //}
        //        //else     //bila tidak closing di atas support
        //        //{
        //        //   return "";
        //        //}
        //    }
        //    else
        //        return "";

        //}
        //private string DetermineTrend(StockPrice x)
        //{

        //    if (x.MA20 == null || x.MA60 == null)
        //        return "N/A";
        //    else
        //    {
        //        if (x.Close >= x.MA20 && x.MA20 >= x.MA60)
        //            return "1"; //blue
        //        else if (x.MA20 >= x.Close && x.Close >= x.MA60)
        //            return "2"; //green
        //        else if (x.MA20 >= x.MA60 && x.MA60 >= x.Close)
        //            return "5"; //indian red
        //        else if (x.MA60 >= x.MA20 && x.MA20 >= x.Close)
        //            return "6";  //dark red
        //        else if (x.MA60 >= x.Close && x.Close >= x.MA20)
        //            return "4"; //white
        //        else if (x.Close >= x.MA60 && x.MA60 >= x.MA20)
        //            return "3"; //green
        //        else
        //            return "N/A";
        //    }


        //}
        //private string DetermineNormalRange(StockPrice x)
        //{

        //    if (x.Close > (decimal)1.025 * x.BBUpper)
        //        return "U";
        //    else if (x.Close < (decimal)0.975 * x.BBLower)
        //        return "D";
        //    else
        //        return "N";
        //}

        //private bool DetermineBigWave(StockPrice x)
        //{
        //    if (x.BigWave > x.AverageBigWave)
        //    {
        //        return true;
        //    }
        //    else
        //        return false;
        //}
        //private Decimal? DetermineBuyLimit(StockPrice x)
        //{

        //    var tradingplan = DetermineTradingPlan(x);

        //    if (x.Resistance != null)
        //    {
        //        if (tradingplan == "Buy" || tradingplan == "Hold" || tradingplan == "Watch")
        //            return Math.Round(((decimal)1.03 * (decimal)x.Resistance));   //3% Up from Resist
        //        else
        //            return null;
        //    }
        //    else
        //        return null;
        //}
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


        public WatchListViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

           
        }

        // GET: StockWatchLists
        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser currentUser, int? page, string filter, string sort, string nowsort)
        {
            _userID = currentUser;
            var watchList = await _context.StockWatchList.Where(s => s.ApplicationUserId == _userID.Id).Include(s => s.Stock).ThenInclude(p=>p.Prices).ToListAsync();


            var model = watchList.Select(x => 
            
            {
                var lastPrice = x.Stock.Prices.OrderBy(m=>m.Date).LastOrDefault();
                var lastTrendBar = "";
                GetAverageVolume(lastPrice, 50, out lastTrendBar);
                return new WatchList()
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
                    LastTrendBar = lastTrendBar


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
            

            Paging<WatchList> pagingModel = new Paging<WatchList>();
            pagingModel.data = (IQueryable<WatchList>)model.AsQueryable();
            pagingModel.attribute.recordPerPage = 25;
            pagingModel.attribute.recordsTotal = watchList.Count();
            pagingModel.attribute.totalPage = Convert.ToInt32(Math.Ceiling((decimal)pagingModel.attribute.recordsTotal / pagingModel.attribute.recordPerPage));
            pagingModel.attribute.url = @"/StockWatchList/Index";
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
