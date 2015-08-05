// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.AspNet.Http
{
    /// <summary>
    /// Provides correct handling for QueryString value when needed to reconstruct a request or redirect URI string
    /// </summary>
    public struct StringValues : IList<string>
    {
        public static StringValues Empty = new StringValues(new string[0]);

        private readonly string _value;
        private readonly string[] _values;

        public StringValues(string value)
        {
            if (value == null)
            {
                _value = null;
                _values = Empty._values;
            }
            else
            {
                _value = value;
                _values = null;
            }
        }

        public StringValues(string[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            _value = null;
            _values = values;
        }       

        public static implicit operator StringValues(string value)
        {
            return new StringValues(value);
        }

        public static implicit operator StringValues(string[] value)
        {
            return new StringValues(value);
        }

        public static implicit operator string (StringValues value)
        {
            return value.GetStringValue();
        }

        private string GetStringValue()
        {
            if (_values == null)
            {
                return _value;
            }
            switch (_values.Length)
            {
                case 0:
                    {
                        return null;
                    }
                case 1:
                    {
                        return _values[0];
                    }
                default:
                    {
                        return string.Join(",", _values);
                    }
            }
        }

        public int Count => _values?.Length ?? (_value != null ? 1 : 0);

        #region IList<string>
        int ICollection<string>.Count => Count;

        bool ICollection<string>.IsReadOnly => true;

        string IList<string>.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        int IList<string>.IndexOf(string item)
        {
            var index = 0;
            foreach(var value in this)
            {
                if (string.Equals(value, item, StringComparison.Ordinal))
                {
                    return index;
                }
                index += 1;
            }
            return -1;
        }

        void IList<string>.Insert(int index, string item)
        {
            throw new NotSupportedException();
        }

        void IList<string>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<string>.Add(string item)
        {
            throw new NotSupportedException();
        }

        void ICollection<string>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<string>.Contains(string item)
        {
            foreach(var value in this)
            {
                if (string.Equals(value, item, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        void ICollection<string>.CopyTo(string[] array, int arrayIndex)
        {
            var index = 0;
            foreach (var value in this)
            {
                array[arrayIndex + index] = value;
                index += 1;
            }
        }

        bool ICollection<string>.Remove(string item)
        {
            throw new NotSupportedException();
        }
        #endregion

        public static implicit operator string[] (StringValues value)
        {
            return value._values ?? new string[1] { value._value };
        }

        public override string ToString()
        {
            return GetStringValue() ?? string.Empty;
        }

        public string this[int key]
        {
            get
            {
                if (_values != null)
                {
                    return _values[key];
                }
                if (key == 0 && _values == null)
                {
                    return _value;
                }
                return Empty[0];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }


        public struct Enumerator : IEnumerator<string>
        {
            private readonly StringValues _values;
            private readonly int _count;
            private int _index;

            public Enumerator(StringValues values)
            {
                _values = values;
                _index = -1;
                _count = _values.Count;
                Current = null;
            }

            public string Current
            {
                get; private set;
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ++_index;
                if (_index < _count)
                {
                    Current = _values[_index];
                    return true;
                }
                Current = null;
                return false;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        public static bool IsNullOrEmpty(StringValues value)
        {
            if (value._values == null)
            {
                return string.IsNullOrEmpty(value._value);
            }
            switch (value._values.Length)
            {
                case 0:
                    {
                        return true;
                    }
                case 1:
                    {
                        return string.IsNullOrEmpty(value._values[0]);
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public static StringValues Concat(StringValues values1, StringValues values2)
        {
            var count1 = values1.Count;
            var count2 = values2.Count;

            if (count1 == 0)
            {
                return values2;
            }

            if (count2 == 0)
            {
                return values1;
            }

            var combined = new string[count1 + count2];
            var index = 0;
            foreach (var value in values1)
            {
                combined[index++] = value;
            }
            foreach (var value in values2)
            {
                combined[index++] = value;
            }
            return new StringValues(combined);
        }

    }
}
