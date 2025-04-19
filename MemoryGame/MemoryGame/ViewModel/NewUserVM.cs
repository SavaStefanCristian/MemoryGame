using MemoryGame.View;
using MemoryGame.Model;
using MemoryGame.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace MemoryGame.ViewModel
{
    public class NewUserVM : BaseVM
    {
        private NewUserModel newUserModel;
        public TextBox NameField { get; set; }

        private int imageIndex;
        public NewUserVM()
        {
            newUserModel = new NewUserModel();
            imageIndex = 0;

            if(newUserModel.ImageList.Count > 0) 
                DisplayedImage = newUserModel.ImageList[imageIndex];

        }

        

        private BitmapImage displayedImage;
        public BitmapImage DisplayedImage 
        {
            get
            {
                return displayedImage;
            }
            set
            {
                displayedImage = value;
                OnPropertyChanged(nameof(DisplayedImage));
            }
        }

        public MainVM MainVM { get; set; }


        #region Commands

        private ICommand backCommand;

        public ICommand BackCommand
        {
            get
            {
                if (backCommand == null) backCommand = new RelayCommand(Back);
                return backCommand;
            }
        }

        private ICommand createCommand;

        public ICommand CreateCommand
        {
            get
            {
                if(createCommand == null) createCommand = new RelayCommand(CreateUser, CanCreateUser);
                return createCommand;
            }
        }

        private ICommand leftImageCommand;
        public ICommand LeftImageCommand
        {
            get
            {
                if (leftImageCommand == null) leftImageCommand = new RelayCommand(MoveToLeftImage,CanMoveLeftImage);
                return leftImageCommand;
            }
        }

        private ICommand rightImageCommand;
        public ICommand RightImageCommand
        {
            get
            {
                if (rightImageCommand == null)  rightImageCommand = new RelayCommand(MoveToRightImage, CanMoveRightImage);
                return rightImageCommand;
            }
        }

        #endregion


        #region Methods

        private void Back(object parameter)
        {
            MainVM.CurrentView = new SignInView(MainVM);
        }

        private void CreateUser(object parameter)
        {
            bool createdUser = newUserModel.CreatePlayer(NameField.Text, imageIndex);
            if(createdUser)
                MainVM.CurrentView = new SignInView(MainVM);
        }

        private bool CanCreateUser(object parameter)
        {
            if (NameField == null) return false;
            if (NameField.Text.Length>0) return true;
            return false;
        }

        private void MoveToLeftImage(object parameter)
        {
            imageIndex--;
            DisplayedImage = newUserModel.ImageList[imageIndex];
        }
        private bool CanMoveLeftImage(object parameter)
        {
            if (imageIndex > 0) return true;
            return false;
        }

        private void MoveToRightImage(object parameter)
        {
            imageIndex++;
            DisplayedImage = newUserModel.ImageList[imageIndex];
        }
        private bool CanMoveRightImage(object parameter)
        {
            if (imageIndex < newUserModel.ImageList.Count - 1) return true;
            return false;
        }

        #endregion
    }
}
