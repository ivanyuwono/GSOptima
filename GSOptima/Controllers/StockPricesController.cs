using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GSOptima.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using GSOptima.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Authorization;

namespace GSOptima.Controllers
{
    public interface IFormFile
    {
        string ContentType { get; }
        string ContentDisposition { get; }
        IHeaderDictionary Headers { get; }
        long Length { get; }
        string Name { get; }
        string FileName { get; }
        Stream OpenReadStream();
        void CopyTo(Stream target);
        Task CopyToAsync(Stream target, CancellationToken? cancellationToken);
    }

    [Authorize]
    //[SessionTimeout]
    public class StockPricesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockPricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DailyUpload()
        {
            return View();
        }

        public IActionResult DownloadData()
        {
            return View();
        }

        //[HttpPost("DailyUpload")]
        //public async Task<IActionResult> DailyUpload(List<IFormFile> files)
        //{
        //    long size = files.Sum(f => f.Length);

        //    // full path to file in temp location
        //    var filePath = Path.GetTempFileName();
        //    //var uploads = Path.Combine(_environment.WebRootPath, "uploads");


        //    foreach (var formFile in files)
        //    {
        //        if (formFile.Length > 0)
        //        {
        //            //using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))


        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }

        //            var stream2 = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        //            using (var streamReader = new StreamReader(stream2, Encoding.UTF8))
        //            {
        //                string line = "";
        //                while ((line = streamReader.ReadLine()) != null)
        //                {
        //                    // process the line
        //                    //Stock Code,	Stock Name,	High,	Low,	Close,	Change,	Volume,	Value,	Frequency
        //                    string[] result = line.Split('|');

        //                    if (result[0].ToUpper() != "STOCK CODE")
        //                    {
        //                        if (!_context.Stock.Any(m => m.StockID == result[0]))
        //                        {
        //                            _context.Stock.Add(new Stock() { StockID = result[0], Name = "Stock " + result[0] });
        //                            await _context.SaveChangesAsync();
        //                        }
        //                        if (_context.StockPrice.Any(m => m.StockID == result[0] && m.Date == DateTime.Today))
        //                        {
        //                            var stock = await _context.StockPrice.SingleOrDefaultAsync(m => m.StockID == result[0] && m.Date == DateTime.Today);
        //                            stock.High = Convert.ToDecimal(result[3]);
        //                            stock.Low = Convert.ToDecimal(result[4]);
        //                            stock.Close = Convert.ToDecimal(result[5]);
        //                            stock.Open = stock.Close + Convert.ToDecimal(result[2]);
        //                            stock.Volume = (long)Convert.ToDecimal(result[6]);
        //                            stock.Frequency = (long)Convert.ToDecimal(result[7]);
        //                            await _context.SaveChangesAsync();
        //                        }
        //                        else
        //                        {
        //                            StockPrice stock = new StockPrice();
        //                            stock.StockID = result[0];
        //                            stock.Date = DateTime.Today;
        //                            stock.High = Convert.ToDecimal(result[3]);
        //                            stock.Low = Convert.ToDecimal(result[4]);
        //                            stock.Close = Convert.ToDecimal(result[5]);
        //                            stock.Open = stock.Close + Convert.ToDecimal(result[2]);
        //                            stock.Volume = (long)Convert.ToDecimal(result[6]);
        //                            stock.Frequency = (long)Convert.ToDecimal(result[7]);
        //                            _context.StockPrice.Add(stock);
        //                            await _context.SaveChangesAsync();

        //                        }
        //                    }
        //                }

        //            }

        //        }
        //    }

        //    // process uploaded files
        //    // Don't rely on or trust the FileName property without validation.

        //    //return Ok(new { count = files.Count, size, filePath });
        //    ViewData["Message"] = "Success";
        //    return View();

        //}

        
        private async Task<int>  CreateStockPriceFile(string filePath)
        {


            var stream = new FileStream(filePath, FileMode.Create);

            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
            {
                var header = "Date;Stock;Open;High;Low;Close;Resist;Support;GSLine";
                streamWriter.WriteLine(header);
                foreach (var stock in await _context.Stock.ToListAsync())
                {
                    var lastPrice = await _context.StockPrice.Where(m => m.StockID == stock.StockID).OrderByDescending(x=>x.Date).FirstOrDefaultAsync();
                    var line = "";
                    if (lastPrice != null)
                        line = lastPrice.Date.ToString("yyyy/MM/dd") + ";" + stock.StockID + ";" + lastPrice.Open + ";" + lastPrice.High + ";" + lastPrice.Low + ";" + lastPrice.Close + ";" + lastPrice.Resistance + ";" + lastPrice.Support + ";" + lastPrice.GSLineDirection;
                    else
                        line = lastPrice.Date.ToString("yyyy/MM/dd") + ";" + stock.StockID + ";0;0;0;0;0;0;0";

                    streamWriter.WriteLine(line);
     
                }
            }
            return 1;
        }


        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    
        public async Task<IActionResult> DownloadLatest()
        {
            //if (file == null)
            //    return Content("filename not present");
            //var files = Request.Form.Files;
            //var date = Request.Form["date"];


            var path = Path.Combine(
   Directory.GetCurrentDirectory(),
   "wwwroot", "DailyData" + DateTime.Today.ToString("yyyyMMddd")+".txt");

            var lastdate = await CreateStockPriceFile(path);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        [HttpPost]
        public async Task<IActionResult> DailyUploadAjax(UploadViewModel model)
        {
            //var files = Request.Form.Files;

            if (model.files == null)
                Json("E-" + "File not uploaded");

            //var date = Request.Form["date"];

            DateTime currentDate = Convert.ToDateTime(model.date);

            //long size = files.Sum(f => f.Length);


            //long size = files.Length;
            int count = 0;
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            //var uploads = Path.Combine(_environment.WebRootPath, "uploads");


            //foreach (var formFile in model.files)
            var formFile = model.files;
            {
                if (formFile.Length > 0)
                {
                    //using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))


                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    var stream2 = new FileStream(filePath, FileMode.Open, FileAccess.Read);


                    try
                    {
                        using (var streamReader = new StreamReader(stream2, Encoding.UTF8))
                        {


                            string line = "";
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                // process the line
                                //Stock Code,	Stock Name,	High,	Low,	Close,	Change,	Volume,	Value,	Frequency
                                string[] result = line.Split('|');

                                if (result[0].ToUpper() != "STOCK CODE")
                                {
                                    count++;

                                    if (!_context.Stock.Any(m => m.StockID == result[0]))
                                    {
                                        _context.Stock.Add(new Stock() { StockID = result[0], Name = result[1] });
                                        await _context.SaveChangesAsync();
                                    }

                                    if (_context.StockPrice.Any(m => m.StockID == result[0] && m.Date == currentDate))
                                    {
                                        var stock = await _context.StockPrice.SingleOrDefaultAsync(m => m.StockID == result[0] && m.Date == currentDate);
                                        stock.High = Convert.ToDecimal(result[3]);
                                        stock.Low = Convert.ToDecimal(result[4]);
                                        stock.Close = Convert.ToDecimal(result[5]);
                                        //stock.Open = stock.Close + Convert.ToDecimal(result[2]);
                                        stock.Open = Convert.ToDecimal(result[2]);
                                        stock.Volume = (long)Convert.ToDecimal(result[6]);
                                        stock.Frequency = (long)Convert.ToDecimal(result[7]);
                                        await CalculateIndicator2(result[0].ToUpper(), stock);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        StockPrice stock = new StockPrice();
                                        stock.StockID = result[0];
                                        stock.Date = currentDate;
                                        stock.High = Convert.ToDecimal(result[3]);
                                        stock.Low = Convert.ToDecimal(result[4]);
                                        stock.Close = Convert.ToDecimal(result[5]);
                                        //stock.Open = stock.Close + Convert.ToDecimal(result[2]);
                                        stock.Open = Convert.ToDecimal(result[2]);
                                        stock.Volume = (long)Convert.ToDecimal(result[6]);
                                        stock.Frequency = (long)Convert.ToDecimal(result[7]);
                                        //_context.StockPrice.Add(stock);
                                        await CalculateIndicator2(result[0].ToUpper(), stock);

                                        _context.StockPrice.Add(stock);
                                        await _context.SaveChangesAsync();
                                        //*Refresh 1-3 Days Previous Prices
                                        //*Get 5 days latest data, refresh only 3 days ago data
                                        //var prevPrices = (_context.StockPrice.Where(m => m.StockID == result[0]).OrderByDescending(m=>m.Date).Take(5)).OrderBy(m=>m.Date);
                                        //*If there is less than 5 data, then skip
                                        //if(prevPrices.Count() == 5)
                                        //{
                                        //    var counter = 0;
                                        //    foreach (var price in prevPrices)
                                        //    {
                                        //        if (counter == 1)
                                        //        {
                                        //            price.Support = Support(1, prevPrices.ToArray());
                                        //            price.Resistance = Resistance(1, prevPrices.ToArray());
                                        //            await _context.SaveChangesAsync();
                                        //        }
                                        //        counter++;
                                        //    }
                                        //}
                                    }
                                }
                            }
                            //_context.SaveChangesAsync();
                        }

                    }
                    catch (Exception ex)
                    {
                        return Json("E-" + ex.InnerException);
                    }


                }
            }

            return Json("I-" + count.ToString() + " stock(s) have been updated successfuly");

        }
        [HttpPost]
        public async Task<IActionResult> DailyUploadNonAjax(UploadViewModel model)
        {
            //var files = Request.Form.Files;

            //if (files == null)
            //    Json("E-" + "File not uploaded");

            //var date = Request.Form["date"];

            DateTime currentDate = Convert.ToDateTime(model.date);

            //long size = files.Sum(f => f.Length);
            
            
            //long size = files.Length;
            int count = 0;
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            //var uploads = Path.Combine(_environment.WebRootPath, "uploads");


            //foreach (var formFile in model.files)
            var formFile = model.files;
            {
                if (formFile.Length > 0)
                {
                    //using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))


                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    var stream2 = new FileStream(filePath, FileMode.Open, FileAccess.Read);


                    try
                    {
                        using (var streamReader = new StreamReader(stream2, Encoding.UTF8))
                        {


                            string line = "";
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                // process the line
                                //Stock Code,	Stock Name,	High,	Low,	Close,	Change,	Volume,	Value,	Frequency
                                string[] result = line.Split('|');

                                if (result[0].ToUpper() != "STOCK CODE")
                                {
                                    count++;

                                    if (!_context.Stock.Any(m => m.StockID == result[0]))
                                    {
                                        _context.Stock.Add(new Stock() { StockID = result[0], Name = result[1] });
                                        await _context.SaveChangesAsync();
                                    }

                                    if (_context.StockPrice.Any(m => m.StockID == result[0] && m.Date == currentDate))
                                    {
                                        var stock = await _context.StockPrice.SingleOrDefaultAsync(m => m.StockID == result[0] && m.Date == currentDate);
                                        stock.High = Convert.ToDecimal(result[3]);
                                        stock.Low = Convert.ToDecimal(result[4]);
                                        stock.Close = Convert.ToDecimal(result[5]);
                                        //stock.Open = stock.Close + Convert.ToDecimal(result[2]);
                                        stock.Open = Convert.ToDecimal(result[2]);
                                        stock.Volume = (long)Convert.ToDecimal(result[6]);
                                        stock.Frequency = (long)Convert.ToDecimal(result[7]);
                                        await CalculateIndicator2(result[0].ToUpper(), stock);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        StockPrice stock = new StockPrice();
                                        stock.StockID = result[0];
                                        stock.Date = currentDate;
                                        stock.High = Convert.ToDecimal(result[3]);
                                        stock.Low = Convert.ToDecimal(result[4]);
                                        stock.Close = Convert.ToDecimal(result[5]);
                                        //stock.Open = stock.Close + Convert.ToDecimal(result[2]);
                                        stock.Open = Convert.ToDecimal(result[2]);
                                        stock.Volume = (long)Convert.ToDecimal(result[6]);
                                        stock.Frequency = (long)Convert.ToDecimal(result[7]);
                                        //_context.StockPrice.Add(stock);
                                        await CalculateIndicator2(result[0].ToUpper(), stock);

                                        _context.StockPrice.Add(stock);
                                        await _context.SaveChangesAsync();
                                        //*Refresh 1-3 Days Previous Prices
                                        //*Get 5 days latest data, refresh only 3 days ago data
                                        //var prevPrices = (_context.StockPrice.Where(m => m.StockID == result[0]).OrderByDescending(m=>m.Date).Take(5)).OrderBy(m=>m.Date);
                                        //*If there is less than 5 data, then skip
                                        //if(prevPrices.Count() == 5)
                                        //{
                                        //    var counter = 0;
                                        //    foreach (var price in prevPrices)
                                        //    {
                                        //        if (counter == 1)
                                        //        {
                                        //            price.Support = Support(1, prevPrices.ToArray());
                                        //            price.Resistance = Resistance(1, prevPrices.ToArray());
                                        //            await _context.SaveChangesAsync();
                                        //        }
                                        //        counter++;
                                        //    }
                                        //}
                                    }
                                }
                            }
                            //_context.SaveChangesAsync();
                        }

                    }
                    catch (Exception ex)
                    {
                        return Json("E-" + ex.InnerException);
                    }


                }
            }

            //var t = await CalculateIndicator();
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            //return Content("Success");
            //return Ok(new { count = files.Count, size, filePath });
            return Json("I-" + count.ToString() + " stock(s) have been updated successfuly");

        }
        //private bool StockPriceExists(string id, DateTime date)
        //{
        //    return _context.StockPrice.Any(e => e.StockID == id && e.Date == date);
        //}

        #region "FORMULA"
        //private async Task<bool> InitIndicator()
        //{

        //    foreach (Stock stock in await _context.Stock.Where(s=>s.StockID == "AALI").ToListAsync())
        //    //foreach (Stock stock in _context.Stock.Where(m => m.StockID == "AALI" || m.StockID == "BBCA" || m.StockID == "PTBA"))

        //    {
        //        //Console.Write("TEST " + stock.StockID);
        //        System.Diagnostics.Debug.WriteLine("TEST " + stock.StockID);

        //        //*cari semua histori harga
        //        var data = _context.StockPrice.Where(c => c.StockID == stock.StockID).OrderBy(c => c.Date).ToList();
        //        var count = data.Count();
        //        if (count > 0)
        //        {

        //            for(int i=0; i<count-1; i++)
        //            {
        //                var price = data[0];
        //                var counter = i;
        //                //foreach (StockPrice price in data.ToList().Last())
        //                //{
        //                price.MA20 = Calculation.MA(counter, data.ToArray(), 20);
        //                price.MA60 = Calculation.MA(counter, data.ToArray(), 60);
        //                price.Support = Calculation.Support(counter, data.ToArray());
        //                price.Resistance = Calculation.Resistance(counter, data.ToArray());


        //                dynamic t3 = Calculation.HighestLowest(counter, data.ToArray(), 3);
        //                price.Highest3Months = (t3 == null ? null : t3.Highest);
        //                price.Lowest3Months = (t3 == null ? null : t3.Lowest);

        //                dynamic t6 = Calculation.HighestLowest(counter, data.ToArray(), 6);
        //                price.Highest6Months = (t6 == null ? null : t6.Highest);
        //                price.Lowest6Months = (t6 == null ? null : t6.Lowest);

        //                dynamic t12 = Calculation.HighestLowest(counter, data.ToArray(), 12);
        //                price.Highest12Months = (t12 == null ? null : t12.Highest);
        //                price.Lowest12Months = (t12 == null ? null : t12.Lowest);

        //                price.TrendHigh = Calculation.TrendHigh(counter, data.ToArray(), price.Highest3Months, price.Highest6Months, price.Highest12Months);
        //                price.TrendLow = Calculation.TrendLow(counter, data.ToArray(), price.Lowest3Months, price.Lowest6Months, price.Lowest12Months);

        //                //dynamic highlow = BreakHighLowCalc(counter, data.ToArray());
        //                //price.TrendHigh = highlow.TrendHigh;
        //                //price.TrendLow = highlow.TrendLow;

        //                price.EMA12 = Calculation.EMA(counter, data.ToArray(), 12);
        //                price.EMA26 = Calculation.EMA(counter, data.ToArray(), 26);
        //                if (price.EMA12 == null || price.EMA26 == null)
        //                    price.MACD = null;
        //                else
        //                    price.MACD = price.EMA12 - price.EMA26;
        //                price.SignalLine = Calculation.SignalLine(counter, data.ToArray(), 9);

        //                if (price.MACD == null || price.SignalLine == null)
        //                    price.GSLine = null;
        //                else
        //                    price.GSLine = (Decimal)(price.MACD + price.SignalLine) / 2;

        //                price.SD20 = Calculation.SD(counter, data.ToArray(), 20);

        //                price.BBUpper = price.MA20 + 2 * price.SD20;
        //                price.BBLower = price.MA20 - 2 * price.SD20;
        //            }

        //            //*ambil tanggal terbaru
        //            //var price = await data.LastAsync();
           

        //            //counter++;
        //            //}
        //        }

        //    }
        //    await _context.SaveChangesAsync();
        //    return true;

        //}
        private async Task<bool> CalculateIndicator2(string stockID, StockPrice price)
        {

            //foreach (Stock stock in await _context.Stock.ToListAsync())
            //foreach (Stock stock in _context.Stock.Where(m => m.StockID == "AALI" || m.StockID == "BBCA" || m.StockID == "PTBA"))

            {
                //Console.Write("TEST " + stock.StockID);
                System.Diagnostics.Debug.WriteLine("TEST " + stockID);


                var prices = _context.StockPrice.Where(c => c.StockID == stockID).AsNoTracking();
                DateTime last_date;
                if (prices.Count() > 0)
                {
                    last_date = prices.Max(m => m.Date);
                }
                else
                    last_date = price.Date;
                

                //*cari semua histori harga
                StockPrice[] data = prices.Where(c => c.Date >= last_date.AddMonths(-13)).OrderBy(c => c.Date).AsNoTracking().ToArray();

                //*tambahkan data terbaru dalam memory
                data = data.Append(price).ToArray();



                var countx = data.Count();

                //StockPrice dataPrev = null; 
                //var indexPrev = 0;
                if (countx >= 5)
                {   
                    //*Update harga 3 hari sebelumnya
                    for (int i=countx-4;i<= countx - 2;i++)
                    {
                        //dataPrev = data[i];
                        StockPrice dataPrev = await _context.StockPrice.SingleOrDefaultAsync(m => m.Date == data[i].Date && m.StockID == stockID);
                        //indexPrev = countx - 3;


                        dataPrev.Support = Calculation.Support(i, data);
                        data[i].Support = dataPrev.Support;
                        dataPrev.Resistance = Calculation.Resistance(i, data);
                        data[i].Resistance = dataPrev.Resistance;
                        //dataPrev.Support = 60;
                        //dataPrev.Resistance = 28000;
                        await _context.SaveChangesAsync();
                    }
                   
                }
                var count = data.Count();
                if (count > 0)
                {

                    //for(int i=0; i<data.Length-1; i++)

                    //*ambil tanggal terbaru
                    //var price = data.Last();
                    var counter = count - 1;
                    //foreach (StockPrice price in data.ToList().Last())
                    //{
                    price.MA20 = Calculation.MA(counter, data, 20);
                    price.MA60 = Calculation.MA(counter, data, 60);
                    price.Support = Calculation.Support(counter, data);
                    price.Resistance = Calculation.Resistance(counter, data);


                    dynamic t3 = Calculation.HighestLowest(counter, data, 3);
                    price.Highest3Months = (t3 == null ? null : t3.Highest);
                    price.Lowest3Months = (t3 == null ? null : t3.Lowest);

                    dynamic t6 = Calculation.HighestLowest(counter, data, 6);
                    price.Highest6Months = (t6 == null ? null : t6.Highest);
                    price.Lowest6Months = (t6 == null ? null : t6.Lowest);

                    dynamic t12 = Calculation.HighestLowest(counter, data, 12);
                    price.Highest12Months = (t12 == null ? null : t12.Highest);
                    price.Lowest12Months = (t12 == null ? null : t12.Lowest);

                    price.TrendHigh = Calculation.TrendHigh(counter, data, price.Highest3Months, price.Highest6Months, price.Highest12Months);
                    price.TrendLow = Calculation.TrendLow(counter, data, price.Lowest3Months, price.Lowest6Months, price.Lowest12Months);

                    //dynamic highlow = BreakHighLowCalc(counter, data);
                    //price.TrendHigh = highlow.TrendHigh;
                    //price.TrendLow = highlow.TrendLow;

                    price.EMA12 = Calculation.EMA(counter, data, 12);
                    price.EMA26 = Calculation.EMA(counter, data, 26);
                    if (price.EMA12 == null || price.EMA26 == null)
                        price.MACD = null;
                    else
                        price.MACD = price.EMA12 - price.EMA26;
                    price.SignalLine = Calculation.SignalLine(counter, data, 9);

                    if (price.MACD == null || price.SignalLine == null)
                        price.GSLine = null;
                    else
                        price.GSLine = (Decimal)(price.MACD + price.SignalLine) / 2;


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

                    price.BBUpper = price.MA20 + 2 * price.SD20;
                    price.BBLower = price.MA20 - 2 * price.SD20;

                    if (price.Frequency > 0)
                        price.BigWave = (Decimal)price.Volume / (Decimal)(price.Frequency * price.Frequency * price.Frequency);
                    else
                        price.BigWave = 0;

                    price.AverageBigWave = Calculation.AverageBigWave(counter, data, 3);

                    //counter++;
                    //}
                }
                
            }
            //await _context.SaveChangesAsync();
            return true;

        }
        //private bool UpdatePreviousPrices(string stockID, StockPrice price)
        //{
        //    return true;
        //}
        //private async Task<bool> CalculateIndicator()
        //{

        //    foreach (Stock stock in await _context.Stock.ToListAsync())
        //    //foreach (Stock stock in _context.Stock.Where(m => m.StockID == "AALI" || m.StockID == "BBCA" || m.StockID == "PTBA"))

        //    {
        //        //Console.Write("TEST " + stock.StockID);
        //        System.Diagnostics.Debug.WriteLine("TEST " + stock.StockID);




        //        //*cari semua histori harga
        //        var data = _context.StockPrice.Where(c => c.StockID == stock.StockID).OrderBy(c => c.Date);
        //        var count = data.Count();
        //        if (count > 0)
        //        {

        //            //for(int i=0; i<data.Length-1; i++)

        //            //*ambil tanggal terbaru
        //            var price = await data.LastAsync();
        //            var counter = count - 1;
        //            //foreach (StockPrice price in data.ToList().Last())
        //            //{
        //            price.MA20 = MA(counter, data.ToArray(), 20);
        //            price.MA60 = MA(counter, data.ToArray(), 60);
        //            price.Support = Support(counter, data.ToArray());
        //            price.Resistance = Resistance(counter, data.ToArray());


        //            dynamic t3 = HighestLowest(counter, data.ToArray(), 3);
        //            price.Highest3Months = (t3 == null ? null : t3.Highest);
        //            price.Lowest3Months = (t3 == null ? null : t3.Lowest);

        //            dynamic t6 = HighestLowest(counter, data.ToArray(), 6);
        //            price.Highest6Months = (t6 == null ? null : t6.Highest);
        //            price.Lowest6Months = (t6 == null ? null : t6.Lowest);

        //            dynamic t12 = HighestLowest(counter, data.ToArray(), 12);
        //            price.Highest12Months = (t12 == null ? null : t12.Highest);
        //            price.Lowest12Months = (t12 == null ? null : t12.Lowest);

        //            price.TrendHigh = TrendHigh(counter, data.ToArray(), price.Highest3Months, price.Highest6Months, price.Highest12Months);
        //            price.TrendLow = TrendLow(counter, data.ToArray(), price.Lowest3Months, price.Lowest6Months, price.Lowest12Months);

        //            //dynamic highlow = BreakHighLowCalc(counter, data.ToArray());
        //            //price.TrendHigh = highlow.TrendHigh;
        //            //price.TrendLow = highlow.TrendLow;

        //            price.EMA12 = EMA(counter, data.ToArray(), 12);
        //            price.EMA26 = EMA(counter, data.ToArray(), 26);
        //            if (price.EMA12 == null || price.EMA26 == null)
        //                price.MACD = null;
        //            else
        //                price.MACD = price.EMA12 - price.EMA26;
        //            price.SignalLine = SignalLine(counter, data.ToArray(), 9);

        //            if (price.MACD == null || price.SignalLine == null)
        //                price.GSLine = null;
        //            else
        //                price.GSLine = (Decimal)(price.MACD + price.SignalLine) / 2;

        //            price.SD20 = SD(counter, data.ToArray(), 20);

        //            price.BBUpper = price.MA20 + 2 * price.SD20;
        //            price.BBLower = price.MA20 - 2 * price.SD20;

        //            //counter++;
        //            //}
        //        }

        //    }
        //    await _context.SaveChangesAsync();
        //    return true;

        //}
        #endregion

        
    }
}