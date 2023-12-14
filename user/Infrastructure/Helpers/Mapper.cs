using user.Data.Entities;
using user.Models;

namespace user.Infrastructure.Helpers
{
    public class Mapper
    {
        public UserDto MapToUserDto(User user)
        {
            var userDto = new UserDto()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName
            };

            return userDto;
        }
    }
}