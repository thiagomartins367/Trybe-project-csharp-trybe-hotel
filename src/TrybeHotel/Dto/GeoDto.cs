using System.ComponentModel.DataAnnotations;

namespace TrybeHotel.Dto
{
    public class GeoDto
    {
        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string State { get; set; } = null!;
    }

    public class GeoDtoResponse
    {
        public string? Lat { get; set; }
        public string? Lon { get; set; }
    }

    public class GeoDtoHotelResponse
    {
        public int HotelId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? CityName { get; set; }
        public string? State { get; set; }
        public int? Distance { get; set; }
    }
}