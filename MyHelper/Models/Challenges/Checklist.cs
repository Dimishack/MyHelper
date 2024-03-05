using MyHelper.ViewModels.Base;

namespace MyHelper.Models.Challenges
{
    internal class Checklist(string time, bool check) : ViewModel
    {
        public string Time { get; set; } = time;
        private bool _check = check;
        public bool Check
        {
            get => _check; 
            set => Set(ref _check, value);
        }
    }
}
