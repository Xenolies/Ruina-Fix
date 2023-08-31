using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class System : RPGByteData
    {
        int ldbID = 0; // 0a, verbose only (2003)
        string boatCharSet = ""; // 0b
        string shipCharSet = ""; // 0c
        string airshipCharSet = ""; // 0d
        int boatIndex = 0; // 0e
        int shipIndex = 0; // 0f
        int airshipIndex = 0; // 10
        string titleGraphic = ""; // 11
        string gameOverGraphic = ""; // 12
        string systemGraphic = ""; // 13
        string systemGraphic2 = ""; // 14 (2003)
        int initialPartySize = 1; // 15
        int[] initialParty; // 16
        int menuCommandsLength = 0; // 1a (2003)
        byte[] menuCommands; // 1b (2003)
        List<Audio> audios; // 1f through 26 (music), 29 through 34 (sounds)
        int teleportErase = 0; // 3d
        int teleportShow = 0; // 3e
        int battleStartErase = 0; // 3f
        int battleStartShow = 0; // 40
        int battleEndErase = 0; // 41
        int battleEndShow = 0; // 42
        int systemGraphicTile = 0; // 47
        int systemFont = 0; // 48
        int animationTestVictim = 0; // 51, verbose only
        int selectedHero = 0; // 52, verbose only
        string testBackdrop = ""; // 54, verbose only
        List<SystemTestBattler> testBattlers; // 55, verbose only
        int saveCount = 0; // 5b, verbose only
        int battleTestTerrain = 0; // 5e, verbose only
        int battleTestFormation = 0; // 5f, verbose only
        int battleTestCondition = 0; // 60, verbose only
        int unknown61 = 0; // 61, verbose only
        bool showFrame = false; // 63 (2003)
        string frameName = ""; // 64 (2003)
        bool invertAnimations = false; // 65 (2003)
        bool showTitle = true; // 6f, verbose only
        
        static string myClass = "System";
        Chunks chunks;
        
        static string[] audioNames = { "Title Screen", "Battle", "Battle End", "Inn",
                                       "Boat", "Ship", "Airship", "Game Over",
                                       // 0x27 and 0x28
                                       "Cursor Move", "Confirm", "Cancel", "Buzz",
                                       "Battle Start", "Escape", "Enemy Attack", "Damage Enemy",
                                       "Damage Ally", "Evade", "Enemy Die", "Use Item" };
        static string[] menuCommandNames = { "", "Items", "Skills", "Equip", "Save", "Status", "Row", "Order", "ATB Mode" };
        static string[] battleTestConditions = { "No Special Conditions", "Party Initiative", "Enemy Initiative", "Party Pincer", "Enemy Pincer" };
        static string[] battleTestFormations = { "Loose Formation", "Tight Formation", "Use Terrain: " };
        
        public System(FileStream f)
        {
            load(f);
        }
        public System()
        {
        }
        
        override public void load(FileStream f)
        {
            M.currentEvent = "System";
            M.currentPage = "";
            M.currentLine = "";
            M.currentEventNum = 0;
            M.currentPageNum = 0;
            
            chunks = new Chunks(f, myClass);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x0a))
                    ldbID = M.readLengthMultibyte(f);
                if (chunks.next(0x0b))
                    boatCharSet = M.readStringAndRewrite(f, M.M_CHARSET, M.S_FILENAME);
                if (chunks.next(0x0c))
                    shipCharSet = M.readStringAndRewrite(f, M.M_CHARSET, M.S_FILENAME);
                if (chunks.next(0x0d))
                    airshipCharSet = M.readStringAndRewrite(f, M.M_CHARSET, M.S_FILENAME);
                if (chunks.next(0x0e))
                    boatIndex = M.readLengthMultibyte(f);
                if (chunks.next(0x0f))
                    shipIndex = M.readLengthMultibyte(f);
                if (chunks.next(0x10))
                    airshipIndex = M.readLengthMultibyte(f);
                if (chunks.next(0x11))
                    titleGraphic = M.readStringAndRewrite(f, M.M_TITLE, M.S_FILENAME);
                if (chunks.next(0x12))
                    gameOverGraphic = M.readStringAndRewrite(f, M.M_GAMEOVER, M.S_FILENAME);
                if (chunks.next(0x13))
                    systemGraphic = M.readStringAndRewrite(f, M.M_SYSTEM, M.S_FILENAME);
                if (chunks.next(0x14))
                    systemGraphic2 = M.readStringAndRewrite(f, M.M_SYSTEM2, M.S_FILENAME);
                
                if (chunks.next(0x15))
                    initialPartySize = M.readLengthMultibyte(f);
                if (chunks.next(0x16))
                    initialParty = M.readTwoByteArray(f);
                
                if (chunks.next(0x1a))
                    menuCommandsLength = M.readLengthMultibyte(f);
                if (chunks.next(0x1b))
                    menuCommands = M.skipLengthBytes(f);
                
                audios = new List<Audio>();
                for (byte i = 0x1f; i <= 0x34; i++) // Music (0x1f to 0x26) and Sounds (0x29 to 0x34)
                {
                    if (i == 0x27 || i == 0x28) // Not music nor sounds
                        continue;
                    
                    if (chunks.next(i))
                    {
                        Audio audio = new Audio(f, i < 0x27);
                        audios.Add(audio);
                    }
                }
                
                if (chunks.next(0x3d))
                    teleportErase = M.readLengthMultibyte(f);
                if (chunks.next(0x3e))
                    teleportShow = M.readLengthMultibyte(f);
                if (chunks.next(0x3f))
                    battleStartErase = M.readLengthMultibyte(f);
                if (chunks.next(0x40))
                    battleStartShow = M.readLengthMultibyte(f);
                if (chunks.next(0x41))
                    battleEndErase = M.readLengthMultibyte(f);
                if (chunks.next(0x42))
                    battleEndShow = M.readLengthMultibyte(f);
                
                if (chunks.next(0x47))
                    systemGraphicTile = M.readLengthMultibyte(f);
                if (chunks.next(0x48))
                    systemFont = M.readLengthMultibyte(f);
                
                if (chunks.next(0x51))
                    animationTestVictim = M.readLengthMultibyte(f);
                if (chunks.next(0x52))
                    selectedHero = M.readLengthMultibyte(f);
                
                if (chunks.next(0x54))
                    testBackdrop = M.readStringAndRewrite(f, M.M_BACKDROP, M.S_FILENAME);
                if (chunks.next(0x55))
                    testBattlers = M.readList<SystemTestBattler>(f);
                
                if (chunks.next(0x5b))
                    saveCount = M.readLengthMultibyte(f);
                
                if (chunks.next(0x5e))
                    battleTestTerrain = M.readLengthMultibyte(f);
                if (chunks.next(0x5f))
                    battleTestFormation = M.readLengthMultibyte(f);
                if (chunks.next(0x60))
                    battleTestCondition = M.readLengthMultibyte(f);
                if (chunks.next(0x61))
                    unknown61 = M.readLengthMultibyte(f);
                
                if (chunks.next(0x63))
                    showFrame = M.readLengthBool(f);
                if (chunks.next(0x64))
                    frameName = M.readStringAndRewrite(f, M.M_FRAME, M.S_FILENAME);
                
                if (chunks.next(0x65))
                    invertAnimations = M.readLengthBool(f);
                if (chunks.next(0x6f))
                    showTitle = M.readLengthBool(f);
                
                M.byteCheck(f, 0x00);
            }
            else // Skip everything.
            {
                M.skipChunkRange(f, 0x01, 0x80);
                M.byteCheck(f, 0x00);
            }
        }
        
        override public string getString()
        {
            if (M.stringScriptExportMode)
                return "";
            
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            StringWriter initialPartyStr = new StringWriter(new StringBuilder());
            StringWriter systemMusicText = new StringWriter(new StringBuilder());
            StringWriter menuCommandsStr = new StringWriter(new StringBuilder());
            StringWriter testBattlerStr = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < initialParty.Length; i++)
            {
                if (initialParty[i] > 0)
                    initialPartyStr.Write(M.getDataHero(initialParty[i]) + (i < initialPartySize - 1? ", " : ""));
                else
                    initialPartyStr.Write("(None)" + (i < initialPartySize - 1? ", " : ""));
            }
            
            for (int i = 0; i < audios.Count; i++)
                systemMusicText.WriteLine(audioNames[i] + ": " + audios[i].getString());
            
            if (M.is2003)
            {
                for (int i = 0; i < menuCommands.Length; i++)
                    if (menuCommands[i] != 0)
                        menuCommandsStr.WriteLine(menuCommandNames[menuCommands[i]]);
            }
            
            if (M.superVerboseStrings)
                for (int i = 0; i < testBattlers.Count; i++)
                    testBattlerStr.WriteLine(testBattlers[i].getString());
            
            if (M.superVerboseStrings && M.is2003)
                tabText.WriteLine("LDB ID: " + ldbID);
            
            tabText.WriteLine("Boat Graphic: " + boatCharSet + ", " + boatIndex);
            tabText.WriteLine("Ship Graphic: " + shipCharSet + ", " + shipIndex);
            tabText.WriteLine("Airship Graphic: " + airshipCharSet + ", " + airshipIndex);
            tabText.WriteLine("Title Picture: " + titleGraphic);
            tabText.WriteLine("Game Over Picture: " + gameOverGraphic);
            tabText.WriteLine("Default System Graphic: " + systemGraphic + ", "
                + (systemGraphicTile == 0? "Stretch" : "Tile") + ", Font " + (systemFont + 1));
            tabText.WriteLine("Initial Party: " + initialPartyStr);
            tabText.WriteLine();
            
            tabText.WriteLine(systemMusicText);
            
            tabText.WriteLine("Teleport Erase: " + M.getEraseEffects(teleportErase));
            tabText.WriteLine("Teleport Show: " + M.getShowEffects(teleportShow));
            tabText.WriteLine("Battle Start Erase: " + M.getEraseEffects(battleStartErase));
            tabText.WriteLine("Battle Start Show: " + M.getShowEffects(battleStartShow));
            tabText.WriteLine("Battle End Erase: " + M.getEraseEffects(battleEndErase));
            tabText.WriteLine("Battle End Show: " + M.getShowEffects(battleEndShow));
            
            if (M.is2003)
            {
                tabText.WriteLine();
                
                tabText.WriteLine("Menu Commands:");
                tabText.WriteLine(menuCommandsStr);
                
                tabText.WriteLine("Battle Graphics: " + systemGraphic2);
                if (showFrame)
                    tabText.WriteLine("Frame Graphics: " + frameName);
                tabText.WriteLine("Flip Battle Animations in Back Attacks: " + invertAnimations);
            }
            
            if (M.superVerboseStrings)
            {
                tabText.WriteLine();
                
                tabText.WriteLine("Animation Test Victim: " + M.getDataMonster(animationTestVictim));
                tabText.WriteLine("Selected Hero: " + M.getDataHero(selectedHero));
                
                tabText.WriteLine("Battle Test Backdrop: " + testBackdrop);
                tabText.WriteLine("Test Battlers:");
                tabText.Write(testBattlerStr);
                
                if (M.is2003)
                {
                    tabText.WriteLine("Battle Test Condition: " + battleTestConditions[battleTestCondition]);
                    tabText.WriteLine("Battle Test Formation: " + battleTestFormations[battleTestFormation]
                        + (battleTestFormation == 2? M.getDataTerrain(battleTestTerrain) : ""));
                }
                
                tabText.WriteLine();
                
                tabText.WriteLine("Save Count: " + saveCount);
                tabText.WriteLine("Unknown 61: " + unknown61);
                tabText.WriteLine("Show Title: " + showTitle);
            }
            
            return tabText.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x0a))
                M.writeLengthMultibyte(ldbID);
            if (chunks.wasNext(0x0b))
                M.writeString(boatCharSet, M.S_FILENAME);
            if (chunks.wasNext(0x0c))
                M.writeString(shipCharSet, M.S_FILENAME);
            if (chunks.wasNext(0x0d))
                M.writeString(airshipCharSet, M.S_FILENAME);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(boatIndex);
            if (chunks.wasNext(0x0f))
                M.writeLengthMultibyte(shipIndex);
            if (chunks.wasNext(0x10))
                M.writeLengthMultibyte(airshipIndex);
            if (chunks.wasNext(0x11))
                M.writeString(titleGraphic, M.S_FILENAME);
            if (chunks.wasNext(0x12))
                M.writeString(gameOverGraphic, M.S_FILENAME);
            if (chunks.wasNext(0x13))
                M.writeString(systemGraphic, M.S_FILENAME);
            if (chunks.wasNext(0x14))
                M.writeString(systemGraphic2, M.S_FILENAME);
            
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(initialPartySize);
            if (chunks.wasNext(0x16))
                M.writeTwoByteArray(initialParty);
            
            if (chunks.wasNext(0x1a))
                M.writeLengthMultibyte(menuCommandsLength);
            if (chunks.wasNext(0x1b))
            {
                M.writeMultibyte(menuCommands.Length);
                M.writeByteArrayNoLength(menuCommands);
            }
            
            int index = 0;
            for (byte i = 0x1f; i <= 0x34; i++) // Music (0x1f to 0x26) and Sounds (0x29 to 0x34)
            {
                if (i == 0x27 || i == 0x28) // Not music nor sounds
                    continue;
                    
                if (chunks.wasNext(i))
                {
                    Audio audio = audios[index++];
                    audio.write();
                }
            }
            
            if (chunks.wasNext(0x3d))
                M.writeLengthMultibyte(teleportErase);
            if (chunks.wasNext(0x3e))
                M.writeLengthMultibyte(teleportShow);
            if (chunks.wasNext(0x3f))
                M.writeLengthMultibyte(battleStartErase);
            if (chunks.wasNext(0x40))
                M.writeLengthMultibyte(battleStartShow);
            if (chunks.wasNext(0x41))
                M.writeLengthMultibyte(battleEndErase);
            if (chunks.wasNext(0x42))
                M.writeLengthMultibyte(battleEndShow);
            
            if (chunks.wasNext(0x47))
                M.writeLengthMultibyte(systemGraphicTile);
            if (chunks.wasNext(0x48))
                M.writeLengthMultibyte(systemFont);
            
            if (chunks.wasNext(0x51))
                M.writeLengthMultibyte(animationTestVictim);
            if (chunks.wasNext(0x52))
                M.writeLengthMultibyte(selectedHero);
            
            if (chunks.wasNext(0x54))
                M.writeString(testBackdrop, M.S_FILENAME);
            if (chunks.wasNext(0x55))
                M.writeList<SystemTestBattler>(testBattlers);
            
            if (chunks.wasNext(0x5b))
                M.writeLengthMultibyte(saveCount);
            
            if (chunks.wasNext(0x5e))
                M.writeLengthMultibyte(battleTestTerrain);
            if (chunks.wasNext(0x5f))
                M.writeLengthMultibyte(battleTestFormation);
            if (chunks.wasNext(0x60))
                M.writeLengthMultibyte(battleTestCondition);
            if (chunks.wasNext(0x61))
                M.writeLengthMultibyte(unknown61);
            
            if (chunks.wasNext(0x63))
                M.writeLengthBool(showFrame);
            if (chunks.wasNext(0x64))
                M.writeString(frameName, M.S_FILENAME);
            
            if (chunks.wasNext(0x65))
                M.writeLengthBool(invertAnimations);
            if (chunks.wasNext(0x6f))
                M.writeLengthBool(showTitle);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
    
    class SystemTestBattler : RPGByteData
    {
        int id = 0;
        int heroID = 1; // 01
        int level = 1; // 02
        int weaponID = 0; // 0b
        int shieldID = 0; // 0c
        int armorID = 0; // 0d
        int helmetID = 0; // 0e
        int accessoryID = 0; // 0f
        
        static string myClass = "SystemTestBattler";
        Chunks chunks;
        
        public SystemTestBattler(FileStream f)
        {
            load(f);
        }
        public SystemTestBattler()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                heroID = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
                level = M.readLengthMultibyte(f);
            if (chunks.next(0x0b))
                weaponID = M.readLengthMultibyte(f);
            if (chunks.next(0x0c))
                shieldID = M.readLengthMultibyte(f);
            if (chunks.next(0x0d))
                armorID = M.readLengthMultibyte(f);
            if (chunks.next(0x0e))
                helmetID = M.readLengthMultibyte(f);
            if (chunks.next(0x0f))
                accessoryID = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        // Not sure how in the world the huge equipment IDs match up to actual items, so it just prints the raw numbers for now.
        override public string getString()
        {
            return "* Battler #" + id + ": " + M.getDataHero(heroID) + " (Level " + level + ")" + Environment.NewLine
                + "- Weapon: " + (weaponID != 0? weaponID.ToString() : "(None)") + Environment.NewLine
                + "- Shield: " + (shieldID != 0? shieldID.ToString() : "(None)") + Environment.NewLine
                + "- Armor: " + (armorID != 0? armorID.ToString() : "(None)") + Environment.NewLine
                + "- Helmet: " + (helmetID != 0? helmetID.ToString() : "(None)") + Environment.NewLine
                + "- Accessory: " + (accessoryID != 0? accessoryID.ToString() : "(None)");
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(heroID);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(level);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(weaponID);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(shieldID);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(armorID);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(helmetID);
            if (chunks.wasNext(0x0f))
                M.writeLengthMultibyte(accessoryID);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
}
