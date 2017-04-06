using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http.Extensions
{
    public class RangeParser
    {
        public static ICollection<RangeItemHeaderValue> ParseRange(HttpContext context, RequestHeaders requestHeaders, DateTimeOffset lastModified, EntityTagHeaderValue etag)
        {
            var rawRangeHeader = context.Request.Headers[HeaderNames.Range];
            if (StringValues.IsNullOrEmpty(rawRangeHeader))
            {
                return null;
            }

            // Perf: Check for a single entry before parsing it
            if (rawRangeHeader.Count > 1 || rawRangeHeader[0].IndexOf(',') >= 0)
            {
                // The spec allows for multiple ranges but we choose not to support them because the client may request
                // very strange ranges (e.g. each byte separately, overlapping ranges, etc.) that could negatively
                // impact the server. Ignore the header and serve the response normally.               
                return null;
            }

            var rangeHeader = requestHeaders.Range;
            if (rangeHeader == null)
            {
                // Invalid
                return null;
            }

            // Already verified above
            Debug.Assert(rangeHeader.Ranges.Count == 1);

            // 14.27 If-Range
            var ifRangeHeader = requestHeaders.IfRange;
            if (ifRangeHeader != null)
            {
                // If the validator given in the If-Range header field matches the
                // current validator for the selected representation of the target
                // resource, then the server SHOULD process the Range header field as
                // requested.  If the validator does not match, the server MUST ignore
                // the Range header field.
                bool ignoreRangeHeader = false;
                if (ifRangeHeader.LastModified.HasValue)
                {
                    if (lastModified > ifRangeHeader.LastModified)
                    {
                        ignoreRangeHeader = true;
                    }
                }
                else if (ifRangeHeader.EntityTag != null && !ifRangeHeader.EntityTag.Compare(etag, useStrongComparison: true))
                {
                    ignoreRangeHeader = true;
                }
                if (ignoreRangeHeader)
                {
                    return null;
                }
            }

            return rangeHeader.Ranges;
        }
    }
}
