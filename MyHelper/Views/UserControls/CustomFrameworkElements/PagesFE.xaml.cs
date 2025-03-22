using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyHelper.Views.UserControls.CustomFrameworkElements
{
    public partial class PagesFE : UserControl
    {
        public PagesFE() => InitializeComponent();

        #region Pages : IList<int> - Страницы

        ///<summary> Страницы (DependencyProperty). </summary>
        public static readonly DependencyProperty PagesProperty =
            DependencyProperty.Register(nameof(Pages),
                typeof(IList<int>),
                typeof(PagesFE),
                new PropertyMetadata(
                    new List<int>()));

        ///<summary> Страницы. </summary>
        public IList<int> Pages
        {
            get => (IList<int>)GetValue(PagesProperty);
            set => SetValue(PagesProperty, value);
        }

        #endregion

        #region SourceMainListBox : ListBox - Основной список

        ///<summary> Основной список (DependencyProperty). </summary>
        public static readonly DependencyProperty SourceMainListBoxProperty =
            DependencyProperty.Register(nameof(SourceMainListBox), typeof(ListBox), typeof(PagesFE), new PropertyMetadata(null));

        ///<summary> Основной список. </summary>
        public ListBox SourceMainListBox
        {
            get => (ListBox)GetValue(SourceMainListBoxProperty);
            set => SetValue(SourceMainListBoxProperty, value);
        }

        #endregion

        #region CurrentPage : int - Текущая страница

        ///<summary> Текущая страница (DependencyProperty). </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int?), typeof(PagesFE), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentPageChanged));

        ///<summary> Текущая страница. </summary>
        public int? CurrentPage
        {
            get => (int?)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PagesFE uc)
            {
                if(uc.SourceMainListBox is not null
                && uc.SourceMainListBox.Items.Count > 0)
                    uc.SourceMainListBox.ScrollIntoView(uc.SourceMainListBox.Items[0]);
            }    
        }

        #endregion

        #region LastPage : int - Последняя страница

        ///<summary> Последняя страница (DependencyProperty). </summary>
        public static readonly DependencyProperty LastPageProperty =
            DependencyProperty.Register(nameof(LastPage), typeof(int), typeof(PagesFE), new PropertyMetadata(0));

        ///<summary> Последняя страница. </summary>
        public int LastPage
        {
            get => (int)GetValue(LastPageProperty);
            set => SetValue(LastPageProperty, value);
        }

        #endregion

        #region SelectedPage : string - Выбранная страница

        ///<summary> Выбранная страница (DependencyProperty). </summary>
        public static readonly DependencyProperty SelectedPageProperty =
            DependencyProperty.Register(nameof(SelectedPage), typeof(string), typeof(PagesFE), new PropertyMetadata(string.Empty));

        ///<summary> Выбранная страница. </summary>
        public string SelectedPage
        {
            get => (string)GetValue(SelectedPageProperty);
            set => SetValue(SelectedPageProperty, value);
        }

        #endregion

        private void OnPreviousPage(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }
        private void OnNextPage(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < LastPage)
                CurrentPage++;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key < Key.D0 || e.Key > Key.D9)
                && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                && e.Key != Key.Enter)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Enter
                && Int32.TryParse(SelectedPage, out int selectedPage)
                && selectedPage != CurrentPage)
            {
                if (LastPage > 10)
                {
                    Pages.Clear();
                    int start = selectedPage / 5 <= 0
                        ? 1
                        : LastPage - selectedPage <= 4
                        ? LastPage - 9
                        : selectedPage - 4;
                    foreach (var page in Enumerable.Range(start, 10))
                        Pages.Add(page); 
                }
                CurrentPage = selectedPage < 1
                    ? 1
                    : selectedPage > LastPage
                    ? LastPage
                    : selectedPage;
                SelectedPage = string.Empty;
            }
        }
    }
}
