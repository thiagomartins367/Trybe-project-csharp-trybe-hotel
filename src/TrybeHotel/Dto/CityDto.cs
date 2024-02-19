namespace TrybeHotel.Dto
{
    public class CityDto
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public string State { get; set; } = null!;
    }

    public class CityDtoInsert
    {
        public string Name { get; set; } = null!;
        public string State { get; set; } = null!;
    }
}
