using System.Windows.Controls;

namespace MyHelper.Views.UserControls
{
    public partial class FilmsUserControl : UserControl
    {
        public FilmsUserControl() => InitializeComponent();

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => listFilms.ScrollIntoView(listFilms.Items[0]);
    }
}
