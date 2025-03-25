using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Contracts;

namespace WeatherForecast.Services
{
    public class RandomWeatherService : IWeatherService
    {
        public int RainProbability()
        {
            return new Random().Next(101);
        }
    }
}
