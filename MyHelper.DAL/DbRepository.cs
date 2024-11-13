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

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task AddAsync(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        public T Get(int id) => Items.SingleOrDefault(x => x.Id == id);

        public async Task<T> GetAsync(int id) => await Items.SingleOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

        public void Remove(int id)
        {
            _db.Remove(new T { Id = id });
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            _db.Remove(new T { Id = id });
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
    }

    internal class TargetsRepository : DbRepository<TargetsGroup>
    {
        public override IQueryable<TargetsGroup> Items => base.Items.Include(item => item.Targets);
        public TargetsRepository(MyHelperDB db) : base(db) { }
    }

}
