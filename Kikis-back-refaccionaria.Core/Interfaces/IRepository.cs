namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IRepository<T> where T : class {

        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        IQueryable<T> GetQuery();
    }
}
