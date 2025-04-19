using MemoryGame.Entities;
using MemoryGame.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Linq;

namespace MemoryGame.Model
{
    public class GameModel : BaseVM
    {
        public GameModel()
        {
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += TimerTick;
        }

        public int ColumnCount { get; 
            set; }
        public int RowCount { get; set; }

        public bool IsRunning { get; private set; } = false;

        public List<BitmapImage> ImageList { get; private set; }

        private List<BitmapImage> backImageList;

        public BitmapImage BackImage { get; private set; }

        public List<int> Cards { get; private set; }

        public List<bool> GuessedCards { get; private set; }

        public Tuple<int,int>? FirstSelectedCard { get; private set; }

        private Player player;
        public Player Player
        {
            get => player;
            set
            {
                player = value;
                RetrieveGames();
            }
        }

        public int Category { get; set; } = 1;

        private int totalTimeSeconds;
        public int TotalTimeSeconds
        {
            get => totalTimeSeconds;
            set
            {
                totalTimeSeconds = value;
                TimeRemaining = TimeSpan.FromSeconds(TotalTimeSeconds);
            }
        }
        private DispatcherTimer _timer;
        private TimeSpan _timeRemaining;
        public TimeSpan TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                _timeRemaining = value;
                
                OnPropertyChanged(nameof(TimeDisplay));
            }
        }

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


        public string TimeDisplay => TimeRemaining.ToString(@"mm\:ss");


        public void LoadImages()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Categories\Category"+Category);

            string[] imageFiles = Directory.GetFiles(folderPath, "*.*")
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string file in imageFiles)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                ImageList.Add(bitmap);
            }


        }

        public void LoadBackImages()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Categories\BackImages");

            string[] imageFiles = Directory.GetFiles(folderPath, "*.*")
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string file in imageFiles)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                backImageList.Add(bitmap);
            }
            BackImage = backImageList[Category - 1];

        }

        public int CardSelection(Tuple<int,int> pos)
        {
            if (GuessedCards[pos.Item1 * ColumnCount + pos.Item2])
            {
                return -1;
            }

            if (FirstSelectedCard == null)
            {
                FirstSelectedCard = pos;
                return 0;
            }
            if (FirstSelectedCard.Item1 == pos.Item1 && FirstSelectedCard.Item2 == pos.Item2)
            {
                return -2;
            }
            if (Cards[FirstSelectedCard.Item1*ColumnCount + FirstSelectedCard.Item2] == Cards[pos.Item1 * ColumnCount + pos.Item2])
            {

                GuessedCards[FirstSelectedCard.Item1 * ColumnCount + FirstSelectedCard.Item2] = true;
                GuessedCards[pos.Item1 * ColumnCount + pos.Item2] = true;
                FirstSelectedCard = null;
                int guessedCardsCount = GuessedCards.Count(p => p);
                if (guessedCardsCount == GuessedCards.Count) EndGame(hasWon:true);
                return 1;
            }
            FirstSelectedCard = null;
            return 2;
        }

        public void StartGame()
        {
            ImageList = new List<BitmapImage>();
            LoadImages();
            backImageList = new List<BitmapImage>();
            LoadBackImages();


            int cardCount = ColumnCount * RowCount;
            if (cardCount % 2 == 1) --cardCount;
            if (cardCount > 0 && cardCount % 2 == 1) cardCount--;
            int[] cardsArray = new int[cardCount];
            for(int i = 0; i < cardsArray.Length; ++i)
            {
                cardsArray[i] = i / 2;
            } 
            System.Random.Shared.Shuffle(cardsArray);
            Cards = new List<int>();
            Cards.AddRange(cardsArray);

            GuessedCards = Enumerable.Repeat(false, Cards.Count).ToList();

            IsRunning = true;
            OnPropertyChanged(nameof(IsRunning));

            TimeRemaining = TimeSpan.FromSeconds(TotalTimeSeconds);
            StartTimer();


        }

        private void EndGame(bool hasWon)
        {
            IsRunning = false;
            OnPropertyChanged(nameof(IsRunning));

            StopTimer();

            string playersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Users\players.json");

            List<Player> allPlayers = new();

            if (File.Exists(playersFilePath))
            {
                string jsonFromFile = File.ReadAllText(playersFilePath);

                if (jsonFromFile != "")
                {

                    List<Player>? loadedPeople = JsonSerializer.Deserialize<List<Player>>(jsonFromFile);
                    if (loadedPeople != null)
                    {
                        allPlayers = loadedPeople;
                        foreach (Player player in loadedPeople)
                        {
                            if (player.Name == this.Player.Name)
                            {
                                player.PlayedGames += 1;
                                if (hasWon) player.WonGames += 1;
                            }
                        }
                    }
                }
            }

            string json = JsonSerializer.Serialize(allPlayers);
            File.WriteAllText(playersFilePath, json);
            if(hasWon)
            {
                MessageBox.Show("Congratulations! You won!");
            }
            else
            {
                MessageBox.Show("Better luck next time! You lost!");
            }
        }




        public void StartTimer()
        {
            
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (TimeRemaining.TotalSeconds > 0)
            {
                TimeRemaining = TimeRemaining.Subtract(TimeSpan.FromSeconds(1));
            }
            else
            {
                _timer.Stop();
                EndGame(hasWon: false);
            }
        }


        public void SaveGame()
        {
            if (!IsRunning) return;

            string savesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Users\Saves\"+Player.Name+".json");

            
            SavedGames.Add(new Game(TotalTimeSeconds, TimeRemaining, GuessedCards, Cards, ColumnCount, RowCount, Category));
            string json = JsonSerializer.Serialize(SavedGames);
            File.WriteAllText(savesFilePath, json);
            RetrieveGames();
        }

        private void RetrieveGames()
        {
            string savesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Users\Saves\" + Player.Name + ".json");

            SavedGames = new();

            if (File.Exists(savesFilePath))
            {
                string jsonFromFile = File.ReadAllText(savesFilePath);

                if (jsonFromFile != "")
                {

                    List<Game>? loadedGames = JsonSerializer.Deserialize<List<Game>>(jsonFromFile);
                    if (loadedGames != null)
                    {
                        SavedGames = loadedGames;
                    }
                }
            }
        }

        public void LoadGame(Game game)
        {
            ImageList = new List<BitmapImage>();
            LoadImages();
            backImageList = new List<BitmapImage>();
            LoadBackImages();

            TotalTimeSeconds = game.TotalTimeSeconds;
            TimeRemaining = game.TimeRemaining;
            Cards = game.Cards;
            GuessedCards = game.GuessedCards;

            IsRunning = true;
            OnPropertyChanged(nameof(IsRunning));

            StartTimer();
        }


    }
}
