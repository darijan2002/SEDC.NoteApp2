using SEDC.NoteApp2.Domain.Models;
using System.Collections.Generic;

namespace SEDC.NoteApp2.DataAccess.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        List<User> GetAllIncludeNotes();
        User GetByIdIncludeNotes(int userId);
        bool IsUsernameInUse(string username);
        User GetUserByUsernameAndPassword(string username, string password);
        void ChangePassword(int userId, string newPasswordHashed);
    }
}
