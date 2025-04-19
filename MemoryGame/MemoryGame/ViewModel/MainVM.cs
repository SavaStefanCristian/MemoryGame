using MemoryGame.Model;
using MemoryGame.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MemoryGame.ViewModel
{
    public class MainVM : BaseVM
    {
        private UserControl currentView;
        public UserControl CurrentView
        {
            get => currentView;
            set
            {
                currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private MainModel mainModel { get; set; }


        public MainVM()
        {
            mainModel = new MainModel();
            currentView = new SignInView(this);
        }
    }
}
