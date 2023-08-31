using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class MapTree : RPGDataFile
    {
        string filepath = "";
        string filename = "";
        
        int mapCount = 0;
        int lastOpenedMap = 0;
        int[] mapTreeList;
        MapTreeMap[] maps;
        
        int partyMapID = 0; // 01
        int partyX = 0; // 02
        int partyY = 0; // 03
        int boatMapID = 0; // 0b
        int boatX = 0; // 0c
        int boatY = 0; // 0d
        int shipMapID = 0; // 15
        int shipX = 0; // 16
        int shipY = 0; // 17
        int airshipMapID = 0; // 1f
        int airshipX = 0; // 20
        int airshipY = 0; // 21
        
        static string myClass = "MapTree";
        Chunks chunks;
        
        public MapTree(string filepath, bool writeLog = true)
        {
            loadFile(filepath, writeLog);
        }
        public MapTree()
        {
        }
        
        public bool loadFile(string filepath, bool writeLog = true)
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Map tree file " + filepath + " not found.");
                return false;
            }
            
            this.filepath = filepath;
            filename = Path.GetFileName(filepath);
            M.currentFile = filename;
            
            if (M.readingDataNames)
                Console.WriteLine("Retrieving map names from map tree...");
            else if (!M.stringScriptImportMode)
                Console.WriteLine(M.globalMode + " map tree...");
            
            FileStream f = File.OpenRead(filepath);
            
            try
            {
                chunks = new Chunks(f, myClass);
                
                M.stringCheck(f, "LcfMapTree");
                
                mapCount = M.readMultibyte(f);
                if (M.readingDataNames)
                    M.mapNames = new string[10000]; // Physical map count may be lower than the highest number, so mapCount isn't safe to use here.
                
                maps = new MapTreeMap[10000];
                for (int i = 0; i < mapCount; i++)
                {
                    M.currentEvent = "Map Tree Index " + i;
                    M.currentPage = "";
                    M.currentLine = "";
                    M.currentEventNum = i;
                    M.currentPageNum = 0;
                    
                    MapTreeMap map = new MapTreeMap(f);
                    maps[map.getID()] = map;
                }
                
                mapTreeList = M.readMultibyteArray(f);
                
                lastOpenedMap = M.readMultibyte(f);
                
                if (chunks.next(0x01))
                    partyMapID = M.readLengthMultibyte(f);
                if (chunks.next(0x02))
                    partyX = M.readLengthMultibyte(f);
                if (chunks.next(0x03))
                    partyY = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0b))
                    boatMapID = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    boatX = M.readLengthMultibyte(f);
                if (chunks.next(0x0d))
                    boatY = M.readLengthMultibyte(f);
                
                if (chunks.next(0x15))
                    shipMapID = M.readLengthMultibyte(f);
                if (chunks.next(0x16))
                    shipX = M.readLengthMultibyte(f);
                if (chunks.next(0x17))
                    shipY = M.readLengthMultibyte(f);
                
                if (chunks.next(0x1f))
                    airshipMapID = M.readLengthMultibyte(f);
                if (chunks.next(0x20))
                    airshipX = M.readLengthMultibyte(f);
                if (chunks.next(0x21))
                    airshipY = M.readLengthMultibyte(f);
                
                M.byteCheck(f, 0x00);
                
                f.Close();
            }
            catch (Exception e)
            {
                M.debugMessage(e.StackTrace);
                M.debugMessage(e.Message);
                Console.WriteLine("Aborting due to error.");
                
                f.Close();
                return false;
            }
            
            if (writeLog)
                M.logSave();
            return true;
        }
        
        public string getString()
        {
            StringWriter mapTreeText = new StringWriter(new StringBuilder());
            
            if (M.stringScriptExportMode)
            {
                if (M.getExtraneousSetting("MapNames"))
                {
                    for (int i = 1; i < maps.Length; i++)
                        if (maps[i] != null)
                            mapTreeText.WriteLine(maps[i].getString());
                    return mapTreeText.ToString();
                }
                else
                    return "";
            }
            
            mapTreeText.WriteLine("Party Start Position: " + (partyMapID != 0? (M.getDataMap(partyMapID) + " (" + partyX + "," + partyY + ")") : "N/A"));
            mapTreeText.WriteLine("Boat Start Position: " + (boatMapID != 0? (M.getDataMap(boatMapID) + " (" + boatX + "," + boatY + ")") : "N/A"));
            mapTreeText.WriteLine("Ship Start Position: " + (shipMapID != 0? (M.getDataMap(shipMapID) + " (" + shipX + "," + shipY + ")") : "N/A"));
            mapTreeText.WriteLine("Airship Start Position: " + (airshipMapID != 0? (M.getDataMap(airshipMapID) + " (" + airshipX + "," + airshipY + ")") : "N/A"));
            mapTreeText.WriteLine();
            
            for (int i = 0; i < maps.Length; i++)
                if (maps[i] != null)
                    mapTreeText.WriteLine(maps[i].getString());
            
            mapTreeText.WriteLine();
            
            for (int i = 0; i < mapTreeList.Length; i++)
                mapTreeText.WriteLine(maps[mapTreeList[i]].getMapTreeString());
            
            return mapTreeText.ToString();
        }
        
        public void importStrings(string scriptDir)
        {
            M.loadStringScriptDatabase(scriptDir + "\\MapTree.txt", 0, true);
            foreach (MapTreeMap map in maps)
                if (map != null)
                    map.importStrings();
        }
        
        public bool writeFile()
        {
            if (M.fileInUse(filepath))
            {
                Console.WriteLine(filename + " is in use; cannot save.");
                return false;
            }
            
            File.Delete(filepath + ".bak");
            File.Move(filepath, filepath + ".bak");
            BinaryWriter mapTreeWriter = new BinaryWriter(new FileStream(filepath, FileMode.Create));
            M.targetWriter = mapTreeWriter;
            
            try
            {
                M.writeString("LcfMapTree", M.S_CONSTANT);
                
                M.writeMultibyte(mapCount);
                
                foreach (MapTreeMap map in maps)
                    if (map != null)
                        map.write();
                
                M.writeMultibyteArray(mapTreeList);
                
                M.writeMultibyte(lastOpenedMap);
                
                if (chunks.wasNext(0x01))
                    M.writeLengthMultibyte(partyMapID);
                if (chunks.wasNext(0x02))
                    M.writeLengthMultibyte(partyX);
                if (chunks.wasNext(0x03))
                    M.writeLengthMultibyte(partyY);
                
                if (chunks.wasNext(0x0b))
                    M.writeLengthMultibyte(boatMapID);
                if (chunks.wasNext(0x0c))
                    M.writeLengthMultibyte(boatX);
                if (chunks.wasNext(0x0d))
                    M.writeLengthMultibyte(boatY);
                
                if (chunks.wasNext(0x15))
                    M.writeLengthMultibyte(shipMapID);
                if (chunks.wasNext(0x16))
                    M.writeLengthMultibyte(shipX);
                if (chunks.wasNext(0x17))
                    M.writeLengthMultibyte(shipY);
                
                if (chunks.wasNext(0x1f))
                    M.writeLengthMultibyte(airshipMapID);
                if (chunks.wasNext(0x20))
                    M.writeLengthMultibyte(airshipX);
                if (chunks.wasNext(0x21))
                    M.writeLengthMultibyte(airshipY);
                
                M.writeByte(0x00);
                
                mapTreeWriter.Close();
                M.targetWriter.Close();
                File.Delete(filepath + ".bak");
                
                chunks.validateParity();
            }
            catch (Exception e)
            {
                M.debugMessage(e.StackTrace);
                M.debugMessage(e.Message);
                Console.WriteLine("Aborting due to error; keeping original file.");
                
                // Close file and restore backup.
                mapTreeWriter.Close();
                M.targetWriter.Close();
                File.Delete(filepath);
                File.Move(filepath + ".bak", filepath);
                return false;
            }
            
            return true;
        }
    }
    
    class MapTreeMap : RPGData
    {
        int id = 0;
        string mapName = ""; // 01
        int openStatus = 0; // 02
        int mapIndent = 0; // 03
        bool isMap = false; // 04
        int xScroll = 0; // 05
        int yScroll = 0; // 06
        bool hasChildren = false; // 07
        int bgmOption = 0; // 0b
        Audio bgm; // 0c
        int battleBGOption = 0; // 15
        string battleBG = ""; // 16
        int teleport = 0; // 1f
        int escape = 0; // 20
        int save = 0; // 21
        List<MapTreeEncounter> encounters; // 29
        int encounterSteps = 25; // 2c
        long[] areaRect; // 33
        
        static string myClass = "MapTreeMap";
        Chunks chunks;
        
        static string[] bgmOptions = { "Parent Map", "No Change", "Set To" };
        static string[] battleBGOptions = { "Parent Map", "Based on Terrain", "Set To" };
        static string[] enableOptions = { "Parent Map", "Enable", "Disable" };
        
        public MapTreeMap(FileStream f)
        {
            load(f);
        }
        public MapTreeMap()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            M.currentEvent = "Map #" + id;
            M.currentEventNum = id;
            
            if (chunks.next(0x01))
                mapName = M.readStringDataName(f, id, ref M.mapNames, M.S_UNTRANSLATED);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    openStatus = M.readLengthMultibyte(f);
                if (chunks.next(0x03))
                    mapIndent = M.readLengthMultibyte(f);
                if (chunks.next(0x04))
                    isMap = M.readLengthBool(f);
                if (chunks.next(0x05))
                    xScroll = M.readLengthMultibyte(f);
                if (chunks.next(0x06))
                    yScroll = M.readLengthMultibyte(f);
                if (chunks.next(0x07))
                    hasChildren = M.readLengthBool(f);
                
                if (chunks.next(0x0b))
                    bgmOption = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    bgm = new Audio(f, true);
                
                if (chunks.next(0x15))
                    battleBGOption = M.readLengthMultibyte(f);
                if (chunks.next(0x16))
                    battleBG = M.readStringAndRewrite(f, M.M_BACKDROP, M.S_FILENAME);
                
                if (chunks.next(0x1f))
                    teleport = M.readLengthMultibyte(f);
                if (chunks.next(0x20))
                    escape = M.readLengthMultibyte(f);
                if (chunks.next(0x21))
                    save = M.readLengthMultibyte(f);
                
                if (chunks.next(0x29))
                    encounters = M.readList<MapTreeEncounter>(f);
                
                if (chunks.next(0x2c))
                    encounterSteps = M.readLengthMultibyte(f);
                
                if (chunks.next(0x33))
                    areaRect = M.readFourByteArray(f);
                
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
            StringWriter mapTreeText = new StringWriter(new StringBuilder());
            
            if (M.stringScriptExportMode)
            {
                if (M.getExtraneousSetting("MapNames"))
                {
                    mapTreeText.WriteLine("*****Entry" + id + "*****");
                    
                    mapTreeText.WriteLine(M.databaseExportString("Name", mapName));
                    mapTreeText.WriteLine();
                    
                    return mapTreeText.ToString();
                }
                else
                    return "";
            }
            
            StringWriter encounterList = new StringWriter(new StringBuilder());
            
            foreach (MapTreeEncounter encounter in encounters)
                encounterList.WriteLine(encounter.getString());
            
            mapTreeText.WriteLine("Map #" + id);
            if (M.includeMessages)
                mapTreeText.WriteLine("Name: " + mapName);
            mapTreeText.WriteLine("BGM: " + bgmOptions[bgmOption] + (bgmOption == 2? " " + bgm.getString() : ""));
            mapTreeText.WriteLine("Battle BG: " + battleBGOptions[battleBGOption] + (battleBGOption == 2? " " + battleBG : ""));
            mapTreeText.WriteLine("Teleport: " + enableOptions[teleport]);
            mapTreeText.WriteLine("Escape: " + enableOptions[escape]);
            mapTreeText.WriteLine("Save: " + enableOptions[save]);
            
            if (encounterList.ToString() != "")
            {
                mapTreeText.WriteLine("Encounters:");
                mapTreeText.Write(encounterList);
            }
            mapTreeText.WriteLine("Encounter Steps: " + encounterSteps);
            
            if (M.superVerboseStrings)
            {
                mapTreeText.WriteLine("Open Status: " + openStatus);
                mapTreeText.WriteLine("Is Map: " + isMap);
                mapTreeText.WriteLine("Has Children: " + hasChildren);
                mapTreeText.WriteLine("Area Rect: " + areaRect[0] + ", " + areaRect[1] + ", " + areaRect[2] + ", " + areaRect[3]);
            }
            
            return mapTreeText.ToString();
        }
        
        public string getMapTreeString()
        {
            string indentStr = "";
            for (int i = 0; i < mapIndent; i++)
                indentStr += "   ";
            
            return indentStr + "[" + id + "]" + (M.includeMessages? " " + mapName : "");
        }
        
        public int getID()
        {
            return id;
        }
        
        public void importStrings()
        {
            M.importDatabaseString(0, id, "Name", ref mapName);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(mapName, M.S_UNTRANSLATED);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(openStatus);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(mapIndent);
            if (chunks.wasNext(0x04))
                M.writeLengthBool(isMap);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(xScroll);
            if (chunks.wasNext(0x06))
                M.writeLengthMultibyte(yScroll);
            if (chunks.wasNext(0x07))
                M.writeLengthBool(hasChildren);
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(bgmOption);
            if (chunks.wasNext(0x0c))
                bgm.write();
            
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(battleBGOption);
            if (chunks.wasNext(0x16))
                M.writeString(battleBG, M.S_FILENAME);
            
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(teleport);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(escape);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(save);
            
            if (chunks.wasNext(0x29))
                M.writeList<MapTreeEncounter>(encounters);
            
            if (chunks.wasNext(0x2c))
                M.writeLengthMultibyte(encounterSteps);
            
            if (chunks.wasNext(0x33))
                M.writeFourByteArray(areaRect);
            
            M.writeByte(0x00);
        }
    }
    
    class MapTreeEncounter : RPGByteData
    {
        int id = 0;
        int troopID = 1; // 01
        
        static string myClass = "MapTreeEncounter";
        Chunks chunks;
        
        public MapTreeEncounter(FileStream f)
        {
            load(f);
        }
        public MapTreeEncounter()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                troopID = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            return M.getDataTroop(troopID);
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(troopID);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
}
