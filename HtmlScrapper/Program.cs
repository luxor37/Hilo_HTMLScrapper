using System.Net;
//https://www.scrapingbee.com/blog/web-scraping-csharp/

public class Scrapper
{
    public static void Main()
    {
        var keepGoing = "y";

        while (keepGoing == "y")
        {
            keepGoing = "";
            var url = "";
            while (!ValidateUrl(url))
            {
                Console.Write("Enter a URL: ");
                url = Console.ReadLine();

                if (string.IsNullOrEmpty(url))
                    url = "https://www.hiloenergie.com/fr-ca/";
            }

            var word = "";
            while (string.IsNullOrEmpty(word))
            {
                Console.Write("Enter a word to find: ");
                word = Console.ReadLine();
            }

            var response = CallUrl(url).Result;
            var count = response.Split(' ').Where(word => 
                word.ToLower().Contains("maison"))
                .ToList();
            Console.WriteLine(count.Count);

            while (keepGoing != "y" && keepGoing != "n")
            {
                Console.Write("Do you want to continue (y/n):");
                keepGoing = Console.ReadLine();
            }
        }
    }

    private static bool ValidateUrl(string url)
    {
        return Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }

    private static async Task<string> CallUrl(string fullUrl)
    {
        HttpClient client = new();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
        client.DefaultRequestHeaders.Accept.Clear();
        var response = client.GetStringAsync(fullUrl);
        return await response;
    }
}
