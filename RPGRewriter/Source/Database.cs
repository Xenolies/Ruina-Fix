using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Database
    {
        string filepath = "";
        string filename = "";
        
        Heroes heroes; // 0b
        Skills skills; // 0c
        Items items; // 0d
        Monsters monsters; // 0e
        Troops troops; // 0f
        Terrains terrains; // 10
        Attributes attributes; // 11
        Conditions conditions; // 12
        Animations animations; // 13
        ChipSets chipSets; // 14
        Vocab vocab; // 15
        System system; // 16
        Switches switches; // 17
        Variables variables; // 18
        CommonEvents commonEvents; // 19
        int version = -1; // 1a (2003 (version = 0) and 2000EN (version = 1))
        // 1b and 1c are unused Common Events 2/3 (2003)
        BattleSettings battleSettings; // 1d (2003)
        Classes classes; // 1e (2003)
        // 1f is unused Classes 2 (2003)
        BattlerAnimations battlerAnimations; // 20 (2003)
        
        static string myClass = "Database";
        Chunks chunks;
        
        public Database(string filepath, bool writeLog = true)
        {
            loadFile(filepath, writeLog);
        }
        public Database()
        {
        }
        
        // Loads the .ldb file. Returns success.
        public bool loadFile(string filepath, bool writeLog = true)
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Database file " + filepath + " not found.");
                return false;
            }
            
            this.filepath = filepath;
            filename = Path.GetFileName(filepath);
            M.currentFile = filename;
            
            if (M.readingDataNames)
                Console.WriteLine("Retrieving data names from database...");
            else if (M.makingDataEntryLists)
                Console.WriteLine("Retrieving data entries from database...");
            else if (M.comparisonMode || M.copyingSwitchVariable || M.copyingCommandValues)
                Console.WriteLine("Reading " + (M.readingOriginal? "original" : "translated") + " database...");
            else if (!M.gettingLDBVersion && !M.stringScriptImportMode)
                Console.WriteLine(M.globalMode + " database...");
            
            FileStream f = File.OpenRead(filepath);
            
            try
            {
                chunks = new Chunks(f, myClass);
                
                M.stringCheck(f, "LcfDataBase");
                
                if (!M.gettingLDBVersion)
                {
                    if (chunks.next(0x0b))
                    {
                        M.readMultibyte(f);
                        heroes = new Heroes(f);
                    }
                    if (chunks.next(0x0c))
                    {
                        M.readMultibyte(f);
                        skills = new Skills(f);
                    }
                    if (chunks.next(0x0d))
                    {
                        M.readMultibyte(f);
                        items = new Items(f);
                    }
                    if (chunks.next(0x0e))
                    {
                        M.readMultibyte(f);
                        monsters = new Monsters(f);
                    }
                    if (chunks.next(0x0f))
                    {
                        M.readMultibyte(f);
                        troops = new Troops(f);
                    }
                    if (chunks.next(0x10))
                    {
                        M.readMultibyte(f);
                        terrains = new Terrains(f);
                    }
                    if (chunks.next(0x11))
                    {
                        M.readMultibyte(f);
                        attributes = new Attributes(f);
                    }
                    if (chunks.next(0x12))
                    {
                        M.readMultibyte(f);
                        conditions = new Conditions(f);
                    }
                    if (chunks.next(0x13))
                    {
                        M.readMultibyte(f);
                        animations = new Animations(f);
                    }
                    if (chunks.next(0x14))
                    {
                        M.readMultibyte(f);
                        chipSets = new ChipSets(f);
                    }
                    if (chunks.next(0x15))
                    {
                        M.readMultibyte(f);
                        vocab = new Vocab(f);
                    }
                    if (chunks.next(0x16))
                    {
                        M.readMultibyte(f);
                        system = new System(f);
                    }
                    if (chunks.next(0x17))
                    {
                        M.readMultibyte(f);
                        switches = new Switches(f);
                    }
                    if (chunks.next(0x18))
                    {
                        M.readMultibyte(f);
                        variables = new Variables(f);
                    }
                    if (chunks.next(0x19))
                    {
                        M.readMultibyte(f);
                        commonEvents = new CommonEvents(f);
                    }
                }
                else // Skip over all tabs until version.
                    M.skipChunkRange(f, 0x0b, 0x19);
                
                if (chunks.next(0x1a))
                {
                    version = M.readMultibyte(f);
                    M.is2003 = version == 0;
                    M.is2000EN = version == 1;
                }
                
                if (!M.gettingLDBVersion) // After getting version, the rest can be skipped
                {
                    if (chunks.next(0x1b))
                        M.byteCheck(f, 0x00);
                    if (chunks.next(0x1c))
                        M.byteCheck(f, 0x00);
                    
                    if (chunks.next(0x1d))
                    {
                        M.readMultibyte(f);
                        battleSettings = new BattleSettings(f);
                    }
                    if (chunks.next(0x1e))
                    {
                        M.readMultibyte(f);
                        classes = new Classes(f);
                    }
                    
                    if (chunks.next(0x1f))
                        M.byteCheck(f, 0x00);
                    
                    if (chunks.next(0x20))
                    {
                        M.readMultibyte(f);
                        battlerAnimations = new BattlerAnimations(f);
                    }
                    
                    // Usually the case, but leaving out due to possibility of saving with a non-English editor and having version = 1 but not this.
                    //if (M.is2000EN)
                        //M.byteCheck(f, 0x01);
                }
                
                f.Close();
            }
            catch (Exception e)
            {
                M.debugMessage(e.StackTrace);
                M.debugMessage(e.Message);
                Console.WriteLine("Aborting due to error.");
                
                f.Close();
                return false;
            }
            
            // Force RPG Maker version if configured in user settings.
            if (M.forceEngineVersion == 1 && version != 1) // RPG Maker 2000 Japanese
            {
                version = -1;
                chunks.remove(0x1a);
                M.is2000EN = false;
                M.is2003 = false;
                M.changesMade = true;
            }
            else if (M.forceEngineVersion == 2 && version != 1) // RPG Maker 2000 English
            {
                version = 1;
                chunks.add(0x1a);
                M.is2000EN = true;
                M.is2003 = false;
                M.changesMade = true;
            }
            else if (M.forceEngineVersion == 3 && version != 0) // RPG Maker 2003
            {
                version = 0;
                chunks.add(0x1a);
                M.is2000EN = false;
                M.is2003 = true;
                M.changesMade = true;
            }
            
            if (writeLog)
                M.logSave();
            return true;
        }
        
        // Returns database string, or writes each section to a file.
        public string getString(bool writeFiles = false, string scriptDir = "")
        {
            StringWriter allDataText = new StringWriter(new StringBuilder());
            
            string lb = Environment.NewLine;
            string result;
            
            if (chunks.used(0x0b) && heroes != null)
            {
                result = heroes.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Hero.txt", result);
                else
                    allDataText.WriteLine("***** Hero *****" + lb + result + lb);
            }
            if (chunks.used(0x0c) && skills != null)
            {
                result = skills.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Skills.txt", result);
                else
                    allDataText.WriteLine("***** Skills *****" + lb + result + lb);
            }
            if (chunks.used(0x0d) && items != null)
            {
                result = items.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Items.txt", result);
                else
                    allDataText.WriteLine("***** Items *****" + lb + result + lb);
            }
            if (chunks.used(0x0e) && monsters != null)
            {
                result = monsters.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Monsters.txt", result);
                else
                    allDataText.WriteLine("***** Monsters *****" + lb + result + lb);
            }
            if (chunks.used(0x0f) && troops != null)
            {
                result = troops.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Troops.txt", result);
                else
                    allDataText.WriteLine("***** Troops *****" + lb + result + lb);
            }
            if (chunks.used(0x10) && terrains != null)
            {
                result = terrains.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Terrain.txt", result);
                else
                    allDataText.WriteLine("***** Terrain *****" + lb + result + lb);
            }
            if (chunks.used(0x11) && attributes != null)
            {
                result = attributes.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Attributes.txt", result);
                else
                    allDataText.WriteLine("***** Attributes *****" + lb + result + lb);
            }
            if (chunks.used(0x12) && conditions != null)
            {
                result = conditions.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Conditions.txt", result);
                else
                    allDataText.WriteLine("***** Conditions *****" + lb + result + lb);
            }
            if (chunks.used(0x13) && animations != null)
            {
                result = animations.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Animations.txt", result);
                else
                    allDataText.WriteLine("***** Animations *****" + lb + result + lb);
            }
            if (chunks.used(0x14) && chipSets != null)
            {
                result = chipSets.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\ChipSet.txt", result);
                else
                    allDataText.WriteLine("***** ChipSet *****" + lb + result + lb);
            }
            if (chunks.used(0x15) && vocab != null)
            {
                result = vocab.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Vocab.txt", result);
                else
                    allDataText.WriteLine("***** Vocab *****" + lb + result + lb);
            }
            
            if (!M.stringScriptExportMode || M.userSettings["StringScriptExtraneous"] == 1)
            {
                if (chunks.used(0x16) && !M.stringScriptExportMode && system != null)
                {
                    result = system.getString();
                    if (writeFiles)
                        M.writeToNewFile(scriptDir + "\\System.txt", result);
                    else
                        allDataText.WriteLine("***** System *****" + lb + result + lb);
                }
                if (chunks.used(0x17) && (!M.stringScriptExportMode || M.getExtraneousSetting("SwitchNames")) && switches != null)
                {
                    result = switches.getString();
                    if (writeFiles)
                        M.writeToNewFile(scriptDir + "\\Switches.txt", result);
                    else
                        allDataText.WriteLine("***** Switches *****" + lb + result + lb);
                }
                if (chunks.used(0x18) && (!M.stringScriptExportMode || M.getExtraneousSetting("VariableNames")) && variables != null)
                {
                    result = variables.getString();
                    if (writeFiles)
                        M.writeToNewFile(scriptDir + "\\Variables.txt", result);
                    else
                        allDataText.WriteLine("***** Variables *****" + lb + result + lb);
                }
            }
            
            if (chunks.used(0x19) && commonEvents != null)
            {
                result = commonEvents.getString(writeFiles, scriptDir + "\\Commons");
                if (!writeFiles)
                    allDataText.WriteLine("***** Common Events *****" + lb + result + lb);
            }
            
            if (!M.stringScriptExportMode)
            {
                if (chunks.used(0x1a))
                {
                    if (writeFiles)
                        M.writeToNewFile(scriptDir + "\\Version.txt", version.ToString());
                    else
                        allDataText.WriteLine("***** Version *****" + lb + version + lb);
                }
            }
            
            if (chunks.used(0x1d) && battleSettings != null)
            {
                result = battleSettings.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\BattleSettings.txt", result);
                else
                    allDataText.WriteLine("***** Battle Settings *****" + lb + result + lb);
            }
            if (chunks.used(0x1e) && classes != null)
            {
                result = classes.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\Classes.txt", result);
                else
                    allDataText.WriteLine("***** Classes *****" + lb + result + lb);
            }
            if (chunks.used(0x20) && battlerAnimations != null)
            {
                result = battlerAnimations.getString();
                if (writeFiles)
                    M.writeToNewFile(scriptDir + "\\BattlerAnimations.txt", result);
                else
                    allDataText.WriteLine("***** Battler Animations *****" + lb + result + lb);
            }
            
            if (!writeFiles)
                return allDataText.ToString();
            else
                return "OK";
        }
        
        // Loads string scripts for each tab and replaces strings.
        public void importStrings(string scriptDir)
        {
            if (chunks.used(0x0b))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Hero.txt", 0x0b);
                if (!M.stringScriptImportCheck)
                    heroes.importStrings();
            }
            if (chunks.used(0x0c))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Skills.txt", 0x0c);
                if (!M.stringScriptImportCheck)
                    skills.importStrings();
            }
            if (chunks.used(0x0d))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Items.txt", 0x0d);
                if (!M.stringScriptImportCheck)
                    items.importStrings();
            }
            if (chunks.used(0x0e))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Monsters.txt", 0x0e);
                if (!M.stringScriptImportCheck)
                    monsters.importStrings();
            }
            if (chunks.used(0x0f))
            {
                M.loadStringScript(scriptDir + "\\Troops.txt", -1, true);
                if (!M.stringScriptImportCheck)
                    troops.importStrings();
            }
            if (chunks.used(0x10))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Terrain.txt", 0x10);
                if (!M.stringScriptImportCheck)
                    terrains.importStrings();
            }
            if (chunks.used(0x11))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Attributes.txt", 0x11);
                if (!M.stringScriptImportCheck)
                    attributes.importStrings();
            }
            if (chunks.used(0x12))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Conditions.txt", 0x12);
                if (!M.stringScriptImportCheck)
                    conditions.importStrings();
            }
            if (chunks.used(0x13))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Animations.txt", 0x13);
                if (!M.stringScriptImportCheck)
                    animations.importStrings();
            }
            if (chunks.used(0x14))
            {
                M.loadStringScriptDatabase(scriptDir + "\\ChipSet.txt", 0x14);
                if (!M.stringScriptImportCheck)
                    chipSets.importStrings();
            }
            if (chunks.used(0x15))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Vocab.txt", 0x15);
                if (!M.stringScriptImportCheck)
                    vocab.importStrings();
            }
            if (chunks.used(0x17))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Switches.txt", 0x17, true);
                if (!M.stringScriptImportCheck)
                    switches.importStrings();
            }
            if (chunks.used(0x18))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Variables.txt", 0x18, true);
                if (!M.stringScriptImportCheck)
                    variables.importStrings();
            }
            if (chunks.used(0x19))
                commonEvents.importStrings(scriptDir + "\\Commons");
            if (chunks.used(0x1d))
            {
                M.loadStringScriptDatabase(scriptDir + "\\BattleSettings.txt", 0x1d);
                if (!M.stringScriptImportCheck)
                    battleSettings.importStrings();
            }
            if (chunks.used(0x1e))
            {
                M.loadStringScriptDatabase(scriptDir + "\\Classes.txt", 0x1e);
                if (!M.stringScriptImportCheck)
                    classes.importStrings();
            }
            if (chunks.used(0x20))
            {
                M.loadStringScriptDatabase(scriptDir + "\\BattlerAnimations.txt", 0x20);
                if (!M.stringScriptImportCheck)
                    battlerAnimations.importStrings();
            }
        }
        
        // Saves database file from stored data.
        public bool writeFile()
        {
            if (M.fileInUse(filepath))
            {
                Console.WriteLine(filename + " is in use; cannot save.");
                return false;
            }
            
            File.Delete(filepath + ".bak");
            File.Move(filepath, filepath + ".bak");
            BinaryWriter dbWriter = new BinaryWriter(new FileStream(filepath, FileMode.Create));
            M.targetWriter = dbWriter;
            
            try
            {
                M.writeString("LcfDataBase", M.S_CONSTANT);
                
                if (chunks.wasNext(0x0b))
                {
                    M.writeMultibyte(heroes.getLength());
                    heroes.write();
                }
                if (chunks.wasNext(0x0c))
                {
                    M.writeMultibyte(skills.getLength());
                    skills.write();
                }
                if (chunks.wasNext(0x0d))
                {
                    M.writeMultibyte(items.getLength());
                    items.write();
                }
                if (chunks.wasNext(0x0e))
                {
                    M.writeMultibyte(monsters.getLength());
                    monsters.write();
                }
                if (chunks.wasNext(0x0f))
                {
                    M.writeMultibyte(troops.getLength());
                    troops.write();
                }
                if (chunks.wasNext(0x10))
                {
                    M.writeMultibyte(terrains.getLength());
                    terrains.write();
                }
                if (chunks.wasNext(0x11))
                {
                    M.writeMultibyte(attributes.getLength());
                    attributes.write();
                }
                if (chunks.wasNext(0x12))
                {
                    M.writeMultibyte(conditions.getLength());
                    conditions.write();
                }
                if (chunks.wasNext(0x13))
                {
                    M.writeMultibyte(animations.getLength());
                    animations.write();
                }
                if (chunks.wasNext(0x14))
                {
                    M.writeMultibyte(chipSets.getLength());
                    chipSets.write();
                }
                if (chunks.wasNext(0x15))
                {
                    M.writeMultibyte(vocab.getLength());
                    vocab.write();
                }
                if (chunks.wasNext(0x16))
                {
                    M.writeMultibyte(system.getLength());
                    system.write();
                }
                if (chunks.wasNext(0x17))
                {
                    M.writeMultibyte(switches.getLength());
                    switches.write();
                }
                if (chunks.wasNext(0x18))
                {
                    M.writeMultibyte(variables.getLength());
                    variables.write();
                }
                if (chunks.wasNext(0x19))
                {
                    M.writeMultibyte(commonEvents.getLength());
                    commonEvents.write();
                }
                if (chunks.wasNext(0x1a))
                    M.writeMultibyte(version);
                
                if (chunks.wasNext(0x1b))
                    M.writeByte(0x00);
                if (chunks.wasNext(0x1c))
                    M.writeByte(0x00);
                
                if (chunks.wasNext(0x1d))
                {
                    M.writeMultibyte(battleSettings.getLength());
                    battleSettings.write();
                }
                if (chunks.wasNext(0x1e))
                {
                    M.writeMultibyte(classes.getLength());
                    classes.write();
                }
                
                if (chunks.wasNext(0x1f))
                    M.writeByte(0x00);
                
                if (chunks.wasNext(0x20))
                {
                    M.writeMultibyte(battlerAnimations.getLength());
                    battlerAnimations.write();
                }
                
                if (M.is2000EN)
                    M.writeByte(0x01);
                
                dbWriter.Close();
                M.targetWriter.Close();
                File.Delete(filepath + ".bak");
                
                chunks.validateParity();
            }
            catch (Exception e)
            {
                M.debugMessage(e.StackTrace);
                M.debugMessage(e.Message);
                Console.WriteLine("Aborting due to error; keeping original file.");
                
                // Close file and restore backup.
                dbWriter.Close();
                M.targetWriter.Close();
                File.Delete(filepath);
                File.Move(filepath + ".bak", filepath);
                return false;
            }
            
            return true;
        }
        
        // Returns an array of all switch names.
        public string[] getSwitchNames()
        {
            return switches.getSwitchNames();
        }
        
        // Returns an array of all variable names.
        public string[] getVariableNames()
        {
            return variables.getVariableNames();
        }
        
        // Updates switch names using given array.
        public bool setSwitchNames(string[] names)
        {
            return switches.setSwitchNames(names);
        }
        
        // Updates variable names using given array.
        public bool setVariableNames(string[] names)
        {
            return variables.setVariableNames(names);
        }
        
        // References for save file modifcation.
        public string getHeroDefaultName(int id)
        {
            return heroes.getHeroDefaultName(id);
        }
        public string getHeroDefaultTitle(int id)
        {
            return heroes.getHeroDefaultTitle(id);
        }
        public int getHeroBaseMaxHP(int id, int level)
        {
            return heroes.getHeroMaxHP(id, level);
        }
        public int getHeroBaseMaxMP(int id, int level)
        {
            return heroes.getHeroMaxMP(id, level);
        }
        public int getHeroEXPForLevel(int id, int level)
        {
            return heroes.getHeroEXPForLevel(id, level);
        }
        public List<HeroLearnSkill> getHeroLearnSkills(int id)
        {
            return heroes.getHeroLearnSkills(id);
        }
    }
}
