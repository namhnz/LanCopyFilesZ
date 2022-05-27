using System;
using System.Windows;
using System.Windows.Controls;

namespace LanCopyFiles.Pages
{
    /// <summary>
    /// Interaction logic for ReceiveFilesBoard.xaml
    /// </summary>
    public partial class ReceiveFilesBoard : Page
    {
        public ReceiveFilesBoard()
        {
            InitializeComponent();
        }

        // Nguon: https://stackoverflow.com/a/24320649/7182661
        private void DataTransferingGifMediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            dataTransferingGifMediaElement.Position = TimeSpan.FromMilliseconds(1);
            dataTransferingGifMediaElement.Play();
        }
    }
}