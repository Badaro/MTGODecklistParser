using MTGODecklistParser.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistParser.Tests.Integration
{
    class BracketLoaderTestsForChallenge : BracketLoaderTests
    {
        public BracketLoaderTestsForChallenge()
        {
        }

        protected override Bracket GetBracket()
        {
            return new Bracket()
            {
                Quarterfinals = new BracketItem[]
                {
                    new BracketItem(){ WinningPlayer = "TSPJendrek", LosingPlayer = "AstralPlane",  Result= "2-0" },
                    new BracketItem(){ WinningPlayer = "JB2002",     LosingPlayer = "signblindman", Result= "2-1" },
                    new BracketItem(){ WinningPlayer = "ZYURYO",     LosingPlayer = "Blitzlion27",  Result= "2-0" },
                    new BracketItem(){ WinningPlayer = "Yanti",      LosingPlayer = "SvenSvenSven", Result= "2-1" }
                },
                Semifinals = new BracketItem[]
                {
                    new BracketItem(){ WinningPlayer = "TSPJendrek", LosingPlayer = "JB2002", Result= "2-1" },
                    new BracketItem(){ WinningPlayer = "ZYURYO",     LosingPlayer = "Yanti",  Result= "2-0" }
                },
                Finals = new BracketItem() { WinningPlayer = "TSPJendrek", LosingPlayer = "ZYURYO", Result = "2-0" }
            };
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-challenge-2020-06-08");
        }
    }
}
