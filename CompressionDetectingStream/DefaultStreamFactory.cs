using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CompressionDetectingStream
{
    public class DefaultStreamFactory : StreamFactory
    {
        public override Stream DetectStream(Span<byte> magicField, Stream peekingStream) => magicField switch
        {
            _ when ContainsMagic(magicField, 0x1F, 0x8B) => new GZipStream(peekingStream, CompressionMode.Decompress),
            _ when ContainsMagic(magicField.Slice(257, 6), "ustar") => peekingStream, // passthrough tar
            _ => peekingStream
        };
    }
}