using System;


using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.DotNet;

using C2eUtils.Caos;


namespace c2eUtils.Tests.Caos
{
    public class  CaosTest
    {

        [Theory]
        [InlineData("outv 99","99")]
        // Game should be started
        public void caosDockingStation(string test, string output)
        {
            CaosInjector caosCommand = new CaosInjector(new SharedMemoryInjector("Docking Station"));
            if(caosCommand.Init())
            {
                CaosResult result = caosCommand.SendCaosCommand(test);
                string stringResult = System.Text.Encoding.ASCII.GetString(result.Content);
                caosCommand.Stop();
                Assert.True(0 == String.Compare(stringResult, ToAscii(output)));
            }
        }

        public string ToAscii(string code){
            byte[] utf = System.Text.Encoding.ASCII.GetBytes(code);
            return System.Text.Encoding.ASCII.GetString(utf);
        }
    }
}
