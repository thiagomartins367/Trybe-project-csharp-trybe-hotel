using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;

        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            User user = GetUserByEmail(email);
            var bookingToInsert = new Booking()
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                UserId = user.UserId,
                RoomId = booking.RoomId,
            };
            _context.Bookings.Add(bookingToInsert);
            _context.SaveChanges();
            return GetBooking(bookingToInsert.BookingId, email);
        }

        public BookingResponse GetBooking(int bookingId, string email)
        {
            User user = GetUserByEmail(email);
            var booking = _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(room => room!.Hotel)
                .ThenInclude(hotel => hotel!.City)
                .FirstOrDefault(b => b.BookingId == bookingId);
            if (booking is null) throw new KeyNotFoundException("booking not found");
            if (user.UserId != booking.UserId)
                throw new UnauthorizedAccessException("booking not belong this user");
            return new BookingResponse()
            {
                BookingId = booking.BookingId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = new RoomDto()
                {
                    RoomId = booking.Room!.RoomId,
                    Name = booking.Room.Name,
                    Capacity = booking.Room.Capacity,
                    Image = booking.Room.Image,
                    Hotel = new HotelDto()
                    {
                        HotelId = booking.Room!.Hotel!.HotelId,
                        Name = booking.Room!.Hotel!.Name,
                        Address = booking.Room!.Hotel!.Address,
                        CityId = booking.Room!.Hotel!.City!.CityId,
                        CityName = booking.Room!.Hotel!.City!.Name,
                    }
                },
            };
        }

        public Room GetRoomById(int RoomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId);
            if (room is null) throw new KeyNotFoundException("Room not found");
            return room;
        }

        private User GetUserByEmail(string userEmail)
        {
            var user = _context.Users.First(user => user.Email == userEmail);
            return user;
        }
    }
}
