using Microsoft.EntityFrameworkCore;
using SEDC.NoteApp2.DataAccess.Interfaces;
using SEDC.NoteApp2.Domain;
using SEDC.NoteApp2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SEDC.NoteApp2.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private NotesDbContext _notesAppDbContext;

        public UserRepository(NotesDbContext notesAppDbContext)
        {
            _notesAppDbContext = notesAppDbContext;
        }
        public void Add(User entity)
        {
            _notesAppDbContext.Users.Add(entity);
            _notesAppDbContext.SaveChanges();
        }

        public void Delete(User entity)
        {
            _notesAppDbContext.Users.Remove(entity);
            _notesAppDbContext.SaveChanges();
        }

        public List<User> GetAll()
        {
            return _notesAppDbContext
                .Users
                .ToList();
        }

        public List<User> GetAllIncludeNotes()
        {
            return _notesAppDbContext
                 .Users
                 .Include(x => x.Notes) //join with table notes
                 .ToList();
        }

        public User GetById(int id)
        {
            return _notesAppDbContext
               .Users
               .FirstOrDefault(x => x.Id == id);
        }

        public User GetByIdIncludeNotes(int id)
        {
            return _notesAppDbContext
               .Users
               .Include(x => x.Notes) //join with table notes
               .FirstOrDefault(x => x.Id == id);
        }

        public User GetUserByUsernameAndPassword(string username, string password)
        {
            return _notesAppDbContext
                .Users
                .SingleOrDefault(x => x.Username == username && x.Password == password);
        }

        public void ChangePassword(int userId, string newPasswordHashed)
        {
            _notesAppDbContext.Users.Single(u => u.Id == userId).Password = newPasswordHashed;
            _notesAppDbContext.SaveChanges();
        }

        public bool IsUsernameInUse(string username)
        {
            return _notesAppDbContext
                .Users
                .Any(q => q.Username == username);
        }

        public void Update(User entity)
        {
            _notesAppDbContext.Users.Update(entity);
            _notesAppDbContext.SaveChanges();
        }
    }
}
