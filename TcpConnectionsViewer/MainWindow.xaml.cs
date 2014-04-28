using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TcpConnectionsViewer.Properties;
using TcpConnectionsViewer.Models;

namespace TcpConnectionsViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    var dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    if (dgr != null && !dgr.IsMouseOver)
                    {
                        dgr.IsSelected = false;
                    }
                }
            }
        }

        private void ScrollToTop_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewerFromMenuItem(sender);
            if (scrollViewer != null)
                scrollViewer.ScrollToTop();
        }

        private void ScrollToBottom_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewerFromMenuItem(sender);
            if (scrollViewer != null)
                scrollViewer.ScrollToBottom();
        }

        private static ScrollViewer GetScrollViewerFromMenuItem(object sender)
        {
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var contextMenu = menuItem.Parent as ContextMenu;
                if (contextMenu != null)
                {
                    var dataGrid = contextMenu.PlacementTarget as DataGrid;
                    if (dataGrid != null)
                    {
                        var border = VisualTreeHelper.GetChild(dataGrid, 0) as Decorator;
                        if (border != null)
                        {
                            return border.Child as ScrollViewer;
                        }
                    }
                }
            }
            return null;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            InteractiveCheckForUpdatesAsync(showMsgIfFailure: true, showMsgIfAlreadyLatestVersion: true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InteractiveCheckForUpdatesAsync(showMsgIfFailure: false, showMsgIfAlreadyLatestVersion: false);
        }

        private void InteractiveCheckForUpdatesAsync(bool showMsgIfFailure, bool showMsgIfAlreadyLatestVersion)
        {
            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += (s, e) =>
            {
                try
                {
                    var versionData = VersionData.GetVersionData(Settings.Default.StaticVersionUri);

                    if (!versionData.IsValid)
                        throw new ArgumentException("Invalid dynamic version data.");

                    if (versionData.NewVersionAvailable)
                    {
                        var msgResult = MessageBox.Show("Congratulations - A new and improved version is available!\r\n\r\nWould you like to obtain the latest version now?", "New Version Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (msgResult == MessageBoxResult.Yes)
                            Process.Start(versionData.DownloadUri);
                    }
                    else if (showMsgIfAlreadyLatestVersion)
                    {
                        MessageBox.Show("You are running the latest version.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.IsCritical())
                        throw;

                    if (showMsgIfFailure)
                        MessageBox.Show(string.Format("Error determining version: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            };
            bgWorker.RunWorkerAsync();
        }
    }
}