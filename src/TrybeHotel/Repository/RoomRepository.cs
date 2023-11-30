using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 6. Desenvolva o endpoint GET /room/:hotelId
        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            return _context.Rooms
                .Where(room => room.HotelId == HotelId)
                .Include(room => room.Hotel)
                .ThenInclude(hotel => hotel!.City)
                .ToList()
                .Select(
                    r =>
                    {
                        return new RoomDto()
                        {
                            RoomId = r.RoomId,
                            Name = r.Name,
                            Capacity = r.Capacity,
                            Image = r.Image,
                            Hotel = new HotelDto()
                            {
                                HotelId = r.Hotel!.HotelId,
                                Name = r.Hotel.Name,
                                Address = r.Hotel.Address,
                                CityId = r.Hotel.CityId,
                                CityName = r.Hotel.City!.Name,
                            }
                        };
                    }
                );
        }

        // 7. Desenvolva o endpoint POST /room
        public RoomDto AddRoom(Room room) {
            throw new NotImplementedException(); 
        }

        // 8. Desenvolva o endpoint DELETE /room/:roomId
        public void DeleteRoom(int RoomId) {
            throw new NotImplementedException();
        }
    }
}