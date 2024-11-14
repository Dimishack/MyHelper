using Microsoft.Extensions.DependencyInjection;

namespace MyHelper.ViewModels.Registrator_Locator
{
    internal class ViewModelsLocator
    {
        public static MainWindowViewModel MainWindowVM => App.Services.GetRequiredService<MainWindowViewModel>();
    }
}
