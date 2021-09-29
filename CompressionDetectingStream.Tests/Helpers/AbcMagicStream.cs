using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CompressionDetectingStream.Tests.Helpers
{
    class AbcFactory : IStreamFactory
    {
        bool ContainsMagic(byte[] magicField, params byte[] prefix) => 
            magicField.Length >= prefix.Length &&
            magicField.Take(prefix.Length).Zip(prefix).All(v => v.First == v.Second);
        bool ContainsMagic(byte[] magicField, string prefix) =>
            ContainsMagic(magicField, Encoding.ASCII.GetBytes(prefix));

        public Stream DetectStream(byte[] magicField, Stream peekingStream)
        {
            return magicField switch
            {
                _ when ContainsMagic(magicField, "abc") => new AbcMagicStream(peekingStream),
                _ when ContainsMagic(magicField, 0x1F, 0x8B) => new GZipStream(peekingStream, CompressionMode.Decompress),
                _ => throw new System.Exception()
            };
        }
        class AbcMagicStream : Stream
        {
            Stream Outer;
            public AbcMagicStream(Stream outer)
            {
                Outer = outer;
            }

            public override bool CanRead => Outer.CanRead;

            public override bool CanSeek => Outer.CanSeek;

            public override bool CanWrite => Outer.CanWrite;

            public override long Length => Outer.Length;

            public override long Position { get => Outer.Position; set => Outer.Position = value; }

            public override void Flush()
            {
                Outer.Flush();
            }
            bool confirmed;
            public override int Read(byte[] buffer, int offset, int count)
            {
                Span<byte> magicBuf = stackalloc byte[3];
                if (!confirmed)
                {
                    int readPos = 0;
                    while (true)
                    {
                        var read = Outer.Read(magicBuf.Slice(readPos));
                        if (read == 0)
                            throw new System.Exception();
                        readPos += read;
                        if (readPos == 3)
                            break;
                    }
                    if (magicBuf[0] == 'a' && magicBuf[1] == 'b' && magicBuf[2] == 'c')
                    {
                        confirmed = true;
                    }
                    else
                        throw new System.Exception();
                }
                return Outer.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return Outer.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                Outer.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                Outer.Write(buffer, offset, count);
            }
        }
    }


}
