using System;
using System.ComponentModel.DataAnnotations;


namespace GSOptima.ViewModels
{
    public class StockScreeningViewModel
    {
        public StockScreeningViewModel()
        {

        }
        public string StockID { get; set; }
        public bool BigWave { get; set; }
        public string RiskProfile { get; set; }
        public string TradingPlan { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        public DateTime Date { get; set; }

        public Decimal? Open { get; set; }
        public Decimal? High { get; set; }
        public Decimal? Low { get; set; }
        public Decimal? Close { get; set; }
        public Decimal? CloseToSupport { get; set; }
        public Decimal? CloseToResistance { get; set; }
        public Decimal? Support { get; set; }
        public Decimal? Resistance { get; set; }
        public string Trend { get; set; }
        public string GSLine { get; set; }
        public string GSLineDirection { get; set; }
        public string NormalRange { get; set; }

        //[DataType(DataType.Currency)]
        //[DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public Decimal? BuyLimit { get; set; }
        public string LastTrendBar { get; set; }
    }

  }
