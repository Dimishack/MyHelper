using System.Windows.Controls;

namespace MyHelper.Views.UserControls
{
    public partial class FilmsUserControl : UserControl
    {
        public FilmsUserControl() => InitializeComponent();

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listFilms.Items.Count > 0)
                listFilms.ScrollIntoView(listFilms.Items[0]);
        }
    }
}
