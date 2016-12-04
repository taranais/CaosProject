using System;
using System.Text;
using System.Linq;

namespace C2eUtils
{
    public static class Utils
    {
        public static string EncodeASCIIToUTF8(string text){
            // encode the string as an ASCII byte array
            byte[] myASCIIBytes = Encoding.ASCII.GetBytes(text);
            // convert the ASCII byte array to a UTF-8 byte array
            byte[] myUTF8Bytes = Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.UTF8, myASCIIBytes);
            // reconstitute a string from the UTF-8 byte array 
            return Encoding.UTF8.GetString(myUTF8Bytes);
        }

        public static string EncodeUTF8ToASCII(string text){
            // encode the string as an UTF-8 byte array
            byte[] myUTF8Bytes = Encoding.UTF8.GetBytes(text);
            // convert the UTF-8 byte array to a ASCII byte array
            byte[] myASCIIBytes = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, myUTF8Bytes);
            // reconstitute a string from the ASCII byte array 
            return Encoding.ASCII.GetString(myASCIIBytes);
        }

        public static byte[] StringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                            .ToArray();
        }
    }
}