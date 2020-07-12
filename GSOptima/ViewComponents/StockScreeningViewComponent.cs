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
    public class StockScreeningViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        private ApplicationUser _userID;

        public StockScreeningViewComponent(ApplicationDbContext context)
        {
            _context = context;

        }
        

        private StockScreeningViewModel BuildStockScreeningModel(StockPrice x, string lastTrendBar)
        {
            return new StockScreeningViewModel()
            {

                Date = x.Date,
                StockID = x.StockID,
                Open = x.Open,
                Close = x.Close,
                High = x.High,
                Low = x.Low,
                Support = x.Support,
                Resistance = x.Resistance,
                CloseToSupport = (x.Close - x.Support) / x.Support * 100,
                CloseToResistance = (x.Close - x.Resistance) / x.Resistance * 100,
                GSLineDirection = x.GSLineDirection,
                Trend = Screening.DetermineTrend(x),
                NormalRange = Screening.DetermineNormalRange(x),
                BigWave = Screening.DetermineBigWave(x),
                RiskProfile = Screening.DetermineRiskProfile(x),
                TradingPlan = Screening.DetermineTradingPlan(x),
                BuyLimit = Screening.DetermineBuyLimit(x),
                LastTrendBar = lastTrendBar
            };

        }
        private Decimal GetAverageVolume(StockPrice stock, int days, out string lastTrendBar)
        {
            //*Fungsi ini menghitung rata2 volume selama (days) terakhir sekaligus mencari last trend bar yang muncul
            var avgdata = _context.StockPrice.Where(m => m.Date <= stock.Date && m.Date >= stock.Date.AddDays(-days) && m.StockID == stock.StockID).OrderByDescending(m=>m.Date).AsNoTracking();
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

            var avg = (Decimal) temp / count;
            //var avg = (Decimal) avgdata.Average(s => s.Volume);
            return avg;
        }

        private List<StockScreeningViewModel> FilterAverageVolume(IEnumerable<StockPrice> raw, int days, long average)
        {
            List<StockScreeningViewModel> model = new List<StockScreeningViewModel>();

            foreach (var data in raw)
            {
                var lastTrendBar = "";
                if (GetAverageVolume(data, days, out lastTrendBar) > average)
                    model.Add(BuildStockScreeningModel(data, lastTrendBar));
            }
            model = model.OrderBy(m=>m.RiskProfile).ThenBy(m => m.StockID).ToList();
            return model;
        }
        // GET: StockWatchLists
        public async Task<IViewComponentResult> InvokeAsync(int? page, string filter, string sort, string nowsort)
        {

            var split = filter.Split(';');
            var plan = split[0];
            var days = split[1];
            var min = split[2];
            var max = split[3];
            var volume = split[4];
            

            //if (split[1] != "")
            //    periode = Convert.ToInt16(split[1]);

            //Bila setting filter tidak diisi maka akan didefault value
            int periode = 0;
            if (!int.TryParse(days, out periode))
            {
                periode = 1;
            }

            long minPrice = 0;
            if(!long.TryParse(min, out minPrice))
            {
                minPrice = 51;
            }
            long maxPrice = 0;
            if (!long.TryParse(max, out maxPrice))
            {
                maxPrice = 100000;
            }
            long avgVolume = 0;
            if (!long.TryParse(volume, out avgVolume))
            {
                avgVolume = 500000;
            }

            //IEnumerable<StockPrice> screen2 = await _context.StockPrice.OrderBy(m => new { m.StockID, m.Date }).GroupBy(m => m.StockID).LastOrDefaultAsync();
            //IEnumerable<StockPrice> screen = await _context.StockPrice.GroupBy(m => m.StockID).Select(g=>g.OrderByDescending(p=>p.Date).FirstOrDefault()).ToListAsync();

            IEnumerable<StockPrice> data = await _context.StockPrice.GroupBy(m => m.StockID).SelectMany(g => g.OrderByDescending(p => p.Date).Take(periode)).ToListAsync();
            var screen = data.Where(m => m.Close >= minPrice && m.Close <= maxPrice);
            
            List<StockScreeningViewModel> model = new List<StockScreeningViewModel>();
            
            if (plan == "buy")
            {
                //var raw = screen.Where(m => m.High > m.Resistance && m.High <= (decimal)1.03 * m.Resistance);
                var raw = Screening.ScreeningBuy(screen);
                model =  FilterAverageVolume(raw, 50, avgVolume);

                //model = screen.Where(m => m.High > m.Resistance && m.High <= (decimal)1.03 * m.Resistance).Select(x =>
                //{
                //    return BuildStockScreeningModel(x);
                //}).OrderBy(m => m.StockID).ToList();
            }
            else if (plan == "sell")
            {
                //var raw = screen.Where(m => m.Low < m.Support && m.Low >= (decimal)0.925 * m.Support);
                var raw =  Screening.ScreeningSell(screen);
                model = FilterAverageVolume(raw, 50, avgVolume);
                //model = screen.Where(m => m.Low
                //< m.Support && m.Low >= (decimal)0.925 * m.Support).Select(x =>
                //{
                //    return BuildStockScreeningModel(x);
                //}).OrderBy(m => m.StockID).ToList();
            }
            else    if (plan == "hold")
            {
                //var raw = screen.Where(m => m.Close > (decimal)1.03 * m.Resistance);
                var raw = Screening.ScreeningHold(screen);
                model = FilterAverageVolume(raw, 50, avgVolume);
                // model = screen.Where(m => m.Close > (decimal)1.03 * m.Resistance).Select(x =>
                //{
                //    return BuildStockScreeningModel(x);
                //}).OrderBy(m => m.StockID).ToList();
            }
            else if (plan == "watch")
            {
                //var raw = screen.Where(m => m.Close >= (decimal)0.97 * m.Resistance && m.Close <= m.Resistance);
                var raw = Screening.ScreeningWatch(screen);
                model = FilterAverageVolume(raw, 50, avgVolume);
                //model = screen.Where(m => m.Close >= (decimal)0.97 * m.Resistance && m.Close <= m.Resistance).Select(x =>
                //  {
                //      return BuildStockScreeningModel(x);
                //  }).OrderBy(m => m.StockID).ToList();
            }
            else if (plan == "abnormal")
            {
                //model = screen.Select(x =>
                //{
                //    return BuildStockScreeningModel(x);
                //}).ToList();
                //model = screen.Where(m => m.BigWave > m.AverageBigWave).Select(x =>
                //{
                //    return BuildStockScreeningModel(x);
                //}).OrderBy(m => m.StockID).ToList();

                //var raw = screen.Where(m => m.Close > (decimal)1.025 * m.BBUpper || m.Close < (decimal)0.975 * m.BBLower);
                var raw = Screening.ScreeningAbnormal(screen);
                model = FilterAverageVolume(raw, 50, avgVolume);
               // model = screen.Where(m => m.Close > (decimal)1.025 * m.BBUpper || m.Close < (decimal)0.975 * m.BBLower).Select(x =>
               //{
               //    return BuildStockScreeningModel(x);
               //}).OrderBy(m => m.StockID).ToList();
            }
            else if (plan == "bigwave")
            {
                //var raw = screen.Where(m => m.BigWave > m.AverageBigWave);
                var raw = Screening.ScreeningBigwave(screen);
                model = FilterAverageVolume(raw, 50, avgVolume);
                //model = screen.Where(m => m.BigWave > m.AverageBigWave).Select(x =>
                //{
                //    return BuildStockScreeningModel(x);
                //}).OrderBy(m => m.StockID).ToList();
            }


            if (string.IsNullOrEmpty(sort))
                //sort = "RiskProfile";
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


            Paging<StockScreeningViewModel> pagingModel = new Paging<StockScreeningViewModel>();
            pagingModel.data = (IQueryable<StockScreeningViewModel>)model.AsQueryable();
            pagingModel.attribute.recordPerPage = 100;
            pagingModel.attribute.recordsTotal = model.Count();
            pagingModel.attribute.totalPage = Convert.ToInt32(Math.Ceiling((decimal)pagingModel.attribute.recordsTotal / pagingModel.attribute.recordPerPage));
            pagingModel.attribute.url = @"/StockScreening/Index";
            pagingModel.attribute.divName = "stockScreening";
            pagingModel.attribute.sorting = sort;
            pagingModel.attribute.filter = filter;
            pagingModel.OrderBy(sort);

            if (page == null)
                page = 1;

            pagingModel.DoPaging((int)page);


            return View(pagingModel);






            //var watchList = await _context.Stock.Include(p => p.Prices).ToListAsync();
            //var model = watchList.Select(x =>

            //{
            //    var lastPrice = x.Prices.OrderBy(m => m.Date).LastOrDefault();
            //    if (lastPrice == null)
            //        return null;

            //    if (plan == "BUY")
            //    {
            //        if (lastPrice.High > lastPrice.Resistance)
            //        {
            //            return new StockScreeningViewModel()
            //            {


            //                StockID = x.StockID,
            //                Open = lastPrice == null ? 0 : lastPrice.Open,
            //                Close = lastPrice == null ? 0 : lastPrice.Close,
            //                High = lastPrice == null ? 0 : lastPrice.High,
            //                Low = lastPrice == null ? 0 : lastPrice.Low,
            //                CloseToSupport = (lastPrice.Close - lastPrice.Support) / lastPrice.Support,
            //                CloseToResistance = (lastPrice.Close - lastPrice.Resistance) / lastPrice.Resistance
            //            };
            //        }
            //        else
            //            return null;
            //    }
            //    else
            //        return null;
            //    //return new WatchList()
            //    //{


            //    //    StockID = x.StockID,
            //    //    Open = lastPrice == null ? 0 : lastPrice.Open,
            //    //    Close = lastPrice == null ? 0 : lastPrice.Close,
            //    //    High = lastPrice == null ? 0 : lastPrice.High,
            //    //    Low = lastPrice == null ? 0 : lastPrice.Low,
            //    //    Volume = lastPrice == null ? 0 : lastPrice.Volume
            //    //};
            //}
            //).ToList();
            //return View(model);

        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IViewComponentResult> DeleteAsync([FromBody] string stockId)
        //{
        //    var watchList = await _context.StockWatchList.SingleOrDefaultAsync(s => s.ApplicationUserId == _userID.Id && s.StockID == stockId);
        //    _context.StockWatchList.Remove(watchList);
        //    await _context.SaveChangesAsync();
        //    return View();
        //}
    }
}
