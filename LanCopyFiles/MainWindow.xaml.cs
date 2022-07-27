using System;
using Wpf.Ui.Controls;

namespace LanCopyFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Nguon: https://stackoverflow.com/q/2688923/7182661
        private void Window_Closed(object sender, EventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).Close();
        }
    }
}