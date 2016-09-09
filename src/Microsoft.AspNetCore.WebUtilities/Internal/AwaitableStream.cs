using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.WebUtilities.Internal
{
    /// <summary>
    /// Meant to be used with CopyToAsync for bufferless reads
    /// </summary>
    public class AwaitableStream : Stream
    {
        private static readonly Action _completed = () => { };

        private BufferSegment _head;
        private BufferSegment _tail;

        private Action _continuation;

        private TaskCompletionSource<object> _initialRead = new TaskCompletionSource<object>();
        private TaskCompletionSource<object> _producing = new TaskCompletionSource<object>();

        internal bool HasData => _producing.Task.IsCompleted || _continuation == _completed;

        public Task Completion => _producing.Task;

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // Wait for the first read operation.
            // This is important because the call to write async wants to call the continuation directly
            // so that the continuation can consume the buffers directly without worrying about where 
            // ownership lies. Once the call to WriteAsync returns, the caller owns the buffer so it can't
            // be stashed away without copying
            await _initialRead.Task;

            // Make a new segment capturing the buffer passed in
            var node = new BufferSegment();
            node.Buffer = new ArraySegment<byte>(buffer, offset, count);
            node.Start = offset;
            node.End = offset + count;

            // Append it to the linked list
            if (_head == null || _head.Length == 0)
            {
                _head = node;
            }
            else
            {
                _tail.Next = node;
            }

            _tail = node;

            // Call the continuation
            Complete();
        }

        public StreamAwaitable ReadAsync() => new StreamAwaitable(this);

        /// <summary>
        /// Tell the awaitable stream how many bytes were consumed. This needs to be called from
        /// the continuation.
        /// </summary>
        /// <param name="count">Number of bytes consumed by the continuation</param>
        public void Consumed(int count)
        {
            var node = _head;
            var nodeIndex = node.Start;

            while (count > 0)
            {
                var consumed = Math.Min(node.Length, count);

                count -= consumed;
                nodeIndex += consumed;

                if (nodeIndex == node.End && _head != _tail)
                {
                    // Move to the next node
                    node = node.Next;
                    nodeIndex = node.Start;
                }

                // End of the list stop
                if (_head == _tail)
                {
                    break;
                }
            }

            // Reset the head to the unconsumed buffer
            _head = node;
            _head.Start = nodeIndex;

            // Loop from head to tail and copy unconsumed data into buffers we own, this
            // is important because after the call the WriteAsync returns, the stream can reuse these
            // buffers for anything else
            int length = 0;

            node = _head;
            while (true)
            {
                if (!node.Owned)
                {
                    length += node.Length;
                }

                if (node == _tail)
                {
                    break;
                }

                node = node.Next;
            }

            // This can happen for 2 reasons:
            // 1. We consumed everything 
            // 2. We own all the buffers with data, so no need to copy again.
            if (length == 0)
            {
                return;
            }

            // REVIEW: Use array pool here?
            // Possibly use fixed size blocks here and just fill them so we can avoid a byte[] per call to write
            var buffer = new byte[length];

            // This loop does 2 things
            // 1. Finds the first owned buffer in the list
            // 2. Copies data into the buffer we just allocated
            BufferSegment owned = null;
            node = _head;
            var offset = 0;

            while (true)
            {
                if (!node.Owned)
                {
                    Buffer.BlockCopy(node.Buffer.Array, node.Start, buffer, offset, node.Length);
                    offset += node.Length;
                }
                else if (owned == null)
                {
                    owned = node;
                }

                if (node == _tail)
                {
                    break;
                }

                node = node.Next;
            }

            var data = new BufferSegment
            {
                Buffer = new ArraySegment<byte>(buffer),
                Start = 0,
                End = buffer.Length,
                Owned = true
            };

            // We didn't own anything in the backlog so replace the entire list
            // with the same data, but into buffers we own
            if (owned == null)
            {
                _head = data;
            }
            else
            {
                // Otherwise append the new data to the Next of the first owned block
                owned.Next = data;
            }

            // Update tail to point to data
            _tail = data;
        }

        protected override void Dispose(bool disposing)
        {
            // Mark the stream as "done" when it's disposed
            _producing.TrySetResult(null);

            // Trigger the callback so user code can react to this state change
            Complete();
        }

        internal void OnCompleted(Action continuation)
        {
            if (_continuation == _completed ||
                Interlocked.CompareExchange(ref _continuation, continuation, null) == _completed)
            {
                continuation();
            }

            // Set the initial read result
            _initialRead.TrySetResult(null);
        }

        private void Complete()
        {
            Interlocked.CompareExchange(ref _continuation, _completed, null)?.Invoke();
        }

        internal ByteBuffer GetBuffer()
        {
            _continuation = null;
            return new ByteBuffer(_head, _tail);
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }

}
