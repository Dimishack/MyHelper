using System.Linq;
using System.Threading.Tasks;

namespace MyHelper.Interfaces
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public interface IRepository<T> where T : class, IEntity, new()
    {
        IQueryable<T> Items { get; }

        T Get(int id);
        Task<T> GetAsync(int id);

        void Add(T item);
        Task AddAsync(T item);

        bool Update(T item);
        Task<bool> UpdateAsync(T item);

        void Remove(int id);
        Task RemoveAsync(int id);
    }
}
