using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GSOptima.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GSOptima.Controllers
{
    [Authorize]
    //[SessionTimeout]
    public class CorporateActionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CorporateActionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StockPriceAdjustment()
        {
           
            var StockID = Request.Form["StockID"];
            var Action = Request.Form["Action"];
            var OldRatio = Request.Form["OldRatio"];
            var NewRatio = Request.Form["NewRatio"];
            var NewPrice = Request.Form["NewPrice"];
            var StartDate = Request.Form["StartDate"];
            var EndDate = Request.Form["EndDate"];

            try
            {
                if(!await _context.Stock.AnyAsync(m=>m.StockID==StockID) && (Action == "S" || Action == "U"))
                {
                    return Json("I-" + "Stock is not valid");
                }

                if (Action == "S")
                {
                    await StockSplit(StockID, Convert.ToDateTime(StartDate), Convert.ToDateTime(EndDate), Convert.ToInt32(OldRatio), Convert.ToInt32(NewRatio));
                    await InitIndicator(StockID);
                }
                else if (Action == "R")
                {

                    await RightIssue(StockID, Convert.ToDateTime(StartDate), Convert.ToDateTime(EndDate), Convert.ToInt32(OldRatio), Convert.ToInt32(NewRatio), Convert.ToDecimal(NewPrice));
                    await InitIndicator(StockID);
                }
                else if(Action == "U")
                    await InitIndicator(StockID);
                else if(Action == "A")
                {
                    var stockList = await _context.Stock.ToListAsync();


                    foreach(var stock in stockList)
                    {
                        await InitIndicator(stock.StockID);

                    }
                }
                else if(Action == "D")
                {

                    await Delete(Convert.ToDateTime(StartDate));
                }
            }
            catch (Exception ex)
            {
                return Json("E-" + ex.InnerException);
            }
  

            return Json("I-" + "Prices have been updated successfuly");

            //var t = await CalculateIndicator();
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            //return Content("Success");
            //return Ok(new { count = files.Count, size, filePath });


        }

        public async Task<bool> Delete(DateTime date)
        {
             _context.StockPrice.RemoveRange(_context.StockPrice.Where(m => m.Date == date));
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> StockSplit(string stockID,  DateTime startDate, DateTime endDate, int oldRatio, int newRatio)
        {
            var prices = await _context.StockPrice.Where(m => m.StockID == stockID && m.Date >= startDate && m.Date<= endDate).ToListAsync();
            foreach (var price in prices)
            {
                price.Open = price.Open * (decimal)oldRatio / (decimal)newRatio;
                price.High = price.High * (decimal)oldRatio / (decimal)newRatio;
                price.Low = price.Low * (decimal)oldRatio / (decimal)newRatio;
                price.Close = price.Close * (decimal)oldRatio / (decimal)newRatio;
                price.Volume = price.Volume * newRatio / oldRatio;
                price.Frequency = price.Frequency * newRatio / oldRatio;
                if (price.Date == endDate)
                    price.Action = "SS;" + oldRatio + ";" + newRatio;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        //*This function also applies for Bonus Issue.
        //*For Bonus Issue, set newPrice = 0
        public async Task<bool> RightIssue(string stockID, DateTime startDate, DateTime endDate, int oldRatio, int newRatio, decimal newPrice)
        {
            var prices = await _context.StockPrice.Where(m => m.StockID == stockID && m.Date >= startDate && m.Date <= endDate).ToListAsync();
            foreach (var price in prices)
            {
                price.Open = ((price.Open * oldRatio) + (newPrice * newRatio)) / (oldRatio + newRatio);
                price.High = ((price.High * oldRatio) + (newPrice * newRatio)) / (oldRatio + newRatio);
                price.Low = ((price.Low * oldRatio) + (newPrice * newRatio)) / (oldRatio + newRatio);
                price.Close = ((price.Close * oldRatio) + (newPrice * newRatio)) / (oldRatio + newRatio);
                //*IVY : tadinya volume akan beribah seperti dibawah ini, tetapi setelah by phone dengan rio tgl 20 Nov 2017 jam 15.30 maka formula ini tidak jadi
                //price.Volume = price.Volume + (newRatio * price.Volume / oldRatio);
                if(price.Date== endDate)
                    price.Action = "RI;" + oldRatio + ";" + newRatio;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InitIndicator(string stockID)
        {

            //*cari semua histori harga
            var allData = await _context.StockPrice.Where(c => c.StockID == stockID).OrderBy(c => c.Date).ToListAsync();
            var data = await _context.StockPrice.Where(c => c.StockID == stockID).OrderBy(c => c.Date).AsNoTracking().ToArrayAsync();

            int counter = 0;
            foreach (var price in allData)
            {
                //var data = _context.StockPrice.Where(c => c.StockID == stock.StockID && c.Date >= price.Date.AddMonths(-13)).OrderBy(c => c.Date).AsNoTracking();

                System.Diagnostics.Debug.WriteLine("TEST " + stockID + " " + counter.ToString());

                price.MA20 = Calculation.MA(counter, data, 20);
                data[counter].MA20 = price.MA20;

                price.MA60 = Calculation.MA(counter, data, 60);
                data[counter].MA60 = price.MA60;

                price.Support = Calculation.Support(counter, data);
                data[counter].Support = price.Support;

                price.Resistance = Calculation.Resistance(counter, data);
                data[counter].Resistance = price.Resistance;

                dynamic t3 = Calculation.HighestLowest(counter, data, 3);
                price.Highest3Months = (t3 == null ? null : t3.Highest);
                data[counter].Highest3Months = price.Highest3Months;

                price.Lowest3Months = (t3 == null ? null : t3.Lowest);
                data[counter].Lowest3Months = price.Lowest3Months;

                dynamic t6 = Calculation.HighestLowest(counter, data, 6);
                price.Highest6Months = (t6 == null ? null : t6.Highest);
                data[counter].Highest6Months = price.Highest6Months;

                price.Lowest6Months = (t6 == null ? null : t6.Lowest);
                data[counter].Lowest6Months = price.Lowest6Months;

                dynamic t12 = Calculation.HighestLowest(counter, data, 12);
                price.Highest12Months = (t12 == null ? null : t12.Highest);
                data[counter].Highest12Months = price.Highest12Months;

                price.Lowest12Months = (t12 == null ? null : t12.Lowest);
                data[counter].Lowest12Months = price.Lowest12Months;

                price.TrendHigh = Calculation.TrendHigh(counter, data, price.Highest3Months, price.Highest6Months, price.Highest12Months);
                data[counter].TrendHigh = price.TrendHigh;

                price.TrendLow = Calculation.TrendLow(counter, data, price.Lowest3Months, price.Lowest6Months, price.Lowest12Months);
                data[counter].TrendLow = price.TrendLow;

                price.EMA12 = Calculation.EMA(counter, data, 12);
                data[counter].EMA12 = price.EMA12;

                price.EMA26 = Calculation.EMA(counter, data, 26);
                data[counter].EMA26 = price.EMA26;


                if (price.EMA12 == null || price.EMA26 == null)
                    price.MACD = null;
                else
                    price.MACD = price.EMA12 - price.EMA26;

                data[counter].MACD = price.MACD;

                price.SignalLine = Calculation.SignalLine(counter, data, 9);
                data[counter].SignalLine = price.SignalLine;

                if (price.MACD == null || price.SignalLine == null)
                    price.GSLine = null;
                else
                    price.GSLine = (Decimal)(price.MACD + price.SignalLine) / 2;
                data[counter].GSLine = price.GSLine;


                if (counter >= 1)
                {
                    if (price.GSLine != null && data[counter - 1].GSLine != null)
                    {
                        if (price.GSLine > data[counter - 1].GSLine)
                            price.GSLineDirection = "U";
                        else if (price.GSLine < data[counter - 1].GSLine)
                            price.GSLineDirection = "D";
                        else
                            price.GSLineDirection = "N";
                    }
                    else
                        price.GSLineDirection = "";
                }
                else
                    price.GSLineDirection = "";

                price.SD20 = Calculation.SD(counter, data, 20);
                data[counter].SD20 = price.SD20;

                if (price.Frequency > 0)
                    price.BigWave = (Decimal)price.Volume / (Decimal)(price.Frequency * price.Frequency * price.Frequency);
                else
                    price.BigWave = 0;

                data[counter].BigWave = price.BigWave;

                price.AverageBigWave = Calculation.AverageBigWave(counter, data, 3);
                data[counter].AverageBigWave = price.AverageBigWave;


                price.BBUpper = price.MA20 + 2 * price.SD20;
                data[counter].BBUpper = price.BBUpper;

                price.BBLower = price.MA20 - 2 * price.SD20;
                data[counter].BBLower = price.BBLower;

                counter++;

            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}