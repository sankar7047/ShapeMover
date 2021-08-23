using System;
using System.Collections.Generic;
using BaseNotify = FrostbiteApp.ShapeMoverLib.Base.BaseNotify;

namespace FrostbiteApp.WPF.ViewModel
{
    public class MainViewModel : BaseNotify
    {

        public MainViewModel()
        {
            Title = "Shape Mover";
        }

        private string title;

        public string Title
        {
            get => title;
            set => SetPropertyChanged(ref title, value);
        }
    }
}
