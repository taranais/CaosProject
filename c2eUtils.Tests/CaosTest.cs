using System;

using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.DotNet;



namespace c2eUtils.Tests
{
    public class  CaosTest
    {

        [Fact]
        // Game shukd be started
        public void caosDockingStation()
        {
            CaosInjector caosCommand = new CaosInjector(new SharedMemoryInjector("Docking Station"));
            caosCommand.Init();

            string test  = "outv 99";
            caosCommand.SendCaosCommand(test);

            caosCommand.Stop();
        }
    }
}
