using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var position = await LocationManager.GetPosition();
            //Call the getWeather() in GetWeatherProxy
            RootObject _weather = await GetWaetherProxy.getWeather(position.Coordinate.Latitude, position.Coordinate.Longitude);

            //Create a url with the icon from the json
            string _weatherIcon = String.Format("http://openweathermap.org/img/w/{0}.png", _weather.weather[0].icon);
            _resultImage.Source = new BitmapImage(new Uri(_weatherIcon, UriKind.Absolute));

            //Get Location Name, temperature and description
            _resultTxt.Text = _weather.name + " -- " + ((int)_weather.main.temp).ToString() + "°" + " - " + _weather.weather[0].description;
        
        }
    }
}
