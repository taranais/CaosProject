using System;
using System.IO;
using ImageProcessorCore;
using ImageProcessorCore.Formats;
using NLog;


namespace C2eUtils.ImageFormats
{
    /// <summary>
    /// Decoder for generating an image out of a BLK encoded stream.
    /// </summary>
    public class BLKDecoder : IImageDecoder
    {
        public int HeaderSize => 10;

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
            return extension.Equals("blk", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public bool IsSupportedFileFormat(byte[] header)
        {
            // TODO comprobaciones
            return header.Length == 10;
        }

        /// <inheritdoc/>
        public void Decode(Image image, Stream stream)
        {
            new BLKDecoderCore().Decode(image, stream);
        }
    }

    /// <summary>
    ///
    /// </summary>
    internal class BLKDecoderCore
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private BLKHeader header;
        private BLKImageHeader[] imageheaders;
        private Image imagedestintion;
        private Stream originStream;
        private float[] imageTemp;

        /// <summary>
        ///
        /// </summary>
        /// <param name="image"></param>
        /// <param name="stream"></param>
        public void Decode(Image image, Stream stream)
        {
            imagedestintion = image;
            originStream = stream;
            GetHeaderBLK(originStream);
            GetImageHeaderBLK(originStream);
            imageTemp = new float[(header.BlocksWidth *128 * header.BlocksHeight *128)  * 4];
            logger.Trace("Decode Image :{0}x{1} Length {2}", header.BlocksWidth *128 , header.BlocksHeight * 128 ,imageTemp.Length );
            for (UInt16 height = 0; height < header.BlocksHeight; height++)
            {
                for (UInt16 width = 0; width < header.BlocksWidth; width++)
                {
                //int pos = (height*header.BlocksWidth) + width;
                int pos = (width *header.BlocksHeight) + height;
                logger.Trace("Decode {0}/{1} => {2}:{3}",pos ,header.Sprites -1 ,height,width);
                DecodeSprite(imageheaders[pos],header.RGBFormat,height,width);
                }
            }
            imagedestintion.SetPixels(header.BlocksWidth * 128 , header.BlocksHeight * 128, imageTemp);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="originStream"></param>
        private void GetHeaderBLK(Stream originStream){
            byte[] buffer = new byte[10];
            originStream.Read(buffer, 0, buffer.Length);
            var rgbFormat = BitConverter.ToUInt32(buffer,0);
            var widthBlocks = BitConverter.ToUInt16(buffer,4);
            var heightBlocks   = BitConverter.ToUInt16(buffer,6);
            var sprites   = BitConverter.ToUInt16(buffer,8);
            header = new BLKHeader(rgbFormat,sprites, widthBlocks, heightBlocks);
            logger.Trace("file header RGB format {0} FRAMES: {1} BLOCKS:{2}x{3}",
                        rgbFormat,sprites,widthBlocks,heightBlocks);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="originStream"></param>
        private void GetImageHeaderBLK(Stream originStream){
            imageheaders = new BLKImageHeader[header.Sprites];
            //leemos cada una de las cabeceras de imagen
            for (UInt16 i = 0; i < header.Sprites; i++)
            {
                int offset = 7 + (i * 8);
                byte[] imageHeader = new byte[8];
                originStream.Read(imageHeader,0,8);
                var OffsetFirstImage = BitConverter.ToUInt32(imageHeader,0);
                var Width   = BitConverter.ToUInt16(imageHeader,4);
                var Height   = BitConverter.ToUInt16(imageHeader,6);
                // logger.Trace("image header offset first image {0} Size {1}x{2}",OffsetFirstImage,Width,Height);
                imageheaders[i] = new BLKImageHeader(OffsetFirstImage,Width,Height);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="imageheader"></param>
        /// <param name="RGBFormat"></param>
        private void DecodeSprite(BLKImageHeader imageheader, UInt32 RGBFormat, UInt16 height, UInt16 width){
            byte[] frameEncoded = new byte[ imageheader.Width * imageheader.Height * 2];
            originStream.Seek(imageheader.Offset, SeekOrigin.Begin);
            originStream.Read(frameEncoded,0,frameEncoded.Length);
            if(RGBFormat == 0){
            //   Decode555(frameEncoded, imageheader.Width, imageheader.Height);
            }
            else if(RGBFormat == 1){
                //logger.Trace("{0}:{1}", height+1, width);
                Decode565(frameEncoded, imageheader.Width, imageheader.Height,height,width);
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
        private void Decode565(byte[] frameEncoded,UInt16 width , UInt16 height, UInt16 heightColumn, UInt16 widthColumn){
                //logger.Trace("Decode 565 {0}x{1}",width,height);
                int offset = 0;
                int offsetX=0;
                int offsetY = 0;
                int  offsetImage = 0;
                UInt16 pixelcolor ;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        offsetX=  (widthColumn  * 128   + x );
                        offsetY= ((heightColumn * 128 ) + y )  ;
                        offsetImage =((offsetY  * ( header.BlocksWidth * 128 ) ) + offsetX) * 4 ;
                    //    logger.Trace("{3}:{4} -> X: {0} Y: {1} O: {2}/{5}",offsetX ,offsetY ,offsetImage , heightColumn, widthColumn, imageTemp.Length);

                            offset     = ((y * width) + x) * 4;
                            pixelcolor = BitConverter.ToUInt16(frameEncoded, offset/2);
                            // Stored in r-> g-> b-> a order.
                            imageTemp[offsetImage]     = (( pixelcolor & 0xF800 ) >> 8 )  / 255f ;// 31f;
                            imageTemp[offsetImage + 1] = (( pixelcolor & 0x07E0 ) >> 3 )  / 255f ;// 63f;
                            imageTemp[offsetImage + 2] = (( pixelcolor & 0x001F ) << 3 )  / 255f ;// 31f;
                            imageTemp[offsetImage + 3] = 1;
                    }
                }
        }
    }

    internal class BLKHeader
    {
        // RGB pixel format; 0= 565  1= 555
        public UInt32   RGBFormat;
        public UInt16   Sprites;
        public UInt16   BlocksWidth;
        public UInt16   BlocksHeight;

        public BLKHeader(UInt32 rgbFormat, UInt16 sprites, UInt16 blocksWidth, UInt16 blocksHeight){
            RGBFormat = rgbFormat;
            Sprites = sprites;
            BlocksWidth = blocksWidth;
            BlocksHeight = blocksHeight;
        }
    }

    internal class BLKImageHeader
    {
        // data lentgh   image width * image height
        public UInt32     Offset;   // offset of the sprite data from the start of the file, minus 4.
        public UInt16     Width;    // 128
        public UInt16     Height;   // 128

        public BLKImageHeader(UInt32 offset, UInt16 width = 128 , UInt16 height = 128){
            Offset = offset + 4;
            Width = width;
            Height = height;
        }
    }
}