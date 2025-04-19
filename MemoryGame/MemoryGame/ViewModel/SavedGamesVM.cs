using MemoryGame.Entities;
using MemoryGame.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.ViewModel
{
    class SavedGamesVM : BaseVM
    {
        public SavedGamesView view { get; set; }
        private List<Game> savedGames;
        public List<Game> SavedGames
        {
            get => savedGames;
            set
            {
                savedGames = value;
                OnPropertyChanged(nameof(SavedGames));
            }
        }
        private Game game;
        public Game Game
        {
            get => game;
            set
            {
                game = value;
                view.Game = game;
                view.Close();
            }
        }
    }
}
