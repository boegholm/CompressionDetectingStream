using System;
using System.IO;


namespace CompressionDetectingStream
{
    public partial class MagicDetctingStream 
    {
        private class PrefixStream : Stream
        {
            public PrefixStream(byte[] magicField, Stream sourceStream)
            {
                MagicField = magicField;
                SourceStream = sourceStream;
            }

            public ReadOnlyMemory<byte> MagicField { get; private set; }
            public Stream SourceStream { get; }

            public override bool CanRead => SourceStream.CanRead;

            public override bool CanSeek => SourceStream.CanSeek;

            public override bool CanWrite => SourceStream.CanWrite;

            public override long Length => SourceStream.Length;

            public override long Position { get => SourceStream.Position; set => SourceStream.Position = value; }

            public override void Flush()
            {
                SourceStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (MagicField.Length > 0)
                {
                    var prefixToRead = Math.Min(count, MagicField.Length);

                    var data = MagicField.Slice(0, prefixToRead);

                    data.CopyTo(buffer.AsMemory().Slice(offset));
                    MagicField = MagicField.Slice(data.Length);
                    if (count - data.Length > 0)
                    {
                        return data.Length+SourceStream.Read(buffer, offset + data.Length, count - data.Length);
                    }
                    else
                        return data.Length;
                } else
                    return SourceStream.Read(buffer, offset, count);
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
}
