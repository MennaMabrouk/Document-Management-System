using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DataContext _context;
        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        public async Task Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public async Task<bool> EntityExists(int id)
        {
            var keyProperty = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.FirstOrDefault();
            if (keyProperty == null)
            {
                throw new InvalidOperationException($"No primary key defined for {typeof(T).Name}");
            }

            var keyName = keyProperty.Name;

            return await _context.Set<T>().AnyAsync(e => EF.Property<int>(e, keyName) == id);
        }

        public async Task<bool> EntityExists(string name)
        {
            var nameProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Equals("Name", StringComparison.OrdinalIgnoreCase)
                                     && p.PropertyType == typeof(string));
            if (nameProperty == null)
            {
                throw new InvalidOperationException($"No suitable 'Name' property of type string defined for {typeof(T).Name}");
            }

            return await _context.Set<T>().AnyAsync(e => EF.Property<string>(e, nameProperty.Name) == name);
        }


        public async Task<ICollection<T>> GetAll()
        {
           return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);

        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
