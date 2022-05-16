using ReactiveUI;
using System.Runtime.CompilerServices;

namespace GsimGUI.ViewModels
{
    internal class ViewModelBase : ReactiveObject
    {
        protected T SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
            => this.RaiseAndSetIfChanged(ref storage, value, propertyName);
    }
}
