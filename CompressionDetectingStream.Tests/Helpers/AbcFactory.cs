using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CompressionDetectingStream.Tests.Helpers
{
    class AbcFactory : StreamFactory
    {
        public override Stream DetectStream(Span<byte> magicField, Stream peekingStream)
        {
            return magicField switch
            {
                _ when ContainsMagic(magicField, "abc") => new AbcMagicStream(peekingStream),
                _ when ContainsMagic(magicField, 0x1F, 0x8B) => new GZipStream(peekingStream, CompressionMode.Decompress),
                _ => throw new System.Exception()
            };
        }
        class AbcMagicStream : AForwardStream
        {
            public override Stream SourceStream { get; }
            public AbcMagicStream(Stream outer)
            {
                SourceStream = outer;
            }
            bool confirmed = false;
            public override int Read(byte[] buffer, int offset, int count)
            {
                Span<byte> magicBuf = stackalloc byte[3];
                if (!confirmed)
                {
                    int readPos = 0;
                    while (readPos < magicBuf.Length)
                    {
                        var read = SourceStream.Read(magicBuf.Slice(readPos));
                        if (read <= 0)
                            throw new System.Exception();
                        readPos += read;
                    }

                    if (magicBuf.SequenceEqual(Encoding.ASCII.GetBytes("abc")))
                        confirmed = true;
                    else
                        throw new Exception();
                }
                return SourceStream.Read(buffer, offset, count);
            }
        }
    }
}
