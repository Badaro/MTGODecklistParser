using HtmlAgilityPack;
using MTGODecklistParser.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Web;

namespace MTGODecklistParser.Data
{
    public static class StandingsLoader
    {
        public static Standing[] GetStandings(Uri eventUri)
        {
            DateTime eventDate = ExtractDateFromUrl(eventUri);

            string randomizedEventUrl = ((DateTime.UtcNow - eventDate).TotalDays < 1) ?
                $"{eventUri}?rand={Guid.NewGuid()}" :
                eventUri.ToString(); // Fixes occasional caching issues on recent events

            string pageContent = new WebClient().DownloadString(randomizedEventUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

           var standingsRoot = doc.DocumentNode.SelectSingleNode("//table[@class='sticky-enabled']");
            if (standingsRoot == null) return null;

            List<Standing> result = new List<Standing>();

            var standingNodes = standingsRoot.SelectNodes("tbody/tr");
            foreach (var standingNode in standingNodes)
            {
                var rows = standingNode.SelectNodes("td");

                int rank = int.Parse(rows[0].InnerText);
                string player = rows[1].InnerText;
                int points = int.Parse(rows[2].InnerText);
                double omwp = double.Parse(rows[3].InnerText, CultureInfo.InvariantCulture);
                double gwp = double.Parse(rows[4].InnerText, CultureInfo.InvariantCulture);
                double ogwp = double.Parse(rows[5].InnerText, CultureInfo.InvariantCulture);

                result.Add(new Standing()
                {
                    Rank = rank,
                    Player = player,
                    Points = points,
                    OMWP = omwp,
                    GWP = gwp,
                    OGWP = ogwp
                });
            }

            return result.ToArray();
        }

        private static DateTime ExtractDateFromUrl(Uri eventUri)
        {
            string eventPath = eventUri.LocalPath;
            string[] eventPathSegments = eventPath.Split("-").Where(e => e.Length > 1).ToArray();
            string eventDate = String.Join("-", eventPathSegments.Skip(eventPathSegments.Length - 3).ToArray());

            if (DateTime.TryParse(eventDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate))
            {
                return parsedDate.ToUniversalTime();
            }
            else
            {
                // This is only used to decide or not to bypass cache, so it's safe to return a fallback for today forcing the bypass
                return DateTime.UtcNow.Date;
            }
        }
    }
}
