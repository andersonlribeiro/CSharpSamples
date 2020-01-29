using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace TaskBasedProgramming
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            string[] words = await CreateWordArray(new Uri(@"http://www.gutenberg.org/files/54700/54700-0.txt"));
            //Parallel Tasks
            Parallel.Invoke(
                () =>
                    {
                        Console.WriteLine("Begin first task...");
                        GetLongestWord(words);
                    },
                () =>
                {
                    Console.WriteLine("Begin second task...");
                    GetMostCommonWords(words);
                },

                () =>
                {
                    Console.WriteLine("Begin third task...");
                    GetCountForWord(words, "sleep");
                }
                );
            Console.ReadKey();
            System.Environment.Exit(0);
        }


        static async Task<string[]> CreateWordArray(Uri uri)
        {
            var content = String.Empty;
            try
            {
                Console.WriteLine($"Retrieving from {uri}");
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
                
            }
            catch (HttpRequestException ex)
            {

                Console.WriteLine("\nException Caught!");
                Console.WriteLine($"\nMessage: {ex.Message} ");
            }

            return content.Split(new char[] { ' ', '\u000A', ',', '.', ';', ':', '-', '_', '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        static string GetLongestWord(string[] words)
        {
            var longestWord = (from w in words
                               orderby w.Length descending
                               select w).First();
            Console.WriteLine($"Task 1 -- the longest word is {longestWord}");
            return longestWord;
        }

        static void GetCountForWord(string[] words, string term)
        {
            var findWord = from w in words
                           where w.ToUpper().Contains(term)
                           select w;
            Console.WriteLine($@"Task 3 -- The word {term} occurs { findWord.Count()} times.");
        }

        static void GetMostCommonWords(string[] words)
        {
            var frequencyOrder = from word in words
                                 where word.Length > 6
                                 group word by word into g
                                 orderby g.Count() descending
                                 select g.Key;
            var commonWords = frequencyOrder.Take(10);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Task 2 -- The most common words are:");
            foreach (var v in commonWords)
            {
                sb.AppendLine(v);
            }

            Console.WriteLine(sb.ToString());

        }
        

        

    }
}
