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
        #region Return GPS Position
        public async static Task<Geoposition> GetPosition()
        {
            //This method returns the GEO Position of the device
            //Can I get access to the geoloactor - get lat and lon
            var access = await Geolocator.RequestAccessAsync();

            if (access != GeolocationAccessStatus.Allowed)
            {       
                throw new Exception();
            }

            //Default value - give me whatever value you can
            var geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };

            //Get the current gps position(lat and lon)
            var position = await geolocator.GetGeopositionAsync();

            return position;
        }
        #endregion
    }
}
