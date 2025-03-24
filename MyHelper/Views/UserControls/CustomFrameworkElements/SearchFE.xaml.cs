using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyHelper.Views.UserControls.CustomFrameworkElements
{
    public partial class SearchFE : UserControl
    {
        public SearchFE() => InitializeComponent();

        #region SearchCommand : ICommand? - Команда - искать

        ///<summary> Команда - искать (DependencyProperty). </summary>
        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register(nameof(SearchCommand), typeof(ICommand), typeof(SearchFE), new PropertyMetadata(null));

        ///<summary> Команда - искать. </summary>
        public ICommand? SearchCommand
        {
            get => (ICommand?)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        #endregion

        #region SearchCommandParameter : object? - Параметр поиска

        ///<summary> Параметр поиска (DependencyProperty). </summary>
        public static readonly DependencyProperty SearchCommandParameterProperty =
            DependencyProperty.Register(nameof(SearchCommandParameter), typeof(object), typeof(SearchFE), new PropertyMetadata(null));

        ///<summary> Параметр поиска. </summary>
        public object? SearchCommandParameter
        {
            get => (object?)GetValue(SearchCommandParameterProperty);
            set => SetValue(SearchCommandParameterProperty, value);
        }

        #endregion

        #region CancelSearchCommand : ICommand? - Команда - отменить поиск

        ///<summary> Команда - отменить поиск (DependencyProperty). </summary>
        public static readonly DependencyProperty CancelSearchCommandProperty =
            DependencyProperty.Register(nameof(CancelSearchCommand), typeof(ICommand), typeof(SearchFE), new PropertyMetadata(null));

        ///<summary> Команда - отменить поиск. </summary>
        public ICommand? CancelSearchCommand
        {
            get => (ICommand?)GetValue(CancelSearchCommandProperty);
            set => SetValue(CancelSearchCommandProperty, value);
        }

        #endregion

        #region CancelSearchCommandParameter : object? - Параметр отмены поиска

        ///<summary> Параметр отмены поиска (DependencyProperty). </summary>
        public static readonly DependencyProperty CancelSearchCommandParameterProperty =
            DependencyProperty.Register(nameof(CancelSearchCommandParameter), typeof(object), typeof(SearchFE), new PropertyMetadata(0));

        ///<summary> Параметр отмены поиска. </summary>
        public object? CancelSearchCommandParameter
        {
            get => (object?)GetValue(CancelSearchCommandParameterProperty);
            set => SetValue(CancelSearchCommandParameterProperty, value);
        }

        #endregion

        #region SearchField : string - Поле поиска

        ///<summary> Поле поиска (DependencyProperty). </summary>
        public static readonly DependencyProperty SearchFieldProperty =
            DependencyProperty.Register(nameof(SearchField), typeof(string), typeof(SearchFE), 
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        ///<summary> Поле поиска. </summary>
        public string SearchField
        {
            get => (string)GetValue(SearchFieldProperty);
            set => SetValue(SearchFieldProperty, value);
        }

        #endregion

        #region ListProperties : IList<string>? - Список свойств для определенного поиска

        ///<summary> Список свойств для определенного поиска (DependencyProperty). </summary>
        public static readonly DependencyProperty ListPropertiesProperty =
            DependencyProperty.Register(nameof(ListProperties), typeof(IList<string>), typeof(SearchFE), new PropertyMetadata(null));

        ///<summary> Список свойств для определенного поиска. </summary>
        public IList<string>? ListProperties
        {
            get => (IList<string>?)GetValue(ListPropertiesProperty);
            set => SetValue(ListPropertiesProperty, value);
        }

        #endregion

        #region SelectedProperty : string? - Выбранное свойство

        ///<summary> Выбранное свойство (DependencyProperty). </summary>
        public static readonly DependencyProperty SelectedPropertyProperty =
            DependencyProperty.Register(nameof(SelectedProperty), typeof(string), typeof(SearchFE),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        ///<summary> Выбранное свойство. </summary>
        public string? SelectedProperty
        {
            get => (string?)GetValue(SelectedPropertyProperty);
            set => SetValue(SelectedPropertyProperty, value);
        }

        #endregion


    }
}
