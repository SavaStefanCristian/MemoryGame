using MemoryGame.Model;
using MemoryGame.View;
using MemoryGame.ViewModel.Commands;
using System.Windows.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using MemoryGame.Entities;

namespace MemoryGame.ViewModel
{
    public class SignInVM : BaseVM
    {
        public MainVM MainVM { get; set; }

        private SignInModel signInModel;

        private BitmapImage? displayedImage;

        public BitmapImage? DisplayedImage
        {
            get => displayedImage;
            set
            {
                displayedImage = value;
                OnPropertyChanged(nameof(DisplayedImage));
            }
        }
        private int selectedItemIndex = -1;
        public int SelectedItemIndex
        {
            get => selectedItemIndex;
            set
            {
                selectedItemIndex = value;
                BitmapImage? image = signInModel.RetrievePlayerImage(selectedItemIndex);
                DisplayedImage = image;
            }
        }

        public ObservableCollection<string> PlayerNames {get;set;}


        public SignInVM()
        {
            signInModel = new SignInModel();
            PlayerNames = new (signInModel.Players.Select(player => player.Name));
        }

        #region Commands

        private ICommand playCommand;
        public ICommand PlayCommand
        {
            get
            {
                if (playCommand == null) playCommand = new RelayCommand(Play,CanPlay);
                return playCommand;
            }
        }

        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null) cancelCommand = new RelayCommand(Cancel, CanCancel);
                return cancelCommand;
            }
        }

        private ICommand newUserCommand;
        public ICommand NewUserCommand
        {
            get
            {
                if(newUserCommand ==null) newUserCommand = new RelayCommand(GoToNewUser);
                return newUserCommand;
            }
        }

        private ICommand deleteUserCommand;
        public ICommand DeleteUserCommand
        {
            get
            {
                if(deleteUserCommand==null) deleteUserCommand = new RelayCommand(DeleteUser,CanDeleteUser);
                return deleteUserCommand;
            }
        }



        #endregion

        #region Methods

        private void GoToNewUser(object parameter)
        {
            MainVM.CurrentView = new NewUserView(MainVM);
        }

        private void DeleteUser(object parameter)
        {
            signInModel.DeletePlayer(SelectedItemIndex);
            PlayerNames.Clear();
            foreach (string player in signInModel.Players.Select(player => player.Name))
            {
                PlayerNames.Add(player);
            }
            SelectedItemIndex = -1;
        }
        private bool CanDeleteUser(object parameter)
        {
            if (SelectedItemIndex != -1) return true;
            return false;
        }

        private void Play(object parameter)
        {
            MainVM.CurrentView = new GameView(MainVM, signInModel.Players[selectedItemIndex]);
        }
        private bool CanPlay(object parameter)
        {
            if (SelectedItemIndex != -1) return true;
            return false;
        }

        private void Cancel(object parameter)
        {
            SelectedItemIndex = -1;
        }
        private bool CanCancel(object parameter)
        {
            if (SelectedItemIndex != -1) return true;
            return false;
        }

        #endregion





    }
}
