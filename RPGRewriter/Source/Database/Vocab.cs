using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Vocab : RPGByteData
    {
        string[] str;
        
        static string myClass = "Vocab";
        Chunks chunks;
        
        public static string[] form;
        public static string[] refName;
        public static int[] lengthLimit;
        
        public Vocab(FileStream f)
        {
            load(f);
        }
        public Vocab()
        {
        }
        
        // Initializes static variables for strings and length limits.
        public static void initStatic()
        {
            form = new string[255];
            refName = new string[255];
            lengthLimit = new int[255];
            
            for (int i = 0; i < 255; i++)
                lengthLimit[i] = 50;
            
            // Battle Messages 1
            
            form[0x01] = "Start Battle: [Enemy]{0}";
            form[0x26] = "Start Battle: {0}";
            form[0x02] = "Preemptive Attack: {0}";
            form[0x27] = "Miss: {0}";
            form[0x03] = "Escape Success: {0}";
            form[0x04] = "Escape Failure: {0}";
            form[0x05] = "Victory: {0}";
            form[0x06] = "Defeat: {0}";
            form[0x07] = "Get EXP: [Value]{0}";
            form[0x08] = "Get Money: {0}[Value][Unit]{1}";
            form[0x09] = ""; // Get Money Part 2
            form[0x0a] = "Get Item: [Item]{0}";
            
            refName[0x01] = "StartBattle";
            refName[0x26] = "StartBattle2003";
            refName[0x02] = "PreemptiveAttack";
            refName[0x27] = "Miss";
            refName[0x03] = "EscapeSuccess";
            refName[0x04] = "EscapeFailure";
            refName[0x05] = "Victory";
            refName[0x06] = "Defeat";
            refName[0x07] = "GetEXP";
            refName[0x08] = "GetMoneyPart1";
            refName[0x09] = "GetMoneyPart2";
            refName[0x0a] = "GetItem";
            
            lengthLimit[0x01] = 30;
            lengthLimit[0x27] = 8;
            lengthLimit[0x07] = 38;
            lengthLimit[0x08] = 10;
            lengthLimit[0x09] = 24;
            lengthLimit[0x0a] = 30;
            
            // Battle Messages 2
            
            form[0x0b] = "Normal Attack: [Character]{0}";
            form[0x0c] = "Critical Hit: {0}";
            form[0x0d] = "Enemy Critical: {0}";
            form[0x0e] = "Defend: [Character]{0}";
            form[0x0f] = "Enemy Observe: [Enemy]{0}";
            form[0x10] = "Enemy Build Strength: [Enemy]{0}";
            form[0x11] = "Enemy Burst: [Enemy]{0}";
            form[0x12] = "Enemy Escape: [Character]{0}";
            form[0x13] = "Enemy Transform: [Enemy]{0}";
            
            refName[0x0b] = "NormalAttack";
            refName[0x0c] = "CriticalHit";
            refName[0x0d] = "EnemyCritical";
            refName[0x0e] = "DefendMessage";
            refName[0x0f] = "EnemyObserve";
            refName[0x10] = "EnemyBuildStrength";
            refName[0x11] = "EnemyBurst";
            refName[0x12] = "EnemyEscape";
            refName[0x13] = "EnemyTransform";
            
            lengthLimit[0x0b] = 30;
            lengthLimit[0x0e] = 30;
            lengthLimit[0x0f] = 30;
            lengthLimit[0x10] = 30;
            lengthLimit[0x11] = 30;
            lengthLimit[0x12] = 30;
            lengthLimit[0x13] = 30;
            
            // Battle Messages 3
            
            form[0x14] = "Damage Enemy: [Enemy][ni][Damage]{0}";
            form[0x15] = "Miss Enemy: [Character]{0}";
            form[0x16] = "Damage Ally: [Character][wa][Damage]{0}";
            form[0x17] = "Miss Ally: [Enemy]{0}";
            form[0x18] = "Skill Failure A: [Character]{0}";
            form[0x19] = "Skill Failure B: [Character]{0}";
            form[0x1a] = "Skill Failure C: [Character]{0}";
            form[0x1b] = "Avoid Physical: [Character]{0}";
            form[0x1c] = "Use Item: [Character][Item]{0}";
            
            refName[0x14] = "DamageEnemy";
            refName[0x15] = "MissEnemy";
            refName[0x16] = "DamageAlly";
            refName[0x17] = "MissAlly";
            refName[0x18] = "SkillFailureA";
            refName[0x19] = "SkillFailureB";
            refName[0x1a] = "SkillFailureC";
            refName[0x1b] = "AvoidPhysical";
            refName[0x1c] = "UseItem";
            
            lengthLimit[0x14] = 24;
            lengthLimit[0x15] = 32;
            lengthLimit[0x16] = 24;
            lengthLimit[0x17] = 32;
            lengthLimit[0x18] = 32;
            lengthLimit[0x19] = 32;
            lengthLimit[0x1a] = 32;
            lengthLimit[0x1b] = 32;
            lengthLimit[0x1c] = 14;
            
            // Battle Messages 4
            
            form[0x1d] = "Recover HP/MP: [Character][no][Stat][ga][Value]{0}";
            form[0x1e] = "Stat Up: [Character][no][Stat][ga][Value]{0}";
            form[0x1f] = "Stat Down: [Character][no][Stat][ga][Value]{0}";
            form[0x20] = "Stat Absorb: [Enemy][no][Stat][wo][Value]{0}";
            form[0x21] = "Stat Sucked: [Character][no][Stat][wo][Value]{0}";
            form[0x22] = "Defense Up: [Character][no][Stat]{0}";
            form[0x23] = "Defense Down: [Character][no][Stat]{0}";
            form[0x24] = "Level Up: [Character][wa][Level][Value]{0}";
            form[0x25] = "Learn Skill: [Skill]{0}";
            
            refName[0x1d] = "RecoverHPMP";
            refName[0x1e] = "StatUp";
            refName[0x1f] = "StatDown";
            refName[0x20] = "StatAbsorb";
            refName[0x21] = "StatSucked";
            refName[0x22] = "DefenseUp";
            refName[0x23] = "DefenseDown";
            refName[0x24] = "LevelUp";
            refName[0x25] = "LearnSkill";
            
            lengthLimit[0x1d] = 12;
            lengthLimit[0x1e] = 12;
            lengthLimit[0x1f] = 12;
            lengthLimit[0x20] = 12;
            lengthLimit[0x21] = 12;
            lengthLimit[0x22] = 20;
            lengthLimit[0x23] = 20;
            lengthLimit[0x24] = 16;
            lengthLimit[0x25] = 26;
            
            // Shop Messages A
            
            form[0x29] = "Choice Screen (First Time): {0}";
            form[0x2a] = "Choice Screen (Repeat): {0}";
            form[0x2b] = "Buy: {0}";
            form[0x2c] = "Sell: {0}";
            form[0x2d] = "Leave: {0}";
            form[0x2e] = "Buy Screen: {0}";
            form[0x2f] = "How Many To Buy: {0}";
            form[0x30] = "Purchase Made: {0}";
            form[0x31] = "Sell Screen: {0}";
            form[0x32] = "How Many To Sell: {0}";
            form[0x33] = "Sale Complete: {0}";
            
            refName[0x29] = "ShopA:ChoiceScreen";
            refName[0x2a] = "ShopA:ChoiceScreenAgain";
            refName[0x2b] = "ShopA:Buy";
            refName[0x2c] = "ShopA:Sell";
            refName[0x2d] = "ShopA:Leave";
            refName[0x2e] = "ShopA:BuyScreen";
            refName[0x2f] = "ShopA:HowManyToBuy";
            refName[0x30] = "ShopA:PurchaseMade";
            refName[0x31] = "ShopA:SellScreen";
            refName[0x32] = "ShopA:HowManyToSell";
            refName[0x33] = "ShopA:SaleComplete";
            
            lengthLimit[0x2b] = 12;
            lengthLimit[0x2c] = 12;
            lengthLimit[0x2d] = 12;
            
            // Shop Messages B
            
            form[0x36] = "Choice Screen (First Time): {0}";
            form[0x37] = "Choice Screen (Repeat): {0}";
            form[0x38] = "Buy: {0}";
            form[0x39] = "Sell: {0}";
            form[0x3a] = "Leave: {0}";
            form[0x3b] = "Buy Screen: {0}";
            form[0x3c] = "How Many To Buy: {0}";
            form[0x3d] = "Purchase Made: {0}";
            form[0x3e] = "Sell Screen: {0}";
            form[0x3f] = "How Many To Sell: {0}";
            form[0x40] = "Sale Complete: {0}";
            
            refName[0x36] = "ShopB:ChoiceScreen";
            refName[0x37] = "ShopB:ChoiceScreenAgain";
            refName[0x38] = "ShopB:Buy";
            refName[0x39] = "ShopB:Sell";
            refName[0x3a] = "ShopB:Leave";
            refName[0x3b] = "ShopB:BuyScreen";
            refName[0x3c] = "ShopB:HowManyToBuy";
            refName[0x3d] = "ShopB:PurchaseMade";
            refName[0x3e] = "ShopB:SellScreen";
            refName[0x3f] = "ShopB:HowManyToSell";
            refName[0x40] = "ShopB:SaleComplete";
            
            lengthLimit[0x38] = 12;
            lengthLimit[0x39] = 12;
            lengthLimit[0x3a] = 12;
            
            // Shop Messages C
            
            form[0x43] = "Choice Screen (First Time): {0}";
            form[0x44] = "Choice Screen (Repeat): {0}";
            form[0x45] = "Buy: {0}";
            form[0x46] = "Sell: {0}";
            form[0x47] = "Leave: {0}";
            form[0x48] = "Buy Screen: {0}";
            form[0x49] = "How Many To Buy: {0}";
            form[0x4a] = "Purchase Made: {0}";
            form[0x4b] = "Sell Screen: {0}";
            form[0x4c] = "How Many To Sell: {0}";
            form[0x4d] = "Sale Complete: {0}";
            
            refName[0x43] = "ShopC:ChoiceScreen";
            refName[0x44] = "ShopC:ChoiceScreenAgain";
            refName[0x45] = "ShopC:Buy";
            refName[0x46] = "ShopC:Sell";
            refName[0x47] = "ShopC:Leave";
            refName[0x48] = "ShopC:BuyScreen";
            refName[0x49] = "ShopC:HowManyToBuy";
            refName[0x4a] = "ShopC:PurchaseMade";
            refName[0x4b] = "ShopC:SellScreen";
            refName[0x4c] = "ShopC:HowManyToSell";
            refName[0x4d] = "ShopC:SaleComplete";
            
            lengthLimit[0x45] = 12;
            lengthLimit[0x46] = 12;
            lengthLimit[0x47] = 12;
            
            // Inn Messages A
            
            form[0x50] = "Inn Intro: {0}[Value][Unit]{1}";
            form[0x51] = ""; // Inn Intro Part 2
            form[0x52] = "Inn Intro 2: {0}";
            form[0x53] = "Lodge: {0}";
            form[0x54] = "Don't Lodge: {0}";
            
            refName[0x50] = "InnA:IntroLine1a";
            refName[0x51] = "InnA:IntroLine1b";
            refName[0x52] = "InnA:IntroLine2";
            refName[0x53] = "InnA:Lodge";
            refName[0x54] = "InnA:DoNotLodge";
            
            lengthLimit[0x50] = 16;
            lengthLimit[0x51] = 20;
            lengthLimit[0x53] = 12;
            lengthLimit[0x54] = 12;
            
            // Inn Messages B
            
            form[0x55] = "Inn Intro: {0}[Value][Unit]{1}";
            form[0x56] = ""; // Inn Intro Part 2
            form[0x57] = "Inn Intro 2: {0}";
            form[0x58] = "Lodge: {0}";
            form[0x59] = "Don't Lodge: {0}";
            
            refName[0x55] = "InnB:IntroLine1a";
            refName[0x56] = "InnB:IntroLine1b";
            refName[0x57] = "InnB:IntroLine2";
            refName[0x58] = "InnB:Lodge";
            refName[0x59] = "InnB:DoNotLodge";
            
            lengthLimit[0x55] = 16;
            lengthLimit[0x56] = 20;
            lengthLimit[0x58] = 12;
            lengthLimit[0x59] = 12;
            
            // Shop Parameters
            
            form[0x5c] = "Owned Quantity: {0}";
            form[0x5d] = "Equipped Quantity: {0}";
            form[0x5f] = "Currency Unit: {0}";
            
            refName[0x5c] = "OwnedQuantity";
            refName[0x5d] = "EquippedQuantity";
            refName[0x5f] = "CurrencyUnit";
            
            lengthLimit[0x5c] = 16;
            lengthLimit[0x5d] = 16;
            lengthLimit[0x5f] = 6;
            
            // Commands
            
            form[0x65] = "Fight: {0}";
            form[0x66] = "Auto: {0}";
            form[0x67] = "Escape: {0}";
            form[0x68] = "Attack: {0}";
            form[0x69] = "Defend: {0}";
            form[0x6a] = "Item: {0}";
            form[0x6b] = "Skill: {0}";
            form[0x6c] = "Equip: {0}";
            form[0x6e] = "Save: {0}";
            form[0x70] = "End Game: {0}";
            form[0x72] = "New Game: {0}";
            form[0x73] = "Continue: {0}";
            form[0x75] = "Quit: {0}";
            form[0x76] = "Status: {0}";
            form[0x77] = "Row: {0}";
            form[0x78] = "Order: {0}";
            form[0x79] = "Wait On: {0}";
            form[0x7a] = "Wait Off: {0}";
            
            refName[0x65] = "Fight";
            refName[0x66] = "Auto";
            refName[0x67] = "Escape";
            refName[0x68] = "Attack";
            refName[0x69] = "Defend";
            refName[0x6a] = "Item";
            refName[0x6b] = "Skill";
            refName[0x6c] = "Equip";
            refName[0x6e] = "Save";
            refName[0x70] = "EndGame";
            refName[0x72] = "NewGame";
            refName[0x73] = "Continue";
            refName[0x75] = "Quit";
            refName[0x76] = "Status";
            refName[0x77] = "Row";
            refName[0x78] = "Order";
            refName[0x79] = "WaitOn";
            refName[0x7a] = "WaitOff";
            
            lengthLimit[0x65] = 10;
            lengthLimit[0x66] = 10;
            lengthLimit[0x67] = 10;
            lengthLimit[0x68] = 10;
            lengthLimit[0x69] = 10;
            lengthLimit[0x6a] = 10;
            lengthLimit[0x6b] = 10;
            lengthLimit[0x6c] = 12;
            lengthLimit[0x6e] = 12;
            lengthLimit[0x70] = 12;
            lengthLimit[0x72] = 16;
            lengthLimit[0x73] = 16;
            lengthLimit[0x75] = 16;
            lengthLimit[0x76] = 12;
            lengthLimit[0x77] = 10;
            lengthLimit[0x78] = 12;
            lengthLimit[0x79] = 12;
            lengthLimit[0x7a] = 12;
            
            // Stats & Equipment
            
            form[0x7b] = "Level: {0}";
            form[0x7c] = "HP: {0}";
            form[0x7d] = "MP: {0}";
            form[0x7e] = "Normal State: {0}";
            form[0x7f] = "EXP (Short): {0}";
            form[0x80] = "Level (Short): {0}";
            form[0x81] = "HP (Short): {0}";
            form[0x82] = "MP (Short): {0}";
            form[0x83] = "MP Cost: {0}";
            form[0x84] = "Attack: {0}";
            form[0x85] = "Defense: {0}";
            form[0x86] = "Mind: {0}";
            form[0x87] = "Agility: {0}";
            form[0x88] = "Arms: {0}";
            form[0x89] = "Shield: {0}";
            form[0x8a] = "Armor: {0}";
            form[0x8b] = "Helmet: {0}";
            form[0x8c] = "Other: {0}";
            
            refName[0x7b] = "Level";
            refName[0x7c] = "HP";
            refName[0x7d] = "MP";
            refName[0x7e] = "NormalState";
            refName[0x7f] = "EXPShort";
            refName[0x80] = "LevelShort";
            refName[0x81] = "HPShort";
            refName[0x82] = "MPShort";
            refName[0x83] = "MPCost";
            refName[0x84] = "Offense";
            refName[0x85] = "Defense";
            refName[0x86] = "Mind";
            refName[0x87] = "Agility";
            refName[0x88] = "Arms";
            refName[0x89] = "Shield";
            refName[0x8a] = "Armor";
            refName[0x8b] = "Helmet";
            refName[0x8c] = "Other";
            
            lengthLimit[0x7b] = 10;
            lengthLimit[0x7c] = 10;
            lengthLimit[0x7d] = 10;
            lengthLimit[0x7e] = 10;
            lengthLimit[0x7f] = 2;
            lengthLimit[0x80] = 2;
            lengthLimit[0x81] = 2;
            lengthLimit[0x82] = 2;
            lengthLimit[0x83] = 10;
            lengthLimit[0x84] = 10;
            lengthLimit[0x85] = 10;
            lengthLimit[0x86] = 10;
            lengthLimit[0x87] = 10;
            lengthLimit[0x88] = 10;
            lengthLimit[0x89] = 10;
            lengthLimit[0x8a] = 10;
            lengthLimit[0x8b] = 10;
            lengthLimit[0x8c] = 10;
            
            // Save/Load/End
            
            form[0x92] = "Choose Save File: {0}";
            form[0x93] = "Choose Load File: {0}";
            form[0x94] = "File: {0}";
            form[0x97] = "Quit Confirm: {0}";
            form[0x98] = "Yes: {0}";
            form[0x99] = "No: {0}";
            
            refName[0x92] = "ChooseSaveFile";
            refName[0x93] = "ChooseLoadFile";
            refName[0x94] = "File";
            refName[0x97] = "QuitConfirm";
            refName[0x98] = "QuitYes";
            refName[0x99] = "QuitNo";
            
            lengthLimit[0x94] = 10;
            lengthLimit[0x98] = 10;
            lengthLimit[0x99] = 10;
        }
        
        override public void load(FileStream f)
        {
            M.currentEvent = "Vocab";
            M.currentPage = "";
            M.currentLine = "";
            M.currentEventNum = 0;
            M.currentPageNum = 0;
            
            chunks = new Chunks(f, myClass);
            
            if (!M.readingDataNames)
            {
                str = new string[255];
                
                for (byte i = 0x01; i <= 0x99; i++)
                    if (chunks.next(i))
                        str[i] = M.readString(f, M.S_TOTRANSLATE);
                
                M.byteCheck(f, 0x00);
            }
            else // Skip everything.
            {
                M.skipChunkRange(f, 0x01, 0x99);
                M.byteCheck(f, 0x00);
            }
        }
        
        // Returns data string.
        override public string getString()
        {
            if (!M.includeMessages)
                return "";
            
            if (form == null)
                initStatic();
            
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            tabText.WriteLine("*** Battle Messages 1 ***");
            
            if (chunks.used(0x01)) // 2000 Battle Start (for each enemy)
                writeSingleString(tabText, 0x01);
            else if (chunks.used(0x26)) // 2003 Battle Start (no enemy name)
                writeSingleString(tabText, 0x26);
            
            writeSingleString(tabText, 0x02); // Preemptive Attack:
            
            if (chunks.used(0x27)) // 2003-only miss message
                writeSingleString(tabText, 0x27);
            
            for (byte i = 0x03; i <= 0x0a; i++)
                writeSingleString(tabText, i);
            
            // In 2003, all other battle messages except Level Up and Learn Skill are absent (but can still be there in the file)
            if (!chunks.used(0x26)) // No Miss message, so it's 2000
            {
                tabText.WriteLine();
                tabText.WriteLine("*** Battle Messages 2 ***");
                
                for (byte i = 0x0b; i <= 0x13; i++)
                    writeSingleString(tabText, i);
                
                tabText.WriteLine();
                tabText.WriteLine("*** Battle Messages 3 ***");
                
                for (byte i = 0x14; i <= 0x1c; i++)
                    writeSingleString(tabText, i);
                
                tabText.WriteLine();
                tabText.WriteLine("*** Battle Messages 4 ***");
                
                for (byte i = 0x1d; i <= 0x25; i++)
                    writeSingleString(tabText, i);
            }
            else // 2003
            {
                tabText.WriteLine();
                tabText.WriteLine("*** Level-Up Messages ***");
                
                for (byte i = 0x24; i <= 0x25; i++)
                    writeSingleString(tabText, i);
            }
            
            tabText.WriteLine();
            tabText.WriteLine("*** Shop Messages A ***");
            
            for (byte i = 0x29; i <= 0x33; i++)
                writeSingleString(tabText, i);
            
            tabText.WriteLine();
            tabText.WriteLine("*** Shop Messages B ***");
            
            for (byte i = 0x36; i <= 0x40; i++)
                writeSingleString(tabText, i);
            
            tabText.WriteLine();
            tabText.WriteLine("*** Shop Messages C ***");
            
            for (byte i = 0x43; i <= 0x4d; i++)
                writeSingleString(tabText, i);
            
            tabText.WriteLine();
            tabText.WriteLine("*** Inn Messages A ***");
            
            for (byte i = 0x50; i <= 0x54; i++)
                writeSingleString(tabText, i);
            
            tabText.WriteLine();
            tabText.WriteLine("*** Inn Messages B ***");
            
            for (byte i = 0x55; i <= 0x59; i++)
                writeSingleString(tabText, i);
            
            tabText.WriteLine();
            tabText.WriteLine("*** Shop Parameters ***");
            
            for (byte i = 0x5c; i <= 0x5f; i++)
                writeSingleString(tabText, i);
            
            tabText.WriteLine();
            tabText.WriteLine("*** Commands ***");
            
            for (byte i = 0x65; i <= 0x75; i++)
                if (!chunks.used(0x76) || (i != 0x68 && i != 0x69)) // Attack and Defend are gone in 2003
                    writeSingleString(tabText, i);
            
            if (chunks.used(0x76)) // 2003
            {
                for (byte i = 0x76; i <= 0x7a; i++)
                    writeSingleString(tabText, i);
            }
            
            tabText.WriteLine();
            tabText.WriteLine("*** Stats & Equipment ***");
            
            for (byte i = 0x7b; i <= 0x8c; i++)
                writeSingleString(tabText, i);
            
            tabText.WriteLine();
            tabText.WriteLine("*** Save/Load/End ***");
            
            for (byte i = 0x92; i <= 0x99; i++)
                writeSingleString(tabText, i);
            
            return tabText.ToString();
        }
        
        // Returns the string to write for this vocab entry.
        void writeSingleString(StringWriter writer, byte index)
        {
            if (M.stringScriptExportMode)
            {
                if (refName[index] != null)
                {
                    M.setCurrentDatabaseEntry(myClass, index);
                    writer.WriteLine(M.databaseExportString(refName[index], str[index], "[" + lengthLimit[index] + "]"));
                }
            }
            else if (chunks.used(index) && form[index] != null && form[index] != "")
            {
                string write = string.Format(form[index], str[index], str[index + 1]);
                writer.WriteLine(write);
            }
        }
        
        // Imports strings from string script if found.
        public void importStrings()
        {
            if (refName == null)
                initStatic();
            
            int tabNum = 0x15;
            
            for (byte i = 0; i < refName.Length; i++)
            {
                if (refName[i] != null && str[i] != null)
                {
                    M.importDatabaseString(tabNum, 0, refName[i], ref str[i], M.ignoreLengthLimits >= 1? -1 : lengthLimit[i]);
                    if (str[i] != "")
                        chunks.add(i);
                }
            }
        }
        
        // Writes data.
        override protected void myWrite()
        {
            for (byte i = 0x01; i <= 0x99; i++)
                if (chunks.wasNext(i))
                    M.writeString(str[i], M.S_TOTRANSLATE);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
}
