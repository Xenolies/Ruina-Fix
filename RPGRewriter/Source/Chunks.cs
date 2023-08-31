using System;
using System.Collections.Generic;
using System.IO;

namespace RPGRewriter
{
    class Chunks
    {
        FileStream f;
        string className = "";
        List<byte> chunkList;
        List<byte> chunkCheckList;
        int lastReadChunk = 0;
        
        public Chunks(FileStream f, string className)
        {
            this.f = f;
            this.className = className;
        }
        
        // Returns whether chunk was used.
        public bool used(byte b)
        {
            if (chunkList == null)
                return false;
            
            return chunkList.Contains(b);
        }
        
        // Adds a chunk as used. Should be called externally if modifications to data add a chunk where it may have been absent before.
        public void add(byte b)
        {
            if (chunkList == null)
                chunkList = new List<byte>();
            
            if (!chunkList.Contains(b))
                chunkList.Add(b);
        }
        
        // Removes a chunk, usually for testing parity on write.
        public void remove(byte b)
        {
            if (chunkList == null)
                return;
            
            chunkList.Remove(b);
        }
        
        // Checks the next byte. If it matches, it returns true. Otherwise, it acts as if it were never read. Adds the byte to usedList if used.
        public bool next(byte b)
        {
            if (M.confirmChunkParity)
            {
                for (byte i = (byte)(lastReadChunk + 1); i < b; i++)
                    if (M.skipChunk(f, i).Length > 0)
                        Console.WriteLine("Warning: Chunk " + M.hex(i, 2) + " not implemented! (" + className + ")");
                lastReadChunk = b;
            }
            
            if (chunkCheckList == null)
                chunkCheckList = new List<byte>();
            chunkCheckList.Add(b);
            
            byte readB = M.readByte(f);
            if (readB >= 128)
            {
                f.Seek(-1, SeekOrigin.Current);
                readB = (byte)M.readMultibyte(f);
            }
            
            if (b == readB)
            {
                add(b);
                return true;
            }
            else
            {
                f.Seek(-M.countMultibyte(readB), SeekOrigin.Current);
                return false;
            }
        }
        
        // Checks if the byte code was used, according to the list; if so, writes it to the target and returns true.
        public bool wasNext(byte b)
        {
            if (M.confirmChunkParity)
            {
                if (chunkCheckList.Contains(b))
                    chunkCheckList.Remove(b);
                else
                    Console.WriteLine("Warning: " + M.hex(b) + " is being written, but was not read. (" + className + ")");
            }
            
            if (used(b))
            {
                M.writeMultibyte(b);
                return true;
            }
            return false;
        }
        
        // When confirming chunk parity, checks whether usedList contains any entries over 0x80, indicating a chunk that was read but not written.
        public void validateParity()
        {
            if (!M.confirmChunkParity)
                return;
            
            if (chunkCheckList != null)
                foreach (byte b in chunkCheckList)
                    Console.WriteLine("Warning: " + M.hex(b) + " was read, but not written. (" + className + ")");
        }
        
        // Barebones static versions using external lists, for classes that are too numerous and thus cause too much overhead.
        public static bool next(FileStream f, byte b, List<int> usedList)
        {
            byte readB = M.readByte(f);
            if (readB >= 128)
            {
                f.Seek(-1, SeekOrigin.Current);
                readB = (byte)M.readMultibyte(f);
            }
            
            if (b == readB)
            {
                usedList.Add(b);
                return true;
            }
            else
            {
                f.Seek(-M.countMultibyte(readB), SeekOrigin.Current);
                return false;
            }
        }
        public static bool wasNext(byte b, List<int> usedList)
        {
            if (usedList.Contains(b))
            {
                M.writeMultibyte(b);
                return true;
            }
            return false;
        }
    }
}
