using WeatherForecast.Contracts;
using WeatherForecast.Exceptions;

namespace WeatherForecast.Services
{
    public enum Weather
    {
        Sunny,
        Cloudy,
        Rainy,
        Stormy
    }

    public class CityWeatherService
    {
        private readonly IWeatherService _weatherService;
        private readonly List<string> _nameCities = []; 

        public CityWeatherService(IWeatherService weatherService)
        {
            _weatherService = weatherService;            
            _nameCities.AddRange(new List<string> { "A Coruña", "Álava", "Albacete", "Alicante", "Almería", "Asturias", "Ávila", "Badajoz", "Barcelona", "Burgos", "Cáceres", "Cádiz", "Cantabria", "Castellón", "Ciudad Real", "Córdoba", "Cuenca", "Gerona", "Granada", "Guadalajara", "Guipúzcoa", "Huelva", "Huesca", "Islas Baleares", "Jaén", "La Rioja", "Las Palmas", "León", "Lérida", "Lugo", "Madrid", "Málaga", "Murcia", "Navarra", "Orense", "Palencia", "Pontevedra", "Salamanca", "Segovia", "Sevilla", "Soria", "Tarragona", "Santa Cruz de Tenerife", "Teruel", "Toledo", "Valencia", "Valladolid", "Vizcaya", "Zamora", "Zaragoza", "Ceuta", "Melilla" });
        }

        public Weather GetWeather(string nameCity)
        {
            ArgumentNullException.ThrowIfNull(nameCity);

            var city = _nameCities.SingleOrDefault(c => c.Equals(nameCity, StringComparison.InvariantCultureIgnoreCase));
            if (city == null)
            {
                throw new CityNotFoundException("No se encontró la ciudad.");
            }

            var rainProbability = _weatherService.RainProbability();

            if (rainProbability < 20)
            {
                return Weather.Sunny;
            }
            else if (rainProbability < 50)
            {
                return Weather.Cloudy;
            }
            else if (rainProbability < 80)
            {
                return Weather.Rainy;
            }
            else
            {
                return Weather.Stormy;
            }
        }
    }
}
