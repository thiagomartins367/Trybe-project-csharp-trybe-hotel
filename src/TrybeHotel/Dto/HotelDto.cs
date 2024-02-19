namespace TrybeHotel.Dto
{
    public class HotelDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int CityId { get; set; }
        public string CityName { get; set; } = null!;
        public string State { get; set; } = null!;
    }

    public class HotelDtoInsert
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int CityId { get; set; }
    }
}
