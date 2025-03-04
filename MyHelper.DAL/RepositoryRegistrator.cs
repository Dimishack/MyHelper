using Microsoft.Extensions.DependencyInjection;
using MyHelper.DAL.Entyties;
using MyHelper.Interfaces;

namespace MyHelper.DAL
{
    public static class RepositoryRegistrator
    {
        public static IServiceCollection AddRepositoriesInDB(this IServiceCollection services) => services
            .AddTransient<IRepository<Target>, DbRepository<Target>>()
            .AddTransient<IRepository<TargetsGroup>, DbRepository<TargetsGroup>>()
            .AddTransient<IRepository<Check>, DbRepository<Check>>()
            .AddTransient<IRepository<Challenge>, DbRepository<Challenge>>()
            .AddTransient<IRepository<MyTask>, DbRepository<MyTask>>()
            .AddTransient<IRepository<Film>, FilmRepository>()
            .AddTransient<IRepository<Genre>, DbRepository<Genre>>()
            .AddTransient<IRepository<FilmGenre>, DbRepository<FilmGenre>>()
            ;
    }
}
