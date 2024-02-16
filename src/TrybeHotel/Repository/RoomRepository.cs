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

        private static RoomDto GetRoomDto(Room room)
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
                    State = room.Hotel.City!.State,
                }
            };
        }

        // 7. Refatore o endpoint GET /room
        public IEnumerable<RoomDto> GetRooms(int hotelId)
        {
            var hotel = _context.Hotels
                .FirstOrDefault(hotel => hotel.HotelId == hotelId);
            if (hotel is null)
                throw new KeyNotFoundException("Hotel not found");
            return _context.Rooms
                .Where(room => room.HotelId == hotelId)
                .Include(room => room.Hotel)
                .ThenInclude(hotel => hotel!.City)
                .Select(room => GetRoomDto(room));
        }

        public RoomDto GetRoomById(int roomId)
        {
            var room = _context.Rooms
                .Include(room => room.Hotel)
                .ThenInclude(hotel => hotel!.City)
                .FirstOrDefault(room => room.RoomId == roomId);
            if (room is null)
                throw new KeyNotFoundException("Room not found");
            return GetRoomDto(room);
        }

        // 8. Refatore o endpoint POST /room
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

        public void DeleteRoom(int RoomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId);
            if (room is null)
                throw new KeyNotFoundException("Room not found");
            _context.Rooms.Remove(room);
            _context.SaveChanges();
        }
    }
}
