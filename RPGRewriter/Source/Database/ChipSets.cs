using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class ChipSets : RPGByteData
    {
        List<ChipSet> chipSets;
        
        public ChipSets(FileStream f)
        {
            load(f);
        }
        public ChipSets()
        {
        }
        
        override public void load(FileStream f)
        {
            chipSets = M.readDatabaseList<ChipSet>(f, "ChipSets", "ChipSet", ref M.chipSetNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < chipSets.Count; i++)
                tabText.Write(chipSets[i].getString()
                    + (i < chipSets.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (ChipSet chipSet in chipSets)
                chipSet.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<ChipSet>(chipSets);
        }
    }
    
    class ChipSet : RPGDatabaseEntry
    {
        int id = 0;
        string chipSetName = ""; // 01
        string chipSetFile = ""; // 02
        int[] lowerTerrain; // 03
        List<ChipSetChip> lowerChips; // 04
        List<ChipSetChip> upperChips; // 05
        int seaSequence = 0; // 0b
        int seaSpeed = 0; // 0c
        
        static string myClass = "ChipSet";
        Chunks chunks;
        
        public static int chipSetNameLimit = 20;
        
        public ChipSet(FileStream f)
        {
            load(f);
        }
        public ChipSet()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                chipSetName = M.readStringDataName(f, id, ref M.chipSetNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    chipSetFile = M.readStringAndRewrite(f, M.M_CHIPSET, M.S_FILENAME);
                
                if (chunks.next(0x03))
                    lowerTerrain = M.readTwoByteArray(f);
                if (chunks.next(0x04))
                    lowerChips = M.readListNoLength<ChipSetChip>(f);
                if (chunks.next(0x05))
                    upperChips = M.readListNoLength<ChipSetChip>(f);
                
                if (chunks.next(0x0b))
                    seaSequence = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    seaSpeed = M.readLengthMultibyte(f);
                
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
                
                tabText.WriteLine(M.databaseExportString("Name", chipSetName, "[" + chipSetNameLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter lowerTerrainStr = new StringWriter(new StringBuilder());
            StringWriter lowerPassingStr = new StringWriter(new StringBuilder());
            StringWriter lowerPassFourStr = new StringWriter(new StringBuilder());
            StringWriter upperPassingStr = new StringWriter(new StringBuilder());
            StringWriter upperPassFourStr = new StringWriter(new StringBuilder());
            StringWriter upperCounterStr = new StringWriter(new StringBuilder());
            
            if (lowerTerrain != null)
            {
                for (int chip = 0; chip < lowerTerrain.Length; chip++)
                {
                    bool lastOfLine = ((chip + 1) % 6 == 0);
                    
                    lowerTerrainStr.Write(lowerTerrain[chip] + (lastOfLine? "" : " "));
                    if (lastOfLine)
                        lowerTerrainStr.WriteLine();
                }
            }
            
            if (lowerChips != null)
            {
                for (int chip = 0; chip < lowerChips.Count; chip++)
                {
                    ChipSetChip c = lowerChips[chip];
                    bool lastOfLine = ((chip + 1) % 6 == 0);
                    
                    lowerPassingStr.Write(c.getPassingString() + (lastOfLine? "" : " "));
                    lowerPassFourStr.Write(c.getPassFourString() + (lastOfLine? "" : " "));
                    
                    if (lastOfLine)
                    {
                        lowerPassingStr.WriteLine();
                        lowerPassFourStr.WriteLine();
                    }
                }
            }
            
            if (upperChips != null)
            {
                for (int chip = 0; chip < upperChips.Count; chip++)
                {
                    ChipSetChip c = upperChips[chip];
                    bool lastOfLine = ((chip + 1) % 6 == 0);
                    
                    upperPassingStr.Write(c.getPassingString() + (lastOfLine? "" : " "));
                    upperPassFourStr.Write(c.getPassFourString() + (lastOfLine? "" : " "));
                    upperCounterStr.Write(c.getCounterString() + (lastOfLine? "" : " "));
                    
                    if (lastOfLine)
                    {
                        upperPassingStr.WriteLine();
                        upperPassFourStr.WriteLine();
                        upperCounterStr.WriteLine();
                    }
                }
            }
            
            tabText.WriteLine("ChipSet #" + id);
            if (M.includeMessages)
                tabText.WriteLine("Name: " + chipSetName);
            tabText.WriteLine("ChipSet: " + chipSetFile);
            tabText.WriteLine("Sea Sequence: " + (seaSequence == 0? "1-2-3-2" : "1-2-3"));
            tabText.WriteLine("Sea Speed: " + (seaSpeed == 0? "Slow" : "Fast"));
            tabText.WriteLine("Lower Chip Terrain:");
            tabText.Write(lowerTerrainStr);
            tabText.WriteLine("Lower Chip Passing:");
            tabText.Write(lowerPassingStr);
            tabText.WriteLine("Lower Chip Four-Pass:");
            tabText.Write(lowerPassFourStr);
            tabText.WriteLine("Upper Chip Passing:");
            tabText.Write(upperPassingStr);
            tabText.WriteLine("Upper Chip Four-Pass:");
            tabText.Write(upperPassFourStr);
            tabText.WriteLine("Upper Chip Counter:");
            tabText.Write(upperCounterStr);
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x14;
            M.importDatabaseString(tabNum, id, "Name", ref chipSetName, chipSetNameLimit);
            
            if (chipSetName != "")
                chunks.add(0x01);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(chipSetName, M.S_TOTRANSLATE);
            
            if (chunks.wasNext(0x02))
                M.writeString(chipSetFile, M.S_FILENAME);
            
            if (chunks.wasNext(0x03))
                M.writeTwoByteArray(lowerTerrain);
            if (chunks.wasNext(0x04))
                M.writeListNoLength<ChipSetChip>(lowerChips);
            if (chunks.wasNext(0x05))
                M.writeListNoLength<ChipSetChip>(upperChips);
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(seaSequence);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(seaSpeed);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (chipSetName != "" // 01
             || chipSetFile != "" // 02
             // 03 below
             // 04 below
             // 05 below
             || seaSequence != 0 // 0b
             || seaSpeed != 0) // 0c
                return false;
            
            if (lowerTerrain != null) // 03
                for (int i = 0; i < lowerTerrain.Length; i++)
                    if (lowerTerrain[i] != 1)
                        return false;
            
            if (lowerChips != null) // 04
                foreach (ChipSetChip chip in lowerChips)
                    if (!chip.isBlank())
                        return false;
            
            if (upperChips != null) // 05
                foreach (ChipSetChip chip in upperChips)
                    if (!chip.isBlank())
                        return false;
            
            return true;
        }
    }
    
    class ChipSetChip : RPGData
    {
        bool down = false;
        bool left = false;
        bool right = false;
        bool up = false;
        bool star = false;
        bool square = false;
        bool counter = false;
        int passType = 0;
        
        static string[] passTypes = { "X", "□", "★", "O" };
        
        public ChipSetChip(FileStream f)
        {
            load(f);
        }
        public ChipSetChip()
        {
        }
        
        public void load(FileStream f)
        {
            int c = M.readByte(f);
            down = (c & 1) != 0;
            left = (c & 2) != 0;
            right = (c & 4) != 0;
            up = (c & 8) != 0;
            star = (c & 16) != 0;
            square = (c & 32) != 0;
            counter = (c & 64) != 0;
            
            passType = 0; // X
            if (square)
                passType = 1; // Square
            else if (star)
                passType = 2; // Star
            else if (down || left || right || up)
                passType = 3; // Circle
        }
        
        public string getString()
        {
            return getPassingString();
        }
        
        public string getPassingString()
        {
            return passTypes[passType];
        }
        
        public string getPassFourString()
        {
            return (up? "U" : "x")
                 + (right? "R" : "x")
                 + (down? "D" : "x")
                 + (left? "L" : "x");
        }
        
        public string getCounterString()
        {
            return counter? "◆" : "x";
        }
        
        public void write()
        {
            int c = (down? 1 : 0)
                  + (left? 2 : 0)
                  + (right? 4 : 0)
                  + (up? 8 : 0)
                  + (star? 16 : 0)
                  + (square? 32 : 0)
                  + (counter? 64 : 0);
            
            M.writeByte(c);
        }
        
        public bool isBlank()
        {
            if (down
             || left
             || right
             || up
             || star
             || square
             || counter
             || passType != 0)
                return false;
            
            return true;
        }
    }
}
