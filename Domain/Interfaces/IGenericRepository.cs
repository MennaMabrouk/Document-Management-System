
    using Microsoft.EntityFrameworkCore.Update.Internal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Domain.Interfaces
    {
        public interface IGenericRepository<T> where T : class
        {
            //CRUD operations on any entity
           Task<ICollection<T>> GetAll();
           Task<T>   GetById(int id);

           Task Create(T entity);
           void Update(T entity);
           void Delete(T entity);

           Task<bool> EntityExists(int id);

           Task<bool> EntityExists(string name);



        }
    }
