using Microsoft.Extensions.DependencyInjection;

namespace MyHelper.ViewModels.Registrator_Locator
{
    internal class ViewModelsLocator
    {
        public static MainWindowViewModel MainWindowVM => App.Services.GetRequiredService<MainWindowViewModel>();
        public static ListPurposesUCViewModel ListPurposesVM => App.Services.GetRequiredService<ListPurposesUCViewModel>();
        public static ListTasksUCViewModel ListTasksVM => App.Services.GetRequiredService<ListTasksUCViewModel>();
        public static ListChallengesUCViewModel ListChallengesVM => App.Services.GetRequiredService<ListChallengesUCViewModel>();
        public static ListBooksUCViewModel ListBooksVM => App.Services.GetRequiredService<ListBooksUCViewModel>();
    }
}
