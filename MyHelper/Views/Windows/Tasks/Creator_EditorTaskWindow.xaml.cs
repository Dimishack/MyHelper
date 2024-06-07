using System.Windows;

namespace MyHelper.Views.Windows
{
    public partial class Creator_EditorTaskWindow : Window
    {
        public Creator_EditorTaskWindow() => InitializeComponent();

        public string? Task
        {
            get { return (string?)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }

        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(string), typeof(Creator_EditorTaskWindow), new PropertyMetadata(null));

        public string? Note
        {
            get { return (string?)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }

        public static readonly DependencyProperty NoteProperty =
            DependencyProperty.Register("Note", typeof(string), typeof(Creator_EditorTaskWindow), new PropertyMetadata(null));

        public DateTime? Term
        {
            get { return (DateTime?)GetValue(TermProperty); }
            set { SetValue(TermProperty, value); }
        }

        public static readonly DependencyProperty TermProperty =
            DependencyProperty.Register("Term", typeof(DateTime?), typeof(Creator_EditorTaskWindow), new PropertyMetadata(DateTime.Today));

        public bool Prompt
        {
            get { return (bool)GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        public static readonly DependencyProperty PromptProperty =
            DependencyProperty.Register("Prompt", typeof(bool), typeof(Creator_EditorTaskWindow), new PropertyMetadata(false));

        public bool Important
        {
            get { return (bool)GetValue(ImportantProperty); }
            set { SetValue(ImportantProperty, value); }
        }

        public static readonly DependencyProperty ImportantProperty =
            DependencyProperty.Register("Important", typeof(bool), typeof(Creator_EditorTaskWindow), new PropertyMetadata(false));



        public IList<string> Groups
        {
            get { return (IList<string>)GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register("Groups", typeof(IList<string>), typeof(Creator_EditorTaskWindow), new PropertyMetadata(null));



        public string SelectedGroup
        {
            get { return (string)GetValue(SelectedGroupProperty); }
            set { SetValue(SelectedGroupProperty, value); }
        }

        public static readonly DependencyProperty SelectedGroupProperty =
            DependencyProperty.Register("SelectedGroup", typeof(string), typeof(Creator_EditorTaskWindow), new PropertyMetadata(string.Empty));



    }
}
