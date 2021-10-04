using Microsoft.EntityFrameworkCore;
using SEDC.NoteApp2.DataAccess.Interfaces;
using SEDC.NoteApp2.Domain;
using SEDC.NoteApp2.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace SEDC.NoteApp2.DataAccess.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private NotesDbContext _notesAppDbContext;

        public NoteRepository(NotesDbContext notesAppDbContext)
        {
            _notesAppDbContext = notesAppDbContext;
        }

        public void Add(Note entity)
        {
            _notesAppDbContext.Notes.Add(entity);
            _notesAppDbContext.SaveChanges();
        }

        public void Delete(Note entity)
        {
            _notesAppDbContext.Notes.Remove(entity);
            _notesAppDbContext.SaveChanges();
        }

        public List<Note> GetAll()
        {
            return _notesAppDbContext
                .Notes
                .Include(x => x.User) //join with table users
                .ToList();
        }

        public List<Note> GetAllByUserId(int userId)
        {
            return _notesAppDbContext
                .Notes
                .Where(q => q.UserId == userId) // where condition in sql
                .Include(x => x.User) //join with table users
                .ToList();
        }

        public Note GetById(int id)
        {
            return _notesAppDbContext
                .Notes
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id);
        }

        public void Update(Note entity)
        {
            _notesAppDbContext.Notes.Update(entity);
            _notesAppDbContext.SaveChanges();
        }
    }
}
