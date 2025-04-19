using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Entities
{
    public class Game
    {
        public int TotalTimeSeconds { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public List<bool> GuessedCards { get; set; }
        public List<int> Cards { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public int Category { get; set; }

        public Game(int totalTimeSeconds, TimeSpan timeRemaining, List<bool> guessedCards, List<int> cards, int columnCount, int rowCount, int category)
        {
            TotalTimeSeconds = totalTimeSeconds;
            TimeRemaining = timeRemaining;
            GuessedCards = guessedCards;
            Cards = cards;
            ColumnCount = columnCount;
            RowCount = rowCount;
            Category = category;
        }
    }
}
