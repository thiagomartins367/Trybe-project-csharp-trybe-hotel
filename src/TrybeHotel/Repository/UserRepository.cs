using TrybeHotel.Models;
using TrybeHotel.Dto;
using System.ComponentModel;
using System.Text.Json;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public UserDto Login(LoginDto login)
        {
            var user = _context.Users.FirstOrDefault(
                user => user.Email == login.Email
                && user.Password == login.Password
            );
            if (user is null)
                throw new KeyNotFoundException("User not found");
            return PassUserEntityToOutput(user);
        }

        public UserDto Add(UserDtoInsert user)
        {
            User newUser = PassInputToUserEntity(user);
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return PassUserEntityToOutput(newUser);
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var user = _context.Users.FirstOrDefault(user => user.Email == userEmail);
            if (user is null)
                throw new KeyNotFoundException("User not found");
            return PassUserEntityToOutput(user);
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _context.Users.Select(
                user => PassUserEntityToOutput(user)
            );
        }

        private static User PassInputToUserEntity(UserDtoInsert input)
        {
            return new User
            {
                Name = input.Name,
                Email = input.Email,
                Password = input.Password,
                UserType = "client",
            };
        }

        private static UserDto PassUserEntityToOutput(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                UserType = user.UserType,
            };
        }
    }
}
