using System;
using System.IO;
using System.Collections.Generic;

namespace C2eUtils.PrayFileFormat.Blocks
{

    public class GLSTBlock
    {
        public GLSTHeader       Header                  {get;set;}
        public List<GLSTEvent>  Events                  {get;set;}
        public GLSTFooter       Footer                  {get;set;}

        public static GLSTBlock ReadBlock(Stream stream)
        {           
           GLSTBlock glst = null;
           if(ReadHeaderTag(stream))
           {
                glst = new GLSTBlock();
                glst.Header = GLSTHeader.ReadHeader(stream);
                glst.Events = new List<GLSTEvent>();
                for (int i = 0; i < glst.Header.NumberEvents ; i++)
                {
                    GLSTEvent glstEvent = GLSTEvent.ReadEvent(stream);
                    glst.Events.Add(glstEvent);
                }
                glst.Footer = GLSTFooter.ReadFooter(stream);
           }           
           return glst;
        }

        private static bool ReadHeaderTag(Stream stream)
        {
            var knownTag = Utils.EncodeUTF8ToASCII("Creatures Evolution Engine - Archived information file. zLib 1.13 compressed");
            Console.WriteLine(knownTag);
            var unKnownTag = Utils.EncodeUTF8ToASCII("2E1A04");
            Console.WriteLine(unKnownTag);
            byte[] knownTagByte = Utils.StringToByteArray(knownTag);
           // byte[] unknownTag = Utils.StringToByteArray(
           //         Utils.EncodeUTF8ToASCII("2E1A04"));
            return true;
        }
        
    }  
    

    public class GLSTHeader
    {
        public char    ControlChar                      {get;set;}      // byte with value 0x29 [ ' ]
        public Int32   ControlInt                       {get;set;}      // 4-byte integer 1
        Int32   MonikerLength                           {get;set;}      // 4-byte integer always 32 (20)
        public string  Moniker                          {get;set;}      // 32-byte string
        Int32   Moniker2Length                          {get;set;}      // 4-byte integer always 32 (20)
        public string  Moniker2                         {get;set;}      // 32-byte string 32-byte string (always seems to be identical to previous moniker)
        Int32   NameLength                              {get;set;}      // 4-byte integer
        public string  Name                             {get;set;}      // n-byte string
        public Int32   Gender                           {get;set;}      // 4-byte integer (1 (m) or 2 (f))
        public Int32   Genus                            {get;set;}      // 4-byte integer 
        public Int32   Species                          {get;set;}      // 4-byte integer 
        public Int32   NumberEvents                     {get;set;}      // 4-byte integer 

        public static GLSTHeader ReadHeader(Stream stream){
            GLSTHeader header = new GLSTHeader();
            return header;
        }
    }

    

    public class GLSTFooter
    {
        public Int32 PointMutations                     {get;set;}      // during conception (if creature was conceived. Seems to be totally different if the creature was not conceived)	4-byte integer
        public Int32 CrossoverPoints                    {get;set;}      // during conception (if creature was conceived. Seems to be totally different if the creature was not conceived)	4-byte integer
        public Int32 Unknown	                        {get;set;}      // 4-byte integer
        public Int32 Warped                             {get;set;}      // 1 if the creature has been in the warp, 0 otherwise	4-byte integer
        string TextLength                               {get;set;}      // 4-byte integer
        public string Text                              {get;set;}      // (only seems to appear in eggs laid by Muco)	4-byte integer

        public static GLSTFooter ReadFooter(Stream stream){
            GLSTFooter footer = new GLSTFooter();
            return footer;
        }
    }

  

    public class GLSTEvent
    {
        public Int32 EventNumber                        {get;set;}      // (numbers defined below)	4-byte integer
        public Int32 WorldTime	                        {get;set;}      // 4-byte integer
        public Int32 CreatureAge                        {get;set;}	    // 4-byte integer
        public Int32 UNIXTimestamp	                    {get;set;}      // 4-byte integer
        public Int32 LifeStage	                        {get;set;}      // 4-byte integer
        Int32 AssociatedMoniker1Length                  {get;set;}      //	4-byte integer
        public string    AssociatedMoniker1             {get;set;}      // (optional)	n-byte string
        Int32 AssociatedMoniker2Length	                {get;set;}      // 4-byte integer
        public string    AssociatedMoniker2             {get;set;}      // (optional)	n-byte string
        Int32 UserTextLength                            {get;set;}	    // 4-byte integer
        public string    UserText                       {get;set;}      // (optional)	n-byte string
        Int32 AssociatedPHOTBlockNameLength             {get;set;}      //	4-byte integer
        public string    AssociatedPHOTBlockName        {get;set;}      // (optional)	n-byte-string
        Int32 WorldNameLength	                        {get;set;}      // 4-byte integer
        public string    WorldName	                    {get;set;}      // n-byte string
        Int32 WorldUniqueIDLength                       {get;set;}      // (28)	4-byte integer
        public string    WorldUniqueID	                {get;set;}      // 28-byte string
        Int32 DockingStationUserIDLength                {get;set;}      // (8)	4-byte integer
        public string    DockingStationUserID           {get;set;}	    // 8-byte string
        public Int32 Unknown1                           {get;set;}      // (usually if not always 1)	4-byte integer
        public Int32 Unknown2                           {get;set;}      // (usually/always null)	4-byte integer

        public static GLSTEvent ReadEvent(Stream stream){
            GLSTEvent glstEvent = new GLSTEvent();

            return glstEvent;
        }
    }

    
}