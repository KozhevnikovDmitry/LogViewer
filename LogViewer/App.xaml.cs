using System.Windows;
using LogViewer.Model;
using LogViewer.UI.View;
using LogViewer.UI.ViewModel;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var vm = new MainVm(new LogReaderFactory("log.txt", 1000));
            var view = new MainView {DataContext = vm};
            view.ShowDialog();
        }
    }
}
