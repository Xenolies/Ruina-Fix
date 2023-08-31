using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Page : RPGByteData
    {
        int pageNum = 0;
        PageConditions conditions; // 02
        string charSet = ""; // 15
        int charIndex = 0; // 16
        int charDir = 2; // 17
        int charPattern = 1; // 18
        bool transparent = false; // 19
        int moveType = 1; // 1f
        int moveFrequency = 3; // 20
        int startTrigger = 0; // 21
        int layer = 0; // 22
        bool allowOverlap = false; // 23
        int animationType = 0; // 24
        int moveSpeed = 3; // 25
        PageCustomRoute customRoute; // 29
        List<Command> commands; // 33 length, 34 content
        
        static string myClass = "Page";
        Chunks chunks;
        
        public static string[] charDirs = { "Up", "Right", "Down", "Left", "Up-Right", "Down-Right", "Down-Left", "Up-Left" };
        public static string[] charPatterns = { "Left", "Middle", "Right" };
        public static string[] moveTypes = { "Stay Still", "Random Move", "Cycle Up-Down", "Cycle Left-Right",
                                             "Step Toward Hero", "Step Away Hero", "Custom Route" };
        public static string[] startTriggers = { "Push Key", "On Hero Touch", "On Touch (Event/Hero)",
                                                 "Auto Start", "Parallel Process" };
        public static string[] layers = { "Below Hero", "Same Level As Hero", "Over Hero" };
        public static string[] animationTypes = { "Normal w/o Stepping", "Normal w/ Stepping", "Fixed Dir. w/o Stepping",
                                                  "Fixed Dir. w/ Stepping", "Fixed Graphic", "Spin Right", "Fixed on Step Frame" };
        
        public Page(FileStream f)
        {
            load(f);
        }
        public Page()
        {
        }
        
        // Loads a single page within a map/troop event.
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            pageNum = M.readMultibyte(f);
            
            if (chunks.next(0x02))
            {
                M.readMultibyte(f); // Length
                conditions = new PageConditions(f);
            }
            
            if (chunks.next(0x15))
                charSet = M.readStringAndRewrite(f, M.M_CHARSET, M.S_FILENAME);
            if (chunks.next(0x16))
                charIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                charDir = M.readLengthMultibyte(f);
            if (chunks.next(0x18))
                charPattern = M.readLengthMultibyte(f);
            if (chunks.next(0x19))
                transparent = M.readLengthBool(f);
            
            if (chunks.next(0x1f))
                moveType = M.readLengthMultibyte(f);
            if (chunks.next(0x20))
                moveFrequency = M.readLengthMultibyte(f);
            if (chunks.next(0x21))
                startTrigger = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                layer = M.readLengthMultibyte(f);
            if (chunks.next(0x23))
                allowOverlap = M.readLengthBool(f);
            if (chunks.next(0x24))
                animationType = M.readLengthMultibyte(f);
            if (chunks.next(0x25))
                moveSpeed = M.readLengthMultibyte(f);
            
            if (chunks.next(0x29))
                customRoute = new PageCustomRoute(f);
            
            if (chunks.next(0x33))
                M.readLengthMultibyte(f); // Command section length
            
            M.completeMessage = null;
            M.messageFaceOn = false;
            M.messagePreceded = false;
            
            if (chunks.next(0x34))
                commands = M.readCommandList(f);
            
            M.byteCheck(f, 0x00);
            
            M.updateTrueChoices(commands);
        }
        
        // Returns page string.
        override public string getString()
        {
            M.currentPageNum = pageNum;
            M.currentPage = pageNum.ToString();
            
            StringWriter pageHeader = new StringWriter(new StringBuilder());
            StringWriter pageSettings = new StringWriter(new StringBuilder());
            StringWriter pageText = new StringWriter(new StringBuilder());
            
            if (!M.stringScriptExportMode)
                pageHeader.WriteLine("--- Page #" + pageNum + " ---");
            else
                pageHeader.WriteLine("-----Page" + pageNum + "-----");
            
            string moveRouteStr = customRoute != null && moveType == 6? Environment.NewLine + customRoute.getString() : "";
            
            if (M.includeActions || M.stringScriptExportMode)
            {
                if (conditions != null && (M.includeActions || M.getDetailSetting("PageConditions")))
                {
                    string conditionStr = conditions.getString();
                    if (conditionStr != "")
                        pageSettings.WriteLine(conditionStr);
                }
                
                if (M.includeActions || M.getDetailSetting("PageSettings"))
                {
                    pageSettings.WriteLine("Graphic: " + (charSet != ""? charSet : "ChipSet")
                                         + ", " + (charIndex + 1)
                                         + (charSet != ""? ", Face " + (charDirs[charDir]) + ", " + (charPatterns[charPattern]) : "")
                                         + (transparent? " (Transparent)" : ""));
                    
                    pageSettings.WriteLine("Move Type: " + moveTypes[moveType] + moveRouteStr);
                    pageSettings.WriteLine("Move Frequency: " + moveFrequency);
                    pageSettings.WriteLine("Event Trigger: " + startTriggers[startTrigger]);
                    pageSettings.WriteLine("Position: " + layers[layer]
                                         + (allowOverlap? " (Allow Event Overlap)" : ""));
                    if (charSet != "")
                        pageSettings.WriteLine("Animation Type: " + animationTypes[animationType]);
                    pageSettings.WriteLine("Move Speed: " + moveSpeed);
                    pageSettings.WriteLine();
                }
            }
            
            M.lastWroteAMessage = false;
            
            for (int i = 0; i < commands.Count; i++)
            {
                M.currentLine = "Line " + (i + 1);
                
                bool lastLineOfMessage = false;
                if (M.stringScriptExportMode && i + 1 < commands.Count)
                    lastLineOfMessage = !commands[i + 1].isMessageFollowOrExtraBox();
                
                string commandText = commands[i].getString(lastLineOfMessage);
                if (commandText != "")
                    pageText.WriteLine(commandText);
            }
            
            if (M.includeActions || pageText.ToString() != "") // In message-only mode, don't include header if content is blank.
                return pageHeader.ToString() + pageSettings + pageText;
            else
                return "";
        }
       
        // Replaces strings from importingStringArgs.
        public void importStrings()
        {
            M.importPageCommands(ref commands, pageNum);
        }
        
        // Writes page data.
        override protected void myWrite()
        {
            M.writeMultibyte(pageNum);
            
            if (chunks.wasNext(0x02))
            {
                M.writeMultibyte(conditions.getLength());
                conditions.write();
            }
            
            if (chunks.wasNext(0x15))
                M.writeString(charSet, M.S_FILENAME);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(charIndex);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(charDir);
            if (chunks.wasNext(0x18))
                M.writeLengthMultibyte(charPattern);
            if (chunks.wasNext(0x19))
                M.writeLengthBool(transparent);
            
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(moveType);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(moveFrequency);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(startTrigger);
            if (chunks.wasNext(0x22))
                M.writeLengthMultibyte(layer);
            if (chunks.wasNext(0x23))
                M.writeLengthBool(allowOverlap);
            if (chunks.wasNext(0x24))
                M.writeLengthMultibyte(animationType);
            if (chunks.wasNext(0x25))
                M.writeLengthMultibyte(moveSpeed);
            
            if (chunks.wasNext(0x29))
                customRoute.write();
            
            if (chunks.wasNext(0x33))
                M.writeLengthMultibyte(getCommandsLength());
            
            if (chunks.wasNext(0x34))
                M.writeCommandList(commands);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        // Returns the byte length of just the commands section.
        public int getCommandsLength()
        {
            int length = 0;
            foreach (Command command in commands)
                length += command.getLength();
            return length;
        }
    }
    
    class PageConditions : RPGByteData
    {
        bool switch1Condition = false; // 01 bits
        bool switch2Condition = false; // 01 bits
        bool variableCondition = false; // 01 bits
        bool itemCondition = false; // 01 bits
        bool heroCondition = false; // 01 bits
        bool timerCondition = false; // 01 bits
        bool timer2Condition = false; // 01 bits
        int switch1Num = 1; // 02
        int switch2Num = 1; // 03
        int variableNum = 1; // 04
        int variableValue = 0; // 05
        int itemValue = 1; // 06
        int heroNumber = 1; // 07
        int timerValue = 0; // 08
        int timer2Value = 0; // 09 (2003)
        int variableComparison = 1; // 0a (2003)
        
        static string myClass = "PageConditions";
        Chunks chunks;
        
        static string[] comparisonTypes = { "==", ">=", "<=", ">", "<", "!=" };
        
        public PageConditions(FileStream f)
        {
            load(f);
        }
        public PageConditions()
        {
        }
        
        // Loads the conditions section.
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
            {
                bool[] flags = M.readLengthFlags(f);
                switch1Condition = flags[0];
                switch2Condition = flags[1];
                variableCondition = flags[2];
                itemCondition = flags[3];
                heroCondition = flags[4];
                timerCondition = flags[5];
                timer2Condition = flags[6];
            }
            if (chunks.next(0x02))
                switch1Num = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                switch2Num = M.readLengthMultibyte(f);
            if (chunks.next(0x04))
                variableNum = M.readLengthMultibyte(f);
            if (chunks.next(0x05))
                variableValue = M.readLengthMultibyte(f);
            if (chunks.next(0x06))
                itemValue = M.readLengthMultibyte(f);
            if (chunks.next(0x07))
                heroNumber = M.readLengthMultibyte(f);
            if (chunks.next(0x08))
                timerValue = M.readLengthMultibyte(f);
            if (chunks.next(0x09))
                timer2Value = M.readLengthMultibyte(f);
            if (chunks.next(0x0a))
                variableComparison = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        // Returns conditions string.
        override public string getString()
        {
            StringWriter conditionStr = new StringWriter(new StringBuilder());
            
            if (switch1Condition)
                conditionStr.WriteLine("* If Switch " + M.getDataSwitch(switch1Num) + " is On");
            if (switch2Condition)
                conditionStr.WriteLine("* If Switch " + M.getDataSwitch(switch2Num) + " is On");
            if (variableCondition)
                conditionStr.WriteLine("* If Variable " + M.getDataVariable(variableNum) + " " + comparisonTypes[variableComparison] + " " + variableValue);
            if (itemCondition)
                conditionStr.WriteLine("* If " + M.getDataItem(itemValue) + " Owned");
            if (heroCondition)
                conditionStr.WriteLine("* If " + M.getDataHero(heroNumber) + " In Party");
            if (timerCondition)
                conditionStr.WriteLine("* If Timer Under " + (timerValue / 60) + " min " + (timerValue % 60) + " sec");
            if (timer2Condition)
                conditionStr.WriteLine("* If Timer 2 Under " + (timer2Value / 60) + " min " + (timer2Value % 60) + " sec");
            
            return conditionStr.ToString();
        }
        
        // Writes conditions section, to parent writer by default, and returns the byte size of that section.
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeLengthFlags(new bool[] { switch1Condition, switch2Condition, variableCondition,
                                                itemCondition, heroCondition, timerCondition, timer2Condition });
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(switch1Num);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(switch2Num);
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(variableNum);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(variableValue);
            if (chunks.wasNext(0x06))
                M.writeLengthMultibyte(itemValue);
            if (chunks.wasNext(0x07))
                M.writeLengthMultibyte(heroNumber);
            if (chunks.wasNext(0x08))
                M.writeLengthMultibyte(timerValue);
            if (chunks.wasNext(0x09))
                M.writeLengthMultibyte(timer2Value);
            if (chunks.wasNext(0x0a))
                M.writeLengthMultibyte(variableComparison);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
    
    class PageCustomRoute : RPGData
    {
        MoveRoute moveRoute; // 0b length, 0c content
        bool moveRepeat = true; // 15
        bool moveIgnore = false; // 16
        
        static string myClass = "PageCustomRoute";
        Chunks chunks;
        
        public PageCustomRoute(FileStream f)
        {
            load(f);
        }
        public PageCustomRoute()
        {
        }
        
        // Loads the custom route section.
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            M.readMultibyte(f); // Total move length
            
            if (chunks.next(0x0b))
                M.readLengthMultibyte(f);
            
            if (chunks.next(0x0c))
            {
                int moveLength = M.readMultibyte(f);
                if (moveLength > 0)
                    moveRoute = new MoveRoute(f, moveLength, "Custom");
            }
            
            if (chunks.next(0x15))
                moveRepeat = M.readLengthBool(f);
            if (chunks.next(0x16))
                moveIgnore = M.readLengthBool(f);
            
            M.byteCheck(f, 0x00);
        }
        
        // Returns move route string.
        public string getString()
        {
            if (moveRoute != null)
            {
                string settings = "";
                if (moveRepeat || moveIgnore)
                {
                    if (moveRepeat)
                        settings += (settings == ""? "[" : ", ") + "Repeat";
                    if (moveIgnore)
                        settings += (settings == ""? "[" : ", ") + "Ignore Impossible Moves";
                    settings += "]" + Environment.NewLine;
                }
                return settings + moveRoute.getString();
            }
            
            return "";
        }
        
        // Writes custom move route section. Not the fancy way, though, because just getting the byte array length wouldn't work.
        public void write()
        {
            int moveLength = moveRoute != null? moveRoute.getLength() : 0;
            
            int totalMoveLength = 0;
            if (chunks.used(0x0b))
            {
                totalMoveLength += 1; // 0x0b
                totalMoveLength += M.countMultibyte(M.countMultibyte(moveLength)); // Length length
                totalMoveLength += M.countMultibyte(moveLength); // Move length
            }
            if (chunks.used(0x0c))
            {
                totalMoveLength += 1; // 0x0c
                totalMoveLength += M.countLengthMultibyte(moveLength); // Move length
                totalMoveLength += moveLength; // Actual route contents
            }
            if (chunks.used(0x15))
                totalMoveLength += 3; // 0x15, length (0x01), bool (0x00/0x01)
            if (chunks.used(0x16))
                totalMoveLength += 3; // 0x16, length (0x01), bool (0x00/0x01)
            
            M.writeMultibyte(totalMoveLength);
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(moveLength);
            
            if (chunks.wasNext(0x0c))
            {
                M.writeMultibyte(moveLength);
                if (moveLength > 0)
                    moveRoute.write();
            }
            
            if (chunks.wasNext(0x15))
                M.writeLengthBool(moveRepeat);
            if (chunks.wasNext(0x16))
                M.writeLengthBool(moveIgnore);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
}
