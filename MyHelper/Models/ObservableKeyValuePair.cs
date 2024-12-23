using System.ComponentModel;

namespace MyHelper.Models
{
    internal class ObservableKeyValuePair<TKey, TValue>(TKey key, TValue value) : INotifyPropertyChanged
    {
        public TKey Key { get; } = key;
        private TValue _value = value;
        public TValue Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


//private struct Enumerator : IEnumerator<ObservableKeyValuePair<TKey, TValue>>
//{
//    ObservableKeyValuePair<TKey, TValue>[] _items;
//    private ObservableKeyValuePair<TKey, TValue> _current;
//    private int _index;

//    public ObservableKeyValuePair<TKey, TValue> Current
//    {
//        get
//        {
//            if (_current == null) throw new InvalidOperationException(nameof(_current));
//            return _current;
//        }
//    }

//    object IEnumerator.Current => Current;

//    public Enumerator(ObservableKeyValuePair<TKey, TValue>[] items)
//    {
//        _items = items;
//        _current = new ObservableKeyValuePair<TKey, TValue>();
//        _index = 0;
//    }

//    public void Dispose()
//    {

//    }

//    public bool MoveNext()
//    {
//        while(_index < _items.Length)
//        {
//            if (_items[_index] is not null)
//            {
//                _current = _items[_index];
//                _index++;
//                return true;
//            }
//            _index++;
//        }
//        _index = _items.Length + 1;
//        _current = new();
//        return false;

//    }

//    public void Reset()
//    {
//        _index = 0;
//        _current = new ObservableKeyValuePair<TKey, TValue>();
//    }
//}