using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Event : RPGByteData
    {
        int eventNum = 0;
        string eventName = ""; // 01
        int eventX = 0; // 02
        int eventY = 0; // 03
        List<Page> pages; // 05
        
        static string myClass = "Event";
        Chunks chunks;
        
        public Event(FileStream f)
        {
            load(f);
        }
        public Event()
        {
        }
        
        // Loads a single event within a map.
        override public void load(FileStream f)
        {
            chunks = new Chunks(f, myClass);
            
            eventNum = M.readMultibyte(f);
            
            if (chunks.next(0x01))
                eventName = M.readString(f, M.S_UNTRANSLATED);
            
            if (chunks.next(0x02))
                eventX = M.readLengthMultibyte(f);
            if (chunks.next(0x03))
                eventY = M.readLengthMultibyte(f);
            
            M.currentEvent = "Event " + eventNum + " (" + eventX + "," + eventY + ")";
            M.currentEventNum = eventNum;
            
            if (chunks.next(0x05))
                pages = M.readList<Page>(f, "Page"); // Argument makes it set currentPage/currentPageNum before loading each page
            
            M.byteCheck(f, 0x00);
        }
        
        // Returns event string.
        override public string getString()
        {
            M.currentEventNum = eventNum;
            M.currentEvent = eventNum.ToString();
            
            StringWriter eventHeader = new StringWriter(new StringBuilder());
            StringWriter eventText = new StringWriter(new StringBuilder());
            
            if (!M.stringScriptExportMode)
            {
                string namePart = M.includeMessages? (" - " + eventName) : "";
                eventHeader.WriteLine("***** Event #" + eventNum + " (" + eventX + "," + eventY + ")" + namePart + " *****");
            }
            else
            {
                bool separateName = M.getExtraneousSetting("MapEventNames");
                eventHeader.WriteLine("**********Event" + eventNum + "**********"
                    + (M.getDetailSetting("EventCoordinates")? " (" + eventX + "," + eventY + ")" : "")
                    + (M.getDetailSetting("EventName") && !separateName? " - " + eventName : ""));
                
                if (separateName)
                    eventHeader.WriteLine(M.databaseExportString("EventName", eventName));
            }
            
            for (int i = 0; i < pages.Count; i++)
            {
                M.wroteStringInPage = false;
                string thisPageText = pages[i].getString();
                
                // Include all pages in actions mode, but otherwise, leave out pages with no content (and in export, check that it's worthwhile content).
                if (M.wroteStringInPage || !M.stringScriptExportMode)
                    if (M.includeActions || thisPageText.ToString() != "")
                        eventText.WriteLine(thisPageText);
            }
            
            if (M.includeActions || eventText.ToString() != "") // In message-only mode, don't include header if content is blank.
                return eventHeader.ToString() + eventText.ToString();
            else
                return "";
        }
        
        // Replaces strings from importingStringArgs.
        public void importStrings()
        {
            M.currentEvent = "Event " + eventNum;
            M.currentEventNum = eventNum;
            
            M.importDatabaseString(eventNum, 0, "EventName", ref eventName);
            if (eventName != "")
                chunks.add(0x01);
            
            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].importStrings();
                M.checkIfImportingStringsExhausted(eventNum, i + 1);
                M.checkIfCommandValuesExhausted(eventNum, i + 1);
            }
        }
        
        // Writes event data, to parent writer by default, and returns the byte size of that data.
        override protected void myWrite()
        {
            M.writeMultibyte(eventNum);
            
            if (chunks.wasNext(0x01))
                M.writeString(eventName, M.S_UNTRANSLATED);
            
            if (chunks.wasNext(0x02))
                M.writeLengthMultibyte(eventX);
            if (chunks.wasNext(0x03))
                M.writeLengthMultibyte(eventY);
                
            if (chunks.wasNext(0x05))
                M.writeList<Page>(pages);
            
            M.writeByte(0x00);
            
            chunks.validateParity();
        }
    }
}
