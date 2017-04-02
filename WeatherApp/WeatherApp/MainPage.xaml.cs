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

        //Page loaded event called on start-up
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                var position = await LocationManager.GetPosition();
                
                var lat = position.Coordinate.Latitude;
                var lon = position.Coordinate.Longitude;

                //Call the getWeather() in GetWeatherProxy
                RootObject _weather = await GetWaetherProxy.getWeather(lat, lon);

                //Schedule Tile Update from web app on azure passing in current longitude and latitude
                var uri = string.Format("http://weathertileupdate.azurewebsites.net/?lat={0}&lon={1}", lat, lon);

                var tileContent = new Uri(uri);
                var requestedInterval = PeriodicUpdateRecurrence.SixHours;

                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.StartPeriodicUpdate(tileContent, requestedInterval);

                //Create a url with the icon from the json
                string _weatherIcon = String.Format("ms-appx:///Assets/weather/{0}.png", _weather.weather[0].icon);
                _resultImage.Source = new BitmapImage(new Uri(_weatherIcon, UriKind.Absolute));

                //Get Location Name, temperature and description       
                _tempTxt.Text = ((int)_weather.main.temp).ToString() + "°";
                _descriptionTxt.Text = _weather.weather[0].description;
                _locationTxt.Text = _weather.name;

                //Turn off progess ring, make log input and buttons visible
                _progressRing.IsActive = false;
                _LogMessage.Visibility = Visibility.Visible;
                _logBtn.Visibility = Visibility.Visible;
                _deleteLogBtn.Visibility = Visibility.Visible;
            }
            catch
            {
                //Handles errors, make retry button visible
                _locationTxt.Text = "Oops - Unable to get weather at this time.";
                _errorButon.Visibility = Visibility.Visible;
                _progressRing.IsActive = false;
            }

        }

        private void _errorButon_Click(object sender, RoutedEventArgs e)
        {
            _errorButon.Visibility = Visibility.Collapsed;
            _progressRing.IsActive = true;
            //when retry button is clicked the Page_Loaded event is called again to retry
            Page_Loaded(sender, e);
            _errorButon.Visibility = Visibility.Collapsed;
               
        }

        private async void _logBtn_Click(object sender, RoutedEventArgs e)
        {
            _LogMessage.Visibility = Visibility.Collapsed;
            _logBtn.Visibility = Visibility.Collapsed;
            _weatherPanel.Visibility = Visibility.Collapsed;
            _deleteLogBtn.Visibility = Visibility.Collapsed;
            _progressRing.IsActive = true;
            _displayLog.Visibility = Visibility.Visible;

            var date = DateTime.Now.ToString("dd/m/yy h:mm:ss");

            try
            {
                var position = await LocationManager.GetPosition();

                var lat = position.Coordinate.Latitude;
                var lon = position.Coordinate.Longitude;
                var logData = "";

                //Call the getWeather() in GetWeatherProxy
                RootObject _weather = await GetWaetherProxy.getWeather(lat, lon);

                //If log no log message is entered
                if (string.IsNullOrEmpty(_LogMessage.Text))
                {
                    logData = "Temperature: " + ((int)_weather.main.temp).ToString() + "°" + "\nLocation: " + _weather.name + "\nDate: " + date;
                }
                else
                {
                    logData = "Temperature: " + ((int)_weather.main.temp).ToString() + "°" + "\nLocation: " + _weather.name + "\nDate: " + date + "\nLog Message: " + _LogMessage.Text;
                }
                
                //Create dataFile.txt in LocalFolder and write “My text” to it 
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
                readLogFile();

            }
            catch
            {
                Page_Loaded(sender, e);
            }
             
        }

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

        private async Task<bool> isFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }

        private async void _deleteLogBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var localFolder = ApplicationData.Current.LocalFolder;
                var thefiles = await localFolder.GetFilesAsync();

                for (int i = 1; i < thefiles.Count; i++)
                {
                    await thefiles[i].DeleteAsync(StorageDeleteOption.Default);
                    
                }
                _locationTxt.Text = "Sucessfully Deleted Log File";
            }
            catch
            {
                _locationTxt.Text = "Can't find File";
            }


        }
    }
}
