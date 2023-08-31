using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class BattleSettings : RPGByteData
    {
        int placement = 0; // 02
        int deathHandler = 0; // 04
        int rowShown = 0; // 06
        int battleType = 0; // 07
        int unknown09 = 0; // 09
        List<BattleCommand> battleCommands; // 0a
        int deathHandler2 = 0; // 0f
        int deathCommonEvent = 0; // 10
        int windowSize = 0; // 14
        int transparentWindow = 0; // 18
        bool deathTeleport = false; // 19
        int teleportMap = 1; // 1a
        int teleportX = 0; // 1b
        int teleportY = 0; // 1c
        int teleportDir = 0; // 1d
        
        static string myClass = "BattleSettings";
        Chunks chunks;
        
        static string[] placements = { "Manual", "Automatic" };
        static string[] battleTypes = { "Traditional", "Alternative", "Gauge" };
        static string[] windowSizes = { "Large", "Small" };
        static string[] deathHandlers = { "Game Over", "Call Event" };
        static string[] teleportDirs = { "Keep Direction", "Face Up", "Face Right", "Face Down", "Face Left" };
        static string[] rowShowns = { "Front", "Back" };
        
        public BattleSettings(FileStream f)
        {
            load(f);
        }
        public BattleSettings()
        {
        }
        
        override public void load(FileStream f)
        {
            M.currentEvent = "Battle Settings";
            M.currentPage = "";
            M.currentLine = "";
            M.currentEventNum = 0;
            M.currentPageNum = 0;
            
            chunks = new Chunks(f, myClass);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    placement = M.readLengthMultibyte(f);
                if (chunks.next(0x04))
                    deathHandler = M.readLengthMultibyte(f);
                if (chunks.next(0x06))
                    rowShown = M.readLengthMultibyte(f);
                if (chunks.next(0x07))
                    battleType = M.readLengthMultibyte(f);
                if (chunks.next(0x09))
                    unknown09 = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0a))
                    battleCommands = M.readList<BattleCommand>(f);
                
                if (chunks.next(0x0f))
                    deathHandler2 = M.readLengthMultibyte(f);
                if (chunks.next(0x10))
                    deathCommonEvent = M.readLengthMultibyte(f);
                if (chunks.next(0x14))
                    windowSize = M.readLengthMultibyte(f);
                if (chunks.next(0x18))
                    transparentWindow = M.readLengthMultibyte(f);
                if (chunks.next(0x19))
                    deathTeleport = M.readLengthBool(f);
                if (chunks.next(0x1a))
                    teleportMap = M.readLengthMultibyte(f);
                if (chunks.next(0x1b))
                    teleportX = M.readLengthMultibyte(f);
                if (chunks.next(0x1c))
                    teleportY = M.readLengthMultibyte(f);
                if (chunks.next(0x1d))
                    teleportDir = M.readLengthMultibyte(f);
                
                M.skipChunkRange(f, 0x01, 0x80);
                
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
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            if (M.stringScriptExportMode)
            {
                foreach (BattleCommand command in battleCommands)
                    tabText.WriteLine(command.getString());
                
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter battleCommandStr = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < battleCommands.Count; i++)
                battleCommandStr.WriteLine(battleCommands[i].getString());
            
            tabText.WriteLine("Character Placement: " + placements[placement]);
            tabText.WriteLine("Battle Type: " + battleTypes[battleType]);
            if (battleType != 2) // No window in Gauge
            {
                tabText.WriteLine("Window Size: " + windowSizes[windowSize]);
                if (battleType == 1) // Alternative
                    tabText.WriteLine("Transparent Window: " + transparentWindow);
            }
            tabText.WriteLine();
            
            tabText.WriteLine("Death Handling: " + deathHandlers[deathHandler]);
            if (deathHandler == 1)
            {
                tabText.WriteLine("Death Common Event: " + M.getDataCommon(deathCommonEvent));
                if (deathTeleport)
                    tabText.WriteLine("Teleport Before Death Event: "
                        + M.getDataMap(teleportMap) + " (" + teleportX + "," + teleportY + "), " + teleportDirs[teleportDir]);
            }
            tabText.WriteLine();
            
            tabText.WriteLine("Battle Commands:");
            tabText.Write(battleCommandStr);
            
            if (M.superVerboseStrings)
            {
                tabText.WriteLine();
                
                tabText.WriteLine("Death Handling 2: " + deathHandler2
                    + (deathHandler2 < deathHandlers.Length? (" (" + deathHandlers[deathHandler2] + ")") : ""));
                tabText.WriteLine("Row Shown: " + rowShowns[rowShown]);
                tabText.WriteLine("Unknown 09: " + unknown09);
            }
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (BattleCommand command in battleCommands)
                command.importStrings();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(placement);
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(deathHandler);
            if (chunks.wasNext(0x06))
                M.writeLengthMultibyte(rowShown);
            if (chunks.wasNext(0x07))
                M.writeLengthMultibyte(battleType);
            if (chunks.wasNext(0x09))
                M.writeLengthMultibyte(unknown09);
            
            if (chunks.wasNext(0x0a))
                M.writeList<BattleCommand>(battleCommands);
            
            if (chunks.wasNext(0x0f))
                M.writeLengthMultibyte(deathHandler2);
            if (chunks.wasNext(0x10))
                M.writeLengthMultibyte(deathCommonEvent);
            if (chunks.wasNext(0x14))
                M.writeLengthMultibyte(windowSize);
            if (chunks.wasNext(0x18))
                M.writeLengthMultibyte(transparentWindow);
            if (chunks.wasNext(0x19))
                M.writeLengthBool(deathTeleport);
            if (chunks.wasNext(0x1a))
                M.writeLengthMultibyte(teleportMap);
            if (chunks.wasNext(0x1b))
                M.writeLengthMultibyte(teleportX);
            if (chunks.wasNext(0x1c))
                M.writeLengthMultibyte(teleportY);
            if (chunks.wasNext(0x1d))
                M.writeLengthMultibyte(teleportDir);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
    
    class BattleCommand : RPGByteData
    {
        int id = 0;
        string commandName = ""; // 01
        int commandType = 0; // 02
        
        static string myClass = "BattleCommand";
        Chunks chunks;
        
        static int commandNameLimit = 10;
        
        public BattleCommand(FileStream f)
        {
            load(f);
        }
        public BattleCommand()
        {
        }
        
        static string[] commandTypes = { "Attack", "Skill", "Skill Subset", "Defend", "Item", "Escape", "Link to Event" };
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                commandName = M.readString(f, M.S_TOTRANSLATE);
            if (chunks.next(0x02))
                commandType = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            if (M.stringScriptExportMode)
                return M.databaseExportString("Command" + id, commandName, "[" + commandNameLimit + "]");
            
            return commandName + " (Type: " + commandTypes[commandType] + ")";
        }
        
        public void importStrings()
        {
            int tabNum = 0x1d;
            M.importDatabaseString(tabNum, 0, "Command" + id, ref commandName, commandNameLimit);
            
            if (commandName != "")
                chunks.add(0x01);
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(commandName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(commandType);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
}
