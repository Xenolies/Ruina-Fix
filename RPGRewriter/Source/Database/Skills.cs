using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Skills : RPGByteData
    {
        List<Skill> skills;
        
        public Skills(FileStream f)
        {
            load(f);
        }
        public Skills()
        {
        }
        
        override public void load(FileStream f)
        {
            skills = M.readDatabaseList<Skill>(f, "Skills", "Skill", ref M.skillNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < skills.Count; i++)
                tabText.Write(skills[i].getString()
                    + (i < skills.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Skill skill in skills)
                skill.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Skill>(skills);
        }
    }
    
    class Skill : RPGDatabaseEntry
    {
        int id = 0;
        string skillName = ""; // 01
        string skillDescription = ""; // 02
        string useMessage = ""; // 03
        string useMessage2 = ""; // 04
        int failureMessage = 0; // 07
        int classification = 0; // 08
        int mpType = 0; // 09 (2003)
        int mpPercent = 1; // 0a (2003)
        int mpCost = 0; // 0b
        int range = 0; // 0c
        int switchNum = 1; // 0d
        int animation = 1; // 0e
        Audio sound; // 10
        bool onField = true; // 12
        bool inBattle = false; // 13
        bool conditionRemove = false; // 14 (2003)
        int hitRate = 0; // 15
        int mindRate = 3; // 16
        int variance = 4; // 17
        int basicEffect = 0; // 18
        int successRate = 100; // 19
        bool[] statUp; // 1f through 26
        int conditionAffectLength; // 29
        bool[] conditionAffect; // 2a
        int attributeLength; // 2b
        bool[] attributeAssigned; // 2c
        bool defenseUpDown = false; // 2d
        int battlerAnimation = 1; // 31 (2003)
        List<SkillAnimationData> battlerAnimationData; // 32 (2003)
        
        static string myClass = "Skill";
        Chunks chunks;
        
        static int skillNameLimit = 20;
        static int skillDescriptionLimit = 50;
        static int useMessageLimit = 30;
        static int useMessage2Limit = 50;
        
        static string[] types = { "Normal", "Teleport", "Escape", "Switch", "Subskill" };
        static string[] ranges = { "Single Enemy", "All Enemies", "User", "Single Ally", "Whole Party" };
        
        public Skill(FileStream f)
        {
            load(f);
        }
        public Skill()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                skillName = M.readStringDataName(f, id, ref M.skillNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    skillDescription = M.readString(f, M.S_TOTRANSLATE);
                if (chunks.next(0x03))
                    useMessage = M.readString(f, M.S_TOTRANSLATE);
                if (chunks.next(0x04))
                    useMessage2 = M.readString(f, M.S_TOTRANSLATE);
                
                if (chunks.next(0x07))
                    failureMessage = M.readLengthMultibyte(f);
                if (chunks.next(0x08))
                    classification = M.readLengthMultibyte(f);
                
                if (chunks.next(0x09))
                    mpType = M.readLengthMultibyte(f);
                if (chunks.next(0x0a))
                    mpPercent = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0b))
                    mpCost = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    range = M.readLengthMultibyte(f);
                if (chunks.next(0x0d))
                    switchNum = M.readLengthMultibyte(f);
                if (chunks.next(0x0e))
                    animation = M.readLengthMultibyte(f);
                
                if (chunks.next(0x10))
                    sound = new Audio(f);
                
                if (chunks.next(0x12))
                    onField = M.readLengthBool(f);
                if (chunks.next(0x13))
                    inBattle = M.readLengthBool(f);
                
                if (chunks.next(0x14))
                    conditionRemove = M.readLengthBool(f);
                
                if (chunks.next(0x15))
                    hitRate = M.readLengthMultibyte(f);
                if (chunks.next(0x16))
                    mindRate = M.readLengthMultibyte(f);
                if (chunks.next(0x17))
                    variance = M.readLengthMultibyte(f);
                if (chunks.next(0x18))
                    basicEffect = M.readLengthMultibyte(f);
                if (chunks.next(0x19))
                    successRate = M.readLengthMultibyte(f);
                
                statUp = new bool[8];
                
                if (chunks.next(0x1f)) // HP Up
                    statUp[0] = M.readLengthBool(f);
                if (chunks.next(0x20)) // MP Up
                    statUp[1] = M.readLengthBool(f);
                if (chunks.next(0x21)) // Attack Up
                    statUp[2] = M.readLengthBool(f);
                if (chunks.next(0x22)) // Defense Up
                    statUp[3] = M.readLengthBool(f);
                if (chunks.next(0x23)) // Mind Up
                    statUp[4] = M.readLengthBool(f);
                if (chunks.next(0x24)) // Agility Up
                    statUp[5] = M.readLengthBool(f);
                if (chunks.next(0x25)) // Absorb Up
                    statUp[6] = M.readLengthBool(f);
                if (chunks.next(0x26)) // Pierce Up
                    statUp[7] = M.readLengthBool(f);
                
                if (chunks.next(0x29))
                    conditionAffectLength = M.readLengthMultibyte(f);
                if (chunks.next(0x2a))
                    conditionAffect = M.readBoolArray(f);
                
                if (chunks.next(0x2b))
                    attributeLength = M.readLengthMultibyte(f);
                if (chunks.next(0x2c))
                    attributeAssigned = M.readBoolArray(f);
                
                if (chunks.next(0x2d))
                    defenseUpDown = M.readLengthBool(f);
                
                if (chunks.next(0x31))
                    battlerAnimation = M.readLengthMultibyte(f);
                if (chunks.next(0x32))
                    battlerAnimationData = M.readList<SkillAnimationData>(f);
                
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
                
                tabText.WriteLine(M.databaseExportString("Name", skillName, "[" + skillNameLimit + "]"));
                tabText.WriteLine(M.databaseExportString("Description", skillDescription, "[" + skillDescriptionLimit + "]"));
                tabText.WriteLine(M.databaseExportString("UseMessage1", useMessage, "[" + useMessageLimit + "]"));
                tabText.WriteLine(M.databaseExportString("UseMessage2", useMessage2, "[" + useMessage2Limit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter conditionListStr = new StringWriter(new StringBuilder());
            StringWriter attributeListStr = new StringWriter(new StringBuilder());
            StringWriter battlerAnimationStr = new StringWriter(new StringBuilder());;
            
            for (int condition = 0; condition < conditionAffect.Length; condition++)
                if (conditionAffect[condition])
                    conditionListStr.WriteLine(M.getDataCondition(condition + 1));
            if (conditionListStr.ToString() == "")
                conditionListStr.WriteLine("(None)");
            
            for (int attribute = 0; attribute < attributeLength; attribute++)
                if (attributeAssigned[attribute])
                    attributeListStr.WriteLine(M.getDataAttribute(attribute + 1));
            if (attributeListStr.ToString() == "")
                attributeListStr.WriteLine("(None)");
            
            if (battlerAnimationData != null)
                for (int hero = 0; hero < battlerAnimationData.Count; hero++)
                    battlerAnimationStr.WriteLine(battlerAnimationData[hero].getString());
            
            tabText.WriteLine("Skill #" + id);
            if (M.includeMessages)
            {
                tabText.WriteLine("Name: " + skillName);
                tabText.WriteLine("Description: " + skillDescription);
            }
            tabText.WriteLine("Type: " + (classification < types.Length? types[classification] : types[0])); // May go past bounds, seemingly treated as Normal
            tabText.WriteLine("MP Cost: " + (mpType == 0? mpCost.ToString() : (mpPercent + "%")));
            
            if (classification == 0 || classification == 4) // Normal or "3" (what)
            {
                bool targetParty = (range > 1);
                
                if (M.includeMessages)
                {
                    tabText.WriteLine("Use Message: " + useMessage);
                    tabText.WriteLine("Use Message 2: " + useMessage2);
                }
                tabText.WriteLine("Failure Message: Type " + failureMessage);
                
                tabText.WriteLine("Target Range: " + ranges[range]);
                tabText.WriteLine("Animation: " + M.getDataAnimation(animation));
                if (M.is2003)
                {
                    tabText.WriteLine("Battler Animation: " + (battlerAnimation > 0? M.getDataBattleAnimSet(battlerAnimation) : "(None)"));
                    tabText.Write(battlerAnimationStr);
                }
                
                tabText.WriteLine("Hit Rate: " + hitRate);
                tabText.WriteLine("Mind Rate: " + mindRate);
                tabText.WriteLine("Variance: " + variance);
                tabText.WriteLine("Basic Effect: " + basicEffect);
                tabText.WriteLine("Success Rate: " + successRate + "%");
                
                tabText.WriteLine("Stat " + (targetParty? "Up:" : "Down:")
                    + (statUp[0]? " HP" : "")
                    + (statUp[1]? " MP" : "")
                    + (statUp[2]? " Attack" : "")
                    + (statUp[3]? " Defense" : "")
                    + (statUp[4]? " Mind" : "")
                    + (statUp[5]? " Agility" : "")
                    + (statUp[6]? " Absorb" : "")
                    + (statUp[7]? " Pierce" : ""));
                
                string conditionTypeStr = conditionRemove? "Remove" : "Inflict";
                
                if (M.is2003)
                    tabText.WriteLine("Condition Type: " + conditionTypeStr);
                tabText.WriteLine(conditionTypeStr + "s Conditions:");
                tabText.Write(conditionListStr);
                
                tabText.WriteLine("Attribute Resistance " + (targetParty? "Up: " : "Down: ") + defenseUpDown);
                tabText.WriteLine("Attributes:");
                tabText.Write(attributeListStr);
            }
            else // Teleport, Escape, Switch
            {
                tabText.WriteLine("Sound Effect: " + sound.getString());
                
                if (classification == 3) // Switch
                {
                    tabText.WriteLine("Enables Switch: " + M.getDataSwitch(switchNum));
                    tabText.WriteLine("Usable On Field: " + onField);
                    tabText.WriteLine("Usable In Battle: " + inBattle);
                    
                    if (!M.is2003 && M.includeMessages)
                    {
                        tabText.WriteLine("Use Message: [User]" + useMessage);
                        tabText.WriteLine("Use Message 2: " + useMessage2);
                    }
                }
            }
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x0c;
            M.importDatabaseString(tabNum, id, "Name", ref skillName, skillNameLimit);
            M.importDatabaseString(tabNum, id, "Description", ref skillDescription, skillDescriptionLimit);
            M.importDatabaseString(tabNum, id, "UseMessage1", ref useMessage, M.ignoreLengthLimits >= 1? -1 : useMessageLimit);
            M.importDatabaseString(tabNum, id, "UseMessage2", ref useMessage2, M.ignoreLengthLimits >= 1? -1 : useMessage2Limit);
            
            if (skillName != "")
                chunks.add(0x01);
            if (skillDescription != "")
                chunks.add(0x02);
            if (useMessage != "")
                chunks.add(0x03);
            if (useMessage2 != "")
                chunks.add(0x04);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(skillName, M.S_TOTRANSLATE);
            
            if (chunks.wasNext(0x02))
                M.writeString(skillDescription, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x03))
                M.writeString(useMessage, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x04))
                M.writeString(useMessage2, M.S_TOTRANSLATE);
            
            if (chunks.wasNext(0x07))
                M.writeLengthMultibyte(failureMessage);
            if (chunks.wasNext(0x08))
                M.writeLengthMultibyte(classification);
            
            if (chunks.wasNext(0x09))
                M.writeLengthMultibyte(mpType);
            if (chunks.wasNext(0x0a))
                M.writeLengthMultibyte(mpPercent);
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(mpCost);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(range);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(switchNum);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(animation);
            
            if (chunks.wasNext(0x10))
                sound.write();
            
            if (chunks.wasNext(0x12))
                M.writeLengthBool(onField);
            if (chunks.wasNext(0x13))
                M.writeLengthBool(inBattle);
            
            if (chunks.wasNext(0x14))
                M.writeLengthBool(conditionRemove);
            
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(hitRate);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(mindRate);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(variance);
            if (chunks.wasNext(0x18))
                M.writeLengthMultibyte(basicEffect);
            if (chunks.wasNext(0x19))
                M.writeLengthMultibyte(successRate);
            
            if (chunks.wasNext(0x1f)) // HP Up
                M.writeLengthBool(statUp[0]);
            if (chunks.wasNext(0x20)) // MP Up
                M.writeLengthBool(statUp[1]);
            if (chunks.wasNext(0x21)) // Attack Up
                M.writeLengthBool(statUp[2]);
            if (chunks.wasNext(0x22)) // Defense Up
                M.writeLengthBool(statUp[3]);
            if (chunks.wasNext(0x23)) // Mind Up
                M.writeLengthBool(statUp[4]);
            if (chunks.wasNext(0x24)) // Agility Up
                M.writeLengthBool(statUp[5]);
            if (chunks.wasNext(0x25)) // Absorb Up
                M.writeLengthBool(statUp[6]);
            if (chunks.wasNext(0x26)) // Pierce Up
                M.writeLengthBool(statUp[7]);
            
            if (chunks.wasNext(0x29))
                M.writeLengthMultibyte(conditionAffectLength);
            if (chunks.wasNext(0x2a))
            {
                M.writeMultibyte(conditionAffect.Length);
                for (int condition = 0; condition < conditionAffect.Length; condition++)
                    M.writeByte(conditionAffect[condition]? 1 : 0);
            }
            
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(attributeLength);
            if (chunks.wasNext(0x2c))
            {
                M.writeMultibyte(attributeAssigned.Length);
                for (int attribute = 0; attribute < attributeAssigned.Length; attribute++)
                    M.writeByte(attributeAssigned[attribute]? 1 : 0);
            }
            
            if (chunks.wasNext(0x2d))
                M.writeLengthBool(defenseUpDown);
            
            if (chunks.wasNext(0x31))
                M.writeLengthMultibyte(battlerAnimation);
            if (chunks.wasNext(0x32))
                M.writeList<SkillAnimationData>(battlerAnimationData);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (skillName != "" // 01
             || skillDescription != "" // 02
             || useMessage != "" // 03
             || useMessage2 != "" // 04
             || failureMessage != 0 // 07
             || classification != 0 // 08
             || mpType != 0 // 09 (2003)
             || mpPercent != 1 // 0a (2003)
             || mpCost != 0 // 0b
             || range != 0 // 0c
             || switchNum != 1 // 0d
             || animation != 1 // 0e
             || !sound.isBlank() // 10
             || !onField // 12
             || inBattle // 13
             || conditionRemove // 14
             || hitRate != 0 // 15
             || mindRate != 3 // 16
             || variance != 4 // 17
             || basicEffect != 0 // 18
             || successRate != 100 // 19
             // 1f through 26 below
             // 29 irrelevant (length)
             // 2a below
             // 2b irrelevant (length)
             // 2c below
             || defenseUpDown // 2d
             || battlerAnimation != 1) // 31
             // 32 below
                return false;
            
            if (statUp != null)
                for (int i = 0; i < statUp.Length; i++) // 1f through 26
                    if (statUp[i])
                        return false;
            
            if (conditionAffect != null)
                for (int i = 0; i < conditionAffect.Length; i++) // 2a
                    if (conditionAffect[i])
                        return false;
            
            if (attributeAssigned != null)
                for (int i = 0; i < attributeAssigned.Length; i++) // 2c
                    if (attributeAssigned[i])
                        return false;
            
            if (battlerAnimationData != null)
                foreach (SkillAnimationData data in battlerAnimationData) // 32
                    if (!data.isBlank())
                        return false;
            
            return true;
        }
    }
    
    class SkillAnimationData : RPGByteData
    {
        int id = 0;
        int unknown = 0; // 02
        int type = 0; // 03
        int weaponAnimation = 0; // 04
        int movement = 0; // 05
        int afterimages = 0; // 06
        int attackCount = 0; // 07
        bool ranged = false; // 08
        int rangedAnimation = 0; // 09
        int rangedAnimationSpeed = 0; // 0c
        int battleAnimation = 1; // 0d
        int pose = 3; // 0e
        
        static string myClass = "SkillAnimationData";
        Chunks chunks;
        
        static string[] moves = { "Don't Move", "Step Forward", "Jump Forward", "Move" };
        
        public SkillAnimationData(FileStream f)
        {
            load(f);
        }
        public SkillAnimationData()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x02))
                unknown = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                type = M.readLengthMultibyte(f); // 0 Weapon, 1 Battle
            if (chunks.next(0x04))
                weaponAnimation = M.readLengthMultibyte(f);
            if (chunks.next(0x05))
                movement = M.readLengthMultibyte(f);
            if (chunks.next(0x06))
                afterimages = M.readLengthMultibyte(f);
            if (chunks.next(0x07))
                attackCount = M.readLengthMultibyte(f);
            if (chunks.next(0x08))
                ranged = M.readLengthBool(f);
            if (chunks.next(0x09))
                rangedAnimation = M.readLengthMultibyte(f);
            if (chunks.next(0x0c))
                rangedAnimationSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x0d))
                battleAnimation = M.readLengthMultibyte(f);
            if (chunks.next(0x0e))
                pose = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            return M.getDataHero(id) + " Animation: " + moves[movement] + ", " + M.getDataBattlerPose(pose)
                + (afterimages == 1 && movement != 0? " (With Afterimages)" : "");
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(unknown);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(type);
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(weaponAnimation);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(movement);
            if (chunks.wasNext(0x06))
                M.writeLengthMultibyte(afterimages);
            if (chunks.wasNext(0x07))
                M.writeLengthMultibyte(attackCount);
            if (chunks.wasNext(0x08))
                M.writeLengthBool(ranged);
            if (chunks.wasNext(0x09))
                M.writeLengthMultibyte(rangedAnimation);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(rangedAnimationSpeed);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(battleAnimation);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(pose);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (movement != 0 // 05
             || afterimages != 0 // 06
             || pose != 4) // 0e
                return false;
            
            return true;
        }
    }
}
