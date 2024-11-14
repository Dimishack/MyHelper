using Microsoft.Extensions.DependencyInjection;

namespace MyHelper.ViewModels.Registrator_Locator
{
    internal static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) =>
            services.AddSingleton<MainWindowViewModel>();
    }
}
