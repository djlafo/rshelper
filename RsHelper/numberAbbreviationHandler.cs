using System;
using System.Text.RegularExpressions;

namespace RsHelper
{
    public class numberAbbreviationHandler
    {
        public static string getNumberEstimate(decimal number, int roundTo)
        {

            double temp = Math.Abs((double)number);
            if (temp >= 1000000000)
            {
                temp = (double)Math.Round(Decimal.Divide(number, 1000000000), roundTo);
                return temp + "B";
            }
            else if (temp >= 1000000)
            {
                temp = (double)Math.Round(Decimal.Divide(number, 1000000), roundTo);
                return temp + "M";
            }
            else if (temp >= 1000)
            {
                temp = (double)Math.Round(Decimal.Divide(number, 1000), roundTo);
                return temp + "K";
            }
            else
            {
                return "" + temp;
            }
        }

        public static string replaceAbbreviations(string number)
        {
            return Regex.Replace(number, "\\d+\\.?\\d*[kmbKMB]{1}", (Match m) => { return "" + replaceAbbreviation(m.ToString()); });
        }

        public static decimal replaceAbbreviation(string m)
        {
            string thestring = m.ToString().ToLower();

            decimal result = 0;

            if (thestring.Contains("b")) // 9 digits after period
            {
                thestring = thestring.Replace("b", "");
                result = decimal.Parse(thestring) * 1000000000;
            }
            else if (thestring.Contains("m")) // 6 digits after period
            {
                thestring = thestring.Replace("m", "");
                result = decimal.Parse(thestring) * 1000000;
            }
            else if (thestring.Contains("k")) // 3 digits after period
            {
                thestring = thestring.Replace("k", "");
                result = decimal.Parse(thestring) * 1000;
            } else
            {
                result = decimal.Parse(thestring);
            }


            return result;
        }
    }
}
