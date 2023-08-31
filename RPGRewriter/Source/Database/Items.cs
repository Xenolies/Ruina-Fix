using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Items : RPGByteData
    {
        List<Item> items;
        
        public Items(FileStream f)
        {
            load(f);
        }
        public Items()
        {
        }
        
        override public void load(FileStream f)
        {
            items = M.readDatabaseList<Item>(f, "Items", "Item", ref M.itemNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < items.Count; i++)
                tabText.Write(items[i].getString()
                    + (i < items.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Item item in items)
                item.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Item>(items);
        }
    }
    
    class Item : RPGDatabaseEntry
    {
        int id = 0;
        string itemName = ""; // 01
        string itemDescription = ""; // 02
        int classification = 0; // 03
        int price = 0; // 05
        int useCount = 1; // 06
        int atkStat = 0; // 0b
        int defStat = 0; // 0c
        int mindStat = 0; // 0d
        int agiStat = 0; // 0e
        bool twoHanded = false; // 0f
        int mpCost = 0; // 10
        int hitChance = 90; // 11
        int critChance = 0; // 12
        int animationID = 1; // 14
        bool preemptive = false; // 15
        bool attackTwice = false; // 16
        bool attackAll = false; // 17
        bool ignoreEvasion = false; // 18
        bool preventCrit = false; // 19
        bool increaseEvasion = false; // 1a
        bool halfMPCost = false; // 1b
        bool noTerrainDamage = false; // 1c
        bool cursed = false; // 1d (2003)
        bool targetEntireParty = false; // 1f
        int recoverHPPercent = 0; // 20
        int recoverHPPlus = 0; // 21
        int recoverMPPercent = 0; // 22
        int recoverMPPlus = 0; // 23
        bool onlyFromMenu = false; // 25
        bool onlyIfFainted = false; // 26
        int maxHPChange = 0; // 29
        int mapMPChange = 0; // 2a
        int atkChange = 0; // 2b
        int defChange = 0; // 2c
        int mindChange = 0; // 2d
        int agiChange = 0; // 2e
        int useMessageType = 0; // 33
        int invokeSkillNum = 1; // 35
        int invokeSwitchNum = 1; // 37
        bool onField = true; // 39
        bool inBattle = false; // 3a
        int heroListLength; // 3d
        bool[] heroList; // 3e
        int conditionListLength; // 3f
        bool[] conditionList; // 40
        int attributeListLength; // 41
        bool[] attributeList; // 42
        int conditionChance = 0; // 43
        bool conditionRemove = false; // 44 (2003)
        int weaponAnimation = 1; // 45 (2003)
        List<ItemAnimationData> animationData; // 46 (2003)
        bool useSkill = false; // 47 (2003)
        int classListLength; // 48 (2003)
        bool[] classList; // 49 (2003)
        int rangedTrajectory = 0; // 4b (2003)
        int rangedTarget = 0; // 4c (2003)
        
        static string myClass = "Item";
        Chunks chunks;
        
        static int itemNameLimit = 20;
        static int itemDescriptionLimit = 50;
        
        static string[] types = { "Goods", "Arms", "Shield", "Armor", "Helmet", "Other",
                                  "Medicine", "Book", "Material", "Skill Scroll", "Switch" };
        static string[] useCounts = { "Limitless", "1 (Normal)", "2", "3", "4", "5" };
        static string[] rangedTargets = { "Single Target", "Fly Down Screen Center", "Strike All Simultaneously", "Strike All One By One" };
        
        public Item(FileStream f)
        {
            load(f);
        }
        public Item()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                itemName = M.readStringDataName(f, id, ref M.itemNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    itemDescription = M.readString(f, M.S_TOTRANSLATE);
                if (chunks.next(0x03))
                    classification = M.readLengthMultibyte(f);
                
                if (chunks.next(0x05))
                    price = M.readLengthMultibyte(f);
                if (chunks.next(0x06))
                    useCount = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0b))
                    atkStat = M.readLengthMultibyte(f);
                if (chunks.next(0x0c))
                    defStat = M.readLengthMultibyte(f);
                if (chunks.next(0x0d))
                    mindStat = M.readLengthMultibyte(f);
                if (chunks.next(0x0e))
                    agiStat = M.readLengthMultibyte(f);
                if (chunks.next(0x0f))
                    twoHanded = M.readLengthBool(f);
                if (chunks.next(0x10))
                    mpCost = M.readLengthMultibyte(f);
                if (chunks.next(0x11))
                    hitChance = M.readLengthMultibyte(f);
                if (chunks.next(0x12))
                    critChance = M.readLengthMultibyte(f);
                
                if (chunks.next(0x14))
                    animationID = M.readLengthMultibyte(f);
                if (chunks.next(0x15))
                    preemptive = M.readLengthBool(f);
                if (chunks.next(0x16))
                    attackTwice = M.readLengthBool(f);
                if (chunks.next(0x17))
                    attackAll = M.readLengthBool(f);
                if (chunks.next(0x18))
                    ignoreEvasion = M.readLengthBool(f);
                if (chunks.next(0x19))
                    preventCrit = M.readLengthBool(f);
                if (chunks.next(0x1a))
                    increaseEvasion = M.readLengthBool(f);
                if (chunks.next(0x1b))
                    halfMPCost = M.readLengthBool(f);
                if (chunks.next(0x1c))
                    noTerrainDamage = M.readLengthBool(f);
                if (chunks.next(0x1d))
                    cursed = M.readLengthBool(f);
                
                if (chunks.next(0x1f))
                    targetEntireParty = M.readLengthBool(f);
                if (chunks.next(0x20))
                    recoverHPPercent = M.readLengthMultibyte(f);
                if (chunks.next(0x21))
                    recoverHPPlus = M.readLengthMultibyte(f);
                if (chunks.next(0x22))
                    recoverMPPercent = M.readLengthMultibyte(f);
                if (chunks.next(0x23))
                    recoverMPPlus = M.readLengthMultibyte(f);
                
                if (chunks.next(0x25))
                    onlyFromMenu = M.readLengthBool(f);
                if (chunks.next(0x26))
                    onlyIfFainted = M.readLengthBool(f);
                
                if (chunks.next(0x29))
                    maxHPChange = M.readLengthMultibyte(f);
                if (chunks.next(0x2a))
                    mapMPChange = M.readLengthMultibyte(f);
                if (chunks.next(0x2b))
                    atkChange = M.readLengthMultibyte(f);
                if (chunks.next(0x2c))
                    defChange = M.readLengthMultibyte(f);
                if (chunks.next(0x2d))
                    mindChange = M.readLengthMultibyte(f);
                if (chunks.next(0x2e))
                    agiChange = M.readLengthMultibyte(f);
                
                if (chunks.next(0x33))
                    useMessageType = M.readLengthMultibyte(f);
                
                if (chunks.next(0x35))
                    invokeSkillNum = M.readLengthMultibyte(f);
                
                if (chunks.next(0x37))
                    invokeSwitchNum = M.readLengthMultibyte(f);
                
                if (chunks.next(0x39))
                    onField = M.readLengthBool(f);
                if (chunks.next(0x3a))
                    inBattle = M.readLengthBool(f);
                
                if (chunks.next(0x3d))
                    heroListLength = M.readLengthMultibyte(f);
                if (chunks.next(0x3e))
                    heroList = M.readBoolArray(f);
                
                if (chunks.next(0x3f))
                    conditionListLength = M.readLengthMultibyte(f);
                if (chunks.next(0x40))
                    conditionList = M.readBoolArray(f);
                
                if (chunks.next(0x41))
                    attributeListLength = M.readLengthMultibyte(f);
                if (chunks.next(0x42))
                    attributeList = M.readBoolArray(f);
                
                if (chunks.next(0x43))
                    conditionChance = M.readLengthMultibyte(f);
                if (chunks.next(0x44))
                    conditionRemove = M.readLengthBool(f);
                
                if (chunks.next(0x45))
                    weaponAnimation = M.readLengthMultibyte(f);
                if (chunks.next(0x46))
                    animationData = M.readList<ItemAnimationData>(f);
                
                if (chunks.next(0x47))
                    useSkill = M.readLengthBool(f);
                if (chunks.next(0x48))
                    classListLength = M.readLengthMultibyte(f);
                
                if (chunks.next(0x49))
                    classList = M.readBoolArray(f);
                
                if (chunks.next(0x4b))
                    rangedTrajectory = M.readLengthMultibyte(f);
                if (chunks.next(0x4c))
                    rangedTarget = M.readLengthMultibyte(f);
                
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
                
                tabText.WriteLine(M.databaseExportString("Name", itemName, "[" + itemNameLimit + "]"));
                tabText.WriteLine(M.databaseExportString("Description", itemDescription, "[" + itemDescriptionLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter heroClassListStr = new StringWriter(new StringBuilder());
            StringWriter conditionListStr = new StringWriter(new StringBuilder());
            StringWriter attributeListStr = new StringWriter(new StringBuilder());
            StringWriter animationDataStr = new StringWriter(new StringBuilder());;
            
            if (heroList.Length > 0 || !M.is2003) // Determine hero/class list from lengths, but it's always hero list in 2000
            {
                // This can go further than the existing length, so leave those out.
                for (int hero = 0; hero < heroList.Length; hero++)
                    if (heroList[hero] && M.getDataHero(hero + 1) != "(undefined)")
                        heroClassListStr.WriteLine(M.getDataHero(hero + 1));
            }
            else if (classList != null)
            {
                for (int classNum = 0; classNum < classList.Length; classNum++)
                    if (classList[classNum] && M.getDataClass(classNum + 1) != "(undefined)")
                        heroClassListStr.WriteLine(M.getDataClass(classNum + 1));
            }
            
            for (int condition = 0; condition < conditionList.Length; condition++)
                if (conditionList[condition])
                    conditionListStr.WriteLine(M.getDataCondition(condition + 1));
            if (conditionListStr.ToString() == "")
                conditionListStr.WriteLine("(None)");
            
            for (int attribute = 0; attribute < attributeList.Length; attribute++)
                if (attributeList[attribute])
                    attributeListStr.WriteLine(M.getDataAttribute(attribute + 1));
            if (attributeListStr.ToString() == "")
                attributeListStr.WriteLine("(None)");
            
            if (animationData != null)
                for (int hero = 0; hero < animationData.Count; hero++)
                    animationDataStr.WriteLine(animationData[hero].getString());
            
            tabText.WriteLine("Item #" + id);
            if (M.includeMessages)
            {
                tabText.WriteLine("Name: " + itemName);
                tabText.WriteLine("Description: " + itemDescription);
            }
            tabText.WriteLine("Type: " + types[classification]);
            tabText.WriteLine("Price: " + price);
            
            if (classification == 1) // Arms
            {
                tabText.WriteLine("Attack: " + atkStat);
                tabText.WriteLine("Defense: " + defStat);
                tabText.WriteLine("Mind: " + mindStat);
                tabText.WriteLine("Agility: " + agiStat);
                
                tabText.WriteLine("Equip Hand: " + (twoHanded? "Both" : "One"));
                tabText.WriteLine("MP Cost: " + mpCost);
                tabText.WriteLine("Hit Chance: " + hitChance);
                tabText.WriteLine("Crit Chance: " + critChance);
                
                if (M.is2003 && useSkill)
                    tabText.WriteLine("Invoke Skill: " + M.getDataSkill(invokeSkillNum));
                
                tabText.WriteLine("Animation: " + M.getDataAnimation(animationID));
                if (M.is2003)
                {
                    tabText.WriteLine("Weapon Animation: " + M.getDataWeaponAnimation(weaponAnimation));
                    tabText.Write(animationDataStr);
                    tabText.WriteLine("Ranged Target: " + rangedTargets[rangedTarget]);
                    tabText.WriteLine("Ranged Trajectory: " + (rangedTrajectory == 0? "Fly Straight to Target" : "Return to User After Striking"));
                }
                
                tabText.WriteLine("Preemptive: " + preemptive);
                tabText.WriteLine("Attack Twice: " + attackTwice);
                tabText.WriteLine("Attack All Enemies: " + attackAll);
                tabText.WriteLine("Ignore Evasion: " + ignoreEvasion);
                if (M.is2003)
                    tabText.WriteLine("Cursed: " + cursed);
                
                tabText.WriteLine("Usable By:");
                tabText.Write(heroClassListStr);
                
                string conditionTypeStr = conditionRemove? "Remove" : "Inflict";
                
                if (M.is2003)
                    tabText.WriteLine("Condition Type: " + conditionTypeStr);
                tabText.WriteLine(conditionTypeStr + "s Conditions:");
                tabText.Write(conditionListStr);
                tabText.WriteLine(conditionTypeStr + " Rate: " + conditionChance + "%");
                
                tabText.WriteLine("Attributes:");
                tabText.Write(attributeListStr);
            }
            else if (classification >= 2 && classification <= 5) // Shield/Armor/Helmet/Other
            {
                tabText.WriteLine("Attack: " + atkStat);
                tabText.WriteLine("Defense: " + defStat);
                tabText.WriteLine("Mind: " + mindStat);
                tabText.WriteLine("Agility: " + agiStat);
                
                tabText.WriteLine("Prevent Crit: " + preventCrit);
                tabText.WriteLine("Increase Evasion: " + increaseEvasion);
                tabText.WriteLine("Half MP: " + halfMPCost);
                tabText.WriteLine("No Terrain Damage: " + noTerrainDamage);
                if (M.is2003)
                    tabText.WriteLine("Cursed: " + cursed);
                
                if (M.is2003 && useSkill)
                    tabText.WriteLine("Invoke Skill: " + M.getDataSkill(invokeSkillNum));
                
                tabText.WriteLine("Usable By:");
                tabText.Write(heroClassListStr);
                
                string conditionTypeStr = conditionRemove? "Resist" : "Inflict";
                
                if (M.is2003)
                    tabText.WriteLine("Condition Type: " + conditionTypeStr);
                tabText.WriteLine(conditionTypeStr + "s Conditions:");
                tabText.Write(conditionListStr);
                tabText.WriteLine(conditionTypeStr + " Rate: " + conditionChance + "%");
                
                tabText.WriteLine("Resists Attributes:");
                tabText.Write(attributeListStr);
            }
            else if (classification == 6) // Medicine
            {
                tabText.WriteLine("Uses: " + useCounts[useCount]);
                tabText.WriteLine("Target: " + (!targetEntireParty? "Single Ally" : "Whole Party"));
                tabText.WriteLine("HP Recovery: " + recoverHPPercent + "% of Max + " + recoverHPPlus);
                tabText.WriteLine("MP Recovery: " + recoverMPPercent + "% of Max + " + recoverMPPlus);
                tabText.WriteLine("Only Usable From Menu: " + onlyFromMenu);
                tabText.WriteLine("Only KOed Heroes: " + onlyIfFainted);
                
                tabText.WriteLine("Usable By:");
                tabText.Write(heroClassListStr);
                
                tabText.WriteLine("Cures Conditions:");
                tabText.Write(conditionListStr);
            }
            else if (classification == 7) // Book
            {
                tabText.WriteLine("Uses: " + useCounts[useCount]);
                tabText.WriteLine("Teaches Skill: " + M.getDataSkill(invokeSkillNum));
                
                tabText.WriteLine("Usable By:");
                tabText.Write(heroClassListStr);
            }
            else if (classification == 8) // Material
            {
                tabText.WriteLine("Uses: " + useCounts[useCount]);
                tabText.WriteLine("Max HP Change: " + maxHPChange);
                tabText.WriteLine("Max MP Change: " + mapMPChange);
                tabText.WriteLine("Attack Change: " + atkChange);
                tabText.WriteLine("Defense Change: " + defChange);
                tabText.WriteLine("Mind Change: " + mindChange);
                tabText.WriteLine("Agility Change: " + agiChange);
                
                tabText.WriteLine("Usable By:");
                tabText.Write(heroClassListStr);
            }
            else if (classification == 9) // Skill Scroll
            {
                tabText.WriteLine("Uses: " + useCounts[useCount]);
                tabText.WriteLine("Invoke Skill: " + M.getDataSkill(invokeSkillNum));
                tabText.WriteLine("Use Message: " + (useMessageType == 0? "Normal Item Use" : "Based On Skill"));
                
                tabText.WriteLine("Usable By:");
                tabText.Write(heroClassListStr);
            }
            else if (classification == 10) // Switch
            {
                tabText.WriteLine("Uses: " + useCounts[useCount]);
                tabText.WriteLine("Enables Switch: " + M.getDataSwitch(invokeSwitchNum));
                tabText.WriteLine("Usable On Field: " + onField);
                tabText.WriteLine("Usable In Battle: " + inBattle);
            }
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x0d;
            M.importDatabaseString(tabNum, id, "Name", ref itemName, itemNameLimit);
            M.importDatabaseString(tabNum, id, "Description", ref itemDescription, itemDescriptionLimit);
            
            if (itemName != "")
                chunks.add(0x01);
            if (itemDescription != "")
                chunks.add(0x02);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(itemName, M.S_TOTRANSLATE);
            
            if (chunks.wasNext(0x02))
                M.writeString(itemDescription, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(classification);
            
            if (chunks.wasNext(0x05))
                M.writeLengthMultibyte(price);
            if (chunks.wasNext(0x06))
                M.writeLengthMultibyte(useCount);
            
            if (chunks.wasNext(0x0b))
                M.writeLengthMultibyte(atkStat);
            if (chunks.wasNext(0x0c))
                M.writeLengthMultibyte(defStat);
            if (chunks.wasNext(0x0d))
                M.writeLengthMultibyte(mindStat);
            if (chunks.wasNext(0x0e))
                M.writeLengthMultibyte(agiStat);
            if (chunks.wasNext(0x0f))
                M.writeLengthBool(twoHanded);
            if (chunks.wasNext(0x10))
                M.writeLengthMultibyte(mpCost);
            if (chunks.wasNext(0x11))
                M.writeLengthMultibyte(hitChance);
            if (chunks.wasNext(0x12))
                M.writeLengthMultibyte(critChance);
            
            if (chunks.wasNext(0x14))
                M.writeLengthMultibyte(animationID);
            if (chunks.wasNext(0x15))
                M.writeLengthBool(preemptive);
            if (chunks.wasNext(0x16))
                M.writeLengthBool(attackTwice);
            if (chunks.wasNext(0x17))
                M.writeLengthBool(attackAll);
            if (chunks.wasNext(0x18))
                M.writeLengthBool(ignoreEvasion);
            if (chunks.wasNext(0x19))
                M.writeLengthBool(preventCrit);
            if (chunks.wasNext(0x1a))
                M.writeLengthBool(increaseEvasion);
            if (chunks.wasNext(0x1b))
                M.writeLengthBool(halfMPCost);
            if (chunks.wasNext(0x1c))
                M.writeLengthBool(noTerrainDamage);
            if (chunks.wasNext(0x1d))
                M.writeLengthBool(cursed);
            
            if (chunks.wasNext(0x1f))
                M.writeLengthBool(targetEntireParty);
            if (chunks.wasNext(0x20))
                M.writeLengthMultibyte(recoverHPPercent);
            if (chunks.wasNext(0x21))
                M.writeLengthMultibyte(recoverHPPlus);
            if (chunks.wasNext(0x22))
                M.writeLengthMultibyte(recoverMPPercent);
            if (chunks.wasNext(0x23))
                M.writeLengthMultibyte(recoverMPPlus);
            
            if (chunks.wasNext(0x25))
                M.writeLengthBool(onlyFromMenu);
            if (chunks.wasNext(0x26))
                M.writeLengthBool(onlyIfFainted);
            
            if (chunks.wasNext(0x29))
                M.writeLengthMultibyte(maxHPChange);
            if (chunks.wasNext(0x2a))
                M.writeLengthMultibyte(mapMPChange);
            if (chunks.wasNext(0x2b))
                M.writeLengthMultibyte(atkChange);
            if (chunks.wasNext(0x2c))
                M.writeLengthMultibyte(defChange);
            if (chunks.wasNext(0x2d))
                M.writeLengthMultibyte(mindChange);
            if (chunks.wasNext(0x2e))
                M.writeLengthMultibyte(agiChange);
            
            if (chunks.wasNext(0x33))
                M.writeLengthMultibyte(useMessageType);
            
            if (chunks.wasNext(0x35))
                M.writeLengthMultibyte(invokeSkillNum);
            
            if (chunks.wasNext(0x37))
                M.writeLengthMultibyte(invokeSwitchNum);
            
            if (chunks.wasNext(0x39))
                M.writeLengthBool(onField);
            if (chunks.wasNext(0x3a))
                M.writeLengthBool(inBattle);
            
            if (chunks.wasNext(0x3d))
                M.writeLengthMultibyte(heroListLength);
            if (chunks.wasNext(0x3e))
                M.writeBoolArray(heroList);
            
            if (chunks.wasNext(0x3f))
                M.writeLengthMultibyte(conditionListLength);
            if (chunks.wasNext(0x40))
                M.writeBoolArray(conditionList);
            
            if (chunks.wasNext(0x41))
                M.writeLengthMultibyte(attributeListLength);
            if (chunks.wasNext(0x42))
                M.writeBoolArray(attributeList);
            
            if (chunks.wasNext(0x43))
                M.writeLengthMultibyte(conditionChance);
            if (chunks.wasNext(0x44))
                M.writeLengthBool(conditionRemove);
            
            if (chunks.wasNext(0x45))
                M.writeLengthMultibyte(weaponAnimation);
            if (chunks.wasNext(0x46))
                M.writeList<ItemAnimationData>(animationData);
            
            if (chunks.wasNext(0x47))
                M.writeLengthBool(useSkill);
            if (chunks.wasNext(0x48))
                M.writeLengthMultibyte(classListLength);
            
            if (chunks.wasNext(0x49))
                M.writeBoolArray(classList);
            
            if (chunks.wasNext(0x4b))
                M.writeLengthMultibyte(rangedTrajectory);
            if (chunks.wasNext(0x4c))
                M.writeLengthMultibyte(rangedTarget);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            // Since the default item type is Common Goods, only fields applicable to those need to be checked. Phew.
            if (itemName != "" // 01
             || itemDescription != "" // 02
             || classification != 0 // 03
             || price != 0) // 05
                return false;
            
            return true;
        }
    }
    
    class ItemAnimationData : RPGByteData
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
        
        static string myClass = "ItemAnimationData";
        Chunks chunks;
        
        static string[] moves = { "Don't Move", "Step Forward", "Jump Forward", "Move" };
        static string[] attacks = { "One Attack", "Two Attacks", "Three Attacks" };
        static string[] animationSpeeds = { "Fast", "Medium", "Slow" };
        
        public ItemAnimationData(FileStream f)
        {
            load(f);
        }
        public ItemAnimationData()
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
            string animStr = "";
            if (type == 0) // Weapon Animation
                animStr = (weaponAnimation != -1? M.getDataWeaponAnimation(weaponAnimation) : "(None)")
                    + (afterimages == 1 && movement != 0? " (With Afterimages)" : "");
            else // Battle Animation
                animStr = M.getDataAnimation(battleAnimation);
            
            string rangedStr = "";
            if (ranged)
                rangedStr = Environment.NewLine + "- Ranged Animation: " + M.getDataWeaponAnimation(rangedAnimation)
                    + " (" + animationSpeeds[rangedAnimationSpeed] + ")";
            
            return M.getDataHero(id) + " Animation: " + moves[movement] + ", " + attacks[attackCount] + ", " + animStr + rangedStr;
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
    }
}
