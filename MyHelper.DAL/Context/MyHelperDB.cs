using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Entyties;

namespace MyHelper.DAL.Context
{
    public class MyHelperDB : DbContext
    {
        public DbSet<TargetsGroup> TargetsGroups { get; set; }

        public MyHelperDB(DbContextOptions<MyHelperDB> options) : base(options) { }

    }
}
