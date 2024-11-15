using System.Linq;
using System.Threading.Tasks;

namespace MyHelper.Interfaces
{
    public interface IRepository<T> where T : class, IEntity, new()
    {
        bool AutoSaveChanges { get; set; }
        IQueryable<T> Items { get; }

        T Get(int id);
        Task<T> GetAsync(int id);

        T Add(T item);
        Task<T> AddAsync(T item);

        bool Update(T item);
        Task<bool> UpdateAsync(T item);

        void Remove(int id);
        Task RemoveAsync(int id);

        void SaveChanged();
        Task SaveChangedAsync();
    }
}
