using System;
using System.IO;
using System.Text;

namespace CompressionDetectingStream
{
    public abstract class StreamFactory: IStreamFactory
    {
        public abstract Stream DetectStream(Span<byte> magicField, Stream peekingStream);
        public bool ContainsMagic(Span<byte> magicField, params byte[] prefix) =>
            magicField.Length >= prefix.Length &&
            magicField.Slice(0, prefix.Length).SequenceEqual(prefix);

        public bool ContainsMagic(Span<byte> magicField, string prefix) =>
            ContainsMagic(magicField, Encoding.ASCII.GetBytes(prefix));
    }
}