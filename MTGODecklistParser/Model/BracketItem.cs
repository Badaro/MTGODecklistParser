using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MTGODecklistParser.Model
{
    public class BracketItem
    {
        public string WinningPlayer { get; set; }
        public string LosingPlayer { get; set; }
        public string Result { get; set; }

        public override string ToString()
        {
            return $"{WinningPlayer} {Result} {LosingPlayer}";
        }
    }
}
