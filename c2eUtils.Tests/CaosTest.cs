using System;

using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.DotNet;

using C2eUtils.Caos;


namespace c2eUtils.Tests.Caos
{
    public class  CaosTest
    {

        [Fact]
        // Game shukd be started
        public void caosDockingStation()
        {
            CaosInjector caosCommand = new CaosInjector(new SharedMemoryInjector("Docking Station"));
            if(caosCommand.Init())
            {
                string test  = "outv 99";
                caosCommand.SendCaosCommand(test);
                caosCommand.Stop();
            }
        }
    }
}
