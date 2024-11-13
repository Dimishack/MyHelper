using Microsoft.Extensions.DependencyInjection;
using MyHelper.DAL.Entyties;
using MyHelper.Interfaces;

namespace MyHelper.DAL
{
    public static class RepositoryRegistrator
    {
        public static IServiceCollection AddRepositoriesInDB(this IServiceCollection services) => services
            .AddTransient<IRepository<TargetsGroup>, TargetsRepository>()
            ;
    }
}
