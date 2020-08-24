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
            var deckNodes = doc.DocumentNode.SelectNodes("//div[@class='deck-group']");
            if (deckNodes == null) return new Deck[0];

            foreach (var deckNode in deckNodes)
            {
                string anchor = deckNode.GetAttributeValue("id", "");
                string playerName = deckNode.SelectSingleNode("span[@class='deck-meta']/h4/text()")?.InnerText.Split("(").First().Trim();
                if (String.IsNullOrEmpty(playerName)) playerName = deckNode.SelectSingleNode("div[@class='title-deckicon']/span[@class='deck-meta']/h4/text()")?.InnerText.Split("(").First().Trim();

                string playerResult = deckNode.SelectSingleNode("span[@class='deck-meta']/h4/text()")?.InnerText.Split("(").Last().TrimEnd(')').Trim();
                if (String.IsNullOrEmpty(playerResult)) playerResult = deckNode.SelectSingleNode("div[@class='title-deckicon']/span[@class='deck-meta']/h4/text()")?.InnerText.Split("(").Last().TrimEnd(')').Trim();

                string deckDateText = deckNode.SelectSingleNode("span[@class='deck-meta']/h5/text()")?.InnerText.Split(" on ").Last().Trim();
                if (String.IsNullOrEmpty(deckDateText)) deckDateText = deckNode.SelectSingleNode("div[@class='title-deckicon']/span[@class='deck-meta']/h5/text()")?.InnerText.Split(" on ").Last().Trim();

                var decklistNode = deckNode.SelectSingleNode("div[@class='toggle-text toggle-subnav']/div[@class='deck-list-text']");
                var mainboardNode = decklistNode.SelectSingleNode("div[@class='sorted-by-overview-container sortedContainer']");
                var sideboardNode = decklistNode.SelectSingleNode("div[@class='sorted-by-sideboard-container  clearfix element']");

                DateTime? deckDate = null;
                if (!String.IsNullOrEmpty(deckDateText)) deckDate = DateTime.ParseExact(deckDateText, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

                result.Add(new Deck()
                {
                    Date = deckDate,
                    Player = playerName,
                    Result = playerResult,
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
            var cardNodes = node.SelectNodes(isSideboard ? "span[@class='row']" : "div/span[@class='row']");
            if (cardNodes == null) return new DeckItem[0];

            foreach (var cardNode in cardNodes)
            {
                var cardCount = cardNode.SelectSingleNode("span[@class='card-count']").InnerText;
                var cardName = FixCardName(HttpUtility.HtmlDecode(cardNode.SelectSingleNode("span[@class='card-name']").InnerText));

                cards.Add(new DeckItem()
                {
                    Count = Int32.Parse(cardCount),
                    CardName = cardName
                });
            }
            return (cards.ToArray());
        }

        // Fix inconsistencies on Wizard's side
        private static string FixCardName(string cardName)
        {
            if (cardName == "Lurrus of the Dream Den") return "Lurrus of the Dream-Den";
            if (cardName == "Kongming, ??quot?Sleeping Dragon??quot?") return "Kongming, \"Sleeping Dragon\"";
            if (cardName == "GhazbA?n Ogre") return "Ghazbán Ogre";
            if (cardName == "Lim-DA?l's Vault") return "Lim-Dûl's Vault";
            if (cardName == "Lim-DAul's Vault") return "Lim-Dûl's Vault";
            if (cardName == "SAcance") return "Séance";
            if (cardName == "Æther Vial") return "Aether Vial";
            if (cardName == "Ghirapur Æther Grid") return "Ghirapur Aether Grid";
            if (cardName == "Unravel the Æther") return "Unravel the Aether";
            if (cardName == "Trinisphère") return "Trinisphere";
            return cardName;
        }

        private static DateTime ExtractDateFromUrl(Uri eventUri)
        {
            string eventPath = eventUri.LocalPath;
            string[] eventPathSegments = eventPath.Split("-").Where(e => e.Length > 1).ToArray();
            string eventDate = String.Join("-", eventPathSegments.Skip(eventPathSegments.Length - 3).ToArray());

            if(DateTime.TryParse(eventDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate))
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
