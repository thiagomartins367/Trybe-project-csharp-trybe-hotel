using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class HotelRepository : IHotelRepository
    {
        protected readonly ITrybeHotelContext _context;
        public HotelRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        //  5. Refatore o endpoint GET /hotel
        public IEnumerable<HotelDto> GetHotels()
        {
            return _context.Hotels.Select(hotel => new HotelDto()
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                CityId = hotel.CityId,
                CityName = hotel.City!.Name,
                State = hotel.City!.State,
            });
        }

        // 6. Refatore o endpoint POST /hotel
        public HotelDto AddHotel(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            _context.SaveChanges();
            Hotel newHotel = _context.Hotels.Include(h => h.City)
                .First(h => h.HotelId == hotel.HotelId);
            return new HotelDto()
            {
                HotelId = newHotel.HotelId,
                Name = newHotel.Name,
                Address = newHotel.Address,
                CityId = newHotel.CityId,
                CityName = newHotel.City!.Name,
                State = newHotel.City!.State,
            };
        }

        public bool HotelExists(int HotelId)
        {
            var hotel = _context.Hotels.FirstOrDefault(hotel => hotel.HotelId == HotelId);
            if (hotel is null) return false;
            return true;
        }
    }
}
