using System;
using Microsoft.AspNetCore.Http.Internal;
using Xunit;

namespace Microsoft.AspNetCore.Http.Tests.Internal
{
    public class InplaceStringFormatterTest
    {
        [Fact]
        public void ToString_ReturnsStringWithAllAppendedValues()
        {
            var s1 = "123";
            var c1 = '4';
            var s2 = "56789";

            var formatter = new InplaceStringFormatter();
            formatter.AppendLength(s1);
            formatter.AppendLength(c1);
            formatter.AppendLength(s2);
            formatter.Append(s1);
            formatter.Append(c1);
            formatter.Append(s2);
            Assert.Equal("123456789", formatter.ToString());
        }

        [Fact]
        public void ToString_ThrowsIfNotEnoughWritten()
        {
            var formatter = new InplaceStringFormatter(5);
            formatter.Append("123");
            var exception = Assert.Throws<InvalidOperationException>(() => formatter.ToString());
            Assert.Equal(exception.Message, "Entire reserved lenght was not used. Length: '5', written '3'.");
        }

        [Fact]
        public void AppendLength_IfAppendWasCalled()
        {
            var formatter = new InplaceStringFormatter(3);
            formatter.Append("123");

            var exception = Assert.Throws<InvalidOperationException>(() => formatter.AppendLength(1));
            Assert.Equal(exception.Message, "Cannot append lenght after write started.");
        }

        [Fact]
        public void Append_ThrowsIfNotEnoughSpace()
        {
            var formatter = new InplaceStringFormatter(1);

            var exception = Assert.Throws<InvalidOperationException>(() => formatter.Append("123"));
            Assert.Equal(exception.Message, "Not enough space to write '3' characters, only '1' left.");
        }
    }
}
