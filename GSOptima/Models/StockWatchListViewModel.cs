using GSOptima.Models;
using System;
using System.ComponentModel.DataAnnotations;


namespace GSOptima.ViewModels
{
    public class StockWatchListViewModel
    {
        public StockWatchListViewModel()
        {

        }
        public ApplicationUser CurrentUser { get; set; }        
        //public Paging<Stock> WatchList { get; set; }
        //public string ModifiedStockId { get; set; }
    }

    public class WatchList
    {
        public string StockID { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0}", ApplyFormatInEditMode = true)]
        public Decimal? Open { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0}", ApplyFormatInEditMode = true)]
        public Decimal? High { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0}", ApplyFormatInEditMode = true)]
        public Decimal? Low { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0}", ApplyFormatInEditMode = true)]
        public Decimal? Close { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0}", ApplyFormatInEditMode = true)]
        public Decimal? Volume { get; set; }
        public string TradingPlan { get; set; }
        public string Trend { get; set; }
        public string RiskProfile { get; set; }
        public bool BigWave { get; set; }
       
        public Decimal? CloseToSupport { get; set; }
        public Decimal? CloseToResistance { get; set; }
        public Decimal? Support { get; set; }
        public Decimal? Resistance { get; set; }

        public string GSLine { get; set; }
        public string GSLineDirection { get; set; }
        public string NormalRange { get; set; }
        public Decimal? BuyLimit { get; set; }
        public string LastTrendBar { get; set; }

    }

    public class GSProAdminWatchListViewModel
    {
        public string StockID { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0}", ApplyFormatInEditMode = true)]
        public Decimal? Target1 { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0}", ApplyFormatInEditMode = true)]
        public Decimal? Target2 { get; set; }
    }
    public class ChartViewModel
    {

        public string StockID { get; set; }
        public int NumberOfDays { get; set; }
        public object Prices { get; set; }
        public string StockName { get; set; }
        public Decimal? LastSupport { get; set; }
        public Decimal? LastResistance { get; set; }
        public Decimal LastOpen { get; set; }
        public Decimal LastHigh { get; set; }
        public Decimal LastLow { get; set; }
        public Decimal LastClose { get; set; }
        public Decimal LastVolume { get; set; }
        public Decimal? PercentToSupport { get; set; }
        public Decimal? PercentToResistance { get; set; }
        public Decimal? Risk { get; set; }
        public DateTime LastDate { get; set; }
        public bool IsTextBoxVisible { get; set; }
        //berisi apakah stockid terdapat pada watchlist dari user yang login. R = terdapat , A = tidak terdapat
        public string IsOnWatchList { get; set; }
        public bool IsModalDialog { get; set; } = false;
    }

    public class ToogleWatchListViewModel
    {
        public string StockID { get; set; }
        public string Action { get; set; }
    }
}
