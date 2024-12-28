using System.Windows;
using System.Windows.Input;

namespace MyHelper
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => e.Handled = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
    }
}
