using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GChan.Data;

namespace GChan.Models.Trackers.Sites
{
    public class Board_4Chan : Board
    {
        public const string IS_BOARD_REGEX = "boards.(4chan|4channel).org/[a-zA-Z0-9]*?/*$";
        public const string BOARD_CODE_REGEX = "(?<=((chan|channel).org/))[a-zA-Z0-9]+(?=(/))?";

        public Board_4Chan(string url) : base(url)
        {
            Site = Site._4chan;

            Match boardCodeMatch = Regex.Match(url, BOARD_CODE_REGEX);
            BoardCode = boardCodeMatch.Groups[0].Value;
        }

        public Board_4Chan(BoardData data) : base($"http://boards.4chan.org/{data.Code}/")
        {
            Site = Site._4chan;
            this.BoardCode = data.Code;
            this.GreatestThreadId = data.GreatestThreadId;
        }

        public static bool UrlIsBoard(string url)
        {
            return Regex.IsMatch(url, IS_BOARD_REGEX);
        }

        override protected async Task<Thread[]> GetThreadsImpl()
        {
            var catalogUrl = "http://a.4cdn.org/" + BoardCode + "/catalog.json";
            var client = Utils.GetHttpClient();
            var json = await client.GetStringAsync(catalogUrl);
            var jArray = JArray.Parse(json);

            var threads = jArray
                .SelectTokens("[*].threads[*]")
                .Select(thread =>
                {
                    var id = thread["no"].Value<long>();
                    var url = "http://boards.4chan.org/" + BoardCode + "/thread/" + id;
                    var subject = thread["sub"]?.Value<string>() ?? Thread.NO_SUBJECT;
                    var fileCount = thread["images"].Value<int>();

                    return new Thread_4Chan(url, subject, fileCount);
                })
                .ToArray();

            return threads;
        }
    }
}
