using System.Net;
//https://www.scrapingbee.com/blog/web-scraping-csharp/

public class Scrapper
{
    public static void Main()
    {
        bool keepGoing = true;

        while (keepGoing)
        {
            var response = CallUrl(GetUrl()).Result.ToLower();

            var count = response.Split(GetWord()).Length - 1;

            Console.WriteLine(count);

            keepGoing = KeepGoing();
        }
    }

    private static bool KeepGoing()
    {
        var keepGoing = "";

        while (keepGoing != "y" && keepGoing != "n")
        {
            Console.Write("Do you want to continue (y/n):");
            keepGoing = Console.ReadLine();
        }

        return keepGoing == "y";
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

    private static async Task<string> CallUrl(string fullUrl)
    {
        HttpClient client = new();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
        client.DefaultRequestHeaders.Accept.Clear();
        var response = client.GetStringAsync(fullUrl);
        return await response;
    }
}
