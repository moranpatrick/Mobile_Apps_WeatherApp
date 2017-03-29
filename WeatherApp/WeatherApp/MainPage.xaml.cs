using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
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

                //Schedule Tile Update from web app on azure
                var uri = string.Format("http://weathertileupdate.azurewebsites.net/?lat={0}&lon={1}", lat, lon);

                var tileContent = new Uri(uri);
                var requestedInterval = PeriodicUpdateRecurrence.SixHours;

                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.StartPeriodicUpdate(tileContent, requestedInterval);

                //Create a url with the icon from the json
                string _weatherIcon = String.Format("http://openweathermap.org/img/w/{0}.png", _weather.weather[0].icon);
                _resultImage.Source = new BitmapImage(new Uri(_weatherIcon, UriKind.Absolute));

                //Get Location Name, temperature and description       
                _tempTxt.Text = ((int)_weather.main.temp).ToString() + "°";
                _descriptionTxt.Text = _weather.weather[0].description;
                _locationTxt.Text = _weather.name;                   

            }
            catch
            {
                _locationTxt.Text = "Oops - Unable to get weather at this time.";
                _errorButon.Visibility = Visibility.Visible;
            }
        }

        private void _errorButon_Click(object sender, RoutedEventArgs e)
        {
            //when retry button is clicked the Page_Loaded event is called again to retry
            Page_Loaded(sender, e);
            _errorButon.Visibility = Visibility.Collapsed;              
        }
    }
}
