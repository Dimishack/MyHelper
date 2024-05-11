using System.Windows;

namespace MyHelper.Views.Windows
{
    public partial class ChecklistChallengeWindow : Window
    {
        public ChecklistChallengeWindow() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            datagrid.Focus();
            if (datagrid.SelectedItem is not null)
                datagrid.ScrollIntoView(datagrid.SelectedItem);
        }
    }
}
