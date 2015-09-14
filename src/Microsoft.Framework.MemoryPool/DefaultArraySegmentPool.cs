// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;

namespace Microsoft.Framework.MemoryPool
{
    public class DefaultArraySegmentPool<T> : IArraySegmentPool<T>, IDisposable
    {
        public readonly static int Capacity = Environment.ProcessorCount * 4;

        public readonly static int BlockSize = 4096;

        private volatile bool _isDisposed;

        private readonly ConcurrentQueue<DefaultLeasedArraySegment> _segments = new ConcurrentQueue<DefaultLeasedArraySegment>();

        public LeasedArraySegment<T> Lease(int size)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(DefaultArraySegmentPool<T>));
            }

            if (size > BlockSize)
            {
                // We don't handle non-standard sizes. Just allocate a new array that's big enough.
                return new DefaultLeasedArraySegment(new ArraySegment<T>(new T[size]), this);
            }

            DefaultLeasedArraySegment segment;
            if (_segments.TryDequeue(out segment))
            {
                segment = new DefaultLeasedArraySegment(new ArraySegment<T>(new T[BlockSize]), this);
            }

            return segment;
        }

        public void Return(LeasedArraySegment<T> buffer)
        {
            var segment = (DefaultLeasedArraySegment)buffer;
            if (buffer.Data.Count != BlockSize)
            {
                // We don't handle non-standard sizes. Just let it be GC'ed.
                segment.Destroy();
                return;
            }

            if (_isDisposed)
            {
                segment.Destroy();
                return;
            }

            if (_segments.Count < Capacity)
            {
                segment.Destroy();
                return;
            }

            _segments.Enqueue(segment);
        }

        public void Dispose()
        {
            _isDisposed = true; // Stops anything from being returned to the pool.

            DefaultLeasedArraySegment segment;
            while (_segments.TryDequeue(out segment))
            {
                segment.Destroy();
            }
        }

        private class DefaultLeasedArraySegment : LeasedArraySegment<T>
        {
            public DefaultLeasedArraySegment(ArraySegment<T> data, DefaultArraySegmentPool<T> pool)
                : base(data, pool)
            {
            }

            public new DefaultArraySegmentPool<T> Owner => (DefaultArraySegmentPool<T>)base.Owner;

            public void Destroy()
            {
                base.Owner = null;
                Data = default(ArraySegment<T>);

                GC.SuppressFinalize(this);
            }

            ~DefaultLeasedArraySegment()
            {
                if (Owner != null && !Owner._isDisposed)
                {
                    throw new InvalidOperationException(
                        $"A {nameof(LeasedArraySegment<T>)} not collected without being returned to the pool. " +
                        "This is a memory leak.");
                }
            }
        }
    }
}
