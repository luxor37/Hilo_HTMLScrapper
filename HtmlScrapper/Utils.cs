using System.Net;

namespace HtmlScrapper
{
    public class Utils
    {
        public enum Choices
        {
            Invalid, History, Search, ClearHistory, Quit
        }

        public static string GetUrl()
        {
            string? url;
            do
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
            while (!ValidateUrl(url));
            return url;
        }

        public static string GetWord()
        {
            var word = "";
            while (string.IsNullOrEmpty(word))
            {
                Console.Write("Enter a word to find: ");
                word = Console.ReadLine();
            }
            return word.ToLower();
        }

        public static bool ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            if (!Uri.IsWellFormedUriString(path, UriKind.Relative) || !Directory.Exists(path))
            {
                Console.WriteLine("This path is invalid");
                return false;
            }

            return true;
        }

        public static bool ValidateUrl(string url)
        {
            var isValid = Uri.IsWellFormedUriString(url, UriKind.Absolute);
            if (!isValid) { 
                Console.WriteLine("This URL is invalid");
                return isValid;
            }

            try
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();
                var response = client.GetAsync(url);

                isValid = response.Result.StatusCode == HttpStatusCode.OK;
            }
            catch(AggregateException ex)
            {
                isValid = false;
            }

            if (!isValid)
                Console.WriteLine("This URL does not exist");

            return isValid;
        }

        //https://www.scrapingbee.com/blog/web-scraping-csharp/
        public static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }
    }
}