using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Animations : RPGByteData
    {
        List<Animation> animations;
        
        public Animations(FileStream f)
        {
            load(f);
        }
        public Animations()
        {
        }
        
        override public void load(FileStream f)
        {
            animations = M.readDatabaseList<Animation>(f, "Animations", "Animation", ref M.animationNames);
        }
        
        override public string getString()
        {
            StringWriter tabText = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < animations.Count; i++)
                tabText.Write(animations[i].getString()
                    + (i < animations.Count - 1? Environment.NewLine : ""));
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            foreach (Animation animation in animations)
                animation.importStrings();
        }
        
        override protected void myWrite()
        {
            M.writeListNoLength<Animation>(animations);
        }
    }
    
    class Animation : RPGDatabaseEntry
    {
        int id = 0;
        string animationName = ""; // 01
        string animationFile = ""; // 02
        bool largeAnimation = false; // 03 (2003)
        List<AnimationTiming> seFlashTimings; // 06
        int scope = 0; // 09
        int yPosition = 2; // 0a
        List<AnimationFrame> frames; // 0c
        
        static string myClass = "Animation";
        Chunks chunks;
        
        static int animationNameLimit = 20;
        
        public Animation(FileStream f)
        {
            load(f);
        }
        public Animation()
        {
        }
        
        public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            id = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                animationName = M.readStringDataName(f, id, ref M.animationNames, M.S_TOTRANSLATE);
            
            if (!M.readingDataNames)
            {
                if (chunks.next(0x02))
                    animationFile = M.readString(f, M.S_FILENAME);
                if (chunks.next(0x03))
                    largeAnimation = M.readLengthBool(f);
                
                // Since the determining factor comes after the file string, can't just use readStringAndRewrite, oh well
                animationFile = M.rewriteString(!largeAnimation? M.M_BATTLE : M.M_BATTLE2, animationFile);
                M.checkStringValidForMode(animationFile, !largeAnimation? M.M_BATTLE : M.M_BATTLE2);
                
                if (chunks.next(0x06))
                    seFlashTimings = M.readList<AnimationTiming>(f);
                
                if (chunks.next(0x09))
                    scope = M.readLengthMultibyte(f);
                if (chunks.next(0x0a))
                    yPosition = M.readLengthMultibyte(f);
                
                if (chunks.next(0x0c))
                    frames = M.readList<AnimationFrame>(f);
                
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
                
                tabText.WriteLine(M.databaseExportString("Name", animationName, "[" + animationNameLimit + "]"));
                tabText.WriteLine();
                
                return tabText.ToString();
            }
            
            StringWriter effectListStr = new StringWriter(new StringBuilder());
            StringWriter frameListStr = new StringWriter(new StringBuilder());
            
            foreach (AnimationTiming effect in seFlashTimings)
                effectListStr.WriteLine(effect.getString());
            
            for (int i = 0; i < frames.Count; i++)
                frameListStr.WriteLine("<" + (i + 1) + "> " + frames[i].getString());
            
            tabText.WriteLine("Animation #" + id);
            if (M.includeMessages)
                tabText.WriteLine("Name: " + animationName);
            tabText.WriteLine("Animation File: " + animationFile + (largeAnimation? " (Large)" : ""));
            tabText.WriteLine("Scope: " + (scope == 0? "Single" : "All"));
            tabText.WriteLine("Y Coord Line: " + (yPosition == 0? "Head" : yPosition == 1? "Center" : "Feet"));
            tabText.WriteLine("Sounds and Flashes:");
            tabText.Write(effectListStr);
            tabText.WriteLine("Animation Frames:");
            tabText.Write(frameListStr);
            
            return tabText.ToString();
        }
        
        public void importStrings()
        {
            int tabNum = 0x13;
            M.importDatabaseString(tabNum, id, "Name", ref animationName, animationNameLimit);
            
            if (animationName != "")
                chunks.add(0x01);
        }
        
        public void write()
        {
            M.writeMultibyte(id);
            
            if (chunks.wasNext(0x01))
                M.writeString(animationName, M.S_TOTRANSLATE);
            if (chunks.wasNext(0x02))
                M.writeString(animationFile, M.S_FILENAME);
            if (chunks.wasNext(0x03))
                M.writeLengthBool(largeAnimation);
            
            if (chunks.wasNext(0x06))
                M.writeList<AnimationTiming>(seFlashTimings);
            
            if (chunks.wasNext(0x09))
                M.writeLengthMultibyte(scope);
            if (chunks.wasNext(0x0a))
                M.writeLengthMultibyte(yPosition);
            
            if (chunks.wasNext(0x0c))
                M.writeList<AnimationFrame>(frames);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
        
        public bool isBlank()
        {
            if (animationName != "" // 01
             || animationFile != "" // 02
             || largeAnimation // 03
             || seFlashTimings.Count > 0 // 06
             || scope != 0 // 09
             || yPosition != 2 // 0a
             || frames.Count != 20) // 0c
                return false;
            
            foreach (AnimationFrame frame in frames)
                if (!frame.isBlank())
                    return false;
            
            return true;
        }
    }
    
    class AnimationTiming : RPGByteData
    {
        int id = 0;
        int frame = 0; // 01
        Audio sound; // 02
        int flashRange = 0; // 03
        int flashRed = 31; // 04
        int flashGreen = 31; // 05
        int flashBlue = 31; // 06
        int flashPower = 31; // 07
        int shakeType = 0; // 08 (2003)
        
        List<int> chunkList;
        
        static string[] shakeTypes = { "No Shake", "Shake Target", "Shake Screen" };
        
        public AnimationTiming(FileStream f)
        {
            load(f);
        }
        public AnimationTiming()
        {
        }
        
        override public void load(FileStream f)
        {
            chunkList = new List<int>();
            
            id = M.readMultibyte(f);
            
            if (Chunks.next(f, 0x01, chunkList))
                frame = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x02, chunkList))
                sound = new Audio(f);
            if (Chunks.next(f, 0x03, chunkList))
                flashRange = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x04, chunkList))
                flashRed = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x05, chunkList))
                flashGreen = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x06, chunkList))
                flashBlue = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x07, chunkList))
                flashPower = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x08, chunkList))
                shakeType = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            string soundStr = sound.getString();
            return "<" + frame + ">"
                + (soundStr != "(OFF)"? " " + soundStr + (flashRange > 0? "," : "") : "")
                + (flashRange > 0? " Flash R" + flashRed + " G" + flashGreen + " B" + flashBlue + " P" + flashPower : "")
                + (M.is2003? shakeTypes[shakeType] : "");
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (Chunks.wasNext(0x01, chunkList))
                M.writeLengthMultibyte(frame);
            if (Chunks.wasNext(0x02, chunkList))
                sound.write();
            if (Chunks.wasNext(0x03, chunkList))
                M.writeLengthMultibyte(flashRange);
            if (Chunks.wasNext(0x04, chunkList))
                M.writeLengthMultibyte(flashRed);
            if (Chunks.wasNext(0x05, chunkList))
                M.writeLengthMultibyte(flashGreen);
            if (Chunks.wasNext(0x06, chunkList))
                M.writeLengthMultibyte(flashBlue);
            if (Chunks.wasNext(0x07, chunkList))
                M.writeLengthMultibyte(flashPower);
            if (Chunks.wasNext(0x08, chunkList))
                M.writeLengthMultibyte(shakeType);
            
            M.writeByte(0x00);
        }
    }
    
    class AnimationFrame : RPGByteData
    {
        int id = 0;
        List<AnimationCell> cells; // 01
        
        List<int> chunkList;
        
        public AnimationFrame(FileStream f)
        {
            load(f);
        }
        public AnimationFrame()
        {
        }
        
        override public void load(FileStream f)
        {
            chunkList = new List<int>();
            
            id = M.readMultibyte(f);
            
            if (Chunks.next(f, 0x01, chunkList))
                cells = M.readList<AnimationCell>(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            StringWriter cellListStr = new StringWriter(new StringBuilder());
            
            for (int i = 0; i < cells.Count; i++)
            {
                cellListStr.Write(cells[i].getString());
                if (i < cells.Count - 1)
                    cellListStr.Write(" / ");
            }
            
            return cellListStr.ToString();
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (Chunks.wasNext(0x01, chunkList))
                M.writeList<AnimationCell>(cells);
            
            M.writeByte(0x00);
        }
        
        public bool isBlank()
        {
            if (cells.Count > 0)
                return false;
            
            return true;
        }
    }
    
    class AnimationCell : RPGByteData
    {
        int id = 0;
        int valid = 1; // 01, verbose only
        int pattern = 0; // 02
        int x = 0; // 03
        int y = 0; // 04
        int magnify = 100; // 05
        int red = 100; // 06
        int green = 100; // 07
        int blue = 100; // 08
        int chroma = 100; // 09
        int transparency = 0; // 0a
        
        List<int> chunkList;
        
        public AnimationCell(FileStream f)
        {
            load(f);
        }
        public AnimationCell()
        {
        }
        
        override public void load(FileStream f)
        {
            chunkList = new List<int>();
            
            id = M.readMultibyte(f);
            
            if (Chunks.next(f, 0x01, chunkList))
                valid = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x02, chunkList))
                pattern = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x03, chunkList))
                x = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x04, chunkList))
                y = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x05, chunkList))
                magnify = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x06, chunkList))
                red = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x07, chunkList))
                green = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x08, chunkList))
                blue = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x09, chunkList))
                chroma = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x0a, chunkList))
                transparency = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            return "[" + id + "] Pattern " + (pattern + 1) + " (" + x + "," + y + ") "
                 + "M" + magnify + "% T" + transparency + "% R" + red + " G" + green + " B" + blue + " S" + chroma
                 + (M.superVerboseStrings? (" (Valid: " + valid + ")") : "");
        }
        
        override protected void myWrite()
        {
            M.writeMultibyte(id);
            
            if (Chunks.wasNext(0x01, chunkList))
                M.writeLengthMultibyte(valid);
            if (Chunks.wasNext(0x02, chunkList))
                M.writeLengthMultibyte(pattern);
            if (Chunks.wasNext(0x03, chunkList))
                M.writeLengthMultibyte(x);
            if (Chunks.wasNext(0x04, chunkList))
                M.writeLengthMultibyte(y);
            if (Chunks.wasNext(0x05, chunkList))
                M.writeLengthMultibyte(magnify);
            if (Chunks.wasNext(0x06, chunkList))
                M.writeLengthMultibyte(red);
            if (Chunks.wasNext(0x07, chunkList))
                M.writeLengthMultibyte(green);
            if (Chunks.wasNext(0x08, chunkList))
                M.writeLengthMultibyte(blue);
            if (Chunks.wasNext(0x09, chunkList))
                M.writeLengthMultibyte(chroma);
            if (Chunks.wasNext(0x0a, chunkList))
                M.writeLengthMultibyte(transparency);
            
            M.writeByte(0x00);
        }
    }
}
