// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Microsoft.AspNetCore.WebUtilities
{
    public class ByteRangeHelperTest
    {
        [Fact]
        public void NormalizeRanges_ReturnsEmptyArrayWhenRangeCountZero()
        {
            // Arrange
            var ranges = new List<RangeItemHeaderValue>();

            // Act
            var normalizedRanges = ByteRangeHelper.NormalizeRanges(ranges, 2);

            // Assert
            Assert.Empty(normalizedRanges);
        }

        [Fact]
        public void NormalizeRanges_ReturnsEmptyArrayWhenLengthZero()
        {
            // Arrange
            var ranges = new[]
            {
                new RangeItemHeaderValue(0, 0),
            };

            // Act
            var normalizedRanges = ByteRangeHelper.NormalizeRanges(ranges, 0);

            // Assert
            Assert.Empty(normalizedRanges);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        public void NormalizeRanges_SkipsItemWhenRangeStartEqualOrGreaterThanLength(long start, long end)
        {
            // Arrange
            var ranges = new[]
            {
                new RangeItemHeaderValue(start, end),
            };

            // Act
            var normalizedRanges = ByteRangeHelper.NormalizeRanges(ranges, 1);

            // Assert
            Assert.Empty(normalizedRanges);
        }

        [Fact]
        public void NormalizeRanges_SkipsItemWhenRangeEndEqualsZero()
        {
            // Arrange
            var ranges = new[]
            {
                new RangeItemHeaderValue(null, 0),
            };

            // Act
            var normalizedRanges = ByteRangeHelper.NormalizeRanges(ranges, 1);

            // Assert
            Assert.Empty(normalizedRanges);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, 0)]
        [InlineData(0, null)]
        [InlineData(0, 0)]
        public void NormalizeRanges_ReturnsNormalizedRange(long start, long end)
        {
            // Arrange
            var ranges = new[]
            {
                new RangeItemHeaderValue(start, end),
            };

            // Act
            var normalizedRanges = ByteRangeHelper.NormalizeRanges(ranges, 1);

            // Assert
            var range = Assert.Single(normalizedRanges);
            Assert.Equal(0, range.From);
            Assert.Equal(0, range.To);
        }

        [Fact]
        public void NormalizeRanges_ReturnsRangeWithNoChange()
        {
            // Arrange
            var ranges = new[]
            {
                new RangeItemHeaderValue(1, 3),
            };

            // Act
            var normalizedRanges = ByteRangeHelper.NormalizeRanges(ranges, 4);

            // Assert
            var range = Assert.Single(normalizedRanges);
            Assert.Equal(1, range.From);
            Assert.Equal(3, range.To);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, 0)]
        [InlineData(0, null)]
        [InlineData(0, 0)]
        public void NormalizeRanges_MultipleRanges_ReturnsNormalizedRange(long start, long end)
        {
            // Arrange
            var ranges = new[]
            {
                new RangeItemHeaderValue(start, end),
                new RangeItemHeaderValue(1, 2),
            };

            // Act
            var normalizedRanges = ByteRangeHelper.NormalizeRanges(ranges, 3);

            // Assert
            Assert.Collection(normalizedRanges,
                range =>
                {
                    Assert.Equal(0, range.From);
                    Assert.Equal(0, range.To);
                },
                range =>
                {
                    Assert.Equal(1, range.From);
                    Assert.Equal(2, range.To);
                });
        }
    }
}
