using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Terrains : RPGByteData
    {
        List<Terrain> terrains;
        
        public Terrains(FileStream f)
        {
            load(f);
        }
        public Terrains()
        {
        }
        
        override public void load(FileStream f)
        {
            terrains = M.readDatabaseList<Terrain>(f, "Terrains", "Terrain", ref M.terrainNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < terrains.Count; i++)
                tabText.Write(terrains[i].getString()
                    + (i < terrains.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Terrain terrain in terrains)
                terrain.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Terrain>(terrains);
        }
    }
    
    class Terrain : RPGDatabaseEntry
    {
        int id = 0;
        string terrainName = ""; // 01
        int damage = 0; // 02
        int encounterRate = 100; // 03
        string backdrop = ""; // 04
        bool boatPass = false; // 05
        bool shipPass = false; // 06
        bool airshipPass = true; // 07
        bool airshipLand = true; // 09
        int charDisplayType = 0; // 0b
        Audio stepSound; // 0f (2003)
        bool soundOnDamage = false; // 10 (2003)
        int backgroundType = 0; // 11 (2003)
        string backgroundName = ""; // 15 (2003)
        bool backgroundScrollH = false; // 16 (2003)
        bool backgroundScrollV = false; // 17 (2003)
        int backgroundScrollHSpeed = 0; // 18 (2003)
        int backgroundScrollVSpeed = 0; // 19 (2003)
        bool useForeground = false; // 1e (2003)
        string foregroundName = ""; // 1f (2003)
        bool foregroundScrollH = false; // 20 (2003)
        bool foregroundScrollV = false; // 21 (2003)
        int foregroundScrollHSpeed = 0; // 22 (2003)
        int foregroundScrollVSpeed = 0; // 23 (2003)
        bool partyInitiative = false; // 28 bits (2003)
        bool enemyInitiative = false; // 28 bits (2003)
        bool partyPincer = false; // 28 bits (2003)
        bool enemyPincer = false; // 28 bits (2003)
        int partyInitiativeChance = 0; // 29 (2003)
        int enemyInitiativeChance = 0; // 2a (2003)
        int partyPincerChance = 0; // 2b (2003)
        int enemyPincerChance = 0; // 2c (2003)
        int gridDepth = 0; // 2d (2003)
        int gridHorizon = 0; // 2e (2003)
        int gridBreadth = 0; // 2f (2003)
        int gridPerspective = 0; // 30 (2003)
        
        static string myClass = "Terrain";
        Chunks chunks;
        
        static int terrainNameLimit = 16;
        
        static string[] charDisplayTypes = { "Normal", "Bottom Third Translucent", "Bottom Half Translucent", "Fully Translucent" };
        
        public Terrain(FileStream f)
        {
            load(f);
        }
        public Terrain()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                terrainName = M.readStringDataName(f, id, ref M.terrainNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    damage = M.readLengthMultibyte(f);
                if (chunks.next(0x03))
                    encounterRate = M.readLengthMultibyte(f);
                if (chunks.next(0x04))
                    backdrop = M.readStringAndRewrite(f, M.M_BACKDROP, M.S_FILENAME);
                if (chunks.next(0x05))
                    boatPass = M.readLengthBool(f);
                if (chunks.next(0x06))
                    shipPass = M.readLengthBool(f);
                if (chunks.next(0x07))
                    airshipPass = M.readLengthBool(f);
                
                if (chunks.next(0x09))
                    airshipLand = M.readLengthBool(f);
                
                if (chunks.next(0x0b))
                    charDisplayType = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0f))
                    stepSound = new Audio(f);
                
                if (chunks.next(0x10))
                    soundOnDamage = M.readLengthBool(f);
                
                if (chunks.next(0x11))
                    backgroundType = M.readLengthMultibyte(f);
                
                if (chunks.next(0x15))
                    backgroundName = M.readStringAndRewrite(f, M.M_BACKDROP, M.S_FILENAME);
                if (chunks.next(0x16))
                    backgroundScrollH = M.readLengthBool(f);
                if (chunks.next(0x17))
                    backgroundScrollV = M.readLengthBool(f);
                if (chunks.next(0x18))
                    backgroundScrollHSpeed = M.readLengthMultibyte(f);
                if (chunks.next(0x19))
                    backgroundScrollVSpeed = M.readLengthMultibyte(f);
                
                if (chunks.next(0x1e))
                    useForeground = M.readLengthBool(f);
                if (chunks.next(0x1f))
                    foregroundName = M.readStringAndRewrite(f, M.M_BACKDROP, M.S_FILENAME);
                if (chunks.next(0x20))
                    foregroundScrollH = M.readLengthBool(f);
                if (chunks.next(0x21))
                    foregroundScrollV = M.readLengthBool(f);
                if (chunks.next(0x22))
                    foregroundScrollHSpeed = M.readLengthMultibyte(f);
                if (chunks.next(0x23))
                    foregroundScrollVSpeed = M.readLengthMultibyte(f);
                
                if (chunks.next(0x28))
                {
                    bool[] flags = M.readLengthFlags(f);
                    partyInitiative = flags[0];
                    enemyInitiative = flags[1];
                    partyPincer = flags[2];
                    enemyPincer = flags[3];
                }
                
                if (chunks.next(0x29))
                    partyInitiativeChance = M.readLengthMultibyte(f);
                if (chunks.next(0x2a))
                    enemyInitiativeChance = M.readLengthMultibyte(f);
                if (chunks.next(0x2b))
                    partyPincerChance = M.readLengthMultibyte(f);
                if (chunks.next(0x2c))
                    enemyPincerChance = M.readLengthMultibyte(f);
                
                if (chunks.next(0x2d))
                    gridDepth = M.readLengthMultibyte(f);
                if (chunks.next(0x2e))
                    gridHorizon = M.readLengthMultibyte(f);
                if (chunks.next(0x2f))
                    gridBreadth = M.readLengthMultibyte(f);
                if (chunks.next(0x30))
                    gridPerspective = M.readLengthMultibyte(f);
                
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
                
                tabText.WriteLine(M.databaseExportString("Name", terrainName, "[" + terrainNameLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            tabText.WriteLine("Terrain #" + id);
            if (M.includeMessages)
                tabText.WriteLine("Name: " + terrainName);
            tabText.WriteLine("Damage: " + damage);
            tabText.WriteLine("Encounter Rate: " + encounterRate + "%");
            
            if (M.is2003)
            {
                tabText.WriteLine("Sound Effect: " + stepSound.getString());
                tabText.WriteLine("Play Sound If Damaged: " + soundOnDamage);
                
                tabText.WriteLine("Background Type: " + (backgroundType == 0? "Background" : "Frame"));
                
                if (backgroundType == 0) // Background
                    tabText.WriteLine("Backdrop: " + backdrop);
                else // Frame
                {
                    tabText.WriteLine("Background: " + backgroundName);
                    if (backgroundScrollH)
                        tabText.WriteLine("Horizontal Scroll: " + backgroundScrollHSpeed);
                    if (backgroundScrollV)
                        tabText.WriteLine("Vertical Scroll: " + backgroundScrollVSpeed);
                    
                    if (useForeground)
                    {
                        tabText.WriteLine("Foreground: " + foregroundName);
                        if (foregroundScrollH)
                            tabText.WriteLine("Horizontal Scroll: " + foregroundScrollHSpeed);
                        if (foregroundScrollV)
                            tabText.WriteLine("Vertical Scroll: " + foregroundScrollVSpeed);
                    }
                }
                
                if (partyInitiative)
                    tabText.WriteLine("Party Initiative Chance: " + partyInitiativeChance);
                if (enemyInitiative)
                    tabText.WriteLine("Enemy Initiative Chance: " + enemyInitiativeChance);
                if (partyPincer)
                    tabText.WriteLine("Party Pincer Chance: " + partyPincerChance);
                if (enemyPincer)
                    tabText.WriteLine("Enemy Pincer Chance: " + enemyPincerChance);
                
                if (gridDepth != 0) // 0 is no selection, 1 and 2 are Shallow/Deep
                    tabText.WriteLine("Grid Depth of Field: " + (gridDepth == 2? "Deep" : "Shallow"));
                tabText.WriteLine("Grid Horizon (Top Line): " + gridHorizon);
                tabText.WriteLine("Grid Breadth (Field Size): " + gridBreadth);
                tabText.WriteLine("Grid Perspective (Vertical Angle): " + gridPerspective);
            }
            else
                tabText.WriteLine("Backdrop: " + backdrop);
            
            tabText.WriteLine("Boat Can Pass: " + boatPass);
            tabText.WriteLine("Ship Can Pass: " + shipPass);
            tabText.WriteLine("Airship Can Pass: " + airshipPass);
            tabText.WriteLine("Airship Can Land: " + airshipLand);
            tabText.WriteLine("Character Display: " + charDisplayTypes[charDisplayType]);
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x10;
            M.importDatabaseString(tabNum, id, "Name", ref terrainName, terrainNameLimit);
            
            if (terrainName != "")
                chunks.add(0x01);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(terrainName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(damage);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(encounterRate);
            if (chunks.wasNext(0x04))
                M.writeString(backdrop, M.S_FILENAME);
            if (chunks.wasNext(0x05))
                M.writeLengthBool(boatPass);
            if (chunks.wasNext(0x06))
                M.writeLengthBool(shipPass);
            if (chunks.wasNext(0x07))
                M.writeLengthBool(airshipPass);
                
            if (chunks.wasNext(0x09))
                M.writeLengthBool(airshipLand);
                
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(charDisplayType);
                
            if (chunks.wasNext(0x0f))
                stepSound.write();
            
            if (chunks.wasNext(0x10))
                M.writeLengthBool(soundOnDamage);
            
            if (chunks.wasNext(0x11))
                M.writeLengthMultibyte(backgroundType);
            
            if (chunks.wasNext(0x15))
                M.writeString(backgroundName, M.S_FILENAME);
            if (chunks.wasNext(0x16))
                M.writeLengthBool(backgroundScrollH);
            if (chunks.wasNext(0x17))
                M.writeLengthBool(backgroundScrollV);
            if (chunks.wasNext(0x18))
                M.writeLengthMultibyte(backgroundScrollHSpeed);
            if (chunks.wasNext(0x19))
                M.writeLengthMultibyte(backgroundScrollVSpeed);
            
            if (chunks.wasNext(0x1e))
                M.writeLengthBool(useForeground);
            if (chunks.wasNext(0x1f))
                M.writeString(foregroundName, M.S_FILENAME);
            if (chunks.wasNext(0x20))
                M.writeLengthBool(foregroundScrollH);
            if (chunks.wasNext(0x21))
                M.writeLengthBool(foregroundScrollV);
            if (chunks.wasNext(0x22))
                M.writeLengthMultibyte(foregroundScrollHSpeed);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(foregroundScrollVSpeed);
            
            if (chunks.wasNext(0x28))
                M.writeLengthFlags(new bool[] { partyInitiative, enemyInitiative, partyPincer, enemyPincer });
            
            if (chunks.wasNext(0x29))
                M.writeLengthMultibyte(partyInitiativeChance);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(enemyInitiativeChance);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(partyPincerChance);
            if (chunks.wasNext(0x2c))
                M.writeLengthMultibyte(enemyPincerChance);
            
            if (chunks.wasNext(0x2d))
                M.writeLengthMultibyte(gridDepth);
            if (chunks.wasNext(0x2e))
                M.writeLengthMultibyte(gridHorizon);
            if (chunks.wasNext(0x2f))
                M.writeLengthMultibyte(gridBreadth);
            if (chunks.wasNext(0x30))
                M.writeLengthMultibyte(gridPerspective);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (terrainName != "" // 01
             || damage != 0 // 02
             || encounterRate != 100 // 03
             || backdrop != "" // 04
             || boatPass // 05
             || shipPass // 06
             || airshipPass // 07
             || airshipLand // 09
             || charDisplayType != 0 // 0b
             || !stepSound.isBlank() // 0f
             || soundOnDamage // 10
             || backgroundType != 0 // 11
             || backgroundName != "" // 15
             || backgroundScrollH // 16
             || backgroundScrollV // 17
             || backgroundScrollHSpeed != 0 // 18
             || backgroundScrollVSpeed != 0 // 19
             || useForeground // 1e
             || foregroundName != "" // 1f
             || foregroundScrollH // 20
             || foregroundScrollV // 21
             || foregroundScrollHSpeed != 0 // 22
             || foregroundScrollVSpeed != 0 // 23
             || partyInitiative // 28 bits
             || enemyInitiative // 28 bits
             || partyPincer // 28 bits
             || enemyPincer // 28 bits
             || partyInitiativeChance != 0 // 29
             || enemyInitiativeChance != 0 // 2a
             || partyPincerChance != 0 // 2b
             || enemyPincerChance != 0 // 2c
             || gridDepth != 0 // 2d
             || gridHorizon != 0 // 2e
             || gridBreadth != 0 // 2f
             || gridPerspective != 0) // 30
                return false;
            
            return true;
        }
    }
}
