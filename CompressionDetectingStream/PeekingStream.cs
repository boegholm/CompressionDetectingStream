using System;
using System.IO;

namespace CompressionDetectingStream
{
    public partial class MagicDetctingStream : Stream
    {
        IStreamFactory Factory { get; }
        Stream SourceStream { get; }

        public override bool CanRead => SourceStream.CanRead;

        public override bool CanSeek => SourceStream.CanSeek;

        public override bool CanWrite => SourceStream.CanWrite;

        public override long Length => SourceStream.Length;

        public override long Position { get => SourceStream.Position; set => SourceStream.Position = value; }

        public MagicDetctingStream(Stream sourceStream, IStreamFactory factory)
        {
            SourceStream = sourceStream;
            Factory = factory;
        }

        public override void Flush()
        {
            SourceStream.Flush();
        }

        Stream f;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (f == null) { 
                Span<byte> buf = stackalloc byte[8];
                int l = SourceStream.Read(buf);
                var magic = buf.Slice(0, l).ToArray();
                f = Factory.DetectStream(magic,  new PrefixStream(magic, SourceStream));
            }
            return f.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return SourceStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            SourceStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            SourceStream.Write(buffer, offset, count);
        }
    }
}
