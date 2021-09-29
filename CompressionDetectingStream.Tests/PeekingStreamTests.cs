using CompressionDetectingStream.Tests.Helpers;
using CompressionDetectingStream.Tests.Properties;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace CompressionDetectingStream.Tests
{
    public class PeekingStreamTests
    {
        [Fact]
        public void TestRead4_Expect_ABC_Skipped_Result_Data()
        {
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("abcDATA"));
            ms.Seek(0, SeekOrigin.Begin);
            var ps = new MagicDetctingStream(ms, new AbcFactory());
            Span<byte> actual = stackalloc byte[4];
            ps.Read(actual);
            Assert.Equal("DATA", Encoding.ASCII.GetString(actual));
        }
        [Fact]
        public void TestRead8_Expect_ABC_Skipped_Result_Data()
        {
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("abcDATA"));
            ms.Seek(0, SeekOrigin.Begin);
            var ps = new MagicDetctingStream(ms, new AbcFactory());
            Span<byte> actual = stackalloc byte[8];
            int k = ps.Read(actual);
            Assert.Equal("DATA", Encoding.ASCII.GetString(actual).Substring(0,4));
        }

        [Fact]
        public void TestReadRealGZFile_expect_fooo_plus_newline()
        {
            MemoryStream ms = new MemoryStream(Resources.fooo);
            ms.Seek(0, SeekOrigin.Begin);
            var ps = new MagicDetctingStream(ms, new AbcFactory());
            Span<byte> actual = stackalloc byte[8];
            int k = ps.Read(actual);
            Assert.Equal("fooo\n", Encoding.ASCII.GetString(actual).Substring(0, 5));
        }
    }
}
