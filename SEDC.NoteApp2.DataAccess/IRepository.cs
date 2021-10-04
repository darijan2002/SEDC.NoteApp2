using System.Collections.Generic;

namespace SEDC.NoteApp2.DataAccess
{
    public interface IRepository<T>
    {
        //CRUD Methods
        List<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
