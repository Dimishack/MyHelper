using Microsoft.Extensions.DependencyInjection;
using MyHelper.Services.Interfaces;

namespace MyHelper.Services.Registrator
{
    internal static class ServicesRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) => services
            .AddSingleton<IUserDialog, UserDialogServices>()
            ;
    }
}
