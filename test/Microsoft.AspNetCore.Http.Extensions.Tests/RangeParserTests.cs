using System;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Microsoft.AspNetCore.Http.Extensions
{
    public class RangeParserTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseRange_ReturnsNullWhenRangeHeaderNotProvided(string range)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.Range] = range;

            // Act
            var parsedRangeResult = RangeParser.ParseRange(httpContext, httpContext.Request.GetTypedHeaders(), new DateTimeOffset(), null);

            // Assert
            Assert.Null(parsedRangeResult);
        }

        [Theory]
        [InlineData("1-2, 3-4")]
        [InlineData("1-2, ")]
        public void ParseRange_ReturnsNullWhenMultipleRangesProvidedInRangeHeader(string range)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[HeaderNames.Range] = range;

            // Act
            var parsedRangeResult = RangeParser.ParseRange(httpContext, httpContext.Request.GetTypedHeaders(), new DateTimeOffset(), null);

            // Assert
            Assert.Null(parsedRangeResult);
        }

        [Fact]
        public void ParseRange_ReturnsNullWhenLastModifiedGreaterThanIfRangeHeaderLastModified()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var range = new RangeHeaderValue(1, 2);
            httpContext.Request.Headers[HeaderNames.Range] = range.ToString();
            var lastModified = new RangeConditionHeaderValue(DateTime.Now);
            httpContext.Request.Headers[HeaderNames.IfRange] = lastModified.ToString();

            // Act
            var parsedRangeResult = RangeParser.ParseRange(httpContext, httpContext.Request.GetTypedHeaders(), DateTime.Now.AddMilliseconds(2), null);

            // Assert
            Assert.Null(parsedRangeResult);
        }

        [Fact]
        public void ParseRange_ReturnsNullWhenETagNotEqualToIfRangeHeaderEntityTag()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var range = new RangeHeaderValue(1, 2);
            httpContext.Request.Headers[HeaderNames.Range] = range.ToString();
            var etag = new RangeConditionHeaderValue("\"tag\"");
            httpContext.Request.Headers[HeaderNames.IfRange] = etag.ToString();

            // Act
            var parsedRangeResult = RangeParser.ParseRange(httpContext, httpContext.Request.GetTypedHeaders(), DateTime.Now, new EntityTagHeaderValue("\"etag\""));

            // Assert
            Assert.Null(parsedRangeResult);
        }

        [Fact]
        public void ParseRange_ReturnsSingleRangeWhenInputValid()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var range = new RangeHeaderValue(1, 2);
            httpContext.Request.Headers[HeaderNames.Range] = range.ToString();
            var lastModified = new RangeConditionHeaderValue(DateTime.Now);
            httpContext.Request.Headers[HeaderNames.IfRange] = lastModified.ToString();
            var etag = new RangeConditionHeaderValue("\"etag\"");
            httpContext.Request.Headers[HeaderNames.IfRange] = etag.ToString();

            // Act
            var parsedRangeResult = RangeParser.ParseRange(httpContext, httpContext.Request.GetTypedHeaders(), DateTime.Now, new EntityTagHeaderValue("\"etag\""));

            // Assert
            var parsedRange = Assert.Single(parsedRangeResult);
            Assert.Equal(1, parsedRange.From);
            Assert.Equal(2, parsedRange.To);
        }

        [Fact]
        public void ParseRange_ReturnsMultipleRangesWhenInputValid()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var ranges = new[]
            {
                new RangeHeaderValue(1, 2),
                new RangeHeaderValue(4, 5),
            };
            httpContext.Request.Headers[HeaderNames.Range] = ranges.ToString();
            var lastModified = new RangeConditionHeaderValue(DateTime.Now);
            httpContext.Request.Headers[HeaderNames.IfRange] = lastModified.ToString();
            var etag = new RangeConditionHeaderValue("\"etag\"");
            httpContext.Request.Headers[HeaderNames.IfRange] = etag.ToString();

            // Act
            var parsedRangeResult = RangeParser.ParseRange(httpContext, httpContext.Request.GetTypedHeaders(), DateTime.Now, new EntityTagHeaderValue("\"etag\""));

            // Assert
            Assert.Collection(parsedRangeResult,
                range =>
                {
                    Assert.Equal(1, range.From);
                    Assert.Equal(2, range.To);
                },
                range =>
                {
                    Assert.Equal(4, range.From);
                    Assert.Equal(5, range.To);
                });
        }
    }
}
