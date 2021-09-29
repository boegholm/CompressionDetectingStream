using System.IO;

namespace CompressionDetectingStream
{
    public abstract class AForwardStream : Stream
    {
        public abstract Stream SourceStream { get; }
        public override bool CanRead => SourceStream.CanRead;
        public override bool CanSeek => SourceStream.CanSeek;
        public override bool CanWrite => SourceStream.CanWrite;
        public override long Length => SourceStream.Length;
        public override long Position { get => SourceStream.Position; set => SourceStream.Position = value; }
        public override void Flush() => SourceStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => SourceStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => SourceStream.Seek(offset, origin);
        public override void SetLength(long value) => SourceStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => SourceStream.Write(buffer, offset, count);
    }
}
