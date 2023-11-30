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

        private RoomDto GetRoomDto(Room room)
        {
            return new RoomDto()
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Capacity = room.Capacity,
                Image = room.Image,
                Hotel = new HotelDto()
                {
                    HotelId = room.Hotel!.HotelId,
                    Name = room.Hotel.Name,
                    Address = room.Hotel.Address,
                    CityId = room.Hotel.CityId,
                    CityName = room.Hotel.City!.Name,
                }
            };
        }

        // 6. Desenvolva o endpoint GET /room/:hotelId
        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            return _context.Rooms
                .Where(room => room.HotelId == HotelId)
                .Include(room => room.Hotel)
                .ThenInclude(hotel => hotel!.City)
                .Select(room => GetRoomDto(room));
        }

        // 7. Desenvolva o endpoint POST /room
        public RoomDto AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            Room newRoom = _context.Rooms
                .Include(r => r.Hotel)
                .ThenInclude(hotel => hotel!.City)
                .First(r => r.RoomId == room.RoomId);
            return GetRoomDto(newRoom);
        }

        // 8. Desenvolva o endpoint DELETE /room/:roomId
        public void DeleteRoom(int RoomId)
        {
            var room = _context.Rooms.First(r => r.RoomId == RoomId);
            _context.Rooms.Remove(room);
            _context.SaveChanges();
        }
    }
}