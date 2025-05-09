﻿using MemoryGame.ViewModel;
using MemoryGame.ViewModel.Commands;
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
    public partial class NewUserView : UserControl
    {
        private NewUserVM newUserVM;
        public NewUserView(MainVM mainVM)
        {
            InitializeComponent();
            newUserVM = (NewUserVM)DataContext;
            newUserVM.MainVM = mainVM;
            newUserVM.NameField = NameField;
        }

        
    }
}
