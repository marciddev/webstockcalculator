using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCalculator
{
    public class YahooModel
    {
        public String symbol { get; set; }
        public List<historicalObject> historical { get; set; }
    }
    public class historicalObject
    {
        public String date { get; set; }
        public String close { get; set; }
    }
}
