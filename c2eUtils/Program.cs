using System;
using System.IO;
using System.Threading;

using NLog;
using NLog.Targets;
using NLog.Config;

using C2eUtils.Caos;
using C2eUtils.ImageFormats;

using ImageProcessorCore;
using ImageProcessorCore.Formats;
using ImageProcessorCore.Quantizers;


namespace C2eUtils
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
        Config();
        
      //  CaosInjector caosCommand = new CaosInjector(new SharedMemoryInjector("Docking Station"));
        //CaosInjector caosCommand = new CaosInjector(new SocketsInjector("Docking Station"));
     //   caosCommand.Init();
        
        
        String textToExecute =
                          "enum 4 0 0 " +                    // iterate creatures
                            "doif targ <> null " +           // check not null ??
                              "sets va01 gtos 0 " +          // get moniker to va01
                              "outs va01 " +                 // PRINT moniker
                              "outs \" | \" " +              // PRINT separator
                            "endi " +
                          "next ";

   //    textToExecute= "outv 99";
   
   string file=@"C:\Users\taran\Documents\Creatures\Docking Station\Images\000e.c16";
   file = @"C:\Users\taran\Documents\Creatures\Docking Station\My Worlds\test\Images\001-star-3ggu4-qbmf6-k3r8v-nphx4-3.s16";
   file = @"C:\Program Files\GOG.com\Creatures Exodus\Docking Station\Backgrounds\NornMeso039.blk";
  //     x16Loader loader = new x16Loader(file);
        
        for (int i=0; i<1000 ; i++) {
      //    caosCommand.SendCaosCommand(textToExecute);    
       //   Thread.Sleep(1000);
        }
     //   caosCommand.Stop();
     
     string ficherosalida = @"a.png";
     
          using (FileStream stream = File.OpenRead(file)){
            
            using (FileStream output = File.OpenWrite(ficherosalida))
            {
               IImageFormat format = new JpegFormat();
               IImageFormat[] formats = new IImageFormat[1];
               formats[0]= new BLKFormat();
                using (Image image = new Image(stream, formats))
                {
                  //Image frame = new Image( image.Frames[2] );
                  //frame.Save(output);
                  //  image.Save(output, new BmpEncoder());
                  image.Save(output, new PngEncoder());
                  //  image.Save(output, new JpgEncoder());
                }
            }
          }
          
      }
 
        private static void Config(){
            
        // Step 1. Create configuration object
        var config = new LoggingConfiguration();

        // Step 2. Create targets and add them to the configuration 
        var consoleTarget = new ColoredConsoleTarget();
        config.AddTarget("console", consoleTarget);

        // Step 3. Set target properties
        //consoleTarget.Layout = "";// @"${date:format=HH\:mm\:ss} ${logger} ${message}"

        // Step 4. Define rules
        var rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
        config.LoggingRules.Add(rule1);

        // Step 5. Activate the configuration
        LogManager.Configuration = config;

        }
    }
}
