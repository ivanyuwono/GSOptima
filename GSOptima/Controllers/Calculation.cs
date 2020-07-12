using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using GSOptima.Data;
using GSOptima.Models;
using System.Reflection;

namespace GSOptima.Controllers
{
    public static class Calculation
    {

        #region Formula 

        public static Decimal? SD(int i, StockPrice[] data, int days)
        {

            if (i < days - 1)
            {
                return null;
            }
            double total = 0;
            Decimal? avg = data[i].MA20;

            for (int x = i; x > i - days; x--)
            {
                //total += data[x].Close;
                total += Math.Pow(((double)data[x].Close - (double)avg), 2);
            }
            return (decimal)Math.Sqrt(total / days);
        }
        public static Decimal? MA(int i, StockPrice[] data, int days)
        {
            if (i < days - 1)
            {
                return null;
            }

            Decimal total = 0;

            for (int x = i; x > i - days; x--)
            {
                total += data[x].Close;
            }
            return (total / days);

        }

        public static Decimal? MAofMACD(int i, StockPrice[] data, int days)
        {
            //*perhatikan ada yg salah
            if (i < days - 1 + 26 - 1)
            {
                return null;
            }

            Decimal total = 0;
            //var x = i, counter_days=0, valid = false;


            for (int x = i; x > i - days; x--)
            {
                total += (Decimal)data[x].MACD;
            }
            return (total / days);


        }



        public static Decimal? Resistance(int i, StockPrice[] data)
        {

            var ok = false; var ok2 = false; var ok3 = false; var ok4 = false;
            if (i > 0)
            {
                if (data[i - 1].High <= data[i].High)
                {
                    ok = true;

                }
            }

            if (i + 1 <= data.Length - 1)
            {
                if (data[i + 1].High <= data[i].High)
                {
                    ok2 = true;

                }
            }
            if (i + 2 <= data.Length - 1)
            {
                if (data[i + 2].High <= data[i].High)
                {
                    ok3 = true;

                }
            }
            if (i + 3 <= data.Length - 1)
            {
                if (data[i + 3].High <= data[i].High)
                {
                    ok4 = true;

                }
            }

            if (ok == true && ok2 == true && ok3 == true && ok4 == true)
            {
                return data[i].High;


            }
            else
            {
                if (i > 0)
                {
                    return data[i - 1].Resistance;

                }
                else
                {
                    return null;
                }
            }
        }

        public static Decimal? Support(int i, StockPrice[] data)
        {
            var ok = false; var ok2 = false; var ok3 = false; var ok4 = false;
            if (i > 0)
            {
                if (data[i - 1].Low >= data[i].Low)
                {
                    ok = true;

                }
            }

            if (i + 1 <= data.Length - 1)
            {
                if (data[i + 1].Low >= data[i].Low)
                {
                    ok2 = true;

                }
            }
            if (i + 2 <= data.Length - 1)
            {
                if (data[i + 2].Low >= data[i].Low)
                {
                    ok3 = true;

                }
            }
            if (i + 3 <= data.Length - 1)
            {
                if (data[i + 3].Low >= data[i].Low)
                {
                    ok4 = true;

                }
            }

            if (ok == true && ok2 == true && ok3 == true && ok4 == true)
            {
                return data[i].Low;


            }
            else
            {
                if (i > 0)
                {
                    return data[i - 1].Support;

                }
                else
                {

                    return null;
                }
            }

        }
        public static Decimal? AverageBigWave(int i, StockPrice[] data, int month)
        {

            //AverageBigWave menghitung rata-rata bigwave untuk N month sebelumnya termasuk tanggal hari ini

            var startDate = data[i].Date;
            var endDate = startDate.AddMonths(-month);
            var thresholdDate = endDate.AddDays(+1);

            var filter = data.Where(c => c.Date >= endDate && c.Date <= startDate);
            //if (data.Count(c => c.Date <= endDate) > 0)
            if (filter.Count() > 0)
            {
                return filter.Average(m => m.BigWave);
            }
            else
            {
                return null;
            }

        }
        public static Object HighestLowest(int i, StockPrice[] data, int month)
        {

            //HighestLowest menghitung nilai tertinggi / terendah selama N month sebelum tapi tidak termasuk tanggal hari ini
        
            var startDate = data[i].Date;
            var endDate = startDate.AddMonths(-month);
            var thresholdDate = endDate.AddDays(+1);

            var filter = data.Where(c => c.Date >= endDate && c.Date < startDate);
            //if (data.Count(c => c.Date <= endDate) > 0)  //data cukup untuk analisa
            //{
                if (filter.Count() > 0)
                {
                        //*var filter = data.Where(c => c.Date >= endDate && c.Date <= startDate);
                    if (filter.Count() > 0)
                        return new { Lowest = filter.Min(d => d.Low), Highest = filter.Max(d => d.High) };
                    else
                        return new { Lowest = 0, Highest = 99999999 };
                    //return (new HighLow {Lowest = filter.Min(d => d.Close), Highest = filter.Max(d => d.Close) });
                }
                else
                {
                    //return new { Lowest = 1, Highest = 1 };
                    return null;
                }

            //}
            //else
            //{
            //    return null;
            //}
            

        }

        private static Object BreakHighLowCalc(int i, StockPrice[] data)
        {
            dynamic t3 = HighestLowest(i, data, 3);
            dynamic t6 = HighestLowest(i, data, 6);
            dynamic t12 = HighestLowest(i, data, 12);

            var trendHigh = 0; var trendLow = 0;

            if (t3 != null)
                if (data[i].Close > t3.Highest)
                    trendHigh += 1;

            if (t6 != null)
                if (data[i].Close > t6.Highest)
                    trendHigh += 1;
            if (t12 != null)
                if (data[i].Close > t12.Highest)
                    trendHigh += 1;

            if (t3 != null)
                if (data[i].Close < t3.Lowest)
                {
                    //alert (data[i].date + " " + 3);
                    trendLow += 1;
                }
            if (t6 != null)
                if (data[i].Close < t6.Lowest)
                {
                    //alert (data[i].date + " " + 6);
                    trendLow += 1;
                }
            if (t12 != null)
                if (data[i].Close < t12.Lowest)
                {
                    //alert (data[i].date + " " + 12);
                    trendLow += 1;
                }
            return new { TrendHigh = trendHigh, TrendLow = trendLow };
            //return (new int[2] { trendHigh, trendLow });
        }

        public static int TrendHigh(int i, StockPrice[] data, Decimal? t3, Decimal? t6, Decimal? t12)
        {

            var trendHigh = 0;

            if (t3 != null)
                if (data[i].High > t3)
                    trendHigh += 1;
            if (t6 != null)
                if (data[i].High > t6)
                    trendHigh += 1;
            if (t12 != null)
                if (data[i].High > t12)
                    trendHigh += 1;

            return trendHigh;
        }

        public static int TrendLow(int i, StockPrice[] data, Decimal? t3, Decimal? t6, Decimal? t12)
        {

            var trendLow = 0;

            if (t3 != null)
                if (data[i].Low < t3)
                    trendLow += 1;
            if (t6 != null)
                if (data[i].Low < t6)
                    trendLow += 1;
            if (t12 != null)
                if (data[i].Low < t12)
                    trendLow += 1;

            return trendLow;
        }

        public static Decimal? EMA(int i, StockPrice[] data, int periode)
        {
            //*perhatikan ada yg salah
            if (i >= 0 && i < periode - 1)
            {
                return null;
            }
            else if (i == periode - 1)
            {
                return MA(i, data, periode);
            }
            else
            {
                var prevEMA = (data[i - 1].GetType().GetProperty("EMA" + periode).GetValue(data[i - 1]));
                if (prevEMA != null)
                {
                    Console.Write(prevEMA);
                    //Double k = (Double)2 / (periode + 1);
                    //return (data[i].close - data[i - 1].EMA12) * k + data[i - 1].EMA12;
                    return (data[i].Close - (Decimal)prevEMA) * ((Decimal)2.0 / (periode + 1)) + (Decimal)prevEMA;
                }
                else
                    return null;
            }
        }

        public static Decimal? SignalLine(int i, StockPrice[] data, int periode)
        {
            //*perhatikan ada yg salah
            if (i >= 0 && i < periode - 1 + 26 - 1)
            {
                return null;
            }
            else if (i == periode - 1 + 26 - 1)
            {
                return MAofMACD(i, data, 9);
            }
            else
            {
                Decimal k = (Decimal)2.0 / (periode + 1);
                return (data[i].MACD - data[i - 1].SignalLine) * k + data[i - 1].SignalLine;
            }



        }
        #endregion


    }
}