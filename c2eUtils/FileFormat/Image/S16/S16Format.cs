using System;
using System.IO;
using ImageProcessorCore.Formats;

namespace C2eUtils.ImageFormats
{
    /// <summary>
    /// Encapsulates the means to encode and decode gif images.
    /// </summary>
    public class S16Format : IImageFormat
    {
        /// <inheritdoc/>
        public IImageDecoder Decoder => new S16Decoder();

        /// <inheritdoc/>
        public IImageEncoder Encoder => new S16Encoder();
    }
}