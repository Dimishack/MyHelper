using System.Windows;

namespace MyHelper.Views.Windows
{
    public partial class SelectStartDateWindow : Window
    {
        public SelectStartDateWindow() => InitializeComponent();

        public string? Challenge
        {
            get { return (string?)GetValue(ChallengeProperty); }
            set { SetValue(ChallengeProperty, value); }
        }

        public static readonly DependencyProperty ChallengeProperty =
            DependencyProperty.Register("Challenge", typeof(string), typeof(SelectStartDateWindow), new PropertyMetadata(string.Empty));

        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(SelectStartDateWindow), new PropertyMetadata(DateTime.Today));
    }
}
