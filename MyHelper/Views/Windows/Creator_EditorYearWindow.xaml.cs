using System.Windows;

namespace MyHelper.Views.Windows
{
    public partial class Creator_EditorYearWindow : Window
    {

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

        public Creator_EditorYearWindow() => InitializeComponent();
    }
}
