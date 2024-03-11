using System.Windows;
using System.Windows.Input;

namespace MyHelper.Views.Windows
{
    public partial class Creator_EditorYearWindow : Window
    {
        public Creator_EditorYearWindow() => InitializeComponent();

        #region Title : string - Заголовок окна

        public static new readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Creator_EditorYearWindow), new PropertyMetadata(default(string)));

        public new string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

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
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key < Key.D0 || e.Key > Key.D9) &&
                (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                && e.Key != Key.Back)
                e.Handled = true;
        }
    }
}
