using System;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace LanCopyFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        private readonly ISnackbarService _snackbarService;
        public MainWindowViewModel ViewModel { get; }

        public MainWindow(ISnackbarService snackbarService)
        {
            _snackbarService = snackbarService;

            InitializeComponent();

            this.ViewModel = (MainWindowViewModel)this.DataContext;

            _snackbarService.SetSnackbarControl(RootSnackbar);
        }

        // Nguon: https://stackoverflow.com/q/2688923/7182661
        private void Window_Closed(object sender, EventArgs e)
        {
            ViewModel.Close();
        }

        public Frame GetFrame() => RootFrame;

        public INavigation GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();
    }
}