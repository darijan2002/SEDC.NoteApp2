using SEDC.NoteApp2.Domain.Models;
using System.Collections.Generic;

namespace SEDC.NoteApp2.DataAccess.Interfaces
{
    public interface INoteRepository : IRepository<Note>
    {
        List<Note> GetAllByUserId(int userId);
    }
}
