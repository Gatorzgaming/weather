using Drizzle.Common;
using Drizzle.Common.Services;
using Drizzle.Models.Weather;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Drizzle.Weather
{
    public class NwsWeatherClient : IWeatherClient
    {
        // NWS API does not require an API key
        public string ApiKey { get; set; }
        public bool IsApiKeyRequired => false;
        public bool IsReverseGeocodingSupported => true;

        private readonly ICacheService cacheService;
        private readonly HttpClient httpClient;

        // NWS API endpoints
        private readonly string forecastApiUrl = "https://api.weather.gov/gridpoints/{office}/{gridX},{gridY}/forecast";
        private readonly string currentApiUrl = "https://api.weather.gov/stations/{stationId}/observations/latest";

        public NwsWeatherClient(IHttpClientFactory httpClientFactory, ICacheService cacheService)
        {
            this.cacheService = cacheService;
            this.httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName");
        }

        public async Task<ForecastWeather> QueryForecastAsync(float latitude, float longitude)
        {
            // Implement method to fetch forecast data from NWS API
            throw new NotImplementedException();
        }

        public Task<ForecastAirQuality> QueryAirQualityAsync(float latitude, float longitude)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Location>> GetLocationDataAsync(float latitude, float longitude)
        {
            var response = await httpClient.GetAsync($"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var locationData = JsonConvert.DeserializeObject<NominatimLocation>(content);

            return new List<Location>
        {
            new Location
            {
                Latitude = latitude,
                Longitude = longitude,
                Name = locationData.display_name
            }
        };
        
        }

        public async Task<IReadOnlyList<Location>> GetLocationDataAsync(string place)
        {
            var response = await httpClient.GetAsync($"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(place)}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var locationData = JsonConvert.DeserializeObject<List<NominatimLocation>>(content);

            return locationData.Select(l => new Location
            {
                Latitude = float.Parse(l.lat, CultureInfo.InvariantCulture),
                Longitude = float.Parse(l.lon, CultureInfo.InvariantCulture),
                Name = l.display_name
            }).ToList();
        }

        public class NominatimLocation
        {
            public string lat { get; set; }
            public string lon { get; set; }
            public string display_name { get; set; }
        }
    }


    // Implement other methods as required by IWeatherClient
}
