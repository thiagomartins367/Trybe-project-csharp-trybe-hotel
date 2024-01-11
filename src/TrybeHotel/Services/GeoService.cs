using System.Net.Http;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _client;
        private const string _baseUrl = "https://nominatim.openstreetmap.org";

        public GeoService(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri(_baseUrl);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", "aspnet-user-agent");
        }

        // 11. Desenvolva o endpoint GET /geo/status
        public async Task<object?> GetGeoStatus()
        {
            var response = await _client.GetAsync("/status.php?format=json");
            if (!response.IsSuccessStatusCode)
                return default(Object);
            var resContent = await response.Content.ReadFromJsonAsync<object>();
            return resContent;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<GeoDtoResponse?> GetGeoLocation(GeoDto geoDto)
        {
            var queryParams = new Dictionary<string, string?>()
            {
                { "street", geoDto.Address },
                { "city", geoDto.City },
                { "state", geoDto.State },
                { "country", "Brazil" },
                { "format", "json" },
                { "limit", "1" },
            };
            var queryParamsString = PassToQueryParamsString(queryParams);
            try
            {
                var response = await _client.GetAsync($"/search?{queryParamsString}");
                if (!response.IsSuccessStatusCode)
                    return default;
                var resContent = await response.Content.ReadFromJsonAsync<List<GeoDtoResponse>>();
                if (resContent is null || resContent.Count == 0)
                    return default;
                return resContent[0];
            }
            catch (Exception)
            {
                return default;
            }
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            var geoLocationOrigin = await GetGeoLocation(geoDto);
            List<HotelDto> hotels = repository.GetHotels().ToList();
            var hotelsGeoDistanceTasks = hotels.Select(async hotel =>
            {
                var hotelGeoLocation = await GetGeoLocation(new GeoDto
                {
                    Address = hotel.Address,
                    City = hotel.CityName,
                    State = hotel.State,
                });
                return new GeoDtoHotelResponse
                {
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityName = hotel.CityName,
                    State = hotel.State,
                    Distance = GetGeoDistance(geoLocationOrigin, hotelGeoLocation),
                };
            });
            var hotelsGeoDistance = (await Task.WhenAll(hotelsGeoDistanceTasks)).ToList();
            SortHotelsByGeoDistance(hotelsGeoDistance);
            return hotelsGeoDistance;
        }

        private int? GetGeoDistance(
            GeoDtoResponse? geoLocationOrigin,
            GeoDtoResponse? geoLocationDestiny
        )
        {
            GeoDtoResponse?[] geoLocations = { geoLocationOrigin, geoLocationDestiny };
            int? distance;
            if (geoLocations.All(x => x?.Lat is not null && x.Lon is not null))
                distance = CalculateDistance(
                    geoLocationOrigin!.Lat!,
                    geoLocationOrigin.Lon!,
                    geoLocationDestiny!.Lat!,
                    geoLocationDestiny.Lon!
                );
            else
                distance = null;
            return distance;
        }

        private static void SortHotelsByGeoDistance(List<GeoDtoHotelResponse> hotelsGeoDistance)
        {
            hotelsGeoDistance.Sort((x, y) =>
            {
                if (x.Distance == y.Distance)
                    return 0;
                if (x.Distance is not null && y.Distance is not null)
                    return x.Distance.Value.CompareTo(y.Distance.Value);
                if (x.Distance is not null && y.Distance is null)
                    return 1;
                return -1;
            });
        }

        public int CalculateDistance(string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny)
        {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.', ','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.', ','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.', ','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.', ','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return int.Parse(Math.Round(distance, 0).ToString());
        }

        public double radiano(double degree)
        {
            return degree * Math.PI / 180;
        }

        private static string PassToQueryParamsString(Dictionary<string, string?> queryParams)
        {
            var abc = queryParams.Select((x, index) =>
            {
                if (index == 0)
                    return string.Concat(x.Key, "=", x.Value);
                return string.Concat("&", x.Key, "=", x.Value);
            });
            return string.Concat(abc);
        }
    }
}
