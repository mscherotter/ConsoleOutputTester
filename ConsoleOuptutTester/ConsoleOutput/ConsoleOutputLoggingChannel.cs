using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.System;

namespace ConsoleOutput
{
    /// <summary>
    /// Console Output Logging Channel
    /// <remarks>
    /// Logging Level Colors
    /// Verbose:     Black
    /// Error:       Red
    /// Warning:     Orange
    /// Critical:    Blue
    /// Information: Green
    /// </remarks>
    /// </summary>
    public sealed class ConsoleOutputLoggingChannel : ILoggingChannel
    {
        #region Fields
        private AppServiceConnection _appServiceConnection;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the ConsoleOutputLoggingChannel class with the app name as the display name.
        /// </summary>
        public ConsoleOutputLoggingChannel()
        {
            InitializeAppServiceConnection(Package.Current.DisplayName);
        }

        /// <summary>
        /// Creates a new instance of the ConsoleOutputLoggingChannel class.
        /// </summary>
        /// <param name="title">the title of the new Console Output window</param>
        public ConsoleOutputLoggingChannel(string title)
        {
            InitializeAppServiceConnection(title);
        }
        #endregion

        #region Properties

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
        #endregion

        #region Events
        public event TypedEventHandler<ILoggingChannel, object> LoggingEnabled;
        #endregion

        #region Methods
        /// <summary>
        /// Disconnect the app service connection by disposing of it
        /// </summary>
        public void Dispose()
        {
            if (_appServiceConnection != null)
            {
                _appServiceConnection.Dispose();
                _appServiceConnection = null;
            }
        }

        /// <summary>
        /// Logs a message to the current LoggingChannel.
        /// </summary>
        /// <param name="eventString">The message to log.</param>
        public async void LogMessage(string eventString)
        {
            var message = new ValueSet
            {
                ["Message"] = eventString
            };

            await _appServiceConnection.SendMessageAsync(message);
        }

        /// <summary>
        /// Logs a message to the current LoggingChannel with the specified LoggingLevel.
        /// </summary>
        /// <param name="eventString">The message to log.</param>
        /// <param name="level">The logging level.</param>
        public async void LogMessage(string eventString, LoggingLevel level)
        {
            var message = new ValueSet
            {
                ["LoggingLevel"] = level.ToString(),
                ["Message"] = eventString
            };

            await _appServiceConnection.SendMessageAsync(message);
        }

        /// <summary>
        /// Logs data to the current LoggingChannel.
        /// </summary>
        /// <param name="value1">The string to associate with value2.</param>
        /// <param name="value2">The value to associate with value1.</param>
        public async void LogValuePair(string value1, int value2)
        {
            var message = new ValueSet
            {
                [value1] = value2
            };

            await _appServiceConnection.SendMessageAsync(message);
        }

        /// <summary>
        /// Logs data to the current LoggingChannel with the specified LoggingLevel.
        /// </summary>
        /// <param name="value1">The string to associate with value2.</param>
        /// <param name="value2">The value to associate with value1.</param>
        /// <param name="level">The logging level</param>
        public async void LogValuePair(string value1, int value2, LoggingLevel level)
        {
            var message = new ValueSet
            {
                ["LoggingLevel"] = level.ToString(),
                [value1] = value2
            };

            await _appServiceConnection.SendMessageAsync(message);
        }
        #endregion

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