using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public interface IRoomRepository
    {
        IEnumerable<RoomDto> GetRooms(int HotelId);
        RoomDto GetRoomById(int roomId);
        RoomDto AddRoom(Room room);
        void DeleteRoom(int RoomId);
        bool RoomExists(int roomId);
    }
}