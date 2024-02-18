namespace TrybeHotel.Dto
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public string Image { get; set; } = null!;
        public HotelDto Hotel { get; set; } = null!;
    }

    public class RoomDtoInsert
    {
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public string Image { get; set; } = null!;
        public int HotelId { get; set; }
    }
}
