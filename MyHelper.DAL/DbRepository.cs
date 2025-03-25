using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Context;
using MyHelper.DAL.Entyties;
using MyHelper.DAL.Entyties.Base;
using MyHelper.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyHelper.DAL
{
    internal class DbRepository<T> : IRepository<T> where T : Entity, new()
    {
        private readonly MyHelperDB _db;
        private readonly DbSet<T> _dbSet;

        public bool AutoSaveChanges { get; set; } = true;

        public DbRepository(MyHelperDB db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }
        public virtual IQueryable<T> Items => _dbSet;

        public T Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
            return item;
        }

        public async Task<T> AddAsync(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync().ConfigureAwait(false);
            return item;
        }

        public T Get(int id) => Items.SingleOrDefault(x => x.Id == id);

        public async Task<T> GetAsync(int id) => await Items.SingleOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

        public void Remove(int id)
        {
            var item = _dbSet.Local.FirstOrDefault(item => item.Id == id) ?? new T() { Id = id };
            _dbSet.Remove(item);
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var item = _dbSet.Local.FirstOrDefault(item => item.Id == id) ?? new T() { Id = id };
            _db.Remove(item);
            if (AutoSaveChanges)
                await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        public bool Update(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            try
            {
                _db.Entry(item).State = EntityState.Modified;
                if (AutoSaveChanges)
                    _db.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> UpdateAsync(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            try
            {
                _db.Entry(item).State = EntityState.Modified;
                if (AutoSaveChanges)
                    await _db.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception) { return false; }
        }

        public IQueryable<T> CustomFromSQLRaw(string sql, params object[] parameters) => _dbSet.FromSqlRaw(sql, parameters);

        public void SaveChanged() => _db.SaveChanges();
        public async Task SaveChangedAsync() => await _db.SaveChangesAsync();
    }

    internal class FilmRepository : DbRepository<Film>
    {
        public override IQueryable<Film> Items => base.Items.Include(item => item.FilmGenres).ThenInclude(mg => mg.Genre);
        public FilmRepository(MyHelperDB db) : base(db) { }
    }
}
