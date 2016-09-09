using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.WebUtilities.Internal
{
    public struct ByteBuffer : IEnumerable<ArraySegment<byte>>
    {
        private readonly BufferSegment _head;
        private readonly BufferSegment _tail;

        public bool IsEmpty => _head == _tail && (_head?.Start == _tail?.End);

        private bool IsSingleBuffer => _head == _tail;

        public int Length
        {
            get
            {
                // TODO: Cache
                int length = 0;
                var node = _head;
                while (true)
                {
                    length += node.Length;
                    if (node == _tail)
                    {
                        break;
                    }
                }
                return length;
            }
        }

        public ByteBuffer(BufferSegment head, BufferSegment tail)
        {
            _head = head;
            _tail = tail;
        }

        public int IndexOf(byte data)
        {
            var segment = _head;
            int index = 0;

            while (true)
            {
                for (int i = 0; i < segment.Length; i++)
                {
                    if (segment.Buffer.Array[i + segment.Start] == data)
                    {
                        return index;
                    }

                    index++;
                }

                if (segment == _tail)
                {
                    break;
                }

                segment = segment.Next;
            }

            return -1;
        }

        public ByteBuffer Slice(int offset, int length)
        {
            return default(ByteBuffer);
        }

        public ArraySegment<byte> GetArraySegment()
        {
            List<ArraySegment<byte>> buffers = null;
            var length = 0;

            foreach (var span in this)
            {
                if (IsSingleBuffer)
                {
                    return span;
                }
                else
                {
                    if (buffers == null)
                    {
                        buffers = new List<ArraySegment<byte>>();
                    }
                    buffers.Add(span);
                    length += span.Count;
                }
            }

            var data = new byte[length];
            int offset = 0;
            foreach (var span in buffers)
            {
                Buffer.BlockCopy(span.Array, span.Offset, data, offset, span.Count);
                offset += span.Count;
            }

            return new ArraySegment<byte>(data, 0, length);
        }

        public IEnumerator<ArraySegment<byte>> GetEnumerator()
        {
            return new Enumerator(_head, _tail);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<ArraySegment<byte>>
        {
            private BufferSegment _head;
            private readonly BufferSegment _tail;
            private ArraySegment<byte> _current;
            private int _offset;

            public Enumerator(BufferSegment head, BufferSegment tail)
            {
                _head = head;
                _tail = tail;
                _current = default(ArraySegment<byte>);
                _offset = head.Start;
            }

            public ArraySegment<byte> Current => _current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_head == _tail && _offset == _tail.End)
                {
                    return false;
                }

                _current = new ArraySegment<byte>(_head.Buffer.Array, _head.Start, _head.Length);

                if (_head != _tail)
                {
                    _head = _head.Next;
                }
                else
                {
                    _offset = _tail.End;
                }

                return true;
            }

            public void Reset()
            {

            }
        }
    }
}
