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
        [Obsolete("Use TournamentDetailsLoader.GetTournamentDetails(eventUri).Decks")]
        public static Deck[] GetDecks(Uri eventUri)
        {
            return TournamentDetailsLoader.GetTournamentDetails(eventUri).Decks;
        }
    }
}
