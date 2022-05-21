using GsimGUI.Models;

namespace GsimGUI.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private IHasProperties? _selectedComponent;

        public IHasProperties? SelectedComponent
        {
            get => _selectedComponent;
            set => SetProperty(ref _selectedComponent, value);
        }

        public MainWindowViewModel()
        {
            SelectedComponent = new Pin();
        }
    }
}
