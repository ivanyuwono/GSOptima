using GSOptima.Data;
using GSOptima.Models;
using GSOptima.Controllers;
using GSOptima.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace GSOptima.ViewComponents
{
    public class StockChartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private ApplicationUser _userID;

        public StockChartViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

           
        }

        // GET: StockWatchLists
        public async Task<IViewComponentResult> InvokeAsync(ChartViewModel parameter)
        {
            //_userID = currentUser;
            //var watchList = await _context.StockWatchList.Where(s => s.ApplicationUserId == _userID.Id).Include(s => s.Stock).ThenInclude(p=>p.Prices).ToListAsync();
            //var prices = await _context.StockPrice.Where(s => s.Date >= DateTime.Now.AddDays(-1 * numberOfDays)).OrderBy(m => m.Date).ToListAsync();
            //var name = await  _context.Stock.SingleOrDefaultAsync(m => m.StockID == parameter.StockID);
            //parameter.StockName = name.Name;


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
            //var prices_obj = prices.Select(s => new {
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

            //var last_data = prices_obj.LastOrDefault();
            //parameter.LastOpen = last_data.Open;
            //parameter.LastHigh = last_data.High;
            //parameter.LastLow = last_data.Low;
            //parameter.LastClose = last_data.Close;

            //parameter.LastSupport = last_data.Support;
            //parameter.LastResistance = last_data.Resistance;
            //parameter.LastVolume = last_data.Volume;
            //parameter.Prices = prices_obj;

            //if (last_data.Support != null)
            //    parameter.PercentToSupport = Math.Abs((last_data.Close - (Decimal)last_data.Support) / (Decimal)last_data.Support * 100);
            //else
            //    parameter.PercentToSupport = null;

            //if (last_data.Resistance != null)
            //    parameter.PercentToResistance = Math.Abs((last_data.Close - (Decimal)last_data.Resistance) / (Decimal)last_data.Resistance * 100);
            //else
            //    parameter.PercentToResistance = null;


            //var stock = await _context.Stock.SingleOrDefaultAsync(m => m.StockID.ToUpper() == parameter.StockID.ToUpper());
            //if (stock != null)
            //    parameter.StockName = stock.Name;

            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            //var isOnWatchlist = _context.StockWatchList.Any(m => m.StockID.ToUpper() == parameter.StockID.ToUpper() && m.ApplicationUserId == currentUser.Id);
            //if (isOnWatchlist)
            //    parameter.IsOnWatchList = "R";
            //else
            //    parameter.IsOnWatchList = "A";


            ChartViewModel vm = await StockChartController.GetChartModel(parameter,_context, currentUser);

            return View(vm);

        }
       
    }
}
