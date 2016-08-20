using System;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.System;

namespace ConsoleOutput
{
    public sealed class ConsoleOutputLoggingChannel : ILoggingChannel
    {
        private AppServiceConnection _appServiceConnection;

        public ConsoleOutputLoggingChannel(string title)
        {
            InitializeAppServiceConnection(title);
        }

        public bool Enabled
        {
            get { return _appServiceConnection != null; }
        }

        public LoggingLevel Level
        {
            get { return LoggingLevel.Verbose; }
        }

        public string Name
        {
            get { return "Console Output"; }
        }

        public event TypedEventHandler<ILoggingChannel, object> LoggingEnabled;

        public void Dispose()
        {
            if (_appServiceConnection != null)
            {
                _appServiceConnection.Dispose();
                _appServiceConnection = null;
            }
        }

        public async void LogMessage(string eventString)
        {
            var message = new ValueSet
            {
                ["Message"] = eventString
            };

            await _appServiceConnection.SendMessageAsync(message);
        }

        public async void LogMessage(string eventString, LoggingLevel level)
        {
            var message = new ValueSet
            {
                ["LoggingLevel"] = level.ToString(),
                ["Message"] = eventString
            };

            await _appServiceConnection.SendMessageAsync(message);
        }

        public async void LogValuePair(string value1, int value2)
        {
            var message = new ValueSet
            {
                [value1] = value2
            };

            await _appServiceConnection.SendMessageAsync(message);
        }

        public async void LogValuePair(string value1, int value2, LoggingLevel level)
        {
            var message = new ValueSet
            {
                ["LoggingLevel"] = level.ToString(),
                [value1] = value2
            };

            await _appServiceConnection.SendMessageAsync(message);
        }

        private async void InitializeAppServiceConnection(string title)
        {
            const string consoleOutputPackageFamilyName = "49752MichaelS.Scherotter.ConsoleOutput_9eg5g21zq32qm";

            var options = new LauncherOptions
            {
                PreferredApplicationDisplayName = "Console Output",
                PreferredApplicationPackageFamilyName = consoleOutputPackageFamilyName,
                TargetApplicationPackageFamilyName = consoleOutputPackageFamilyName
            };

            var uriString = string.Format("consoleoutput:?title={0}", title);

            var uri = new Uri(uriString);

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

            if (status != AppServiceConnectionStatus.Success)
            {
                return;
            }

            _appServiceConnection = appServiceConnection;

            LoggingEnabled?.Invoke(this, new EventArgs());
        }
    }
}