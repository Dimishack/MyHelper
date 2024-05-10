using Microsoft.Extensions.DependencyInjection;

namespace MyHelper.ViewModels.Registrator_Locator
{
    internal class ViewModelsLocator
    {
        public static MainWindowViewModel MainWindowViewModel => App.Services.GetRequiredService<MainWindowViewModel>();
        public static ListChallengesUCViewModel ListChallengesVM => App.Services.GetRequiredService<ListChallengesUCViewModel>();
    }
}
