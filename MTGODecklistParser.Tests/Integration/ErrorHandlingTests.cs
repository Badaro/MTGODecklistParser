using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;
using MTGODecklistParser.Model;
using MTGODecklistParser.Data;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace MTGODecklistParser.Tests.Integration
{
    public class ErrorHandlingTests
    {
        [Test]
        public void ShouldNotBreakOnEmptyPage()
        {
            // Broken tournament, should return empty dataset
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-mocs-2019-07-17")).Decks
                .Should().HaveCount(0);
        }

        [Test]
        public void ShouldNotBreakOnEmptyDecks()
        {
            // Broken tournament, should return empty dataset
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/sealed-war-block-mcq-2019-05-10")).Decks
                .ToList()
                .ForEach(d =>
                {
                    d.Mainboard.Should().HaveCount(0);
                    d.Sideboard.Should().HaveCount(0);
                });
        }

        [Test]
        public void ShouldNotBreakOnOutOfStandardUrls()
        {
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/content/pauper-league")).Decks
                .Should().HaveCount(20);
        }
    }
}