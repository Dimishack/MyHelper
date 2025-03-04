using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Entyties;

namespace MyHelper.DAL.Context
{
    public class MyHelperDB : DbContext
    {
        public DbSet<TargetsGroup> TargetsGroups { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<MyTask> Tasks { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<FilmGenre> FilmGenres { get; set; }

        public MyHelperDB(DbContextOptions<MyHelperDB> options) : base(options) { }

    }
}
