using System.IO;

namespace RPGRewriter
{
    class MoveStep
    {
        int id;
        int value;
        int value2;
        int value3;
        string stringArg;
        string source;
        
        byte[] dataBytes;
        
        static string[] moveStr = { "Up", "Right", "Down", "Left", "Right-Up",
                                    "Right-Down", "Left-Down", "Left-Up", "Random Step", "Toward Hero",
                                    "Away Hero", "Forward", "Face Up", "Face Right", "Face Down",
                                    "Face Left", "Turn Right", "Turn Left", "About Face", "Turn Right/Left",
                                    "Face Random", "Face Hero", "Face Away Hero", "Wait", "Start Jump",
                                    "End Jump", "Fix Dir", "Unfix Dir", "Speed Up", "Speed Down",
                                    "Freq-Up", "Freq-Down", "[Switch On]", "[Switch Off]", "[CharSet]",
                                    "[Sound]", "Slip-Thru", "Unslip-Thru", "Stop Anim", "Resume Anim",
                                    "Transp-Up", "Transp-Down" };
       
        public MoveStep(FileStream f, ref int lengthTemp, string source)
        {
            load(f, ref lengthTemp, source);
        }
        public MoveStep()
        {
        }
        
        // Loads data for one move route step.
        public void load(FileStream f, ref int lengthTemp, string source)
        {
            this.source = source;
            
            id = M.readByte(f);
            
            // loadMoveRoute increments i up to lengthTemp, but also subtract from lengthTemp for additional bytes read.
            if (id == 0x20 || id == 0x21) // Switch On/Off
            {
                if (source != "Custom")
                {
                    value = M.readByte(f); // Switch Number
                    lengthTemp--;
                    
                    if (value == 129) // Weird fake-multibyte, NOT used in custom routes. Seriously, what.
                    {
                        value += (M.readByte(f) - 1) * 128;
                        value += M.readByte(f) - 1;
                        lengthTemp--;
                    }
                }
                else
                {
                    value = M.readMultibyte(f); // Switch Number
                    lengthTemp -= M.countMultibyte(value);
                }
            }
            else if (id == 0x22) // CharSet Change
            {
                stringArg = M.readStringMoveAndRewrite(f, M.M_CHARSET, ref lengthTemp, M.S_FILENAME, source);
                value = M.readByte(f); // Index
                lengthTemp--;
            }
            else if (id == 0x23) // Sound
            {
                stringArg = M.readStringMoveAndRewrite(f, M.M_SOUND, ref lengthTemp, M.S_FILENAME, source);
                value = M.readByte(f); // Volume
                lengthTemp--;
                
                if (source != "Custom")
                {
                    value2 = M.readByte(f); // Tempo
                    lengthTemp--;
                    if (value2 == 129) // Fake Multibyte
                    {
                        M.byteCheck(f, 0x01);
                        value2 += M.readByte(f) - 1;
                        lengthTemp--;
                    }
                }
                else
                {
                    value2 = M.readMultibyte(f); // Tempo
                    lengthTemp -= M.countMultibyte(value2);
                }
                
                value3 = M.readByte(f); // Balance
                lengthTemp--;
            }
        }
        
        // Returns move step string.
        public string getString()
        {
            string moveString = moveStr[id];
            
            if (id == 0x20) // Switch On
                moveString = "Switch " + M.getDataSwitch(value) + " On";
            else if (id == 0x21) // Switch Off
                moveString = "Switch " + M.getDataSwitch(value) + " Off";
            else if (id == 0x22) // CharSet Change
                moveString = "CharSet " + stringArg + " Index " + (value + 1);
            else if (id == 0x23) // Sound
                moveString = "Sound " + stringArg + ", " + M.getSoundVTB(value, value2, value3);
            
            return "- " + moveString;
        }
        
        // Writes move step data, to parent writer by default, and returns the byte size of that data.
        public int write(ref int lengthMinus, bool writeToParent = true)
        {
            if (dataBytes == null) // Only need to write once to create dataBytes
            {
                BinaryWriter parentWriter = M.targetWriter;
                BinaryWriter moveStepData = new BinaryWriter(new MemoryStream());
                M.targetWriter = moveStepData;
                
                M.writeByte(id);
                
                if (id == 0x20 || id == 0x21) // Switch On/Off
                {
                    if (source != "Custom")
                    {
                        if (value < 128) // Switch Number
                            M.writeByte(value);
                        else // Weird fake-multibyte, NOT used in custom routes. Seriously, what.
                        {
                            M.writeByte(0x81);
                            M.writeByte(M.div(value, 128));
                            M.writeByte(value % 128);
                            lengthMinus++; // One of these bytes doesn't get counted in length
                        }
                    }
                    else
                        M.writeMultibyte(value); // Switch Number
                }
                else if (id == 0x22) // CharSet Change
                {
                    lengthMinus += M.writeString(stringArg, M.S_FILENAME, source);
                    M.writeByte(value); // Index
                }
                else if (id == 0x23) // Sound
                {
                    lengthMinus += M.writeString(stringArg, M.S_FILENAME, source);
                    M.writeByte(value); // Volume
                    
                    if (source != "Custom")
                    {
                        if (value2 < 128) // Tempo
                            M.writeByte(value2);
                        else // Fake Multibyte
                        {
                            M.writeByte(0x81);
                            M.writeByte(0x01);
                            M.writeByte(value2 % 128);
                            lengthMinus++; // One of these bytes doesn't get counted in length
                        }
                    }
                    else
                        M.writeMultibyte(value2); // Tempo
                    
                    M.writeByte(value3); // Balance
                }
                
                M.targetWriter = parentWriter;
                dataBytes = (moveStepData.BaseStream as MemoryStream).ToArray();
                moveStepData.Close();
            }
            
            if (writeToParent)
                M.writeByteArrayNoLength(dataBytes);
            return dataBytes.Length;
        }
        
        // Replaces filename references.
        public void replaceFilenames()
        {
            if (id == 0x22) // CharSet Change
                stringArg = M.rewriteString(M.M_CHARSET, stringArg);
            else if (id == 0x23) // Sound
                stringArg = M.rewriteString(M.M_SOUND, stringArg);
        }
    }
}
