using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LanCopyFiles.Extensions;
using LanCopyFiles.Pages.ViewModels;
using Wpf.Ui.Controls;
using XamlAnimatedGif;

namespace LanCopyFiles.Pages.Views
{
    /// <summary>
    /// Interaction logic for ReceiveFilesBoard.xaml
    /// </summary>
    public partial class ReceiveFilesBoard : UiPage
    {
        private ReceiveFilesBoardViewModel _viewModel;

        public ReceiveFilesBoard()
        {
            InitializeComponent();

            // Goi phuong thuc trong code behind khi property trong view model thay doi: https://stackoverflow.com/questions/8658499/calling-a-method-in-views-codebehind-from-viewmodel
            _viewModel = (ReceiveFilesBoardViewModel)this.DataContext;
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Tham chieu den element trong data template theo ten: https://stackoverflow.com/a/19380935/7182661
            if (e.PropertyName == nameof(ReceiveFilesBoardViewModel.IsReceivingIndicatorShow))
            {
                if (_viewModel.IsReceivingIndicatorShow)
                {
                    var contentPresenter =
                        VisualHelper.FindVisualChild<ContentPresenter>(ReceiveFilesBoardMainDisplayContentControl);
                    var dataTemplate = contentPresenter.ContentTemplate;

                    var dataTransferingGifImageElement =
                        dataTemplate.FindName("dataTransferingGifImage", contentPresenter) as Image;

                    if (dataTransferingGifImageElement != null)
                    {
                        var controller = AnimationBehavior.GetAnimator(dataTransferingGifImageElement);
                        controller.Play();
                    }
                }
                else
                {
                    var contentPresenter =
                        VisualHelper.FindVisualChild<ContentPresenter>(ReceiveFilesBoardMainDisplayContentControl);
                    var dataTemplate = contentPresenter.ContentTemplate;

                    var dataTransferingGifImageElement =
                        dataTemplate.FindName("dataTransferingGifImage", contentPresenter) as Image;

                    if (dataTransferingGifImageElement != null)
                    {
                        var controller = AnimationBehavior.GetAnimator(dataTransferingGifImageElement);
                        controller.Pause();
                    }
                }
            }
        }


        // // Nguon: https://stackoverflow.com/a/24320649/7182661
        // private void DataTransferingGifMediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        // {
        //     dataTransferingGifMediaElement.Position = TimeSpan.FromMilliseconds(1);
        //     dataTransferingGifMediaElement.Play();
        // }
    }
}