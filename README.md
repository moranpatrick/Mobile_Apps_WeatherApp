# Software Development Year 3 - (UWP) Mobile Apps Project

## Contents
[Project Overview](#overview)  
[Architecture](#architecture)   
[Sensor Used](#Sensor)  
[Isolated Storage](#Storage)  
[ASP.net Web App](#Webapp)      
[References](#References)  

![alt text](https://upload.wikimedia.org/wikipedia/commons/thumb/0/05/Windows_10_Logo.svg/2000px-Windows_10_Logo.svg.png)

## Project Overview<a name = "overview"></a>   
Project Instructions:
>Create a Universal Windows Project (UWP) that will each demonstrate the use of Isolated Storage
and at least one other sensor or service available on the devices.   


<p>For this project I decided to make a weather application which uses the devices GPS to get your location. The gps coordinates are passed through a HTTP request to a [weather API](http://openweathermap.org/) and the current weather for that location is displayed to the user.</p>
<p>The user can also save the current weather along with the date, time and an optional message (Like a weather diary).</p>

[Top](#overview)     

## Architecture<a name = "architecture"></a>     
![alt text](https://upload.wikimedia.org/wikipedia/commons/thumb/1/19/Visual_Studio_2012_logo_and_wordmark.svg/2000px-Visual_Studio_2012_logo_and_wordmark.svg.png)  
<p>This application was developed using Visual studio 2015 community edition and was written in C#</p>


[Top](#overview)  

## Sensor<a name = "Sensor"></a>  
<P>The Sensor used in this application is the devices GPS. Without an Internet connection or GPS, this application will not work</p>
[Top](#overview)   

## Isolated Storage<a name = "Storage"></a>  
<p>For the isolated storage requirement I use the devices local storage to permanently store information. To use windows local storage I used:</p>  
```
ApplicationData.Current.LocalFolder;   
```  
Here the users data is saved in a file and is loaded every time the application is used.  

[Top](#overview)   
## ASP.net Web App<a name = "Webapp"></a>  
[Top](#overview)  

## References<a name = "References"></a>  
* Docs  
https://developer.microsoft.com/en-us/windows/apps  

* Weather API Used  
https://openweathermap.org/  

* Online Tutorials  
https://channel9.msdn.com/Series/Windows-10-development-for-absolute-beginners
http://www.tutorialspoint.com/xaml/  

* Isolated Storage  
http://www.c-sharpcorner.com/UploadFile/6d1860/how-to-implement-local-storage-in-universal-windows-apps/  
http://stackoverflow.com/questions/37119464/uwp-check-if-file-exists#answer-37152526    

* Images Used (Images were all labeled as "free to use with modification")  
https://img.clipartfest.com/afc951befb11640ec879fb6157cc9b1f_overcast-clipart-cloudy-weather-clip-art_800-800.png  
http://www.clipartkid.com/images/579/cloudy-rain-weather-free-cliparts-that-you-can-download-to-you-ay4F5Z-clipart.png  
http://www.iconarchive.com/download/i43449/oxygen-icons.org/oxygen/Status-weather-showers-scattered-night.ico  
https://upload.wikimedia.org/wikipedia/commons/thumb/e/e7/Gnome-weather-clear.svg/2000px-Gnome-weather-clear.svg.png  
https://pixabay.com/p-151378/?no_redirect  
https://upload.wikimedia.org/wikipedia/commons/thumb/5/52/Weather-few-clouds.svg/2000px-Weather-few-clouds.svg.png  
https://upload.wikimedia.org/wikipedia/commons/thumb/e/e7/Weather-few-clouds-night.svg/1000px-Weather-few-clouds-night.svg.png  
https://cdn.pixabay.com/photo/2015/06/24/15/20/clouds-820219_960_720.png  
https://upload.wikimedia.org/wikipedia/commons/thumb/4/4b/Weather-sun-clouds-hard-shower.svg/2000px-Weather-sun-clouds-hard-shower.svg.png  
https://cdn1.iconfinder.com/data/icons/weatherly-2/512/Cloudy_rainy_and_night-512.png  
https://upload.wikimedia.org/wikipedia/commons/thumb/e/e7/Weather-rain-thunderstorm.svg/2000px-Weather-rain-thunderstorm.svg.png  
https://upload.wikimedia.org/wikipedia/commons/thumb/8/84/Weather-snow.svg/2000px-Weather-snow.svg.png  
https://pixabay.com/p-1265203/?no_redirect  
https://pixabay.com/p-1265209/?no_redirect  
https://pixabay.com/p-2031244/?no_redirect  
* Tile Template  
https://msdn.microsoft.com/en-us/library/windows/apps/hh761491.aspx#TileSquareText01      
[Top](#overview)
