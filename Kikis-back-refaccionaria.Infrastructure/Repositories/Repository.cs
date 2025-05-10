using Kikis_back_refaccionaria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class Repository<T> : IRepository<T> where T : class {

        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(DbContext context) {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll() {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetById(int id) {
            return await _dbSet.FindAsync(id);
        }

        public void Add(T entity) {
            _dbSet.AddAsync(entity);
        }

        public void AddRange(IEnumerable<T> entities) {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity) {
            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities) {
            _dbSet.UpdateRange(entities);
        }

        public void Delete(T entity) {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities) {
            _dbSet.RemoveRange(entities);
        }

        public IQueryable<T> GetQuery() {
            return _dbSet.AsQueryable();
        }
    }
}
