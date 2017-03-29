using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WeatherTileUpdate.Models
{
    public class GetWeatherProxy
    {
        #region Get The Weather
        //Return object must be a Task - A promise to return a task of type rootobject
        public async static Task<RootObject> getWeather(double lat, double lon)
        {
            var http = new HttpClient();

            var url = String.Format("http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&units=metric&appid=8805913b8c73f7ed5c7e280f72d09f6b", lat, lon);

            //Use await because I'm waiting for a result - Method signature must be async also
            var response = await http.GetAsync(url);
            //Result from request in string format
            var result = await response.Content.ReadAsStringAsync();

            /* Convert that string into an object graph so I can easily access elements of the json anywhere
             * DataContractJsonSerializer object used to serialize and deserialize json data*/
            var serializer = new DataContractJsonSerializer(typeof(RootObject));

            //Memory stream allows data to flow - Json is UTF8
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(result));
            //Get data out of the serializer
            var data = (RootObject)serializer.ReadObject(memoryStream);

            return data;
        }//getWeather()
    }//GetWeatherProxy
    #endregion

    #region
    /* Json code from weather http request converted into c# classes - http://json2csharp.com/  
        Every class has a data contract and attributes are data members */
    [DataContract]
    public class Coord
    {
        [DataMember]
        public double lon { get; set; }
        [DataMember]
        public double lat { get; set; }
    }

    [DataContract]
    public class Weather
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string main { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string icon { get; set; }
    }

    [DataContract]
    public class Main
    {
        [DataMember]
        public double temp { get; set; }
        [DataMember]
        public double pressure { get; set; }
        [DataMember]
        public int humidity { get; set; }
        [DataMember]
        public double temp_min { get; set; }
        [DataMember]
        public double temp_max { get; set; }
        [DataMember]
        public double sea_level { get; set; }
        [DataMember]
        public double grnd_level { get; set; }
    }

    [DataContract]
    public class Wind
    {
        [DataMember]
        public double speed { get; set; }
        [DataMember]
        public int deg { get; set; }
    }

    [DataContract]
    public class Clouds
    {
        [DataMember]
        public int all { get; set; }
    }

    [DataContract]
    public class Sys
    {
        [DataMember]
        public double message { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public int sunrise { get; set; }
        [DataMember]
        public int sunset { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public Coord coord { get; set; }
        [DataMember]
        public List<Weather> weather { get; set; }
        [DataMember]
        public string @base { get; set; }
        [DataMember]
        public Main main { get; set; }
        [DataMember]
        public Wind wind { get; set; }
        [DataMember]
        public Clouds clouds { get; set; }
        [DataMember]
        public int dt { get; set; }
        [DataMember]
        public Sys sys { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int cod { get; set; }
    }
    #endregion

}