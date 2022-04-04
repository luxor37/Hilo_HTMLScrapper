using System.Net;

namespace HtmlScrapper
{
    public class Scrapper
    {
        private const string file = "searchHistory.csv";
        private enum Choices
        {
            Invalid, History, Search, ClearHistory, Quit
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
                        File.Create(file).Close();
                        Console.WriteLine("History Cleared");
                        break;
                }
            }
        }

        private static Choices Menu()
        {
            Choices choice = Choices.Invalid;
            while (choice == Choices.Invalid)
            {
                Console.Write("\n---\n1. Consult history\n2. Search\n3. Clear History\n4. Quit\nEnter your choice: ");
                choice = Console.ReadLine() switch
                {
                    "1" => Choices.History,
                    "2" => Choices.Search,
                    "3" => Choices.ClearHistory,
                    "4" => Choices.Quit,
                    _ => Choices.Invalid,
                };

                if (choice == Choices.Invalid)
                    Console.WriteLine("Invalid choice");
                else
                    Console.WriteLine("\n");
            }

            return choice;
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
                    Console.WriteLine($"'{result[1]}' appeared {result[2]} times at '{result[0]}' on {result[3]}");
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

            Console.WriteLine($"The word appears {count} times in the source code.");

            //Saving to search history file
            SaveSearch(url, word, count);

            var yOrN = "";
            while(yOrN != "y" && yOrN != "n")
            {
                Console.WriteLine("Do you want to save this serach? (y/n):");
                yOrN = Console.ReadLine();
            }

            if(yOrN == "y")
            {
                var location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\search.csv";
                SaveSearch(url, word, count, location);
                Console.WriteLine($"This search was saved at '{location}'");
            }
        }

        private static void SaveSearch(string url, string word, int count, string location = "")
        {
            var line = $"'{url}',{word},{count},{DateTime.Now:MMMM dd yyyy}";

            if (string.IsNullOrEmpty(location))
            {
                using StreamWriter sw = File.AppendText(file);

                sw.WriteLine(line);
            }
            else
                File.WriteAllText(location, line);
        }

        private static string GetUrl()
        {
            var url = "";
            while (!ValidateUrl(url))
            {
                Console.Write("Enter a URL: ");
                url = Console.ReadLine();

                if (string.IsNullOrEmpty(url))
                {
                    url = "";
                    continue;
                }

                if (!url.Contains("https://"))
                    url = "https://" + url;
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