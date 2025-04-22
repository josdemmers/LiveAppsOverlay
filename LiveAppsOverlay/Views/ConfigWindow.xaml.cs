using LiveAppsOverlay.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace LiveAppsOverlay.Views
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {

        #region Constructors

        public ConfigWindow()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = App.Current.Services.GetRequiredService<ConfigWindowViewModel>();
            }

            InitializeComponent();
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public bool IsClosed { get; set; } = false;

        #endregion

        #region Event handlers

        private void ConfigWindow_CloseRequest(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            IsClosed = true;
        }

        #endregion

        #region Methods

        #endregion


    }
}
