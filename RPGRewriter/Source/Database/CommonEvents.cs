using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class CommonEvents : RPGByteData
    {
        List<CommonEvent> commonEvents;
        
        public CommonEvents(FileStream f)
        {
            load(f);
        }
        public CommonEvents()
        {
        }
        
        override public void load(FileStream f)
        {
            commonEvents = M.readDatabaseList<CommonEvent>(f, "Common Events", "Common", ref M.commonNames);
        }
        
        // Default version.
        override public string getString()
        {
            return getString(false, "");
        }
        
        // Returns string of all common events, or writes each event to a file.
        public string getString(bool writeFiles = false, string scriptDir = "")
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            StreamWriter allCommonFile = null;
            if (writeFiles && !M.stringScriptExportMode)
            {
                Directory.CreateDirectory(scriptDir);
                allCommonFile = new StreamWriter(scriptDir + "\\AllCommons.txt", false, M.UNICODE);
            }
            
            for (int i = 0; i < commonEvents.Count; i++)
            {
                M.currentEvent = "Common " + (i + 1);
                M.currentEventNum = i + 1;
                
                string result = commonEvents[i].getString(writeFiles, scriptDir);
                if (result != "")
                {
                    tabText.WriteLine(result + Environment.NewLine);
                    if (writeFiles && !M.stringScriptExportMode)
                        allCommonFile.WriteLine(result + Environment.NewLine);
                }
            }
            
            if (writeFiles && !M.stringScriptExportMode)
                allCommonFile.Close();
            
            return tabText.ToString();
        }
        
        public void importStrings(string scriptDir)
        {
            foreach (CommonEvent commonEvent in commonEvents)
                commonEvent.importStrings(scriptDir);
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<CommonEvent>(commonEvents);
        }
    }
    
    class CommonEvent : RPGDatabaseEntry
    {
        int id = 0;
        string commonName = ""; // 01
        int eventTrigger = 5; // 0b
        bool usingSwitch = false; // 0c
        int switchNum = 1; // 0d
        List<Command> commands; // 15 length, 16 content
        
        static string myClass = "CommonEvent";
        Chunks chunks;
        
        static string[] eventTriggers = { "0", "1", "2", "Auto Start", "Parallel Process", "Call" };
        
        public CommonEvent(FileStream f)
        {
            load(f);
        }
        public CommonEvent()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                commonName = M.readStringDataName(f, id, ref M.commonNames, M.S_UNTRANSLATED);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x0b))
                    eventTrigger = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    usingSwitch = M.readLengthBool(f);
                if (chunks.next(0x0d))
                    switchNum = M.readLengthMultibyte(f);
                
                if (chunks.next(0x15))
                    M.readLengthMultibyte(f); // Commands section length
                
                M.completeMessage = null;
                M.messageFaceOn = false;
                M.messagePreceded = false;
                
                if (chunks.next(0x16))
                    commands = M.readCommandList(f);
                
                M.byteCheck(f, 0x00);
                
                M.updateTrueChoices(commands);
            }
            else // Skip over everything but name.
            {
                M.skipChunkRange(f, 0x02, 0x80);
                M.byteCheck(f, 0x00);
            }
        }
        
        public string getString()
        {
            return getString(false, "");
        }
        
        public string getString(bool writeFiles = false, string commonScriptDir = "")
        {
            StringWriter commonHeader = new StringWriter(new StringBuilder());
            StringWriter commonConditions = new StringWriter(new StringBuilder());
            StringWriter commonText = new StringWriter(new StringBuilder());
            
            if (M.checkUnusedData && eventTrigger != 5) // Auto Start and Parallel Process common events should be instantly considered "used."
                M.unusedDataEntries["Common Events"].Remove(id);
            
            if (!M.stringScriptExportMode)
            {
                if (M.includeMessages)
                    commonHeader.WriteLine("===== Common " + id + ": " + commonName + " =====");
                else
                    commonHeader.WriteLine("===== Common " + id + " =====");
                
                commonConditions.WriteLine("* Event Trigger: " + eventTriggers[eventTrigger]);
                if (usingSwitch)
                    commonConditions.WriteLine("* If Switch " + M.getDataSwitch(switchNum));
                commonConditions.WriteLine();
            }
            else if (M.getExtraneousSetting("CommonEventNames"))
                commonHeader.WriteLine(M.databaseExportString("CommonEventName", commonName));
            
            M.wroteStringInPage = false;
            M.lastWroteAMessage = false;
            
            for (int i = 0; i < commands.Count; i++)
            {
                M.currentLine = "Line " + (i + 1);
                
                bool lastLineOfMessage = false;
                if (M.stringScriptExportMode && i + 1 < commands.Count)
                    lastLineOfMessage = !commands[i + 1].isMessageFollowOrExtraBox();
                
                string commandText = commands[i].getString(lastLineOfMessage);
                if (commandText != "")
                    commonText.WriteLine(commandText);
            }
            
            if (commonText.ToString() != "" || M.stringScriptExportMode)
            {
                if (M.stringScriptExportMode && !M.wroteStringInPage) // Nothing "worthwhile" was written, so blank it out
                    commonText.GetStringBuilder().Clear();
                
                string completeStr = commonHeader + (M.includeActions? commonConditions.ToString() : "") + commonText;
                if (writeFiles)
                    M.writeToNewFile(commonScriptDir + "\\Common" + id.ToString("D4") + ".txt", completeStr);
                return completeStr;
            }
            
            return "";
        }
        
        public void importStrings(string commonScriptDir)
        {
            M.currentEvent = "Common Event " + id;
            M.currentEventNum = id;
            
            M.loadStringScript(commonScriptDir + "\\Common" + id.ToString("D4") + ".txt", id);
            if (!M.stringScriptImportCheck)
            {
                M.importDatabaseString(id, 0, "CommonEventName", ref commonName);
                if (commonName != "")
                    chunks.add(0x01);
                
                M.importPageCommands(ref commands, 0);
                M.checkIfImportingStringsExhausted(id);
                M.checkIfCommandValuesExhausted(id);
            }
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(commonName, M.S_UNTRANSLATED);
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(eventTrigger);
            if (chunks.wasNext(0x0c))
                M.writeLengthBool(usingSwitch);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(switchNum);
            
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(getCommandsLength());
            
            if (chunks.wasNext(0x16))
                M.writeCommandList(commands);
           
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public int getCommandsLength()
        {
            int length = 0;
            foreach (Command command in commands)
                length += command.getLength();
            return length;
        }
        
        public bool isBlank()
        {
            if (commonName != "" // 01
             || eventTrigger != 5 // 0b
             || usingSwitch // 0c
             // 0d irrelevant (usingSwitch is all that matters)
             || commands.Count > 1) // 15; the end command counts!
                return false;
            
            return true;
        }
    }
}
