using System.Collections.Generic;
using System.IO;

namespace RPGRewriter
{
    // A wrapper for AudioContents that incorporates the preceding byte length, so that not everything using Audio has to.
    class Audio : RPGData
    {
        AudioContents audio;
        
        public bool isMusic = false;
        
        public Audio(FileStream f, bool music = false)
        {
            isMusic = music;
            load(f);
        }
        public Audio()
        {
        }
        
        public void load(FileStream f)
        {
            M.readMultibyte(f); // Length
            audio = new AudioContents(f, isMusic);
        }
        
        public string getString()
        {
            return audio.getString();
        }
        
        public void write()
        {
            M.writeMultibyte(audio.getLength());
            audio.write();
        }
        
        public bool isBlank()
        {
            return audio.isBlank();
        }
        
        public void replaceFilenames()
        {
            audio.replaceFilenames();
        }
    }
    
    // The actual audio content.
    class AudioContents : RPGByteData
    {
        string sound = "(OFF)"; // 01
        int fade = 0; // 02
        int volume = 100; // 03
        int tempo = 100; // 04
        int balance = 0; // 05
        
        public bool isMusic = false;
        
        List<int> chunkList;
        
        public AudioContents(FileStream f, bool music = false)
        {
            isMusic = music;
            load(f);
        }
        public AudioContents()
        {
        }
        
        override public void load(FileStream f)
        {
            chunkList = new List<int>();
            
            if (Chunks.next(f, 0x01, chunkList))
                sound = M.readStringAndRewrite(f, isMusic? M.M_MUSIC : M.M_SOUND, M.S_FILENAME);
            if (Chunks.next(f, 0x02, chunkList))
                fade = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x03, chunkList))
                volume = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x04, chunkList))
                tempo = M.readLengthMultibyte(f);
            if (Chunks.next(f, 0x05, chunkList))
                balance = M.readLengthMultibyte(f);
            
            M.byteCheck(f, 0x00);
        }
        
        override public string getString()
        {
            return sound != "(OFF)" && sound != ""? sound
                                                  + (chunkList.Contains(0x02)? ", Fade " + (fade / 1000) + " sec" : "") + ", "
                                                  + M.getSoundVTB(volume, tempo, balance + 50)
                                                  : "(OFF)";
        }
        
        override protected void myWrite()
        {
            if (Chunks.wasNext(0x01, chunkList))
                M.writeString(sound, M.S_FILENAME);
            if (Chunks.wasNext(0x02, chunkList))
                M.writeLengthMultibyte(fade);
            if (Chunks.wasNext(0x03, chunkList))
                M.writeLengthMultibyte(volume);
            if (Chunks.wasNext(0x04, chunkList))
                M.writeLengthMultibyte(tempo);
            if (Chunks.wasNext(0x05, chunkList))
                M.writeLengthMultibyte(balance);
            
            M.writeByte(0x00);
        }
        
        public bool isBlank()
        {
            if (sound != "(OFF)" // 01
             || fade != 0 // 02
             || volume != 100 // 03
             || tempo != 100 // 04
             || balance != 0) // 05
                return false;
            
            return true;
        }
        
        public void replaceFilenames()
        {
            sound = M.rewriteString(M.M_SOUND, sound);
        }
    }
}
