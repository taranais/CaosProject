using System;
using System.Text;
using System.IO.MemoryMappedFiles;
using NLog;

namespace C2eUtils.Caos
{
    /// <summary>
    ///
    /// </summary>
    public class BufferLayout{
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public string   c2e                     { get; private set; }
        public int      ProcessID               { get; private set; }
        public int      ResultCode              { get; private set; }
        public uint     Size                    { get; private set; }
        public uint     SharedMemoryBufferSize  { get; private set; }
        public byte[]   Data                    { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="MemViewAccessor"></param>
        /// <returns></returns>
        private string GetControlArray(MemoryMappedViewAccessor MemViewAccessor)
        {
            string ControlCharSet = string.Empty;
            byte[] Caracteres = new byte[4];
            for (int i = 0; i<4 ;i++){
                Caracteres[i] = MemViewAccessor.ReadByte(i);
            }
            ControlCharSet = Encoding.ASCII.GetString(Caracteres);
            return ControlCharSet;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="CaosAsString"></param>
        /// <param name="Action"></param>
        public void PrepareBufferLayout(string CaosAsString, string Action) {
            Log.Trace("Prepare buffer layout");
            Data= GeneratePlayload(CaosAsString,Action);
            Size = Convert.ToUInt32(Data.Length);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="CaosAsString"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        private byte[] GeneratePlayload(string CaosAsString, string Action){
            byte[] action = Encoding.ASCII.GetBytes(Action);
            byte[] caos = Encoding.ASCII.GetBytes(CaosAsString);
            byte[] rv = new byte[action.Length + caos.Length + 2];
            System.Buffer.BlockCopy(action, 0, rv, 0, action.Length);
            System.Buffer.SetByte(rv, action.Length, 13); // '\n'
            System.Buffer.BlockCopy(caos, 0 , rv, action.Length + 1, caos.Length);
            System.Buffer.SetByte(rv, rv.Length -1, 0); // '\0' null terminated text
            return rv;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="MemViewAccessor"></param>
        public void SetSharedMemory(MemoryMappedViewAccessor MemViewAccessor) {
            Log.Trace("Write Shared Memory");
            MemViewAccessor.Write(12, Size);
            for (int i = 0; i < Size; i++)
            {
                MemViewAccessor.Write(24 + i,Data[i]);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="MemViewAccessor"></param>
        public void GetSharedMemory(MemoryMappedViewAccessor MemViewAccessor)
        {
            Log.Trace("Read Shared Memory");
            c2e                     = GetControlArray(MemViewAccessor);
            ProcessID               = MemViewAccessor.ReadInt16(4);
            ResultCode              = MemViewAccessor.ReadInt16(8);
            Size                    = MemViewAccessor.ReadUInt16(12);
            SharedMemoryBufferSize  = MemViewAccessor.ReadUInt16(16);
            Data = new byte[Size];
            for (int i = 0; i < Size; i++)
            {
                Data[i] = MemViewAccessor.ReadByte(24 + i);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static string stringCode(){
            string s = "c2e@";
            return stringToASCII(s);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string stringToASCII(string code){
            byte[] utf = System.Text.Encoding.ASCII.GetBytes(code);
            return System.Text.Encoding.ASCII.GetString(utf);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public CaosResult GetCaosResult()
        {
            if (!c2e.Equals(stringCode()))
            {
                // TODO own exception
                Exception exc = new InvalidOperationException(" either the buffer is corrupt or you're looking in the wrong place");
                Log.Error(exc,"Buffer corrupt: {0}",c2e);
                throw exc;
            }
            else{
                Log.Trace("Buffer tagged: {0}",c2e);
            }
            return new CaosResult(ResultCode, Data, ProcessID);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class CaosResult
    {
        public bool     Failed                  { get; private set; }
        public int      ProcessID               { get; private set; }
        //public string   Content                 { get; private set; }
        public byte[]   Content                 { get; private set; }

        public CaosResult(int failed,  byte[] content, int processID)
        {
            Failed      = Convert.ToBoolean(failed);
            Content     = content;
            ProcessID   = processID;
        }
    }
}