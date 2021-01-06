using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistParser.Model
{
    public class TournamentDetails
    {
        public Deck[] Decks { get; set; }
        public Standing[] Standings { get; set; }
        public Round[] Rounds { get; set; }
        public Bracket Bracket { get; set; }

        public override string ToString()
        {
            return $"{Decks.Length} decks";
        }
    }
}
