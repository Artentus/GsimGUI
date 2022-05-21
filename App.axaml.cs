using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GsimGUI.ViewModels;
using GsimGUI.Views;
using System.IO;
using System.Reflection;

namespace GsimGUI
{
    internal partial class App : Application
    {
        public static new App? Current => Application.Current as App;

        public Theme Theme => (Theme)Styles[0];

        public DirectoryInfo BinaryDirectory { get; }

        public App()
        {
            var assembly = Assembly.GetEntryAssembly();
            BinaryDirectory = new DirectoryInfo(Path.GetDirectoryName(assembly!.Location)!);
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
