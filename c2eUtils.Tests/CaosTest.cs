using System;
using System.IO;
using System.Globalization;

using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.DotNet;

using C2eUtils.Caos;


namespace c2eUtils.Tests.Caos
{
    public class  CaosTest
    {

        [Theory]
        [InlineData("outv 99","99\0")]
        // Game should be started
        public void caosDockingStation(string test, string output)
        {
            CaosInjector caosCommand = new CaosInjector(new SharedMemoryInjector("Docking Station"));
            if(caosCommand.Init())
            {
                CaosResult result = caosCommand.SendCaosCommand(test);
                string stringResult = System.Text.Encoding.ASCII.GetString(result.Content);
                caosCommand.Stop();
                stringResult.ShouldEqualWithDiff(output);
                //Assert.True(0 == String.Compare(stringResult, ToAscii(output)));
            }
        }

        public string ToAscii(string code){
            byte[] utf = System.Text.Encoding.ASCII.GetBytes(code);
            return System.Text.Encoding.ASCII.GetString(utf);
        }
    }

public static class TestHelpers
{
    public static void ShouldEqualWithDiff(this string actualValue, string expectedValue)
    {
        ShouldEqualWithDiff(actualValue, expectedValue, DiffStyle.Full, Console.Out);
    }

    public static void ShouldEqualWithDiff(this string actualValue, string expectedValue, DiffStyle diffStyle)
    {
        ShouldEqualWithDiff(actualValue, expectedValue, diffStyle, Console.Out);
    }

    public static void ShouldEqualWithDiff(this string actualValue, string expectedValue, DiffStyle diffStyle, System.IO.TextWriter output)
    {
        if(actualValue == null || expectedValue == null)
        {
            //Assert.AreEqual(expectedValue, actualValue);
            Assert.Equal(expectedValue, actualValue);
            return;
        }

        if (actualValue.Equals(expectedValue, StringComparison.Ordinal)) return;

        output.WriteLine("  Idx Expected  Actual");
        output.WriteLine("-------------------------");
        int maxLen = Math.Max(actualValue.Length, expectedValue.Length);
        int minLen = Math.Min(actualValue.Length, expectedValue.Length);
        for (int i = 0; i < maxLen; i++)
        {
            if (diffStyle != DiffStyle.Minimal || i >= minLen || actualValue[i] != expectedValue[i])
            {
                output.WriteLine(String.Format("{0} {1,-3} {2,-4} {3,-3}  {4,-4} {5,-3}",
                    i < minLen && actualValue[i] == expectedValue[i] ? " " : "*", // put a mark beside a differing row
                    i, // the index
                    i < expectedValue.Length ? ((int)expectedValue[i]).ToString() : "", // character decimal value
                    i < expectedValue.Length ? expectedValue[i].ToSafeString() : "", // character safe string
                    i < actualValue.Length ? ((int)actualValue[i]).ToString() : "", // character decimal value
                    i < actualValue.Length ? actualValue[i].ToSafeString() : "" // character safe string
                ));
            }
        }
        output.WriteLine();

        //Assert.AreEqual(expectedValue, actualValue);
        Assert.Equal(expectedValue, actualValue);
    }

    private static string ToSafeString(this char c)
    {
        if (Char.IsControl(c) || Char.IsWhiteSpace(c))
        {
            switch (c)
            {
                case '\r':
                    return @"\r";
                case '\n':
                    return @"\n";
                case '\t':
                    return @"\t";
                case '\a':
                    return @"\a";
                case '\v':
                    return @"\v";
                case '\f':
                    return @"\f";
                case ' ':
                    return @"\s";
                default:
                    return String.Format("\\u{0:X};", (int)c);
            }
        }
        return c.ToString();
    }
}

public enum DiffStyle
{
    Full, 
    Minimal
}


}
