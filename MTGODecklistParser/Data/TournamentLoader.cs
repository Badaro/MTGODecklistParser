﻿using HtmlAgilityPack;
using MTGODecklistParser.Model;
using MTGODecklistParser.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace MTGODecklistParser.Data
{
    public static class TournamentLoader
    {
        static string _listUrl = "https://magic.wizards.com/en/section-articles-see-more-ajax?l=en&f=9041&search-result-theme=&limit=20&fromDate={fromDate}&toDate={toDate}&sort=DESC&word=&offset=0";
        static string _rootUrl = "https://magic.wizards.com";

        public static Tournament[] GetTournaments(DateTime startDate, DateTime? endDate = null)
        {
            if (endDate == null) endDate = DateTime.Now;
            Dictionary<string, Tournament> result = new Dictionary<string, Tournament>();

            var date = startDate;
            while (date < endDate)
            {
                var fromDate = date;
                var toDate = fromDate.AddDays(1);

                string tournamentListUrl = _listUrl.Replace("{fromDate}", FormatDateForUrl(fromDate)).Replace("{toDate}", FormatDateForUrl(toDate));

                string randomizedTournamentListUrl =
                     ((DateTime.UtcNow - toDate).TotalDays < 1) ?
                    $"{tournamentListUrl}&rand={Guid.NewGuid()}" : 
                    tournamentListUrl; // Fixes occasional caching issues on recent events

                string jsonData = new WebClient().DownloadString(randomizedTournamentListUrl);
                string pageContent = String.Join(String.Empty, JsonConvert.DeserializeObject<WizardsAjaxResult>(jsonData).data);

                if(pageContent.Length>0)
                {
                    Tournament[] parsedEvents = ParseTournaments(pageContent);
                    foreach (Tournament parsedEvent in parsedEvents) if (!result.ContainsKey(parsedEvent.Uri.ToString())) result.Add(parsedEvent.Uri.ToString(), parsedEvent);
                }

                date = date.AddDays(1);
            }

            return result.Select(kvp => kvp.Value).ToArray();
        }

        private static Tournament[] ParseTournaments(string pageContent)
        {
            List<Tournament> result = new List<Tournament>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            foreach (var tournamentNode in doc.DocumentNode.SelectNodes("div/a"))
            {
                var tournamentUrl = tournamentNode.Attributes["href"].Value;
                var tournamentName = tournamentNode.SelectSingleNode("div/div[@class='title']").InnerText.Replace("\t", "").Replace("\n", "").Trim();

                string tournamentDate = String.Join(" ", tournamentName.Split(' ').TakeLast(3));
                tournamentName = tournamentName.Replace(tournamentDate, "").Trim();

                result.Add(new Tournament()
                {
                    Name = tournamentName,
                    Date = DateTime.Parse(tournamentDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime(),
                    Uri = new Uri(_rootUrl + tournamentUrl)
                });
            }

            return result.ToArray();
        }

        private static string FormatDateForUrl(DateTime date)
        {
            var day = date.Day.ToString("D2");
            var month = date.Month.ToString("D2");
            var year = date.Year.ToString("D4");
            return $"{month}%2F{day}%2F{year}";
        }
    }

}
