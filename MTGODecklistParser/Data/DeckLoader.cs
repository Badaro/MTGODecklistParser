using HtmlAgilityPack;
using MTGODecklistParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MTGODecklistParser.Data
{
    public static class DeckLoader
    {
        public static Deck[] GetDecks(Uri eventUri)
        {
            string pageContent = new WebClient().DownloadString(eventUri);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            List<Deck> result = new List<Deck>();
            foreach (var deckNode in doc.DocumentNode.SelectNodes("//div[@class='deck-group']"))
            {
                string anchor = deckNode.GetAttributeValue("id", "");
                string playerName = deckNode.SelectSingleNode("span[@class='deck-meta']/h4/text()")?.InnerText.Split("(").First().Trim();

                var decklistNode = deckNode.SelectSingleNode("div[@class='toggle-text toggle-subnav']/div[@class='deck-list-text']");
                var mainboardNode = decklistNode.SelectSingleNode("div[@class='sorted-by-overview-container sortedContainer']");
                var sideboardNode = decklistNode.SelectSingleNode("div[@class='sorted-by-sideboard-container  clearfix element']");

                result.Add(new Deck()
                {
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

                cards.Add(new DeckItem()
                {
                    Count = Int32.Parse(cardCount),
                    CardName = cardName
                });
            }
            return (cards.ToArray());
        }
    }
}
