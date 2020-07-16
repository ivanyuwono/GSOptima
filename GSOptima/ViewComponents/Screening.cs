using GSOptima.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSOptima.ViewComponents
{
    public static class Screening
    {
        public static IEnumerable<StockPrice> ScreeningBuy(IEnumerable<StockPrice> raw)
        {
            //return raw.Where(x => x.Close > x.Resistance && x.Close <= (decimal)1.03 * x.Resistance && x.High > x.Resistance);
            //IVY 20181008
            return raw.Where(x => x.High > x.Resistance && x.Close <= (decimal)1.03 * x.Resistance && x.High > x.Resistance);
        }
        public static IEnumerable<StockPrice> ScreeningHold(IEnumerable<StockPrice> raw)
        {
            return raw.Where(x => x.Close > (decimal)1.03 * x.Resistance);
        }
        public static IEnumerable<StockPrice> ScreeningWatch(IEnumerable<StockPrice> raw)
        {
            return raw.Where(x=>x.Close <= x.Resistance && x.Close >= (decimal)0.97 * x.Resistance);
        }
        public static IEnumerable<StockPrice> ScreeningSell(IEnumerable<StockPrice> raw)
        {
            return raw.Where(x => x.Low < x.Support && x.Low >= (decimal)0.925 * x.Support);
        }
        public static IEnumerable<StockPrice> ScreeningAbnormal(IEnumerable<StockPrice> raw)
        {
            return raw.Where(x => x.Close > (decimal)1.025 * x.BBUpper || x.Close < (decimal)0.975 * x.BBLower);
        }
        public static IEnumerable<StockPrice> ScreeningBigwave(IEnumerable<StockPrice> raw)
        {
            return raw.Where(x => x.BigWave > x.AverageBigWave);
        }

        public static string DetermineTradingPlan(StockPrice x)
        {

            //if (x.High > x.Resistance && x.High <= (decimal)1.03 * x.Resistance)   //If Highest Price > Resist (0% < X <= +3%) | X = Closing price
            if (x.Close > x.Resistance && x.Close <= (decimal)1.03 * x.Resistance && x.High > x.Resistance)
            {
                return "Buy";
            }
            else if (x.Close > (decimal)1.03 * x.Resistance)  //If Closing Price > Resist (X > +3%) | X = Closing price
            {
                return "Hold";
            }
            else if (x.Close <= x.Resistance && x.Close >= (decimal)0.97 * x.Resistance)  //If Closing Price < Resist (-3% <= X <= 0% ) | X = Closing price
            {

                return "Watch";
            }
            else if (x.Low < x.Support && x.Low >= (decimal)0.925 * x.Support)  //If Lowest Price < Support ( -7.5% <= X < 0%)
            {

                return "Sell";
            }
            else
                return "";
        }
        public static string DetermineRiskProfile(StockPrice x)
        {
            if (x.Support != null && x.Resistance != null)
            {
                //if (x.Close >= x.Support)  //bila closing di atas support
                //{
                var closeToS = (decimal)(x.Close - x.Support) / x.Support * 100;
                var closeToR = (decimal)(x.Close - x.Resistance) / x.Resistance * 100;
                var risk = (Math.Abs((decimal)closeToS) + Math.Abs((decimal)closeToR)) / 2;
                if (risk >= 0 && risk <= (decimal)2.0)   //0% <= X <= 2%
                    return "1";  //low
                else if (risk > 2 && risk <= (decimal)3.5)    //2% < X <= 3.5%
                {
                    return "2";  //medium
                }
                else            //  3.5% < X
                    return "3";    //high   
                //}
                //else     //bila tidak closing di atas support
                //{
                //   return "";
                //}
            }
            else
                return "";

        }
        public static string DetermineTrend(StockPrice x)
        {

            if (x.MA20 == null || x.MA60 == null)
                return "N/A";
            else
            {
                if (x.Close >= x.MA20 && x.MA20 >= x.MA60)
                    return "1"; //blue
                else if (x.MA20 >= x.Close && x.Close >= x.MA60)
                    return "2"; //green
                else if (x.MA20 >= x.MA60 && x.MA60 >= x.Close)
                    return "5"; //indian red
                else if (x.MA60 >= x.MA20 && x.MA20 >= x.Close)
                    return "6";  //dark red
                else if (x.MA60 >= x.Close && x.Close >= x.MA20)
                    return "4"; //white
                else if (x.Close >= x.MA60 && x.MA60 >= x.MA20)
                    return "3"; //green
                else
                    return "N/A";
            }
        }
        public static string DetermineTrendByWord(StockPrice x)
        {

            if (x.MA20 == null || x.MA60 == null)
                return "N/A";
            else
            {
                if (x.Close >= x.MA20 && x.MA20 >= x.MA60)
                    return "Strong Uptrend"; //blue
                else if (x.MA20 >= x.Close && x.Close >= x.MA60)
                    return "Uptrend"; //green
                else if (x.MA20 >= x.MA60 && x.MA60 >= x.Close)
                    return "Downtrend"; //indian red
                else if (x.MA60 >= x.MA20 && x.MA20 >= x.Close)
                    return "Strong Downtrend";  //dark red
                else if (x.MA60 >= x.Close && x.Close >= x.MA20)
                    return "Neutral"; //white
                else if (x.Close >= x.MA60 && x.MA60 >= x.MA20)
                    return "Uptrend"; //green
                else
                    return "N/A";
            }


        }

        public static string DetermineNormalRange(StockPrice x)
        {

            if (x.Close > (decimal)1.025 * x.BBUpper)
                return "U";
            else if (x.Close < (decimal)0.975 * x.BBLower)
                return "D";
            else
                return "N";
        }

        public static bool DetermineBigWave(StockPrice x)
        {
            if (x.BigWave > x.AverageBigWave)
            {
                return true;
            }
            else
                return false;
        }
        public static Decimal? DetermineBuyLimit(StockPrice x)
        {

            var tradingplan = DetermineTradingPlan(x);

            if (x.Resistance != null)
            {
                if (tradingplan == "Buy" || tradingplan == "Hold" || tradingplan == "Watch")
                    return Math.Round(((decimal)1.03 * (decimal)x.Resistance));   //3% Up from Resist
                else
                    return null;
            }
            else
                return null;
        }
        
    }
}
