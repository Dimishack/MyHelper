namespace MyHelper.Models
{
    class Filter(string name, bool isSelected = false, int count = 0) : Base.BaseModel
    {
        public string Name { get; set; } = name;

        private bool _isSelected = isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        private int _count = count;
        public int Count
        {
            get => _count;
            set
            {
                if(!Set(ref _count, value)) return;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        public bool IsEnabled => _count > 0;
    }
}
