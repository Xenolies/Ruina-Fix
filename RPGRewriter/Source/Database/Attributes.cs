using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Attributes : RPGByteData
    {
        List<Attribute> attributes;
        
        public Attributes(FileStream f)
        {
            load(f);
        }
        public Attributes()
        {
        }
        
        override public void load(FileStream f)
        {
            attributes = M.readDatabaseList<Attribute>(f, "Attributes", "Attribute", ref M.attributeNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < attributes.Count; i++)
                tabText.Write(attributes[i].getString()
                    + (i < attributes.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Attribute attribute in attributes)
                attribute.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Attribute>(attributes);
        }
    }
    
    class Attribute : RPGDatabaseEntry
    {
        int id = 0;
        string attributeName = ""; // 01
        int physicalOrMagical = 0; // 02
        int rateA = 300; // 0b
        int rateB = 200; // 0c
        int rateC = 100; // 0d
        int rateD = 50; // 0e
        int rateE = 0; // 0f
        
        static string myClass = "Attribute";
        Chunks chunks;
        
        static int attributeNameLimit = 8;
        
        public Attribute(FileStream f)
        {
            load(f);
        }
        public Attribute()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                attributeName = M.readStringDataName(f, id, ref M.attributeNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    physicalOrMagical = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0b))
                    rateA = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    rateB = M.readLengthMultibyte(f);
                if (chunks.next(0x0d))
                    rateC = M.readLengthMultibyte(f);
                if (chunks.next(0x0e))
                    rateD = M.readLengthMultibyte(f);
                if (chunks.next(0x0f))
                    rateE = M.readLengthMultibyte(f);
                
                M.byteCheck(f, 0x00);
            }
            else // Skip over everything but name.
            {
                M.skipChunkRange(f, 0x02, 0x80);
                M.byteCheck(f, 0x00);
            }
        }
        
        public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            if (M.stringScriptExportMode)
            {
                M.setCurrentDatabaseEntry(myClass, id);
                
                tabText.WriteLine("*****Entry" + id + "*****");
                
                tabText.WriteLine(M.databaseExportString("Name", attributeName, "[" + attributeNameLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            tabText.WriteLine("Attribute #" + id);
            if (M.includeMessages)
                tabText.WriteLine("Name: " + attributeName);
            tabText.WriteLine("Type: " + (physicalOrMagical == 0? "Physical" : "Magical"));
            tabText.WriteLine("A Rate: " + rateA + "%");
            tabText.WriteLine("B Rate: " + rateB + "%");
            tabText.WriteLine("C Rate: " + rateC + "%");
            tabText.WriteLine("D Rate: " + rateD + "%");
            tabText.WriteLine("E Rate: " + rateE + "%");
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x11;
            M.importDatabaseString(tabNum, id, "Name", ref attributeName, attributeNameLimit);
            
            if (attributeName != "")
                chunks.add(0x01);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(attributeName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(physicalOrMagical);
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(rateA);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(rateB);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(rateC);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(rateD);
            if (chunks.wasNext(0x0f))
                M.writeLengthMultibyte(rateE);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (attributeName != "" // 01
             || physicalOrMagical != 0 // 02
             || rateA != 300 // 0b
             || rateB != 200 // 0c
             || rateC != 100 // 0d
             || rateD != 50 // 0e
             || rateE != 0) // 0f
                return false;
            
            return true;
        }
    }
}
