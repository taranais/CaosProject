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
        testCaos();
        }

        private static void testCaos()
        {
          CaosInjector caosCommand = new CaosInjector(new SharedMemoryInjector("Docking Station"));
          //CaosInjector caosCommand = new CaosInjector(new SocketsInjector("Docking Station"));
          
          if(caosCommand.Init())
          {
            string test  = "outv 99";
            caosCommand.SendCaosCommand(test);

            String getallcreatures =
                              "enum 4 0 0 " +                    // iterate creatures
                                "doif targ <> null " +           // check not null ??
                                  "sets va01 gtos 0 " +          // get moniker to va01
                                  "outs va01 " +                 // PRINT moniker
                                  "outs \" | \" " +              // PRINT separator
                                "endi " +
                              "next ";

    
            caosCommand.SendCaosCommand(getallcreatures);

            string moniker= "001-tree-czkfm-custf-q2u2j-jqp4v";
            // TARG (command) agent (agent)
            // NORN (command) creature (agent)
            // SETA (command) var (variable) value (agent)
            // MTOC (agent) moniker (string)
            string testexport  =  "targ mtoc \""  + moniker + "\" "
                                + "pray expo \"WARP\" ";
            caosCommand.SendCaosCommand(testexport);

            // CHEM (float) chemical (integer)
            // Returns concentration (0.0 to 1.0) of chemical (1 to 255) in the target creature's bloodstream.
            caosCommand.Stop();
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
