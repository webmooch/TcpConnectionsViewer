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
using System.Reflection;

namespace TcpConnectionsViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            InitializeComponent();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            PromptToSendCrashReport(e);
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

        private static void PromptToSendCrashReport(UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    try
                    {
                        var stackTrace = new StackTrace(exception, true) == null ? "null" : new StackTrace(exception, true).ToString();
                        var message = string.IsNullOrWhiteSpace(exception.Message) ? "null" : exception.Message;
                        var targetSite = exception.TargetSite != null ? string.IsNullOrWhiteSpace(exception.StackTrace) ? "null" : exception.TargetSite.ToString() : "null";
                        var hResult = exception.HResult;

                        var response = MessageBox.Show("Sorry, an unexpected error has occurred.\r\n\r\nWould you please allow an error report to be sent to the developer?\r\n\r\nNo personally identifiable information is collected.", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (response == MessageBoxResult.Yes)
                        {
                            var subject = string.Format("Error: {0} v{1}", Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version);
                            var body = string.Format("M:{0}{4}TS:{1}{4}hR:{2}{4}ST:{3}", message, targetSite, hResult, stackTrace, "|");
                            PopupSendEmail(Settings.Default.CrashEmailAddress, subject, body);
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        private static void PopupSendEmail(string emailAddress, string subject, string body)
        {
            Process.Start(string.Format("mailto:{0}?subject={1}&body={2}", emailAddress, subject, body));
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