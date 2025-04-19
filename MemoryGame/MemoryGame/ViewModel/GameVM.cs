using MemoryGame.Entities;
using MemoryGame.Model;
using MemoryGame.View;
using MemoryGame.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MemoryGame.ViewModel
{
    public class GameVM : BaseVM
    {
        public MainVM MainVM { get; set; }
        private Player player;
        public Player Player 
        {
            get => player;
            set
            {
                player = value;
                GameModel.Player = player;
            }
        }
        public Grid GameGrid { get; set; }
        private bool isShowingFlippedCards = false;

        public GameModel GameModel { get; set; }
        public GameVM()
        {
            GameModel = new GameModel();
            StandardChecked = true;
        }

        #region Properties

        private bool gameNotRunning;
        public bool GameNotRunning
        {
            get => gameNotRunning;
            set
            {
                gameNotRunning = value;
                OnPropertyChanged(nameof(gameNotRunning));
            }
        }

        private bool standardChecked;
        public bool StandardChecked 
        {
            get => standardChecked;
            set
            {
                standardChecked = value;
                if (standardChecked == true)
                {
                    GameModel.ColumnCount = 4;
                    GameModel.RowCount = 4;
                    TimeSliderValue = 60;
                    CustomOptionsVisibility = Visibility.Hidden;
                }

            }
        }

        private bool customChecked = false;
        public bool CustomChecked
        {
            get => customChecked;
            set
            {
                customChecked = value;
                if (customChecked == true)
                {
                    CustomOptionsVisibility = Visibility.Visible;
                }
            }
        }

        private Visibility customOptionsVisibility = Visibility.Hidden;
        public Visibility CustomOptionsVisibility
        {
            get => customOptionsVisibility;
            set
            {
                customOptionsVisibility = value;
                OnPropertyChanged(nameof(CustomOptionsVisibility));
            }
        }

        private ObservableCollection<int> rowAndColumnsList;
        public ObservableCollection<int> RowAndColumnsList
        {
            get
            {
                if (rowAndColumnsList == null)
                {
                    rowAndColumnsList = new ObservableCollection<int>(
                        Enumerable.Range(2, 5)
                    );
                }
                return rowAndColumnsList;
            }
        }

        private ObservableCollection<string> categoryList;
        public ObservableCollection<string> CategoryList
        {
            get
            {
                if (categoryList == null)
                {
                    categoryList = new ObservableCollection<string>();
                    categoryList.Add("Flowers");
                    categoryList.Add("Cars");
                    categoryList.Add("Puppies");
                }
                return categoryList;
            }
        }
        private int selectedCategoryIndex;
        public int SelectedCategoryIndex
        {
            get => selectedCategoryIndex;
            set
            {
                selectedCategoryIndex = value;
                GameModel.Category = selectedCategoryIndex + 1;
            }
        }

        private int selectedRows = 4;
        public int SelectedRows
        {
            get => selectedRows;
            set
            {
                selectedRows = value;
                if (CustomChecked) GameModel.RowCount = selectedRows;
            }
        }
        private int selectedColumns = 4;
        public int SelectedColumns
        {
            get => selectedColumns;
            set
            {
                selectedColumns = value;
                if (CustomChecked) GameModel.ColumnCount = selectedColumns;
            }
        }

        private int timeSliderValue;
        public int TimeSliderValue
        {
            get => timeSliderValue;
            set
            {
                timeSliderValue = value;
                GameModel.TotalTimeSeconds = timeSliderValue;
            }
        }




        #endregion

        #region Commands

        private ICommand statisticsCommand;
        public ICommand StatisticsCommand
        {
            get
            {
                if (statisticsCommand == null) statisticsCommand = new RelayCommand(ShowStats);
                return statisticsCommand;
            }
        }

        private ICommand aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (aboutCommand == null) aboutCommand = new RelayCommand(ShowAbout);
                return aboutCommand;
            }
        }

        private ICommand exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null) exitCommand = new RelayCommand(Exit);
                return exitCommand;
            }
        }

        private ICommand imageCommand;
        public ICommand ImageCommand
        {
            get
            {
                if (imageCommand == null) imageCommand = new RelayCommand(ImageClicked, CanClickImage);
                return imageCommand;
            }
        }

        private ICommand startCommand;
        public ICommand StartCommand
        {
            get
            {
                if (startCommand == null) startCommand = new RelayCommand(StartGame, CanStartGame);
                return startCommand;
            }
        }

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null) saveCommand = new RelayCommand(SaveGame, CanSaveGame);
                return saveCommand;
            }
        }

        private ICommand openGameCommand;
        public ICommand OpenGameCommand
        {
            get
            {
                if (openGameCommand == null) openGameCommand = new RelayCommand(OpenGame, CanOpenGame);
                return openGameCommand;
            }
        }

        #endregion

        #region Methods

        private void OpenGame(object parameter)
        {
            Game? selectedGame = null;
            SavedGamesView sgv = new SavedGamesView(GameModel.SavedGames);
            sgv.ShowDialog();
            if(sgv.Game != null)
            {
                SelectedCategoryIndex = sgv.Game.Category - 1;
                SelectedColumns = sgv.Game.ColumnCount;
                GameModel.ColumnCount = selectedColumns;
                SelectedRows = sgv.Game.RowCount;
                GameModel.RowCount = selectedRows;
                GameModel.LoadGame(sgv.Game);
                SetGrid();
            }
        }
        private bool CanOpenGame(object parameter)
        {
            return !GameModel.IsRunning;
        }
        private void SaveGame(object parameter)
        {
            GameModel.SaveGame();
        }
        private bool CanSaveGame(object parameter)
        {
            return GameModel.IsRunning;
        }
        private void StartGame(object parameter)
        {
            GameModel.StartGame();
            SetGrid();
        }

        private bool CanStartGame(object parameter)
        {
            return !GameModel.IsRunning;
        }

        private void SetGrid()
        {
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.Children.Clear();

            for (int row = 0; row < GameModel.RowCount; ++row)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                GameGrid.RowDefinitions.Add(rowDef);
            }

            for (int column = 0; column < GameModel.ColumnCount; ++column)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                colDef.Width = new GridLength(1, GridUnitType.Star);
                GameGrid.ColumnDefinitions.Add(colDef);
            }

            for (int row = 0; row < GameModel.RowCount; ++row)
            {
                for (int column = 0; column < GameModel.ColumnCount; ++column)
                {
                    if ((GameModel.RowCount * GameModel.ColumnCount % 2 == 1) && row == GameModel.RowCount - 1 && column == GameModel.ColumnCount - 1) continue;

                    Image image = new Image();
                    image.Source = GameModel.BackImage;
                    image.Stretch = Stretch.Uniform;

                    Button button = new Button();
                    button.Content = image;
                    button.Margin = new Thickness(2);
                    button.Background = Brushes.White;
                    button.BorderBrush = Brushes.DarkGray;
                    button.BorderThickness = new Thickness(1);

                    button.Command = ImageCommand;
                    button.CommandParameter = new Tuple<int, int>(row, column);

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, column);
                    GameGrid.Children.Add(button);

                    if (GameModel.GuessedCards[row * GameModel.ColumnCount + column])
                    {
                        button.Visibility = Visibility.Hidden;
                        button.IsEnabled = false;
                    }

                }

            }
        }

        private async void ImageClicked(object parameter)
        {
            if (GameGrid.Children == null || GameGrid.Children.Count == 0) return;
            Tuple<int, int> pos = parameter as Tuple<int, int>;
            if(pos == null)
            {
                return;
            }

            if (pos.Item1 * GameModel.ColumnCount + pos.Item2 >= GameGrid.Children.Count) return;
            Button button = GameGrid.Children[pos.Item1 * GameModel.ColumnCount + pos.Item2] as Button;
            if (button == null) return;

            Image image = button.Content as Image;
            if (image == null) return;

            image.Source = GameModel.ImageList[GameModel.Cards[pos.Item1 * GameModel.ColumnCount + pos.Item2]];

            Tuple<int, int>? firstCard = GameModel.FirstSelectedCard;
            

            switch (GameModel.CardSelection(pos))
            {
                case 0:
                    break;
                case 1:
                    {
                        if (firstCard.Item1 * GameModel.ColumnCount + firstCard.Item2 >= GameGrid.Children.Count) return;
                        Button button2 = GameGrid.Children[firstCard.Item1 * GameModel.ColumnCount + firstCard.Item2] as Button;
                        if (button2 == null) return;

                        Image image2 = button2.Content as Image;
                        if (image2 == null) return;

                        isShowingFlippedCards = true;

                        await Task.Delay(1000);

                        isShowingFlippedCards = false;

                        CommandManager.InvalidateRequerySuggested();

                        button.IsEnabled = false;
                        button2.IsEnabled = false;
                        button.Visibility = Visibility.Hidden;
                        button2.Visibility = Visibility.Hidden;
                    }
                    break;
                case 2:
                    {
                        Button button2 = GameGrid.Children[firstCard.Item1 * GameModel.ColumnCount + firstCard.Item2] as Button;
                        if (button2 == null) return;

                        Image image2 = button2.Content as Image;
                        if (image2 == null) return;

                        isShowingFlippedCards = true;

                        await Task.Delay(1000);

                        isShowingFlippedCards = false;

                        CommandManager.InvalidateRequerySuggested();
                        
                        image.Source = GameModel.BackImage;
                        image2.Source = GameModel.BackImage;
                    }
                    break;


            }

        }

        private bool CanClickImage(object parameter)
        {
            if (!GameModel.IsRunning)
            {
                if(CustomChecked)
                    CustomOptionsVisibility = Visibility.Visible;
                GameGrid.RowDefinitions.Clear();
                GameGrid.ColumnDefinitions.Clear();
                GameGrid.Children.Clear();
            }
            else
            {
                CustomOptionsVisibility = Visibility.Hidden;
            }
                return !isShowingFlippedCards;
        }

        private void ShowStats(object parameter)
        {
            StringBuilder sb = new StringBuilder();

            string playersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Users\players.json");

            if (File.Exists(playersFilePath))
            {
                string jsonFromFile = File.ReadAllText(playersFilePath);

                if (jsonFromFile != "")
                {

                    List<Player>? loadedPeople = JsonSerializer.Deserialize<List<Player>>(jsonFromFile);
                    if (loadedPeople != null)
                    {
                        foreach (Player player in loadedPeople)
                        {
                            sb.AppendLine($"{player.Name,-30}: Games Played : {player.PlayedGames,-3} | Games Won : {player.WonGames,-3}");
                        }
                    }
                }
            }
            MessageBox.Show(sb.ToString());
        }

        private void ShowAbout(object parameter)
        {
            new AboutView().ShowDialog();
        }

        private void Exit(object parameter)
        {
            GameModel.StopTimer();
            MainVM.CurrentView = new SignInView(MainVM);
        }


            #endregion

        }
}
