using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;
using TrybeHotel.Errors.ApiExceptions;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        private readonly IUserRepository _userRepository;

        public BookingRepository(ITrybeHotelContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            UserDto user = _userRepository.GetUserByEmail(email);
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
            UserDto user = _userRepository.GetUserByEmail(email);
            var booking = _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(room => room!.Hotel)
                .ThenInclude(hotel => hotel!.City)
                .FirstOrDefault(b => b.BookingId == bookingId);
            if (booking is null) throw new NotFoundException("Booking not found");
            if (user.UserId != booking.UserId)
                throw new UnauthorizedException("Booking not belong this user");
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
                        State = booking.Room!.Hotel!.City!.State,
                    }
                },
            };
        }
    }
}
