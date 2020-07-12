using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GSOptima.Data;
using GSOptima.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GSOptima.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace GSOptima.Controllers
{
    [Authorize]
    //[SessionTimeout]
    public class StockChartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public StockChartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StockPrices
        public async Task<IActionResult> Index(string stockId, int numberOfDays, bool isModalDialog)
        {
 
            if (Request?.Headers != null &&
              Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return ViewComponent("StockChart", new { parameter = new ChartViewModel() { StockID = stockId, NumberOfDays = numberOfDays, IsModalDialog = isModalDialog } } );

            }
            return View();

        }

        [HttpGet]
        public async Task<IActionResult> DisplayChart(string stockId, int numberOfDays, bool isModalDialog)
        {

            return PartialView("_StockChart", new ChartViewModel() { StockID = stockId, NumberOfDays = numberOfDays, IsModalDialog = isModalDialog });
            //return ViewComponent("StockChart", new { parameter = new ChartViewModel() { StockID = stockId, NumberOfDays = numberOfDays, IsModalDialog = isModalDialog } });
        }

        public static async Task<ChartViewModel> GetChartModel(ChartViewModel parameter, ApplicationDbContext _context, ApplicationUser currentUser)
        {

            if (parameter.NumberOfDays == 0)
                parameter.NumberOfDays = 360;

            var now = DateTime.Now;
            //*WARNING : DIAKALIN SEMENTARA
            //now = new DateTime(2016, 10, 5);
            now = DateTime.Today;


            var stock = await _context.Stock.SingleOrDefaultAsync(m => m.StockID.ToUpper() == parameter.StockID.ToUpper());
            if (stock != null)
                parameter.StockName = stock.Name;
            else
            {
                parameter.StockName = "";
                return parameter;
            }


            //var prices = await _context.StockPrice.Where(s => s.StockID.ToUpper() == parameter.StockID.ToUpper() && s.Date<=now &&  s.Date >= now.AddDays(-1 * parameter.NumberOfDays)).OrderBy(m => m.Date).ToListAsync();
            var prices = _context.StockPrice.Where(s => s.StockID.ToUpper() == parameter.StockID.ToUpper() && s.Date <= now).OrderBy(m => m.Date).ToList();


            //*----------------------------------------------------------------------------------------------------*//
            //* NOTE : 
            //* Ini cara yang sedikit aneh, kita membuat satu anonymous object mirip seperti object 
            //* Stock Prices tapi tanpa field Stocks untuk mencegah circular reference.
            //* Sebetulnya circular reference bisa diatasi dengan menambah setting serializer pada startup class
            //* tapi setelah dicoba ternyata performancenya sangat lambat.
            //*----------------------------------------------------------------------------------------------------*//
            var prices_obj = prices.Select(s => new
            {
                BBLower = s.BBLower,
                BBUpper = s.BBUpper,
                BigWave = s.BigWave,
                Close = s.Close,
                Date = s.Date,
                EMA12 = s.EMA12,
                EMA26 = s.EMA26,
                Frequency = s.Frequency,
                GSLine = s.GSLine,
                High = s.High,
                Highest12Months = s.Highest12Months,
                Highest3Months = s.Highest3Months,
                Highest6Months = s.Highest6Months,
                //Id = s.Id,
                Low = s.Low,
                Lowest12Months = s.Lowest12Months,
                Lowest3Months = s.Lowest3Months,
                Lowest6Months = s.Lowest6Months,
                MA20 = s.MA20,
                MA60 = s.MA60,
                MACD = s.MACD,
                Open = s.Open,
                Resistance = s.Resistance,
                SD20 = s.SD20,
                SignalLine = s.SignalLine,
                StockID = s.StockID,
                Support = s.Support,
                TrendHigh = s.TrendHigh,
                TrendLow = s.TrendLow,
                Volume = s.Volume,
                Action = s.Action,
                LastDate = s.Date
            });

            parameter.Prices = prices_obj;

            var last_data = prices_obj.LastOrDefault();

            if (last_data != null)
            {
                //var stockName = await _context.Stock.SingleOrDefaultAsync(s => s.StockID.ToUpper() == parameter.StockID.ToUpper());
                //parameter.StockName = stockName.Name;

                //var stock = await _context.Stock.SingleOrDefaultAsync(m => m.StockID.ToUpper() == parameter.StockID.ToUpper());
                //if (stock != null)
                //    parameter.StockName = stock.Name;
                //else
                //    parameter.StockName = "";

                parameter.LastDate = last_data.LastDate;
                parameter.StockID = parameter.StockID.ToUpper();

                parameter.LastOpen = last_data.Open;
                parameter.LastHigh = last_data.High;
                parameter.LastLow = last_data.Low;
                parameter.LastClose = last_data.Close;

                parameter.LastSupport = last_data.Support;
                parameter.LastResistance = last_data.Resistance;
                parameter.LastVolume = last_data.Volume;
                if (last_data.Support != null)
                    parameter.PercentToSupport = (last_data.Close - (Decimal)last_data.Support) / (Decimal)last_data.Support * 100;
                else
                    parameter.PercentToSupport = null;

                if (last_data.Resistance != null)
                    parameter.PercentToResistance = (last_data.Close - (Decimal)last_data.Resistance) / (Decimal)last_data.Resistance * 100;
                else
                    parameter.PercentToResistance = null;

                if (parameter.LastSupport != null && parameter.LastResistance != null)
                {
                    if (parameter.LastClose > parameter.LastResistance && parameter.LastClose > parameter.LastSupport)
                        parameter.Risk = parameter.PercentToSupport;
                    else if (parameter.LastClose <= parameter.LastResistance && parameter.LastClose >= parameter.LastSupport)
                        parameter.Risk = Math.Abs((decimal)parameter.PercentToSupport) + Math.Abs((decimal)parameter.PercentToResistance);
                    else
                        parameter.Risk = 0;
                }
                else
                    parameter.Risk = null; 
            }
            //parameter.Prices = prices_obj;


           

            //ApplicationUser currentUser = await usermanager.GetUserAsync(HttpContext.User);
            var isOnWatchlist = _context.StockWatchList.Any(m => m.StockID.ToUpper() == parameter.StockID.ToUpper() && m.ApplicationUserId == currentUser.Id);
            if (isOnWatchlist)
                parameter.IsOnWatchList = "R";  
            else
                parameter.IsOnWatchList = "A";

            return parameter;
        }

        //Methode ini dipanggil lewat AJAX waktu ingin menampilkan chart, bukan pada saat pertama kali page load. Untuk pertama kali page load, lihat StockChartViewComponent
        public async Task<IActionResult> GetPrices([FromBody] ChartViewModel parameter)
        {
            //if (parameter.NumberOfDays == 0)
            //    parameter.NumberOfDays = 360;

            //var now = DateTime.Now;
            ////*WARNING : DIAKALIN SEMENTARA
            ////now = new DateTime(2016, 10, 5);
            //now = DateTime.Today;


            ////var prices = await _context.StockPrice.Where(s => s.StockID.ToUpper() == parameter.StockID.ToUpper() && s.Date<=now &&  s.Date >= now.AddDays(-1 * parameter.NumberOfDays)).OrderBy(m => m.Date).ToListAsync();
            //var prices = _context.StockPrice.Where(s => s.StockID.ToUpper() == parameter.StockID.ToUpper() && s.Date <= now).OrderBy(m => m.Date).ToList();


            ////*----------------------------------------------------------------------------------------------------*//
            ////* NOTE : 
            ////* Ini cara yang sedikit aneh, kita membuat satu anonymous object mirip seperti object 
            ////* Stock Prices tapi tanpa field Stocks untuk mencegah circular reference.
            ////* Sebetulnya circular reference bisa diatasi dengan menambah setting serializer pada startup class
            ////* tapi setelah dicoba ternyata performancenya sangat lambat.
            ////*----------------------------------------------------------------------------------------------------*//
            //var prices_obj = prices.Select(s => new
            //{
            //    BBLower = s.BBLower,
            //    BBUpper = s.BBUpper,
            //    BigWave = s.BigWave,
            //    Close = s.Close,
            //    Date = s.Date,
            //    EMA12 = s.EMA12,
            //    EMA26 = s.EMA26,
            //    Frequency = s.Frequency,
            //    GSLine = s.GSLine,
            //    High = s.High,
            //    Highest12Months = s.Highest12Months,
            //    Highest3Months = s.Highest3Months,
            //    Highest6Months = s.Highest6Months,
            //    //Id = s.Id,
            //    Low = s.Low,
            //    Lowest12Months = s.Lowest12Months,
            //    Lowest3Months = s.Lowest3Months,
            //    Lowest6Months = s.Lowest6Months,
            //    MA20 = s.MA20,
            //    MA60 = s.MA60,
            //    MACD = s.MACD,
            //    Open = s.Open,
            //    Resistance = s.Resistance,
            //    SD20 = s.SD20,
            //    SignalLine = s.SignalLine,
            //    StockID = s.StockID,
            //    Support = s.Support,
            //    TrendHigh = s.TrendHigh,
            //    TrendLow = s.TrendLow,
            //    Volume = s.Volume,
            //    Action = s.Action
            //});

            //parameter.Prices = prices_obj;

            //var last_data = prices_obj.LastOrDefault();

            //if (last_data != null)
            //{
            //    var stockName = await _context.Stock.SingleOrDefaultAsync(s => s.StockID.ToUpper() == parameter.StockID.ToUpper());
            //    parameter.StockName = stockName.Name;
            //    parameter.StockID = parameter.StockID.ToUpper();

            //    parameter.LastOpen = last_data.Open;
            //    parameter.LastHigh = last_data.High;
            //    parameter.LastLow = last_data.Low;
            //    parameter.LastClose = last_data.Close;

            //    parameter.LastSupport = last_data.Support;
            //    parameter.LastResistance = last_data.Resistance;
            //    parameter.LastVolume = last_data.Volume;
            //    if (last_data.Support != null)
            //        parameter.PercentToSupport = Math.Abs((last_data.Close - (Decimal)last_data.Support) / (Decimal)last_data.Support * 100);
            //    else
            //        parameter.PercentToSupport = null;

            //    if (last_data.Resistance != null)
            //        parameter.PercentToResistance = Math.Abs((last_data.Close - (Decimal)last_data.Resistance) / (Decimal)last_data.Resistance * 100);
            //    else
            //        parameter.PercentToResistance = null;
            //}
            //parameter.Prices = prices_obj;


            //var stock = await _context.Stock.SingleOrDefaultAsync(m => m.StockID.ToUpper() == parameter.StockID.ToUpper());
            //if (stock != null)
            //    parameter.StockName = stock.Name;



            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            //var isOnWatchlist = _context.StockWatchList.Any(m => m.StockID.ToUpper() == parameter.StockID.ToUpper() && m.ApplicationUserId == currentUser.Id);
            //if (isOnWatchlist)
            //    parameter.IsOnWatchList = "R";
            //else
            //    parameter.IsOnWatchList = "A";

            ChartViewModel vm = await GetChartModel(parameter, _context, currentUser);

            //if (prices.Count > 0)
            if(!String.IsNullOrEmpty(vm.StockName)) //means that stock is valid
                return Json(vm);
            //return Json(prices);
            else
                return Json(null);

        }




    }
}
