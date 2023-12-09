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
            return CopyEqualPropertiesFromUser(user, new UserDto(), new { });
        }

        public UserDto Add(UserDtoInsert user)
        {
            var newUser = new User()
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = "client",
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return CopyEqualPropertiesFromUser(newUser, new UserDto(), new { });
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var user = _context.Users.FirstOrDefault(user => user.Email == userEmail);
            if (user is null)
                throw new KeyNotFoundException("User not found");
            return CopyEqualPropertiesFromUser(user, new UserDto(), null);
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _context.Users.Select(
                user => CopyEqualPropertiesFromUser(user, new UserDto(), new { })
            );
        }

        private static T CopyEqualPropertiesFromUser<T>(
            User user,
            T expectedObject,
            object? defaultValues
        )
        {
            if (expectedObject is null) return expectedObject;
            var expectedObjectCopied = FindAndCopyProperties(user, expectedObject, null);
            expectedObjectCopied = FindAndCopyProperties(
                user,
                expectedObjectCopied,
                defaultValues
            );
            return expectedObjectCopied;
        }

        private static T FindAndCopyProperties<T>(
            User user,
            T expectedObject,
            object? defaultValues
        )
        {
            var objectToIterate = defaultValues is null ? expectedObject : defaultValues;
            if (objectToIterate is null) return expectedObject;
            var userProperties = user.GetType().GetProperties();
            foreach (var property in objectToIterate.GetType().GetProperties())
            {
                var userProperty = userProperties
                    .FirstOrDefault(
                        propertyInfo => propertyInfo.Name == property.Name
                        && propertyInfo.GetType() == property.GetType()
                    );
                if (userProperty is not null)
                    property.SetValue(expectedObject, userProperty.GetValue(user));
            }
            return expectedObject;
        }
    }
}
