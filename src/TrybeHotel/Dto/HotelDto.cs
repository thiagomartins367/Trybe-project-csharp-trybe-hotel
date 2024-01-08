namespace TrybeHotel.Dto
{
    public class HotelDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int CityId { get; set; }
        public string CityName { get; set; } = null!;
    }
}
