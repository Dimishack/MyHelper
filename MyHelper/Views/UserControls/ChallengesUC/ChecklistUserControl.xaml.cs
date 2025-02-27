using System.Windows.Controls;

namespace MyHelper.Views.UserControls.ChallengesUC
{
    public partial class ChecklistUserControl : UserControl
    {
        public ChecklistUserControl() => InitializeComponent();

        private void DataGrid_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                dataGrid.Focus();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is not null)
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }
    }
}
