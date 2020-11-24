﻿using MTGODecklistParser.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistParser.Tests.Integration
{
    class BracketLoaderTestsForLeague : BracketLoaderTests
    {
        public BracketLoaderTestsForLeague()
        {
        }

        protected override Bracket GetBracket()
        {
            return null;
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-league-2020-08-04");
        }
    }
}
