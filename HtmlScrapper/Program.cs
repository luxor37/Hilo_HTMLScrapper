using System.Net;

namespace HtmlScrapper
{
    public class Scrapper
    {
        private const string file = "searchHistory.csv";
        private enum Choices
        {
            History, Search, ClearHistory, Quit
        }

        public static void Main()
        {
            Choices choice;
            while ((choice = Menu()) != Choices.Quit)
            {
                switch (choice)
                {
                    case Choices.History:
                        ShowHistory();
                        break;
                    case Choices.Search:
                        Search();
                        break;
                    case Choices.ClearHistory:
                        var sr = File.Create(file);
                        sr.Close();
                        Console.WriteLine("History Cleared");
                        break;
                }
            }
        }

        private static Choices Menu()
        {
            var choice = "";
            while (choice != "1" && choice != "2" && choice != "3" && choice != "4")
            {
                Console.Write("\n---\n1. Consult history\n2. Search\n3. Clear History\n4. Quit\nEnter your choice: ");
                choice = Console.ReadLine();

                if (choice != "1" && choice != "2" && choice != "3" && choice != "4")
                    Console.WriteLine("Invalid choice");
                else
                    Console.WriteLine("\n");
            }

            return choice switch
            {
                "1" => Choices.History,
                "2" => Choices.Search,
                "3" => Choices.ClearHistory,
                "4" => Choices.Quit,
                _ => Choices.Search,
            };
        }

        private static void ShowHistory()
        {
            using StreamReader sr = File.OpenText(file);

            var s = sr.ReadLine();

            if (s == null)
                Console.WriteLine("No result");
            else
            {
                Console.WriteLine("Previous search results:");
                do
                {
                    var result = s.Split(",");
                    Console.WriteLine("'" + result[1] + "' appeared " + result[2] + " times at '" + result[0] + "' on " + result[3]);
                }
                while ((s = sr.ReadLine()) != null);
            }
        }

        private static void Search()
        {
            var url = GetUrl();

            var word = GetWord();

            var response = CallUrl(url).Result.ToLower();

            var count = response.Split(word).Length - 1;

            Console.WriteLine("The word appears " + count + " times in the source code.");

            SaveSearch(url, word, count);
        }

        private static void SaveSearch(string url, string word, int count)
        {
            var date = DateTime.Now;
            var line = $"{url},{word},{count},{date:MMMM dd yyyy}";

            using StreamWriter sw = File.AppendText(file);

            sw.WriteLine(line);
        }

        private static string GetUrl()
        {
            var url = "";
            while (!ValidateUrl(url))
            {
                Console.Write("Enter a URL: ");
                url = Console.ReadLine();

                if (string.IsNullOrEmpty(url))
                    url = "https://www.hiloenergie.com/fr-ca/";

                if (!url.Contains("https://"))
                {
                    url = "https://" + url;
                }
            }
            return url;
        }

        private static string GetWord()
        {
            var word = "";
            while (string.IsNullOrEmpty(word))
            {
                Console.Write("Enter a word to find: ");
                word = Console.ReadLine();
            }
            return word.ToLower();
        }

        private static bool ValidateUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        //https://www.scrapingbee.com/blog/web-scraping-csharp/
        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }
    }
}