using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities.Internal;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.WebUtilities
{
    public class Utf8FormReader
    {
        private static readonly Encoding _utf8Encoding = Encoding.UTF8;

        private readonly Stream _body;
        private readonly AwaitableStream _stream = new AwaitableStream();

        public Utf8FormReader(Stream body)
        {
            _body = body;
        }

        public int ValueLengthLimit { get; set; }

        public async Task<Dictionary<string, StringValues>> ReadFormAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Drain the body, the buffer size is used for the default implementation
            var ignore = _body.CopyToAsync(_stream, 2048, cancellationToken).ContinueWith((t, state) =>
            {
                ((AwaitableStream)state).Dispose();
            },
            _stream);

            var formValues = new Dictionary<string, StringValues>();

            while (true)
            {
                var buffer = await _stream.ReadAsync();

                if (_stream.Completion.IsCanceled)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (buffer.IsEmpty && _stream.Completion.IsCompleted)
                {
                    break;
                }

                while (true)
                {
                    // foo=bar&baz=2
                    var index = buffer.IndexOf((byte)'&');

                    if (index == -1)
                    {
                        // TODO: Check for EOF
                        break;
                    }

                    var key = buffer.Slice(0, index);

                    index = buffer.IndexOf((byte)'&', index);

                    var value = buffer.Slice(0, index);
                }
            }

            return formValues;
        }
    }
}
