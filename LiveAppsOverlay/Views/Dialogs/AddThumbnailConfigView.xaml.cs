using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace LiveAppsOverlay.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AddThumbnailConfigView.xaml
    /// </summary>
    public partial class AddThumbnailConfigView : UserControl
    {
        public AddThumbnailConfigView()
        {
            InitializeComponent();
        }

        private async void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (sender as DependencyObject).TryFindParent<BaseMetroDialog>();
            await (Application.Current.MainWindow as MetroWindow).HideMetroDialogAsync(dialog);
        }

        private async void ButtonDone_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (sender as DependencyObject).TryFindParent<BaseMetroDialog>();
            await (Application.Current.MainWindow as MetroWindow).HideMetroDialogAsync(dialog);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxName.Focus();
        }
    }
}
