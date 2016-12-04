using System;
using System.IO;
using System.Collections.Generic;

using System.Linq;
using NLog;
using ComponentAce.Compression.Libs.zlib;
using C2eUtils.PrayFileFormat.Blocks;
using C2eUtils;

namespace C2eUtils.PrayFileFormat
{
    /// <summary>
    /// 
    /// </summary>
    public class PrayFileFormatReader
    {

        public string FilePath                      { get; set;}
        public string TagHeader                     { get; set;}
        public List<PrayBlockHeader> headers        { get; set;}

        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        public void ReadFile(string filePath){
                if(filePath == null)
                {
                    throw new ArgumentNullException("filePath");
                }
                headers = new List<PrayBlockHeader>();
                FilePath = filePath;
                // TODO comprobar si exite el fichero
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    long position = 0;
                    TagHeader = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4));
                    if(TagHeader == "PRAY")
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)  // Check EOF 
                        {
                            PrayBlockHeader prayHeader = PrayBlockHeader.ReadHeader(reader);
                            headers.Add(prayHeader);
                            // movemos el puntero de lectura
                            position = reader.BaseStream.Position;
                            position = position + prayHeader.BlockDataSize;
                            reader.BaseStream.Seek(position, SeekOrigin.Begin);
                        }
                    }
                    else{
                         throw new ArgumentException("Not Pray a file", "File type exception");
                    }
                }
        }

        public void DecompileBlock(PrayBlockHeader header)
        {
            string fileOut = header.BlockType;
            using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
            {      
                using(FileStream outFileStream = new FileStream(fileOut, FileMode.Create))   
                {
                    reader.BaseStream.Seek(header.DataOffSet, SeekOrigin.Begin);
                    using( ZInputStream zinput = new ZInputStream(reader.BaseStream,6))
                    {
                        int data = 0;
                        while(outFileStream.Length < header.BlockDataSizeUnCompressed ||  data != -1) // Check EOF and end of Block
                        {
                            data = zinput.Read();
                            byte _dataByte = (byte)data;
                            outFileStream.WriteByte(_dataByte);    
                        }
                    }
                }                                  
            }       
        }

        public void SaveBlock(PrayBlockHeader header, string outPutDir)
        {
            string fileOut = CleanFileName(header.BlockName);
            using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
            {      
                using(FileStream outFileStream = new FileStream(outPutDir + fileOut, FileMode.Create))   
                {
                    
                    byte[] datosLeidos = new  byte[header.BlockDataSize];
                    reader.BaseStream.Seek(header.DataOffSet, SeekOrigin.Begin);
                    reader.Read(datosLeidos,0,header.BlockDataSize);
                    outFileStream.Write(datosLeidos,0,header.BlockDataSize);
                }                                
            }       
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars()
                        .Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        

        public Stream InflateStream(PrayBlockHeader header){
            MemoryStream stream = new MemoryStream();
            using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
            { 
                reader.BaseStream.Seek(header.DataOffSet, SeekOrigin.Begin);
                using( ZInputStream zinput = new ZInputStream(reader.BaseStream, 6))
                {
                    int data = 0;
                    while(stream.Length < header.BlockDataSizeUnCompressed)
                    {
                        data = zinput.Read();
                        byte _dataByte = (byte)data;
                        stream.WriteByte(_dataByte);    
                    }
                }
            }
            return stream;
        }
        


        public void ReadBlock(PrayBlockHeader header)
        {       
            switch (header.BlockType)
            {
                case "GLST":
                    GLSTBlock.ReadBlock(InflateStream(header));
                    break;
                default:
                    break;
            }
        }
    }

    public class PrayBlockHeader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 4 bytes
        /// </summary>
        /// <returns></returns>
        public string BlockType {get;set;}

        /// <summary>
        /// 128 bytes
        /// </summary>
        /// <returns></returns>
        public string BlockName {get;set;}

        /// <summary>
        /// Int32 Block dataSize
        /// </summary>
        /// <returns></returns>
        public Int32 BlockDataSize {get;set;}

        /// <summary>
        /// Int32 Block dataSize Un Compressed
        /// </summary>
        /// <returns></returns>
        public Int32 BlockDataSizeUnCompressed {get;set;}

        /// <summary>
        /// Int32 Size
        /// </summary>
        /// <returns></returns>
        public Boolean ZlibCompressed {get;set;}

        /// <summary>
        /// File position offset
        /// </summary>
        /// <returns></returns>
        public long DataOffSet  {get;set;}

        public static PrayBlockHeader ReadHeader(BinaryReader bReader)
        {
                PrayBlockHeader header = new PrayBlockHeader();
                
                header.BlockType                    = Utils.EncodeASCIIToUTF8(System.Text.Encoding.ASCII.GetString(bReader.ReadBytes(4)));
                header.BlockName                    = Utils.EncodeASCIIToUTF8(System.Text.Encoding.ASCII.GetString(bReader.ReadBytes(128)));
                header.BlockDataSize                = bReader.ReadInt32();
                header.BlockDataSizeUnCompressed    = bReader.ReadInt32();
                header.ZlibCompressed               = Convert.ToBoolean(bReader.ReadInt32());
                header.DataOffSet                   = bReader.BaseStream.Position;

                logger.Trace( "Pray file header: BlockType: {0} BlockName: {1}"
                            + " BlockDataSize: {2} BlockDataSizeUnCompressed: {3}"
                            + " ZlibCompressed: {4} DataOffSet: {5}",
                            header.BlockType, header.BlockName,
                            header.BlockDataSize, header.BlockDataSizeUnCompressed,
                            header.ZlibCompressed, header.DataOffSet);
                return header;
        }
    }
}