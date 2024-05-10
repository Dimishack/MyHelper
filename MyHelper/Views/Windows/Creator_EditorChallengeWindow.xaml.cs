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
    /// <summary>
    /// Interaction logic for Creator_EditorChallengeWindow.xaml
    /// </summary>
    public partial class Creator_EditorChallengeWindow : Window
    {
        public Creator_EditorChallengeWindow()
        {
            InitializeComponent();
        }

        public string Note
        {
            get { return (string)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }

        public static readonly DependencyProperty NoteProperty =
            DependencyProperty.Register("Note", typeof(string), 
                typeof(Creator_EditorChallengeWindow), 
                new PropertyMetadata(string.Empty));

        public string Challenge
        {
            get { return (string)GetValue(ChallengeProperty); }
            set { SetValue(ChallengeProperty, value); }
        }

        public static readonly DependencyProperty ChallengeProperty =
            DependencyProperty.Register("Challenge", typeof(string), 
                typeof(Creator_EditorChallengeWindow), 
                new PropertyMetadata(string.Empty));



        public string[] Durations
        {
            get { return (string[])GetValue(DurationsProperty); }
            set { SetValue(DurationsProperty, value); }
        }

        public static readonly DependencyProperty DurationsProperty =
            DependencyProperty.Register("Durations", typeof(string[]), 
                typeof(Creator_EditorChallengeWindow), 
                new PropertyMetadata(null));



        public string Duration
        {
            get { return (string)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(string), typeof(Creator_EditorChallengeWindow), new PropertyMetadata(string.Empty));




    }
}
