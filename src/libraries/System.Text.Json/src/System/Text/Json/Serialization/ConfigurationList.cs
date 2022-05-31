// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// A list of configuration items that respects the options class being immutable once (de)serialization occurs.
    /// </summary>
    internal sealed class ConfigurationList<TItem> : IList<TItem>
    {
        private readonly List<TItem> _list;

        public Action<TItem>? OnElementAdded { get; set; }
        public Func<bool>? IsReadOnlyFunc { get; set; }
        public Action? ThrowImmutableFunc { get; set; }

        public ConfigurationList()
        {
            _list = new List<TItem>();
        }

        public ConfigurationList(IList<TItem> source)
        {
            _list = new List<TItem>(source is ConfigurationList<TItem> cl ? cl._list : source);
        }

        public TItem this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                VerifyMutable();
                _list[index] = value;
                OnElementAdded?.Invoke(value);
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly => IsReadOnlyFunc != null ? IsReadOnlyFunc() : false;

        private void VerifyMutable()
        {
            Debug.Assert((IsReadOnlyFunc == null) == (ThrowImmutableFunc == null), "IsReadOnlyFunc and ThrowImmutableFunc should be either both set or both unset");

            if (IsReadOnlyFunc != null && IsReadOnlyFunc())
            {
                Debug.Assert(ThrowImmutableFunc != null);
                ThrowImmutableFunc();
            }
        }

        public void Add(TItem item)
        {
            if (item is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(item));
            }

            VerifyMutable();
            _list.Add(item);
            OnElementAdded?.Invoke(item);
        }

        public void Clear()
        {
            VerifyMutable();
            _list.Clear();
        }

        public bool Contains(TItem item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(TItem item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
            if (item is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(item));
            }

            VerifyMutable();
            _list.Insert(index, item);
            OnElementAdded?.Invoke(item);
        }

        public bool Remove(TItem item)
        {
            VerifyMutable();
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            VerifyMutable();
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
