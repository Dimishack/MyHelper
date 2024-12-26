using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MyHelper.Models
{
    class ObservableDictionary<TKey, TValue> : ICollection<ObservableKeyValuePair<TKey, TValue>>, INotifyCollectionChanged, INotifyPropertyChanged where TKey : notnull
    {
        const int ANDHASH = 0x7FFFFFFF;
        private ObservableKeyValuePair<TKey, TValue>[] _items;
        private IList<int> _freeLists = [];
        private int[] _nexts;
        private int[] _buckets;
        private int _freeList = -1;
        private int _freeCount = 0;
        private int _count = 0;

        public int Count => _count;

        public bool IsReadOnly => false;

        public IList<TKey> Keys => _items.Where(x => x != null).Select(x => x.Key).ToList();

        public TValue this[TKey key]
        {
            get
            {
                var findedItem = FindItem(key);
                return findedItem == null
                    ? throw new KeyNotFoundException(nameof(key))
                    : findedItem.Value;
            }
            set => Insert(new ObservableKeyValuePair<TKey, TValue>(key, value), false);
        }

        public ObservableDictionary() : this(10) { }

        public ObservableDictionary(int size)
        {
            if (size <= 0) size = 10;
            _items = new ObservableKeyValuePair<TKey, TValue>[size];
            _nexts = new int[size];
            _buckets = new int[size];
            for (int i = 0; i < size; i++) _buckets[i] = -1;
        }


        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            => CollectionChanged?.Invoke(_items, e);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Add(TKey key, TValue value)
            => Add(new ObservableKeyValuePair<TKey, TValue>(key, value));

        public void Add(ObservableKeyValuePair<TKey, TValue> item) => Insert(item, true);

        public void Clear()
        {
            Array.Clear(_nexts, 0, _count);
            Array.Clear(_items, 0, _count);
            _freeLists.Clear();
            for (int i = 0; i < _buckets.Length; i++) _buckets[i] = -1;
            _freeCount = 0;
            _freeList = -1;
            _count = 0;
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Remove(TKey key) => RemoveItem(key, false);

        public bool Remove(ObservableKeyValuePair<TKey, TValue> item) => RemoveItem(item.Key, true, item.Value);

        public ObservableKeyValuePair<TKey, TValue> GetKeyValuePair(TKey key)
            => FindItem(key) is ObservableKeyValuePair<TKey, TValue> item
            ? item
            : throw new KeyNotFoundException($"{key}");

        public bool ContainsKey(TKey key) => FindItem(key) != null;

        public bool Contains(ObservableKeyValuePair<TKey, TValue> item)
        {
            int targetBucket = (item.Key.GetHashCode() & ANDHASH) % _buckets.Length;
            for (int i = _buckets[targetBucket]; i >= 0; i = _nexts[i])
                if (Equals(item.Key, _items[i].Key) && Equals(item.Key, _items[i].Value)) return true;
            return false;
        }

        public void CopyTo(ObservableKeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => throw new NotImplementedException();

        public IEnumerator<ObservableKeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _items[i];
        }


        private bool RemoveItem(TKey key, bool checkValue, TValue value = default)
        {
            int targetBucket = (key.GetHashCode() & ANDHASH) % _buckets.Length;
            int last = -1;
            for (int index = _buckets[targetBucket]; index >= 0; last = index, index = _nexts[index])
            {
                bool isFind = checkValue
                    ? Equals(key, _items[index].Key) && Equals(value, _items[index].Value)
                    : Equals(key, _items[index].Key);
                if (isFind)
                {
                    var item = _items[index];
                    if (last < 0)
                        _buckets[targetBucket] = _nexts[index];
                    else
                        _nexts[last] = _nexts[index];
                    _items[index] = null;
                    _nexts[index] = _freeList;
                    _freeList = index;
                    _freeCount++;
                    _count--;
                    OnPropertyChanged(nameof(Count));
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index - FindIndexForFreeList(index, true)));
                    return true;
                }
            }
            return false;
        }

        private int FindIndexForFreeList(int index, bool add = false)
        {
            int start = 0;
            if (_freeLists.Count > 0 && index > _freeLists[0])
            {
                if (index >= _freeLists[^1])
                    start = _freeLists.Count;
                else
                {
                    int end = _freeLists.Count - 1;
                    while (end - start != 1)
                    {
                        int indexDiff = (start + end) / 2;
                        int diff = _freeLists[(start + end) / 2];
                        if (index < diff) end = indexDiff;
                        else if (index > diff) start = indexDiff;
                        else if (index == diff)
                        {
                            end = indexDiff;
                            break;
                        }
                    }
                    start = end;
                }
            }

            if (add) _freeLists.Insert(start, index);
            return start;
        }

        private void Insert(ObservableKeyValuePair<TKey, TValue> item, bool add)
        {
            int hashCode = item.Key.GetHashCode() & ANDHASH;
            int targerBucket = hashCode % _buckets.Length;
            for (int i = _buckets[targerBucket]; i >= 0; i = _nexts[i])
            {
                if (Equals(item.Key, _items[i].Key))
                {
                    if (add)
                        throw new ArgumentException("An element with the same key already exists. " + item.Key);
                    else
                    {
                        _items[i].Value = item.Value;
                        return;
                    }
                }
            }

            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                _freeLists.Remove(_freeList);
                _freeList = _nexts[index];
                _freeCount--;
            }
            else
            {
                if (_count == _items.Length)
                {
                    Resize();
                    targerBucket = hashCode % _buckets.Length;
                }
                index = _count;
            }
            _count++;
            _items[index] = item;
            _nexts[index] = _buckets[targerBucket];
            _buckets[targerBucket] = index;
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _items[index], index));
        }

        private void Resize()
        {
            int newSize = _buckets.Length + 10;
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newSize; i++) newBuckets[i] = -1;
            ObservableKeyValuePair<TKey, TValue>[] newItems = new ObservableKeyValuePair<TKey, TValue>[newSize];
            int[] newNexts = new int[newSize];
            Array.Copy(_items, 0, newItems, 0, _count);
            Array.Copy(_nexts, 0, newNexts, 0, _count);

            for (int i = 0; i < _buckets.Length; i++)
            {
                int bucket = _buckets[i];
                while (bucket >= 0)
                {
                    int next = newNexts[bucket];
                    int targetBucket = (newItems[bucket].Key.GetHashCode() & ANDHASH) % newSize;
                    newNexts[bucket] = newBuckets[targetBucket];
                    newBuckets[targetBucket] = bucket;
                    bucket = next;
                }
            }

            _buckets = newBuckets;
            _nexts = newNexts;
            _items = newItems;
        }

        private ObservableKeyValuePair<TKey, TValue>? FindItem(TKey key)
        {
            int targetBucket = (key.GetHashCode() & ANDHASH) % _buckets.Length;
            for (int i = _buckets[targetBucket]; i >= 0; i = _nexts[i])
                if (Equals(key, _items[i].Key)) return _items[i];
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _items[i];
            //return new Enumerator(_items);
        }

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