namespace MyHelper.Models
{
    class Filter(string name, bool isSelected = false) : Base.BaseModel
    {
        public string Name { get; set; } = name;

        private bool _isSelected = isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
    }
}
