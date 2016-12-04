using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.DotNet;

using System.IO;

using C2eUtils.PrayFileFormat;


namespace c2eUtils.Tests.Pray
{
    public class  PrayTest
    {

        [Fact]
        public void PrayExport()
        {
            if (!Directory.Exists("TestOutput/Blocks/"))
            {
                Directory.CreateDirectory("TestOutput/Blocks/");
            }

            //string file = @"TestFiles/Areria_001_twig_2z4ce_72266_k3g7z_2jfww.ds.creature";
            string file = @"TestFiles/1_001_star_3ggu4_qbmf6_k3r8v_nphx4.ds.creature";
            PrayFileFormatReader prayFile = new PrayFileFormatReader();
            prayFile.ReadFile(file);
            foreach (PrayBlockHeader header in prayFile.headers)
            {                    
                prayFile.SaveBlock(header, @"TestOutput\\Blocks\\");  
                prayFile.ReadBlock(header);         
            }
        }

    }
}