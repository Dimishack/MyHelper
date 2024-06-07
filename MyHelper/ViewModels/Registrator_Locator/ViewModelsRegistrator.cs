using Microsoft.Extensions.DependencyInjection;
using MyHelper.Views.Windows;

namespace MyHelper.ViewModels.Registrator_Locator
{
    internal static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<ListChallengesUCViewModel>()
                .AddTransient<ChecklistChallengeViewModel>()
                .AddSingleton<ListPurposesUCViewModel>()
                .AddSingleton<ListTasksUCViewModel>()
                ;

            services.AddTransient(
                s =>
                {
                    var model = s.GetRequiredService<ChecklistChallengeViewModel>();
                    var window = new ChecklistChallengeWindow { DataContext = model };
                    return window;
                });

            return services;
        }
    }
}
