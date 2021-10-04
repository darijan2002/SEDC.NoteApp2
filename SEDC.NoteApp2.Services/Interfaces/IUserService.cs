using SEDC.NoteApp2.Dto.Models;
using System.Collections.Generic;

namespace SEDC.NoteApp2.Services.Interfaces
{
    public interface IUserService
    {
        List<UserDto> GetAllUsers();
        List<UserDto> GetAllUsersIncludeNotes();
        UserDto GetUserById(int id);
        UserDto GetUserByIdIncludeNotes(int id);
        void AddUser(RegisterUserDto userDto);
        void UpdateUser(UserDto userDto);
        void DeleteUser(int id);
        TokenDto Authenticate(string username, string password);
        bool ChangeUserPassword(int userId, string password, string newPassword);
    }
}
