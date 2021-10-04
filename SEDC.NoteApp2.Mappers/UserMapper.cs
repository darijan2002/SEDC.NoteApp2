using SEDC.NoteApp2.Domain.Models;
using SEDC.NoteApp2.Dto.Models;
using SEDC.NoteApp2.Shared.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SEDC.NoteApp2.Mappers
{
    public static class UserMapper
    {
        public static User ToUser(this UserDto userDto)
        {
            User user = new User()
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Address = userDto.Address,
                Age = userDto.Age,
                Username = userDto.Username,
                Notes = new List<Note>()
            };

            if (userDto.Notes != null)
            {
                user.Notes = userDto.Notes.Select(x => x.ToNote()).ToList();
            }

            return user;
        }

        public static User ToUser(this RegisterUserDto userDto)
        {
            User user = new User()
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Address = userDto.Address,
                Age = userDto.Age,
                Username = userDto.Username,
                Password = userDto.Password.GenerateMD5(),
                Notes = new List<Note>()
            };

            return user;
        }

        public static UserDto ToUserDto(this User user)
        {
            UserDto userDto = new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Age = user.Age,
                Username = user.Username,
                Notes = new List<NoteDto>()
            };

            if (user.Notes != null)
            {
                userDto.Notes = user.Notes.Select(x => x.ToNoteDto()).ToList();
            }

            return userDto;
        }
    }
}
