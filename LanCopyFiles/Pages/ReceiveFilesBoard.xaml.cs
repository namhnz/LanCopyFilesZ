using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LanCopyFiles.Services.GetIPAddress;

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

            InitIPAddressValues();
        }

        // Nguon: https://stackoverflow.com/a/24320649/7182661
        private void DataTransferingGifMediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            // dataTransferingGifMediaElement.Position = TimeSpan.FromMilliseconds(1);
            // dataTransferingGifMediaElement.Play();
        }

        private void InitIPAddressValues()
        {
            // Dia chi IP uu tien nhat
            var currentConnectionIPAddress = GetCurrentConnectionIPAddress.GetIPv4();
            currentConnectionIPAddressDisplayCardExpander.Subtitle =
                $"Use this IP Address to receive files/folders: {currentConnectionIPAddress}:8085";

            // Toan bo dia chi IP khac
            var allIOnAllAdapters = GetIPAddressOnAllAdapters.GetAllIPv4().Except(new []{ currentConnectionIPAddress }).Select(x => $"{x}:8085");
            allIPAddressOnAllAdaperDisplayTextBlock.Text = "Other IP Addresses on this PC:\n" + string.Join("\n", allIOnAllAdapters.ToArray());
        }

        private void copyIPAddressButton_Click(object sender, RoutedEventArgs e)
        {
            var currentConnectionIPAddress = GetCurrentConnectionIPAddress.GetIPv4();

            Clipboard.SetText(currentConnectionIPAddress);
        }
    }
}