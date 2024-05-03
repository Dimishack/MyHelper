using System.Windows;

namespace MyHelper.Views.Windows
{
    public partial class Creator_EditorYearWindow : Window
    {
        public Creator_EditorYearWindow() => InitializeComponent();

        #region Year : int - Год

        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(int), typeof(Creator_EditorYearWindow), new PropertyMetadata(0));

        public int Year
        {
            get => (int)GetValue(YearProperty);
            set => SetValue(YearProperty, value);
        }
        #endregion

        #region NameYear : string - Название года

        public static readonly DependencyProperty NameYearProperty =
            DependencyProperty.Register("NameYear", typeof(string), typeof(Creator_EditorYearWindow), new PropertyMetadata(default(string)));

        public string NameYear
        {
            get => (string)GetValue(NameYearProperty);
            set => SetValue(NameYearProperty, value);
        }

        #endregion

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) => e.Handled = NameYear.Length >= 50;
    }
}
