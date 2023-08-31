using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class BattlerAnimations : RPGByteData
    {
        List<BattlerAnimation> battlerAnimations;
        
        public BattlerAnimations(FileStream f)
        {
            load(f);
        }
        public BattlerAnimations()
        {
        }
        
        override public void load(FileStream f)
        {
            battlerAnimations = M.readDatabaseList<BattlerAnimation>(f, "Battler Animations", "Battler Animation", ref M.battlerAnimSetNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < battlerAnimations.Count; i++)
                tabText.Write(battlerAnimations[i].getString()
                    + (i < battlerAnimations.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (BattlerAnimation battlerAnimation in battlerAnimations)
                battlerAnimation.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<BattlerAnimation>(battlerAnimations);
        }
    }
    
    class BattlerAnimation : RPGDatabaseEntry
    {
        int id = 0;
        string animationSetName = ""; // 01
        int animationSpeed = 0; // 02
        List<BattlerAnimationPose> poseData; // 0a
        List<BattlerAnimationWeapon> weaponData; // 0b
        
        static string myClass = "BattlerAnimation";
        Chunks chunks;
        
        static int animationSetNameLimit = 16;
        
        static string[] animationSpeeds = { "Slow", "Medium", "Fast" };
        
        public BattlerAnimation(FileStream f)
        {
            load(f);
        }
        public BattlerAnimation()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                animationSetName = M.readStringDataName(f, id, ref M.battlerAnimSetNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    animationSpeed = M.readLengthMultibyte(f);
                if (chunks.next(0x0a))
                    poseData = M.readList<BattlerAnimationPose>(f, "Pose", id);
                if (chunks.next(0x0b))
                    weaponData = M.readList<BattlerAnimationWeapon>(f, "Weapon", id);
                
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
                
                tabText.WriteLine(M.databaseExportString("Name", animationSetName, "[" + animationSetNameLimit + "]"));
                
                foreach (BattlerAnimationWeapon weapon in weaponData)
                    tabText.WriteLine(weapon.getString());
                
                foreach (BattlerAnimationPose pose in poseData)
                    tabText.WriteLine(pose.getString());
                
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter weaponAnimStr = new StringWriter(new StringBuilder());
            StringWriter battlerPosesStr = new StringWriter(new StringBuilder());
            
            foreach (BattlerAnimationWeapon weapon in weaponData)
                weaponAnimStr.WriteLine(weapon.getString());
            
            foreach (BattlerAnimationPose pose in poseData)
                battlerPosesStr.WriteLine(pose.getString());
            
            tabText.WriteLine("Battler Animation Set #" + id);
            // Speed may go past bounds, seemingly treated as Medium
            tabText.WriteLine("Animation Speed: " + (animationSpeed < animationSpeeds.Length? animationSpeeds[animationSpeed] : animationSpeeds[1]));
            
            tabText.WriteLine("Weapon Animations:");
            tabText.Write(weaponAnimStr);
            
            tabText.WriteLine("Poses:");
            tabText.Write(battlerPosesStr);
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x20;
            M.importDatabaseString(tabNum, id, "Name", ref animationSetName, animationSetNameLimit);
            
            foreach (BattlerAnimationPose pose in poseData)
                pose.importStrings(id);
            foreach (BattlerAnimationWeapon weapon in weaponData)
                weapon.importStrings(id);
            
            if (animationSetName != "")
                chunks.add(0x01);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(animationSetName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(animationSpeed);
            if (chunks.wasNext(0x0a))
                M.writeList<BattlerAnimationPose>(poseData);
            if (chunks.wasNext(0x0b))
                M.writeList<BattlerAnimationWeapon>(weaponData);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (animationSetName != "") // 01
                return false;
            
            return true;
        }
    }
    
    class BattlerAnimationPose : RPGByteData
    {
        int id = 0;
        string poseName = ""; // 01
        string battleCharSet = ""; // 02
        int charSetIndex = 0; // 03
        int animationType = 0; // 04
        int animationID = 0; // 05
        
        List<int> chunkList;
        
        static int poseNameLimit = 18;
        
        public BattlerAnimationPose(FileStream f)
        {
            load(f);
        }
        public BattlerAnimationPose()
        {
        }
        
        override public void load(FileStream f)
        {
            chunkList = new List<int>();
            
            id = M.readMultibyte(f);
            
            if (Chunks.next(f, 0x01, chunkList))
                poseName = M.readStringDataName(f, id, ref M.battlerPoseNames[M.currentBattlerAnimation], M.S_TOTRANSLATE);
            if (Chunks.next(f, 0x02, chunkList))
                battleCharSet = M.readStringAndRewrite(f, M.M_BATTLECHARSET, M.S_FILENAME);
            if (Chunks.next(f, 0x03, chunkList))
                charSetIndex = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x04, chunkList))
                animationType = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x05, chunkList))
                animationID = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            if (M.stringScriptExportMode)
                return M.databaseExportString("Pose" + id + "Name", poseName, "[" + poseNameLimit + "]");
            
            if (animationType == 0) // Character Pose
                return "Pose #" + id + " (" + poseName + "): " + (battleCharSet != ""? (battleCharSet + ", " + charSetIndex) : "N/A");
            else // Battle Animation
                return "Pose #" + id + " (" + poseName + "): " + M.getDataAnimation(animationID);
        }
        
        public void importStrings(int entry)
        {
            int tabNum = 0x20;
            M.importDatabaseString(tabNum, entry, "Pose" + id + "Name", ref poseName, poseNameLimit);
            
            if (poseName != "")
                chunkList.Add(0x01);
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (Chunks.wasNext(0x01, chunkList))
                M.writeString(poseName, M.S_TOTRANSLATE);
            if (Chunks.wasNext(0x02, chunkList))
                M.writeString(battleCharSet, M.S_FILENAME);
            if (Chunks.wasNext(0x03, chunkList))
                M.writeLengthMultibyte(charSetIndex);
            if (Chunks.wasNext(0x04, chunkList))
                M.writeLengthMultibyte(animationType);
            if (Chunks.wasNext(0x05, chunkList))
                M.writeLengthMultibyte(animationID);
            
            M.writeByte(0x00);
        }
    }
    
    class BattlerAnimationWeapon : RPGByteData
    {
        int id = 0;
        string weaponAnimName = ""; // 01
        string battleWeaponGraphic = ""; // 02
        int weaponGraphicIndex = 0; // 03
        
        List<int> chunkList;
        
        static int weaponAnimNameLimit = 18;
        
        public BattlerAnimationWeapon(FileStream f)
        {
            load(f);
        }
        public BattlerAnimationWeapon()
        {
        }
        
        override public void load(FileStream f)
        {
            chunkList = new List<int>();
            
            id = M.readMultibyte(f);
            
            if (Chunks.next(f, 0x01, chunkList))
                weaponAnimName = M.readStringDataName(f, id, ref M.weaponAnimationNames[M.currentBattlerAnimation], M.S_TOTRANSLATE);
            if (Chunks.next(f, 0x02, chunkList))
                battleWeaponGraphic = M.readStringAndRewrite(f, M.M_BATTLEWEAPON, M.S_FILENAME);
            if (Chunks.next(f, 0x03, chunkList))
                weaponGraphicIndex = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            if (M.stringScriptExportMode)
                return M.databaseExportString("Weapon" + id + "Name", weaponAnimName, "[" + weaponAnimNameLimit+ "]");
            
            return "Weapon #" + id + " (" + weaponAnimName + "): " + (battleWeaponGraphic != ""? (battleWeaponGraphic + ", " + weaponGraphicIndex) : "N/A");
        }
        
        public void importStrings(int entry)
        {
            int tabNum = 0x20;
            M.importDatabaseString(tabNum, entry, "Weapon" + id + "Name", ref weaponAnimName, weaponAnimNameLimit);
            
            if (weaponAnimName != "")
                chunkList.Add(0x01);
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (Chunks.wasNext(0x01, chunkList))
                M.writeString(weaponAnimName, M.S_TOTRANSLATE);
            if (Chunks.wasNext(0x02, chunkList))
                M.writeString(battleWeaponGraphic, M.S_FILENAME);
            if (Chunks.wasNext(0x03, chunkList))
                M.writeLengthMultibyte(weaponGraphicIndex);
            
            M.writeByte(0x00);
        }
    }
}
