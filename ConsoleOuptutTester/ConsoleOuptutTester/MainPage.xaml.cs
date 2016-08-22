using System;
using System.Threading.Tasks;
using ConsoleOutput;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
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
        private ConsoleOutputLoggingChannel _channel;

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
            string messageText = string.Format("{0} x {1}", e.NewSize.Width, e.NewSize.Height);

            if (_appServiceConnection != null)
            {
                var message = new ValueSet
                {
                    ["Message"] = string.Format("{0} x {1}", e.NewSize.Width, e.NewSize.Height)
                };

                await _appServiceConnection.SendMessageAsync(message);
            }

            if (_channel != null && _channel.Enabled)
            {
                _channel.LogMessage(messageText);

                LoggingLevel widthLevel = e.NewSize.Width < 640 ? LoggingLevel.Warning : LoggingLevel.Verbose;
                LoggingLevel heightLevel = e.NewSize.Height < 480 ? LoggingLevel.Warning : LoggingLevel.Verbose;

                // Log a warning if the size is less than 640x480
                _channel.LogValuePair("Width", System.Convert.ToInt32(e.NewSize.Width), widthLevel);

                _channel.LogValuePair("Height", System.Convert.ToInt32(e.NewSize.Height), heightLevel);
            }
        }

        private async void StartLogging(object sender, RoutedEventArgs e)
        {
            await InitializeAppServiceConnectionAsync();
        }

        /// <summary>
        /// Create the console ouput logging channel and attach an event that will
        /// trigger when the channel is enabled
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the routed event arguments</param>
        private void StartLoggingWithChannel(object sender, RoutedEventArgs e)
        {
            _channel = new ConsoleOutput.ConsoleOutputLoggingChannel();

            _channel.LoggingEnabled += _channel_LoggingEnabled;
        }

        /// <summary>
        /// Test all of the logging channel methods
        /// </summary>
        /// <param name="sender">the Console Output logging channel</param>
        /// <param name="args">the event argumetns</param>
        private void _channel_LoggingEnabled(ILoggingChannel sender, object args)
        {
            sender.LogMessage("event string");
            sender.LogValuePair("Ticks", DateTime.UtcNow.Ticks.GetHashCode());

            foreach (var item in Enum.GetNames(typeof(LoggingLevel)))
            {
                var eventString = string.Format("{0} event string", item);

                LoggingLevel level = (LoggingLevel)Enum.Parse(typeof(LoggingLevel), item);

                sender.LogMessage(eventString, level);

                sender.LogValuePair("Logging Level", (int) level, level);
            }
        }
    }
}
