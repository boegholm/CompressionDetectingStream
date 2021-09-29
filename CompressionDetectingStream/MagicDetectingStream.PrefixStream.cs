using System;
using System.IO;


namespace CompressionDetectingStream
{
    public partial class MagicDetctingStream 
    {
        private class PrefixStream : AForwardStream
        {
            public PrefixStream(byte[] magicField, Stream sourceStream)
            {
                MagicField = magicField;
                SourceStream = sourceStream; 
            }
            public ReadOnlyMemory<byte> MagicField { get; private set; }
            public override Stream SourceStream { get; }

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
        }
    }
}
