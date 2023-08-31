using System.IO;

namespace RPGRewriter
{
    // A segment of data that can be loaded and rewrite its bytes.
    interface RPGData
    {
        // Loads data from file.
        void load(FileStream f);
        
        // Returns a string representation of the data.
        string getString();
        
        // Writes data back.
        void write();
    }
    
    // A variation of RPGData that is an entire data file.
    interface RPGDataFile
    {
        // Loads all data from file.
        bool loadFile(string filepath, bool writeLog = true);
        
        // Returns a string representation of the file.
        string getString();
        
        // Writes data back as the same file.
        bool writeFile();
    }
    
    // A variation of RPGData that writes its bytes to a dataBytes byte array, which may return its length or be written to the target file.
    abstract class RPGByteData
    {
        protected byte[] dataBytes; // Stores written data, to check byte length or write as required
        
        // Loads data from file.
        abstract public void load(FileStream f);
        
        // Returns a string representation of the data.
        abstract public string getString();
        
        // Writes to dataBytes if not already written, writes to parent writer if specified, and returns byte length of the data.
        public int write(bool writeToParent = true)
        {
            if (dataBytes == null) // Only need to write once to create dataBytes
            {
                BinaryWriter parentWriter = M.targetWriter;
                BinaryWriter dbData = new BinaryWriter(new MemoryStream());
                M.targetWriter = dbData;
                
                myWrite();
                
                M.targetWriter = parentWriter;
                dataBytes = (dbData.BaseStream as MemoryStream).ToArray();
                dbData.Close();
            }
            
            if (writeToParent)
                M.writeByteArrayNoLength(dataBytes);
            return dataBytes.Length;
        }
        
        // Writes back the data to dataBytes (because it's called in write() after targetWriter has been set to dataBytes).
        abstract protected void myWrite();
        
        // Returns the byte length of the data via a write() call.
        public int getLength()
        {
            return write(false);
        }
    }
    
    // A single entry in a list-based database.
    interface RPGDatabaseEntry : RPGData
    {
        // Loads data from file.
        new void load(FileStream f);
        
        // Writes back the data.
        new void write();
        
        // Returns whether the database entry is "blank" (default). Blank entries will not be considered "unused" in Checking.
        bool isBlank();
    }
}
