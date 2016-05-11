using System;
using System.IO;
using ImageProcessorCore.Formats;

namespace C2eUtils.ImageFormats
{
    /// <summary>
    /// Encapsulates the means to encode and decode gif images.
    /// </summary>
    public class BLKFormat : IImageFormat
    {
        /// <inheritdoc/>
        public IImageDecoder Decoder => new BLKDecoder();

        /// <inheritdoc/>
        public IImageEncoder Encoder => new BLKEncoder();
    }
}