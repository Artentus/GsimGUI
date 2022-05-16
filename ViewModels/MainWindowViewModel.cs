using GsimGUI.Models;
using System;

namespace GsimGUI.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public Component? SelectedComponent { get; set; }

        public MainWindowViewModel()
        {
            SelectedComponent = new Pin();
        }
    }
}
