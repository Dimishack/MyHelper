using System.Windows.Controls;
using System.Windows.Input;

namespace MyHelper.Views.UserControls
{
    public partial class TasksUserControl : UserControl
    {
        public TasksUserControl() => InitializeComponent();

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) 
            => e.Handled = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
    }
}
