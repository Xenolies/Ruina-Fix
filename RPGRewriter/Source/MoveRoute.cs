using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class MoveRoute
    {
        List<MoveStep> steps;
        
        byte[] dataBytes;
        int lengthMinus = 0;
        
        public MoveRoute(FileStream f, int lengthTemp, string source)
        {
            load(f, lengthTemp, source);
        }
        public MoveRoute()
        {
        }
        
        // Loads move route data. Source can be "Move" (move command) or "Custom" (event page custom route); these two read some things in different ways.
        public void load(FileStream f, int lengthTemp, string source)
        {
            steps = new List<MoveStep>();
            for (int i = 0; i < lengthTemp; i++)
            {
                MoveStep step = new MoveStep(f, ref lengthTemp, source);
                steps.Add(step);
            }
        }
        
        // Returns move route string.
        public string getString()
        {
            StringWriter allMovesStr = new StringWriter(new StringBuilder());
            for (int i = 0; i < steps.Count; i++)
                allMovesStr.Write(steps[i].getString() + (i < steps.Count - 1? Environment.NewLine : ""));
            return allMovesStr.ToString();
        }
        
        // Writes move route data, to parent writer by default, and returns the byte size of that data.
        public int write(bool writeToParent = true)
        {
            if (dataBytes == null) // Only need to write once to create dataBytes
            {
                BinaryWriter parentWriter = M.targetWriter;
                BinaryWriter moveRouteData = new BinaryWriter(new MemoryStream());
                M.targetWriter = moveRouteData;
                
                lengthMinus = 0;
                
                foreach (MoveStep step in steps)
                    step.write(ref lengthMinus);
                
                M.targetWriter = parentWriter;
                dataBytes = (moveRouteData.BaseStream as MemoryStream).ToArray();
                moveRouteData.Close();
            }
            
            if (writeToParent)
                M.writeByteArrayNoLength(dataBytes);
            return dataBytes.Length - lengthMinus;
        }
        
        // Returns the byte length of the move route data.
        public int getLength()
        {
            return write(false);
        }
        
        // Replaces filename references.
        public void replaceFilenames()
        {
            foreach (MoveStep step in steps)
                step.replaceFilenames();
        }
    }
}
