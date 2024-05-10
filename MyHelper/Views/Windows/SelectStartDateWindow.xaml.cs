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

namespace MyHelper.Views.Windows
{
    public partial class SelectStartDateWindow : Window
    {
        public SelectStartDateWindow() => InitializeComponent();

        public string Challenge
        {
            get { return (string)GetValue(ChallengeProperty); }
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
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(SelectStartDateWindow), new PropertyMetadata(null));
    }
}
