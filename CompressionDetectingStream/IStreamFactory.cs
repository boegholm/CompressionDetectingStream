using System.IO;

namespace CompressionDetectingStream
{
    public interface IStreamFactory
    {
        Stream DetectStream(byte[] magicField, Stream peekingStream);
    }
}