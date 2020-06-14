using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebCalculator.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace WebCalculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.low = 0;
            ApiModel m = new ApiModel()
            {
                IVE = "0",
                IVV = "0",
                IVW = "0",
                IJJ = "0",
                IJH = "0",
                IJK = "0",
                IWN = "0",
                IWM = "0", 
                IWO = "0"
            };
            

            return View(m);
        }
        [HttpPost("/{upperInt}/{lowerInt}")]
        public async Task<IActionResult> PostRequest(int upperInt, int lowerInt)
        {
            Console.WriteLine("here");
            static async Task<YahooModel> grabData(String s)
            {
                using (var client = new HttpClient())
                {
                    Console.WriteLine("starting" + s);
                    String url = "https://financialmodelingprep.com/api/v3/historical-price-full/" + s.ToUpper() + "?apikey=4992d801ad7f4e2b40e12d596b5dbcae&from=1900-10-10";
                    var response = await client.GetStringAsync(url);
                    YahooModel returnObj = JsonConvert.DeserializeObject<YahooModel>(response);
                    Console.WriteLine("ending" + s);
                    return returnObj;
                }
            }
            var IVEt = grabData("ive");
            var IVVt = grabData("ivv");
            var IVWt = grabData("ivw");
            var IJJt = grabData("ijj");
            var IJHt = grabData("ijh");
            var IJKt = grabData("ijk");
            var IWNt = grabData("iwn");
            var IWMt = grabData("iwm");
            var IWOt = grabData("iwo");
            
            await Task.WhenAll(IVEt, IVVt, IVWt, IJJt, IJHt, IJKt, IWNt, IWMt, IWOt);
            YahooModel IVE = IVEt.Result;
            YahooModel IVV = IVVt.Result;
            YahooModel IVW = IVWt.Result;
            YahooModel IJJ = IJJt.Result;
            YahooModel IJH = IJHt.Result;
            YahooModel IJK = IJKt.Result;
            YahooModel IWN = IWNt.Result;
            YahooModel IWM = IWMt.Result;
            YahooModel IWO = IWOt.Result;


            String percentChange(YahooModel m)
            {

                static DateTime EasterDate(int Year)
                {

                    // Gauss Calculation
                    ////////////////////

                    int Month = 3;

                    // Determine the Golden number:
                    int G = Year % 19 + 1;

                    // Determine the century number:
                    int C = Year / 100 + 1;

                    // Correct for the years who are not leap years:
                    int X = (3 * C) / 4 - 12;

                    // Mooncorrection:
                    int Y = (8 * C + 5) / 25 - 5;

                    // Find sunday:
                    int Z = (5 * Year) / 4 - X - 10;

                    // Determine epact(age of moon on 1 januari of that year(follows a cycle of 19 years):
                    int E = (11 * G + 20 + Y - X) % 30;
                    if (E == 24) { E++; }
                    if ((E == 25) && (G > 11)) { E++; }

                    // Get the full moon:
                    int N = 44 - E;
                    if (N < 21) { N = N + 30; }

                    // Up to sunday:
                    int P = (N + 7) - ((Z + N) % 7);

                    // Easterdate: 
                    if (P > 31)
                    {
                        P = P - 31;
                        Month = 4;
                    }
                    return new DateTime(Year, Month, P);
                }


                static double CalculateChange(double previous, double current)
                {
                    if (previous == 0)
                        return 999999.99;

                    var change = current - previous;
                    return (double)change / previous;
                }


                static string DoubleToPercentageString(double d)
                {
                    return "" + (d * 100).ToString("#.##") + "%";
                }


                DateTime current = DateTime.Now;
                DateTime comparingTo = DateTime.Now;


                if (lowerInt != 0)
                {
                    comparingTo = comparingTo.AddDays(lowerInt * -1);
                }
                DateTime comparingFrom = current.AddDays(upperInt * -1);
                Console.WriteLine(comparingFrom);
                Console.WriteLine(comparingTo);

                //NEW YEARS
                if (comparingTo.ToString("MM-dd") == "01-01")
                {
                    comparingTo = comparingTo.AddDays(-1);
                }
                if (comparingFrom.ToString("MM-dd") == "01-01")
                {
                    comparingFrom = comparingFrom.AddDays(-1);
                }



                //MLKJ
                if (comparingTo.ToString("MM") == "01" && comparingTo.DayOfWeek == DayOfWeek.Monday)
                {
                    if (comparingTo.Day > 14 && comparingTo.Day < 22)
                    {
                        comparingTo = comparingTo.AddDays(-1);
                    }
                }
                if (comparingFrom.ToString("MM") == "01" && comparingFrom.DayOfWeek == DayOfWeek.Monday)
                {
                    if (comparingFrom.Day > 14 && comparingFrom.Day < 22)
                    {
                        comparingFrom = comparingFrom.AddDays(-1);
                    }
                }

                //presidents day
                if (comparingTo.ToString("MM") == "02" && comparingTo.DayOfWeek == DayOfWeek.Monday)
                {
                    if (comparingTo.Day > 14 && comparingTo.Day < 22)
                    {
                        comparingTo = comparingTo.AddDays(-1);
                    }
                }
                if (comparingFrom.ToString("MM") == "02" && comparingFrom.DayOfWeek == DayOfWeek.Monday)
                {
                    if (comparingFrom.Day > 14 && comparingFrom.Day < 22)
                    {
                        comparingFrom = comparingFrom.AddDays(-1);
                    }
                }

                //easter


                if (comparingTo.Date == EasterDate(comparingTo.Year))
                {
                    comparingTo = comparingTo.AddDays(-2);//probably should double check this.
                }
                if (comparingFrom.Date == EasterDate(comparingFrom.Year))
                {
                    comparingFrom = comparingFrom.AddDays(-2);
                }



                //good friday
                if (comparingTo.ToString("yyyy-MM-dd") == EasterDate(comparingTo.Year).AddDays(-2).ToString("yyyy-MM-dd") || comparingTo.ToString("yyyy-MM-dd") == EasterDate(comparingTo.Year).AddDays(-1).ToString("yyyy-MM-dd"))
                {
                    comparingTo = comparingTo.AddDays(-2);
                }
                if (comparingFrom.ToString("yyyy-MM-dd") == EasterDate(comparingFrom.Year).AddDays(-2).ToString("yyyy-MM-dd") || comparingFrom.ToString("yyyy-MM-dd") == EasterDate(comparingFrom.Year).AddDays(-1).ToString("yyyy-MM-dd"))
                {
                    comparingFrom = comparingFrom.AddDays(-2);
                }

                //memorial day

                if (comparingTo.DayOfWeek == DayOfWeek.Monday && comparingTo.ToString("MM") == "05")
                {
                    if (comparingTo.Day > 24 && comparingTo.Day <= 31)
                    {
                        comparingTo = comparingTo.AddDays(-3);
                    }
                }
                if (comparingFrom.DayOfWeek == DayOfWeek.Monday && comparingFrom.ToString("MM") == "05")
                {
                    if (comparingFrom.Day > 24 && comparingFrom.Day <= 31)
                    {
                        comparingFrom = comparingFrom.AddDays(-3);
                    }
                }

                //independence day



                if (comparingTo.ToString("MM-dd") == "07-04")
                {
                    if (comparingTo.DayOfWeek == DayOfWeek.Saturday)
                    {
                        comparingTo = comparingTo.AddDays(-2);
                    }
                    if (comparingTo.DayOfWeek == DayOfWeek.Sunday)
                    {
                        comparingTo = comparingTo.AddDays(-2);
                    }
                    else
                    {
                        comparingTo = comparingTo.AddDays(-1);
                    }
                }
                if (comparingTo.ToString("MM-dd") == "07-03")
                {
                    if (comparingTo.AddDays(1).DayOfWeek == DayOfWeek.Saturday)
                    {
                        comparingTo = comparingTo.AddDays(-1);
                    }
                }
                if (comparingTo.ToString("MM-dd") == "07-05")
                {
                    if (comparingTo.AddDays(-1).DayOfWeek == DayOfWeek.Sunday)
                    {
                        comparingTo = comparingTo.AddDays(-3);
                    }
                }


                if (comparingFrom.ToString("MM-dd") == "07-04")
                {
                    if (comparingFrom.DayOfWeek == DayOfWeek.Saturday)
                    {
                        comparingFrom = comparingFrom.AddDays(-2);
                    }
                    if (comparingFrom.DayOfWeek == DayOfWeek.Sunday)
                    {
                        comparingFrom = comparingFrom.AddDays(-2);
                    }
                    else
                    {
                        comparingFrom = comparingFrom.AddDays(-1);
                    }
                }
                if (comparingFrom.ToString("MM-dd") == "07-03")
                {
                    if (comparingFrom.AddDays(1).DayOfWeek == DayOfWeek.Saturday)
                    {
                        comparingFrom = comparingFrom.AddDays(-1);
                    }
                }
                if (comparingFrom.ToString("MM-dd") == "07-05")
                {
                    if (comparingFrom.AddDays(-1).DayOfWeek == DayOfWeek.Sunday)
                    {
                        comparingFrom = comparingFrom.AddDays(-3);
                    }
                }




                //labor day

                if (comparingTo.ToString("MM") == "09" && comparingTo.DayOfWeek == DayOfWeek.Monday)
                {
                    if (comparingTo.Day >= 1 && comparingTo.Day <= 7)
                    {
                        comparingTo = comparingTo.AddDays(-3);
                    }
                }
                if (comparingFrom.ToString("MM") == "09" && comparingFrom.DayOfWeek == DayOfWeek.Monday)
                {
                    if (comparingFrom.Day >= 1 && comparingFrom.Day <= 7)
                    {
                        comparingFrom = comparingFrom.AddDays(-3);
                    }
                }

                //thanksgiving

                if (comparingTo.ToString("MM") == "11" && comparingTo.DayOfWeek == DayOfWeek.Thursday)
                {
                    if (comparingTo.Day >= 22 && comparingTo.Day <= 28)
                    {
                        comparingTo = comparingTo.AddDays(-1);
                    }
                }
                if (comparingFrom.ToString("MM") == "11" && comparingFrom.DayOfWeek == DayOfWeek.Thursday)
                {
                    if (comparingFrom.Day >= 22 && comparingFrom.Day <= 28)
                    {
                        comparingFrom = comparingFrom.AddDays(-1);
                    }
                }

                //check christmas

                if (comparingTo.ToString("MM") == "12" && comparingTo.ToString("dd") == "25")
                {
                    comparingTo = comparingTo.AddDays(-1);
                }
                if (comparingFrom.ToString("MM") == "12" && comparingFrom.ToString("dd") == "25")
                {
                    comparingFrom = comparingFrom.AddDays(-1);
                }


                //check weekend

                if (comparingTo.DayOfWeek == DayOfWeek.Saturday)
                {
                    comparingTo = comparingTo.AddDays(-1);
                }
                if (comparingTo.DayOfWeek == DayOfWeek.Sunday)
                {
                    comparingTo = comparingTo.AddDays(-2);
                }
                if (comparingFrom.DayOfWeek == DayOfWeek.Saturday)
                {
                    comparingFrom = comparingFrom.AddDays(-1);
                }
                if (comparingFrom.DayOfWeek == DayOfWeek.Sunday)
                {
                    comparingFrom = comparingFrom.AddDays(-2);
                }




                double comparingToClose = 0.00;
                double comparingFromClose = 0.00;
                double difference = 999999.99;
                Boolean doesExist1 = true;
                Boolean doesExist2 = true;
                while (difference == 999999.99)
                {

                    var historicalDates = m.historical.Select(x=>(Date: x.date, Close: x.close)).ToList();
                    while(doesExist1) {
                        for (int i = 0; i < historicalDates.Count; i++)
                        {
                            if (historicalDates[i].Date == comparingTo.ToString("yyyy-MM-dd"))
                            {
                                doesExist1 = false;
                                comparingToClose = double.Parse(historicalDates[i].Close);
                            } 
                        }
                        if(doesExist1 == true) {
                            comparingTo = comparingTo.AddDays(-1);
                        }
                    }
                    while(doesExist2) {
                        for (int i = 0; i < historicalDates.Count; i++)
                        {
                            if (historicalDates[i].Date == comparingFrom.ToString("yyyy-MM-dd"))
                            {
                                doesExist2 = false;
                                comparingFromClose = double.Parse(historicalDates[i].Close);
                            }
                        }
                        if(doesExist2 == true) {
                            comparingFrom = comparingFrom.AddDays(-1);
                        }
                    }

                    Console.WriteLine("inside while loop.");

                    difference = CalculateChange(comparingFromClose, comparingToClose);
                }
                String percentageChange = DoubleToPercentageString(difference);
                return percentageChange;

            }
            Console.WriteLine("end");

            ApiModel model = new ApiModel();
            model.IVE = percentChange(IVE);
            model.IVV = percentChange(IVV);
            model.IVW = percentChange(IVW);
            model.IJJ = percentChange(IJJ);
            model.IJH = percentChange(IJH);
            model.IJK = percentChange(IJK);
            model.IWN = percentChange(IWN);
            model.IWM = percentChange(IWM);
            model.IWO = percentChange(IWO);
            ViewBag.up = upperInt;
            Console.WriteLine(model.IVE + " " + model.IVV + " " + model.IVW + " " + model.IJJ + " " + model.IJH + " " + model.IJK + " " + model.IWN + " " + model.IWM + " " + model.IWO);
            ViewBag.low = lowerInt;
            ViewBag.up = upperInt;
            ViewBag.hasClicked = "true";
            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
