using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class SaveFile : RPGDataFile
    {
        string filepath = "";
        string filename = "";
        
        SaveTitle title; // 64
        SaveSystem system; // 65
        SaveScreen screen; // 66
        List<SavePicture> pictures; // 67
        SavePartyLocation partyLocation; // 68
        SaveVehicleLocation boatLocation; // 69
        SaveVehicleLocation shipLocation; // 6a
        SaveVehicleLocation airshipLocation; // 6b
        List<SaveHero> heroes; // 6c
        SaveInventory inventory; // 6d
        List<SaveTarget> targets; // 6e
        SaveMapInfo mapInfo; // 6f
        byte[] panoramaData; // 70
        SaveEventData eventData; // 71
        List<SaveCommonEvent> commonEvents; // 72
        
        static string myClass = "SaveFile";
        Chunks chunks;
        
        public Database db; // For reference purposes in modification
        
        public SaveFile(string filepath, bool writeLog = true)
        {
            loadFile(filepath, writeLog);
        }
        public SaveFile()
        {
        }
        
        public bool loadFile(string filepath, bool writeLog = true)
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Save file " + filepath + " not found.");
                return false;
            }
            
            this.filepath = filepath;
            filename = Path.GetFileName(filepath);
            M.currentFile = filename;
            
            Console.WriteLine(M.globalMode + " save file...");
            
            FileStream f = File.OpenRead(filepath);
            
            try
            {
                chunks = new Chunks(f, myClass);
                
                M.stringCheck(f, "LcfSaveData");
                
                if (chunks.next(0x64))
                {
                    M.readMultibyte(f); // Length
                    title = new SaveTitle(f);
                }
                if (chunks.next(0x65))
                {
                    M.readMultibyte(f); // Length
                    system = new SaveSystem(f);
                }
                if (chunks.next(0x66))
                {
                    M.readMultibyte(f); // Length
                    screen = new SaveScreen(f);
                }
                if (chunks.next(0x67))
                    pictures = M.readList<SavePicture>(f);
                if (chunks.next(0x68))
                {
                    M.readMultibyte(f); // Length
                    partyLocation = new SavePartyLocation(f);
                }
                if (chunks.next(0x69))
                {
                    M.readMultibyte(f); // Length
                    boatLocation = new SaveVehicleLocation(f);
                }
                if (chunks.next(0x6a))
                {
                    M.readMultibyte(f); // Length
                    shipLocation = new SaveVehicleLocation(f);
                }
                if (chunks.next(0x6b))
                {
                    M.readMultibyte(f); // Length
                    airshipLocation = new SaveVehicleLocation(f);
                }
                if (chunks.next(0x6c))
                    heroes = M.readList<SaveHero>(f);
                if (chunks.next(0x6d))
                {
                    M.readMultibyte(f); // Length
                    inventory = new SaveInventory(f);
                }
                if (chunks.next(0x6e))
                    targets = M.readList<SaveTarget>(f);
                if (chunks.next(0x6f))
                {
                    M.readMultibyte(f); // Length
                    mapInfo = new SaveMapInfo(f);
                }
                if (chunks.next(0x70))
                    panoramaData = M.skipLengthBytes(f);
                if (chunks.next(0x71))
                {
                    M.readMultibyte(f); // Length
                    eventData = new SaveEventData(f);
                }
                if (chunks.next(0x72))
                    commonEvents = M.readList<SaveCommonEvent>(f);
                
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
            
            if (writeLog)
                M.logSave();
            return true;
        }
        
        public string getString()
        {
            StringWriter saveText = new StringWriter(new StringBuilder());
            
            if (title != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Title >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(title.getString());
                saveText.WriteLine();
            }
            if (system != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< System >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(system.getString());
                saveText.WriteLine();
            }
            if (screen != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Screen >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(screen.getString());
                saveText.WriteLine();
            }
            if (pictures != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Pictures >>>>>>>>>>>>>>>>>>>>");
                foreach (SavePicture picture in pictures)
                {
                    string result = picture.getString();
                    if (result != "")
                        saveText.Write(result);
                }
                saveText.WriteLine();
            }
            if (partyLocation != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Party Location >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(partyLocation.getString());
                saveText.WriteLine();
            }
            if (boatLocation != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Boat Location >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(boatLocation.getString());
                saveText.WriteLine();
            }
            if (shipLocation != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Ship Location >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(shipLocation.getString());
                saveText.WriteLine();
            }
            if (airshipLocation != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Airship Location >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(airshipLocation.getString());
                saveText.WriteLine();
            }
            if (heroes != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Heroes >>>>>>>>>>>>>>>>>>>>");
                foreach (SaveHero hero in heroes)
                    saveText.WriteLine(hero.getString());
                saveText.WriteLine();
            }
            if (inventory != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Inventory >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(inventory.getString());
                saveText.WriteLine();
            }
            if (targets != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Targets >>>>>>>>>>>>>>>>>>>>");
                foreach (SaveTarget target in targets)
                    saveText.WriteLine(target.getString());
            }
            if (mapInfo != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Map Info >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(mapInfo.getString());
            }
            if (eventData != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Events >>>>>>>>>>>>>>>>>>>>");
                saveText.WriteLine(eventData.getString());
            }
            if (commonEvents != null)
            {
                saveText.WriteLine("<<<<<<<<<<<<<<<<<<<< Common Events >>>>>>>>>>>>>>>>>>>>");
                foreach (SaveCommonEvent common in commonEvents)
                {
                    string result = common.getString();
                    if (result != "")
                        saveText.WriteLine(result);
                }
            }
            
            return saveText.ToString();
        }
        
        public bool writeFile()
        {
            if (M.fileInUse(filepath))
            {
                Console.WriteLine(filename + " is in use; cannot save.");
                return false;
            }
            
            File.Delete(filepath + ".bak");
            File.Move(filepath, filepath + ".bak");
            BinaryWriter mapTreeWriter = new BinaryWriter(new FileStream(filepath, FileMode.Create));
            M.targetWriter = mapTreeWriter;
            
            try
            {
                M.writeString("LcfSaveData", M.S_CONSTANT);
                
                if (chunks.wasNext(0x64))
                {
                    M.writeMultibyte(title.getLength());
                    title.write();
                }
                if (chunks.wasNext(0x65))
                {
                    M.writeMultibyte(system.getLength());
                    system.write();
                }
                if (chunks.wasNext(0x66))
                {
                    M.writeMultibyte(screen.getLength());
                    screen.write();
                }
                if (chunks.wasNext(0x67))
                    M.writeList<SavePicture>(pictures);
                if (chunks.wasNext(0x68))
                {
                    M.writeMultibyte(partyLocation.getLength());
                    partyLocation.write();
                }
                if (chunks.wasNext(0x69))
                {
                    M.writeMultibyte(boatLocation.getLength());
                    boatLocation.write();
                }
                if (chunks.wasNext(0x6a))
                {
                    M.writeMultibyte(shipLocation.getLength());
                    shipLocation.write();
                }
                if (chunks.wasNext(0x6b))
                {
                    M.writeMultibyte(airshipLocation.getLength());
                    airshipLocation.write();
                }
                if (chunks.wasNext(0x6c))
                    M.writeList<SaveHero>(heroes);
                if (chunks.wasNext(0x6d))
                {
                    M.writeMultibyte(inventory.getLength());
                    inventory.write();
                }
                if (chunks.wasNext(0x6e))
                    M.writeList<SaveTarget>(targets);
                if (chunks.wasNext(0x6f))
                {
                    M.writeMultibyte(mapInfo.getLength());
                    mapInfo.write();
                }
                if (chunks.wasNext(0x70))
                {
                    M.writeMultibyte(panoramaData.Length);
                    M.writeByteArrayNoLength(panoramaData);
                }
                if (chunks.wasNext(0x71))
                {
                    M.writeMultibyte(eventData.getLength());
                    eventData.write();
                }
                if (chunks.wasNext(0x72))
                    M.writeList<SaveCommonEvent>(commonEvents);
                
                mapTreeWriter.Close();
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
                mapTreeWriter.Close();
                M.targetWriter.Close();
                File.Delete(filepath);
                File.Move(filepath + ".bak", filepath);
                return false;
            }
            
            return true;
        }
        
        public void modifySubmenu()
        {
            M.saveRef = this;
            if (File.Exists(M.gamePath + "\\RPG_RT.ldb"))
                db = new Database(M.gamePath + "\\RPG_RT.ldb", false);
            
            bool menuShow = true;
            string menuOption;
            while (menuShow)
            {
                Console.Clear();
                Console.WriteLine("Save Modification Main Menu");
                Console.WriteLine("1. Edit Switch");
                Console.WriteLine("2. Edit Variable");
                Console.WriteLine("3. Edit Party Location");
                Console.WriteLine("4. Edit Vehicle Location");
                Console.WriteLine("5. Edit Heroes");
                Console.WriteLine("6. Edit Party Lineup");
                Console.WriteLine("7. Edit Inventory");
                Console.WriteLine("8. Edit Money");
                Console.WriteLine("9. Replace Filenames");
                if (chunks.used(0x71) || chunks.used(0x72))
                    Console.WriteLine("0. Clear Active Events");
                
                if (!M.changesMade)
                    Console.WriteLine("Z. Cancel");
                else
                {
                    Console.WriteLine("Z. Save Changes");
                    Console.WriteLine("X. Cancel");
                }
                
                menuOption = Console.ReadLine().ToUpper();
                switch (menuOption)
                {
                    case "1":
                        system.modifySwitch();
                        break;
                    case "2":
                        system.modifyVariable();
                        break;
                    case "3":
                        partyLocation.modify();
                        break;
                    case "4":
                        Console.WriteLine("A. Edit Boat Location");
                        Console.WriteLine("B. Edit Ship Location");
                        Console.WriteLine("C. Edit Airship Location");
                        string vehicle = Console.ReadLine().ToUpper();
                        switch (vehicle)
                        {
                            case "A": boatLocation.modify(); break;
                            case "B": shipLocation.modify(); break;
                            case "C": airshipLocation.modify(); break;
                        }
                        break;
                    case "5":
                        Console.WriteLine("1 to " + M.heroNames.Length + ". Edit Hero X");
                        Console.WriteLine("Z. Complete HP/MP Recovery");
                        string heroOption = Console.ReadLine();
                        int heroID;
                        if (heroOption.ToLower().Equals("z"))
                        {
                            if (heroes != null)
                                foreach (SaveHero hero in heroes)
                                    hero.fullRecovery();
                            Console.WriteLine("All heroes fully recovered.");
                            M.enterToContinue();
                        }
                        else if (int.TryParse(heroOption, out heroID))
                            tryToSwitchHeroEdit(heroID);
                        break;
                    case "6":
                        inventory.modifyParty();
                        break;
                    case "7":
                        inventory.modifyInventory();
                        break;
                    case "8":
                        inventory.modifyMoney();
                        break;
                    case "9":
                        replaceFilenames();
                        break;
                    case "0":
                        if (chunks.used(0x71) || chunks.used(0x72))
                        {
                            Console.WriteLine("Clear active events from save data, forcing them to refresh? (Y/N)");
                            if (M.yesNoPrompt(true))
                            {
                                M.changesMade = true;
                                chunks.remove(0x71);
                                chunks.remove(0x72);
                                eventData = null;
                                commonEvents.Clear();
                                Console.WriteLine("All active events/common events removed from stack.");
                                M.enterToContinue();
                            }
                        }
                        break;
                    case "Z":
                        menuShow = false;
                        break;
                    case "X":
                        M.changesMade = false;
                        menuShow = false;
                        break;
                }
            }
        }
        
        void replaceFilenames()
        {
            if (M.lastInputFile.Equals("N/A"))
            {
                Console.WriteLine("No replacement list has been loaded.");
                Console.WriteLine("To rewrite filenames in save, go back and load a replacement list.");
                M.enterToContinue();
                return;
            }
            
            Console.WriteLine("Replace all filename references in save using " + M.lastInputFile + "? (Y/N)");
            if (M.yesNoPrompt(true))
            {
                bool changesMadeOld = M.changesMade;
                M.changesMade = false;
                
                title.replaceFilenames();
                system.replaceFilenames();
                foreach (SavePicture picture in pictures)
                    picture.replaceFilenames();
                partyLocation.replaceFilenames();
                boatLocation.replaceFilenames();
                shipLocation.replaceFilenames();
                airshipLocation.replaceFilenames();
                foreach (SaveHero hero in heroes)
                    hero.replaceFilenames();
                mapInfo.replaceFilenames();
                eventData.replaceFilenames();
                foreach (SaveCommonEvent commonEvent in commonEvents)
                    commonEvent.replaceFilenames();
                
                if (M.changesMade)
                    Console.WriteLine("Filenames replaced.");
                else
                {
                    Console.WriteLine("No filenames needed replacing.");
                    M.changesMade = changesMadeOld;
                }
                M.enterToContinue();
            }
        }
        
        public bool tryToSwitchHeroEdit(int heroID)
        {
            if (heroID >= 1 && heroID <= M.heroNames.Length)
            {
                if (heroes != null && heroID <= heroes.Count)
                {
                    heroes[heroID - 1].modify();
                    return true;
                }
                else
                {
                    Console.WriteLine("Error: Hero does not exist in save.");
                    M.enterToContinue();
                }
            }
            else
            {
                Console.WriteLine("Hero number is out of range (1 to " + M.heroNames.Length + ").");
                M.enterToContinue();
            }
            return false;
        }
        
        public void updateFileHP(int heroID, int hp)
        {
            if (inventory.getPartyLeader() == heroID)
                title.setMainHeroHP(hp);
        }
        public void updateFileName(int heroID, string name)
        {
            if (inventory.getPartyLeader() == heroID)
                title.setMainHeroName(name);
        }
        public void updateFileTitle(int heroID, string name)
        {
            if (inventory.getPartyLeader() == heroID)
                title.setMainHeroTitle(name);
        }
        public void updateFileForNewPartyLead()
        {
            int leader = inventory.getPartyLeader();
            if (leader < heroes.Count)
            {
                title.setMainHeroHP(heroes[leader].getCurrentHP());
                string argName = heroes[leader].getName();
                string argTitle = heroes[leader].getTitle();
                title.setMainHeroName(argName != "\x01"? argName : db.getHeroDefaultName(leader));
                title.setMainHeroTitle(argTitle != "\x01"? argTitle : db.getHeroDefaultTitle(leader));
            }
        }
    }
    
    class SaveTitle : RPGByteData
    {
        double timestamp = 0; // 01
        string mainHeroName = ""; // 0b
        string mainHeroTitle = ""; // 0c
        int mainHeroHP = 0; // 0d
        string faceSet1 = ""; // 15
        int faceIndex1 = 0; // 16
        string faceSet2 = ""; // 17
        int faceIndex2 = 0; // 18
        string faceSet3 = ""; // 19
        int faceIndex3 = 0; // 1a
        string faceSet4 = ""; // 1b
        int faceIndex4 = 0; // 1c
        
        static string myClass = "SaveTitle";
        Chunks chunks;
        
        public SaveTitle(FileStream f)
        {
            load(f);
        }
        public SaveTitle()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
                timestamp = M.readLengthDouble(f);
            if (chunks.next(0x0b))
                mainHeroName = M.readString(f, M.S_TOTRANSLATE);
            if (chunks.next(0x0c))
                mainHeroTitle = M.readString(f, M.S_TOTRANSLATE);
            if (chunks.next(0x0d))
                mainHeroHP = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                faceSet1 = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x16))
                faceIndex1 = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                faceSet2 = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x18))
                faceIndex2 = M.readLengthMultibyte(f);
            if (chunks.next(0x19))
                faceSet3 = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x1a))
                faceIndex3 = M.readLengthMultibyte(f);
            if (chunks.next(0x1b))
                faceSet4 = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x1c))
                faceIndex4 = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            DateTime saveDate = DateTime.FromOADate(timestamp);
            
            str.WriteLine("Timestamp: " + saveDate.ToLongDateString() + ", " + saveDate.ToLongTimeString());
            str.WriteLine("Main Hero Name: " + M.x01Default(mainHeroName));
            str.WriteLine("Main Hero Title: " + M.x01Default(mainHeroTitle));
            str.WriteLine("Main Hero HP: " + mainHeroHP);
            
            if (faceSet1 != "")
                str.WriteLine("Face Graphic 1: " + faceSet1 + ", " + (faceIndex1 + 1));
            if (faceSet2 != "")
                str.WriteLine("Face Graphic 2: " + faceSet2 + ", " + (faceIndex2 + 1));
            if (faceSet3 != "")
                str.WriteLine("Face Graphic 3: " + faceSet3 + ", " + (faceIndex3 + 1));
            if (faceSet4 != "")
                str.WriteLine("Face Graphic 4: " + faceSet4 + ", " + (faceIndex4 + 1));
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeLengthDouble(timestamp);
            if (chunks.wasNext(0x0b))
                M.writeString(mainHeroName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x0c))
                M.writeString(mainHeroTitle, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(mainHeroHP);
            if (chunks.wasNext(0x15))
                M.writeString(faceSet1, M.S_FILENAME);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(faceIndex1);
            if (chunks.wasNext(0x17))
                M.writeString(faceSet2, M.S_FILENAME);
            if (chunks.wasNext(0x18))
                M.writeLengthMultibyte(faceIndex2);
            if (chunks.wasNext(0x19))
                M.writeString(faceSet3, M.S_FILENAME);
            if (chunks.wasNext(0x1a))
                M.writeLengthMultibyte(faceIndex3);
            if (chunks.wasNext(0x1b))
                M.writeString(faceSet4, M.S_FILENAME);
            if (chunks.wasNext(0x1c))
                M.writeLengthMultibyte(faceIndex4);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void setMainHeroName(string name)
        {
            if (mainHeroName != name)
                M.changesMade = true;
            mainHeroName = name;
        }
        public void setMainHeroTitle(string name)
        {
            if (mainHeroTitle != name)
                M.changesMade = true;
            mainHeroTitle = name;
        }
        public void setMainHeroHP(int value)
        {
            if (mainHeroHP != value)
                M.changesMade = true;
            mainHeroHP = value;
        }
        
        public void replaceFilenames()
        {
            faceSet1 = M.rewriteString(M.M_FACESET, faceSet1);
            faceSet2 = M.rewriteString(M.M_FACESET, faceSet2);
            faceSet3 = M.rewriteString(M.M_FACESET, faceSet3);
            faceSet4 = M.rewriteString(M.M_FACESET, faceSet4);
        }
    }
    
    class SaveSystem : RPGByteData
    {
        int screen = 0; // 01, seems to always be 5 (except games started in EasyRPG save 0)
        int frameCount = 0; // 0b
        string systemGraphic = "[Default]"; // 15
        int systemGraphicTile = 0; // 16
        int systemFont = 0; // 17
        int switchLength = 0; // 1f
        bool[] switches; // 20
        int variableLength = 0; // 21
        long[] variables; // 22
        bool messageTransparent = false; // 29
        int messagePosition = 2; // 2a
        bool messageDontCoverHero = true; // 2b
        bool messageContinueEvents = false; // 2c
        string faceName = ""; // 33
        int faceIndex = 0; // 34
        bool faceRight = false; // 35
        bool faceFlip = false; // 36
        bool transparent = false; // 37
        int musicFadeOut = 0; // 3d
        List<Audio> audios; // 47 through 52 (music), 5b through 66 (sounds)
        int teleportErase = 0; // 6f
        int teleportShow = 0; // 70
        int battleStartErase = 0; // 71
        int battleStartShow = 0; // 72
        int battleEndErase = 0; // 73
        int battleEndShow = 0; // 74
        bool teleportAllowed = false; // 79
        bool escapeAllowed = false; // 7a
        bool saveAllowed = false; // 7b
        bool menuAllowed = false; // 7c
        string background = ""; // 7d
        int saveCount = 0; // 83
        int saveSlot = 0; // 84
        int atbMode = 0; // 8c
        
        static string myClass = "SaveSystem";
        Chunks chunks;
        
        static string[] messagePositions = { "Top", "Middle", "Bottom" };
        static string[] audioNames = { "Title Screen", "Battle", "Battle End", "Inn",
                                       "Current Song", "Pre-Vehicle Song", "Pre-Battle Song", "Memorized Song",
                                       "Boat", "Ship", "Airship", "Game Over",
                                       // 0x53 through 5a
                                       "Cursor Move", "Confirm", "Cancel", "Buzz",
                                       "Battle Start", "Escape", "Enemy Attack", "Damage Enemy",
                                       "Damage Ally", "Evade", "Enemy Die", "Use Item" };
        
        public SaveSystem(FileStream f)
        {
            load(f);
        }
        public SaveSystem()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
                screen = M.readLengthMultibyte(f);
            if (chunks.next(0x0b))
                frameCount = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                systemGraphic = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x16))
                systemGraphicTile = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                systemFont = M.readLengthMultibyte(f);
            if (chunks.next(0x1f))
                switchLength = M.readLengthMultibyte(f);
            if (chunks.next(0x20))
                switches = M.readBoolArray(f);
            if (chunks.next(0x21))
                variableLength = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                variables = M.readFourByteArray(f);
            if (chunks.next(0x29))
                messageTransparent = M.readLengthBool(f);
            if (chunks.next(0x2a))
                messagePosition = M.readLengthMultibyte(f);
            if (chunks.next(0x2b))
                messageDontCoverHero = M.readLengthBool(f);
            if (chunks.next(0x2c))
                messageContinueEvents = M.readLengthBool(f);
            if (chunks.next(0x33))
                faceName = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x34))
                faceIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x35))
                faceRight = M.readLengthBool(f);
            if (chunks.next(0x36))
                faceFlip = M.readLengthBool(f);
            if (chunks.next(0x37))
                transparent = M.readLengthBool(f);
            if (chunks.next(0x3d))
                musicFadeOut = M.readLengthMultibyte(f);
            
            audios = new List<Audio>();
            for (byte i = 0x47; i <= 0x66; i++) // Music (0x47 to 0x52) and Sounds (0x5b to 0x66)
            {
                if (i > 0x52 && i < 0x5b) // Not music nor sounds
                    continue;
                
                if (chunks.next(i))
                {
                    Audio audio = new Audio(f, i < 0x52);
                    audios.Add(audio);
                }
            }
            
            if (chunks.next(0x6f))
                teleportErase = M.readLengthBytes(f);
            if (chunks.next(0x70))
                teleportShow = M.readLengthBytes(f);
            if (chunks.next(0x71))
                battleStartErase = M.readLengthBytes(f);
            if (chunks.next(0x72))
                battleStartShow = M.readLengthBytes(f);
            if (chunks.next(0x73))
                battleEndErase = M.readLengthBytes(f);
            if (chunks.next(0x74))
                battleEndShow = M.readLengthBytes(f);
            if (chunks.next(0x79))
                teleportAllowed = M.readLengthBool(f);
            if (chunks.next(0x7a))
                escapeAllowed = M.readLengthBool(f);
            if (chunks.next(0x7b))
                saveAllowed = M.readLengthBool(f);
            if (chunks.next(0x7c))
                menuAllowed = M.readLengthBool(f);
            if (chunks.next(0x7d))
                background = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x83))
                saveCount = M.readLengthMultibyte(f);
            if (chunks.next(0x84))
                saveSlot = M.readLengthMultibyte(f);
            if (chunks.next(0x8c))
                atbMode = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Screen (Always 5): " + screen);
            str.WriteLine("Frame Count: " + frameCount + " (" + M.framesToTime(frameCount) + ")");
            str.WriteLine();
            
            str.WriteLine("System Graphic: " + systemGraphic + ", "
                + (systemGraphicTile == 0? "Stretch" : "Tile") + ", Font " + (systemFont + 1));
            str.WriteLine();
            
            str.WriteLine("Switches:");
            for (int i = 0; i < switches.Length; i++)
                str.WriteLine(M.getDataSwitch(i + 1) + " " + (switches[i]? "On" : "Off"));
            str.WriteLine();
            
            str.WriteLine("Variables:");
            for (int i = 0; i < variables.Length; i++)
                str.WriteLine(M.getDataVariable(i + 1) + " " + variables[i]);
            str.WriteLine();
            
            str.WriteLine("Message Transparent: " + messageTransparent);
            str.WriteLine("Message Position: " + messagePositions[messagePosition]);
            str.WriteLine("Message Avoid Covering Hero: " + messageDontCoverHero);
            str.WriteLine("Message Continue Events: " + messageContinueEvents);
            if (faceName != "")
                str.WriteLine("Message Face: " + faceName + ", " + (faceIndex + 1)
                    + (faceRight? " (Right)" : "") + (faceFlip? " (Flip)" : "") + (transparent? " (Transparent)" : ""));
            str.WriteLine();
            
            int index = 0;
            for (int i = 0; i < audios.Count; i++)
                str.WriteLine(audioNames[index++] + ": " + audios[i].getString());
            str.WriteLine("Music Fade Out: " + musicFadeOut);
            str.WriteLine();
            
            str.WriteLine("Teleport Erase: " + M.getEraseEffects(teleportErase));
            str.WriteLine("Teleport Show: " + M.getShowEffects(teleportShow));
            str.WriteLine("Battle Start Erase: " + M.getEraseEffects(battleStartErase));
            str.WriteLine("Battle Start Show: " + M.getShowEffects(battleStartShow));
            str.WriteLine("Battle End Erase: " + M.getEraseEffects(battleEndErase));
            str.WriteLine("Battle End Show: " + M.getShowEffects(battleEndShow));
            str.WriteLine();
            
            str.WriteLine("Teleport Allowed: " + teleportAllowed);
            str.WriteLine("Escape Allowed: " + escapeAllowed);
            str.WriteLine("Save Allowed: " + saveAllowed);
            str.WriteLine("Menu Allowed: " + menuAllowed);
            str.WriteLine();
            
            str.WriteLine("Background: " + (background != ""? background : "(None)"));
            str.WriteLine("Save Count: " + saveCount);
            str.WriteLine("Save Slot: " + (saveSlot + 1));
            if (M.is2003)
                str.WriteLine("ATB Mode: " + atbMode);
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(screen);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(frameCount);
            if (chunks.wasNext(0x15))
                M.writeString(systemGraphic, M.S_FILENAME);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(systemGraphicTile);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(systemFont);
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(switchLength);
            if (chunks.wasNext(0x20))
                M.writeBoolArray(switches);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(variableLength);
            if (chunks.wasNext(0x22))
                M.writeFourByteArray(variables);
            if (chunks.wasNext(0x29))
                M.writeLengthBool(messageTransparent);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(messagePosition);
            if (chunks.wasNext(0x2b))
                M.writeLengthBool(messageDontCoverHero);
            if (chunks.wasNext(0x2c))
                M.writeLengthBool(messageContinueEvents);
            if (chunks.wasNext(0x33))
                M.writeString(faceName, M.S_FILENAME);
            if (chunks.wasNext(0x34))
                M.writeLengthMultibyte(faceIndex);
            if (chunks.wasNext(0x35))
                M.writeLengthBool(faceRight);
            if (chunks.wasNext(0x36))
                M.writeLengthBool(faceFlip);
            if (chunks.wasNext(0x37))
                M.writeLengthBool(transparent);
            if (chunks.wasNext(0x3d))
                M.writeLengthMultibyte(musicFadeOut);
            
            int index = 0;
            for (byte i = 0x47; i <= 0x66; i++) // Music (0x47 to 0x52) and Sounds (0x5b to 0x66)
            {
                if (i > 0x52 && i < 0x5b) // Not music nor sounds
                    continue;
                
                if (chunks.wasNext(i))
                {
                    Audio audio = audios[index++];
                    audio.write();
                }
            }
            
            if (chunks.wasNext(0x6f))
                M.writeLengthBytes(teleportErase);
            if (chunks.wasNext(0x70))
                M.writeLengthBytes(teleportShow);
            if (chunks.wasNext(0x71))
                M.writeLengthBytes(battleStartErase);
            if (chunks.wasNext(0x72))
                M.writeLengthBytes(battleStartShow);
            if (chunks.wasNext(0x73))
                M.writeLengthBytes(battleEndErase);
            if (chunks.wasNext(0x74))
                M.writeLengthBytes(battleEndShow);
            if (chunks.wasNext(0x79))
                M.writeLengthBool(teleportAllowed);
            if (chunks.wasNext(0x7a))
                M.writeLengthBool(escapeAllowed);
            if (chunks.wasNext(0x7b))
                M.writeLengthBool(saveAllowed);
            if (chunks.wasNext(0x7c))
                M.writeLengthBool(menuAllowed);
            if (chunks.wasNext(0x7d))
                M.writeString(background, M.S_FILENAME);
            if (chunks.wasNext(0x83))
                M.writeLengthMultibyte(saveCount);
            if (chunks.wasNext(0x84))
                M.writeLengthMultibyte(saveSlot);
            if (chunks.wasNext(0x8c))
                M.writeLengthMultibyte(atbMode);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void modifySwitch()
        {
            Console.WriteLine("Input switch number.");
            string switchStr = Console.ReadLine();
            int switchID;
            if (int.TryParse(switchStr, out switchID))
            {
                if (switchID >= 1 && switchID <= 5000)
                {
                    Console.WriteLine("Current value of " + M.getDataSwitch(switchID) + ": " + M.onOff(getSwitch(switchID)));
                    Console.WriteLine("Turn it on (1) or off (0)?");
                    string valueStr = Console.ReadLine();
                    if (valueStr.Equals("1"))
                        setSwitch(switchID, true);
                    else if (valueStr.Equals("0"))
                        setSwitch(switchID, false);
                    else
                    {
                        Console.WriteLine("Kept value the same.");
                        M.enterToContinue();
                    }
                }
                else
                {
                    Console.WriteLine("Switch number is out of range (1 to " + M.switchNames.Length + ").");
                    M.enterToContinue();
                }
            }
        }
        public void modifyVariable()
        {
            Console.WriteLine("Input variable number.");
            string variableStr = Console.ReadLine();
            int variableID;
            if (int.TryParse(variableStr, out variableID))
            {
                if (variableID >= 1 && variableID <= 5000)
                {
                    Console.WriteLine("Current value of " + M.getDataVariable(variableID) + ": " + getVariable(variableID));
                    Console.WriteLine("Please input new value.");
                    string valueStr = Console.ReadLine();
                    int value;
                    if (int.TryParse(valueStr, out value))
                    {
                        if (value >= -999999 && value <= 999999)
                            setVariable(variableID, value);
                        else
                            Console.WriteLine("Value is out of range (-999999 to 999999).");
                    }
                }
                else
                {
                    Console.WriteLine("Variable number is out of range (1 to " + M.variableNames.Length + ").");
                    M.enterToContinue();
                }
            }
        }
        
        bool getSwitch(int id)
        {
            if (id - 1 >= switches.Length)
                return false;
            
            return switches[id - 1];
        }
        int getVariable(int id)
        {
            if (id - 1 >= variables.Length)
                return 0;
            
            return (int)variables[id - 1];
        }
        
        void setSwitch(int id, bool value)
        {
            if (id - 1 >= switches.Length)
            {
                bool[] newSwitches = new bool[id];
                switches.CopyTo(newSwitches, 0);
                switches = newSwitches;
                M.changesMade = true;
            }
            
            if (switches[id - 1] != value)
                M.changesMade = true;
            switches[id - 1] = value;
        }
        void setVariable(int id, int value)
        {
            if (id - 1 >= variables.Length)
            {
                long[] newVariables = new long[id];
                variables.CopyTo(newVariables, 0);
                variables = newVariables;
            }
            
            if (variables[id - 1] != value)
                M.changesMade = true;
            variables[id - 1] = value;
        }
        
        public void replaceFilenames()
        {
            systemGraphic = M.rewriteString(M.M_SYSTEM, systemGraphic);
            foreach (Audio audio in audios)
                audio.replaceFilenames();
            faceName = M.rewriteString(M.M_FACESET, faceName);
            background = M.rewriteString(M.M_PANORAMA, background);
        }
    }
    
    class SaveScreen : RPGByteData
    {
        int tintFinishRed = 0; // 01
        int tintFinishGreen = 0; // 02
        int tintFinishBlue = 0; // 03
        int tintFinishSat = 0; // 04
        double tintCurrentRed = 0; // 0b
        double tintCurrentGreen = 0; // 0c
        double tintCurrentBlue = 0; // 0d
        double tintCurrentSat = 0; // 0e
        int tintTimeLeft = 0; // 0f
        int flashContinuous = 0; // 14
        int flashRed = 0; // 15
        int flashGreen = 0; // 16
        int flashBlue = 0; // 17
        double flashCurrentLevel = 0; // 18
        int flashTimeLeft = 0; // 19
        int shakeContinuous = 0; // 1e
        int shakeStrength = 0; // 1f
        int shakeSpeed = 0; // 20
        int shakePosition = 0; // 21
        int shakePositionY = 0; // 22, unused
        int shakeTimeLeft = 0; // 23
        int panX = 0; // 29
        int panY = 0; // 2a
        int animationID = 0; // 2b
        int animationTarget = 0; // 2c
        int animationFrame = 0; // 2d
        bool animationActive = false; // 2e
        int animationGlobalScope = 0; // 2f
        int weather = 0; // 30
        int weatherStrength = 0; // 31
        
        static string myClass = "SaveScreen";
        Chunks chunks;
        
        static string[] weathers = { "None", "Rain", "Snow", "Fog", "Sandstorm" };
        
        public SaveScreen(FileStream f)
        {
            load(f);
        }
        public SaveScreen()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
                tintFinishRed = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
                tintFinishGreen = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                tintFinishBlue = M.readLengthMultibyte(f);
            if (chunks.next(0x04))
                tintFinishSat = M.readLengthMultibyte(f);
            if (chunks.next(0x0b))
                tintCurrentRed = M.readLengthDouble(f);
            if (chunks.next(0x0c))
                tintCurrentGreen = M.readLengthDouble(f);
            if (chunks.next(0x0d))
                tintCurrentBlue = M.readLengthDouble(f);
            if (chunks.next(0x0e))
                tintCurrentSat = M.readLengthDouble(f);
            if (chunks.next(0x0f))
                tintTimeLeft = M.readLengthMultibyte(f);
            if (chunks.next(0x14))
                flashContinuous = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                flashRed = M.readLengthMultibyte(f);
            if (chunks.next(0x16))
                flashGreen = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                flashBlue = M.readLengthMultibyte(f);
            if (chunks.next(0x18))
                flashCurrentLevel = M.readLengthDouble(f);
            if (chunks.next(0x19))
                flashTimeLeft = M.readLengthMultibyte(f);
            if (chunks.next(0x1e))
                shakeContinuous = M.readLengthMultibyte(f);
            if (chunks.next(0x1f))
                shakeStrength = M.readLengthMultibyte(f);
            if (chunks.next(0x20))
                shakeSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x21))
                shakePosition = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                shakePositionY = M.readLengthMultibyte(f);
            if (chunks.next(0x23))
                shakeTimeLeft = M.readLengthMultibyte(f);
            if (chunks.next(0x29))
                panX = M.readLengthMultibyte(f);
            if (chunks.next(0x2a))
                panY = M.readLengthMultibyte(f);
            if (chunks.next(0x2b))
                animationID = M.readLengthMultibyte(f);
            if (chunks.next(0x2c))
                animationTarget = M.readLengthMultibyte(f);
            if (chunks.next(0x2d))
                animationFrame = M.readLengthMultibyte(f);
            if (chunks.next(0x2e))
                animationActive = M.readLengthBool(f);
            if (chunks.next(0x2f))
                animationGlobalScope = M.readLengthMultibyte(f);
            if (chunks.next(0x30))
                weather = M.readLengthMultibyte(f);
            if (chunks.next(0x31))
                weatherStrength = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Current Tint: R" + tintCurrentRed + " G" + tintCurrentGreen + " B" + tintCurrentBlue + " S" + tintCurrentSat);
            str.WriteLine("Destination Tint: R" + tintFinishRed + " G" + tintFinishGreen + " B" + tintFinishBlue + " S" + tintFinishSat);
            str.WriteLine("Tint Time Left: " + tintTimeLeft);
            str.WriteLine();
            
            str.WriteLine("Flash Color: R" + flashRed + " G" + flashGreen + " B" + flashBlue);
            str.WriteLine("Flash Current Level: " + flashCurrentLevel);
            str.WriteLine("Flash Time Left: " + flashTimeLeft);
            str.WriteLine("Flash Continuous: " + flashContinuous);
            str.WriteLine();
            
            str.WriteLine("Shake Strength: " + shakeStrength);
            str.WriteLine("Shake Speed: " + shakeSpeed);
            str.WriteLine("Shake Position: " + shakePosition);
            str.WriteLine("Shake Time Left: " + shakeTimeLeft);
            str.WriteLine("Shake Continuous: " + shakeContinuous);
            str.WriteLine();
            
            str.WriteLine("Pan X: " + panX);
            str.WriteLine("Pan Y: " + panY);
            str.WriteLine();
            
            if (animationActive)
            {
                str.WriteLine("Animation: " + (animationID != 0? M.getDataAnimation(animationID) : "(None)"));
                str.WriteLine("Animation Frame: " + (animationFrame + 1));
                str.WriteLine("Animation Target: " + (animationTarget != 0? M.getTargetEvent(animationTarget) : "(None)"));
                str.WriteLine("Animation Global Scope: " + (animationGlobalScope == 0? "Event" : "Screen"));
                str.WriteLine();
            }
            
            str.WriteLine("Weather: " + weathers[weather]);
            str.WriteLine("Weather Strength: " + weatherStrength);
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(tintFinishRed);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(tintFinishGreen);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(tintFinishBlue);
            if (chunks.wasNext(0x04))
                M.writeLengthMultibyte(tintFinishSat);
            if (chunks.wasNext(0x0b))
                M.writeLengthDouble(tintCurrentRed);
            if (chunks.wasNext(0x0c))
                M.writeLengthDouble(tintCurrentGreen);
            if (chunks.wasNext(0x0d))
                M.writeLengthDouble(tintCurrentBlue);
            if (chunks.wasNext(0x0e))
                M.writeLengthDouble(tintCurrentSat);
            if (chunks.wasNext(0x0f))
                M.writeLengthMultibyte(tintTimeLeft);
            if (chunks.wasNext(0x14))
                M.writeLengthMultibyte(flashContinuous);
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(flashRed);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(flashGreen);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(flashBlue);
            if (chunks.wasNext(0x18))
                M.writeLengthDouble(flashCurrentLevel);
            if (chunks.wasNext(0x19))
                M.writeLengthMultibyte(flashTimeLeft);
            if (chunks.wasNext(0x1e))
                M.writeLengthMultibyte(shakeContinuous);
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(shakeStrength);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(shakeSpeed);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(shakePosition);
            if (chunks.wasNext(0x22))
                M.writeLengthMultibyte(shakePositionY);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(shakeTimeLeft);
            if (chunks.wasNext(0x29))
                M.writeLengthMultibyte(panX);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(panY);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(animationID);
            if (chunks.wasNext(0x2c))
                M.writeLengthMultibyte(animationTarget);
            if (chunks.wasNext(0x2d))
                M.writeLengthMultibyte(animationFrame);
            if (chunks.wasNext(0x2e))
                M.writeLengthBool(animationActive);
            if (chunks.wasNext(0x2f))
                M.writeLengthMultibyte(animationGlobalScope);
            if (chunks.wasNext(0x30))
                M.writeLengthMultibyte(weather);
            if (chunks.wasNext(0x31))
                M.writeLengthMultibyte(weatherStrength);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
    
    class SavePicture : RPGByteData
    {
        int id = 0;
        string filename = ""; // 01
        double startX = 0; // 02
        double startY = 0; // 03
        double currentX = 0; // 04
        double currentY = 0; // 05
        bool fixedToMap = false; // 06
        double currentMagnify = 0; // 07
        double currentTopTrans = 0; // 08
        bool transparency = false; // 09
        double currentRed = 0; // 0b
        double currentGreen = 0; // 0c
        double currentBlue = 0; // 0d
        double currentSat = 0; // 0e
        int effectMode = 0; // 0f
        double currentEffect = 0; // 10
        double currentBottomTrans = 0; // 12
        int spritesheetColumns = 0; // 13
        int spritesheetRows = 0; // 14
        int spritesheetFrame = 0; // 15
        int spritesheetSpeed = 0; // 16
        int frames = 0; // 17
        bool spritesheetPlayOnce = false; // 18
        int mapLayer = 0; // 19
        int battleLayer = 0; // 1a
        bool eraseOnMapChange = false; // 1b bits
        bool eraseOnBattleEnd = false; // 1b bits
        bool unusedFlag = false; // 1b bits
        bool unusedFlag2 = false; // 1b bits
        bool affectedByTint = false; // 1b bits
        bool affectedByFlash = false; // 1b bits
        bool affectedByShake = false; // 1b bits
        double finishX = 0; // 1f
        double finishY = 0; // 20
        int finishMagnify = 0; // 21
        int finishTopTrans = 0; // 22
        int finishBottomTrans = 0; // 23
        int finishRed = 0; // 29
        int finishGreen = 0; // 2a
        int finishBlue = 0; // 2b
        int finishSat = 0; // 2c
        int finishEffect = 0; // 2e
        int timeLeft = 0; // 33
        double currentRotation = 0; // 34
        int currentWaver = 0; // 35
        
        static string myClass = "SaveScreen";
        Chunks chunks;
        
        public SavePicture(FileStream f)
        {
            load(f);
        }
        public SavePicture()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                filename = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x02))
                startX = M.readLengthDouble(f);
            if (chunks.next(0x03))
                startY = M.readLengthDouble(f);
            if (chunks.next(0x04))
                currentX = M.readLengthDouble(f);
            if (chunks.next(0x05))
                currentY = M.readLengthDouble(f);
            if (chunks.next(0x06))
                fixedToMap = M.readLengthBool(f);
            if (chunks.next(0x07))
                currentMagnify = M.readLengthDouble(f);
            if (chunks.next(0x08))
                currentTopTrans = M.readLengthDouble(f);
            if (chunks.next(0x09))
                transparency = M.readLengthBool(f);
            if (chunks.next(0x0b))
                currentRed = M.readLengthDouble(f);
            if (chunks.next(0x0c))
                currentGreen = M.readLengthDouble(f);
            if (chunks.next(0x0d))
                currentBlue = M.readLengthDouble(f);
            if (chunks.next(0x0e))
                currentSat = M.readLengthDouble(f);
            if (chunks.next(0x0f))
                effectMode = M.readLengthMultibyte(f);
            if (chunks.next(0x10))
                currentEffect = M.readLengthDouble(f);
            if (chunks.next(0x12))
                currentBottomTrans = M.readLengthDouble(f);
            if (chunks.next(0x13))
                spritesheetColumns = M.readLengthMultibyte(f);
            if (chunks.next(0x14))
                spritesheetRows = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                spritesheetFrame = M.readLengthMultibyte(f);
            if (chunks.next(0x16))
                spritesheetSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                frames = M.readLengthMultibyte(f);
            if (chunks.next(0x18))
                spritesheetPlayOnce = M.readLengthBool(f);
            if (chunks.next(0x19))
                mapLayer = M.readLengthMultibyte(f);
            if (chunks.next(0x1a))
                battleLayer = M.readLengthMultibyte(f);
            if (chunks.next(0x1b))
            {
                bool[] flags = M.readLengthFlags(f);
                eraseOnMapChange = flags[0];
                eraseOnBattleEnd = flags[1];
                unusedFlag = flags[2];
                unusedFlag2 = flags[3];
                affectedByTint = flags[4];
                affectedByFlash = flags[5];
                affectedByShake = flags[6];
            }
            if (chunks.next(0x1f))
                finishX = M.readLengthDouble(f);
            if (chunks.next(0x20))
                finishY = M.readLengthDouble(f);
            if (chunks.next(0x21))
                finishMagnify = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                finishTopTrans = M.readLengthMultibyte(f);
            if (chunks.next(0x23))
                finishBottomTrans = M.readLengthMultibyte(f);
            if (chunks.next(0x29))
                finishRed = M.readLengthMultibyte(f);
            if (chunks.next(0x2a))
                finishGreen = M.readLengthMultibyte(f);
            if (chunks.next(0x2b))
                finishBlue = M.readLengthMultibyte(f);
            if (chunks.next(0x2c))
                finishSat = M.readLengthMultibyte(f);
            if (chunks.next(0x2e))
                finishEffect = M.readLengthMultibyte(f);
            if (chunks.next(0x33))
                timeLeft = M.readLengthMultibyte(f);
            if (chunks.next(0x34))
                currentRotation = M.readLengthDouble(f);
            if (chunks.next(0x35))
                currentWaver = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        // todo: effect modes
        override public string getString()
        {
            if (filename == "")
                return "";
            
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Picture #" + id);
            str.WriteLine("Filename: " + filename);
            str.WriteLine("Start Position: " + startX + "," + startY);
            str.WriteLine("Current Position: " + currentX + "," + currentY);
            str.WriteLine("Destination Position: " + finishX + "," + finishY);
            str.WriteLine("Fixed To Map: " + fixedToMap);
            
            str.WriteLine("Zoom: " + currentMagnify);
            str.WriteLine("Destination Zoom: " + finishMagnify);
            str.WriteLine("Transparency: " + transparency);
            str.WriteLine("Current Top Trans: " + currentTopTrans);
            str.WriteLine("Destination Top Trans: " + finishTopTrans);
            str.WriteLine("Current Bottom Trans: " + currentBottomTrans);
            str.WriteLine("Destination Bottom Trans: " + finishBottomTrans);
            str.WriteLine("Color: R" + currentRed + " G" + currentGreen + " B" + currentBlue + " S" + currentSat);
            str.WriteLine("Destination Color: R" + finishRed + " G" + finishGreen + " B" + finishBlue + " S" + finishSat);
            str.WriteLine("Current Effect: " + currentEffect);
            str.WriteLine("Destination Effect: " + finishEffect);
            str.WriteLine("Effect Mode: " + effectMode);
            
            str.WriteLine("Spritesheet Columns: " + spritesheetColumns);
            str.WriteLine("Spritesheet Rows: " + spritesheetRows);
            str.WriteLine("Spritesheet Frame: " + spritesheetFrame);
            str.WriteLine("Spritesheet Speed: " + spritesheetSpeed);
            str.WriteLine("Spritesheet Play Once: " + spritesheetPlayOnce);
            str.WriteLine("Frames: " + frames);
            
            str.WriteLine("Map Layer: " + mapLayer);
            str.WriteLine("Battle Layer: " + battleLayer);
            str.WriteLine("Erase On Map Change: " + eraseOnMapChange);
            str.WriteLine("Erase On Battle End: " + eraseOnBattleEnd);
            str.WriteLine("Affected By Tint: " + affectedByTint);
            str.WriteLine("Affected By Flash: " + affectedByFlash);
            str.WriteLine("Affected By Shake: " + affectedByShake);
            
            str.WriteLine("Time Left: " + timeLeft);
            str.WriteLine("Rotation: " + currentRotation);
            str.WriteLine("Waver: " + currentWaver);
            
            str.WriteLine();
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(filename, M.S_FILENAME);
            if (chunks.wasNext(0x02))
                M.writeLengthDouble(startX);
            if (chunks.wasNext(0x03))
                M.writeLengthDouble(startY);
            if (chunks.wasNext(0x04))
                M.writeLengthDouble(currentX);
            if (chunks.wasNext(0x05))
                M.writeLengthDouble(currentY);
            if (chunks.wasNext(0x06))
                M.writeLengthBool(fixedToMap);
            if (chunks.wasNext(0x07))
                M.writeLengthDouble(currentMagnify);
            if (chunks.wasNext(0x08))
                M.writeLengthDouble(currentTopTrans);
            if (chunks.wasNext(0x09))
                M.writeLengthBool(transparency);
            if (chunks.wasNext(0x0b))
                M.writeLengthDouble(currentRed);
            if (chunks.wasNext(0x0c))
                M.writeLengthDouble(currentGreen);
            if (chunks.wasNext(0x0d))
                M.writeLengthDouble(currentBlue);
            if (chunks.wasNext(0x0e))
                M.writeLengthDouble(currentSat);
            if (chunks.wasNext(0x0f))
                M.writeLengthMultibyte(effectMode);
            if (chunks.wasNext(0x10))
                M.writeLengthDouble(currentEffect);
            if (chunks.wasNext(0x12))
                M.writeLengthDouble(currentBottomTrans);
            if (chunks.wasNext(0x13))
                M.writeLengthMultibyte(spritesheetColumns);
            if (chunks.wasNext(0x14))
                M.writeLengthMultibyte(spritesheetRows);
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(spritesheetFrame);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(spritesheetSpeed);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(frames);
            if (chunks.wasNext(0x18))
                M.writeLengthBool(spritesheetPlayOnce);
            if (chunks.wasNext(0x19))
                M.writeLengthMultibyte(mapLayer);
            if (chunks.wasNext(0x1a))
                M.writeLengthMultibyte(battleLayer);
            if (chunks.wasNext(0x1b))
                M.writeLengthFlags(new bool[] { eraseOnMapChange, eraseOnBattleEnd, unusedFlag, unusedFlag2,
                                                affectedByTint, affectedByFlash, affectedByShake });
            if (chunks.wasNext(0x1f))
                M.writeLengthDouble(finishX);
            if (chunks.wasNext(0x20))
                M.writeLengthDouble(finishY);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(finishMagnify);
            if (chunks.wasNext(0x22))
                M.writeLengthMultibyte(finishTopTrans);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(finishBottomTrans);
            if (chunks.wasNext(0x29))
                M.writeLengthMultibyte(finishRed);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(finishGreen);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(finishBlue);
            if (chunks.wasNext(0x2c))
                M.writeLengthMultibyte(finishSat);
            if (chunks.wasNext(0x2e))
                M.writeLengthMultibyte(finishEffect);
            if (chunks.wasNext(0x33))
                M.writeLengthMultibyte(timeLeft);
            if (chunks.wasNext(0x34))
                M.writeLengthDouble(currentRotation);
            if (chunks.wasNext(0x35))
                M.writeLengthMultibyte(currentWaver);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void replaceFilenames()
        {
            filename = M.rewriteString(M.M_PICTURE, filename);
        }
    }
    
    class SavePartyLocation : RPGByteData
    {
        bool active = false; // 01
        int mapID = 0; // 0b
        int positionX = 0; // 0c
        int positionY = 0; // 0d
        int direction = 0; // 15
        int spriteDirection = 0; // 16
        int animFrame = 0; // 17
        int transparency = 0; // 18
        int remainingStep = 0; // 1f
        int moveFrequency = 0; // 20
        int layer = 0; // 21
        bool overlapForbidden = false; // 22
        int animationType = 0; // 23
        bool lockFacing = false; // 24
        int moveSpeed = 0; // 25
        MoveRoute moveRoute; // 29
        bool moveRouteOverwrite = false; // 2a
        int moveRouteIndex = 0; // 2b
        bool moveRouteRepeated = false; // 2c
        bool spriteTransparent = false; // 2e
        bool overlap = false; // 2f
        bool animPaused = false; // 30
        bool through = false; // 33
        int stopCount = 0; // 34
        int animCount = 0; // 35
        int maxStopCount = 0; // 36
        bool jumping = false; // 3d
        int beginJumpX = 0; // 3e
        int beginJumpY = 0; // 3f
        bool eventPause = false; // 47
        bool flying = false; // 48
        string charSet = ""; // 49
        int charIndex = 0; // 4a
        bool spriteMoved = false; // 4b
        int flashRed = 0; // 51
        int flashGreen = 0; // 52
        int flashBlue = 0; // 53
        double flashCurrentLevel = 0; // 54
        int flashTimeLeft = 0; // 55
        bool boarding = false; // 65
        bool aboard = false; // 66
        int vehicle = 0; // 67
        bool unboarding = false; // 68
        int preboardMoveSpeed = 0; // 69
        bool menuCalling = false; // 6c
        int panState = 0; // 6f
        int panCurrentX = 0; // 70
        int panCurrentY = 0; // 71
        int panFinishX = 0; // 72
        int panFinishY = 0; // 73
        int panSpeed = 0; // 79
        int encounterSteps = 0; // 7c
        bool encounterCalling = false; // 7d
        int mapSaveCount = 0; // 83
        int databaseSaveCount = 0; // 84
        
        static string myClass = "SavePartyLocation";
        Chunks chunks;
        
        static string[] vehicles = { "None", "Boat", "Ship", "Airship" };
        
        public SavePartyLocation(FileStream f)
        {
            load(f);
        }
        public SavePartyLocation()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
                active = M.readLengthBool(f);
            if (chunks.next(0x0b))
                mapID = M.readLengthMultibyte(f);
            if (chunks.next(0x0c))
                positionX = M.readLengthMultibyte(f);
            if (chunks.next(0x0d))
                positionY = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                direction = M.readLengthMultibyte(f);
            if (chunks.next(0x16))
                spriteDirection = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                animFrame = M.readLengthMultibyte(f);
            if (chunks.next(0x18))
                transparency = M.readLengthMultibyte(f);
            if (chunks.next(0x1f))
                remainingStep = M.readLengthMultibyte(f);
            if (chunks.next(0x20))
                moveFrequency = M.readLengthMultibyte(f);
            if (chunks.next(0x21))
                layer = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                overlapForbidden = M.readLengthBool(f);
            if (chunks.next(0x23))
                animationType = M.readLengthMultibyte(f);
            if (chunks.next(0x24))
                lockFacing = M.readLengthBool(f);
            if (chunks.next(0x25))
                moveSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x29))
            {
                int length = M.readMultibyte(f);
                moveRoute = new MoveRoute(f, length, "Custom");
            }
            if (chunks.next(0x2a))
                moveRouteOverwrite = M.readLengthBool(f);
            if (chunks.next(0x2b))
                moveRouteIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x2c))
                moveRouteRepeated = M.readLengthBool(f);
            if (chunks.next(0x2e))
                spriteTransparent = M.readLengthBool(f);
            if (chunks.next(0x2f))
                overlap = M.readLengthBool(f);
            if (chunks.next(0x30))
                animPaused = M.readLengthBool(f);
            if (chunks.next(0x33))
                through = M.readLengthBool(f);
            if (chunks.next(0x34))
                stopCount = M.readLengthMultibyte(f);
            if (chunks.next(0x35))
                animCount = M.readLengthMultibyte(f);
            if (chunks.next(0x36))
                maxStopCount = M.readLengthMultibyte(f);
            if (chunks.next(0x3d))
                jumping = M.readLengthBool(f);
            if (chunks.next(0x3e))
                beginJumpX = M.readLengthMultibyte(f);
            if (chunks.next(0x3f))
                beginJumpY = M.readLengthMultibyte(f);
            if (chunks.next(0x47))
                eventPause = M.readLengthBool(f);
            if (chunks.next(0x48))
                flying = M.readLengthBool(f);
            if (chunks.next(0x49))
                charSet = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x4a))
                charIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x4b))
                spriteMoved = M.readLengthBool(f);
            if (chunks.next(0x51))
                flashRed = M.readLengthMultibyte(f);
            if (chunks.next(0x52))
                flashGreen = M.readLengthMultibyte(f);
            if (chunks.next(0x53))
                flashBlue = M.readLengthMultibyte(f);
            if (chunks.next(0x54))
                flashCurrentLevel = M.readLengthDouble(f);
            if (chunks.next(0x55))
                flashTimeLeft = M.readLengthMultibyte(f);
            if (chunks.next(0x65))
                boarding = M.readLengthBool(f);
            if (chunks.next(0x66))
                aboard = M.readLengthBool(f);
            if (chunks.next(0x67))
                vehicle = M.readLengthMultibyte(f);
            if (chunks.next(0x68))
                unboarding = M.readLengthBool(f);
            if (chunks.next(0x69))
                preboardMoveSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x6c))
                menuCalling = M.readLengthBool(f);
            if (chunks.next(0x6f))
                panState = M.readLengthMultibyte(f);
            if (chunks.next(0x70))
                panCurrentX = M.readLengthMultibyte(f);
            if (chunks.next(0x71))
                panCurrentY = M.readLengthMultibyte(f);
            if (chunks.next(0x72))
                panFinishX = M.readLengthMultibyte(f);
            if (chunks.next(0x73))
                panFinishY = M.readLengthMultibyte(f);
            if (chunks.next(0x79))
                panSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x7c))
                encounterSteps = M.readLengthMultibyte(f);
            if (chunks.next(0x7d))
                encounterCalling = M.readLengthBool(f);
            if (chunks.next(0x83))
                mapSaveCount = M.readLengthMultibyte(f);
            if (chunks.next(0x84))
                databaseSaveCount = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        // todo: pan states
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Active: " + active);
            str.WriteLine("Current Map: " + M.getDataMap(mapID));
            str.WriteLine("Position: " + positionX + "," + positionY);
            if (charSet != "")
                str.WriteLine("Character Graphic: " + charSet + ", " + (charIndex + 1));
            str.WriteLine("Direction: " + Page.charDirs[direction]);
            str.WriteLine("Sprite Direction: " + Page.charDirs[spriteDirection]);
            str.WriteLine("Fixed Direction: " + lockFacing);
            str.WriteLine();
            
            str.WriteLine("Move Speed: " + moveSpeed);
            str.WriteLine("Move Frequency: " + moveFrequency);
            str.WriteLine("Transparency: " + transparency);
            str.WriteLine("Sprite Transparent: " + spriteTransparent);
            str.WriteLine("Layer: " + Page.layers[layer]);
            str.WriteLine("Overlap Forbidden: " + overlapForbidden);
            str.WriteLine("Overlap: " + overlap);
            str.WriteLine("Walk Through Walls: " + through);
            str.WriteLine("Remaining Step: " + remainingStep);
            str.WriteLine("Sprite Moved: " + spriteMoved);
            str.WriteLine();
            
            str.WriteLine("Animation Type: " + Page.animationTypes[animationType]);
            str.WriteLine("Animation Frame: " + animFrame);
            str.WriteLine("Animation Paused: " + animPaused);
            str.WriteLine("Animation Count: " + animCount);
            str.WriteLine("Stop Count: " + stopCount);
            str.WriteLine("Max Stop Count: " + maxStopCount);
            str.WriteLine();
            
            str.WriteLine("Jumping: " + jumping);
            str.WriteLine("Jump Start Position: " + beginJumpX + "," + beginJumpY);
            str.WriteLine("Event Pause: " + eventPause);
            str.WriteLine("Flying: " + flying);
            str.WriteLine("Boarding: " + boarding);
            str.WriteLine("Aboard: " + aboard);
            str.WriteLine("Vehicle: " + vehicles[vehicle]);
            str.WriteLine("Unboarding: " + unboarding);
            str.WriteLine("Preboard Move Speed: " + preboardMoveSpeed);
            str.WriteLine();
            
            str.WriteLine("Move Route:");
            str.WriteLine(moveRoute != null? moveRoute.getString() : "(None)");
            str.WriteLine("Move Route Overwrite: " + moveRouteOverwrite);
            str.WriteLine("Move Route Index: " + moveRouteIndex);
            str.WriteLine("Move Route Repeated: " + moveRouteRepeated);
            str.WriteLine();
            
            str.WriteLine("Flash: R" + flashRed + " G" + flashGreen + " B" + flashBlue);
            str.WriteLine("Flash Current Level: " + flashCurrentLevel);
            str.WriteLine("Flash Time Left: " + flashTimeLeft);
            str.WriteLine();
            
            str.WriteLine("Pan State: " + panState);
            str.WriteLine("Pan Position: " + panCurrentX + "," + panCurrentY);
            str.WriteLine("Pan Destination: " + panFinishX + "," + panFinishY);
            str.WriteLine("Pan Speed: " + panSpeed);
            str.WriteLine();
            
            str.WriteLine("Encounter Steps: " + encounterSteps);
            str.WriteLine("Calling Menu: " + menuCalling);
            str.WriteLine("Calling Encounter: " + encounterCalling);
            str.WriteLine();
            
            str.WriteLine("Map Save Count: " + mapSaveCount);
            str.WriteLine("Database Save Count: " + databaseSaveCount);
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeLengthBool(active);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(mapID);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(positionX);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(positionY);
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(direction);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(spriteDirection);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(animFrame);
            if (chunks.wasNext(0x18))
                M.writeLengthMultibyte(transparency);
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(remainingStep);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(moveFrequency);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(layer);
            if (chunks.wasNext(0x22))
                M.writeLengthBool(overlapForbidden);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(animationType);
            if (chunks.wasNext(0x24))
                M.writeLengthBool(lockFacing);
            if (chunks.wasNext(0x25))
                M.writeLengthMultibyte(moveSpeed);
            if (chunks.wasNext(0x29))
            {
                M.writeMultibyte(moveRoute.getLength());
                moveRoute.write();
            }
            if (chunks.wasNext(0x2a))
                M.writeLengthBool(moveRouteOverwrite);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(moveRouteIndex);
            if (chunks.wasNext(0x2c))
                M.writeLengthBool(moveRouteRepeated);
            if (chunks.wasNext(0x2e))
                M.writeLengthBool(spriteTransparent);
            if (chunks.wasNext(0x2f))
                M.writeLengthBool(overlap);
            if (chunks.wasNext(0x30))
                M.writeLengthBool(animPaused);
            if (chunks.wasNext(0x33))
                M.writeLengthBool(through);
            if (chunks.wasNext(0x34))
                M.writeLengthMultibyte(stopCount);
            if (chunks.wasNext(0x35))
                M.writeLengthMultibyte(animCount);
            if (chunks.wasNext(0x36))
                M.writeLengthMultibyte(maxStopCount);
            if (chunks.wasNext(0x3d))
                M.writeLengthBool(jumping);
            if (chunks.wasNext(0x3e))
                M.writeLengthMultibyte(beginJumpX);
            if (chunks.wasNext(0x3f))
                M.writeLengthMultibyte(beginJumpY);
            if (chunks.wasNext(0x47))
                M.writeLengthBool(eventPause);
            if (chunks.wasNext(0x48))
                M.writeLengthBool(flying);
            if (chunks.wasNext(0x49))
                M.writeString(charSet, M.S_FILENAME);
            if (chunks.wasNext(0x4a))
                M.writeLengthMultibyte(charIndex);
            if (chunks.wasNext(0x4b))
                M.writeLengthBool(spriteMoved);
            if (chunks.wasNext(0x51))
                M.writeLengthMultibyte(flashRed);
            if (chunks.wasNext(0x52))
                M.writeLengthMultibyte(flashGreen);
            if (chunks.wasNext(0x53))
                M.writeLengthMultibyte(flashBlue);
            if (chunks.wasNext(0x54))
                M.writeLengthDouble(flashCurrentLevel);
            if (chunks.wasNext(0x55))
                M.writeLengthMultibyte(flashTimeLeft);
            if (chunks.wasNext(0x65))
                M.writeLengthBool(boarding);
            if (chunks.wasNext(0x66))
                M.writeLengthBool(aboard);
            if (chunks.wasNext(0x67))
                M.writeLengthMultibyte(vehicle);
            if (chunks.wasNext(0x68))
                M.writeLengthBool(unboarding);
            if (chunks.wasNext(0x69))
                M.writeLengthMultibyte(preboardMoveSpeed);
            if (chunks.wasNext(0x6c))
                M.writeLengthBool(menuCalling);
            if (chunks.wasNext(0x6f))
                M.writeLengthMultibyte(panState);
            if (chunks.wasNext(0x70))
                M.writeLengthMultibyte(panCurrentX);
            if (chunks.wasNext(0x71))
                M.writeLengthMultibyte(panCurrentY);
            if (chunks.wasNext(0x72))
                M.writeLengthMultibyte(panFinishX);
            if (chunks.wasNext(0x73))
                M.writeLengthMultibyte(panFinishY);
            if (chunks.wasNext(0x79))
                M.writeLengthMultibyte(panSpeed);
            if (chunks.wasNext(0x7c))
                M.writeLengthMultibyte(encounterSteps);
            if (chunks.wasNext(0x7d))
                M.writeLengthBool(encounterCalling);
            if (chunks.wasNext(0x83))
                M.writeLengthMultibyte(mapSaveCount);
            if (chunks.wasNext(0x84))
                M.writeLengthMultibyte(databaseSaveCount);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void modify()
        {
            Console.Clear();
            Console.WriteLine("Editing Party Location");
            Console.WriteLine("Current party location: Map " + M.getDataMap(mapID) + " (" + positionX + "," + positionY + ")");
            
            Console.Write("Enter new map ID: ");
            string mapStr = Console.ReadLine();
            int newMapID;
            if (int.TryParse(mapStr, out newMapID))
            {
                if (newMapID >= 1 && newMapID <= M.mapNames.Length)
                {
                    Console.Write("Enter new X position: ");
                    string xStr = Console.ReadLine();
                    int newX;
                    if (int.TryParse(xStr, out newX))
                    {
                        if (newX >= 0 && newX < 500)
                        {
                            Console.Write("Enter new Y position: ");
                            string yStr = Console.ReadLine();
                            int newY;
                            if (int.TryParse(yStr, out newY))
                            {
                                if (newY >= 0 && newY < 500)
                                {
                                    Console.WriteLine("Setting party location to " + M.getDataMap(newMapID) + " (" + newX + "," + newY + ").");
                                    Console.WriteLine("Is this okay? (Y/N)");
                                    if (M.yesNoPrompt())
                                    {
                                        if (mapID != newMapID || positionX != newX || positionY != newY)
                                            M.changesMade = true;
                                        mapID = newMapID;
                                        positionX = newX;
                                        positionY = newY;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Location was not changed.");
                                        M.enterToContinue();
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("X is out of range (0 to 499).");
                            M.enterToContinue();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Map number is out of range (1 to " + M.mapNames.Length + ").");
                    M.enterToContinue();
                }
            }
        }
        
        public void replaceFilenames()
        {
            charSet = M.rewriteString(M.M_CHARSET, charSet);
            moveRoute.replaceFilenames();
        }
    }
    
    class SaveVehicleLocation : RPGByteData
    {
        bool active = false; // 01
        int mapID = 0; // 0b
        int positionX = 0; // 0c
        int positionY = 0; // 0d
        int direction = 0; // 15
        int spriteDirection = 0; // 16
        int animFrame = 0; // 17
        bool transparency = false; // 18
        int remainingStep = 0; // 1f
        int moveFrequency = 0; // 20
        int layer = 0; // 21
        bool overlapForbidden = false; // 22
        int animationType = 0; // 23
        bool lockFacing = false; // 24
        int moveSpeed = 0; // 25
        MoveRoute moveRoute; // 29
        bool moveRouteOverwrite = false; // 2a
        int moveRouteIndex = 0; // 2b
        bool moveRouteRepeated = false; // 2c
        bool animPaused = false; // 30
        bool through = false; // 33
        int stopCount = 0; // 34
        int animCount = 0; // 35
        int maxStopCount = 0; // 36
        bool jumping = false; // 3d
        int beginJumpX = 0; // 3e
        int beginJumpY = 0; // 3f
        bool eventPause = false; // 47
        bool flying = false; // 48
        string charSet = ""; // 49
        int charIndex = 0; // 4a
        bool spriteMoved = false; // 4b
        int flashRed = 0; // 51
        int flashGreen = 0; // 52
        int flashBlue = 0; // 53
        double flashCurrentLevel = 0; // 54
        int flashTimeLeft = 0; // 55
        int vehicle = 0; // 65
        int originalMoveRouteIndex = 0; // 66
        int remainingAscent = 0; // 6a
        int remainingDescent = 0; // 6b
        string originalCharSet = ""; // 6f
        int originalCharIndex = 0; // 70
        
        static string myClass = "SaveVehicleLocation";
        Chunks chunks;
        
        static string[] vehicles = { "None", "Boat", "Ship", "Airship" };
        
        public SaveVehicleLocation(FileStream f)
        {
            load(f);
        }
        public SaveVehicleLocation()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
                active = M.readLengthBool(f);
            if (chunks.next(0x0b))
                mapID = M.readLengthMultibyte(f);
            if (chunks.next(0x0c))
                positionX = M.readLengthMultibyte(f);
            if (chunks.next(0x0d))
                positionY = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                direction = M.readLengthMultibyte(f);
            if (chunks.next(0x16))
                spriteDirection = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                animFrame = M.readLengthMultibyte(f);
            if (chunks.next(0x18))
                transparency = M.readLengthBool(f);
            if (chunks.next(0x1f))
                remainingStep = M.readLengthMultibyte(f);
            if (chunks.next(0x20))
                moveFrequency = M.readLengthMultibyte(f);
            if (chunks.next(0x21))
                layer = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                overlapForbidden = M.readLengthBool(f);
            if (chunks.next(0x23))
                animationType = M.readLengthMultibyte(f);
            if (chunks.next(0x24))
                lockFacing = M.readLengthBool(f);
            if (chunks.next(0x25))
                moveSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x29))
            {
                int length = M.readMultibyte(f);
                moveRoute = new MoveRoute(f, length, "Custom");
            }
            if (chunks.next(0x2a))
                moveRouteOverwrite = M.readLengthBool(f);
            if (chunks.next(0x2b))
                moveRouteIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x2c))
                moveRouteRepeated = M.readLengthBool(f);
            if (chunks.next(0x30))
                animPaused = M.readLengthBool(f);
            if (chunks.next(0x33))
                through = M.readLengthBool(f);
            if (chunks.next(0x34))
                stopCount = M.readLengthMultibyte(f);
            if (chunks.next(0x35))
                animCount = M.readLengthMultibyte(f);
            if (chunks.next(0x36))
                maxStopCount = M.readLengthMultibyte(f);
            if (chunks.next(0x3d))
                jumping = M.readLengthBool(f);
            if (chunks.next(0x3e))
                beginJumpX = M.readLengthMultibyte(f);
            if (chunks.next(0x3f))
                beginJumpY = M.readLengthMultibyte(f);
            if (chunks.next(0x47))
                eventPause = M.readLengthBool(f);
            if (chunks.next(0x48))
                flying = M.readLengthBool(f);
            if (chunks.next(0x49))
                charSet = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x4a))
                charIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x4b))
                spriteMoved = M.readLengthBool(f);
            if (chunks.next(0x51))
                flashRed = M.readLengthMultibyte(f);
            if (chunks.next(0x52))
                flashGreen = M.readLengthMultibyte(f);
            if (chunks.next(0x53))
                flashBlue = M.readLengthMultibyte(f);
            if (chunks.next(0x54))
                flashCurrentLevel = M.readLengthMultibyte(f);
            if (chunks.next(0x55))
                flashTimeLeft = M.readLengthMultibyte(f);
            if (chunks.next(0x65))
                vehicle = M.readLengthMultibyte(f);
            if (chunks.next(0x66))
                originalMoveRouteIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x6a))
                remainingAscent = M.readLengthMultibyte(f);
            if (chunks.next(0x6b))
                remainingDescent = M.readLengthMultibyte(f);
            if (chunks.next(0x6f))
                originalCharSet = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x70))
                originalCharIndex = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Active: " + active);
            str.WriteLine("Vehicle: " + vehicles[vehicle]);
            str.WriteLine("Current Map: " + M.getDataMap(mapID));
            str.WriteLine("Position: " + positionX + "," + positionY);
            if (charSet != "")
                str.WriteLine("Character Graphic: " + charSet + ", " + (charIndex + 1));
            if (originalCharSet != "")
                str.WriteLine("Original Character Graphic: " + originalCharSet + ", " + (originalCharIndex + 1));
            str.WriteLine("Direction: " + Page.charDirs[direction]);
            str.WriteLine("Sprite Direction: " + Page.charDirs[spriteDirection]);
            str.WriteLine("Fixed Direction: " + lockFacing);
            str.WriteLine();
            
            str.WriteLine("Move Speed: " + moveSpeed);
            str.WriteLine("Move Frequency: " + moveFrequency);
            str.WriteLine("Transparency: " + transparency);
            str.WriteLine("Layer: " + Page.layers[layer]);
            str.WriteLine("Overlap Forbidden: " + overlapForbidden);
            str.WriteLine("Walk Through Walls: " + through);
            str.WriteLine("Remaining Step: " + remainingStep);
            str.WriteLine("Remaining Ascent: " + remainingAscent);
            str.WriteLine("Remaining Descent: " + remainingDescent);
            str.WriteLine("Sprite Moved: " + spriteMoved);
            str.WriteLine();
            
            str.WriteLine("Animation Type: " + Page.animationTypes[animationType]);
            str.WriteLine("Animation Frame: " + animFrame);
            str.WriteLine("Animation Paused: " + animPaused);
            str.WriteLine("Animation Count: " + animCount);
            str.WriteLine("Stop Count: " + stopCount);
            str.WriteLine("Max Stop Count: " + maxStopCount);
            str.WriteLine();
            
            str.WriteLine("Jumping: " + jumping);
            str.WriteLine("Jump Start Position: " + beginJumpX + "," + beginJumpY);
            str.WriteLine("Event Pause: " + eventPause);
            str.WriteLine("Flying: " + flying);
            str.WriteLine();
            
            str.WriteLine("Move Route:");
            str.WriteLine(moveRoute != null? moveRoute.getString() : "(None)");
            str.WriteLine("Move Route Overwrite: " + moveRouteOverwrite);
            str.WriteLine("Move Route Index: " + moveRouteIndex);
            str.WriteLine("Move Route Repeated: " + moveRouteRepeated);
            str.WriteLine("Original Move Route Index: " + originalMoveRouteIndex);
            str.WriteLine();
            
            str.WriteLine("Flash: R" + flashRed + " G" + flashGreen + " B" + flashBlue);
            str.WriteLine("Flash Current Level: " + flashCurrentLevel);
            str.WriteLine("Flash Time Left: " + flashTimeLeft);
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeLengthBool(active);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(mapID);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(positionX);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(positionY);
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(direction);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(spriteDirection);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(animFrame);
            if (chunks.wasNext(0x18))
                M.writeLengthBool(transparency);
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(remainingStep);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(moveFrequency);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(layer);
            if (chunks.wasNext(0x22))
                M.writeLengthBool(overlapForbidden);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(animationType);
            if (chunks.wasNext(0x24))
                M.writeLengthBool(lockFacing);
            if (chunks.wasNext(0x25))
                M.writeLengthMultibyte(moveSpeed);
            if (chunks.wasNext(0x29))
            {
                M.writeMultibyte(moveRoute.getLength());
                moveRoute.write();
            }
            if (chunks.wasNext(0x2a))
                M.writeLengthBool(moveRouteOverwrite);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(moveRouteIndex);
            if (chunks.wasNext(0x2c))
                M.writeLengthBool(moveRouteRepeated);
            if (chunks.wasNext(0x30))
                M.writeLengthBool(animPaused);
            if (chunks.wasNext(0x33))
                M.writeLengthBool(through);
            if (chunks.wasNext(0x34))
                M.writeLengthMultibyte(stopCount);
            if (chunks.wasNext(0x35))
                M.writeLengthMultibyte(animCount);
            if (chunks.wasNext(0x36))
                M.writeLengthMultibyte(maxStopCount);
            if (chunks.wasNext(0x3d))
                M.writeLengthBool(jumping);
            if (chunks.wasNext(0x3e))
                M.writeLengthMultibyte(beginJumpX);
            if (chunks.wasNext(0x3f))
                M.writeLengthMultibyte(beginJumpY);
            if (chunks.wasNext(0x47))
                M.writeLengthBool(eventPause);
            if (chunks.wasNext(0x48))
                M.writeLengthBool(flying);
            if (chunks.wasNext(0x49))
                M.writeString(charSet, M.S_FILENAME);
            if (chunks.wasNext(0x4a))
                M.writeLengthMultibyte(charIndex);
            if (chunks.wasNext(0x4b))
                M.writeLengthBool(spriteMoved);
            if (chunks.wasNext(0x51))
                M.writeLengthMultibyte(flashRed);
            if (chunks.wasNext(0x52))
                M.writeLengthMultibyte(flashGreen);
            if (chunks.wasNext(0x53))
                M.writeLengthMultibyte(flashBlue);
            if (chunks.wasNext(0x54))
                M.writeLengthDouble(flashCurrentLevel);
            if (chunks.wasNext(0x55))
                M.writeLengthMultibyte(flashTimeLeft);
            if (chunks.wasNext(0x65))
                M.writeLengthMultibyte(vehicle);
            if (chunks.wasNext(0x66))
                M.writeLengthMultibyte(originalMoveRouteIndex);
            if (chunks.wasNext(0x6a))
                M.writeLengthMultibyte(remainingAscent);
            if (chunks.wasNext(0x6b))
                M.writeLengthMultibyte(remainingDescent);
            if (chunks.wasNext(0x6f))
                M.writeString(originalCharSet, M.S_FILENAME);
            if (chunks.wasNext(0x70))
                M.writeLengthMultibyte(originalCharIndex);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void modify()
        {
            Console.Clear();
            Console.WriteLine("Editing " + vehicles[vehicle] + " Location");
            Console.WriteLine("Current vehicle location: "
                + (mapID != 0? ("Map " + M.getDataMap(mapID) + " (" + positionX + "," + positionY + ")") : "Nowhere"));
            
            Console.Write("Enter new map ID (0 for nowhere): ");
            string mapStr = Console.ReadLine();
            int newMapID;
            if (int.TryParse(mapStr, out newMapID))
            {
                if (newMapID >= 0 && newMapID <= M.mapNames.Length)
                {
                    Console.Write("Enter new X position: ");
                    string xStr = Console.ReadLine();
                    int newX;
                    if (int.TryParse(xStr, out newX))
                    {
                        if (newX >= 0 && newX < 500)
                        {
                            Console.Write("Enter new Y position: ");
                            string yStr = Console.ReadLine();
                            int newY;
                            if (int.TryParse(yStr, out newY))
                            {
                                if (newY >= 0 && newY < 500)
                                {
                                    Console.WriteLine("Setting vehicle location to "
                                        + (newMapID != 0? (M.getDataMap(newMapID) + " (" + newX + "," + newY + ")") : "nowhere") + ".");
                                    Console.WriteLine("Is this okay? (Y/N)");
                                    if (M.yesNoPrompt())
                                    {
                                        if (mapID != newMapID || positionX != newX || positionY != newY)
                                            M.changesMade = true;
                                        mapID = newMapID;
                                        positionX = newX;
                                        positionY = newY;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Location was not changed.");
                                        Console.ReadLine();
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("X is out of range (0 to 499).");
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Map number is out of range (0 to " + M.mapNames.Length + ").");
                    Console.ReadLine();
                }
            }
        }
        
        public void replaceFilenames()
        {
            charSet = M.rewriteString(M.M_CHARSET, charSet);
            originalCharSet = M.rewriteString(M.M_CHARSET, originalCharSet);
            moveRoute.replaceFilenames();
        }
    }
    
    class SaveHero : RPGByteData
    {
        int id = 0;
        string heroName = ""; // 01
        string heroTitle = ""; // 02
        string charSet = "[Default]"; // 0b
        int charIndex = 0; // 0c
        int spriteFlags = 0; // 0d
        string faceSet = "[Default]"; // 15
        int faceIndex = 0; // 16
        int level = 0; // 1f
        int exp = 0; // 20
        int hpMod = 0; // 21
        int mpMod = 0; // 22
        int attackMod = 0; // 29
        int defenseMod = 0; // 2a
        int spiritMod = 0; // 2b
        int agilityMod = 0; // 2c
        int skillLength = 0; // 33
        int[] skills; // 34
        int[] equipment; // 3d
        int currentHP = 0; // 47
        int currentMP = 0; // 48
        int[] battleCommands; // 50
        int conditionLength = 0; // 51
        int[] conditions; // 52
        bool changedBattleCommands = false; // 53
        int classID = 0; // 5a
        int row = 0; // 5b
        bool twoSwordStyle = false; // 5c
        bool fixedEquipment = false; // 5d
        bool aiControl = false; // 5e
        bool strongDefense = false; // 5f
        int battlerAnimation = 0; // 60
        
        static string myClass = "SaveHero";
        Chunks chunks;
        
        public SaveHero(FileStream f)
        {
            load(f);
        }
        public SaveHero()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                heroName = M.readString(f, M.S_TOTRANSLATE);
            if (chunks.next(0x02))
                heroTitle = M.readString(f, M.S_TOTRANSLATE);
            if (chunks.next(0x0b))
                charSet = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x0c))
                charIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x0d))
                spriteFlags = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                faceSet = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x16))
                faceIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x1f))
                level = M.readLengthMultibyte(f);
            if (chunks.next(0x20))
                exp = M.readLengthMultibyte(f);
            if (chunks.next(0x21))
                hpMod = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                mpMod = M.readLengthMultibyte(f);
            if (chunks.next(0x29))
                attackMod = M.readLengthMultibyte(f);
            if (chunks.next(0x2a))
                defenseMod = M.readLengthMultibyte(f);
            if (chunks.next(0x2b))
                spiritMod = M.readLengthMultibyte(f);
            if (chunks.next(0x2c))
                agilityMod = M.readLengthMultibyte(f);
            if (chunks.next(0x33))
                skillLength = M.readLengthMultibyte(f);
            if (chunks.next(0x34))
                skills = M.readTwoByteArray(f);
            if (chunks.next(0x3d))
                equipment = M.readTwoByteArray(f);
            if (chunks.next(0x47))
                currentHP = M.readLengthMultibyte(f);
            if (chunks.next(0x48))
                currentMP = M.readLengthMultibyte(f);
            if (chunks.next(0x50))
                battleCommands = M.readTwoByteArray(f);
            if (chunks.next(0x51))
                conditionLength = M.readLengthMultibyte(f);
            if (chunks.next(0x52))
                conditions = M.readByteArray(f);
            if (chunks.next(0x53))
                changedBattleCommands = M.readLengthBool(f);
            if (chunks.next(0x5a))
                classID = M.readLengthMultibyte(f);
            if (chunks.next(0x5b))
                row = M.readLengthMultibyte(f);
            if (chunks.next(0x5c))
                twoSwordStyle = M.readLengthBool(f);
            if (chunks.next(0x5d))
                fixedEquipment = M.readLengthBool(f);
            if (chunks.next(0x5e))
                aiControl = M.readLengthBool(f);
            if (chunks.next(0x5f))
                strongDefense = M.readLengthBool(f);
            if (chunks.next(0x60))
                battlerAnimation = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        // todo: identify spriteFlags (transparency level?)
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Hero #" + id + (M.includeDataNames? " (" + M.getDataHero(id) + ")" : ""));
            str.WriteLine("Name: " + M.x01Default(heroName));
            str.WriteLine("Title: " + M.x01Default(heroTitle));
            if (M.is2003)
                str.WriteLine("Class: " + (classID != 0? M.getDataClass(classID) : "(None)"));
            
            str.WriteLine("Walk Sprite: " + charSet + ", " + (charIndex + 1));
            str.WriteLine("Sprite Flags: " + spriteFlags);
            str.WriteLine("Face Graphic: " + faceSet + ", " + (faceIndex + 1));
            str.WriteLine("Level: " + level);
            str.WriteLine("Experience: " + exp);
            str.WriteLine("Current HP: " + currentHP);
            str.WriteLine("Current MP: " + currentMP);
            str.WriteLine("HP Modifier: " + hpMod);
            str.WriteLine("MP Modifier: " + mpMod);
            str.WriteLine("Attack Modifier: " + attackMod);
            str.WriteLine("Defense Modifier: " + defenseMod);
            str.WriteLine("Spirit Modifier: " + spiritMod);
            str.WriteLine("Agility Modifier: " + agilityMod);
            
            bool written = false;
            str.WriteLine("Skills:");
            if (skills != null)
            {
                for (int i = 0; i < skills.Length; i++)
                {
                    if (skills[i] != 0)
                    {
                        str.WriteLine(M.getDataSkill(skills[i]));
                        written = true;
                    }
                }
            }
            if (!written)
                str.WriteLine("(None)");
            
            written = false;
            str.WriteLine("Equipment:");
            if (equipment != null)
            {
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] != 0)
                    {
                        str.WriteLine(M.getDataItem(equipment[i]));
                        written = true;
                    }
                }
            }
            if (!written)
                str.WriteLine("(None)");
            
            written = false;
            str.WriteLine("Conditions:");
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i] != 0)
                    {
                        str.WriteLine(M.getDataCondition(conditions[i]));
                        written = true;
                    }
                }
            }
            if (!written)
                str.WriteLine("(None)");
            
            if (M.is2003)
            {
                written = false;
                str.WriteLine("Battle Commands:");
                if (battleCommands != null)
                {
                    for (int i = 0; i < battleCommands.Length; i++)
                    {
                        if (battleCommands[i] != 0)
                        {
                            str.WriteLine(M.getDataBattleCommand(battleCommands[i]));
                            written = true;
                        }
                    }
                }
                if (!written)
                    str.WriteLine("(None)");
            }
            
            if (M.is2003)
            {
                str.WriteLine("Changed Battle Commands: " + changedBattleCommands);
                str.WriteLine("Row: " + (row == 1? "Front" : "Back"));
            }
            
            str.WriteLine("Two Sword Style: " + twoSwordStyle);
            str.WriteLine("Fixed Equipment: " + fixedEquipment);
            str.WriteLine("AI Control: " + aiControl);
            str.WriteLine("Strong Defense: " + strongDefense);
            
            if (M.is2003)
                str.WriteLine("Battler Animation: " + (battlerAnimation > 0? M.getDataBattleAnimSet(battlerAnimation) : "(None)"));
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(heroName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x02))
                M.writeString(heroTitle, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x0b))
                M.writeString(charSet, M.S_FILENAME);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(charIndex);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(spriteFlags);
            if (chunks.wasNext(0x15))
                M.writeString(faceSet, M.S_FILENAME);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(faceIndex);
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(level);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(exp);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(hpMod);
            if (chunks.wasNext(0x22))
                M.writeLengthMultibyte(mpMod);
            if (chunks.wasNext(0x29))
                M.writeLengthMultibyte(attackMod);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(defenseMod);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(spiritMod);
            if (chunks.wasNext(0x2c))
                M.writeLengthMultibyte(agilityMod);
            if (chunks.wasNext(0x33))
                M.writeLengthMultibyte(skillLength);
            if (chunks.wasNext(0x34))
                M.writeTwoByteArray(skills);
            if (chunks.wasNext(0x3d))
                M.writeTwoByteArray(equipment);
            if (chunks.wasNext(0x47))
                M.writeLengthMultibyte(currentHP);
            if (chunks.wasNext(0x48))
                M.writeLengthMultibyte(currentMP);
            if (chunks.wasNext(0x50))
                M.writeTwoByteArray(battleCommands);
            if (chunks.wasNext(0x51))
                M.writeLengthMultibyte(conditionLength);
            if (chunks.wasNext(0x52))
                M.writeByteArray(conditions);
            if (chunks.wasNext(0x53))
                M.writeLengthBool(changedBattleCommands);
            if (chunks.wasNext(0x5a))
                M.writeLengthMultibyte(classID);
            if (chunks.wasNext(0x5b))
                M.writeLengthMultibyte(row);
            if (chunks.wasNext(0x5c))
                M.writeLengthBool(twoSwordStyle);
            if (chunks.wasNext(0x5d))
                M.writeLengthBool(fixedEquipment);
            if (chunks.wasNext(0x5e))
                M.writeLengthBool(aiControl);
            if (chunks.wasNext(0x5f))
                M.writeLengthBool(strongDefense);
            if (chunks.wasNext(0x60))
                M.writeLengthMultibyte(battlerAnimation);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        // todo: probably harmless, but level display on file can be wrong when levels are changed and I have no idea why
        public void modify()
        {
            bool menuShow = true;
            while (menuShow)
            {
                Console.Clear();
                Console.WriteLine("Editing Hero #" + id + " (" + M.getDataHero(id) + ")");
                Console.WriteLine("Name: " + M.x01Default(heroName));
                Console.WriteLine("Title: " + M.x01Default(heroTitle));
                Console.WriteLine("Level: " + level);
                Console.WriteLine("EXP: " + exp);
                Console.WriteLine("HP: " + currentHP + "/" + getModdedMaxHP() + " (" + M.withPlusMinus(hpMod) + ")");
                Console.WriteLine("MP: " + currentMP + "/" + getModdedMaxMP() + " (" + M.withPlusMinus(mpMod) + ")");
                Console.WriteLine("ATK " + M.withPlusMinus(attackMod) + " / DEF " + M.withPlusMinus(defenseMod) + " / SPR " + M.withPlusMinus(spiritMod) + " / AGI " + M.withPlusMinus(agilityMod));
                Console.WriteLine();
                
                Console.WriteLine("A. Edit Name");
                Console.WriteLine("B. Edit Title");
                Console.WriteLine("C. Edit Level");
                Console.WriteLine("D. Edit EXP");
                Console.WriteLine("E. Edit Stat Modifiers");
                Console.WriteLine("F. Cure All Conditions");
                Console.WriteLine("Z. Done");
                
                string heroOption = Console.ReadLine().ToUpper();
                switch (heroOption)
                {
                    case "A":
                        Console.Write("Enter new name (12 characters, \\ for default): ");
                        string newName = Console.ReadLine();
                        
                        newName = M.limitLength(newName, 12);
                        if (newName.Equals("\\"))
                            newName = "\x01";
                        if (heroName != newName)
                            M.changesMade = true;
                        heroName = newName;
                        M.saveRef.updateFileName(id, heroName);
                        break;
                    case "B":
                        Console.Write("Enter new title (12 characters, \\ for default): ");
                        string newTitle = Console.ReadLine();
                        
                        newTitle = M.limitLength(newTitle, 12);
                        if (newTitle.Equals("\\"))
                            newTitle = "\x01";
                        if (heroTitle != newTitle)
                            M.changesMade = true;
                        heroTitle = newTitle;
                        M.saveRef.updateFileTitle(id, heroTitle);
                        break;
                    case "C":
                        Console.Write("Enter new level: ");
                        string newLevelStr = Console.ReadLine();
                        int newLevel;
                        if (int.TryParse(newLevelStr, out newLevel))
                        {
                            if (newLevel >= 1 && newLevel <= (M.is2003? 99 : 50))
                            {
                                if (level != newLevel)
                                {
                                    M.changesMade = true;
                                    int oldLevel = level;
                                    level = newLevel;
                                    exp = M.saveRef.db.getHeroEXPForLevel(id, level - 1);
                                    clampHPMPToMax();
                                    adjustSkillsForLevel(id, oldLevel, level);
                                    Console.WriteLine("Level set to " + level + ", and EXP adjusted to " + exp + ".");
                                    Console.ReadLine();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Level is out of range (1 to " + (M.is2003? 99 : 50) + ").");
                                Console.ReadLine();
                            }
                        }
                        break;
                    case "D":
                        Console.Write("Enter new EXP: ");
                        string newEXPStr = Console.ReadLine();
                        int newEXP;
                        if (int.TryParse(newEXPStr, out newEXP))
                        {
                            if (newEXP >= 0 && newEXP <= 999999)
                            {
                                if (exp != newEXP)
                                {
                                    M.changesMade = true;
                                    int oldLevel = level;
                                    exp = newEXP;
                                    for (int lv = 1; lv < (M.is2003? 99 : 50); lv++)
                                    {
                                        if (exp >= M.saveRef.db.getHeroEXPForLevel(id, lv - 1))
                                            level = lv;
                                        else
                                            break;
                                    }
                                    clampHPMPToMax();
                                    adjustSkillsForLevel(id, oldLevel, level);
                                    Console.WriteLine("EXP set to " + exp + ", and level adjusted to " + level + ".");
                                    Console.ReadLine();
                                }
                            }
                            else
                            {
                                Console.WriteLine("EXP is out of range (0 to 999999).");
                                Console.ReadLine();
                            }
                        }
                        break;
                    case "E":
                        Console.WriteLine("1. HP Modifier");
                        Console.WriteLine("2. MP Modifier");
                        Console.WriteLine("3. Attack Modifier");
                        Console.WriteLine("4. Defense Modifier");
                        Console.WriteLine("5. Spirit Modifier");
                        Console.WriteLine("6. Agility Modifier");
                        string stat = Console.ReadLine();
                        switch (stat)
                        {
                            case "1": editStatMod(ref hpMod, "HP"); clampHPMPToMax(); break;
                            case "2": editStatMod(ref mpMod, "MP"); clampHPMPToMax(); break;
                            case "3": editStatMod(ref attackMod, "Attack"); break;
                            case "4": editStatMod(ref defenseMod, "Defense"); break;
                            case "5": editStatMod(ref spiritMod, "Spirit"); break;
                            case "6": editStatMod(ref agilityMod, "Agility"); break;
                        }
                        break;
                    case "F":
                        cureConditions();
                        Console.WriteLine("Conditions cured.");
                        Console.ReadLine();
                        break;
                    case "Z":
                        menuShow = false;
                        break;
                    default:
                        int heroID;
                        if (int.TryParse(heroOption, out heroID))
                        {
                            if (M.saveRef.tryToSwitchHeroEdit(heroID))
                                return;
                        }
                        break;
                }
            }
        }
        
        public void fullRecovery()
        {
            currentHP = M.saveRef.db.getHeroBaseMaxHP(id, level) + hpMod;
            currentMP = M.saveRef.db.getHeroBaseMaxMP(id, level) + mpMod;
            cureConditions();
            M.saveRef.updateFileHP(id, currentHP);
            M.changesMade = true;
        }
        
        void editStatMod(ref int stat, string statName)
        {
            int range = 999;
            if (statName.Equals("HP") && M.is2003)
                range = 9999;
            
            do
            {
                Console.WriteLine("Enter new " + statName + " modifier. (Currently " + stat + ")");
                string statMod = Console.ReadLine();
                int newValue;
                if (int.TryParse(statMod, out newValue))
                {
                    if (newValue >= -range && newValue <= range)
                    {
                        if (stat != newValue)
                        {
                            stat = newValue;
                            M.changesMade = true;
                        }
                        break;
                    }
                    else
                        Console.WriteLine("Value is out of range (-" + range + " to +" + range + ").");
                }
                else // Prompt again if out of range, but cancel if non-number is entered
                    break;
            } while (1 == 1);
        }
        
        void cureConditions()
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i] != 0)
                    M.changesMade = true;
                conditions[i] = 0;
            }
        }
        
        public void clampHPMPToMax()
        {
            int maxHP = getModdedMaxHP();
            int maxMP = getModdedMaxMP();
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
                M.saveRef.updateFileHP(id, currentHP);
            }
            if (currentMP > maxMP)
                currentMP = maxMP;
        }
        
        public void adjustSkillsForLevel(int id, int oldLevel, int newLevel)
        {
            if (newLevel == oldLevel)
                return;
            
            List<HeroLearnSkill> learnSkills = M.saveRef.db.getHeroLearnSkills(id);
            List<int> skillList = new List<int>(skills);
            
            for (int i = 0; i < learnSkills.Count; i++)
            {
                HeroLearnSkill learn = learnSkills[i];
                int learnLevel = learn.getLevel();
                int skillID = learn.getSkillID();
                if (newLevel < oldLevel) // Leveling down; remove any skills learned in this range
                {
                    if (skillList.Contains(skillID) && learnLevel > newLevel && learnLevel <= oldLevel)
                    {
                        Console.WriteLine("Unlearned " + M.getDataSkill(skillID) + ".");
                        skillList.Remove(skillID);
                    }
                }
                else // Leveling up; add any skills learned in this range
                {
                    if (!skillList.Contains(skillID) && learnLevel > oldLevel && learnLevel <= newLevel)
                    {
                        Console.WriteLine("Learned " + M.getDataSkill(skillID) + ".");
                        skillList.Add(skillID);
                    }
                }
            }
            
            skills = skillList.ToArray();
        }
        
        public string getName()
        {
            return heroName;
        }
        public string getTitle()
        {
            return heroTitle;
        }
        public int getCurrentHP()
        {
            return currentHP;
        }
        public int getModdedMaxHP()
        {
            int trueHP = M.saveRef.db.getHeroBaseMaxHP(id, level) + hpMod;
            if (trueHP < 1)
                trueHP = 1;
            if (trueHP > 999 && !M.is2003)
                trueHP = 999;
            if (trueHP > 9999)
                trueHP = 9999;
            return trueHP;
        }
        public int getModdedMaxMP()
        {
            int trueMP = M.saveRef.db.getHeroBaseMaxMP(id, level) + mpMod;
            if (trueMP < 0)
                trueMP = 0;
            if (trueMP > 999)
                trueMP = 999;
            return trueMP;
        }
        
        public void replaceFilenames()
        {
            charSet = M.rewriteString(M.M_CHARSET, charSet);
            faceSet = M.rewriteString(M.M_FACESET, faceSet);
        }
    }
    
    class SaveInventory : RPGByteData
    {
        int partyLength = 0; // 01
        int[] partyMembers; // 02
        int itemLength = 0; // 0b
        int[] itemIDs; // 0c
        int[] itemQuantities; // 0d
        int[] itemUsage; // 0e
        int money = 0; // 15
        int timer1Secs = 0; // 17
        bool timer1Active = false; // 18
        bool timer1Visible = false; // 19
        bool timer1Battle = false; // 1a
        int timer2Secs = 0; // 1b
        bool timer2Active = false; // 1c
        bool timer2Visible = false; // 1d
        bool timer2Battle = false; // 1e
        int battles = 0; // 20
        int defeats = 0; // 21
        int escapes = 0; // 22
        int victories = 0; // 23
        int turns = 0; // 29
        int steps = 0; // 2a
        
        static string myClass = "SaveInventory";
        Chunks chunks;
        
        public SaveInventory(FileStream f)
        {
            load(f);
        }
        public SaveInventory()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
                partyLength = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
                partyMembers = M.readTwoByteArray(f);
            if (chunks.next(0x0b))
                itemLength = M.readLengthMultibyte(f);
            if (chunks.next(0x0c))
                itemIDs = M.readTwoByteArray(f);
            if (chunks.next(0x0d))
                itemQuantities = M.readByteArray(f);
            if (chunks.next(0x0e))
                itemUsage = M.readByteArray(f);
            if (chunks.next(0x15))
                money = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                timer1Secs = M.readLengthMultibyte(f);
            if (chunks.next(0x18))
                timer1Active = M.readLengthBool(f);
            if (chunks.next(0x19))
                timer1Visible = M.readLengthBool(f);
            if (chunks.next(0x1a))
                timer1Battle = M.readLengthBool(f);
            if (chunks.next(0x1b))
                timer2Secs = M.readLengthMultibyte(f);
            if (chunks.next(0x1c))
                timer2Active = M.readLengthBool(f);
            if (chunks.next(0x1d))
                timer2Visible = M.readLengthBool(f);
            if (chunks.next(0x1e))
                timer2Battle = M.readLengthBool(f);
            if (chunks.next(0x20))
                battles = M.readLengthMultibyte(f);
            if (chunks.next(0x21))
                defeats = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                escapes = M.readLengthMultibyte(f);
            if (chunks.next(0x23))
                victories = M.readLengthMultibyte(f);
            if (chunks.next(0x29))
                turns = M.readLengthMultibyte(f);
            if (chunks.next(0x2a))
                steps = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Party Members:");
            if (partyMembers != null)
            {
                for (int i = 0; i < partyMembers.Length; i++)
                    if (partyMembers[i] != 0)
                        str.WriteLine(M.getDataHero(partyMembers[i]));
            }
            str.WriteLine();
            
            bool written = false;
            str.WriteLine("Items:");
            if (itemIDs != null)
            {
                for (int i = 0; i < itemIDs.Length; i++)
                {
                    if (itemIDs[i] != 0)
                    {
                        str.WriteLine(M.getDataItem(itemIDs[i]) + " x" + itemQuantities[i / 2] + (itemUsage[i / 2] > 0? (" (Uses: " + itemUsage[i] + ")") : ""));
                        written = true;
                    }
                }
            }
            if (!written)
                str.WriteLine("(None)");
            str.WriteLine();
            
            str.WriteLine("Money: " + money);
            str.WriteLine("Timer 1: " + M.secondsToTime(timer1Secs));
            str.WriteLine("Timer 1 Active: " + timer1Active);
            str.WriteLine("Timer 1 Visible: " + timer1Visible);
            str.WriteLine("Timer 1 Battle: " + timer1Battle);
            str.WriteLine("Timer 2: " + M.secondsToTime(timer2Secs));
            str.WriteLine("Timer 2 Active: " + timer2Active);
            str.WriteLine("Timer 2 Visible: " + timer2Visible);
            str.WriteLine("Timer 2 Battle: " + timer2Battle);
            str.WriteLine();
            
            str.WriteLine("Total Battles: " + battles);
            str.WriteLine("Defeats: " + defeats);
            str.WriteLine("Escapes: " + escapes);
            str.WriteLine("Victories: " + victories);
            str.WriteLine("Battle Turns: " + turns);
            str.WriteLine("Steps Taken: " + steps);
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(partyLength);
            if (chunks.wasNext(0x02))
                M.writeTwoByteArray(partyMembers);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(itemLength);
            if (chunks.wasNext(0x0c))
                M.writeTwoByteArray(itemIDs);
            if (chunks.wasNext(0x0d))
                M.writeByteArray(itemQuantities);
            if (chunks.wasNext(0x0e))
                M.writeByteArray(itemUsage);
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(money);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(timer1Secs);
            if (chunks.wasNext(0x18))
                M.writeLengthBool(timer1Active);
            if (chunks.wasNext(0x19))
                M.writeLengthBool(timer1Visible);
            if (chunks.wasNext(0x1a))
                M.writeLengthBool(timer1Battle);
            if (chunks.wasNext(0x1b))
                M.writeLengthMultibyte(timer2Secs);
            if (chunks.wasNext(0x1c))
                M.writeLengthBool(timer2Active);
            if (chunks.wasNext(0x1d))
                M.writeLengthBool(timer2Visible);
            if (chunks.wasNext(0x1e))
                M.writeLengthBool(timer2Battle);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(battles);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(defeats);
            if (chunks.wasNext(0x22))
                M.writeLengthMultibyte(escapes);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(victories);
            if (chunks.wasNext(0x29))
                M.writeLengthMultibyte(turns);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(steps);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void modifyParty()
        {
            bool menuShow = true;
            while (menuShow)
            {
                Console.Clear();
                Console.WriteLine("Current Party:");
                for (int i = 0; i < partyMembers.Length; i++)
                    if (partyMembers[i] != 0)
                        Console.WriteLine((i + 1) + ". Hero #" + partyMembers[i] + " (" + M.getDataHero(partyMembers[i]) + ")");
                Console.WriteLine();
                
                Console.WriteLine("A. Add Member");
                Console.WriteLine("B. Remove Member");
                Console.WriteLine("Z. Done");
                
                string partyOption = Console.ReadLine().ToUpper();
                switch (partyOption)
                {
                    case "A":
                        if (partyMembers.Length >= 4)
                        {
                            Console.WriteLine("Cannot add members past four.");
                            Console.ReadLine();
                            break;
                        }
                        
                        Console.Write("Enter hero number: ");
                        string heroStr = Console.ReadLine();
                        int heroID;
                        if (int.TryParse(heroStr, out heroID))
                        {
                            if (heroID >= 1 && heroID <= M.heroNames.Length)
                            {
                                bool found = false;
                                for (int i = 0; i < partyMembers.Length; i++)
                                {
                                    if (partyMembers[i] == heroID)
                                    {
                                        Console.WriteLine("Hero #" + heroID + " (" + M.getDataHero(heroID) + ") is already in the party.");
                                        Console.ReadLine();
                                        found = true;
                                        break;
                                    }
                                }
                                if (found)
                                    break;
                                
                                Console.WriteLine("Add hero #" + heroID + " (" + M.getDataHero(heroID) + ")? (Y/N)");
                                if (M.yesNoPrompt())
                                {
                                    partyLength++;
                                    int[] newPartyMembers = new int[partyLength];
                                    partyMembers.CopyTo(newPartyMembers, 0);
                                    newPartyMembers[partyLength - 1] = heroID;
                                    
                                    partyMembers = newPartyMembers;
                                    M.changesMade = true;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Hero number is out of range (1 to " + M.heroNames.Length + ").");
                                Console.ReadLine();
                            }
                        }
                        break;
                    case "B":
                        if (partyMembers.Length <= 1)
                        {
                            Console.WriteLine("Cannot remove final party member.");
                            Console.ReadLine();
                            break;
                        }
                        
                        Console.Write("Enter party member (1 to " + partyLength + ") to remove: ");
                        string removeStr = Console.ReadLine();
                        int removeIndex;
                        if (int.TryParse(removeStr, out removeIndex))
                        {
                            if (removeIndex >= 1 && removeIndex <= partyLength)
                            {
                                removeIndex--;
                                
                                partyLength--;
                                int[] newPartyMembers = new int[partyLength];
                                int index = 0;
                                for (int i = 0; i < partyMembers.Length; i++)
                                    if (i != removeIndex)
                                        newPartyMembers[index++] = partyMembers[i];
                                
                                partyMembers = newPartyMembers;
                                M.saveRef.updateFileForNewPartyLead();
                                M.changesMade = true;
                            }
                            else
                            {
                                Console.WriteLine("Number is out of range (1 to " + partyLength + ").");
                                Console.ReadLine();
                            }
                        }
                        
                        break;
                    case "Z":
                        menuShow = false;
                        break;
                }
            }
        }
        
        public void modifyInventory()
        {
            bool menuShow = true;
            while (menuShow)
            {
                Console.Clear();
                Console.WriteLine("Editing Inventory");
                Console.WriteLine("1 to " + M.itemNames.Length + ". View Item Quantity");
                Console.WriteLine("A. Add Item");
                Console.WriteLine("B. Remove Item");
                Console.WriteLine("Z. Done");
                
                string inventoryOption = Console.ReadLine().ToUpper();
                int viewItemID;
                if (int.TryParse(inventoryOption, out viewItemID))
                {
                    int itemIndex = getItemIndex(viewItemID);
                    int currentQuantity = itemIndex != -1? itemQuantities[itemIndex] : 0;
                    Console.WriteLine("Item #" + viewItemID + " (" + M.getDataItem(viewItemID) + ") x" + currentQuantity);
                    Console.ReadLine();
                }
                else
                {
                    string itemStr;
                    int itemID;
                    switch (inventoryOption)
                    {
                        case "A":
                            Console.Write("Enter item number: ");
                            itemStr = Console.ReadLine();
                            if (int.TryParse(itemStr, out itemID))
                            {
                                if (itemID >= 1 && itemID <= M.itemNames.Length)
                                {
                                    int itemIndex = getItemIndex(itemID);
                                    if (itemIndex != -1)
                                    {
                                        if (itemQuantities[itemIndex] >= 99)
                                        {
                                            Console.WriteLine("Already have 99 of Item #" + itemID + " (" + M.getDataItem(itemID) + ").");
                                            Console.ReadLine();
                                            break;
                                        }
                                    }
                                    
                                    int currentQuantity = itemIndex != -1? itemQuantities[itemIndex] : 0;
                                    Console.WriteLine("Item #" + itemID + " (" + M.getDataItem(itemID) + ") x" + currentQuantity);
                                    
                                    Console.Write("Enter amount to add: ");
                                    string addStr = Console.ReadLine();
                                    int addAmount;
                                    if (int.TryParse(addStr, out addAmount))
                                    {
                                        if (addAmount > 0)
                                        {
                                            addItem(itemID, addAmount);
                                            M.changesMade = true;
                                        }
                                        else if (addAmount != 0)
                                        {
                                            Console.WriteLine("Negative values not allowed.");
                                            Console.ReadLine();
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Item number is out of range (1 to " + M.itemNames.Length + ").");
                                    Console.ReadLine();
                                }
                            }
                            break;
                        case "B":
                            Console.Write("Enter item number: ");
                            itemStr = Console.ReadLine();
                            if (int.TryParse(itemStr, out itemID))
                            {
                                if (itemID >= 1 && itemID <= M.itemNames.Length)
                                {
                                    int itemIndex = getItemIndex(itemID);
                                    if (itemIndex == -1)
                                    {
                                        Console.WriteLine("Item not found in inventory.");
                                        Console.ReadLine();
                                        break;
                                    }
                                    
                                    Console.WriteLine("Item #" + itemID + " (" + M.getDataItem(itemID) + ") x" + itemQuantities[itemIndex]);
                                    
                                    Console.Write("Enter amount to remove: ");
                                    string removeStr = Console.ReadLine();
                                    int removeAmount;
                                    if (int.TryParse(removeStr, out removeAmount))
                                    {
                                        if (removeAmount > 0)
                                        {
                                            removeItem(itemID, removeAmount);
                                            M.changesMade = true;
                                        }
                                        else if (removeAmount != 0)
                                        {
                                            Console.WriteLine("Negative values not allowed.");
                                            Console.ReadLine();
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Item number is out of range (1 to " + M.itemNames.Length + ").");
                                    Console.ReadLine();
                                }
                            }
                            break;
                        case "Z":
                            menuShow = false;
                            break;
                    }
                }
            }
        }
        
        int getItemIndex(int id)
        {
            for (int i = 0; i < itemIDs.Length; i++)
                if (itemIDs[i] == id)
                    return i;
            return -1;
        }
        
        void addItem(int id, int amount)
        {
            int itemIndex = getItemIndex(id);
            if (amount > 99)
                amount = 99;
            
            if (itemIndex != -1) // Already exists in list
            {
                itemQuantities[itemIndex] += amount;
                if (itemQuantities[itemIndex] > 99)
                    itemQuantities[itemIndex] = 99;
            }
            else // Needs to be added to list
            {
                int[] newItemIDs = new int[itemIDs.Length + 1];
                int[] newItemQuantities = new int[itemQuantities.Length + 1];
                int[] newItemUsage = new int[itemUsage.Length + 1];
                
                itemIDs.CopyTo(newItemIDs, 0);
                itemQuantities.CopyTo(newItemQuantities, 0);
                itemUsage.CopyTo(newItemUsage, 0);
                
                int newIndex = newItemIDs.Length - 1;
                newItemIDs[newIndex] = id;
                newItemQuantities[newIndex] = amount;
                newItemUsage[newIndex] = 0;
                
                itemIDs = newItemIDs;
                itemQuantities = newItemQuantities;
                itemUsage = newItemUsage;
                itemLength = newItemIDs.Length;
            }
        }
        
        void removeItem(int id, int amount)
        {
            int itemIndex = getItemIndex(id);
            if (itemIndex == -1)
                return;
            
            itemQuantities[itemIndex] -= amount;
            if (itemQuantities[itemIndex] <= 0)
            {
                int[] newItemIDs = new int[itemIDs.Length - 1];
                int[] newItemQuantities = new int[itemQuantities.Length - 1];
                int[] newItemUsage = new int[itemUsage.Length - 1];
                
                int index = 0;
                for (int i = 0; i < itemIDs.Length; i++)
                {
                    if (i != itemIndex)
                    {
                        newItemIDs[index] = itemIDs[i];
                        newItemQuantities[index] = itemQuantities[i];
                        newItemUsage[index] = itemUsage[i];
                        index++;
                    }
                }
                
                itemIDs = newItemIDs;
                itemQuantities = newItemQuantities;
                itemUsage = newItemUsage;
                itemLength = newItemIDs.Length;
            }
        }
        
        public void modifyMoney()
        {
            Console.WriteLine("Current money: " + money);
            Console.WriteLine("Enter new money amount: ");
            string moneyStr = Console.ReadLine();
            int newMoney;
            if (int.TryParse(moneyStr, out newMoney))
            {
                if (newMoney >= 0 && newMoney <= 999999)
                {
                    if (money != newMoney)
                        M.changesMade = true;
                    money = newMoney;
                }
                else
                {
                    Console.WriteLine("Money is out of range (0 to 999999).");
                    Console.ReadLine();
                }
            }
        }
        
        public int getPartyLeader()
        {
            return partyMembers[0];
        }
    }
    
    class SaveTarget : RPGByteData
    {
        int id = 0;
        int mapID = 0; // 01
        int mapX = 0; // 02
        int mapY = 0; // 03
        bool switchOn = false; // 04
        int switchID = 0; // 05
        
        static string myClass = "SaveTarget";
        Chunks chunks;
        
        public SaveTarget(FileStream f)
        {
            load(f);
        }
        public SaveTarget()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                mapID = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
                mapX = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                mapY = M.readLengthMultibyte(f);
            if (chunks.next(0x04))
                switchOn = M.readLengthBool(f);
            if (chunks.next(0x05))
                switchID = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            if (id == 0)
                return "";
            
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Target #" + id);
            str.WriteLine("Map ID: " + M.getDataMap(mapID));
            str.WriteLine("Map Position: " + mapX + "," + mapY);
            if (switchOn)
                str.WriteLine("Enable Switch: " + M.getDataSwitch(switchID));
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(mapID);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(mapX);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(mapY);
            if (chunks.wasNext(0x04))
                M.writeLengthBool(switchOn);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(switchID);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
    
    class SaveMapInfo : RPGByteData
    {
        int positionX = 0; // 01
        int positionY = 0; // 02
        int encounterRate = 0; // 03
        int tileset = 0; // 05
        List<SaveEvent> events; // 0b
        int[] layer1Tiles; // 15
        int[] layer2Tiles; // 16
        string parallaxName = "[Default]"; // 20
        bool horzLoop = false; // 21
        bool vertLoop = false; // 22
        bool horzAuto = false; // 23
        int horzSpeed = 0; // 24
        bool vertAuto = false; // 25
        int vertSpeed = 0; // 26
        
        static string myClass = "SaveMapInfo";
        Chunks chunks;
        
        public SaveMapInfo(FileStream f)
        {
            load(f);
        }
        public SaveMapInfo()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
                positionX = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
                positionY = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                encounterRate = M.readLengthMultibyte(f);
            if (chunks.next(0x05))
                tileset = M.readLengthMultibyte(f);
            if (chunks.next(0x0b))
                events = M.readList<SaveEvent>(f);
            if (chunks.next(0x15))
                layer1Tiles = M.readTwoByteArray(f);
            if (chunks.next(0x16))
                layer2Tiles = M.readTwoByteArray(f);
            if (chunks.next(0x20))
                parallaxName = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x21))
                horzLoop = M.readLengthBool(f);
            if (chunks.next(0x22))
                vertLoop = M.readLengthBool(f);
            if (chunks.next(0x23))
                horzAuto = M.readLengthBool(f);
            if (chunks.next(0x24))
                horzSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x25))
                vertAuto = M.readLengthBool(f);
            if (chunks.next(0x26))
                vertSpeed = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Position: " + positionX + "," + positionY);
            str.WriteLine("Encounter Rate: " + encounterRate);
            str.WriteLine("Tileset: " + (tileset != 0? M.getDataChipSet(tileset) : "[Default]"));
            str.WriteLine("Parallax: " + parallaxName
                + (horzLoop? (", Horizontal Loop" + (horzAuto? " (" + horzSpeed + ")" : "")) : "")
                + (vertLoop? (", Vertical Loop" + (vertAuto? " (" + vertSpeed + ")" : "")) : ""));
            str.WriteLine();
            
            if (events != null)
                foreach (SaveEvent ev in events)
                    str.WriteLine(ev.getString());
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(positionX);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(positionY);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(encounterRate);
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(tileset);
            if (chunks.wasNext(0x0b))
                M.writeList<SaveEvent>(events);
            if (chunks.wasNext(0x15))
                M.writeTwoByteArray(layer1Tiles);
            if (chunks.wasNext(0x16))
                M.writeTwoByteArray(layer2Tiles);
            if (chunks.wasNext(0x20))
                M.writeString(parallaxName, M.S_FILENAME);
            if (chunks.wasNext(0x21))
                M.writeLengthBool(horzLoop);
            if (chunks.wasNext(0x22))
                M.writeLengthBool(vertLoop);
            if (chunks.wasNext(0x23))
                M.writeLengthBool(horzAuto);
            if (chunks.wasNext(0x24))
                M.writeLengthMultibyte(horzSpeed);
            if (chunks.wasNext(0x25))
                M.writeLengthBool(vertAuto);
            if (chunks.wasNext(0x26))
                M.writeLengthMultibyte(vertSpeed);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void replaceFilenames()
        {
            parallaxName = M.rewriteString(M.M_PANORAMA, parallaxName);
            foreach (SaveEvent ev in events)
                ev.replaceFilenames();
        }
    }
    
    class SaveEvent : RPGByteData
    {
        int id = 0;
        bool active = false; // 01
        int mapID = 0; // 0b
        int positionX = 0; // 0c
        int positionY = 0; // 0d
        int direction = 0; // 15
        int spriteDirection = 0; // 16
        int animFrame = 0; // 17
        int transparency = 0; // 18
        int remainingStep = 0; // 1f
        int moveFrequency = 0; // 20
        int layer = 0; // 21
        bool overlapForbidden = false; // 22
        int animationType = 0; // 23
        bool lockFacing = false; // 24
        int moveSpeed = 0; // 25
        MoveRoute moveRoute; // 29
        bool moveRouteOverwrite = false; // 2a
        int moveRouteIndex = 0; // 2b
        bool moveRouteRepeated = false; // 2c
        bool overlap = false; // 2f
        bool animPaused = false; // 30
        bool through = false; // 33
        int stopCount = 0; // 34
        int animCount = 0; // 35
        int maxStopCount = 0; // 36
        bool jumping = false; // 3d
        int beginJumpX = 0; // 3e
        int beginJumpY = 0; // 3f
        bool eventPause = false; // 47
        bool flying = false; // 48
        string charSet = "[Default]"; // 49
        int charIndex = 0; // 4a
        bool spriteMoved = false; // 4b
        int flashRed = 0; // 51
        int flashGreen = 0; // 52
        int flashBlue = 0; // 53
        double flashCurrentLevel = 0; // 54
        int flashTimeLeft = 0; // 55
        bool running = false; // 65
        int originalMoveRouteIndex = 0; // 66
        bool pending = false; // 67
        SaveEventData eventData; // 6c
        
        static string myClass = "SaveEvent";
        Chunks chunks;
        
        public SaveEvent(FileStream f)
        {
            load(f);
        }
        public SaveEvent()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                active = M.readLengthBool(f);
            if (chunks.next(0x0b))
                mapID = M.readLengthMultibyte(f);
            if (chunks.next(0x0c))
                positionX = M.readLengthMultibyte(f);
            if (chunks.next(0x0d))
                positionY = M.readLengthMultibyte(f);
            if (chunks.next(0x15))
                direction = M.readLengthMultibyte(f);
            if (chunks.next(0x16))
                spriteDirection = M.readLengthMultibyte(f);
            if (chunks.next(0x17))
                animFrame = M.readLengthMultibyte(f);
            if (chunks.next(0x18))
                transparency = M.readLengthMultibyte(f);
            if (chunks.next(0x1f))
                remainingStep = M.readLengthMultibyte(f);
            if (chunks.next(0x20))
                moveFrequency = M.readLengthMultibyte(f);
            if (chunks.next(0x21))
                layer = M.readLengthMultibyte(f);
            if (chunks.next(0x22))
                overlapForbidden = M.readLengthBool(f);
            if (chunks.next(0x23))
                animationType = M.readLengthMultibyte(f);
            if (chunks.next(0x24))
                lockFacing = M.readLengthBool(f);
            if (chunks.next(0x25))
                moveSpeed = M.readLengthMultibyte(f);
            if (chunks.next(0x29))
            {
                int length = M.readMultibyte(f);
                moveRoute = new MoveRoute(f, length, "Custom");
            }
            if (chunks.next(0x2a))
                moveRouteOverwrite = M.readLengthBool(f);
            if (chunks.next(0x2b))
                moveRouteIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x2c))
                moveRouteRepeated = M.readLengthBool(f);
            if (chunks.next(0x2f))
                overlap = M.readLengthBool(f);
            if (chunks.next(0x30))
                animPaused = M.readLengthBool(f);
            if (chunks.next(0x33))
                through = M.readLengthBool(f);
            if (chunks.next(0x34))
                stopCount = M.readLengthMultibyte(f);
            if (chunks.next(0x35))
                animCount = M.readLengthMultibyte(f);
            if (chunks.next(0x36))
                maxStopCount = M.readLengthMultibyte(f);
            if (chunks.next(0x3d))
                jumping = M.readLengthBool(f);
            if (chunks.next(0x3e))
                beginJumpX = M.readLengthMultibyte(f);
            if (chunks.next(0x3f))
                beginJumpY = M.readLengthMultibyte(f);
            if (chunks.next(0x47))
                eventPause = M.readLengthBool(f);
            if (chunks.next(0x48))
                flying = M.readLengthBool(f);
            if (chunks.next(0x49))
                charSet = M.readString(f, M.S_FILENAME);
            if (chunks.next(0x4a))
                charIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x4b))
                spriteMoved = M.readLengthBool(f);
            if (chunks.next(0x51))
                flashRed = M.readLengthMultibyte(f);
            if (chunks.next(0x52))
                flashGreen = M.readLengthMultibyte(f);
            if (chunks.next(0x53))
                flashBlue = M.readLengthMultibyte(f);
            if (chunks.next(0x54))
                flashCurrentLevel = M.readLengthDouble(f);
            if (chunks.next(0x55))
                flashTimeLeft = M.readLengthMultibyte(f);
            if (chunks.next(0x65))
                running = M.readLengthBool(f);
            if (chunks.next(0x66))
                originalMoveRouteIndex = M.readLengthMultibyte(f);
            if (chunks.next(0x67))
                pending = M.readLengthBool(f);
            if (chunks.next(0x6c))
            {
                M.readMultibyte(f); // Length
                eventData = new SaveEventData(f);
            }
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Event #" + id);
            str.WriteLine("Active: " + active);
            if (mapID != 0)
                str.WriteLine("Current Map: " + M.getDataMap(mapID));
            str.WriteLine("Position: " + positionX + "," + positionY);
            if (charSet != "")
                str.WriteLine("Character Graphic: " + charSet + ", " + (charIndex + 1));
            str.WriteLine("Direction: " + Page.charDirs[direction]);
            str.WriteLine("Sprite Direction: " + Page.charDirs[spriteDirection]);
            str.WriteLine("Fixed Direction: " + lockFacing);
            str.WriteLine();
            
            str.WriteLine("Move Speed: " + moveSpeed);
            str.WriteLine("Move Frequency: " + moveFrequency);
            str.WriteLine("Transparency: " + transparency);
            str.WriteLine("Layer: " + Page.layers[layer]);
            str.WriteLine("Overlap Forbidden: " + overlapForbidden);
            str.WriteLine("Overlap: " + overlap);
            str.WriteLine("Walk Through Walls: " + through);
            str.WriteLine("Remaining Step: " + remainingStep);
            str.WriteLine("Sprite Moved: " + spriteMoved);
            str.WriteLine();
            
            str.WriteLine("Animation Type: " + Page.animationTypes[animationType]);
            str.WriteLine("Animation Frame: " + animFrame);
            str.WriteLine("Animation Paused: " + animPaused);
            str.WriteLine("Animation Count: " + animCount);
            str.WriteLine("Stop Count: " + stopCount);
            str.WriteLine("Max Stop Count: " + maxStopCount);
            str.WriteLine();
            
            str.WriteLine("Jumping: " + jumping);
            str.WriteLine("Jump Start Position: " + beginJumpX + "," + beginJumpY);
            str.WriteLine("Event Pause: " + eventPause);
            str.WriteLine("Flying: " + flying);
            str.WriteLine();
            
            str.WriteLine("Move Route:");
            str.WriteLine(moveRoute != null? moveRoute.getString() : "(None)");
            str.WriteLine("Move Route Overwrite: " + moveRouteOverwrite);
            str.WriteLine("Move Route Index: " + moveRouteIndex);
            str.WriteLine("Move Route Repeated: " + moveRouteRepeated);
            str.WriteLine("Original Move Route Index: " + originalMoveRouteIndex);
            str.WriteLine();
            
            str.WriteLine("Flash: R" + flashRed + " G" + flashGreen + " B" + flashBlue);
            str.WriteLine("Flash Current Level: " + flashCurrentLevel);
            str.WriteLine("Flash Time Left: " + flashTimeLeft);
            str.WriteLine();
            
            str.WriteLine("Event Running: " + running);
            str.WriteLine("Event Pending: " + pending);
            str.WriteLine();
            
            str.WriteLine(eventData.getString());
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeLengthBool(active);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(mapID);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(positionX);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(positionY);
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(direction);
            if (chunks.wasNext(0x16))
                M.writeLengthMultibyte(spriteDirection);
            if (chunks.wasNext(0x17))
                M.writeLengthMultibyte(animFrame);
            if (chunks.wasNext(0x18))
                M.writeLengthMultibyte(transparency);
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(remainingStep);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(moveFrequency);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(layer);
            if (chunks.wasNext(0x22))
                M.writeLengthBool(overlapForbidden);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(animationType);
            if (chunks.wasNext(0x24))
                M.writeLengthBool(lockFacing);
            if (chunks.wasNext(0x25))
                M.writeLengthMultibyte(moveSpeed);
            if (chunks.wasNext(0x29))
            {
                M.writeMultibyte(moveRoute.getLength());
                moveRoute.write();
            }
            if (chunks.wasNext(0x2a))
                M.writeLengthBool(moveRouteOverwrite);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(moveRouteIndex);
            if (chunks.wasNext(0x2c))
                M.writeLengthBool(moveRouteRepeated);
            if (chunks.wasNext(0x2f))
                M.writeLengthBool(overlap);
            if (chunks.wasNext(0x30))
                M.writeLengthBool(animPaused);
            if (chunks.wasNext(0x33))
                M.writeLengthBool(through);
            if (chunks.wasNext(0x34))
                M.writeLengthMultibyte(stopCount);
            if (chunks.wasNext(0x35))
                M.writeLengthMultibyte(animCount);
            if (chunks.wasNext(0x36))
                M.writeLengthMultibyte(maxStopCount);
            if (chunks.wasNext(0x3d))
                M.writeLengthBool(jumping);
            if (chunks.wasNext(0x3e))
                M.writeLengthMultibyte(beginJumpX);
            if (chunks.wasNext(0x3f))
                M.writeLengthMultibyte(beginJumpY);
            if (chunks.wasNext(0x47))
                M.writeLengthBool(eventPause);
            if (chunks.wasNext(0x48))
                M.writeLengthBool(flying);
            if (chunks.wasNext(0x49))
                M.writeString(charSet, M.S_FILENAME);
            if (chunks.wasNext(0x4a))
                M.writeLengthMultibyte(charIndex);
            if (chunks.wasNext(0x4b))
                M.writeLengthBool(spriteMoved);
            if (chunks.wasNext(0x51))
                M.writeLengthMultibyte(flashRed);
            if (chunks.wasNext(0x52))
                M.writeLengthMultibyte(flashGreen);
            if (chunks.wasNext(0x53))
                M.writeLengthMultibyte(flashBlue);
            if (chunks.wasNext(0x54))
                M.writeLengthDouble(flashCurrentLevel);
            if (chunks.wasNext(0x55))
                M.writeLengthMultibyte(flashTimeLeft);
            if (chunks.wasNext(0x65))
                M.writeLengthBool(running);
            if (chunks.wasNext(0x66))
                M.writeLengthMultibyte(originalMoveRouteIndex);
            if (chunks.wasNext(0x67))
                M.writeLengthBool(pending);
            if (chunks.wasNext(0x6c))
            {
                M.writeMultibyte(eventData.getLength());
                eventData.write();
            }
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void replaceFilenames()
        {
            charSet = M.rewriteString(M.M_CHARSET, charSet);
            eventData.replaceFilenames();
        }
    }
    
    class SaveEventData : RPGByteData
    {
        List<SaveCommands> commands; // 01
        bool showMessage = false; // 04
        bool escapeTriggered = false; // 0b
        bool waitingForMove = false; // 0d
        bool keyInputWait = false; // 15
        int keyInputVariable = 0; // 16
        bool keyInputAllDirections = false; // 17
        bool keyInputConfirm = false; // 18
        bool keyInputCancel = false; // 19
        bool keyInputNumbers = false; // 1a
        bool keyInputOperators = false; // 1b
        bool keyInputShift = false; // 1c
        bool keyInputValueRight = false; // 1d
        bool keyInputValueUp = false; // 1e
        int waitTime = 0; // 1f
        int keyInputTimeVariable = 0; // 20
        bool keyInputDown = false; // 23
        bool keyInputLeft = false; // 24
        bool keyInputRight = false; // 25
        bool keyInputUp = false; // 26
        bool keyInputTimed = false; // 29
        int framesLeft = 0; // 2a
        
        static string myClass = "SaveEventData";
        Chunks chunks;
        
        public SaveEventData(FileStream f)
        {
            load(f);
        }
        public SaveEventData()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            if (chunks.next(0x01))
                commands = M.readList<SaveCommands>(f);
            if (chunks.next(0x04))
                showMessage = M.readLengthBool(f);
            if (chunks.next(0x0b))
                escapeTriggered = M.readLengthBool(f);
            if (chunks.next(0x0d))
                waitingForMove = M.readLengthBool(f);
            if (chunks.next(0x15))
                keyInputWait = M.readLengthBool(f);
            if (chunks.next(0x16))
                keyInputVariable = M.readLengthBytes(f);
            if (chunks.next(0x17))
                keyInputAllDirections = M.readLengthBool(f);
            if (chunks.next(0x18))
                keyInputConfirm = M.readLengthBool(f);
            if (chunks.next(0x19))
                keyInputCancel = M.readLengthBool(f);
            if (chunks.next(0x1a))
                keyInputNumbers = M.readLengthBool(f);
            if (chunks.next(0x1b))
                keyInputOperators = M.readLengthBool(f);
            if (chunks.next(0x1c))
                keyInputShift = M.readLengthBool(f);
            if (chunks.next(0x1d))
                keyInputValueRight = M.readLengthBool(f);
            if (chunks.next(0x1e))
                keyInputValueUp = M.readLengthBool(f);
            if (chunks.next(0x1f))
                waitTime = M.readLengthMultibyte(f);
            if (chunks.next(0x20))
                keyInputTimeVariable = M.readLengthMultibyte(f);
            if (chunks.next(0x23))
                keyInputDown = M.readLengthBool(f);
            if (chunks.next(0x24))
                keyInputLeft = M.readLengthBool(f);
            if (chunks.next(0x25))
                keyInputRight = M.readLengthBool(f);
            if (chunks.next(0x26))
                keyInputUp = M.readLengthBool(f);
            if (chunks.next(0x29))
                keyInputTimed = M.readLengthBool(f);
            if (chunks.next(0x2a))
                framesLeft = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Showing Message: " + showMessage);
            str.WriteLine("Triggered By Escape: " + escapeTriggered);
            str.WriteLine("Waiting For Move: " + waitingForMove);
            str.WriteLine("Key Input Wait: " + keyInputWait);
            str.WriteLine("Key Input Variable: " + keyInputVariable);
            str.WriteLine("Key Input All Directions: " + keyInputAllDirections);
            str.WriteLine("Key Input Confirm: " + keyInputConfirm);
            str.WriteLine("Key Input Cancel: " + keyInputCancel);
            str.WriteLine("Key Input Numbers: " + keyInputNumbers);
            str.WriteLine("Key Input Operators: " + keyInputOperators);
            str.WriteLine("Key Input Shift: " + keyInputShift);
            str.WriteLine("Key Input Value Right: " + keyInputValueRight);
            str.WriteLine("Key Input Value Up: " + keyInputValueUp);
            str.WriteLine("Key Input Time Variable: " + keyInputTimeVariable);
            str.WriteLine("Key Input Down: " + keyInputDown);
            str.WriteLine("Key Input Left: " + keyInputLeft);
            str.WriteLine("Key Input Right: " + keyInputRight);
            str.WriteLine("Key Input Up: " + keyInputUp);
            str.WriteLine("Key Input Timed: " + keyInputTimed);
            str.WriteLine("Wait Time: " + waitTime);
            str.WriteLine("Frames Left: " + framesLeft);
            str.WriteLine();
            
            if (commands != null)
                for (int i = 0; i < commands.Count; i++)
                    str.WriteLine(commands[i].getString(i == commands.Count - 1));
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            if (chunks.wasNext(0x01))
                M.writeList<SaveCommands>(commands);
            if (chunks.wasNext(0x04))
                M.writeLengthBool(showMessage);
            if (chunks.wasNext(0x0b))
                M.writeLengthBool(escapeTriggered);
            if (chunks.wasNext(0x0d))
                M.writeLengthBool(waitingForMove);
            if (chunks.wasNext(0x15))
                M.writeLengthBool(keyInputWait);
            if (chunks.wasNext(0x16))
                M.writeLengthBytes(keyInputVariable);
            if (chunks.wasNext(0x17))
                M.writeLengthBool(keyInputAllDirections);
            if (chunks.wasNext(0x18))
                M.writeLengthBool(keyInputConfirm);
            if (chunks.wasNext(0x19))
                M.writeLengthBool(keyInputCancel);
            if (chunks.wasNext(0x1a))
                M.writeLengthBool(keyInputNumbers);
            if (chunks.wasNext(0x1b))
                M.writeLengthBool(keyInputOperators);
            if (chunks.wasNext(0x1c))
                M.writeLengthBool(keyInputShift);
            if (chunks.wasNext(0x1d))
                M.writeLengthBool(keyInputValueRight);
            if (chunks.wasNext(0x1e))
                M.writeLengthBool(keyInputValueUp);
            if (chunks.wasNext(0x1f))
                M.writeLengthMultibyte(waitTime);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(keyInputTimeVariable);
            if (chunks.wasNext(0x23))
                M.writeLengthBool(keyInputDown);
            if (chunks.wasNext(0x24))
                M.writeLengthBool(keyInputLeft);
            if (chunks.wasNext(0x25))
                M.writeLengthBool(keyInputRight);
            if (chunks.wasNext(0x26))
                M.writeLengthBool(keyInputUp);
            if (chunks.wasNext(0x29))
                M.writeLengthBool(keyInputTimed);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(framesLeft);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool hasCommands()
        {
            if (commands == null)
                return false;
            return commands.Count > 0;
        }
        
        public void replaceFilenames()
        {
            foreach (SaveCommands command in commands)
                command.replaceFilenames();
        }
    }
    
    class SaveCommands : RPGByteData
    {
        int id = 0;
        int commandLength = 0; // 01
        List<Command> commands; // 02
        int currentLine = 0; // 0b
        int eventID = 0; // 0c
        bool triggeredByConfirm = false; // 0d
        int subcommandPathLength = 0; // 15
        int[] subcommandPaths; // 16
        
        static string myClass = "SaveCommands";
        Chunks chunks;
        
        public SaveCommands(FileStream f)
        {
            load(f);
        }
        public SaveCommands()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                commandLength = M.readLengthMultibyte(f);
            if (chunks.next(0x02))
                commands = M.readCommandList(f);
            if (chunks.next(0x0b))
                currentLine = M.readLengthMultibyte(f);
            if (chunks.next(0x0c))
                eventID = M.readLengthMultibyte(f);
            if (chunks.next(0x0d))
                triggeredByConfirm = M.readLengthBool(f);
            if (chunks.next(0x15))
                subcommandPathLength = M.readLengthMultibyte(f);
            if (chunks.next(0x16))
                subcommandPaths = M.readByteArray(f);
            
            M.byteCheck(f, 0x00);
        }
        
        // Default version.
        override public string getString()
        {
            return getString(false);
        }
        
        public string getString(bool topOfStack = false)
        {
            StringWriter str = new StringWriter(new StringBuilder());
            
            str.WriteLine("Command Stack Level #" + id);
            if (eventID != 0)
                str.WriteLine("Event: Event #" + eventID);
            else
                str.WriteLine("Event: Common Event");
            str.WriteLine((topOfStack? "Current Line" : "Return Point") + ": " + (currentLine + 1));
            str.WriteLine("Triggered By Confirm: " + triggeredByConfirm);
            
            if (subcommandPaths != null)
            {
                str.WriteLine("Subcommand Paths:");
                for (int i = 0; i < subcommandPaths.Length; i++)
                {
                    string sub = subcommandPaths[i] != 255? ("Enter Case " + subcommandPaths[i]) : "Don't Enter Cases";
                    str.WriteLine("Indent " + i + ": " + sub);
                }
                str.WriteLine();
            }
            
            if (commands != null)
            {
                str.WriteLine("******************************");
                for (int i = 0; i < commands.Count; i++)
                {
                    string commandStr = commands[i].getString(false, true, i == currentLine);
                    if (commandStr != "")
                        str.WriteLine(commandStr);
                }
                str.WriteLine("******************************");
            }
            
            return str.ToString();
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeLengthMultibyte(commandLength);
            if (chunks.wasNext(0x02))
                M.writeCommandList(commands);
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(currentLine);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(eventID);
            if (chunks.wasNext(0x0d))
                M.writeLengthBool(triggeredByConfirm);
            if (chunks.wasNext(0x15))
                M.writeLengthMultibyte(subcommandPathLength);
            if (chunks.wasNext(0x16))
                M.writeByteArray(subcommandPaths);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void replaceFilenames()
        {
            foreach (Command command in commands)
                command.replaceFilenames();
        }
    }
    
    class SaveCommonEvent : RPGByteData
    {
        int id = 0;
        SaveEventData eventData; // 01
        
        static string myClass = "SaveCommonEvent";
        Chunks chunks;
        
        public SaveCommonEvent(FileStream f)
        {
            load(f);
        }
        public SaveCommonEvent()
        {
        }
        
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
            {
                M.readMultibyte(f); // Length
                eventData = new SaveEventData(f);
            }
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            if (eventData != null)
            {
                if (eventData.hasCommands())
                {
                    StringWriter str = new StringWriter(new StringBuilder());
                    
                    str.WriteLine("Common Event #" + id + (M.includeDataNames? " (" + M.getDataCommon(id) + ")" : ""));
                    str.Write(eventData.getString());
                    
                    return str.ToString();
                }
            }
            
            return "";
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
            {
                M.writeMultibyte(eventData.getLength());
                eventData.write();
            }
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public void replaceFilenames()
        {
            eventData.replaceFilenames();
        }
    }
}
