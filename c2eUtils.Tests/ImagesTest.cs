using System;
using System.IO;

using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.DotNet;

using ImageProcessorCore;
using ImageProcessorCore.Formats;
using ImageProcessorCore.Quantizers;

using C2eUtils.ImageFormats;



namespace c2eUtils.Tests.C2eImages
{
    public class  C2EImagesTest
    {

        [Fact(Skip="unImplemented")]
        public void c16Image()
        {
            if (!Directory.Exists("TestOutput/Decode/c16"))
            {
                Directory.CreateDirectory("TestOutput/Decode/c16");
            }
            string file = @"TestFiles/favouriteplaces.c16";
            using (FileStream stream = File.OpenRead(file))
            {
                IImageFormat[] formats = new IImageFormat[1];
                // formats[0]= new C16Format();
                //using (Image image = new Image(stream, formats))
                //{
                //   string encodeFilename = "TestOutput/Decode/c16/" + Path.GetFileNameWithoutExtension(file) + ".png";
                //   using (FileStream output = File.OpenWrite(encodeFilename))
                //   {
                //       Image frame = new Image( image.Frames[0] );
                //       frame.Save(output, new PngEncoder());
                //   }
                //}
            }
        }

        [Fact]
        public void s16Image()
        {
            if (!Directory.Exists("TestOutput/Decode/s16"))
            {
                Directory.CreateDirectory("TestOutput/Decode/s16");
            }
            string file = @"TestFiles/001-tree-czkfm-custf-q2u2j-jqp4v-5.s16";
            using (FileStream stream = File.OpenRead(file))
            {
                IImageFormat[] formats = new IImageFormat[1];
                formats[0]= new S16Format();
                using (Image image = new Image(stream, formats))
                {
                    string encodeFilename = "TestOutput/Decode/s16/" + Path.GetFileNameWithoutExtension(file) + ".png";
                    using (FileStream output = File.OpenWrite(encodeFilename))
                    {
                        image.Save(output, new PngEncoder());
                    }
                }
            }
        }

        [Fact]
        public void blkImage()
        {
            if (!Directory.Exists("TestOutput/Decode/blk"))
            {
                Directory.CreateDirectory("TestOutput/Decode/blk");
            }
            string file = @"TestFiles/DS_splash.blk";
            using (FileStream stream = File.OpenRead(file))
            {
                IImageFormat[] formats = new IImageFormat[1];
                formats[0]= new BLKFormat();
                using (Image image = new Image(stream, formats))
                {
                    string encodeFilename = "TestOutput/Decode/blk/" + Path.GetFileNameWithoutExtension(file) + ".png";
                    using (FileStream output = File.OpenWrite(encodeFilename))
                    {
                        image.Save(output, new PngEncoder());
                    }
                }
            }
        }
    }
}
