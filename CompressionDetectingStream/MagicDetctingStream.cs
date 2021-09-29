using System;
using System.IO;

namespace CompressionDetectingStream
{
    public partial class MagicDetctingStream : AForwardStream
    {
        public const int MAGIC_BUFFER_SIZE = 512; // tar has "ustar" magic at 257

        public Stream RawStream { get; }
        IStreamFactory StreamFactory { get; }
        public Stream StreamImplementation { get; private set; }
        public MagicDetctingStream(Stream rawStream) : this(rawStream, new DefaultStreamFactory())
        {}
        public MagicDetctingStream(Stream rawStream, IStreamFactory streamFactory) 
        {
            RawStream = rawStream;
            StreamFactory = streamFactory;
        }

        public override Stream SourceStream
        {
            get
            {
                if (StreamImplementation == null)
                {
                    Span<byte> buf = stackalloc byte[MAGIC_BUFFER_SIZE];
                    int l = 0;
                    while (l < MAGIC_BUFFER_SIZE)
                    {
                        var read = RawStream.Read(buf.Slice(l));
                        if (read <= 0)
                            break;
                        l += read;
                    }
                    var magic = buf.Slice(0, l).ToArray();
                    StreamImplementation = StreamFactory.DetectStream(magic, new PrefixStream(magic, RawStream));
                }
                return StreamImplementation;
            }
        }
    }
}
