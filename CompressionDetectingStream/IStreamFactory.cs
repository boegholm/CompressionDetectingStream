using System;
using System.IO;

namespace CompressionDetectingStream
{
    public interface IStreamFactory
    {
        Stream DetectStream(Span<byte> magicField, Stream peekingStream);
    }
}