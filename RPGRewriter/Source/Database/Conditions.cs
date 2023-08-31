using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Conditions : RPGByteData
    {
        List<Condition> conditions;
        
        public Conditions(FileStream f)
        {
            load(f);
        }
        public Conditions()
        {
        }
        
        override public void load(FileStream f)
        {
            conditions = M.readDatabaseList<Condition>(f, "Conditions", "Condition", ref M.conditionNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < conditions.Count; i++)
                tabText.Write(conditions[i].getString()
                    + (i < conditions.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Condition condition in conditions)
                condition.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Condition>(conditions);
        }
    }
    
    class Condition : RPGDatabaseEntry
    {
        int id = 0;
        string conditionName = ""; // 01
        int classification = 0; // 02
        int color = 6; // 03
        int priority = 50; // 04
        int limitation = 0; // 05
        int rateA = 100; // 0b
        int rateB = 80; // 0c
        int rateC = 60; // 0d
        int rateD = 30; // 0e
        int rateE = 0; // 0f
        int naturalHealTurns = 0; // 15
        int naturalHealChance = 0; // 16
        int hitHealChance = 0; // 17
        int affectType = 0; // 1e (2003)
        bool affectsAtk = false; // 1f
        bool affectsDef = false; // 20
        bool affecsMind = false; // 21
        bool affectsAgi = false; // 22
        int hitRate = 100; // 23
        bool evadeAttacks = false; // 24 (2003)
        bool reflectSkills = false; // 25 (2003)
        bool cursed = false; // 26 (2003)
        int battlerAnimationID = 100; // 27 (2003)
        bool hitSkillRestrict = false; // 29
        int hitSkillCutoff = 0; // 2a
        bool mindSkillRestrict = false; // 2b
        int mindSkillCutoff = 0; // 2c
        int hpAlterationType = 0; // 2d (2003)
        int mpAlterationType = 0; // 2e (2003)
        string messageAllyGets = ""; // 33
        string messageEnemyGets = ""; // 34
        string messageAlreadyGot = ""; // 35
        string messageStillGot = ""; // 36
        string messageRecovered = ""; // 37
        int turnHPPercent = 0; // 3d
        int turnHPPlus = 0; // 3e
        int mapHPPercent = 0; // 3f
        int mapHPPlus = 0; // 40
        int turnMPPercent = 0; // 41
        int turnMPPlus = 0; // 42
        int mapMPPercent = 0; // 43
        int mapMPPlus = 0; // 44
        
        static string myClass = "Condition";
        Chunks chunks;
        
        static int conditionNameLimit = 8;
        static int messageLimit = 30;
        
        static string[] limitations = { "None", "Can't Act", "Attack Random Enemies", "Attack Allies" };
        static string[] affectTypes = { "Half", "Double", "None" };
        static string[] hpMPAlterationTypes = { "Damage", "Recovery", "None" };
        
        public Condition(FileStream f)
        {
            load(f);
        }
        public Condition()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                conditionName = M.readStringDataName(f, id, ref M.conditionNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    classification = M.readLengthMultibyte(f);
                if (chunks.next(0x03))
                    color = M.readLengthMultibyte(f);
                if (chunks.next(0x04))
                    priority = M.readLengthMultibyte(f);
                if (chunks.next(0x05))
                    limitation = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0b))
                    rateA = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    rateB = M.readLengthMultibyte(f);
                if (chunks.next(0x0d))
                    rateC = M.readLengthMultibyte(f);
                if (chunks.next(0x0e))
                    rateD = M.readLengthMultibyte(f);
                if (chunks.next(0x0f))
                    rateE = M.readLengthMultibyte(f);
                
                if (chunks.next(0x15))
                    naturalHealTurns = M.readLengthMultibyte(f);
                if (chunks.next(0x16))
                    naturalHealChance = M.readLengthMultibyte(f);
                if (chunks.next(0x17))
                    hitHealChance = M.readLengthMultibyte(f);
                
                if (chunks.next(0x1e))
                    affectType = M.readLengthMultibyte(f);
                if (chunks.next(0x1f))
                    affectsAtk = M.readLengthBool(f);
                if (chunks.next(0x20))
                    affectsDef = M.readLengthBool(f);
                if (chunks.next(0x21))
                    affecsMind = M.readLengthBool(f);
                if (chunks.next(0x22))
                    affectsAgi = M.readLengthBool(f);
                if (chunks.next(0x23))
                    hitRate = M.readLengthMultibyte(f);
                
                if (chunks.next(0x24))
                    evadeAttacks = M.readLengthBool(f);
                if (chunks.next(0x25))
                    reflectSkills = M.readLengthBool(f);
                if (chunks.next(0x26))
                    cursed = M.readLengthBool(f);
                
                if (chunks.next(0x27))
                    battlerAnimationID = M.readLengthMultibyte(f);
                
                if (chunks.next(0x29))
                    hitSkillRestrict = M.readLengthBool(f);
                if (chunks.next(0x2a))
                    hitSkillCutoff = M.readLengthMultibyte(f);
                if (chunks.next(0x2b))
                    mindSkillRestrict = M.readLengthBool(f);
                if (chunks.next(0x2c))
                    mindSkillCutoff = M.readLengthMultibyte(f);
                
                if (chunks.next(0x2d))
                    hpAlterationType = M.readLengthMultibyte(f);
                if (chunks.next(0x2e))
                    mpAlterationType = M.readLengthMultibyte(f);
                
                if (chunks.next(0x33))
                    messageAllyGets = M.readString(f, M.S_TOTRANSLATE);
                if (chunks.next(0x34))
                    messageEnemyGets = M.readString(f, M.S_TOTRANSLATE);
                if (chunks.next(0x35))
                    messageAlreadyGot = M.readString(f, M.S_TOTRANSLATE);
                if (chunks.next(0x36))
                    messageStillGot = M.readString(f, M.S_TOTRANSLATE);
                if (chunks.next(0x37))
                    messageRecovered = M.readString(f, M.S_TOTRANSLATE);
                
                if (chunks.next(0x3d))
                    turnHPPercent = M.readLengthMultibyte(f);
                if (chunks.next(0x3e))
                    turnHPPlus = M.readLengthMultibyte(f);
                if (chunks.next(0x3f))
                    mapHPPercent = M.readLengthMultibyte(f);
                if (chunks.next(0x40))
                    mapHPPlus = M.readLengthMultibyte(f);
                if (chunks.next(0x41))
                    turnMPPercent = M.readLengthMultibyte(f);
                if (chunks.next(0x42))
                    turnMPPlus = M.readLengthMultibyte(f);
                if (chunks.next(0x43))
                    mapMPPercent = M.readLengthMultibyte(f);
                if (chunks.next(0x44))
                    mapMPPlus = M.readLengthMultibyte(f);
                
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
            
            bool defaultState = id == 1; // Condition 1 lacks most customization due to designation as "fainted" state.
            
            if (M.stringScriptExportMode)
            {
                M.setCurrentDatabaseEntry(myClass, id);
                
                tabText.WriteLine("*****Entry" + id + "*****");
                
                tabText.WriteLine(M.databaseExportString("Name", conditionName, "[" + conditionNameLimit + "]"));
                tabText.WriteLine(M.databaseExportString("InflictedOnAlly", messageAllyGets, "[" + messageLimit + "]"));
                tabText.WriteLine(M.databaseExportString("InflictedOnEnemy", messageEnemyGets, "[" + messageLimit + "]"));
                if (!defaultState)
                {
                    tabText.WriteLine(M.databaseExportString("AlreadyInflicted", messageAlreadyGot, "[" + messageLimit + "]"));
                    tabText.WriteLine(M.databaseExportString("Continuing", messageStillGot, "[" + messageLimit + "]"));
                }
                tabText.WriteLine(M.databaseExportString("Recovered", messageRecovered, "[" + messageLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            tabText.WriteLine("Condition #" + id);
            if (M.includeMessages)
                tabText.WriteLine("Name: " + conditionName);
            tabText.WriteLine("Type: " + (classification == 0? "Battle" : "Map"));
            tabText.WriteLine("Color: " + color);
            if (!defaultState)
            {
                tabText.WriteLine("Priority: " + priority);
                tabText.WriteLine("Limitation: " + limitations[limitation]);
            }
            tabText.WriteLine("A Chance: " + rateA + "%");
            tabText.WriteLine("B Chance: " + rateB + "%");
            tabText.WriteLine("C Chance: " + rateC + "%");
            tabText.WriteLine("D Chance: " + rateD + "%");
            tabText.WriteLine("E Chance: " + rateE + "%");
            if (M.includeMessages)
            {
                tabText.WriteLine("Ally Inflicted Message: " + messageAllyGets);
                tabText.WriteLine("Enemy Inflicted Message: " + messageEnemyGets);
                if (!defaultState)
                {
                    tabText.WriteLine("Already Inflicted Message: " + messageAlreadyGot);
                    tabText.WriteLine("Continue Message: " + messageStillGot);
                }
                tabText.WriteLine("Recovery Message: " + messageRecovered);
            }
            
            if (!defaultState)
            {
                tabText.WriteLine("Turns For Natural Heal: " + naturalHealTurns);
                tabText.WriteLine("Chance of Natural Heal: " + naturalHealChance + "%");
                tabText.WriteLine("Chance of Heal By Hit: " + hitHealChance + "%");
                
                tabText.WriteLine("Hit Rate: " + hitRate + "%");
                
                if (M.is2003)
                    tabText.WriteLine("Stat Alteration: " + affectTypes[affectType]);
                if (affectType != 2)
                {
                    tabText.WriteLine(affectTypes[affectType] + " Attack: " + affectsAtk);
                    tabText.WriteLine(affectTypes[affectType] + " Defense: " + affectsDef);
                    tabText.WriteLine(affectTypes[affectType] + " Mind: " + affecsMind);
                    tabText.WriteLine(affectTypes[affectType] + " Agility: " + affectsAgi);
                }
                
                if (M.is2003)
                {
                    tabText.WriteLine("Evade All Attacks: " + evadeAttacks);
                    tabText.WriteLine("Reflect Skills: " + reflectSkills);
                    tabText.WriteLine("Lock Equipment: " + cursed);
                }
                
                if (hitSkillRestrict)
                    tabText.WriteLine("Can't Use Skills With Attack Above: " + hitSkillCutoff);
                if (mindSkillRestrict)
                    tabText.WriteLine("Can't Use Skills With Mind Above: " + mindSkillCutoff);
                
                if (M.is2003)
                    tabText.WriteLine("Battler Animation: " + M.getDataBattlerPose(battlerAnimationID));
                
                if (M.is2003)
                    tabText.WriteLine("HP Alteration: " + hpMPAlterationTypes[hpAlterationType]);
                if (hpAlterationType != 2)
                {
                    tabText.WriteLine("HP " + hpMPAlterationTypes[hpAlterationType] + " Per Turn: " + turnHPPercent + "% + " + turnHPPlus);
                    if (classification == 1) // Map
                        tabText.WriteLine("HP " + hpMPAlterationTypes[hpAlterationType] + " Per Step: " + mapHPPercent + "% + " + mapHPPlus);
                }
                
                if (M.is2003)
                    tabText.WriteLine("MP Alteration: " + hpMPAlterationTypes[mpAlterationType]);
                if (mpAlterationType != 2)
                {
                    tabText.WriteLine("MP " + hpMPAlterationTypes[mpAlterationType] + " Per Turn: " + turnMPPercent + "% + " + turnMPPlus);
                    if (classification == 1) // Movement
                        tabText.WriteLine("MP " + hpMPAlterationTypes[mpAlterationType] + " Per Step: " + mapMPPercent + "% + " + mapMPPlus);
                }
            }
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x12;
            M.importDatabaseString(tabNum, id, "Name", ref conditionName, conditionNameLimit);
            M.importDatabaseString(tabNum, id, "InflictedOnAlly", ref messageAllyGets, M.ignoreLengthLimits >= 1? -1 : messageLimit);
            M.importDatabaseString(tabNum, id, "InflictedOnEnemy", ref messageEnemyGets, M.ignoreLengthLimits >= 1? -1 : messageLimit);
            M.importDatabaseString(tabNum, id, "AlreadyInflicted", ref messageAlreadyGot, M.ignoreLengthLimits >= 1? -1 : messageLimit);
            M.importDatabaseString(tabNum, id, "Continuing", ref messageStillGot, M.ignoreLengthLimits >= 1? -1 : messageLimit);
            M.importDatabaseString(tabNum, id, "Recovered", ref messageRecovered, M.ignoreLengthLimits >= 1? -1 : messageLimit);
            
            if (conditionName != "")
                chunks.add(0x01);
            if (messageAllyGets != "")
                chunks.add(0x33);
            if (messageEnemyGets != "")
                chunks.add(0x34);
            if (messageAlreadyGot != "")
                chunks.add(0x35);
            if (messageStillGot != "")
                chunks.add(0x36);
            if (messageRecovered != "")
                chunks.add(0x37);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(conditionName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(classification);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(color);
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(priority);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(limitation);
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(rateA);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(rateB);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(rateC);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(rateD);
            if (chunks.wasNext(0x0f))
                M.writeLengthMultibyte(rateE);
            
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(naturalHealTurns);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(naturalHealChance);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(hitHealChance);
            
            if (chunks.wasNext(0x1e))
                M.writeLengthMultibyte(affectType);
            if (chunks.wasNext(0x1f))
                M.writeLengthBool(affectsAtk);
            if (chunks.wasNext(0x20))
                M.writeLengthBool(affectsDef);
            if (chunks.wasNext(0x21))
                M.writeLengthBool(affecsMind);
            if (chunks.wasNext(0x22))
                M.writeLengthBool(affectsAgi);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(hitRate);
            
            if (chunks.wasNext(0x24))
                M.writeLengthBool(evadeAttacks);
            if (chunks.wasNext(0x25))
                M.writeLengthBool(reflectSkills);
            if (chunks.wasNext(0x26))
                M.writeLengthBool(cursed);
            
            if (chunks.wasNext(0x27))
                M.writeLengthMultibyte(battlerAnimationID);
            
            if (chunks.wasNext(0x29))
                M.writeLengthBool(hitSkillRestrict);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(hitSkillCutoff);
            if (chunks.wasNext(0x2b))
                M.writeLengthBool(mindSkillRestrict);
            if (chunks.wasNext(0x2c))
                M.writeLengthMultibyte(mindSkillCutoff);
            
            if (chunks.wasNext(0x2d))
                M.writeLengthMultibyte(hpAlterationType);
            if (chunks.wasNext(0x2e))
                M.writeLengthMultibyte(mpAlterationType);
            
            if (chunks.wasNext(0x33))
                M.writeString(messageAllyGets, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x34))
                M.writeString(messageEnemyGets, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x35))
                M.writeString(messageAlreadyGot, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x36))
                M.writeString(messageStillGot, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x37))
                M.writeString(messageRecovered, M.S_TOTRANSLATE);
            
            if (chunks.wasNext(0x3d))
                M.writeLengthMultibyte(turnHPPercent);
            if (chunks.wasNext(0x3e))
                M.writeLengthMultibyte(turnHPPlus);
            if (chunks.wasNext(0x3f))
                M.writeLengthMultibyte(mapHPPercent);
            if (chunks.wasNext(0x40))
                M.writeLengthMultibyte(mapHPPlus);
            if (chunks.wasNext(0x41))
                M.writeLengthMultibyte(turnMPPercent);
            if (chunks.wasNext(0x42))
                M.writeLengthMultibyte(turnMPPlus);
            if (chunks.wasNext(0x43))
                M.writeLengthMultibyte(mapMPPercent);
            if (chunks.wasNext(0x44))
                M.writeLengthMultibyte(mapMPPlus);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (conditionName != "" // 01
             || classification != 0 // 02
             || color != 6 // 03
             || priority != 50 // 04
             || limitation != 0 // 05
             || rateA != 100 // 0b
             || rateB != 80 // 0c
             || rateC != 60 // 0d
             || rateD != 30 // 0e
             || rateE != 0 // 0f
             || naturalHealTurns != 0 // 15
             || naturalHealChance != 0 // 16
             || hitHealChance != 0 // 17
             || affectType != 0 // 1e
             || affectsAtk // 1f
             || affectsDef // 20
             || affecsMind // 21
             || affectsAgi // 22
             || hitRate != 100 // 23
             || evadeAttacks // 24
             || reflectSkills // 25
             || cursed // 26
             || battlerAnimationID != 100 // 27
             || hitSkillRestrict // 29
             || hitSkillCutoff != 0 // 2a
             || mindSkillRestrict // 2b
             || mindSkillCutoff != 0 // 2c
             || hpAlterationType != 0 // 2d
             || mpAlterationType != 0 // 2e
             || messageAllyGets != "" // 33
             || messageEnemyGets != "" // 34
             || messageAlreadyGot != "" // 35
             || messageStillGot != "" // 36
             || messageRecovered != "" // 37
             || turnHPPercent != 0 // 3d
             || turnHPPlus != 0 // 3e
             || mapHPPercent != 0 // 3f
             || mapHPPlus != 0 // 40
             || turnMPPercent != 0 // 41
             || turnMPPlus != 0 // 42
             || mapMPPercent != 0 // 43
             || mapMPPlus != 0) // 44)
                return false;
            
            return true;
        }
    }
}
