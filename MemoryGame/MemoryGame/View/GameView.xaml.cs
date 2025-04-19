using MemoryGame.Entities;
using MemoryGame.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemoryGame.View
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        private GameVM gameVM;

        public GameView(MainVM mainVM, Player player)
        {
            InitializeComponent();
            gameVM = (GameVM)DataContext;
            gameVM.MainVM = mainVM;
            gameVM.Player = player;
            gameVM.GameGrid = GameGrid;
        }
    }
}
