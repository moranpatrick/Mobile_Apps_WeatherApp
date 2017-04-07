using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

        }
        #region Page Loaded 
        //Page loaded event called immediately on start-up
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var position = await LocationManager.GetPosition();
                
                var lat = position.Coordinate.Latitude;
                var lon = position.Coordinate.Longitude;

                //Call the getWeather() in GetWeatherProxy and pass through latitude and longitude
                RootObject _weather = await GetWeatherProxy.getWeather(lat, lon);

                //Schedule Tile Update from web app on azure passing in current longitude and latitude
                var uri = string.Format("http://weathertileupdate.azurewebsites.net/?lat={0}&lon={1}", lat, lon);

                var tileContent = new Uri(uri);
                //Update The Start menu tile every six hours
                var requestedInterval = PeriodicUpdateRecurrence.SixHours;

                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.StartPeriodicUpdate(tileContent, requestedInterval);

                //Create a path to the assets folder with the icon returned in the json
                string _weatherIcon = String.Format("ms-appx:///Assets/weather/{0}.png", _weather.weather[0].icon);
                _resultImage.Source = new BitmapImage(new Uri(_weatherIcon, UriKind.Absolute));

                //Display Location Name, temperature and description       
                _tempTxt.Text = ((int)_weather.main.temp).ToString() + "°";
                _descriptionTxt.Text = _weather.weather[0].description;
                _locationTxt.Text = _weather.name;

                //Turn off progess ring, make log input and buttons visible
                _refresh.Visibility = Visibility.Visible;
                _progressRing.IsActive = false;
                _LogMessage.Visibility = Visibility.Visible;
                _logBtn.Visibility = Visibility.Visible;
                _showLog.Visibility = Visibility.Visible;
            }
            catch
            {
                //Handles errors, make retry button visible - If the GPS or Internet Fails the whole app fails as it needs both
                _locationTxt.Text = "Oops - Unable to get weather at this time.";
                _errorButon.Visibility = Visibility.Visible;
                _progressRing.IsActive = false;
                _refresh.Visibility = Visibility.Collapsed;
            }

        }
        #endregion

        private void _errorButon_Click(object sender, RoutedEventArgs e)
        {
            _errorButon.Visibility = Visibility.Collapsed;
            _progressRing.IsActive = true;
            //when retry button is clicked the Page_Loaded event is called again to retry
            Page_Loaded(sender, e);
            _errorButon.Visibility = Visibility.Collapsed;
               
        }

        #region
        private async void _logBtn_Click(object sender, RoutedEventArgs e)
        {
            _LogMessage.Visibility = Visibility.Collapsed;
            _logBtn.Visibility = Visibility.Collapsed;
            _weatherPanel.Visibility = Visibility.Collapsed;
            _deleteLogBtn.Visibility = Visibility.Collapsed;
            _showLog.Visibility = Visibility.Collapsed;
            _progressRing.IsActive = true;
            _displayLog.Visibility = Visibility.Visible;
            _home.Visibility = Visibility.Visible;
            _deleteLogBtn.Visibility = Visibility.Visible;

            //Get Current Date and Time
            var date = DateTime.Now.ToString("dd/m/yy h:mm:ss");

            try
            {
                //Get The Position
                var position = await LocationManager.GetPosition();

                var lat = position.Coordinate.Latitude;
                var lon = position.Coordinate.Longitude;
                var logData = "";

                //Call the getWeather() in GetWeatherProxy
                RootObject _weather = await GetWeatherProxy.getWeather(lat, lon);

                //If log no log message is entered
                if (string.IsNullOrEmpty(_LogMessage.Text))
                {
                    logData = "Temperature: " + ((int)_weather.main.temp).ToString() + "°" + "\nLocation: " + _weather.name + "\nDate: " + date + "\n=======================\n";
                }
                else
                {
                    logData = "Temperature: " + ((int)_weather.main.temp).ToString() + "°" + "\nLocation: " + _weather.name + "\nDate: " + date + "\nLog Message: " + _LogMessage.Text + "\n=======================\n";
                }

                /* Local Storage Reference: https://blogs.windows.com/buildingapps/2016/05/10/getting-started-storing-app-data-locally/#bMwMep36lwQ3xqDD.97*/
                var dataFolder = ApplicationData.Current.LocalFolder;
                var newFolder = await dataFolder.CreateFolderAsync("LogFolder", CreationCollisionOption.OpenIfExists);

                var logFile = await newFolder.CreateFileAsync("log.txt", CreationCollisionOption.OpenIfExists);

                var exists = await isFilePresent("logFile");
                if (exists == true)
                {
                    await FileIO.WriteTextAsync(logFile, logData);
                }
                else
                {
                    await FileIO.AppendTextAsync(logFile, logData);
                }

                /*Only read file if it exists - Catch file exception*/
                var fileExists = await isFilePresent("LogFolder");
                if (fileExists == true)
                {
                    readLogFile();
                }
          
            }
            catch
            {
                _locationTxt.Text = "Error Logging File!";
                //Call the home_click event
                _home_Click(sender, e);
                _progressRing.IsActive = false;
            }
             
        }
        #endregion

        private async void readLogFile()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var newFolder = await folder.CreateFolderAsync("LogFolder", CreationCollisionOption.OpenIfExists);
            var textFile = await newFolder.CreateFileAsync("log.txt", CreationCollisionOption.OpenIfExists);
            var files = await newFolder.GetFilesAsync();
            var desiredFile = files.FirstOrDefault(x => x.Name == "log.txt");
            var textContent = await FileIO.ReadTextAsync(desiredFile);
            _progressRing.IsActive = false;
            _displayLog.Text = textContent + "\n";
        }

        /*Method Reference: http://stackoverflow.com/questions/8626018/how-to-check-if-file-exists-in-a-windows-store-app*/
        private async Task<bool> isFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }

        private async void _deleteLogBtn_Click(object sender, RoutedEventArgs e)
        {
            //To clear the log, just write over it instead of appending to the file
            var dataFolder = ApplicationData.Current.LocalFolder;
            var newFolder = await dataFolder.CreateFolderAsync("LogFolder", CreationCollisionOption.OpenIfExists);

            var logFile = await newFolder.CreateFileAsync("log.txt", CreationCollisionOption.OpenIfExists);

            //Write to the file overwriting the data on it
            await FileIO.WriteTextAsync(logFile, "");
            _showLog_Click(sender, e);

        }

        private async void _showLog_Click(object sender, RoutedEventArgs e)
        {
            //Display The Weather log
            _LogMessage.Visibility = Visibility.Collapsed;
            _logBtn.Visibility = Visibility.Collapsed;
            _weatherPanel.Visibility = Visibility.Collapsed;
            _deleteLogBtn.Visibility = Visibility.Collapsed;
            _showLog.Visibility = Visibility.Collapsed;

            _deleteLogBtn.Visibility = Visibility.Visible;
            _home.Visibility = Visibility.Visible;
            _displayLog.Visibility = Visibility.Visible;
            
            /*Only read file if it exists - Catch file not found exception*/
            var fileExists = await isFilePresent("LogFolder");
            if (fileExists == true)
            {
                readLogFile();
            }
        }

        private void _home_Click(object sender, RoutedEventArgs e)
        {
            _home.Visibility = Visibility.Collapsed;
            _displayLog.Visibility = Visibility.Collapsed;

            _weatherPanel.Visibility = Visibility.Visible;
            _showLog.Visibility = Visibility.Visible;
            _logBtn.Visibility = Visibility.Visible;
            _LogMessage.Visibility = Visibility.Visible;
            _deleteLogBtn.Visibility = Visibility.Collapsed;
            //Clear TextBox
            _LogMessage.Text = "";
        }

        private void _refresh_Click(object sender, RoutedEventArgs e)
        {
            _refresh.Visibility = Visibility.Collapsed;
            _progressRing.IsActive = true;
            //Clear Input Box
            _LogMessage.Text = "";
            //Reload Page_Loaded Event
            Page_Loaded(sender, e);
        }
    }
}
