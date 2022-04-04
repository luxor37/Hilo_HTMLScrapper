using static HtmlScrapper.Utils;

namespace HtmlScrapper
{
    public class Scrapper
    {
        private const string file = "searchHistory.csv";

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
                Console.Write("\n1. Consult history\n2. Search\n3. Clear History\n4. Quit\nEnter your choice: ");
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

            SaveCurrentSearch(url, word, count);
        }

        //Ask the user to save the current search to a specific location
        private static void SaveCurrentSearch(string url, string word, int count) {
            string? yOrN;
            do
            {
                Console.Write("Do you want to save this search? (y/n):");
                yOrN = Console.ReadLine();
            }
            while (yOrN != "y" && yOrN != "n");

            if (yOrN == "y")
            {
                var location = "";
                while (!ValidatePath(location))
                {
                    Console.Write("Enter a relative path:");
                    location = Console.ReadLine();
                }
                var absolutePath = Path.GetFullPath(location) + "\\search.csv";
                SaveSearch(url, word, count, absolutePath);
                Console.WriteLine($"This search was saved at '{absolutePath}'");
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
    }
}