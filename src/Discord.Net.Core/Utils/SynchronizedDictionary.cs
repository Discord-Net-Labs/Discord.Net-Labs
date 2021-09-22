using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a thread safe dictionary
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private object _lock = new object();
        private readonly IDictionary<TKey, TValue> _dictionary;

        /// <inheritdoc/>
        public ICollection<TKey> Keys
        {
            get
            {
                lock (_lock)
                {
                    var array = new TKey[_dictionary.Count];
                    _dictionary.Keys.CopyTo(array, 0);
                    return array.ToImmutableArray();
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<TValue> Values
        {
            get
            {
                lock (_lock)
                {
                    var array = new TValue[_dictionary.Count];
                    _dictionary.Values.CopyTo(array, 0);
                    return array.ToImmutableArray();
                }
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _dictionary.Count;
                }
            }
        }

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                lock (_lock)
                {
                    return _dictionary[key];
                }
            }
            set
            {
                lock (_lock)
                {
                    _dictionary[key] = value;
                }
            }
        }
        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
        /// <inheritdoc/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        /// <inheritdoc/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
       
        /// <inheritdoc/>
        public SynchronizedDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        /// <inheritdoc/>
        public SynchronizedDictionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <inheritdoc/>
        public SynchronizedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <inheritdoc/>
        public bool TryAdd(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (_dictionary.ContainsKey(key))
                    return false;

                _dictionary.Add(key, value);
                return true;
            }
        }

        /// <inheritdoc/>
        public bool TryRemove(TKey key, out TValue value)
        {
            value = default;

            lock (_lock)
            {
                if (!_dictionary.ContainsKey(key))
                    return false;

                value = _dictionary[key];
                return _dictionary.Remove(key);
            }
        }

        /// <inheritdoc/>
        public bool TryUpdate(TKey key, TValue value, out TValue oldValue)
        {
            oldValue = default;

            lock (_lock)
            {
                if (!_dictionary.ContainsKey(key))
                    return false;

                oldValue = _dictionary[key];
                _dictionary[key] = value;
                return true;
            }
        }

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                _dictionary.Add(key, value);
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            lock (_lock)
            {
                _dictionary.Clear();
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            lock (_lock)
            {
                return _dictionary.ContainsKey(key);
            }
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (_lock)
            {
                _dictionary.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                return _dictionary.Remove(key);
            }
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lock)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            => Add(item.Key, item.Value);

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            => ContainsKey(item.Key);

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => Remove(item.Key);

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
