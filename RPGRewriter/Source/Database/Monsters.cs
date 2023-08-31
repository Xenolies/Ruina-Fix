using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Monsters : RPGByteData
    {
        List<Monster> monsters;
        
        public Monsters(FileStream f)
        {
            load(f);
        }
        public Monsters()
        {
        }
        
        override public void load(FileStream f)
        {
            monsters = M.readDatabaseList<Monster>(f, "Monsters", "Monster", ref M.monsterNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < monsters.Count; i++)
                tabText.Write(monsters[i].getString()
                    + (i < monsters.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Monster monster in monsters)
                monster.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Monster>(monsters);
        }
    }
    
    class Monster : RPGDatabaseEntry
    {
        int id = 0;
        string monsterName = ""; // 01
        string monsterGraphic = ""; // 02
        int graphicHue = 0; // 03
        int hpStat = 10; // 04
        int mpStat = 10; // 05
        int atkStat = 10; // 06
        int defStat = 10; // 07
        int mindStat = 10; // 08
        int agiStat = 10; // 09
        bool transparent = false; // 0a
        int exp = 0; // 0b
        int money = 0; // 0c
        int itemDrop = 0; // 0d
        int dropRate = 100; // 0e
        bool useCrit = false; // 15
        int critRate = 30; // 16
        bool oftenMiss = false; // 1a
        bool inAir = false; // 1c
        int conditionEffectLength; // 1f
        int[] conditionEffect; // 20
        int attributeRankLength; // 21
        int[] attributeRank; // 22
        List<MonsterAction> actions; // 2a
        
        static string myClass = "Monster";
        Chunks chunks;
        
        static int monsterNameLimit = 20;
        
        static string[] effectRanks = { "A", "B", "C", "D", "E" };
        
        public Monster(FileStream f)
        {
            load(f);
        }
        public Monster()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                monsterName = M.readStringDataName(f, id, ref M.monsterNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    monsterGraphic = M.readStringAndRewrite(f, M.M_MONSTER, M.S_FILENAME);
                if (chunks.next(0x03))
                    graphicHue = M.readLengthMultibyte(f);
                if (chunks.next(0x04))
                    hpStat = M.readLengthMultibyte(f);
                if (chunks.next(0x05))
                    mpStat = M.readLengthMultibyte(f);
                if (chunks.next(0x06))
                    atkStat = M.readLengthMultibyte(f);
                if (chunks.next(0x07))
                    defStat = M.readLengthMultibyte(f);
                if (chunks.next(0x08))
                    mindStat = M.readLengthMultibyte(f);
                if (chunks.next(0x09))
                    agiStat = M.readLengthMultibyte(f);
                if (chunks.next(0x0a))
                    transparent = M.readLengthBool(f);
                if (chunks.next(0x0b))
                    exp = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    money = M.readLengthMultibyte(f);
                if (chunks.next(0x0d))
                    itemDrop = M.readLengthMultibyte(f);
                if (chunks.next(0x0e))
                    dropRate = M.readLengthMultibyte(f);
                
                if (chunks.next(0x15))
                    useCrit = M.readLengthBool(f);
                if (chunks.next(0x16))
                    critRate = M.readLengthMultibyte(f);
                
                if (chunks.next(0x1a))
                    oftenMiss = M.readLengthBool(f);
                
                if (chunks.next(0x1c))
                    inAir = M.readLengthBool(f);
                
                if (chunks.next(0x1f))
                    conditionEffectLength = M.readLengthMultibyte(f);
                if (chunks.next(0x20))
                    conditionEffect = M.readByteArray(f);
                
                if (chunks.next(0x21))
                    attributeRankLength = M.readLengthMultibyte(f);
                if (chunks.next(0x22))
                    attributeRank = M.readByteArray(f);
                
                if (chunks.next(0x2a))
                    actions = M.readList<MonsterAction>(f);
                
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
                
                tabText.WriteLine(M.databaseExportString("Name", monsterName, "[" + monsterNameLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter condEffect = new StringWriter(new StringBuilder());
            StringWriter attributeList = new StringWriter(new StringBuilder());
            StringWriter actionList = new StringWriter(new StringBuilder());
            
            for (int condition = 0; condition < conditionEffect.Length; condition++)
                condEffect.WriteLine(M.getDataCondition(condition + 1) + ": " + effectRanks[conditionEffect[condition]]);
            for (int attribute = 0; attribute < attributeRank.Length; attribute++)
                attributeList.WriteLine(M.getDataAttribute(attribute + 1) + ": " + effectRanks[attributeRank[attribute]]);
            
            foreach (MonsterAction action in actions)
                actionList.WriteLine(action.getString());
            
            tabText.WriteLine("Monster #" + id);
            if (M.includeMessages)
                tabText.WriteLine("Name: " + monsterName);
            tabText.WriteLine("Graphic: " + monsterGraphic);
            tabText.WriteLine("Hue: " + graphicHue);
            tabText.WriteLine("Transparent: " + transparent);
            tabText.WriteLine("In Air: " + inAir);
            tabText.WriteLine("HP: " + hpStat);
            tabText.WriteLine("MP: " + mpStat);
            tabText.WriteLine("Attack: " + atkStat);
            tabText.WriteLine("Defense: " + defStat);
            tabText.WriteLine("Mind: " + mindStat);
            tabText.WriteLine("Agility: " + agiStat);
            tabText.WriteLine("EXP: " + exp);
            tabText.WriteLine("Money: " + money);
            tabText.WriteLine("Drop Item: " + (itemDrop > 0? M.getDataItem(itemDrop) : "(None)"));
            tabText.WriteLine("Drop Rate: " + dropRate + "%");
            if (useCrit)
                tabText.WriteLine("Crit Rate: " + critRate + "%");
            tabText.WriteLine("Misses Often: " + oftenMiss);
            tabText.WriteLine("Condition Effect:");
            tabText.Write(condEffect);
            tabText.WriteLine("Attribute Ranks:");
            tabText.WriteLine(attributeList);
            tabText.WriteLine("Action Patterns:");
            tabText.Write(actionList);
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x0e;
            M.importDatabaseString(tabNum, id, "Name", ref monsterName, monsterNameLimit);
            
            if (monsterName != "")
                chunks.add(0x01);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(monsterName, M.S_TOTRANSLATE);
            
            if (chunks.wasNext(0x02))
                M.writeString(monsterGraphic, M.S_FILENAME);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(graphicHue);
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(hpStat);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(mpStat);
            if (chunks.wasNext(0x06))
                M.writeLengthMultibyte(atkStat);
            if (chunks.wasNext(0x07))
                M.writeLengthMultibyte(defStat);
            if (chunks.wasNext(0x08))
                M.writeLengthMultibyte(mindStat);
            if (chunks.wasNext(0x09))
                M.writeLengthMultibyte(agiStat);
            if (chunks.wasNext(0x0a))
                M.writeLengthBool(transparent);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(exp);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(money);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(itemDrop);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(dropRate);
                
            if (chunks.wasNext(0x15))
                M.writeLengthBool(useCrit);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(critRate);
                
            if (chunks.wasNext(0x1a))
                M.writeLengthBool(oftenMiss);
                
            if (chunks.wasNext(0x1c))
                M.writeLengthBool(inAir);
                
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(conditionEffectLength);
            if (chunks.wasNext(0x20))
                M.writeByteArray(conditionEffect);
            
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(attributeRankLength);
            if (chunks.wasNext(0x22))
                M.writeByteArray(attributeRank);
            
            if (chunks.wasNext(0x2a))
                M.writeList<MonsterAction>(actions);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (monsterName != "" // 01
             || monsterGraphic != "" // 02
             || graphicHue != 0 // 03
             || hpStat != 10 // 04
             || mpStat != 10 // 05
             || atkStat != 10 // 06
             || defStat != 10 // 07
             || mindStat != 10 // 08
             || agiStat != 10 // 09
             || transparent // 0a
             || exp != 0 // 0b
             || money != 0 // 0c
             || itemDrop != 0 // 0d
             || dropRate != 100 // 0e
             || useCrit // 15
             || critRate != 30 // 16
             || oftenMiss // 1a
             || inAir // 1c
             || actions.Count > 0) // 2a
                return false;
            
            for (int i = 0; i < conditionEffect.Length; i++)
                if (conditionEffect[i] != 1)
                    return false;
            for (int i = 0; i < attributeRank.Length; i++)
                if (attributeRank[i] != 2)
                    return false;
            
            return true;
        }
    }
    
    class MonsterAction : RPGByteData
    {
        int id = 0;
        int subjectType = 0; // 01
        int behavior = 1; // 02
        int skill = 1; // 03
        int transformIntoEnemy = 1; // 04
        int conditionType = 0; // 05
        int conditionValue1 = 0; // 06
        int conditionValue2 = 100; // 07
        int conditionSwitch = 1; // 08
        bool turnSwitchOn = false; // 09
        int onSwitch = 1; // 0a
        bool turnSwitchOff = false; // 0b
        int offSwitch = 1; // 0c
        int priority = 50; // 0d
        
        static string myClass = "MonsterAction";
        Chunks chunks;
        
        static string[] behaviors = { "Normal Hit", "Critical Hit", "Defend", "Watch",
                                      "Store Power", "Self-Destruct", "Flee", "Nothing" };
        
        public MonsterAction(FileStream f)
        {
            load(f);
        }
        public MonsterAction()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                subjectType = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
                behavior = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                skill = M.readLengthMultibyte(f);
            if (chunks.next(0x04))
                transformIntoEnemy = M.readLengthMultibyte(f);
            if (chunks.next(0x05))
                conditionType = M.readLengthMultibyte(f);
            if (chunks.next(0x06))
                conditionValue1 = M.readLengthMultibyte(f);
            if (chunks.next(0x07))
                conditionValue2 = M.readLengthMultibyte(f);
            if (chunks.next(0x08))
                conditionSwitch = M.readLengthMultibyte(f);
            if (chunks.next(0x09))
                turnSwitchOn = M.readLengthBool(f);
            if (chunks.next(0x0a))
                onSwitch = M.readLengthMultibyte(f);
            if (chunks.next(0x0b))
                turnSwitchOff = M.readLengthBool(f);
            if (chunks.next(0x0c))
                offSwitch = M.readLengthMultibyte(f);
            if (chunks.next(0x0d))
                priority = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            string subject = "", condition = "";
            
            if (subjectType == 0) // Behavior
                subject = behaviors[behavior];
            else if (subjectType == 1) // Skills
                subject = M.getDataSkill(skill);
            else if (subjectType == 2) // Transform
                subject = "Transform " + M.getDataMonster(transformIntoEnemy);
            
            if (turnSwitchOn)
                subject += " (" + M.getDataSwitch(onSwitch) + " On)";
            if (turnSwitchOff)
                subject += " (" + M.getDataSwitch(offSwitch) + " Off)";
            
            if (conditionType == 0) // Always
                condition = "Always";
            else if (conditionType == 1) // Switch
                condition = "Switch " + M.getDataSwitch(conditionSwitch) + " On";
            else if (conditionType == 2) // Turn No.
                condition = "Turn No. " + conditionValue1 + "X + " + conditionValue2;
            else if (conditionType == 3) // Monster Quantity
                condition = "Monster Quantity " + conditionValue1 + " to " + conditionValue2;
            else if (conditionType == 4) // Own HP
                condition = "Own HP " + conditionValue1 + "% to " + conditionValue2 + "%";
            else if (conditionType == 5) // Own MP
                condition = "Own MP " + conditionValue1 + "% to " + conditionValue2 + "%";
            else if (conditionType == 6) // Average Level
                condition = "Party Average Level " + conditionValue1 + " to " + conditionValue2;
            else if (conditionType == 7) // Exhaustion
                condition = "Party Exhaustion " + conditionValue1 + "% to " + conditionValue2 + "%";
            
            return subject + " / " + condition + " / " + priority;
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(subjectType);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(behavior);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(skill);
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(transformIntoEnemy);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(conditionType);
            if (chunks.wasNext(0x06))
                M.writeLengthMultibyte(conditionValue1);
            if (chunks.wasNext(0x07))
                M.writeLengthMultibyte(conditionValue2);
            if (chunks.wasNext(0x08))
                M.writeLengthMultibyte(conditionSwitch);
            if (chunks.wasNext(0x09))
                M.writeLengthBool(turnSwitchOn);
            if (chunks.wasNext(0x0a))
                M.writeLengthMultibyte(onSwitch);
            if (chunks.wasNext(0x0b))
                M.writeLengthBool(turnSwitchOff);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(offSwitch);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(priority);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
}
