using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Troops : RPGByteData
    {
        List<Troop> troops;
        
        public Troops(FileStream f)
        {
            load(f);
        }
        public Troops()
        {
        }
        
        override public void load(FileStream f)
        {
            troops = M.readDatabaseList<Troop>(f, "Troops", "Troop", ref M.troopNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < troops.Count; i++)
            {
                string str = troops[i].getString();
                if (str != "")
                    tabText.Write(str + (i < troops.Count - 1? Environment.NewLine : ""));
            }
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Troop troop in troops)
                troop.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Troop>(troops);
        }
    }
    
    class Troop : RPGDatabaseEntry
    {
        int id = 0;
        string troopName = ""; // 01
        List<TroopMonster> monsters; // 02
        bool autoAlignment = false; // 03 (2003)
        int terrainListSize = 0; // 04
        bool[] appearInTerrains; // 05
        bool appearRandomly = false; // 06 (2003)
        List<TroopPage> pages; // 0b
        
        static string myClass = "Troop";
        Chunks chunks;
        
        public Troop(FileStream f)
        {
            load(f);
        }
        public Troop()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                troopName = M.readStringDataName(f, id, ref M.troopNames, M.S_UNTRANSLATED);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    monsters = M.readList<TroopMonster>(f);
                
                if (chunks.next(0x03))
                    autoAlignment = M.readLengthBool(f);
                
                if (chunks.next(0x04))
                    terrainListSize = M.readLengthMultibyte(f);
                if (chunks.next(0x05))
                    appearInTerrains = M.readBoolArray(f);
                
                if (chunks.next(0x06))
                    appearRandomly = M.readLengthBool(f);
                
                if (chunks.next(0x0b))
                    pages = M.readList<TroopPage>(f, "Page"); // Argument makes it set currentPage/currentPageNum before loading each page
                
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
                string troopHeader = "**********Troop" + id + "**********"
                    + (!M.getExtraneousSetting("TroopNames")? " (" + troopName + ")" : "") + Environment.NewLine;
                
                if (M.getExtraneousSetting("TroopNames"))
                    tabText.WriteLine(M.databaseExportString("TroopName", troopName));
                
                foreach (TroopPage page in pages)
                {
                    string result = page.getString();
                    if (result != "")
                        tabText.WriteLine(result);
                }
                
                return tabText.ToString() != ""? troopHeader + tabText.ToString() : "";
            }
            
            StringWriter monsterListStr = new StringWriter(new StringBuilder());
            StringWriter appearanceListStr = new StringWriter(new StringBuilder());
            StringWriter battleEventText = new StringWriter(new StringBuilder());
            
            foreach (TroopMonster monster in monsters)
                monsterListStr.WriteLine(monster.getString());
            
            for (int terrain = 0; terrain < appearInTerrains.Length; terrain++)
                if (appearInTerrains[terrain])
                    appearanceListStr.WriteLine(M.getDataTerrain(terrain + 1));
            
            foreach (TroopPage page in pages)
            {
                string result = page.getString();
                if (result != "")
                    battleEventText.WriteLine(result);
            }
            
            tabText.WriteLine("Troop #" + id);
            if (M.includeMessages)
                tabText.WriteLine("Name: " + troopName);
            
            if (M.is2003)
            {
                tabText.WriteLine("Alignment: " + (!autoAlignment? "Manual" : "Automatic"));
                tabText.WriteLine("Appears Randomly: " + appearRandomly);
            }
            
            tabText.WriteLine("Monsters:");
            tabText.Write(monsterListStr);
            
            tabText.WriteLine("Appears in Terrain:");
            tabText.Write(appearanceListStr);
            
            tabText.Write(battleEventText);
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            M.currentEvent = "Troop " + id;
            M.currentEventNum = id;
            
            M.importDatabaseString(id, 0, "TroopName", ref troopName, -1);
            if (troopName != "")
                chunks.add(0x01);
            
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i] != null)
                {
                    pages[i].importStrings();
                    M.checkIfImportingStringsExhausted(id, i + 1, true);
                    M.checkIfCommandValuesExhausted(id, i + 1, true);
                }
            }
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(troopName, M.S_UNTRANSLATED);
            if (chunks.wasNext(0x02))
                M.writeList<TroopMonster>(monsters);
            
            if (chunks.wasNext(0x03))
                M.writeLengthBool(autoAlignment);
            
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(terrainListSize);
            if (chunks.wasNext(0x05))
                M.writeBoolArray(appearInTerrains);
            
            if (chunks.wasNext(0x06))
                M.writeLengthBool(appearRandomly);
            
            if (chunks.wasNext(0x0b))
                M.writeList<TroopPage>(pages);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (troopName != "" // 01
             || monsters.Count > 0 // 02
             || pages.Count > 1) // 0b)
                return false;
            
            for (int i = 0; i < appearInTerrains.Length; i++)
                if (!appearInTerrains[i])
                    return false;
            
            foreach (TroopPage page in pages)
                if (!page.isBlank())
                    return false;
            
            return true;
        }
    }
    
    class TroopMonster : RPGByteData
    {
        int id = 0;
        int monster = 1; // 01
        int x = 0; // 02
        int y = 0; // 03
        bool invisible = false; // 04
        
        static string myClass = "TroopMonster";
        Chunks chunks;
        
        public TroopMonster(FileStream f)
        {
            load(f);
        }
        public TroopMonster()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                monster = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
                x = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                y = M.readLengthMultibyte(f);
            if (chunks.next(0x04))
                invisible = M.readLengthBool(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            return M.getDataMonster(monster) + " (" + x + "," + y + ")";
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(monster);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(x);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(y);
            if (chunks.wasNext(0x04))
                M.writeLengthBool(invisible);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
    
    class TroopPage : RPGByteData
    {
        int id = 0;
        TroopPageConditions conditions; // 02
        List<Command> commands; // 0b length, 0c content
        
        static string myClass = "TroopPage";
        Chunks chunks;
        
        public TroopPage(FileStream f)
        {
            load(f);
        }
        public TroopPage()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x02))
            {
                M.readMultibyte(f); // Length
                conditions = new TroopPageConditions(f);
            }
            
            if (chunks.next(0x0b))
                M.readLengthMultibyte(f); // Command section length
            
            M.completeMessage = null;
            M.messageFaceOn = false;
            M.messagePreceded = false;
            
            if (chunks.next(0x0c))
                commands = M.readCommandList(f);
            
            M.byteCheck(f, 0x00);
            
            M.updateTrueChoices(commands);
        }
        
        override public string getString()
        {
            StringWriter battlePageText = new StringWriter(new StringBuilder());
            
            string battlePageHeader = !M.stringScriptExportMode? "--- Page #" + id + " ---" : "---Page" + id + "---";
            string conditionsStr = conditions != null && !M.stringScriptExportMode? conditions.getString() : "";
            
            if (conditionsStr != "")
                battlePageText.Write(conditions);
            
            M.lastWroteAMessage = false;
            
            for (int i = 0; i < commands.Count; i++)
            {
                M.currentLine = "Line " + (i + 1);
                
                bool lastLineOfMessage = false;
                if (M.stringScriptExportMode && i + 1 < commands.Count)
                    lastLineOfMessage = !commands[i + 1].isMessageFollowOrExtraBox();
                
                string commandText = commands[i].getString(lastLineOfMessage);
                if (commandText != "")
                    battlePageText.WriteLine(commandText);
            }
            
            if (M.stringScriptExportMode && !M.wroteStringInPage) // Nothing "worthwhile" was written, so return blank
                return "";
            
            if (battlePageText.ToString() != "")
                return battlePageHeader + Environment.NewLine + battlePageText;
            else
                return "";
        }
        
        public void importStrings()
        {
            M.importPageCommands(ref commands, id);
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x02))
            {
                M.writeMultibyte(conditions.getLength());
                conditions.write();
            }
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(getCommandsLength());
            
            if (chunks.wasNext(0x0c))
                M.writeCommandList(commands);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        int getCommandsLength()
        {
            int length = 0;
            foreach (Command command in commands)
                length += command.getLength();
            return length;
        }
        
        public bool isBlank()
        {
            if (commands.Count > 1) // The end command counts!
                return false;
            
            return true;
        }
    }
    
    class TroopPageConditions : RPGByteData
    {
        bool switch1On = false; // 01 bits
        bool switch2On = false; // 01 bits
        bool variableOn = false; // 01 bits
        bool turnsElapsedOn = false; // 01 bits
        bool partyExhaustionOn = false; // 01 bits
        bool hpRangeMonsterOn = false; // 01 bits
        bool hpRangeHeroOn = false; // 01 bits
        bool turnsTakenMonsterOn = false; // 01 bits (2003)
        bool turnsTakenHeroOn = false; // 01 bits (2003)
        bool commandUsedHeroOn = false; // 01 bits (2003)
        bool flags2003 = false; // Whether 01 has the 2003 number of flags
        int switch1Num = 1; // 02
        int switch2Num = 1; // 03
        int variableNum = 1; // 04
        int variableCompareValue = 0; // 05
        int turnsElapsedX = 0; // 06
        int turnsElapsedPlus = 0; // 07
        int partyExhaustionMin = 0; // 08
        int partyExhaustionMax = 100; // 09
        int hpRangeMonsterNum = 0; // 0a
        int hpRangeMonsterHPMin = 0; // 0b
        int hpRangeMonsterHPMax = 100; // 0c
        int hpRangeHeroID = 0; // 0d
        int hpRangeHeroHPMin = 0; // 0e
        int hpRangeHeroHPMax = 100; // 0f
        int turnsTakenMonsterNum = 0; // 10 (2003)
        int turnsTakenMonsterX = 0; // 11 (2003)
        int turnsTakenMonsterPlus = 0; // 12 (2003)
        int turnsTakenHeroID = 1; // 13 (2003)
        int turnsTakenHeroX = 0; // 14 (2003)
        int turnsTakenHeroPlus = 0; // 15 (2003)
        int commandUsedHeroID = 1; // 16 (2003)
        int commandUsedCommandID = 1; // 17 (2003)
        
        static string myClass = "TroopPageConditions";
        Chunks chunks;
        
        public TroopPageConditions(FileStream f)
        {
            load(f);
        }
        public TroopPageConditions()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
            {
                bool[] flags = M.readLengthFlags(f);
                switch1On = flags[0];
                switch2On = flags[1];
                variableOn = flags[2];
                turnsElapsedOn = flags[3];
                partyExhaustionOn = flags[4];
                hpRangeMonsterOn = flags[5];
                hpRangeHeroOn = flags[6];
                
                if (flags.Length > 8)
                {
                    flags2003 = true;
                    turnsTakenMonsterOn = flags[7];
                    turnsTakenHeroOn = flags[8];
                    commandUsedHeroOn = flags[9];
                }
            }
            
            if (chunks.next(0x02))
                switch1Num = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                switch2Num = M.readLengthMultibyte(f);
            
            if (chunks.next(0x04))
                variableNum = M.readLengthMultibyte(f);
            if (chunks.next(0x05))
                variableCompareValue = M.readLengthMultibyte(f);
            
            if (chunks.next(0x06))
                turnsElapsedX = M.readLengthMultibyte(f);
            if (chunks.next(0x07))
                turnsElapsedPlus = M.readLengthMultibyte(f);
            
            if (chunks.next(0x08))
                partyExhaustionMin = M.readLengthMultibyte(f);
            if (chunks.next(0x09))
                partyExhaustionMax = M.readLengthMultibyte(f);
            
            if (chunks.next(0x0a))
                hpRangeMonsterNum = M.readLengthMultibyte(f);
            if (chunks.next(0x0b))
                hpRangeMonsterHPMin = M.readLengthMultibyte(f);
            if (chunks.next(0x0c))
                hpRangeMonsterHPMax = M.readLengthMultibyte(f);
            
            if (chunks.next(0x0d))
                hpRangeHeroID = M.readLengthMultibyte(f);
            if (chunks.next(0x0e))
                hpRangeHeroHPMin = M.readLengthMultibyte(f);
            if (chunks.next(0x0f))
                hpRangeHeroHPMax = M.readLengthMultibyte(f);
            
            if (chunks.next(0x10))
                turnsTakenMonsterNum = M.readLengthMultibyte(f);
            if (chunks.next(0x11))
                turnsTakenMonsterX = M.readLengthMultibyte(f);
            if (chunks.next(0x12))
                turnsTakenMonsterPlus = M.readLengthMultibyte(f);
            
            if (chunks.next(0x13))
                turnsTakenHeroID = M.readLengthMultibyte(f);
            if (chunks.next(0x14))
                turnsTakenHeroX = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                turnsTakenHeroPlus = M.readLengthMultibyte(f);
            
            if (chunks.next(0x16))
                commandUsedHeroID = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                commandUsedCommandID = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter conditionText = new StringWriter(new StringBuilder());
            
            if (switch1On)
                conditionText.WriteLine("* If Switch " + M.getDataSwitch(switch1Num) + " is On");
            if (switch2On)
                conditionText.WriteLine("* If Switch " + M.getDataSwitch(switch2Num) + " is On");
            if (variableOn)
                conditionText.WriteLine("* If Variable " + M.getDataVariable(variableNum) + " >= " + variableCompareValue);
            if (turnsElapsedOn)
                conditionText.WriteLine("* If Turn " + turnsElapsedX + "X + " + turnsElapsedPlus);
            
            if (M.is2003)
            {
                if (turnsTakenMonsterOn)
                    conditionText.WriteLine("* If Monster " + (turnsTakenMonsterNum + 1) // Monster index in current troop, not monster ID
                        + " Has Taken " + turnsTakenMonsterX + "X + " + turnsTakenMonsterPlus + " Turns");
                if (turnsTakenHeroOn)
                    conditionText.WriteLine("* If " + M.getDataHero(turnsTakenHeroID)
                        + " Has Taken " + turnsTakenHeroX + "X + " + turnsTakenHeroPlus + " Turns");
            }
            
            if (partyExhaustionOn)
                conditionText.WriteLine("* If Party Exhaustion Between "
                    + partyExhaustionMin + "% and " + partyExhaustionMax + "%");
            
            if (hpRangeMonsterOn)
                conditionText.WriteLine("* If Monster " + (hpRangeMonsterNum + 1) // Monster index in current troop, not monster ID
                    + " Has HP " + hpRangeMonsterHPMin + "% to " + hpRangeMonsterHPMax + "%");
            
            if (hpRangeHeroOn)
                conditionText.WriteLine("* If " + M.getDataHero(hpRangeHeroID)
                    + " Has HP " + hpRangeHeroHPMin + "% to " + hpRangeHeroHPMax + "%");
            
            if (M.is2003)
            {
                if (commandUsedHeroOn)
                    conditionText.WriteLine("* If " + M.getDataHero(commandUsedHeroID)
                        + " Uses Battle Command " + M.getDataBattleCommand(commandUsedCommandID));
            }
            
            return conditionText.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
            {
                bool[] flags;
                if (!flags2003)
                    flags = new bool[] { switch1On, switch2On, variableOn, turnsElapsedOn, partyExhaustionOn, hpRangeMonsterOn, hpRangeHeroOn };
                else
                    flags = new bool[] { switch1On, switch2On, variableOn, turnsElapsedOn, partyExhaustionOn,
                                         hpRangeMonsterOn, hpRangeHeroOn, turnsTakenMonsterOn, turnsTakenHeroOn, commandUsedHeroOn };
                M.writeLengthFlags(flags);
            }
            
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(switch1Num);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(switch2Num);
            
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(variableNum);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(variableCompareValue);
            
            if (chunks.wasNext(0x06))
                M.writeLengthMultibyte(turnsElapsedX);
            if (chunks.wasNext(0x07))
                M.writeLengthMultibyte(turnsElapsedPlus);
            
            if (chunks.wasNext(0x08))
                M.writeLengthMultibyte(partyExhaustionMin);
            if (chunks.wasNext(0x09))
                M.writeLengthMultibyte(partyExhaustionMax);
            
            if (chunks.wasNext(0x0a))
                M.writeLengthMultibyte(hpRangeMonsterNum);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(hpRangeMonsterHPMin);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(hpRangeMonsterHPMax);
            
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(hpRangeHeroID);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(hpRangeHeroHPMin);
            if (chunks.wasNext(0x0f))
                M.writeLengthMultibyte(hpRangeHeroHPMax);
            
            if (chunks.wasNext(0x10))
                M.writeLengthMultibyte(turnsTakenMonsterNum);
            if (chunks.wasNext(0x11))
                M.writeLengthMultibyte(turnsTakenMonsterX);
            if (chunks.wasNext(0x12))
                M.writeLengthMultibyte(turnsTakenMonsterPlus);
            
            if (chunks.wasNext(0x13))
                M.writeLengthMultibyte(turnsTakenHeroID);
            if (chunks.wasNext(0x14))
                M.writeLengthMultibyte(turnsTakenHeroX);
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(turnsTakenHeroPlus);
            
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(commandUsedHeroID);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(commandUsedCommandID);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
}
