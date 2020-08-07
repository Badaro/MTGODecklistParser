using HtmlAgilityPack;
using MTGODecklistParser.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace MTGODecklistParser.Data
{
    public static class DeckLoader
    {
        public static Deck[] GetDecks(Uri eventUri)
        {
            DateTime eventDate = ExtractDateFromUrl(eventUri);

            string randomizedEventUrl = ((DateTime.UtcNow - eventDate).TotalDays < 1) ?
                $"{eventUri}?rand={Guid.NewGuid()}" :
                eventUri.ToString(); // Fixes occasional caching issues on recent events

            string pageContent = new WebClient().DownloadString(randomizedEventUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            List<Deck> result = new List<Deck>();
            foreach (var deckNode in doc.DocumentNode.SelectNodes("//div[@class='deck-group']"))
            {
                string anchor = deckNode.GetAttributeValue("id", "");
                string playerName = deckNode.SelectSingleNode("span[@class='deck-meta']/h4/text()")?.InnerText.Split("(").First().Trim();
                if (String.IsNullOrEmpty(playerName)) playerName = deckNode.SelectSingleNode("div[@class='title-deckicon']/span[@class='deck-meta']/h4/text()")?.InnerText.Split("(").First().Trim();

                string deckDateText = deckNode.SelectSingleNode("span[@class='deck-meta']/h5/text()")?.InnerText.Split(" on ").Last().Trim();
                if (String.IsNullOrEmpty(deckDateText)) deckDateText = deckNode.SelectSingleNode("div[@class='title-deckicon']/span[@class='deck-meta']/h5/text()")?.InnerText.Split(" on ").Last().Trim();

                var decklistNode = deckNode.SelectSingleNode("div[@class='toggle-text toggle-subnav']/div[@class='deck-list-text']");
                var mainboardNode = decklistNode.SelectSingleNode("div[@class='sorted-by-overview-container sortedContainer']");
                var sideboardNode = decklistNode.SelectSingleNode("div[@class='sorted-by-sideboard-container  clearfix element']");

                DateTime deckDate = eventDate;
                if (!String.IsNullOrEmpty(deckDateText)) deckDate = DateTime.ParseExact(deckDateText, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

                result.Add(new Deck()
                {
                    Date = deckDate,
                    Player = playerName,
                    AnchorUri = new Uri($"{eventUri.ToString()}#{anchor}"),
                    Mainboard = ParseCards(mainboardNode, false),
                    Sideboard = ParseCards(sideboardNode, true)
                });
            }

            return result.ToArray();
        }

        private static DeckItem[] ParseCards(HtmlNode node, bool isSideboard)
        {
            if (node == null) return new DeckItem[0];

            List<DeckItem> cards = new List<DeckItem>();
            foreach (var cardNode in node.SelectNodes(isSideboard ? "span[@class='row']" : "div/span[@class='row']"))
            {
                var cardCount = cardNode.SelectSingleNode("span[@class='card-count']").InnerText;
                var cardName = cardNode.SelectSingleNode("span[@class='card-name']").InnerText;

                // Wizard's website is very inconsistent about this card
                if (cardName == "Lurrus of the Dream Den") cardName = "Lurrus of the Dream-Den";

                cards.Add(new DeckItem()
                {
                    Count = Int32.Parse(cardCount),
                    CardName = cardName
                });
            }
            return (cards.ToArray());
        }

        private static DateTime ExtractDateFromUrl(Uri eventUri)
        {
            string eventPath = eventUri.LocalPath;
            string[] eventPathSegments = eventPath.Split("-");
            string eventDate = String.Join("-", eventPathSegments.Skip(eventPathSegments.Length - 3).ToArray());

            return DateTime.Parse(eventDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
        }
    }
}
