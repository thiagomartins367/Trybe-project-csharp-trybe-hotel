using TrybeHotel.Models;
using TrybeHotel.Dto;
using TrybeHotel.Errors.ApiExceptions;

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
            bool VerifyLoginIsBinaryEqual(User userToValidation)
            {
                return StringIsBinaryEqual(login.Email, userToValidation.Email)
                    && StringIsBinaryEqual(login.Password, userToValidation.Password);
            }
            var user = _context.Users.FirstOrDefault(VerifyLoginIsBinaryEqual);
            if (user is null)
                throw new NotFoundException("Incorrect email or password");
            return PassUserEntityToOutput(user);
        }

        public UserDto Add(UserDtoInsert user)
        {
            if (UserExists(user.Email))
                throw new ConflictException("User email already exists");
            User newUser = PassInputToUserEntity(user);
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return PassUserEntityToOutput(newUser);
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            bool VerifyEmailIsBinaryEqual(User userToValidation)
            {
                return StringIsBinaryEqual(userEmail, userToValidation.Email);
            }
            var user = _context.Users.FirstOrDefault(VerifyEmailIsBinaryEqual);
            if (user is null)
                throw new NotFoundException("User not found");
            return PassUserEntityToOutput(user);
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _context.Users.Select(
                user => PassUserEntityToOutput(user)
            );
        }

        public bool UserExists(int userId)
        {
            var user = _context.Users.FirstOrDefault(user => user.UserId == userId);
            if (user is null) return false;
            return true;
        }

        public bool UserExists(string userEmail)
        {
            bool VerifyEmailIsBinaryEqual(User userToValidation)
            {
                return StringIsBinaryEqual(userEmail, userToValidation.Email);
            }
            var user = _context.Users.FirstOrDefault(VerifyEmailIsBinaryEqual);
            if (user is null) return false;
            return true;
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

        private static bool StringIsBinaryEqual(string StringA, string StringB)
        {
            return string.Equals(StringA, StringB, StringComparison.Ordinal);
        }
    }
}
