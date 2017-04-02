using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace WeatherApp
{
    public class LocationManager
    {
        public async static Task<Geoposition> GetPosition()
        {
            //Can I get access to the geoloactor - get lat and long
            var access = await Geolocator.RequestAccessAsync();

            if (access != GeolocationAccessStatus.Allowed)
            {       
                throw new Exception();
            }

            //Default value - give me whatever value you can
            var geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };

            var position = await geolocator.GetGeopositionAsync();

            return position;
        }
    }
}
