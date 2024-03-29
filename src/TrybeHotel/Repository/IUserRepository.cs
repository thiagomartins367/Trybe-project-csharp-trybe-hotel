using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public interface IUserRepository
    {
        UserDto GetUserById(int userId);
        UserDto Add(UserDtoInsert user);
        UserDto Login(LoginDto login);
        UserDto GetUserByEmail(string userEmail);
        IEnumerable<UserDto> GetUsers();
        bool UserExists(int userId);
        bool UserExists(string userEmail);
    }

}