using System;
using System.IO;
using ImageProcessorCore;
using ImageProcessorCore.Formats;
using NLog;


namespace C2eUtils.ImageFormats
{   
    /// <summary>
    /// Decoder for generating an image out of a s16 encoded stream.
    /// </summary>
    public class S16Decoder : IImageDecoder
    {
        public int HeaderSize => 6;

        /// <inheritdoc/>
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
            return extension.Equals("s16", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public bool IsSupportedFileFormat(byte[] header)
        {
            // TODO comprobaciones
            return header.Length == 6;
        }

        /// <inheritdoc/>
        public void Decode(Image image, Stream stream)
        {
            new s16DecoderCore().Decode(image, stream);
        }
    }
    
    internal class s16DecoderCore
    {
         private static Logger logger = LogManager.GetCurrentClassLogger();
        
         private s16Header header;
         private s16ImageHeader[] imageheaders;
         private Image imagedestintion;
         private Stream originStream;
         
         /// <summary>
         /// 
         /// </summary>
         /// <param name="image"></param>
         /// <param name="stream"></param>
         public void Decode(Image image, Stream stream) 
         {
             imagedestintion = image;
             originStream = stream;
             
              // leemos la cabecera del fichero 
              GetHeaderS16(originStream);
              GetImageHeaderS16(originStream);
              foreach (var imageHeader in imageheaders)
              {
                  DecodeSprite(imageHeader,header.RGBFormat);
              }
         }
         
         /// <summary>
         /// 
         /// </summary>
         /// <param name="originStream"></param>
         private void GetHeaderS16(Stream originStream){
              byte[] buffer = new byte[6]; 
              originStream.Read(buffer, 0, buffer.Length); 
              var rgbFormat = BitConverter.ToUInt32(buffer,0);
              var sprites   = BitConverter.ToUInt16(buffer,4);
              header = new s16Header(rgbFormat,sprites);  
              logger.Trace("file header RGB format {0} Sprites: {1}",rgbFormat,sprites);
         }
         
         /// <summary>
         /// 
         /// </summary>
         /// <param name="originStream"></param>
         private void GetImageHeaderS16(Stream originStream){
             imageheaders = new s16ImageHeader[header.Sprites];
              //leemos cada una de las cabeceras de imagen
              for (UInt16 i = 0; i < header.Sprites; i++)
              {
                int offset = 7 + (i * 8);
                byte[] imageHeader = new byte[8];
                originStream.Read(imageHeader,0,8);
                var OffsetFirstImage = BitConverter.ToUInt32(imageHeader,0);
                var Width   = BitConverter.ToUInt16(imageHeader,4);          
                var Height   = BitConverter.ToUInt16(imageHeader,6);
                logger.Trace("image header offset first image {0} Size {1}x{2}",OffsetFirstImage,Width,Height);
                imageheaders[i] = new s16ImageHeader(OffsetFirstImage,Width,Height);    
              }
         }
         
         /// <summary>
         /// 
         /// </summary>
         /// <param name="imageheader"></param>
         /// <param name="RGBFormat"></param>
         private void DecodeSprite(s16ImageHeader imageheader, UInt32 RGBFormat){
             byte[] frameEncoded = new byte[ imageheader.Width * imageheader.Height * 2];
             originStream.Seek(imageheader.OffsetFirstImage, SeekOrigin.Begin);
             originStream.Read(frameEncoded,0,frameEncoded.Length);
             if(RGBFormat == 0){
                 Decode555(frameEncoded, imageheader.Width, imageheader.Height);
             }
             else if(RGBFormat == 1){
                 Decode565(frameEncoded, imageheader.Width, imageheader.Height);
             }
                 
         }
         
         /// <summary>
         /// 
         /// </summary>
         /// <param name="frameEncoded"></param>
         /// <param name="width"></param>
         /// <param name="height"></param>
         private void Decode555(byte[] frameEncoded,UInt16 width , UInt16 height){
                 logger.Trace("Decode 555");
         }
         
         /// <summary>
         /// 
         /// </summary>
         /// <param name="frameEncoded"></param>
         /// <param name="width"></param>
         /// <param name="height"></param>
         private void Decode565(byte[] frameEncoded,UInt16 width , UInt16 height){
                 logger.Trace("Decode 565 {0}x{1}",width,height);
                 float[] pixels = new float[width * height * 4];
                 int offset = 0;
                 int offsetByte = 0;
                 UInt16 pixelcolor ;
                 for (int y = 0; y < height; y++)
                 {
                     for (int x = 0; x < width; x++)
                     {
                             offset     = ((y * width) + x) * 4; 
                             pixelcolor = BitConverter.ToUInt16(frameEncoded, offset/2);
                             // Stored in r-> g-> b-> a order.                
                             pixels[offset]     = (( pixelcolor & 0xF800 ) >> 8 )  / 255f ;// 31f; 
                             pixels[offset + 1] = (( pixelcolor & 0x07E0 ) >> 3 )  / 255f ;// 63f; 
                             pixels[offset + 2] = (( pixelcolor & 0x001F ) << 3 )  / 255f ;// 31f; 
                             pixels[offset + 3] = 1;
                     }
                 }
                 imagedestintion.SetPixels(width, height, pixels);
         }
    }
    
    
    //------------------------------------
    
   internal class s16Header
   {
       // RGB pixel format; 0= 555 sc16,  1= 556 sc16 
       //                  2?= 555 c16 , 3?= 556 c16
        public UInt32    RGBFormat; 
        public UInt16    Sprites;
        
        public s16Header(UInt32 rgbFormat, UInt16 sprites){
            RGBFormat = rgbFormat;
            Sprites = sprites;
        }
    }
    
    internal class s16ImageHeader
    {
        // data lentgh   image width * image height
        public UInt32     OffsetFirstImage;
        public UInt16     Width;
        public UInt16     Height;
        
        public s16ImageHeader(UInt32 offsetFristImage, UInt16 width , UInt16 height){
             OffsetFirstImage = offsetFristImage;
             Width = width;
             Height = height;
         }
    }
}