using System.Windows;

namespace MyHelper.Views.Windows
{
    public partial class Creator_EditorPurposeWindow : Window
    {
        #region Title : string - Заголовок окна

        public static new readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(Creator_EditorPurposeWindow),
                new PropertyMetadata(default(string)));

        public new string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region Purpose : string - Цель

        public static readonly DependencyProperty PurposeProperty =
            DependencyProperty.Register(
                "Purpose", 
                typeof(string), 
                typeof(Creator_EditorPurposeWindow),
                new PropertyMetadata(default(string)));

        public string Purpose
        {
            get => (string)GetValue(PurposeProperty);
            set => SetValue(PurposeProperty, value);
        }

        #endregion

        #region Note : string - Примечание

        public static readonly DependencyProperty NoteProperty =
            DependencyProperty.Register(
                "Note", 
                typeof(string), 
                typeof(Creator_EditorPurposeWindow),
                new PropertyMetadata(default(string)));

        public string Note
        {
            get => (string)GetValue(NoteProperty);
            set => SetValue(NoteProperty, value);
        }

        #endregion

        public Creator_EditorPurposeWindow() => InitializeComponent();
    }
}
