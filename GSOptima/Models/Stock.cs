using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GSOptima.Models
{
    public class CorporateAction
    {
        public string StockID { get; set; }
        public string Action { get; set; }
        public string OldRatio { get; set; }
        public string NewRatio { get; set; }
        public string NewPrice { get; set; }
    }


    public class StockWatchList
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
        public string StockID { get; set; }
        public Stock Stock { get; set; }
    }

    public class Stock
    {
        [Key]
        public string StockID { get; set; }
        public string Name { get; set; }
        public List<StockPrice> Prices { get; set; }

        public List<StockWatchList> StockWatchList { get; set; }


    }

    public class StockPrice
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Decimal Open { get; set; }
        public Decimal High { get; set; }
        public Decimal Low { get; set; }
        public Decimal Close { get; set; }
        public Int64 Volume { get; set; }
        public Int64? Frequency { get; set; }

        public Decimal? MA20 { get; set; }
        public Decimal? MA60 { get; set; }
        public Decimal? BBUpper { get; set; }
        public Decimal? BBLower { get; set; }
        public int? TrendHigh { get; set; }
        public int? TrendLow { get; set; }

        public Decimal? SD20 { get; set; }

        public Decimal? Support { get; set; }
        public Decimal? Resistance { get; set; }

        public Decimal? EMA12 { get; set; }
        public Decimal? EMA26 { get; set; }
        public Decimal? MACD { get; set; }
        public Decimal? SignalLine { get; set; }
        public Decimal? GSLine { get; set; }

        public string GSLineDirection { get; set; }
        public Decimal? BigWave { get; set; }
        public Decimal? AverageBigWave { get; set; }

        public Decimal? Highest3Months { get; set; }
        public Decimal? Highest6Months { get; set; }
        public Decimal? Highest12Months { get; set; }

        public Decimal? Lowest3Months { get; set; }
        public Decimal? Lowest6Months { get; set; }
        public Decimal? Lowest12Months { get; set; }

        public string Action { get; set; }

        public string StockID { get; set; }
        [ForeignKey("StockID")]
        public Stock Stocks { get; set; }
    }


    public class UploadViewModel
    {
        public IFormFile files { get; set; }
        public DateTime date { get; set; }
 
    }

    public class GSProWatchList
    {
        [Key]
        public string StockID { get; set; }
        public Decimal? Target1 { get; set; }
        public Decimal? Target2 { get; set; }
        [ForeignKey("StockID")]
        public Stock Stocks { get; set; }

    }

}
