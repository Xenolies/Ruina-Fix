using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Classes : RPGByteData
    {
        List<Class> classes;
        
        public Classes(FileStream f)
        {
            load(f);
        }
        public Classes()
        {
        }
        
        override public void load(FileStream f)
        {
            classes = M.readDatabaseList<Class>(f, "Classes", "Class", ref M.classNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < classes.Count; i++)
                tabText.Write(classes[i].getString()
                    + (i < classes.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Class classs in classes)
                classs.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Class>(classes);
        }
    }
    
    class Class : RPGDatabaseEntry
    {
        int id = 0;
        string className = ""; // 01
        bool twoSwordStyle = false; // 15
        bool fixedEquipment = false; // 16
        bool aiControl = false; // 17
        bool strongDefense = false; // 18
        int[][] statsForLevel; // 1f
        int expCurveBase = 300; // 29
        int expCurveIncrease = 300; // 2a
        int expCurveAdjustment = 0; // 2b
        int battlerAnimation = 1; // 3e
        List<HeroLearnSkill> learnSkills; // 3f
        int conditionEffectLength = 0; // 47
        int[] conditionEffect; // 48
        int attributeRankLength = 0; // 49
        int[] attributeRank; // 4a
        long[] battleCommands; // 50
        
        static string myClass = "Class";
        Chunks chunks;
        
        static int classNameLimit = 12;
        
        static string[] stats = { "Max HP", "Max MP", "Attack", "Defense", "Mind", "Agility" };
        static string[] effectRanks = { "A", "B", "C", "D", "E" };
        
        public Class(FileStream f)
        {
            load(f);
        }
        public Class()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                className = M.readStringDataName(f, id, ref M.heroNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x15))
                    twoSwordStyle = M.readLengthBool(f);
                if (chunks.next(0x16))
                    fixedEquipment = M.readLengthBool(f);
                if (chunks.next(0x17))
                    aiControl = M.readLengthBool(f);
                if (chunks.next(0x18))
                    strongDefense = M.readLengthBool(f);
                
                if (chunks.next(0x1f))
                    statsForLevel = M.readTwoByteArray2D(f, 6);
                
                if (chunks.next(0x29))
                    expCurveBase = M.readLengthMultibyte(f);
                if (chunks.next(0x2a))
                    expCurveIncrease = M.readLengthMultibyte(f);
                if (chunks.next(0x2b))
                    expCurveAdjustment = M.readLengthMultibyte(f);
                
                if (chunks.next(0x3e))
                    battlerAnimation = M.readLengthMultibyte(f);
                
                if (chunks.next(0x3f))
                    learnSkills = M.readList<HeroLearnSkill>(f);
                
                if (chunks.next(0x47))
                    conditionEffectLength = M.readLengthMultibyte(f);
                if (chunks.next(0x48))
                    conditionEffect = M.readByteArray(f);
                
                if (chunks.next(0x49))
                    attributeRankLength = M.readLengthMultibyte(f);
                if (chunks.next(0x4a))
                    attributeRank = M.readByteArray(f);
                
                if (chunks.next(0x50))
                    battleCommands = M.readFourByteArray(f);
                
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
                
                tabText.WriteLine(M.databaseExportString("Name", className, "[" + classNameLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter statCurvesStr = new StringWriter(new StringBuilder());
            StringWriter learnSkillsStr = new StringWriter(new StringBuilder());
            StringWriter conditionEffectStr = new StringWriter(new StringBuilder());
            StringWriter attributeRankStr = new StringWriter(new StringBuilder());
            StringWriter battleCommandStr = new StringWriter(new StringBuilder());
            
            for (int stat = 0; stat < 6; stat++)
            {
                statCurvesStr.Write(stats[stat] + ":");
                for (int level = 0; level < statsForLevel[stat].Length; level++)
                    statCurvesStr.Write(" " + statsForLevel[stat][level]);
                if (stat < 5)
                    statCurvesStr.WriteLine();
            }
            
            for (int i = 0; i < learnSkills.Count; i++)
                learnSkillsStr.WriteLine(learnSkills[i].getString());
            if (learnSkillsStr.ToString() == "")
                learnSkillsStr.WriteLine("(None)");
            
            for (int condition = 0; condition < conditionEffect.Length; condition++)
                conditionEffectStr.WriteLine(M.getDataCondition(condition + 1) + ": " + effectRanks[conditionEffect[condition]]);
            if (conditionEffectStr.ToString() == "")
                conditionEffectStr.WriteLine("(Default)");
            
            for (int attribute = 0; attribute < attributeRank.Length; attribute++)
                attributeRankStr.WriteLine(M.getDataAttribute(attribute + 1) + ": " + effectRanks[attributeRank[attribute]]);
            if (attributeRankStr.ToString() == "")
                attributeRankStr.WriteLine("(Default)");
            
            for (int i = 0; i < battleCommands.Length; i++)
                if (battleCommands[i] > 0)
                    battleCommandStr.WriteLine(M.getDataBattleCommand((int)battleCommands[i]));
            if (battleCommandStr.ToString() == "")
                battleCommandStr.WriteLine("(None)");
            
            tabText.WriteLine("Class #" + id);
            if (M.includeMessages)
                tabText.WriteLine("Name: " + className);
            
            tabText.WriteLine("Battler Animations: " + M.getDataBattleAnimSet(battlerAnimation));
            
            tabText.WriteLine("Stat Curves:");
            tabText.WriteLine(statCurvesStr);
            tabText.WriteLine("EXP Curve: " + expCurveBase + " Base, " + expCurveIncrease + " Increase, " + expCurveAdjustment + " Adjust");
            
            tabText.WriteLine("Two Sword Style: " + twoSwordStyle);
            tabText.WriteLine("Fixed Equipment: " + fixedEquipment);
            tabText.WriteLine("AI Control: " + aiControl);
            tabText.WriteLine("Strong Defense: " + strongDefense);
            
            tabText.WriteLine("Skill Progression:");
            tabText.Write(learnSkillsStr);
            
            tabText.WriteLine("Condition Effect:");
            tabText.Write(conditionEffectStr);
            
            tabText.WriteLine("Attribute Ranks:");
            tabText.Write(attributeRankStr);
            
            tabText.WriteLine("Battle Commands:");
            tabText.Write(battleCommandStr);
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x1e;
            M.importDatabaseString(tabNum, id, "Name", ref className, classNameLimit);
            
            if (className != "")
                chunks.add(0x01);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(className, M.S_TOTRANSLATE);
            
            if (chunks.wasNext(0x15))
                M.writeLengthBool(twoSwordStyle);
            if (chunks.wasNext(0x16))
                M.writeLengthBool(fixedEquipment);
            if (chunks.wasNext(0x17))
                M.writeLengthBool(aiControl);
            if (chunks.wasNext(0x18))
                M.writeLengthBool(strongDefense);
            
            if (chunks.wasNext(0x1f))
                M.writeTwoByteArray2D(statsForLevel);
            
            if (chunks.wasNext(0x29))
                M.writeLengthMultibyte(expCurveBase);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(expCurveIncrease);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(expCurveAdjustment);
            
            if (chunks.wasNext(0x3e))
                M.writeLengthMultibyte(battlerAnimation);
            
            if (chunks.wasNext(0x3f))
                M.writeList<HeroLearnSkill>(learnSkills);
            
            if (chunks.wasNext(0x47))
                M.writeLengthMultibyte(conditionEffectLength);
            if (chunks.wasNext(0x48))
                M.writeByteArray(conditionEffect);
            
            if (chunks.wasNext(0x49))
                M.writeLengthMultibyte(attributeRankLength);
            if (chunks.wasNext(0x4a))
                M.writeByteArray(attributeRank);
            
            if (chunks.wasNext(0x50))
                M.writeFourByteArray(battleCommands);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (className != "") // 01
                return false;
            
            return true;
        }
    }
}
