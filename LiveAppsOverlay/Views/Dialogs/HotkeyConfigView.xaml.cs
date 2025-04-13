using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace LiveAppsOverlay.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for HotkeyConfigView.xaml
    /// </summary>
    public partial class HotkeyConfigView : UserControl
    {
        public HotkeyConfigView()
        {
            InitializeComponent();
        }

        private async void ButtonDone_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (sender as DependencyObject).TryFindParent<BaseMetroDialog>();
            await (Application.Current.MainWindow as MetroWindow).HideMetroDialogAsync(dialog);
        }
    }
}
