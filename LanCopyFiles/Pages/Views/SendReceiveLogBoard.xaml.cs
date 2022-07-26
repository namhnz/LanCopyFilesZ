using System.Diagnostics;
using System.Windows.Controls;
using LanCopyFiles.Pages.ViewModels;
using Wpf.Ui.Controls;

namespace LanCopyFiles.Pages.Views
{
    /// <summary>
    /// Interaction logic for SendReceiveLogBoard.xaml
    /// </summary>
    public partial class SendReceiveLogBoard : UiPage
    {
        public SendReceiveLogBoard()
        {
            InitializeComponent();
        }

        private void AllSendReceiveLogListView_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //this is for vertical check & will avoid the call at the load time (first time)
            if (e.VerticalChange > 0)
            {
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    Debug.WriteLine("end");
                    ((SendReceiveLogBoardViewModel)this.DataContext).LoadWithNewLogs();
                }
            }
        }
    }
}
