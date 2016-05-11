using System;
using System.IO;
using ImageProcessorCore;
using ImageProcessorCore.Formats;
using ImageProcessorCore.Quantizers;

namespace C2eUtils.ImageFormats
{
    /// <summary>
    /// Encapsulates the means to encode and decode gif images.
    /// </summary>
    public class S16Encoder : IImageEncoder
    {
        public int Quality {get;set;}

        public string Extension => "s16";

        public string MimeType => "image/s16";

        public bool IsSupportedFileExtension(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("Value cannot be null or empty and cannot contain only blanks.", "Extension");
            }
            extension = extension.StartsWith(".") ? extension.Substring(1) : extension;
            return extension.Equals(this.Extension, StringComparison.OrdinalIgnoreCase);
        }

        public void Encode(ImageBase image, Stream stream)
        {
         //   GifEncoderCore encoder = new GifEncoderCore
         //   {
         //       Quality = this.Quality,
         //       Quantizer = this.Quantizer,
         //       Threshold = this.Threshold
         //   };
         //   encoder.Encode(image, stream);
        }
    }
}