using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CompressionDetectingStream
{
    public interface IStreamFactory
    {
        Stream DetectStream(Span<byte> magicField, Stream peekingStream);
    }

    public class DefaultStreamFactory : IStreamFactory
    {
        bool ContainsMagic(Span<byte> magicField, params byte[] prefix) =>
            magicField.Length >= prefix.Length && magicField.Slice(0, prefix.Length).SequenceEqual(prefix);
        bool ContainsMagic(Span<byte> magicField, string prefix) =>
            ContainsMagic(magicField, Encoding.ASCII.GetBytes(prefix));

        public Stream DetectStream(Span<byte> magicField, Stream peekingStream)
        {
            return magicField switch
            {
                _ when ContainsMagic(magicField, 0x1F, 0x8B) => new GZipStream(peekingStream, CompressionMode.Decompress),
                _ when ContainsMagic(magicField.Slice(257, 6), "ustar") => peekingStream, // passthrough tar
                _ => throw new System.Exception()
            };
        }
    }
}