using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Heroes : RPGByteData
    {
        List<Hero> heroes;
        
        public Heroes(FileStream f)
        {
            load(f);
        }
        public Heroes()
        {
        }
        
        override public void load(FileStream f)
        {
            heroes = M.readDatabaseList<Hero>(f, "Heroes", "Hero", ref M.heroNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < heroes.Count; i++)
                tabText.Write(heroes[i].getString()
                    + (i < heroes.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Hero hero in heroes)
                hero.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Hero>(heroes);
        }
        
        public string getHeroDefaultName(int id)
        {
            if (heroes != null)
                if (id < heroes.Count)
                    return heroes[id - 1].getDefaultName();
            return "";
        }
        public string getHeroDefaultTitle(int id)
        {
            if (heroes != null)
                if (id < heroes.Count)
                    return heroes[id - 1].getDefaultTitle();
            return "";
        }
        public int getHeroMaxHP(int id, int level)
        {
            if (heroes != null)
                if (id < heroes.Count)
                    return heroes[id - 1].getMaxHP(level);
            return 1;
        }
        public int getHeroMaxMP(int id, int level)
        {
            if (heroes != null)
                if (id < heroes.Count)
                    return heroes[id - 1].getMaxMP(level);
            return 0;
        }
        public int getHeroEXPForLevel(int id, int level)
        {
            if (heroes != null)
                if (id < heroes.Count)
                    return heroes[id - 1].getEXPForLevel(level);
            return 0;
        }
        public List<HeroLearnSkill> getHeroLearnSkills(int id)
        {
            if (heroes != null)
                if (id < heroes.Count)
                    return heroes[id - 1].getLearnSkills();
            return new List<HeroLearnSkill>();
        }
    }
    
    class Hero : RPGDatabaseEntry
    {
        int id = 0;
        string heroName = ""; // 01
        string heroTitle = ""; // 02
        string charSet = ""; // 03
        int charIndex = 0; // 04
        bool charTransparent = false; // 05
        int initialLevel = 1; // 07
        int maxLevel = -1; // 08; default is 50 in 2000, 99 in 2003
        bool useCritChance = true; // 09
        int critChance = 30; // 0a
        string faceSet = ""; // 0f
        int faceIndex = 0; // 10
        bool twoSwordStyle = false; // 15
        bool fixedEquipment = false; // 16
        bool aiControl = false; // 17
        bool strongDefense = false; // 18
        int[][] statsForLevel; // 1f
        int expCurveBase = -1; // 29; default is 30 in 2000, 300 in 2003
        int expCurveIncrease = -1; // 2a; default is 30 in 2000, 300 in 2003
        int expCurveAdjustment = 0; // 2b
        int[] startingEquipment; // 33
        int unarmedAnimation = 1; // 38
        int classID = 0; // 39 (2003)
        int battleSpriteX = 220; // 3b (2003)
        int battleSpriteY = 120; // 3c (2003)
        int battlerAnimation = 1; // 3e (2003)
        List<HeroLearnSkill> learnSkills; // 3f
        bool skillCommandRenamed = false; // 42
        string skillCommandName = ""; // 43
        int conditionEffectLength = 0; // 47
        int[] conditionEffect; // 48
        int attributeRankLength = 0; // 49
        int[] attributeRank; // 4a
        long[] battleCommands; // 50, verbose only (2003)
        
        static string myClass = "Hero";
        Chunks chunks;
        
        static int heroNameLimit = 12;
        static int heroTitleLimit = 12;
        static int skillCommandLimit = 10;
        
        static string[] statNames = { "Max HP", "Max MP", "Attack", "Defense", "Mind", "Agility" };
        static string[] equipSlots = { "Arms", "Weapon", "Armor", "Helmet", "Other" };
        static string[] effectRanks = { "A", "B", "C", "D", "E" };
        
        public Hero(FileStream f)
        {
            load(f);
        }
        public Hero()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                heroName = M.readStringDataName(f, id, ref M.heroNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    heroTitle = M.readString(f, M.S_TOTRANSLATE);
                if (chunks.next(0x03))
                    charSet = M.readStringAndRewrite(f, M.M_CHARSET, M.S_FILENAME);
                if (chunks.next(0x04))
                    charIndex = M.readLengthMultibyte(f);
                if (chunks.next(0x05))
                    charTransparent = M.readLengthBool(f);
                if (chunks.next(0x07))
                    initialLevel = M.readLengthMultibyte(f);
                if (chunks.next(0x08))
                    maxLevel = M.readLengthMultibyte(f);
                if (chunks.next(0x09))
                    useCritChance = M.readLengthBool(f);
                if (chunks.next(0x0a))
                    critChance = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0f))
                    faceSet = M.readStringAndRewrite(f, M.M_FACESET, M.S_FILENAME);
                if (chunks.next(0x10))
                    faceIndex = M.readLengthMultibyte(f);
                
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
                
                if (chunks.next(0x33))
                    startingEquipment = M.readTwoByteArray(f);
                
                if (chunks.next(0x38))
                    unarmedAnimation = M.readLengthMultibyte(f);
                
                if (chunks.next(0x39))
                    classID = M.readLengthMultibyte(f);
                if (chunks.next(0x3b))
                    battleSpriteX = M.readLengthMultibyte(f);
                if (chunks.next(0x3c))
                    battleSpriteY = M.readLengthMultibyte(f);
                if (chunks.next(0x3e))
                    battlerAnimation = M.readLengthMultibyte(f);
                
                if (chunks.next(0x3f))
                    learnSkills = M.readList<HeroLearnSkill>(f);
                
                if (chunks.next(0x42))
                    skillCommandRenamed = M.readLengthBool(f);
                if (chunks.next(0x43))
                    skillCommandName = M.readString(f, M.S_TOTRANSLATE);
                
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
                
                // Determine version-varying defaults based on level count.
                if (statsForLevel != null)
                {
                    if (maxLevel == -1)
                        maxLevel = statsForLevel[0].Length;
                    if (expCurveBase == -1)
                        expCurveBase = statsForLevel[0].Length <= 50? 30 : 300;
                    if (expCurveIncrease == -1)
                        expCurveIncrease = statsForLevel[0].Length <= 50? 30 : 300;
                }
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
                
                tabText.WriteLine(M.databaseExportString("Name", heroName, "[" + heroNameLimit + "]"));
                tabText.WriteLine(M.databaseExportString("Title", heroTitle, "[" + heroTitleLimit + "]"));
                if (skillCommandRenamed)
                    tabText.WriteLine(M.databaseExportString("SkillCommand", skillCommandName, "[" + skillCommandLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter statCurvesStr = new StringWriter(new StringBuilder());
            StringWriter startingEquipmentStr = new StringWriter(new StringBuilder());
            StringWriter learnSkillsStr = new StringWriter(new StringBuilder());
            StringWriter conditionEffectStr = new StringWriter(new StringBuilder());
            StringWriter attributeRankStr = new StringWriter(new StringBuilder());
            StringWriter battleCommandStr = new StringWriter(new StringBuilder());
            
            for (int stat = 0; stat < 6; stat++)
            {
                statCurvesStr.Write(statNames[stat] + ":");
                for (int level = 0; level < statsForLevel[stat].Length; level++)
                    statCurvesStr.Write(" " + statsForLevel[stat][level]);
                if (stat < 5)
                    statCurvesStr.WriteLine();
            }
            
            for (int slot = 0; slot < 5; slot++)
            {
                if (startingEquipment[slot] > 0)
                    startingEquipmentStr.Write(equipSlots[slot] + ": " + M.getDataItem(startingEquipment[slot]));
                else
                    startingEquipmentStr.Write(equipSlots[slot] + ": None");
                if (slot < 4)
                    startingEquipmentStr.WriteLine();
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
            
            if (battleCommands != null)
                for (int i = 0; i < battleCommands.Length; i++)
                    if (battleCommands[i] > 0)
                        battleCommandStr.WriteLine(M.getDataBattleCommand((int)battleCommands[i]));
            
            tabText.WriteLine("Hero #" + id);
            if (M.includeMessages)
            {
                tabText.WriteLine("Name: " + heroName);
                tabText.WriteLine("Title: " + heroTitle);
            }
            if (M.is2003)
                tabText.WriteLine("Class: " + (classID != 0? M.getDataClass(classID) : "(None)"));
            
            tabText.WriteLine("Walk Sprite: " + charSet + ", " + (charIndex + 1) + (charTransparent? " (Transparent)" : ""));
            tabText.WriteLine("Face Graphic: " + faceSet + ", " + (faceIndex + 1));
            if (M.is2003)
            {
                tabText.WriteLine("Battle Sprite: " + battleSpriteX + "," + battleSpriteY);
                tabText.WriteLine("Battler Animations: " + M.getDataBattleAnimSet(battlerAnimation));
            }
            
            tabText.WriteLine("Initial Level: " + initialLevel);
            tabText.WriteLine("Max Level: " + maxLevel);
            if (useCritChance)
                tabText.WriteLine("Critical Chance: " + critChance + "%");
            
            tabText.WriteLine("Stat Curves:");
            tabText.WriteLine(statCurvesStr);
            tabText.WriteLine("EXP Curve: " + expCurveBase + " Base, " + expCurveIncrease + " Increase, " + expCurveAdjustment + " Adjust");
            
            tabText.WriteLine("Two Sword Style: " + twoSwordStyle);
            tabText.WriteLine("Fixed Equipment: " + fixedEquipment);
            tabText.WriteLine("AI Control: " + aiControl);
            tabText.WriteLine("Strong Defense: " + strongDefense);
            
            tabText.WriteLine("Initial Equipment:");
            tabText.WriteLine(startingEquipmentStr);
            
            tabText.WriteLine("Unarmed Animation: " + M.getDataAnimation(unarmedAnimation));
            
            tabText.WriteLine("Skill Progression:");
            tabText.Write(learnSkillsStr);
            
            if (skillCommandRenamed && M.includeMessages)
                tabText.WriteLine("Skill Command Name: " + skillCommandName);
            
            tabText.WriteLine("Condition Effect:");
            tabText.Write(conditionEffectStr);
            
            tabText.WriteLine("Attribute Ranks:");
            tabText.Write(attributeRankStr);
            
            if (M.is2003 && id == 1) // Weirdly, default battle commands from Battle Layout tab are stored in Hero #1?
            {
                tabText.WriteLine("Default Battle Commands:");
                tabText.Write(battleCommandStr);
            }
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x0b;
            M.importDatabaseString(tabNum, id, "Name", ref heroName, heroNameLimit);
            M.importDatabaseString(tabNum, id, "Title", ref heroTitle, heroTitleLimit);
            M.importDatabaseString(tabNum, id, "SkillCommand", ref skillCommandName, skillCommandLimit);
            
            if (heroName != "")
                chunks.add(0x01);
            if (heroTitle != "")
                chunks.add(0x02);
            if (skillCommandName != "")
                chunks.add(0x43);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(heroName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x02))
                M.writeString(heroTitle, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x03))
                M.writeString(charSet, M.S_FILENAME);
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(charIndex);
            if (chunks.wasNext(0x05))
                M.writeLengthBool(charTransparent);
            if (chunks.wasNext(0x07))
                M.writeLengthMultibyte(initialLevel);
            if (chunks.wasNext(0x08))
                M.writeLengthMultibyte(maxLevel);
            if (chunks.wasNext(0x09))
                M.writeLengthBool(useCritChance);
            if (chunks.wasNext(0x0a))
                M.writeLengthMultibyte(critChance);
            
            if (chunks.wasNext(0x0f))
                M.writeString(faceSet, M.S_FILENAME);
            if (chunks.wasNext(0x10))
                M.writeLengthMultibyte(faceIndex);
            
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
            
            if (chunks.wasNext(0x33))
                M.writeTwoByteArray(startingEquipment);
            
            if (chunks.wasNext(0x38))
                M.writeLengthMultibyte(unarmedAnimation);
            
            if (chunks.wasNext(0x39))
                M.writeLengthMultibyte(classID);
            if (chunks.wasNext(0x3b))
                M.writeLengthMultibyte(battleSpriteX);
            if (chunks.wasNext(0x3c))
                M.writeLengthMultibyte(battleSpriteY);
            if (chunks.wasNext(0x3e))
                M.writeLengthMultibyte(battlerAnimation);
            
            if (chunks.wasNext(0x3f))
                M.writeList<HeroLearnSkill>(learnSkills);
            
            if (chunks.wasNext(0x42))
                M.writeLengthBool(skillCommandRenamed);
            if (chunks.wasNext(0x43))
                M.writeString(skillCommandName, M.S_TOTRANSLATE);
            
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
            bool is2003 = statsForLevel[0].Length > 50;
            
            if (heroName != "" // 01
             || heroTitle != "" // 02
             || charSet != "" // 03
             || charIndex != 0 // 04
             || charTransparent // 05
             || initialLevel != 1 // 07
             // 08 below
             || !useCritChance // 09
             || critChance != 30 // 0a
             || faceSet != "" // 0f
             || faceIndex != 0 // 10
             || twoSwordStyle // 15
             || fixedEquipment // 16
             || aiControl // 17
             || strongDefense // 18
             // 1f below
             || (!is2003 && expCurveBase != 30) // 29
             || (is2003 && expCurveBase != 300) // 29
             || (!is2003 && expCurveIncrease != 30) // 2a
             || (is2003 && expCurveIncrease != 300) // 2a
             || expCurveAdjustment != 0 // 2b
             // 33 below
             || unarmedAnimation != 1 // 38
             || classID != 0 // 39
             || battleSpriteX != 220 // 3b
             || battleSpriteY != 120 // 3c
             || battlerAnimation != 1 // 3e
             || learnSkills.Count > 0 // 3f
             || skillCommandRenamed // 42
             || skillCommandName != "") // 43
             // 47 irrelevant (length)
             // 48 below
             // 49 irrelevant (length)
             // 4a below
             // 50 below
                return false;
            
            if (maxLevel != statsForLevel[0].Length) // 08
                return false;
            
            // 1f
            int[] defaultHPMP = new int[] { 40, 43, 49, 55, 60, 66, 72, 79, 85, 92,
                                            98, 105, 112, 120, 127, 135, 143, 151, 160, 168,
                                            177, 186, 195, 205, 215, 225, 236, 247, 258, 269,
                                            281, 293, 305, 318, 331, 345, 359, 373, 388, 404,
                                            419, 436, 453, 471, 489, 508, 527, 547, 568, 600 };
            int[] defaultAtkDef = new int[] { 15, 16, 18, 19, 20, 22, 23, 25, 26, 28,
                                              30, 31, 33, 35, 37, 38, 40, 42, 44, 47,
                                              49, 51, 53, 55, 58, 60, 63, 65, 68, 71,
                                              74, 77, 80, 83, 86, 89, 93, 96, 100, 103,
                                              107, 111, 115, 119, 124, 128, 133, 138, 143, 150 };
            int[] defaultMindAgi = new int[] { 20, 22, 25, 28, 31, 34, 37, 40, 43, 46,
                                               50, 53, 57, 61, 64, 68, 72, 76, 81, 85,
                                               89, 94, 98, 103, 108, 113, 119, 124, 129, 135,
                                               141, 147, 153, 160, 166, 173, 180, 187, 194, 202,
                                               210, 218, 227, 236, 245, 254, 264, 274, 284, 300 };
            
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < statsForLevel[0].Length; j++)
                {
                    if (!is2003)
                    {
                        int defaultValue = (i == 0 || i == 1)? defaultHPMP[j]
                                         : (i == 2 || i == 3)? defaultAtkDef[j]
                                                             : defaultMindAgi[j];
                        if (statsForLevel[i][j] != defaultValue)
                            return false;
                    }
                    else
                    {
                        int defaultValue = i != 1? 1 : 0;
                        if (statsForLevel[i][j] != defaultValue)
                            return false;
                    }
                }
            }
            
            if (startingEquipment != null)
                for (int i = 0; i < startingEquipment.Length; i++) // 33
                    if (startingEquipment[i] != 0)
                        return false;
            
            if (conditionEffect != null)
                for (int i = 0; i < conditionEffect.Length; i++) // 48
                    if (conditionEffect[i] != 2)
                        return false;
            
            if (attributeRank != null)
                for (int i = 0; i < attributeRank.Length; i++) // 4a
                    if (attributeRank[i] != 2)
                        return false;
            
            if (battleCommands != null)
                for (int i = 0; i < battleCommands.Length; i++) // 50
                    if (battleCommands[i] != i)
                        return false;
            
            return true;
        }
        
        public string getDefaultName()
        {
            return heroName;
        }
        public string getDefaultTitle()
        {
            return heroTitle;
        }
        public int getMaxHP(int level)
        {
            return statsForLevel[0][level - 1];
        }
        public int getMaxMP(int level)
        {
            return statsForLevel[1][level - 1];
        }
        public int getEXPForLevel(int level)
        {
            double basic = expCurveBase, inflation = expCurveIncrease, correction = expCurveAdjustment;
            int result = 0;
            
            if (!M.is2003)
            {
                inflation = 1.5 + (expCurveIncrease * 0.01);
                for (int i = level; i >= 1; i--)
                {
                    result = result + (int)(correction + basic);
                    basic = basic * inflation;
                    inflation = ((level + 1) * 0.002f + 0.8f) * (inflation - 1) + 1;
                }
            }
            else
            {
                for (int i = 1; i <= level; i++)
                {
                    result += (int)basic;
                    result += i * (int)inflation;
                    result += (int)correction;
                }
            }
            
            return Math.Min(result, 999999);
        }
        public List<HeroLearnSkill> getLearnSkills()
        {
            return learnSkills;
        }
    }
    
    class HeroLearnSkill : RPGByteData
    {
        int id = 0;
        int learnLevel = 1; // 01
        int learnID = 1; // 02
        
        static string myClass = "HeroLearnSkill";
        Chunks chunks;
        
        public HeroLearnSkill(FileStream f)
        {
            load(f);
        }
        public HeroLearnSkill()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                learnLevel = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
            {
                int length = M.readByte(f);
                learnID = length == 1? M.readByte(f) : M.readMultibyte(f);
            }
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            return "Level " + learnLevel + ": " + M.getDataSkill(learnID);
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(learnLevel);
            if (chunks.wasNext(0x02))
            {
                int value = learnID;
                int length = M.countMultibyte(value);
                M.writeByte(length);
                
                if (length == 1)
                    M.writeByte(value);
                else
                    M.writeMultibyte(value);
            }
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public int getLevel()
        {
            return learnLevel;
        }
        public int getSkillID()
        {
            return learnID;
        }
    }
}
