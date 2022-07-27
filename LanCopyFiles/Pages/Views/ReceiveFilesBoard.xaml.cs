using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LanCopyFiles.Events;
using LanCopyFiles.Models;
using LanCopyFiles.Pages.ViewModels;
using LanCopyFiles.Services.IPAddressManager;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using LanCopyFiles.Services.StorageServices.FilePrepare;
using LanCopyFiles.TransferFilesEngine.Server;
using log4net;
using Prism.Events;
using Wpf.Ui.Controls;
using XamlAnimatedGif;
using Clipboard = System.Windows.Clipboard;

namespace LanCopyFiles.Pages.Views
{
    /// <summary>
    /// Interaction logic for ReceiveFilesBoard.xaml
    /// </summary>
    public partial class ReceiveFilesBoard : UiPage
    {

        

        public ReceiveFilesBoard()
        {
            InitializeComponent();

            
        }

        // // Nguon: https://stackoverflow.com/a/24320649/7182661
        // private void DataTransferingGifMediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        // {
        //     dataTransferingGifMediaElement.Position = TimeSpan.FromMilliseconds(1);
        //     dataTransferingGifMediaElement.Play();
        // }

    }
}