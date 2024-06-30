using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyHelper.Views.Windows.Books
{
    public partial class Creator_EditorBookWindow : Window
    {
        public Creator_EditorBookWindow() => InitializeComponent();

        public string? Author
        {
            get { return (string?)GetValue(AuthorProperty); }
            set { SetValue(AuthorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Author.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AuthorProperty =
            DependencyProperty.Register("Author", typeof(string), typeof(Creator_EditorBookWindow), new PropertyMetadata(null));

        public string? NameBook
        {
            get { return (string?)GetValue(NameBookProperty); }
            set { SetValue(NameBookProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NameBook.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameBookProperty =
            DependencyProperty.Register("NameBook", typeof(string), typeof(Creator_EditorBookWindow), new PropertyMetadata(null));



        public ushort Pages
        {
            get { return (ushort)GetValue(PagesProperty); }
            set { SetValue(PagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Pages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagesProperty =
            DependencyProperty.Register("Pages", typeof(ushort), typeof(Creator_EditorBookWindow), new PropertyMetadata(default(ushort)));

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key < Key.D0 || e.Key > Key.D9)
                && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9);
                ;
        }
    }
}
