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

        public BufferSegment Next;
    }
}
