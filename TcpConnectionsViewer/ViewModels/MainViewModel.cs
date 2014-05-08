using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TcpConnectionsViewer.Models;
using TcpConnectionsViewer.Properties;

namespace TcpConnectionsViewer.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        // TODO: Remove 'kill connection' option totally if not running in elevated mode
        // TODO: Will slam geo server if doing internal multi-host port scan - Dont do geo lookup if IP is private (Create method "IsReservedNetworkAddress" or similar): http://stackoverflow.com/questions/2814002/private-ip-address-identifier-in-regular-expression
        // TODO: Configure and cleanup with custom stylecop rules

        // Known Issues:
        // TODO: Accelerator key not working on ContextMenu items with header populated by stringformat
        // TODO: Issue with sorting of async-populated colums (datagrid.items.refresh?) - possibly post SO question and demo

        // Future versions:
        // TODO: Use an MVVM-based MessageBox solution
        // TODO: Use better format for 'time since process started'
        // TODO: Add icons to all menus
        // TODO: GUI testing with CodeUI
        // TODO: Add about window
        // TODO: When all columns in the same group are not selected disable that async lookup (eg. no GEO data columns selected then disable all calls to lookup GEO data)
        // TODO: Change dispatcher to something less WPF specific
        // TODO: Populate GEO data column header tooltips from [Description("")] attributes
        // TODO: Change null values that are being looked up to 'loading / pending etc' and if no data then populate with "-"
        // TODO: Replicate column selection into main menu view branch - http://stackoverflow.com/questions/150150/how-do-i-share-a-menu-definition-between-a-context-menu-and-a-regular-menu-in-wp
        // TODO: Persist columns position and sort order: http://bengribaudo.com/blog/2012/03/14/1942/saving-restoring-wpf-datagrid-columns-size-sorting-and-order http://stackoverflow.com/questions/6468488/retain-the-user-defined-sort-order-of-a-wpf-datagrid http://www.codeproject.com/Articles/37087/DataGridView-that-Saves-Column-Order-Width-and-Vis
        // TODO: Persist window size and position: http://www.codeproject.com/Articles/50761/Save-and-Restore-WPF-Window-Size-Position-and-or-S http://stackoverflow.com/questions/847752/net-wpf-remember-window-size-between-sessions

        // Control over data:
        // TODO: Add options down bottom (insert window that slides up and then slides back back - contains all advanced options) - or as options dialog box
        // TODO: Drop down box to allow refresh rate specification
        // TODO: Loop every 5 minutes(?) refreshing local Internet IP address
        // TODO: Button - Pause Monitoring / Resume Monitoring
        // TODO: Option to clear all caches
        // TODO: Drop down box allowing to monitor certain connection state, or all
        // TODO: Checkbox to include / exclude all loopback entries
        // TODO: Options to enable / disable all Internet-based lookups

        // Future features to be investigated:
        // TODO: Option to save default settings to either profile xml file (currently how it's done) or as xml file saved alongside the exe. If both exist use which?
        // TODO: Live statistics summary / overview
        // TODO: scrolling log of all connection state changes, their duration, timestamps, etc
        // TODO: Displaying and monitoring total data transferred per connection
        // TODO: Colours for recently added entries? Colours to indicate connection state?

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private TcpConnectionsMonitor connectionsMonitor;

        public ObservableCollection<TcpConnection> TcpInfo { get; private set; }

        private bool? _isElevated;
        public bool IsElevated
        {
            get
            {
                if (_isElevated == null)
                    _isElevated = ThreadIsElevated();

                return (bool)_isElevated;
            }
        }

        private TcpConnection _selectedItem;
        public TcpConnection SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        private ICommand _terminateAllConnectionsToSelectedItemRemoteAddressCommand;
        public ICommand TerminateAllConnectionsToSelectedItemRemoteAddressCommand
        {
            get { return _terminateAllConnectionsToSelectedItemRemoteAddressCommand; }
            private set
            {
                _terminateAllConnectionsToSelectedItemRemoteAddressCommand = value;
                OnPropertyChanged();

            }
        }

        private ICommand _terminateSelectedItemProcessCommand;
        public ICommand TerminateSelectedItemProcessCommand
        {
            get { return _terminateSelectedItemProcessCommand; }
            private set
            {
                _terminateSelectedItemProcessCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _browseSelectedItemProcessDirectoryCommand;
        public ICommand BrowseSelectedItemProcessDirectoryCommand
        {
            get { return _browseSelectedItemProcessDirectoryCommand; }
            private set
            {
                _browseSelectedItemProcessDirectoryCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _launchSelectedItemCoordinatesInBrowserMapCommand;
        public ICommand LaunchSelectedItemCoordinatesInBrowserMapCommand
        {
            get { return _launchSelectedItemCoordinatesInBrowserMapCommand; }
            private set
            {
                _launchSelectedItemCoordinatesInBrowserMapCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _launchSelectedItemRemoteAddressInBrowserCommand;
        public ICommand LaunchSelectedItemRemoteAddressInBrowserCommand
        {
            get { return _launchSelectedItemRemoteAddressInBrowserCommand; }
            private set
            {
                _launchSelectedItemRemoteAddressInBrowserCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _launchStartPageSearchForSelectedItemIspInBrowserCommand;
        public ICommand LaunchStartPageSearchForSelectedItemIspInBrowserCommand
        {
            get { return _launchStartPageSearchForSelectedItemIspInBrowserCommand; }
            private set
            {
                _launchStartPageSearchForSelectedItemIspInBrowserCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _launchStartPageSearchForSelectedRemoteAddressInBrowserCommand;
        public ICommand LaunchStartPageSearchForSelectedRemoteAddressInBrowserCommand
        {
            get { return _launchStartPageSearchForSelectedRemoteAddressInBrowserCommand; }
            private set
            {
                _launchStartPageSearchForSelectedRemoteAddressInBrowserCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _displayDefaultColumnsCommand;
        public ICommand DisplayDefaultColumnsCommand
        {
            get { return _displayDefaultColumnsCommand; }
            private set
            {
                _displayDefaultColumnsCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _displayAllColumnsCommand;
        public ICommand DisplayAllColumnsCommand
        {
            get { return _displayAllColumnsCommand; }
            private set
            {
                _displayAllColumnsCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _visitProjectHomepageCommand;
        public ICommand VisitProjectHomepageCommand
        {
            get { return _visitProjectHomepageCommand; }
            private set
            {
                _visitProjectHomepageCommand = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region IsDisplayed Properties

        public bool LocalAddressIsDisplayed
        {
            get { return Settings.Default.LocalAddressIsDisplayed; }
            set
            {
                Settings.Default.LocalAddressIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool LocalHostnameIsDisplayed
        {
            get { return Settings.Default.LocalHostnameIsDisplayed; }
            set
            {
                Settings.Default.LocalHostnameIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool LocalPortIsDisplayed
        {
            get { return Settings.Default.LocalPortIsDisplayed; }
            set
            {
                Settings.Default.LocalPortIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool LocalPortCommonNameIsDisplayed
        {
            get { return Settings.Default.LocalPortCommonNameIsDisplayed; }
            set
            {
                Settings.Default.LocalPortCommonNameIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool LocalPortCommonDescriptionIsDisplayed
        {
            get { return Settings.Default.LocalPortCommonDescriptionIsDisplayed; }
            set
            {
                Settings.Default.LocalPortCommonDescriptionIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool StateIsDisplayed
        {
            get { return Settings.Default.StateIsDisplayed; }
            set
            {
                Settings.Default.StateIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteAddressIsDisplayed
        {
            get { return Settings.Default.RemoteAddressIsDisplayed; }
            set
            {
                Settings.Default.RemoteAddressIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteMacAddressIsDisplayed
        {
            get { return Settings.Default.RemoteMacAddressIsDisplayed; }
            set
            {
                Settings.Default.RemoteMacAddressIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteMacAddressManufacturerIsDisplayed
        {
            get { return Settings.Default.RemoteMacAddressManufacturerIsDisplayed; }
            set
            {
                Settings.Default.RemoteMacAddressManufacturerIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteHostnameIsDisplayed
        {
            get { return Settings.Default.RemoteHostnameIsDisplayed; }
            set
            {
                Settings.Default.RemoteHostnameIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemotePortIsDisplayed
        {
            get { return Settings.Default.RemotePortIsDisplayed; }
            set
            {
                Settings.Default.RemotePortIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemotePortCommonNameIsDisplayed
        {
            get { return Settings.Default.RemotePortCommonNameIsDisplayed; }
            set
            {
                Settings.Default.RemotePortCommonNameIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemotePortCommonDescriptionIsDisplayed
        {
            get { return Settings.Default.RemotePortCommonDescriptionIsDisplayed; }
            set
            {
                Settings.Default.RemotePortCommonDescriptionIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteCountryIsDisplayed
        {
            get { return Settings.Default.RemoteCountryIsDisplayed; }
            set
            {
                Settings.Default.RemoteCountryIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteIspIsDisplayed
        {
            get { return Settings.Default.RemoteIspIsDisplayed; }
            set
            {
                Settings.Default.RemoteIspIsDisplayed = value;
                OnPropertyChanged();
            }
        }


        public bool RemoteDmaCodeIsDisplayed
        {
            get { return Settings.Default.RemoteDmaCodeIsDisplayed; }
            set
            {
                Settings.Default.RemoteDmaCodeIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteTimezoneIsDisplayed
        {
            get { return Settings.Default.RemoteTimezoneIsDisplayed; }
            set
            {
                Settings.Default.RemoteTimezoneIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteAreaCodeIsDisplayed
        {
            get { return Settings.Default.RemoteAreaCodeIsDisplayed; }
            set
            {
                Settings.Default.RemoteAreaCodeIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteAsnIsDisplayed
        {
            get { return Settings.Default.RemoteAsnIsDisplayed; }
            set
            {
                Settings.Default.RemoteAsnIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteContinentCodeIsDisplayed
        {
            get { return Settings.Default.RemoteContinentCodeIsDisplayed; }
            set
            {
                Settings.Default.RemoteContinentCodeIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteLongitudeIsDisplayed
        {
            get { return Settings.Default.RemoteLongitudeIsDisplayed; }
            set
            {
                Settings.Default.RemoteLongitudeIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteLatitudeIsDisplayed
        {
            get { return Settings.Default.RemoteLatitudeIsDisplayed; }
            set
            {
                Settings.Default.RemoteLatitudeIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteCountryCodeIsDisplayed
        {
            get { return Settings.Default.RemoteCountryCodeIsDisplayed; }
            set
            {
                Settings.Default.RemoteCountryCodeIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool RemoteCountryCode3IsDisplayed
        {
            get { return Settings.Default.RemoteCountryCode3IsDisplayed; }
            set
            {
                Settings.Default.RemoteCountryCode3IsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool ProcessIdIsDisplayed
        {
            get { return Settings.Default.ProcessIdIsDisplayed; }
            set
            {
                Settings.Default.ProcessIdIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool ProcessCaptionIsDisplayed
        {
            get { return Settings.Default.ProcessCaptionIsDisplayed; }
            set
            {
                Settings.Default.ProcessCaptionIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool ProcessOwnerIsDisplayed
        {
            get { return Settings.Default.ProcessOwnerIsDisplayed; }
            set
            {
                Settings.Default.ProcessOwnerIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool ProcessCreationDateIsDisplayed
        {
            get { return Settings.Default.ProcessCreationDateIsDisplayed; }
            set
            {
                Settings.Default.ProcessCreationDateIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool TimeSinceProcessCreationIsDisplayed
        {
            get { return Settings.Default.TimeSinceProcessCreationIsDisplayed; }
            set
            {
                Settings.Default.TimeSinceProcessCreationIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool ProcessExecutablePathIsDisplayed
        {
            get { return Settings.Default.ProcessExecutablePathIsDisplayed; }
            set
            {
                Settings.Default.ProcessExecutablePathIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        public bool ProcessCommandLineIsDisplayed
        {
            get { return Settings.Default.ProcessCommandLineIsDisplayed; }
            set
            {
                Settings.Default.ProcessCommandLineIsDisplayed = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainViewModel()
        {
            this.TerminateAllConnectionsToSelectedItemRemoteAddressCommand = new Helpers.DelegateCommand(param => this.TerminateAllConnectionsToSelectedItemRemoteAddress(), param => true);
            this.TerminateSelectedItemProcessCommand = new Helpers.DelegateCommand(param => this.TerminateSelectedItemProcess(), param => true);
            this.BrowseSelectedItemProcessDirectoryCommand = new Helpers.DelegateCommand(param => this.BrowseSelectedItemProcessDirectory(), param => true);
            this.LaunchSelectedItemCoordinatesInBrowserMapCommand = new Helpers.DelegateCommand(param => this.LaunchSelectedItemCoordinatesInBrowserMap(), param => true);
            this.LaunchSelectedItemRemoteAddressInBrowserCommand = new Helpers.DelegateCommand(param => this.LaunchSelectedItemRemoteAddressInBrowser(), param => true);
            this.LaunchStartPageSearchForSelectedItemIspInBrowserCommand = new Helpers.DelegateCommand(param => this.LaunchStartPageSearchForSelectedItemIspInBrowser(), param => true);
            this.LaunchStartPageSearchForSelectedRemoteAddressInBrowserCommand = new Helpers.DelegateCommand(param => this.LaunchStartPageSearchForSelectedRemoteAddressInBrowser(), param => true);
            this.DisplayDefaultColumnsCommand = new Helpers.DelegateCommand(param => this.DisplayDefaultColumns(), param => true);
            this.DisplayAllColumnsCommand = new Helpers.DelegateCommand(param => this.DisplayAllColumns(), param => true);
            this.VisitProjectHomepageCommand = new Helpers.DelegateCommand(param => this.VisitProjectHomepage(), param => true);

            this.connectionsMonitor = new TcpConnectionsMonitor(this.dispatcher, new TimeSpan(0, 0, 1), Settings.Default.AsyncPropertyPendingText, Settings.Default.AsyncPropertyLoadingText);
            this.TcpInfo = this.connectionsMonitor.TcpInfo;
            this.connectionsMonitor.BeginMonitoringConnectionsAsync();
        }

        private void VisitProjectHomepage()
        {
            LaunchUrlInBrowser(Settings.Default.ProjectHomepageUri);
        }

        private void DisplayAllColumns()
        {
            foreach (var property in this.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(bool) && property.Name.EndsWith("IsDisplayed"))
                {
                    property.SetValue(this, true);
                }
            }
        }

        private void DisplayDefaultColumns()
        {
            Settings.Default.Reset();
            Settings.Default.Reload();
            OnPropertyChanged("");
        }

        private void LaunchStartPageSearchForSelectedRemoteAddressInBrowser()
        {
            if (SelectedItem != null && !string.IsNullOrWhiteSpace(SelectedItem.RemoteAddress))
            {
                string target = string.Format(@"https://startpage.com/do/search?query=%22{0}%22&cat=web&language=english", SelectedItem.RemoteAddress);
                LaunchUrlInBrowser(target);
            }
        }

        private void LaunchStartPageSearchForSelectedItemIspInBrowser()
        {
            if (SelectedItem != null && SelectedItem.RemoteGeoAsync != null && !string.IsNullOrWhiteSpace(SelectedItem.RemoteGeoAsync.Isp))
            {
                string target = string.Format(@"https://startpage.com/do/search?query=%22{0}%22&cat=web&language=english", SelectedItem.RemoteGeoAsync.Isp); //todo: escape isp string
                LaunchUrlInBrowser(target);
            }
        }

        private void LaunchSelectedItemCoordinatesInBrowserMap()
        {
            if (SelectedItem != null && SelectedItem.RemoteGeoAsync != null && !string.IsNullOrWhiteSpace(SelectedItem.RemoteGeoAsync.Latitude) && !string.IsNullOrWhiteSpace(SelectedItem.RemoteGeoAsync.Longitude))
            {
                string target = string.Format(@"http://maps.google.com/maps?q={0},{1}", SelectedItem.RemoteGeoAsync.Latitude, SelectedItem.RemoteGeoAsync.Longitude);
                LaunchUrlInBrowser(target);
            }
        }

        private void LaunchSelectedItemRemoteAddressInBrowser()
        {
            if (SelectedItem != null && !string.IsNullOrWhiteSpace(SelectedItem.RemoteAddress))
            {
                string target = string.Format(@"http://{0}", SelectedItem.RemoteAddress);
                LaunchUrlInBrowser(target);
            }
        }

        private void BrowseSelectedItemProcessDirectory()
        {
            if (SelectedItem != null && SelectedItem.RunningProcessAsync != null && !string.IsNullOrWhiteSpace(SelectedItem.RunningProcessAsync.ExecutablePath))
            {
                try
                {
                    var fi = new FileInfo(SelectedItem.RunningProcessAsync.ExecutablePath);
                    if (fi != null && fi.Directory != null && !string.IsNullOrWhiteSpace(fi.Directory.FullName))
                        Process.Start(fi.Directory.FullName);
                }
                catch (Exception ex)
                {
                    if (!ex.IsCritical())
                        MessageBox.Show(string.Format("Exception encountered while browsing process directory '{0}':\r\n\r\n{1}", SelectedItem.RunningProcessAsync.ExecutablePath, ex.Message), "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        throw;
                }
            }
        }

        private void TerminateSelectedItemProcess()
        {
            if (SelectedItem != null && SelectedItem.ProcessId != 0)
            {
                try
                {
                    SelectedItem.ProcessId.TerminateProcess();
                }
                catch (Exception ex)
                {
                    if (!ex.IsCritical())
                        MessageBox.Show(string.Format("Exception encountered while attempting to terminate process '{0}':\r\n\r\n{1}", SelectedItem.ProcessId, ex.Message), "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        throw;
                }
            }
        }

        private void TerminateAllConnectionsToSelectedItemRemoteAddress()
        {
            if (SelectedItem != null && !string.IsNullOrWhiteSpace(SelectedItem.RemoteAddress))
            {
                try
                {
                    IpHlpApi.CloseRemoteTcpConnection(IPAddress.Parse(SelectedItem.RemoteAddress));
                }
                catch (Exception ex)
                {
                    if (!ex.IsCritical())
                        MessageBox.Show(string.Format("Exception encountered while attempting to terminate all connections to remote address '{0}':\r\n\r\n{1}", SelectedItem.RemoteAddress, ex.Message), "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        throw;
                }
            }
        }

        private static void LaunchUrlInBrowser(string target)
        {
            try
            {
                Process.Start(target);
            }
            catch (Win32Exception ex)
            {
                if (ex.ErrorCode == -2147467259)
                    MessageBox.Show(string.Format("Exception encountered while launching URL '{0}':\r\n\r\nNo Internet browser was found.", target), "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                if (!ex.IsCritical())
                    MessageBox.Show(string.Format("Exception encountered while launching URL '{0}':\r\n\r\n{1}", target, ex.Message), "", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    throw;
            }
        }

        private static bool ThreadIsElevated()
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        public void Dispose()
        {
            if (this.connectionsMonitor != null)
                this.connectionsMonitor.Dispose();
        }
    }
}