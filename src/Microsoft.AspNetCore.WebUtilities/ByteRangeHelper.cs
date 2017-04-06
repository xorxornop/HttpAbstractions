// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.WebUtilities
{
    /// <summary>
    /// Helpers to normalize ranges.
    /// </summary>
    public static class ByteRangeHelper
    {
        /// <summary>
        /// A helper method to normalize a collection of <see cref="RangeItemHeaderValue"/>s.
        /// </summary>
        /// <param name="ranges">A collection of <see cref="RangeItemHeaderValue"/> to normalize.</param>
        /// <param name="length">The length of the provided <see cref="RangeItemHeaderValue"/>.</param>
        /// <returns>A normalized list of <see cref="RangeItemHeaderValue"/>s.</returns>
        // 14.35.1 Byte Ranges - If a syntactically valid byte-range-set includes at least one byte-range-spec whose
        // first-byte-pos is less than the current length of the entity-body, or at least one suffix-byte-range-spec
        // with a non-zero suffix-length, then the byte-range-set is satisfiable.
        // Adjusts ranges to be absolute and within bounds.
        public static IList<RangeItemHeaderValue> NormalizeRanges(ICollection<RangeItemHeaderValue> ranges, long length)
        {
            if (ranges.Count == 0)
            {
                return Array.Empty<RangeItemHeaderValue>();
            }

            if (length == 0)
            {
                return Array.Empty<RangeItemHeaderValue>();
            }

            var normalizedRanges = new List<RangeItemHeaderValue>(ranges.Count);
            foreach (var range in ranges)
            {
                var normalizedRange = NormalizeRange(range, length);

                if (normalizedRange != null)
                {
                    normalizedRanges.Add(normalizedRange);
                }
            }

            return normalizedRanges;
        }

        /// <summary>
        /// A helper method to normalize a <see cref="RangeItemHeaderValue"/>.
        /// </summary>
        /// <param name="range">The <see cref="RangeItemHeaderValue"/> to normalize.</param>
        /// <param name="length">The length of the provided <see cref="RangeItemHeaderValue"/>.</param>
        /// <returns>A normalized <see cref="RangeItemHeaderValue"/>.</returns>
        public static RangeItemHeaderValue NormalizeRange(RangeItemHeaderValue range, long length)
        {
            var start = range.From;
            var end = range.To;

            // X-[Y]
            if (start.HasValue)
            {
                if (start.Value >= length)
                {
                    // Not satisfiable, skip/discard.
                    return null;
                }
                if (!end.HasValue || end.Value >= length)
                {
                    end = length - 1;
                }
            }
            else
            {
                // suffix range "-X" e.g. the last X bytes, resolve
                if (end.Value == 0)
                {
                    // Not satisfiable, skip/discard.
                    return null;
                }

                var bytes = Math.Min(end.Value, length);
                start = length - bytes;
                end = start + bytes - 1;
            }

            var normalizedRange = new RangeItemHeaderValue(start, end);
            return normalizedRange;
        }
    }
}
