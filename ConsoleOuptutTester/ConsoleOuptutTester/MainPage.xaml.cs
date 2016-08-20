using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ConsoleOuptutTester
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AppServiceConnection _appServiceConnection;

        public MainPage()
        {
            InitializeComponent();
        }

        private async Task InitializeAppServiceConnectionAsync()
        {
            const string consoleOutputPackageFamilyName = "49752MichaelS.Scherotter.ConsoleOutput_9eg5g21zq32qm";

            var options = new LauncherOptions
            {
                PreferredApplicationDisplayName = "Console Output",
                PreferredApplicationPackageFamilyName = consoleOutputPackageFamilyName,
                TargetApplicationPackageFamilyName = consoleOutputPackageFamilyName,
            };

            var uri = new Uri("consoleoutput:?Title=Console Output Tester&input=true");

            if (!await Launcher.LaunchUriAsync(uri, options))
            {
                return;
            }

            var appServiceConnection = new AppServiceConnection
            {
                AppServiceName = "consoleoutput",
                PackageFamilyName = consoleOutputPackageFamilyName
            };

            var status = await appServiceConnection.OpenAsync();

            if (status == AppServiceConnectionStatus.Success)
            {
                _appServiceConnection = appServiceConnection;

                // because we want to get messages back from the console, we launched the app with the input=true parameter
                _appServiceConnection.RequestReceived += _appServiceConnection_RequestReceived;
            }
        }

        private async void _appServiceConnection_RequestReceived(
            AppServiceConnection sender,
            AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message["Message"] as string;

            await OutputBox.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
            {
                OutputBox.IsReadOnly = false;
                OutputBox.Text = OutputBox.Text + message + "\r\n";
                OutputBox.IsReadOnly = true;
            });
        }

        private async void MainPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_appServiceConnection == null)
            {
                return;
            }

            var message = new ValueSet
            {
                ["Message"] = string.Format("{0} x {1}", e.NewSize.Width, e.NewSize.Height)
            };

            await _appServiceConnection.SendMessageAsync(message);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await InitializeAppServiceConnectionAsync();
        }
    }
}