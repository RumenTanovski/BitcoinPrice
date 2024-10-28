using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using TT.Api.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace TT.Api.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile uploadedFile)
        {
            string fileName = "portfolio.txt";
            string content = string.Empty;
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                content = OpenFileByName(fileName);
                ViewBag.Message = "Please select a file.";
            }
            else
            {
                using (var reader = new StreamReader(uploadedFile.OpenReadStream(), Encoding.UTF8))
                {
                    content = await reader.ReadToEndAsync();
                    fileName = uploadedFile.FileName;
                }
                SaveFile(fileName, content);
            }

            var coins = new List<CoinModel>();
            decimal totalInitialPrice = 0;
            decimal totalNewPrice = 0;

            CoinApiResponse resultContent = await FetchDataForCoins();

            string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            PrepareDataForView(coins, ref totalInitialPrice, ref totalNewPrice, resultContent, lines);

            ViewBag.Coins = coins;
            ViewBag.TotalInitialPrice = totalInitialPrice;
            ViewBag.TotalNewPrice = totalNewPrice;

            RecordToLogFile(coins);

            return View("Index");
        }

        //TO DO a Service

        private static void PrepareDataForView(List<CoinModel> coins, ref decimal totalInitialPrice, ref decimal totalNewPrice, CoinApiResponse resultContent, string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                // Parse each line: Example input: 10|ETH|123.14
                var data = lines[i].Split('|');
                if (data.Length == 3)
                {
                    var numCoins = Convert.ToDecimal(data[0].Replace(".", ",")); // Current value of the currency
                    var name = data[1]; // Currency type (e.g., ETH)
                    var initialBuyPrice = Convert.ToDecimal(data[2].Replace(".", ",")); // Initial buy price

                    // Set the current value by api.coinlore
                    var newPrice = resultContent.Data.Where(x => x.Symbol == name)
                        .Select(v => v.Price_usd)
                        .FirstOrDefault();

                    // Create a CoinModel to store this information
                    var coin = new CoinModel
                    {
                        NumCoin = numCoins,
                        Name = name,
                        InitialPriceCoin = initialBuyPrice,
                        NewPriceCoin = newPrice
                    };

                    coins.Add(coin);

                    totalInitialPrice += initialBuyPrice * numCoins;
                    totalNewPrice += newPrice * numCoins;
                }
            }
        }

        public static string OpenFileByName(string fileName)
        {
            string filePath = Path.Combine(@"D:\Logs\", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return "File not found.";
            }
            else
            {
                string fileContent = System.IO.File.ReadAllText(filePath);

                return fileContent;
            }

        }

        public static void SaveFile(string fileName, string content)
        {
            string filePath = Path.Combine(@"D:\", fileName);

            try
            {
                System.IO.File.WriteAllText(filePath, content);
                Console.WriteLine("File saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while saving the file: " + ex.Message);
            }
        }

        private static void RecordToLogFile(List<CoinModel> coins)
        {
            // TO DO StringBuilder
            string text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + System.Environment.NewLine;
            text = text + JsonConvert.SerializeObject(coins) + System.Environment.NewLine;

            string folderPath = Path.Combine("D:", "Logs");
            string filePath = Path.Combine(folderPath, "log.txt");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            System.IO.File.AppendAllText(filePath, text);
        }

        private static async Task<CoinApiResponse> FetchDataForCoins()
        {
            var resultContent = new CoinApiResponse();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.coinlore.net/api/tickers/");
                var result = await client.GetAsync("https://api.coinlore.net/api/tickers/");
                result.EnsureSuccessStatusCode();
                string resultContentString = await result.Content.ReadAsStringAsync();
                resultContent = JsonConvert.DeserializeObject<CoinApiResponse>(resultContentString);
            }
            return resultContent;
        }
    }
}
