using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Switches : RPGByteData
    {
        List<Switch> switches;
        
        public Switches(FileStream f)
        {
            load(f);
        }
        public Switches()
        {
        }
        
        override public void load(FileStream f)
        {
            switches = M.readDatabaseList<Switch>(f, "Switches", "Switch", ref M.switchNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            if (M.stringScriptExportMode)
            {
                if (M.getExtraneousSetting("SwitchNames"))
                {
                    for (int i = 0; i < switches.Count; i++)
                        tabText.WriteLine(M.databaseExportString((i + 1).ToString("D4"), switches[i].getString()));
                    return tabText.ToString();
                }
                else
                    return "";
            }
            
            if (!M.includeMessages)
                return "";
            
            for (int i = 0; i < switches.Count; i++)
                tabText.WriteLine("[" + (i + 1).ToString("D4") + "] " + switches[i].getString());
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Switch sw in switches)
                sw.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Switch>(switches);
        }
        
        public string[] getSwitchNames()
        {
            string[] names = new string[switches.Count];
            for (int i = 0; i < switches.Count; i++)
                names[i] = switches[i].getString();
            return names;
        }
        
        public bool setSwitchNames(string[] names)
        {
            if (switches.Count != names.Length)
                Console.WriteLine("Warning: Switch count differs between projects.");
            
            bool changed = false;
            for (int i = 0; i < switches.Count && i < names.Length; i++)
            {
                if (switches[i].setName(names[i]))
                    changed = true;
            }
            
            return changed;
        }
    }
    
    class Switch : RPGDatabaseEntry
    {
        int id = 0;
        string switchName = ""; // 01
        
        List<int> chunkList;
        
        public Switch(FileStream f)
        {
            load(f);
        }
        public Switch()
        {
        }
        
        public void load(FileStream f)
        {
            chunkList = new List<int>();
            
            id = M.readMultibyte(f);
            
            if (Chunks.next(f, 0x01, chunkList))
                switchName = M.readStringDataName(f, id, ref M.switchNames, M.S_UNTRANSLATED);
            
            M.byteCheck(f, 0x00);
        }
        
        public string getString()
        {
            return switchName;
        }
        
        public void importStrings()
        {
            string nameRef = switchName;
            M.importDatabaseString(0x17, 0, id.ToString("D4"), ref nameRef);
            setName(nameRef); // Use this to ensure name chunk is set correctly
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (Chunks.wasNext(0x01, chunkList))
                M.writeString(switchName, M.S_UNTRANSLATED);
            
            M.writeByte(0x00);
        }
        
        public bool isBlank()
        {
            if (switchName != "") // 01
                return false;
            
            return true;
        }
        
        public bool setName(string name)
        {
            if (!switchName.Equals(name))
            {
                switchName = name;
                if (name != "")
                    chunkList.Add(0x01);
                else
                    chunkList.Remove(0x01);
                return true;
            }
            return false;
        }
    }
}
