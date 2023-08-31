using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Variables : RPGByteData
    {
        List<Variable> variables;
        
        public Variables(FileStream f)
        {
            load(f);
        }
        public Variables()
        {
        }
        
        override public void load(FileStream f)
        {
            variables = M.readDatabaseList<Variable>(f, "Variables", "Variable", ref M.variableNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            if (M.stringScriptExportMode)
            {
                if (M.getExtraneousSetting("SwitchNames"))
                {
                    for (int i = 0; i < variables.Count; i++)
                        tabText.WriteLine(M.databaseExportString((i + 1).ToString("D4"), variables[i].getString()));
                    return tabText.ToString();
                }
                else
                    return "";
            }
            
            if (!M.includeMessages)
                return "";
            
            for (int i = 0; i < variables.Count; i++)
                tabText.WriteLine("[" + (i + 1).ToString("D4") + "] " + variables[i].getString());
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Variable variable in variables)
                variable.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Variable>(variables);
        }
        
        public string[] getVariableNames()
        {
            string[] names = new string[variables.Count];
            for (int i = 0; i < variables.Count; i++)
                names[i] = variables[i].getString();
            return names;
        }
        
        public bool setVariableNames(string[] names)
        {
            if (variables.Count != names.Length)
                Console.WriteLine("Warning: Variable count differs between projects.");
            
            bool changed = false;
            for (int i = 0; i < variables.Count && i < names.Length; i++)
            {
                if (variables[i].setName(names[i]))
                    changed = true;
            }
            
            return changed;
        }
    }
    
    class Variable : RPGDatabaseEntry
    {
        int id = 0;
        string variableName = ""; // 01
        
        List<int> chunkList;
        
        public Variable(FileStream f)
        {
            load(f);
        }
        public Variable()
        {
        }
        
        public void load(FileStream f)
        {
            chunkList = new List<int>();
            
            id = M.readMultibyte(f);
            
            if (Chunks.next(f, 0x01, chunkList))
                variableName = M.readStringDataName(f, id, ref M.variableNames, M.S_UNTRANSLATED);
            
            M.byteCheck(f, 0x00);
        }
        
        public string getString()
        {
            return variableName;
        }
        
        public void importStrings()
        {
            string nameRef = variableName;
            M.importDatabaseString(0x18, 0, id.ToString("D4"), ref nameRef);
            setName(nameRef); // Use this to ensure name chunk is set correctly
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (Chunks.wasNext(0x01, chunkList))
                M.writeString(variableName, M.S_UNTRANSLATED);
            
            M.writeByte(0x00);
        }
        
        public bool isBlank()
        {
            if (variableName != "") // 01
                return false;
            
            return true;
        }
        
        public bool setName(string name)
        {
            if (!variableName.Equals(name))
            {
                variableName = name;
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
