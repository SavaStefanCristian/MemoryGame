using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Entities
{
    public class Player
    {
        public string Name { get; set; } = "";

        public string Photo { get; set; } = "";

        public int PlayedGames { get; set; } = 0;
        public int WonGames { get; set; } = 0;
    }
}
