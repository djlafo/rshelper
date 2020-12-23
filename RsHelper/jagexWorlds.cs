using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RsHelper
{
    public class jagexWorlds
    {
        public static readonly int[] americanWorlds = { 5, 6, 13, 14, 22, 29, 30, 38, 46, 53, 54, 62, 69, 70, 77, 78, 86 };

        public static string worldToIp(int world)
        {
            return "oldschool" + world + ".runescape.com";
        }

        public static List<string> americanWorldsToIp()
        {
            List<string> temp = new List<string>();
            foreach (int i in americanWorlds)
            {
                temp.Add(worldToIp(i));
            }
            return temp;
        }
    }
}
