using System;
using System.IO;
using ImageProcessorCore;
using ImageProcessorCore.Formats;
using NLog;


namespace C2eUtils.ImageFormats
{   
    /// <summary>
    /// Decoder for generating an image out of a gif encoded stream.
    /// </summary>
    public class BLKDecoder : IImageDecoder
    {
        public int HeaderSize => 10;

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
            return extension.Equals("blk", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsSupportedFileFormat(byte[] header)
        {
            // TODO comprobaciones
            return header.Length == 10;
        }

        public void Decode(Image image, Stream stream)
        {
            new BLKDecoderCore().Decode(image, stream);
        }
    }
    
    internal class BLKDecoderCore
    {
         private static Logger logger = LogManager.GetCurrentClassLogger();
        
         private BLKHeader header;
         private BLKImageHeader[] imageheaders;
         private Image imagedestintion;
         private Stream originStream;
         
         public void Decode(Image image, Stream stream) 
         {
             imagedestintion = image;
             originStream = stream;
             
              // leemos la cabecera del fichero 
              GetHeaderBLK(originStream);
              
              
         }
         
         private void GetHeaderBLK(Stream originStream){
             // byte[] buffer = new byte[1*]; 
             // originStream.Read(buffer, 0, buffer.Length); 
             // var rgbFormat = BitConverter.ToUInt32(buffer,0);
             // var sprites   = BitConverter.ToUInt16(buffer,4);
             // var widthBlocks = BitConverter.ToUInt32(buffer,6);
             // var heightBlocks   = BitConverter.ToUInt16(buffer,8);
             // header = new BLKHeader(rgbFormat,sprites);  
             // logger.Trace("file header RGB format {0} Sprites: {1}",rgbFormat,sprites);
         }
         
        
         
         private void DecodeSprite(BLKImageHeader imageheader, UInt32 RGBFormat){
             byte[] frameEncoded = new byte[ imageheader.Width * imageheader.Height * 2];
             originStream.Seek(imageheader.OffsetFirstImage, SeekOrigin.Begin);
             originStream.Read(frameEncoded,0,frameEncoded.Length);
             if(RGBFormat == 0){
              //   Decode555(frameEncoded, imageheader.Width, imageheader.Height);
             }
             else if(RGBFormat == 1){
              //  Decode565(frameEncoded, imageheader.Width, imageheader.Height);
             }
                 
         }
         
         private void Decode555(byte[] frameEncoded,UInt16 width , UInt16 height){
                 logger.Trace("Decode 555");
         }
         
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
    
   internal class BLKHeader
   {
       // RGB pixel format; 0= 565  1= 555 
        public UInt32   RGBFormat; 
        public UInt16   Sprites;
        public UInt16   BlocksWidth;
        public UInt16   BlocksHeight;
        
        
        public BLKHeader(UInt32 rgbFormat, UInt16 sprites){
            RGBFormat = rgbFormat;
            Sprites = sprites;
        }
    }
    
    internal class BLKImageHeader
    {
        // data lentgh   image width * image height
        public UInt32     Offset;   // offset of the sprite data from the start of the file, minus 4.
        public UInt16     Width;    // 128
        public UInt16     Height;   // 128
        
        public BLKImageHeader(UInt32 offset, UInt16 width = 128 , UInt16 height = 128){
             Offset = offset;
             Width = width;
             Height = height;
         }
    }
}