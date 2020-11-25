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
    public static class BracketLoader
    {
        [Obsolete("Use TournamentDetailsLoader.GetTournamentDetails(eventUri).Bracket")]
        public static Bracket GetBracket(Uri eventUri)
        {
            return TournamentDetailsLoader.GetTournamentDetails(eventUri).Bracket;
        }
    }
}
