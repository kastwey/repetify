using WeatherForecast.Services;

var cityWeatherService = new CityWeatherService(new RandomWeatherService());
var weather = cityWeatherService.GetWeather("Madrid");

Console.WriteLine($"The weather in Madrid is {weather}");
