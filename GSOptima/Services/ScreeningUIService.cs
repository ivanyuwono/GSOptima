using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSOptima.Services
{
    public class ScreeningUIService
    {
        public string DisplayLastTrendBar(string last)
        {
            string[] part = last.Split(';');
            if (last.Contains("H") && last.Contains("L"))
            {
                //return part[1].ToString() + " Green " + part[3].ToString() + " Red ";
                return "Green Red";
            }
            else if (last.Contains("H"))
            {
                //return part[1].ToString() + " Green";
                return "Green";
            }
            else if (last.Contains("L"))
            {
                //return part[1].ToString() + " Red";
                return "Red";
            }
            else
                return "";


        }
        public string DisplayTradingPlanColor(string plan)
        {
            if (plan == "Buy")
                //return "LightGreen";
                return "tradingplangreen";
            else if (plan == "Hold")
                //return "LightSkyBlue";
                return "tradingplanblue";
            else if (plan == "Watch")
                //return "Yellow";
                return "tradingplanyellow";
            else if (plan == "Sell")
                //return "IndianRed";
                return "tradingplanred";
            else
                //return "White";
                return "tradingplanwhite";

        }


        

        public string DisplayBigWave(bool bigwave)
        {
            if (bigwave)
                return "Detected";
            else
                return "";
        }
        public string DisplayBigWaveColor(bool bigwave)
        {
            if (bigwave)
                return "bwdetected";
            else
                return "bwnotdetected";
        }


       

        public string DisplayRiskColor(string risk)
        {
            if (risk == "1")
                //return "LightSkyBlue";
                return "risklow";
            else if (risk == "2")
                //return "LightGreen";
                return "riskmedium";
            else if (risk == "3")
                //return "IndianRed";
                return "riskhigh";
            else
                //return "White";
                return "risknotavailable";
        }

        public string DisplayRiskLevel(string risk)
        {
            if (risk == "1")
                return "Low";
            else if (risk == "2")
                return "Medium";
            else if (risk == "3")
                return "High";
            else
                return "N/A";
        }

        public string DisplayNormalRangeColor(string direction)
        {
            if (direction == "U")
                //return "IndianRed";
                return "normalrangeabove";
            else if (direction == "D")
                //return "LightGreen";
                return "normalrangebelow";
            else
                //return "White";
                return "normalrangenone";
        }

        public string DisplayNormalRange(string direction)
        {
            if (direction == "U")
                return "Above Normal";
            else if (direction == "D")
                return "Below Normal";
            else
                return "";
        }

        public string DisplayGSLineDirection(string direction)
        {
            if (direction == "U")
                return "Positive";
            else if (direction == "D")
                return "Negative";
            else
                return "Neutral";
        }
        public string DisplayGSLineDirectionColor(string direction)
        {
            if (direction == "U")
                //return "LightGreen";
                return "gslineup";
            else if (direction == "D")
                //return "IndianRed";
                return "gslinedown";
            else
                //return "White";
                return "gslineneutral";
        }
        public string DisplayTrend(string trend)
        {

            if (trend == "1")
                return "Strong Up";
            else if (trend == "2")
                return "Uptrend";
            else if (trend == "3")
                return "Uptrend";
            else if (trend == "4")
                return "Neutral";
            else if (trend == "5")
                return "Downtrend";
            else if (trend == "6")
                return "Strong Down";
            else
                return "N/A";
        }

        public string DisplayTrendColor(string trend)
        {

            if (trend == "1")
                //return "LightSkyBlue";
                return "trendcolorstrongup";
            else if (trend == "2")
                //return "LightGreen";
                return "trendcolorup";
            else if (trend == "3")
                //return "LightGreen";
                return "trendcolorup";
            else if (trend == "4")
                //return "White";
                return "trendcolorneutral";
            else if (trend == "5")
                //return "IndianRed";
                return "trendcolordown";
            else if (trend == "6")
                //return "Firebrick";
                return "trendcolorstrongdown";
            else
                //return "White";
                return "trendcolornone";
        }

        public string DisplayTrendFontColor(string trend)
        {

            if (trend == "1")
                return "White";
            else if (trend == "2")
                return "White";
            else if (trend == "3")
                return "White";
            else if (trend == "4")
                return "Black";
            else if (trend == "5")
                return "White";
            else if (trend == "6")
                return "White";
            else
                return "Black";
        }

        public string DisplayBuyLimitColor(string plan)
        {

            if (plan == "Buy")
                //return "LightGreen";
                return "buylimitgreen";
            else if (plan == "Hold")
                //return "White";
                return "buylimitwhite";
            else if (plan == "Watch")
                //return "White";
                return "buylimitwhite";
            else
                //return "White";
                return "buylimitwhite";
        }

        public string DisplayCloseToResistColor(Decimal? percentage)
        {
            if (percentage == null)
                //return "White";
                return "closetoresistwhite";
            if (percentage > -3 && percentage <= (decimal)0.0)
                //return "LightSkyBlue";
                return "closetoresistblue";
            else if (percentage > 0 && percentage <= (decimal)3.0)
                //return "LightGreen";
                return "closetoresistgreen";
            else
                //return "White";
                return "closetoresistwhite";

        }

        public string DisplayCloseToSupportColor(Decimal? percentage)
        {
            if (percentage == null)
                //return "White";
                return "closetosupportwhite";
            if (percentage >= 0 && percentage <= (decimal)3.0)
                //return "IndianRed";
                return "closetosupportred";
            else
                //return "White";
                return "closetosupportwhite";
        }

        public string DisplayLastTrendBarColor(string last)
        {
            string[] part = last.Split(';');
            if (last.Contains("H") && last.Contains("L"))
            {
                //return part[1].ToString() + " Green " + part[3].ToString() + " Red ";
                //return "LightGreen";
                return "lasttrendgreen";
            }
            else if (last.Contains("H"))
            {
                //return part[1].ToString() + " Green";
                //return "LightGreen";
                return "lasttrendgreen";
            }
            else if (last.Contains("L"))
            {
                //return part[1].ToString() + " Red";
                //return "Firebrick";
                return "lasttrendred";
            }
            else
                return "";

        }

       
    }
}
