using FluentAssertions;
using MTGODecklistParser.Data;
using MTGODecklistParser.Model;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistParser.Tests.Integration
{
    class DeckLoaderTests
    {
        private Deck[] _testData = null;

        [SetUp]
        public void GetTestData()
        {
            _testData = DeckLoader.GetDecks(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-preliminary-2020-06-02"));
        }

        [Test]
        public void DeckCountIsCorrect()
        {
            _testData.Length.Should().Be(16);
        }

        [Test]
        public void DecksHaveMainboards()
        {
            foreach (var deck in _testData) deck.Mainboard.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public void DecksHaveSideboards()
        {
            foreach (var deck in _testData) deck.Sideboard.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public void DecksHaveValidMainboards()
        {
            foreach (var deck in _testData) deck.Mainboard.Sum(i => i.Count).Should().BeGreaterOrEqualTo(60); ;
        }

        [Test]
        public void DecksHaveValidSideboards()
        {
            foreach (var deck in _testData) deck.Sideboard.Sum(i => i.Count).Should().BeLessOrEqualTo(15);
        }

        [Test]
        public void DeckDataIsCorret()
        {
            Deck testDeck = _testData.First();
            testDeck.Should().BeEquivalentTo(new Deck()
            {
                Mainboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Abbot of Keral Keep",  Count=4 },
                    new DeckItem(){ CardName="Kiln Fiend",           Count=2 },
                    new DeckItem(){ CardName="Monastery Swiftspear", Count=4 },
                    new DeckItem(){ CardName="Soul-Scar Mage",       Count=4 },
                    new DeckItem(){ CardName="Thoughtseize",         Count=4 },
                    new DeckItem(){ CardName="Cling to Dust",        Count=3 },
                    new DeckItem(){ CardName="Fatal Push",           Count=3 },
                    new DeckItem(){ CardName="Kolaghan's Command",   Count=1 },
                    new DeckItem(){ CardName="Lava Dart",            Count=2 },
                    new DeckItem(){ CardName="Lightning Bolt",       Count=4 },
                    new DeckItem(){ CardName="Manamorphose",         Count=4 },
                    new DeckItem(){ CardName="Mishra's Bauble",      Count=4 },
                    new DeckItem(){ CardName="Seal of Fire",         Count=2 },
                    new DeckItem(){ CardName="Arid Mesa",            Count=1 },
                    new DeckItem(){ CardName="Blackcleave Cliffs",   Count=4 },
                    new DeckItem(){ CardName="Blood Crypt",          Count=2 },
                    new DeckItem(){ CardName="Bloodstained Mire",    Count=4 },
                    new DeckItem(){ CardName="Marsh Flats",          Count=1 },
                    new DeckItem(){ CardName="Mountain",             Count=3 },
                    new DeckItem(){ CardName="Sacred Foundry",       Count=1 },
                    new DeckItem(){ CardName="Sunbaked Canyon",      Count=2 },
                    new DeckItem(){ CardName="Swamp",                Count=1 }
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Fatal Push",              Count=1 },
                    new DeckItem(){ CardName="Alpine Moon",             Count=2 },
                    new DeckItem(){ CardName="Engineered Explosives",   Count=2 },
                    new DeckItem(){ CardName="Lurrus of the Dream Den", Count=1 },
                    new DeckItem(){ CardName="Unearth",                 Count=1 },
                    new DeckItem(){ CardName="Kolaghan's Command",      Count=1 },
                    new DeckItem(){ CardName="Collective Brutality",    Count=2 },
                    new DeckItem(){ CardName="Goblin Cratermaker",      Count=1 },
                    new DeckItem(){ CardName="Nihil Spellbomb",         Count=2 },
                    new DeckItem(){ CardName="Wear // Tear",            Count=2 }
                },
            });
        }
    }
}
