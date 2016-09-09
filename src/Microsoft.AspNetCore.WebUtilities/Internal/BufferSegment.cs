using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.WebUtilities.Internal
{
    public class BufferSegment
    {
        public ArraySegment<byte> Buffer;
        public bool Owned;
        public int Start;
        public int End;

        public BufferSegment Next;

        public int Length => End - Start;
    }
}
