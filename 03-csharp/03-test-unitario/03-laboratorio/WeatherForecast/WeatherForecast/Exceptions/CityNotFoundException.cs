namespace WeatherForecast.Exceptions
{
    public class CityNotFoundException : Exception
    {

        public CityNotFoundException()
        {
        }

        public CityNotFoundException(string message) : base(message)
        {
        }
    }
}
