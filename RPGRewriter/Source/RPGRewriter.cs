using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace RPGRewriter
{
    class M
    {
        #region // Constants //
        
        // Constants for each resource folder and its mode.
        public const int M_BACKDROP = 0;
        public const int M_BATTLE = 1;
        public const int M_BATTLE2 = 2;
        public const int M_BATTLECHARSET = 3;
        public const int M_BATTLEWEAPON = 4;
        public const int M_CHARSET = 5;
        public const int M_CHIPSET = 6;
        public const int M_FACESET = 7;
        public const int M_FRAME = 8;
        public const int M_GAMEOVER = 9;
        public const int M_MONSTER = 10;
        public const int M_MOVIE = 11;
        public const int M_MUSIC = 12;
        public const int M_PANORAMA = 13;
        public const int M_PICTURE = 14;
        public const int M_SOUND = 15;
        public const int M_SYSTEM = 16;
        public const int M_SYSTEM2 = 17;
        public const int M_TITLE = 18;
        public const int FOLDERCOUNT = 19;
        
        // Constants for other modes.
        public const int M_MESSAGEALL = 19;
        public const int M_MESSAGEPART = 20;
        public const int M_MESSAGESTART = 21;
        public const int M_OPTION = 22;
        public const int M_NAME = 23;
        public const int M_NICKNAME = 24;
        public const int M_COMMENT = 25;
        public const int MODECOUNT = 26;
        
        // Constant for all folder/mode names.
        public static string[] FOLDER = {
                                            "Backdrop",
                                            "Battle",
                                            "Battle2",
                                            "BattleCharSet",
                                            "BattleWeapon",
                                            "CharSet",
                                            "ChipSet",
                                            "FaceSet",
                                            "Frame",
                                            "GameOver",
                                            "Monster",
                                            "Movie",
                                            "Music",
                                            "Panorama",
                                            "Picture",
                                            "Sound",
                                            "System",
                                            "System2",
                                            "Title",
                                            "MessageAll",
                                            "MessagePart",
                                            "MessageStart",
                                            "Option",
                                            "Name",
                                            "Nickname",
                                            "Comment"
                                        };
        
        // Constants for string categories.
        public static int S_TOTRANSLATE = 0;
        public static int S_UNTRANSLATED = 1;
        public static int S_FILENAME = 2;
        public static int S_CONSTANT = 3;
        #endregion
        
        #region // Variables //
        
        // References for encodings.
        public static Encoding UNICODE;
        public static Encoding[] readEncodings;
        public static Encoding[] writeEncodings;
        public static int[] readEncodingIDs;
        public static int[] writeEncodingIDs;
        
        // Indicators of "where" the program currently is.
        public static string gamePath = "";
        public static string currentFile = "";
        public static string currentEvent = "";
        public static string currentPage = "";
        public static string currentLine = "";
        public static int currentEventNum = 0;
        public static int currentPageNum = 0;
        public static int currentBattlerAnimation = 0;
        public static SaveFile saveRef;
        
        // Game version determined from LDB.
        public static bool is2003 = false; // Version 0
        public static bool is2000EN = false; // Version 1
        
        // User options.
        public static string globalMode = "Extracting";
        public static bool allFiles = true;
        public static bool includeMessages = true;
        public static bool includeActions = true;
        public static bool includeDataNames = false;
        public static bool useRewrittenStrings = false;
        public static bool checkUnusedFiles = false;
        public static bool checkLineLength = false;
        public static bool checkUnusedData = false;
        public static bool blankOutMessages = true;
        
        public static bool superVerboseStrings = false;
        public static int ignoreLengthLimits = 0;
        public static int forceEngineVersion = 0;
        
        public static int functionListPage = 0;
        
        // Variables for various modes/options for the independent functions.
        public static bool comparisonMode = false;
        public static bool completenessMode = false;
        public static bool readingOriginal = true;
        public static bool allowAlternateTrans = false;
        public static bool completenessReplacement = false;
        public static bool compilingUnique = false;
        public static bool messageUniqueSource = false;
        public static bool messageUniqueSourceMulti = false;
        public static bool compilingDuplicates = false;
        public static bool copyingTileData = false;
        public static bool stringScriptExportMode = false;
        public static bool stringScriptImportMode = false;
        public static bool stringScriptImportCheck = false;
        public static bool extractDoubleMode = false;
        public static bool copyingSwitchVariable = false;
        public static bool copyingCommandValues = false;
        public static bool commandValueChangesMade = false;
        
        // Variables for running with command line arguments.
        public static bool commandLineMode = false;
        public static string commandLineFunction = "";
        public static string projectFile1 = "";
        public static string projectFile2 = "";
        public static bool cmdCompletenessReplacement = true;
        public static int cmdUniqueSources = 0;
        public static int cmdGameMode = 0;
        public static bool cmdFileListNonASCII = true;
        public static bool cmdIsolateUnusedFiles = false;
        
        // General variables for rewriting.
        public static BinaryWriter targetWriter;
        public static List<string>[,] transList;
        public static string lastInputFile = "N/A";
        public static bool changesMade = false;
        
        // Log for missing translations, non-existing files, etc.
        public static List<string> logStrings;
        public static bool logExists = false;
        
        public static Dictionary<string, List<string>> missingFiles;
        public static Dictionary<string, List<string>> unusedFiles;
        public static Dictionary<string, List<string>> unusedFilesLowercase;
        public static Dictionary<string, List<string>> sameNameFiles;
        public static Dictionary<string, List<string>> sameNameFilesLowercase;
        public static Dictionary<string, List<int>> unusedDataEntries;
        public static List<string> variableReferencedDatabases;
        
        // Variables for partial loading of LDB.
        public static bool gettingLDBVersion = false;
        public static bool readingDataNames = false;
        public static bool makingDataEntryLists = false;
        
        // Variables tracking command context.
        public static bool messagePreceded = false;
        public static bool messageFaceOn = false;
        public static bool lastWroteAMessage = false;
        public static bool wroteStringInPage = false;
        public static string fullMessageHeader = "";
        public static string fullMessageContent = "";
        
        // Variables to track what issues came up in Checking mode.
        public static bool checkIssueReference = false;
        public static bool checkIssueUnusedFile = false;
        public static bool checkIssueUnusedDatabase = false;
        public static bool checkIssueMessage = false;
        public static bool checkIssueOverflow = false;
        
        // Database name storage.
        public static string[] heroNames = { };
        public static string[] skillNames = { };
        public static string[] itemNames = { };
        public static string[] monsterNames = { };
        public static string[] troopNames = { };
        public static string[] attributeNames = { };
        public static string[] conditionNames = { };
        public static string[] animationNames = { };
        public static string[] terrainNames = { };
        public static string[] chipSetNames = { };
        public static string[] commonNames = { };
        public static string[] mapNames = { };
        public static string[] switchNames = { };
        public static string[] variableNames = { };
        public static string[] battleCommandNames = { };
        public static string[] classNames = { };
        public static string[] battlerAnimSetNames = { };
        public static string[][] battlerPoseNames = { };
        public static string[][] weaponAnimationNames = { };
        
        // For translation consistency check function.
        public static Dictionary<string, Dictionary<int, Dictionary<int, List<string>>>> origStringList;
        public static Dictionary<string, Dictionary<int, Dictionary<int, List<string>>>> transStringList;
        public static Dictionary<string, Dictionary<int, Dictionary<int, string>>> sourceLocationString;
        public static Dictionary<string, string> translationBook;
        public static Dictionary<string, List<string>> translationAlt;
        public static Dictionary<string, string> translationSource;
        public static Dictionary<string, List<string>> translationAltSource;
        public static StringWriter completeMessage;
        
        // For unique/duplicate message functions.
        public static List<string> uniqueList;
        public static List<List<string>> uniqueListSources;
        public static Dictionary<string, int> duplicateMessageList;
        public static int duplicateMessageHighest;
        
        // For tile copy function.
        public static Dictionary<string, int[][]> mapTileBytesLayer1;
        public static Dictionary<string, int[][]> mapTileBytesLayer2;
        
        // For action command copy function.
        public static Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<int, List<int[]>>>>> actionCommandValues;
        
        // For string importing function.
        public static Dictionary<int, Dictionary<int, Dictionary<string, List<string>>>> importingStringArgs;
        
        // For original string function.
        public static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> originalStringDB;
        public static bool generatingOriginalStringDB = false;
        
        // User settings in external file.
        public static Dictionary<string, int> userSettings;
        public static Dictionary<string, string> userSettingsStr;
        static List<string> detailSettings;
        static List<string> extraneousSettings;
        
        // Special modes for specific games.
        public static bool ibMode = false;
        public static bool starUnknownMode = false;
        public static bool hanoiMode = false;
        public static string currentSpeaker = "";
        #endregion
        
        // Variables for various debug or development purposes.
        public static bool debugMode = false; // Shows debug messages (stack traces for exceptions).
        public static bool showLogMessages = false; // Makes log messages write to console as well.
        public static bool alwaysResaveFiles = false; // Makes Rewriting always save files, even if no changes were made. (For testing write validity.)
        public static bool confirmChunkParity = false; // Detects chunk mismatches between load() and write(). (Should enable alwaysResaveFiles.)
        
        [STAThread]
        static void Main(string[] args)
        {
            UNICODE = Encoding.Unicode;
            
            readEncodings = new Encoding[4];
            readEncodingIDs = new int[4];
            writeEncodings = new Encoding[4];
            writeEncodingIDs = new int[4];
            for (int i = 0; i < 4; i++)
            {
                readEncodings[S_CONSTANT] = Encoding.GetEncoding(932);
                readEncodingIDs[S_CONSTANT] = 932;
                writeEncodings[S_CONSTANT] = Encoding.GetEncoding(932);
                writeEncodingIDs[S_CONSTANT] = 932;
            }
            
            loadUserSettings(true);
            parseCommandLineArgs(args);
            
            loadTransList();
            
            bool menuShow = true;
            string menuOption;
            while (menuShow)
            {
                logStrings = new List<string>();
                
                checkIssueReference = false;
                checkIssueUnusedFile = false;
                checkIssueUnusedDatabase = false;
                checkIssueMessage = false;
                checkIssueOverflow = false;
                
                saveRef = null;
                
                if (!commandLineMode)
                    loadUserSettings();
                
                string infoString = "";
                if (readEncodingIDs[S_TOTRANSLATE] != 932)
                    infoString += "Read: " + readEncodingIDs[S_TOTRANSLATE];
                if (writeEncodingIDs[S_TOTRANSLATE] != 932)
                    infoString += (infoString != ""? " / " : "") + "Write: " + writeEncodingIDs[S_TOTRANSLATE];
                if (ibMode)
                    infoString += (infoString != ""? " / " : "") + "Ib Mode";
                if (starUnknownMode)
                    infoString += (infoString != ""? " / " : "") + "Star Unknown Mode";
                if (hanoiMode)
                    infoString += (infoString != ""? " / " : "") + "HANOI Mode";
                if (infoString != "")
                    infoString = " [" + infoString + "]";
                
                if (!commandLineMode)
                {
                    Console.Clear();
                    Console.WriteLine("RPG Maker 200X Translation Assistant" + infoString + "\n"
                        + "Enter a number to toggle that option.\n"
                        + "1. " + globalMode + "\n"
                        + "2. " + (allFiles? "All Files" : "Single File") + "\n"
                        + "3. " + (globalMode == "Extracting"?
                                   (includeMessages? "Including Messages" : "Skipping Messages")
                                 : globalMode == "Checking"?
                                   (includeMessages? "Check Message Validity" : "Don't Check Message Validity")
                                 : "-----") + "\n"
                        + "4. " + (globalMode == "Extracting"?
                                   (includeActions? "Including Actions" : "Skipping Actions")
                                 : globalMode == "Checking"?
                                   (includeActions? "Check File References" : "Don't Check File References")
                                 : "-----") + "\n"
                        + "5. " + (globalMode == "Extracting"?
                                   (includeDataNames? "Using Names for Data" : "Using Numbers for Data")
                                 : globalMode == "Checking" && allFiles?
                                   (checkUnusedFiles? "Check Unused Files" : "Don't Check Unused Files")
                                 : "-----") + "\n"
                        + "6. " + (globalMode == "Extracting"?
                                   (useRewrittenStrings? "Use Rewritten Strings" : "Keep Original Strings")
                                 : globalMode == "Checking" && allFiles?
                                   (checkUnusedData? "Check Unused Data Entries" : "Don't Check Unused Data Entries")
                                 : "-----") + "\n"
                        + "7. " + (globalMode == "Extracting" && !includeMessages?
                                   (!blankOutMessages? "Omit Messages Entirely" : "Blank Out Messages")
                                 : globalMode == "Checking" && includeMessages?
                                   (checkLineLength? "Check Line Lengths" : "Don't Check Line Lengths")
                                 : "-----") + "\n"
                        + "\n"
                        + "Z. Go!");
                    
                    if (functionListPage == 0)
                        Console.WriteLine("A. Load replacement list file (Current list: " + lastInputFile + ")\n"
                            + "Q. Translation consistency checker\n"
                            + "W. Translation completeness checker\n"
                            + "S. Compile duplicate message list\n"
                            + "E. Extract only unique messages\n"
                            + "C. Copy map tiles from one project to another\n"
                            + "T. Export editable scripts\n"
                            + "G. Import editable scripts\n"
                            + "?. (See additional functions)\n"
                            + "X. Exit");
                    else if (functionListPage == 1)
                        Console.WriteLine("D. Extract map tile data\n"
                            + "R. Set special game mode (for extraction)\n"
                            + "F. Generate resource file list\n"
                            + "V. Rename resource files\n"
                            + "Y. Export two-in-one message scripts\n"
                            + "H. Copy switch/variable names between projects\n"
                            + "N. Copy action command values between projects\n"
                            + "?. (Back to common functions)\n"
                            + "X. Exit");
                    
                    menuOption = Console.ReadLine().ToUpper();
                }
                else
                    menuOption = commandLineFunction.ToUpper();
                
                string filepath, filepath2;
                switch (menuOption)
                {
                    case "1":
                        if (globalMode == "Extracting")
                            globalMode = "Rewriting";
                        else if (globalMode == "Rewriting")
                            globalMode = "Checking";
                        else if (globalMode == "Checking")
                            globalMode = "Extracting";
                        break;
                    case "2":
                        allFiles = !allFiles;
                        break;
                    case "3":
                        includeMessages = !includeMessages;
                        break;
                    case "4":
                        includeActions = !includeActions;
                        break;
                    case "5":
                        if (globalMode != "Checking")
                            includeDataNames = !includeDataNames;
                        else
                            checkUnusedFiles = !checkUnusedFiles;
                        break;
                    case "6":
                        if (globalMode != "Checking")
                            useRewrittenStrings = !useRewrittenStrings;
                        else
                            checkUnusedData = !checkUnusedData;
                        break;
                    case "7":
                        if (globalMode != "Checking")
                            blankOutMessages = !blankOutMessages;
                        else
                            checkLineLength = !checkLineLength;
                        break;
                    case "9":
                    case "0":
                        Console.WriteLine("Enter a codepage to use for general string " + (menuOption == "9"? "reading" : "writing") + ".");
                        Console.WriteLine("(You can use any codepage number, not just the ones listed.)");
                        Console.WriteLine("Refer to Readme for help. Make backups if you don't know what you're doing.");
                        Console.WriteLine("932: Japanese/Basic English (Shift-JIS, Default)");
                        Console.WriteLine("1252: Single-Byte Latin (Spanish)");
                        Console.WriteLine("1250: Latin Extended (contains accented characters, etc.)");
                        Console.WriteLine("1251: Cryllic");
                        Console.WriteLine("874: Thai");
                        Console.WriteLine("950: Traditional Chinese (Big5)");
                        Console.WriteLine("936: Simplified Chinese");
                        Console.WriteLine("949: Korean Hangul");
                        Console.WriteLine("65001: Unicode (definitely won't work with standard RPG_RT.exe)");
                        
                        string newEncodingStr = Console.ReadLine();
                        int newEncoding;
                        if (int.TryParse(newEncodingStr, out newEncoding))
                        {
                            if (menuOption == "9")
                                tryToSetReadEncoding(newEncoding, S_TOTRANSLATE);
                            else if (menuOption == "0")
                                tryToSetWriteEncoding(newEncoding, S_TOTRANSLATE);
                        }
                        break;
                    
                    case "Z": // Go!
                        filepath = fileDialog();
                        if (filepath != "")
                        {
                            string fileExt = Path.GetExtension(filepath);
                            gamePath = Path.GetDirectoryName(filepath);
                            
                            if (globalMode != "Rewriting")
                                getLDBVersion();
                            if (checkUnusedFiles)
                                initializeUnusedFiles(filepath);
                            
                            if (allFiles)
                            {
                                if (globalMode == "Extracting")
                                {
                                    extractAll(filepath);
                                    Console.WriteLine("All files extracted.");
                                }
                                else if (globalMode == "Rewriting")
                                {
                                    if (rewriteAll(filepath))
                                        Console.WriteLine("All files rewritten.");
                                    else
                                        Console.WriteLine("Error rewriting files.");
                                }
                                else if (globalMode == "Checking")
                                {
                                    checkAll(filepath);
                                    if (logExists)
                                    {
                                        promptIsolateUnusedFiles();
                                        Console.WriteLine("Checked all files.");
                                    }
                                    else
                                        Console.WriteLine("Checked all files; no issues were found.");
                                }
                            }
                            else
                            {
                                string result = "";
                                string filename = Path.GetFileName(filepath);
                                
                                readInDataNames();
                                
                                if (fileExt == ".lmu")
                                    result = processMap(filepath);
                                else if (fileExt == ".lmt")
                                    result = processMapTree(filepath);
                                else if (fileExt == ".ldb")
                                    result = processDatabase(filepath);
                                else if (fileExt == ".lsd")
                                    result = processSaveFile(filepath);
                                else
                                    Console.WriteLine(fileExt + " extension not valid.");
                                
                                if (globalMode == "Extracting")
                                {
                                    if (result != "")
                                        clipboardSetText(result, filename + " extracted");
                                    else
                                        Console.WriteLine("Error extracting " + filename + ".");
                                }
                                else if (globalMode == "Checking")
                                {
                                    if (result != "")
                                    {
                                        writeFileIssues();
                                        if (logExists)
                                            Console.WriteLine("Checked " + filename + ".");
                                        else
                                            Console.WriteLine("Checked " + filename + "; no issues were found.");
                                    }
                                    else
                                        Console.WriteLine("Error checking " + filename + ".");
                                }
                            }
                            
                            enterToContinue();
                        }
                        break;
                    
                    case "A": // Load replacement list file
                        printFileList("", "*.txt");
                        Console.WriteLine("Enter input filename (leave blank for \"" + userSettingsStr["ReplacementInputFile"] + ".txt\"): ");
                        string inputFile = Console.ReadLine();
                        loadTransList(inputFile != ""? inputFile : userSettingsStr["ReplacementInputFile"]);
                        break;
                    
                    case "Q": // Translation consistency checker
                        filepath = fileDialog("compare");
                        if (filepath != "")
                        {
                            filepath2 = fileDialog("compare", true);
                            if (filepath2 != "")
                            {
                                string formerMode = globalMode;
                                globalMode = "Extracting";
                                
                                completenessMode = false;
                                compareAll(filepath, filepath2);
                                
                                enterToContinue();
                                
                                globalMode = formerMode;
                            }
                        }
                        break;
                    
                    case "W": // Translation completeness checker
                        if (!commandLineMode)
                        {
                            Console.WriteLine("Apply replacements from input file to original text? (Y/N)");
                            completenessReplacement = yesNoPrompt();
                        }
                        else
                            completenessReplacement = cmdCompletenessReplacement;
                        
                        filepath = fileDialog("compare");
                        if (filepath != "")
                        {
                            filepath2 = fileDialog("compare", true);
                            if (filepath2 != "")
                            {
                                string formerMode = globalMode;
                                globalMode = "Extracting";
                                
                                completenessMode = true;
                                compareAll(filepath, filepath2);
                                
                                enterToContinue();
                                
                                globalMode = formerMode;
                            }
                        }
                        break;
                    
                    case "S": // Compile duplicate message list
                        filepath = fileDialog("lmt");
                        if (filepath != "")
                        {
                            string formerMode = globalMode;
                            globalMode = "Extracting";
                            
                            gamePath = Path.GetDirectoryName(filepath);
                            string result = compileMessageDuplicates(filepath);
                            
                            clipboardSetText(result, "Duplicate message list extracted");
                            
                            enterToContinue();
                            
                            globalMode = formerMode;
                        }
                        break;
                    
                    case "E": // Extract only unique messages
                        if (!commandLineMode)
                        {
                            Console.WriteLine("Include message sources? (Y/N)");
                            messageUniqueSource = yesNoPrompt();
                            if (messageUniqueSource)
                            {
                                Console.WriteLine("Include all sources instead of just first? (Y/N)");
                                messageUniqueSourceMulti = yesNoPrompt();
                            }
                        }
                        else
                        {
                            messageUniqueSource = cmdUniqueSources > 0;
                            messageUniqueSourceMulti = cmdUniqueSources > 1;
                        }
                        
                        filepath = fileDialog("lmt");
                        if (filepath != "")
                        {
                            string formerMode = globalMode;
                            globalMode = "Extracting";
                            
                            gamePath = Path.GetDirectoryName(filepath);
                            string result = compileUniqueMessages(filepath);
                            
                            clipboardSetText(result, "All unique messages extracted");
                            
                            enterToContinue();
                            
                            globalMode = formerMode;
                        }
                        break;
                    
                    case "D": // Extract map tile data
                        filepath = fileDialog("lmt");
                        if (filepath != "")
                        {
                            string formerMode = globalMode;
                            globalMode = "Extracting";
                            
                            extractAllTiles(filepath);
                            
                            Console.WriteLine("Tile data text extracted to TileData.");
                            
                            enterToContinue();
                            
                            globalMode = formerMode;
                        }
                        break;
                    
                    case "C": // Copy map tiles from one project to another
                        filepath = fileDialog("copy");
                        if (filepath != "")
                        {
                            filepath2 = fileDialog("copy", true);
                            if (filepath2 != "")
                            {
                                string formerMode = globalMode;
                                
                                if (copyAllTileData(filepath, filepath2))
                                    Console.WriteLine("Destination project maps updated with tiles from source.");
                                else
                                    Console.WriteLine("Tiles matched source exactly, so no maps had to be updated.");
                                
                                enterToContinue();
                                
                                globalMode = formerMode;
                            }
                        }
                        break;
                    
                    case "R": // Set special game mode
                        ibMode = false;
                        starUnknownMode = false;
                        hanoiMode = false;
                        
                        string modeOption;
                        if (!commandLineMode)
                        {
                            Console.WriteLine("1. Ib (Show speaker names based on face sprites)\n"
                                + "2. Star Unknown (Show associated names for face commands)\n"
                                + "3. TOWER of HANOI (Show associated names for face commands)\n"
                                + "Other. Normal Mode");
                            
                            modeOption = Console.ReadLine();
                        }
                        else
                            modeOption = cmdGameMode.ToString();
                        
                        if (modeOption.Equals("1"))
                            ibMode = true;
                        else if (modeOption.Equals("2"))
                            starUnknownMode = true;
                        else if (modeOption.Equals("3"))
                            hanoiMode = true;
                        break;
                    
                    case "F": // Generate resource file list
                        bool nonASCIIOnly = true;
                        if (!commandLineMode)
                        {
                            Console.WriteLine("List only non-ASCII filenames (Y), or all files (N)?");
                            nonASCIIOnly = yesNoPrompt();
                        }
                        else
                            nonASCIIOnly = cmdFileListNonASCII;
                        
                        filepath = fileDialog("lmt");
                        if (filepath != "")
                        {
                            if (generateFilenames(filepath, nonASCIIOnly))
                                Console.WriteLine("filelist.txt generated.");
                            else
                                Console.WriteLine("Error generating file list.");
                            
                            enterToContinue();
                        }
                        break;
                    
                    case "V": // Rename resource files
                        filepath = fileDialog("lmt");
                        if (filepath != "")
                        {
                            string formerMode = globalMode;
                            globalMode = "Rewriting";
                            
                            if (translateFilenames(filepath))
                                Console.WriteLine("Files renamed.");
                            else
                                Console.WriteLine("No files needed renaming.");
                            
                            enterToContinue();
                            
                            globalMode = formerMode;
                        }
                        break;
                    
                    case "T": // Export editable scripts
                        filepath = fileDialog("lmt");
                        if (filepath != "")
                        {
                            string formerMode = globalMode;
                            globalMode = "Extracting";
                            
                            if (stringScriptExport(filepath))
                                Console.WriteLine("Exported editable files to StringScripts.");
                            
                            enterToContinue();
                            
                            globalMode = formerMode;
                        }
                        break;
                    
                    case "G": // Import editable scripts
                    case "G1": // Check validity in StringScripts
                        filepath = fileDialog("lmt");
                        if (filepath != "")
                        {
                            string formerMode = globalMode;
                            globalMode = "Rewriting";
                            
                            stringScriptImportCheck = menuOption.Equals("G1");
                            if (stringScriptImport(filepath))
                                Console.WriteLine("Strings imported from scripts.");
                            
                            enterToContinue();
                            
                            globalMode = formerMode;
                        }
                        break;
                    
                    case "Y": // Export two-in-one message scripts
                        filepath = fileDialog("combo");
                        if (filepath != "")
                        {
                            filepath2 = fileDialog("combo", true);
                            if (filepath2 != "")
                            {
                                string formerMode = globalMode;
                                globalMode = "Extracting";
                                
                                string result = extractDouble(filepath, filepath2);
                                
                                clipboardSetText(result, "Scripts extracted");
                                
                                enterToContinue();
                                
                                globalMode = formerMode;
                            }
                        }
                        break;
                    
                    case "H": // Copy switch/variable names between projects
                        filepath = fileDialog("copy");
                        if (filepath != "")
                        {
                            filepath2 = fileDialog("copy", true);
                            if (filepath2 != "")
                            {
                                string formerMode = globalMode;
                                
                                if (copySwitchVariableNames(filepath, filepath2))
                                    Console.WriteLine("Destination project updated with source switch/variable names.");
                                else
                                    Console.WriteLine("Names matched source exactly, so nothing had to be updated.");
                                
                                enterToContinue();
                                
                                globalMode = formerMode;
                            }
                        }
                        break;
                        
                        case "N": // Copy action command values between project
                        filepath = fileDialog("copy");
                        if (filepath != "")
                        {
                            filepath2 = fileDialog("copy", true);
                            if (filepath2 != "")
                            {
                                string formerMode = globalMode;
                                
                                if (copyActionCommandValues(filepath, filepath2))
                                    Console.WriteLine("Destination project updated with source command values.");
                                else
                                    Console.WriteLine("No command values needed updating.");
                                
                                enterToContinue();
                                
                                globalMode = formerMode;
                            }
                        }
                        break;
                    
                    case "?": // See other functions
                    case "/":
                    case " ":
                    case "":
                        functionListPage = (functionListPage + 1) % 2;
                        break;
                    
                    case "X": // Exit
                        menuShow = false;
                        break;
                }
                
                if (commandLineMode)
                    menuShow = false;
            }
        }
        
        // Parses command line arguments (if there are any) and prepares to run desired function.
        static void parseCommandLineArgs(string[] args)
        {
            if (args == null)
                return;
            if (args.Length == 0)
                return;
            
            commandLineMode = true;
            commandLineFunction = "";
            
            string code = "";
            bool readProjectFilenames = false;
            
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                
                if (arg.StartsWith("-"))
                {
                    if (projectFile1 == "")
                    {
                        Console.WriteLine("No filename found. Filename should be the first argument (or first two).");
                        Console.WriteLine("Continuing in standard menu mode.");
                        Console.ReadLine();
                        Console.Clear();
                        commandLineMode = false;
                        return;
                    }
                    
                    readProjectFilenames = true;
                    code = arg.Substring(1, arg.Length - 1).Trim().ToLower();
                    
                    switch (code)
                    {
                        case "extract": globalMode = "Extracting"; break;
                        case "rewrite": globalMode = "Rewriting"; break;
                        case "check": globalMode = "Checking"; break;
                        case "single": allFiles = false; break;
                        case "all": allFiles = true; break;
                    }
                    
                    if (commandLineFunction == "") // Only let function be set once
                    {
                        switch (code)
                        {
                            case "extract":
                            case "rewrite":
                            case "check":
                            case "single":
                            case "all": commandLineFunction = "z"; break;
                            case "q": case "consistency": commandLineFunction = "q"; break;
                            case "w": case "completeness": commandLineFunction = "w"; break;
                            case "s": case "duplicate": commandLineFunction = "s"; break;
                            case "e": case "unique": commandLineFunction = "e"; break;
                            case "d": case "tiledata": commandLineFunction = "d"; break;
                            case "c": case "tilecopy": commandLineFunction = "c"; break;
                            case "f": case "filelist": commandLineFunction = "f"; break;
                            case "v": case "filerename": commandLineFunction = "v"; break;
                            case "t": case "export": commandLineFunction = "t"; break;
                            case "g": case "import": commandLineFunction = "g"; break;
                            case "y": case "combo": commandLineFunction = "y"; break;
                            case "h": case "svcopy": commandLineFunction = "h"; break;
                            case "n": case "actcopy": commandLineFunction = "n"; break;
                        }
                    }
                }
                else
                {
                    if (!readProjectFilenames)
                    {
                        if (projectFile1 == "")
                            projectFile1 = arg;
                        else if (projectFile2 == "")
                            projectFile2 = arg;
                    }
                    else
                    {
                        arg = arg.Trim().ToLower();
                        int num;
                        bool validInt = int.TryParse(arg, out num);
                        
                        if (validTrueFalse(arg)) // Code followed by boolean argument
                        {
                            bool value = boolString(arg);
                            switch (code)
                            {
                                case "messages": includeMessages = value; break;
                                case "actions": includeActions = value; break;
                                case "datanames": includeDataNames = value; break;
                                case "stringreplace": useRewrittenStrings = value; break;
                                case "messageblank": blankOutMessages = value; break;
                                case "checkmessage": includeMessages = value; break;
                                case "checkfilename": includeActions = value; break;
                                case "checkfileuse": checkUnusedFiles = value; break;
                                case "checkdatause": checkUnusedData = value; break;
                                case "checklength": checkLineLength = value; break;
                                case "isolateunused": cmdIsolateUnusedFiles = value; break;
                                case "verbose": superVerboseStrings = value; break;
                                case "w": case "completeness": cmdCompletenessReplacement = value; break;
                                case "f": case "filelist": cmdFileListNonASCII = value; break;
                            }
                        }
                        
                        if (validInt) // Code followed by integer argument
                        {
                            switch (code)
                            {
                                case "e": case "unique": cmdUniqueSources = num; break;
                                case "r": case "gamemode": cmdGameMode = num; break;
                                case "nolimit": ignoreLengthLimits = num; break;
                                case "forceversion": forceEngineVersion = num; break;
                                case "readcode": tryToSetReadEncoding(num, S_TOTRANSLATE); break;
                                case "writecode": tryToSetWriteEncoding(num, S_TOTRANSLATE); break;
                                case "filereadcode": tryToSetReadEncoding(num, S_FILENAME); break;
                                case "filewritecode": tryToSetWriteEncoding(num, S_FILENAME); break;
                                case "miscreadcode": tryToSetReadEncoding(num, S_UNTRANSLATED); break;
                                case "miscwritecode": tryToSetWriteEncoding(num, S_UNTRANSLATED); break;
                            }
                        }
                        
                        switch (code) // Code followed by string argument
                        {
                            case "log": case "output": userSettingsStr["LogFilename"] = arg; break;
                            case "a": case "input": userSettingsStr["ReplacementInputFile"] = arg; break;
                        }
                        
                        code = "";
                    }
                }
            }
            
            if (commandLineFunction == "") // No function was explicitly set
                commandLineFunction = "z";
        }
        
        // Extracts all data to text files in a Scripts folder.
        static void extractAll(string filepath)
        {
            gamePath = Path.GetDirectoryName(filepath);
            
            string scriptDir = gamePath + "\\Scripts";
            if (includeMessages && !includeActions)
                scriptDir = gamePath + "\\MessageScripts";
            else if (!includeMessages && includeActions)
                scriptDir = gamePath + "\\ActionScripts";
            Directory.CreateDirectory(scriptDir);
            
            string dataScriptDir = scriptDir + "\\Database";
            Directory.CreateDirectory(dataScriptDir);
            
            readInDataNames();
            
            if (includeActions)
            {
                string mapTreeText = processMapTree(gamePath + "\\RPG_RT.lmt", false);
                writeToNewFile(dataScriptDir + "\\MapTree.txt", mapTreeText);
            }
            
            processDatabase(gamePath + "\\RPG_RT.ldb", false, true, dataScriptDir);
            
            string mapScriptDir = scriptDir + "\\Maps";
            Directory.CreateDirectory(mapScriptDir);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            
            using (StreamWriter allFile = new StreamWriter(mapScriptDir + "\\AllMaps.txt", false, UNICODE))
            {
                foreach (string file in mapList)
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    string mapText = processMap(file, false);
                    if (mapText != "")
                    {
                        writeToNewFile(mapScriptDir + "\\" + filename + ".txt", mapText);
                        allFile.Write(mapText);
                    }
                }
                allFile.Close();
            }
        }
        
        // Rewrites all files according to the loaded input file.
        static bool rewriteAll(string filepath)
        {
            gamePath = Path.GetDirectoryName(filepath);
            
            if (!translateFilenames(gamePath + "\\RPG_RT.lmt"))
                Console.WriteLine("No files needed renaming.");
            
            processMapTree(gamePath + "\\RPG_RT.lmt", false);
            processDatabase(gamePath + "\\RPG_RT.ldb", false);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            foreach(string file in mapList)
                processMap(file, false);
            
            logSave();
            
            return true;
        }
        
        // Checks all maps, map tree, and database for non-existing file references.
        static void checkAll(string filepath)
        {
            if (includeActions)
                missingFiles = new Dictionary<string,List<string>>();
            
            gamePath = Path.GetDirectoryName(filepath);
            
            if (checkUnusedData) // Compile list of all non-blank data entries to start with.
            {
                unusedDataEntries = new Dictionary<string, List<int>>();
                variableReferencedDatabases = new List<string>();
                
                makingDataEntryLists = true;
                processDatabase(gamePath + "\\RPG_RT.ldb", false);
                makingDataEntryLists = false;
            }
            
            processMapTree(gamePath + "\\RPG_RT.lmt", false);
            processDatabase(gamePath + "\\RPG_RT.ldb", false);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            foreach (string file in mapList)
                processMap(file, false);
            
            writeFileIssues();
            
            logSave();
        }
        
        // Compares original and translated versions of a game for either consistency or completeness.
        static void compareAll(string filepath1, string filepath2)
        {
            comparisonMode = true;
            
            origStringList = new Dictionary<string, Dictionary<int, Dictionary<int, List<string>>>>();
            transStringList = new Dictionary<string, Dictionary<int, Dictionary<int, List<string>>>>();
            sourceLocationString = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();
            translationBook = new Dictionary<string, string>();
            translationAlt = new Dictionary<string, List<string>>();
            translationSource = new Dictionary<string, string>();
            translationAltSource = new Dictionary<string, List<string>>();
            uniqueList = new List<string>();
            
            Console.WriteLine("Processing original project...");
            readingOriginal = true;
            loadForComparison(filepath1);
            
            Console.WriteLine("Processing translated project...");
            readingOriginal = false;
            loadForComparison(filepath2);
            
            Console.WriteLine("Comparing projects...");
            
            int changedCount = 0, totalCount = 0;
            int changedUniqueCount = 0, totalUniqueCount = 0;
            Dictionary<string, int> mapChangedUniqueCount = new Dictionary<string, int>();
            Dictionary<string, int> mapTotalUniqueCount = new Dictionary<string, int>();
            
            string sect = "";
            
            foreach (string map in origStringList.Keys)
            {
                sect = map;
                mapChangedUniqueCount[sect] = 0;
                mapTotalUniqueCount[sect] = 0;
                
                foreach (int ev in origStringList[map].Keys)
                {
                    foreach (int page in origStringList[map][ev].Keys)
                    {
                        if (map.Equals("RPG_RT.ldb"))
                        {
                            sect = "Common " + ev;
                            mapChangedUniqueCount[sect] = 0;
                            mapTotalUniqueCount[sect] = 0;
                        }
                        
                        if (transStringList.ContainsKey(map)
                         && transStringList[map].ContainsKey(ev)
                         && transStringList[map][ev].ContainsKey(page)
                         && transStringList[map][ev][page] != null)
                        {
                            string comparePos = sourceLocationString[map][ev][page];
                            
                            if (origStringList[map][ev][page].Count != transStringList[map][ev][page].Count)
                                Console.WriteLine(comparePos + ": Size discrepancy, page skipped.");
                            else
                            {
                                for (int i = 0; i < origStringList[map][ev][page].Count; i++)
                                {
                                    string origStr = origStringList[map][ev][page][i];
                                    string transStr = transStringList[map][ev][page][i];
                                    
                                    if (!completenessMode) // Consistency
                                    {
                                        if (!translationBook.ContainsKey(origStr))
                                        {
                                            translationBook[origStr] = transStr;
                                            translationSource[origStr] = comparePos;
                                        }
                                        else
                                        {
                                            bool valid = false;
                                            if (translationBook[origStr].Equals(transStr))
                                                valid = true;
                                            if (allowAlternateTrans && translationAlt.ContainsKey(origStr))
                                            {
                                                foreach (string str in translationAlt[origStr])
                                                {
                                                    if (str.Equals(transStr))
                                                    {
                                                        valid = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            
                                            if (!valid)
                                            {
                                                logMessage("[EXPECTED FROM " + translationSource[origStr] + "]");
                                                logMessage(translationBook[origStr]);
                                                if (allowAlternateTrans && translationAlt.ContainsKey(origStr))
                                                {
                                                    for (int j = 0; j < translationAlt[origStr].Count; j++)
                                                    {
                                                        logMessage("[OR FROM " + translationAltSource[origStr][j] + "]");
                                                        logMessage(translationAlt[origStr][j]);
                                                    }
                                                }
                                                logMessage("[FOUND AT " + comparePos + "]");
                                                logMessage(transStr);
                                                logMessage("------------------------------");
                                                logMessage("");
                                                
                                                if (allowAlternateTrans)
                                                {
                                                    if (!translationAlt.ContainsKey(origStr))
                                                    {
                                                        translationAlt[origStr] = new List<string>();
                                                        translationAltSource[origStr] = new List<string>();
                                                    }
                                                    translationAlt[origStr].Add(transStr);
                                                    translationAltSource[origStr].Add(comparePos);
                                                }
                                            }
                                        }
                                    }
                                    else // Completeness
                                    {
                                        if (origStr.Equals(transStr))
                                        {
                                            if (transStr != "" && transStr != "\r\n" && transStr != "\n")
                                            {
                                                logMessage("[FOUND AT " + comparePos + "]");
                                                logMessage(transStr);
                                                logMessage("------------------------------");
                                                logMessage("");
                                            }
                                            else
                                            {
                                                changedCount++;
                                                if (!uniqueList.Contains(origStr))
                                                {
                                                    changedUniqueCount++;
                                                    mapChangedUniqueCount[sect]++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            changedCount++;
                                            if (!uniqueList.Contains(origStr))
                                            {
                                                changedUniqueCount++;
                                                mapChangedUniqueCount[sect]++;
                                            }
                                        }
                                        
                                        totalCount++;
                                        if (!uniqueList.Contains(origStr))
                                        {
                                            uniqueList.Add(origStr);
                                            totalUniqueCount++;
                                            mapTotalUniqueCount[sect]++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            if (completenessMode)
            {
                float percentage = (changedCount / (float)totalCount) * 100;
                float uniquePercent = (changedUniqueCount / (float)totalUniqueCount) * 100;
                Console.WriteLine("Translation Completeness: " + percentage + "% (" + changedCount + "/" + totalCount + " messages)");
                Console.WriteLine("Unique Messages Translated: " + uniquePercent + "% (" + changedUniqueCount + "/" + totalUniqueCount + " messages)");
                
                foreach (string file in mapTotalUniqueCount.Keys)
                {
                    if (mapChangedUniqueCount[file] < mapTotalUniqueCount[file])
                        Console.WriteLine(file + " Messages Left: " + (mapTotalUniqueCount[file] - mapChangedUniqueCount[file]));
                }
            }
            
            logSave();
            comparisonMode = false;
            
            if (!completenessMode && !logExists)
                Console.WriteLine("Projects are perfectly consistent.");
        }
        
        // Reads data from project to create lists for comparison. readingOriginal determines which lists to use.
        static void loadForComparison(string filepath)
        {
            gamePath = Path.GetDirectoryName(filepath);
            
            processDatabase(gamePath + "\\RPG_RT.ldb", false);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            foreach (string file in mapList)
                processMap(file, false);
        }
        
        // Extracts two versions of a game, combining their map scripts into one.
        static string extractDouble(string filepath1, string filepath2)
        {
            extractDoubleMode = true;
            
            bool formerActions = includeActions;
            bool formerMessages = includeMessages;
            includeActions = false;
            includeMessages = true;
            
            origStringList = new Dictionary<string, Dictionary<int, Dictionary<int, List<string>>>>();
            transStringList = new Dictionary<string, Dictionary<int, Dictionary<int, List<string>>>>();
            
            Console.WriteLine("Processing original project...");
            readingOriginal = true;
            loadForDoubleExtract(filepath1);
            
            Console.WriteLine("Processing translated project...");
            readingOriginal = false;
            loadForDoubleExtract(filepath2);
            
            Console.WriteLine("Combining project scripts...");
            
            StringWriter str = new StringWriter(new StringBuilder());
            bool wroteMapHeader = false;
            bool wroteEventHeader = false;
            bool wrotePageHeader = false;
            
            foreach (string map in origStringList.Keys)
            {
                wroteMapHeader = false;
                foreach (int ev in origStringList[map].Keys)
                {
                    wroteEventHeader = false;
                    foreach (int page in origStringList[map][ev].Keys)
                    {
                        wrotePageHeader = false;
                        
                        if (transStringList.ContainsKey(map)
                         && transStringList[map].ContainsKey(ev)
                         && transStringList[map][ev].ContainsKey(page)
                         && transStringList[map][ev][page] != null)
                        {
                            int maxCount = Math.Max(origStringList[map][ev][page].Count, transStringList[map][ev][page].Count);
                            for (int i = 0; i < maxCount; i++)
                            {
                                string origStr = i < origStringList[map][ev][page].Count? origStringList[map][ev][page][i] : "";
                                string transStr = i < transStringList[map][ev][page].Count? transStringList[map][ev][page][i] : "";
                                
                                if (origStr != "" || transStr != "")
                                {
                                    if (!wroteMapHeader)
                                    {
                                        str.WriteLine("<<<<<<<<<< " + map + " >>>>>>>>>>");
                                        wroteMapHeader = true;
                                    }
                                    if (!wroteEventHeader)
                                    {
                                        str.WriteLine("***** Event #" + ev + " *****");
                                        wroteEventHeader = true;
                                    }
                                    if (!wrotePageHeader)
                                    {
                                        str.WriteLine("--- Page #" + page + " ---");
                                        wrotePageHeader = true;
                                    }
                                    
                                    string[] origSplit = origStr.Replace("\r\n", "\n").Split('\n');
                                    string[] transSplit = transStr.Replace("\r\n", "\n").Split('\n');
                                    for (int k = 0; k < Math.Max(origSplit.Length, transSplit.Length); k++)
                                    {
                                        if (k < origSplit.Length && k < transSplit.Length)
                                            str.WriteLine(origSplit[k] + "\t" + transSplit[k]);
                                        else if (k < transSplit.Length)
                                            str.WriteLine("\t" + transSplit[k]);
                                        else
                                            str.WriteLine(origSplit[k]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            includeActions = formerActions;
            includeMessages = formerMessages;
            
            return str.ToString();
        }
        
        // Reads data from project to create lists for double extraction. readingOriginal determines which lists to use.
        static void loadForDoubleExtract(string filepath)
        {
            gamePath = Path.GetDirectoryName(filepath);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            foreach (string file in mapList)
                processMap(file, false);
        }
        
        // Extracts all map tiles to text files in a TileData folder.
        static void extractAllTiles(string filepath)
        {
            gamePath = Path.GetDirectoryName(filepath);
            
            string tileDir = gamePath + "\\TileData";
            Directory.CreateDirectory(tileDir);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            
            foreach (string file in mapList)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                string tileText = extractMapTiles(file);
                if (tileText != "")
                    writeToNewFile(tileDir + "\\" + filename + ".txt", tileText);
            }
        }
        
        // Loads a single .lmu map and returns tile layout as text.
        static string extractMapTiles(string filepath)
        {
            Map map = new Map(filepath, false);
            return map.getTilesString();
        }
        
        // Reads tile data from source maps and pastes it into destination maps. Returns whether any maps were actually modified.
        static bool copyAllTileData(string filepath1, string filepath2)
        {
            copyingTileData = true;
            
            mapTileBytesLayer1 = new Dictionary<string, int[][]>();
            mapTileBytesLayer2 = new Dictionary<string, int[][]>();
            
            loadAllTileData(filepath1);
            bool anyChanges = pasteAllTileData(filepath2);
            
            copyingTileData = false;
            
            return anyChanges;
        }
        
        // Reads maps from source project and stores the tile portion of every map.
        static void loadAllTileData(string filepath)
        {
            globalMode = "Extracting";
            
            string dir = Path.GetDirectoryName(filepath);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(dir, "*.lmu");
            foreach (string file in mapList)
                loadMapTileData(file);
        }
        
        // Gets tile data for a single map into mapTileBytes.
        static void loadMapTileData(string filepath)
        {
            Map map = new Map(filepath, false);
            
            string filename = Path.GetFileName(filepath);
            mapTileBytesLayer1[filename] = map.getLayer1Tiles();
            mapTileBytesLayer2[filename] = map.getLayer2Tiles();
        }
        
        // Pastes tile data into every map in the destination project. Returns whether any maps were actually modified.
        static bool pasteAllTileData(string filepath)
        {
            globalMode = "Rewriting";
            
            string dir = Path.GetDirectoryName(filepath);
            
            bool anyChanges = false;
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(dir, "*.lmu");
            foreach (string file in mapList)
            {
                if (pasteMapTileData(file))
                    anyChanges = true;
            }
            
            return anyChanges;
        }
        
        // Pastes the source map tile data from mapTileBytes into the destination map. Returns whether map was actually modified.
        static bool pasteMapTileData(string filepath)
        {
            Map map = new Map(filepath, false);
            
            string filename = Path.GetFileName(filepath);
            
            if (!mapTileBytesLayer1.ContainsKey(filename) && !mapTileBytesLayer2.ContainsKey(filename))
            {
                Console.WriteLine("Source map " + filename + " not found in destination project.");
                return false;
            }
            
            if (map.pasteTiles(mapTileBytesLayer1[filename], mapTileBytesLayer2[filename], filename))
            {
                Console.WriteLine("Pasting into destination " + filename + "...");
                map.writeFile();
                return true;
            }
            else
                Console.WriteLine("Tiles are up to date in destination " + filename + ".");
            
            return false;
        }
        
        // Reads switch/variable names from source LDB and pastes it into destination LDB. Returns whether there was actually a difference.
        static bool copySwitchVariableNames(string filepath1, string filepath2)
        {
            string dir1 = Path.GetDirectoryName(filepath1);
            string dir2 = Path.GetDirectoryName(filepath2);
            
            copyingSwitchVariable = true;
            
            readingOriginal = true;
            Database db1 = new Database(dir1 + "\\RPG_RT.ldb", false);
            
            readingOriginal = false;
            Database db2 = new Database(dir2 + "\\RPG_RT.ldb", false);
            
            copyingSwitchVariable = false;
            
            string[] sourceSwitchNames = db1.getSwitchNames();
            string[] sourceVariableNames = db1.getVariableNames();
            
            bool anyChanges = false;
            if (db2.setSwitchNames(sourceSwitchNames))
                anyChanges = true;
            if (db2.setVariableNames(sourceVariableNames))
                anyChanges = true;
            
            if (anyChanges)
                db2.writeFile();
            
            return anyChanges;
        }
        
        // Reads action command values from source project and pastes them into destination project. Returns whether there was actually a difference.
        static bool copyActionCommandValues(string filepath1, string filepath2)
        {
            string dir1 = Path.GetDirectoryName(filepath1);
            string dir2 = Path.GetDirectoryName(filepath2);
            
            actionCommandValues = new Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<int, List<int[]>>>>>();
            
            copyingCommandValues = true;
            commandValueChangesMade = false;
            
            readingOriginal = true;
            processDatabase(dir1 + "\\RPG_RT.ldb", false);
            IEnumerable<string> mapList = Directory.EnumerateFiles(dir1, "*.lmu");
            foreach (string file in mapList)
                processMap(file, false);
            
            globalMode = "Rewriting";
            
            readingOriginal = false;
            processDatabase(dir2 + "\\RPG_RT.ldb", false);
            mapList = Directory.EnumerateFiles(dir2, "*.lmu");
            foreach (string file in mapList)
                processMap(file, false);
            
            copyingCommandValues = false;
            
            return commandValueChangesMade;
        }
        
        // Finds all duplicate messages and ranks them by occurrences, returning this in a string.
        static string compileMessageDuplicates(string filepath)
        {
            compilingDuplicates = true;
            
            duplicateMessageList = new Dictionary<string, int>();
            duplicateMessageHighest = 0;
            
            processDatabase(gamePath + "\\RPG_RT.ldb", false);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            foreach (string file in mapList)
                processMap(file, false);
            
            StringWriter str = new StringWriter(new StringBuilder());
            for (int i = duplicateMessageHighest; i >= 2; i--)
            {
                bool rankHasContent = false;
                foreach (string key in duplicateMessageList.Keys)
                {
                    if (duplicateMessageList[key] == i)
                    {
                        str.WriteLine("[Occurrences: " + i + "]");
                        str.WriteLine(key);
                        str.WriteLine();
                        rankHasContent = true;
                    }
                }
                if (rankHasContent) // Put divider between each occurrence rank, but only necessary if there was actual content.
                {
                    str.WriteLine("------------------------------------------------------------");
                    str.WriteLine();
                }
            }
            
            compilingDuplicates = false;
            
            return str.ToString();
        }
        
        // Extracts all unique messages in commons and maps and returns them in a string.
        static string compileUniqueMessages(string filepath)
        {
            compilingUnique = true;
            
            uniqueList = new List<string>();
            uniqueListSources = new List<List<string>>();
            
            processDatabase(gamePath + "\\RPG_RT.ldb", false);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            foreach (string file in mapList)
                processMap(file, false);
            
            StringWriter str = new StringWriter(new StringBuilder());
            for (int i = 0; i < uniqueList.Count; i++)
            {
                if (messageUniqueSource)
                {
                    foreach (string source in uniqueListSources[i])
                    {
                        str.WriteLine("[" + source + "]");
                        if (!messageUniqueSourceMulti)
                            break;
                    }
                }
                str.WriteLine(uniqueList[i]);
            }
            
            compilingUnique = false;
            
            return str.ToString();
        }
        
        // Exports editable scripts into the StringScripts folder.
        static bool stringScriptExport(string filepath)
        {
            gamePath = Path.GetDirectoryName(filepath);
            
            string scriptDir = gamePath + "\\StringScripts";
            
            if (Directory.Exists(scriptDir) && !commandLineMode)
            {
                Console.WriteLine("StringScripts folder already exists. Export and overwrite? (Y/N)");
                if (!yesNoPrompt())
                    return false;
            }
            
            Directory.CreateDirectory(scriptDir);
            string dataScriptDir = scriptDir + "\\Database";
            Directory.CreateDirectory(dataScriptDir);
            
            bool includeMessagesOld = includeMessages, includeActionsOld = includeActions, dataNamesOld = includeDataNames;
            stringScriptExportMode = true;
            includeMessages = true;
            includeActions = false;
            
            if (getDetailSetting("OriginalCommandStrings") || getDetailSetting("OriginalDatabaseStrings")) // User wants original strings, so try to load DB
                loadOriginalStringDatabase();
            
            if (getDetailSetting("DataNames")) // User wants data names for StringScripts, so force load them
            {
                includeDataNames = true;
                readInDataNames(true);
            }
            
            if (getExtraneousSetting("MapNames")) // User wants map names for StringScripts
            {
                string mapTreeText = processMapTree(gamePath + "\\RPG_RT.lmt", false);
                writeToNewFile(dataScriptDir + "\\MapTree.txt", mapTreeText);
            }
            
            processDatabase(gamePath + "\\RPG_RT.ldb", false, true, dataScriptDir);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            
            foreach (string file in mapList)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                string mapText = processMap(file, false);
                writeToNewFile(scriptDir + "\\" + filename + ".txt", mapText);
            }
            
            // After populating original string database, write it to file.
            if (generatingOriginalStringDB)
            {
                string json = JsonConvert.SerializeObject(originalStringDB);
                File.WriteAllText(scriptDir + "\\OriginalStringDB.json", json);
                generatingOriginalStringDB = false;
            }
            
            stringScriptExportMode = false;
            includeMessages = includeMessagesOld;
            includeActions = includeActionsOld;
            includeDataNames = dataNamesOld;
            
            return true;
        }
        
        // For all maps and database sections, loads corresponding StringScript and inserts the strings.
        static bool stringScriptImport(string filepath)
        {
            gamePath = Path.GetDirectoryName(filepath);
            
            string scriptDir = gamePath + "\\StringScripts";
            string dataScriptDir = scriptDir + "\\Database";
            
            stringScriptImportMode = true;
            
            if (File.Exists(dataScriptDir + "\\MapTree.txt"))
                processMapTree(gamePath + "\\RPG_RT.lmt", false, dataScriptDir);
            
            processDatabase(gamePath + "\\RPG_RT.ldb", false, false, dataScriptDir);
            
            IEnumerable<string> mapList = Directory.EnumerateFiles(gamePath, "*.lmu");
            foreach (string file in mapList)
            {
                loadStringScript(scriptDir + "\\" + Path.GetFileNameWithoutExtension(file) + ".txt");
                if (importingStringArgs != null && !stringScriptImportCheck)
                    processMap(file, false);
            }
            
            stringScriptImportMode = false;
            
            if (stringScriptImportCheck)
                logSave();
            
            return true;
        }
        
        // Loads the StringScript for a map, common event, or troop page into a dictionary for insertion.
        public static void loadStringScript(string file, int commonEventNum = -1, bool troop = false, bool optional = false)
        {
            if (!File.Exists(file))
            {
                if (!optional)
                    Console.WriteLine("Script not found for " + Path.GetFileNameWithoutExtension(file) + ".");
                importingStringArgs = null;
                return;
            }
            
            importingStringArgs = new Dictionary<int, Dictionary<int, Dictionary<string, List<string>>>>();
            
            using (StreamReader input = new StreamReader(file))
            {
                string line;
                bool readingArg = false;
                bool firstLineRead = false;
                int ev = commonEventNum;
                int page = 0;
                string commandName = "";
                string multiLineStr = "";
                
                while ((line = input.ReadLine()) != null)
                {
                    if (!readingArg)
                    {
                        if (commonEventNum == -1 && line.StartsWith("*")
                         && (!troop && line.ToLower().Contains("*event")) || (troop && line.ToLower().Contains("*troop")))
                        {
                            int start = line.ToLower().IndexOf(!troop? "*event" : "*troop") + 6;
                            int end = line.IndexOf("*", start);
                            string num = line.Substring(start, end - start);
                            
                            int oldEv = ev;
                            if (!int.TryParse(num, out ev))
                                ev = oldEv;
                            if (ev < 0)
                                ev = oldEv;
                        }
                        else if (commonEventNum == -1 && line.StartsWith("-") && line.ToLower().Contains("-page"))
                        {
                            int start = line.ToLower().IndexOf("-page") + 5;
                            int end = line.IndexOf("-", start);
                            string num = line.Substring(start, end - start);
                            
                            int oldPage = page;
                            if (!int.TryParse(num, out page))
                                page = oldPage;
                            if (page < 1 || page > 100)
                                page = oldPage;
                        }
                        else if (line.StartsWith("#") &&
                                 (line.ToLower().Contains("#message#")
                               || line.ToLower().Contains("#choice#")
                               || line.ToLower().Contains("#namechange#")
                               || line.ToLower().Contains("#titlechange#")
                               || line.ToLower().Contains("#namefork#")
                               || line.ToLower().Contains("#stringpicture#")
                               || line.ToLower().Contains("#eventname#")
                               || line.ToLower().Contains("#commoneventname#")
                               || line.ToLower().Contains("#troopname#")))
                        {
                            readingArg = true;
                            firstLineRead = false;
                            multiLineStr = "";
                            
                            if (line.ToLower().Contains("#message#"))
                                commandName = "Message";
                            else if (line.ToLower().Contains("#choice#"))
                                commandName = "Choice";
                            else if (line.ToLower().Contains("#namechange#"))
                                commandName = "NameChange";
                            else if (line.ToLower().Contains("#titlechange#"))
                                commandName = "TitleChange";
                            else if (line.ToLower().Contains("#namefork#"))
                                commandName = "NameFork";
                            else if (line.ToLower().Contains("#stringpicture#"))
                                commandName = "StringPicture";
                            else if (line.ToLower().Contains("#eventname#"))
                                commandName = "eventname"; // Lowercase since it uses database-style approach
                            else if (line.ToLower().Contains("#commoneventname#"))
                                commandName = "commoneventname"; // Lowercase since it uses database-style approach
                            else if (line.ToLower().Contains("#troopname#"))
                                commandName = "troopname"; // Lowercase since it uses database-style approach
                        }
                    }
                    else
                    {
                        if (commandName.Equals("Message") || commandName.Equals("Choice") || commandName.Equals("StringPicture")) // Multi-line types
                        {
                            if (line.StartsWith("##"))
                            {
                                readingArg = false;
                                
                                if (commandName.Equals("Choice"))
                                {
                                    string[] choices = multiLineStr.Replace("\r\n", "\n").Split('\n');
                                    multiLineStr = "";
                                    
                                    for (int i = 0; i < choices.Length; i++)
                                    {
                                        choices[i] = limitLength(choices[i], 32);
                                        multiLineStr += choices[i] + (i < choices.Length - 1? "\n" : "");
                                    }
                                }
                                
                                if (!importingStringArgs.ContainsKey(ev))
                                    importingStringArgs[ev] = new Dictionary<int, Dictionary<string, List<string>>>();
                                if (!importingStringArgs[ev].ContainsKey(page))
                                    importingStringArgs[ev][page] = new Dictionary<string, List<string>>();
                                if (!importingStringArgs[ev][page].ContainsKey(commandName))
                                    importingStringArgs[ev][page][commandName] = new List<string>();
                                importingStringArgs[ev][page][commandName].Add(multiLineStr);
                                
                                if (stringScriptImportCheck && !isValid(multiLineStr))
                                    logMessage(Path.GetFileNameWithoutExtension(file) + " " + ev + " " + page + " " + commandName + " " + multiLineStr);
                            }
                            else
                            {
                                multiLineStr += (firstLineRead? "\n" : "") + line;
                                firstLineRead = true;
                            }
                        }
                        else // For other types, line right after header is read, ## not actually necessary
                        {
                            readingArg = false;
                            
                            if (commandName.Equals("NameChange")
                             || commandName.Equals("TitleChange")
                             || commandName.Equals("NameFork"))
                                line = limitLength(line, 12);
                            
                            int oldPage = page;
                            if (commandName.Equals("eventname")
                             || commandName.Equals("commoneventname")
                             || commandName.Equals("troopname"))
                                page = 0; // Page number is irrelevant for these, so make it consistent
                            
                            if (!importingStringArgs.ContainsKey(ev))
                                importingStringArgs[ev] = new Dictionary<int, Dictionary<string, List<string>>>();
                            if (!importingStringArgs[ev].ContainsKey(page))
                                importingStringArgs[ev][page] = new Dictionary<string, List<string>>();
                            if (!importingStringArgs[ev][page].ContainsKey(commandName))
                                importingStringArgs[ev][page][commandName] = new List<string>();
                            importingStringArgs[ev][page][commandName].Add(line);
                            
                            if (stringScriptImportCheck && !isValid(line))
                                logMessage(Path.GetFileNameWithoutExtension(file) + " " + ev + " " + page + " " + commandName + " " + line);
                            
                            page = oldPage;
                        }
                    }
                }
            }
        }
        
        // Loads the StringScript for a database section into a dictionary for insertion.
        public static void loadStringScriptDatabase(string file, int tabNum, bool optional = false)
        {
            if (!File.Exists(file))
            {
                if (!optional)
                    Console.WriteLine("Script not found for " + Path.GetFileNameWithoutExtension(file) + ".");
                importingStringArgs = null;
                return;
            }
            
            importingStringArgs = new Dictionary<int, Dictionary<int, Dictionary<string, List<string>>>>();
            
            using (StreamReader input = new StreamReader(file))
            {
                string line;
                bool readingArg = false;
                string fieldName = "";
                int entry = 0;
                
                bool vocab = tabNum == 0x15;
                
                while ((line = input.ReadLine()) != null)
                {
                    if (!readingArg)
                    {
                        if (!vocab && line.StartsWith("*") && line.ToLower().Contains("*entry"))
                        {
                            int start = line.ToLower().IndexOf("*entry") + 6;
                            int end = line.IndexOf("*", start);
                            string num = line.Substring(start, end - start);
                            
                            int oldEntry = entry;
                            if (!int.TryParse(num, out entry))
                                entry = oldEntry;
                            if (entry < 1)
                                entry = oldEntry;
                        }
                        else if (line.StartsWith("#"))
                        {
                            readingArg = true;
                            
                            int firstPound = line.IndexOf("#");
                            int secondPound = line.IndexOf("#", firstPound + 1);
                            fieldName = line.Substring(firstPound + 1, secondPound - firstPound - 1).ToLower();
                            
                            if (line.Contains(":"))
                            {
                                int poundIndex = line.IndexOf("#");
                                int colonIndex = line.IndexOf(":");
                                string preColon = line.Substring(poundIndex, colonIndex - poundIndex - 1);
                                
                                while (!int.TryParse(preColon, out entry) && preColon.Length > 0) // Remove characters from left until only number is left
                                    preColon = preColon.Substring(1, preColon.Length - 1);
                                if (!int.TryParse(preColon, out entry))
                                    entry = 0;
                            }
                        }
                    }
                    else
                    {
                        readingArg = false;
                        
                        if (!importingStringArgs.ContainsKey(tabNum))
                            importingStringArgs[tabNum] = new Dictionary<int, Dictionary<string, List<string>>>();
                        if (!importingStringArgs[tabNum].ContainsKey(entry))
                            importingStringArgs[tabNum][entry] = new Dictionary<string, List<string>>();
                        if (!importingStringArgs[tabNum][entry].ContainsKey(fieldName))
                            importingStringArgs[tabNum][entry][fieldName] = new List<string>();
                        importingStringArgs[tabNum][entry][fieldName].Add(line);
                        
                        if (stringScriptImportCheck && !isValid(line))
                            logMessage(Path.GetFileNameWithoutExtension(file) + " " + tabNum + " " + entry + " " + fieldName + " " + line);
                    }
                }
            }
        }
        
        // Attempts to load the original string database file. If not found, database will be populated with strings during export and saved to file at end.
        static void loadOriginalStringDatabase()
        {
            string filename = gamePath + "\\StringScripts\\OriginalStringDB.json";
            if (!File.Exists(filename))
            {
                generatingOriginalStringDB = true;
                originalStringDB = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>();
            }
            else
            {
                string json = File.ReadAllText(filename);
                originalStringDB = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>>(json);
                generatingOriginalStringDB = false;
            }
        }
        
        // Loads an .lmu map file and either returns its script string, or re-saves it with altered contents.
        static string processMap(string filepath, bool writeLog = true)
        {
            changesMade = alwaysResaveFiles;
            
            Map map = new Map(filepath, writeLog);
            
            if (globalMode == "Extracting")
                return map.getString();
            else if (globalMode == "Rewriting")
            {
                string filename = Path.GetFileName(filepath);
                if (stringScriptImportMode)
                {
                    map.importStrings();
                    if (changesMade)
                        Console.WriteLine("Updating " + filename + "...");
                    else if (userSettings["NoteUnchanged"] == 1)
                        Console.WriteLine("No changes necessary for " + filename + ".");
                }
                else
                {
                    if (changesMade)
                        Console.WriteLine("Rewriting " + filename + "...");
                    else if (userSettings["NoteUnchanged"] == 1)
                        Console.WriteLine("No changes necessary for " + filename + ".");
                }
                if (changesMade)
                    map.writeFile();
            }
            else if (checkUnusedData)
                map.getString(); // Not to actually get string, but to let it call getDataX functions, which remove entries as they're referenced
            
            if (globalMode == "Rewriting" && changesMade)
                return "Rewritten";
            else
                return "OK";
        }
        
        // Processes the .lmt file (contains tree structure and map info) and either returns its script string, or re-saves it with altered contents.
        static string processMapTree(string filepath, bool writeLog = true, string scriptDir = "")
        {
            changesMade = alwaysResaveFiles;
            
            MapTree mapTree = new MapTree(filepath, false);
            
            if (!readingDataNames)
            {
                if (globalMode == "Extracting")
                    return mapTree.getString();
                else if (globalMode == "Rewriting")
                {
                    string filename = Path.GetFileName(filepath);
                    if (stringScriptImportMode)
                    {
                        mapTree.importStrings(scriptDir);
                        if (changesMade)
                            Console.WriteLine("Updating " + filename + "...");
                        else if (userSettings["NoteUnchanged"] == 1)
                            Console.WriteLine("No changes necessary for " + filename + ".");
                    }
                    else
                    {
                        if (changesMade)
                            Console.WriteLine("Rewriting " + filename + "...");
                        else if (userSettings["NoteUnchanged"] == 1)
                            Console.WriteLine("No changes necessary for " + filename + ".");
                    }
                    if (changesMade)
                        mapTree.writeFile();
                }
                else if (checkUnusedData)
                    mapTree.getString(); // Not to actually get string, but to let it call getDataX functions, which remove entries as they're referenced
            }
            
            if (writeLog)
                logSave();
            
            if (globalMode == "Rewriting" && changesMade)
                return "Rewritten";
            else
                return "OK";
        }
        
        // Processes the .ldb file (database tabs and extras). Returns combined string, or writes each section to a file, or re-saves with altered contents.
        static string processDatabase(string filepath, bool writeLog = true, bool writeFiles = false, string scriptDir = "")
        {
            changesMade = alwaysResaveFiles;
            
            Database db = new Database(filepath, false);
            
            if (!readingDataNames)
            {
                if (globalMode == "Extracting")
                    return db.getString(writeFiles, scriptDir);
                else if (globalMode == "Rewriting")
                {
                    string filename = Path.GetFileName(filepath);
                    if (stringScriptImportMode)
                    {
                        db.importStrings(scriptDir);
                        if (changesMade)
                            Console.WriteLine("Updating " + filename + "...");
                        else if (userSettings["NoteUnchanged"] == 1)
                            Console.WriteLine("No changes necessary for " + filename + ".");
                    }
                    else
                    {
                        if (changesMade)
                            Console.WriteLine("Rewriting " + filename + "...");
                        else if (userSettings["NoteUnchanged"] == 1)
                            Console.WriteLine("No changes necessary for " + filename + ".");
                    }
                    if (changesMade)
                        db.writeFile();
                }
                else if (checkUnusedData)
                    db.getString(); // Not to actually get string, but to let it call getDataX functions, which remove entries as they're referenced
            }
            
            if (writeLog)
                logSave();
            
            if (globalMode == "Rewriting" && changesMade)
                return "Rewritten";
            else
                return "OK";
        }
        
        // Processes the .lsd save file and either returns its script string, or presents a save modification menu.
        static string processSaveFile(string filepath)
        {
            changesMade = alwaysResaveFiles;
            
            SaveFile save = new SaveFile(filepath, false);
            
            if (globalMode == "Extracting")
                return save.getString();
            else if (globalMode == "Rewriting")
            {
                if (commandLineMode)
                {
                    Console.WriteLine("Save modification from command line is not supported.");
                    return "OK";
                }
                
                readInDataNames(true);
                bool oldDataNames = includeDataNames;
                includeDataNames = true;
                
                save.modifySubmenu();
                
                string filename = Path.GetFileName(filepath);
                if (changesMade)
                {
                    Console.WriteLine("Rewriting " + filename + "...");
                    save.writeFile();
                }
                else
                    Console.WriteLine("No changes made to " + filename + ".");
                
                includeDataNames = oldDataNames;
            }
            
            if (globalMode == "Rewriting" && changesMade)
                return "Rewritten";
            else
                return "OK";
        }
        
        // Loads the list of translations in the input file into an array of lists.
        static void loadTransList(string filename = "")
        {
            if (filename == "")
                filename = userSettingsStr["ReplacementInputFile"];
            
            // Create empty original and translation lists for all types.
            transList = new List<string>[MODECOUNT, 2];
            for (int i = 0; i < MODECOUNT; i++)
            {
                transList[i, 0] = new List<string>();
                transList[i, 1] = new List<string>();
            }
            
            lastInputFile = "N/A";
            
            if (File.Exists(filename + ".txt"))
            {
                lastInputFile = filename + ".txt";
                
                bool switchMatch, transToggle = false;
                int mode = -1;
                using (StreamReader input = new StreamReader(filename + ".txt"))
                {
                    string line;
                    while ((line = input.ReadLine()) != null)
                    {
                        // If the line matches with any "***FOLDER", change mode and break loop.
                        switchMatch = false;
                        for (int i = 0; i < MODECOUNT; i++)
                        {
                            if (line == "***" + FOLDER[i].ToUpper())
                            {
                                // If translations are mismatched, alert.
                                if (transToggle)
                                    Console.WriteLine("Warning! Translations do not appear to be properly paired.");
                                
                                // Change mode.
                                Console.WriteLine("Reading in " + FOLDER[i] + " translations...");
                                mode = i;
                                switchMatch = true;
                                break;
                            }
                        }
                        
                        // If this line is not a mode change, and not blank, read it as an original or translation, according to transToggle.
                        if (!switchMatch && line != "")
                        {
                            if (mode == -1)
                            {
                                Console.WriteLine("Warning! Mode not set. Lines without a designated mode will be ignored.");
                            }
                            else
                            {
                                if (!transToggle) // Original
                                {
                                    // Warn the user if an entry appears twice, but always add it.
                                    // The translation could differ and thus get added, throwing everything off.
                                    if (transList[mode, 0].Contains(line))
                                        Console.WriteLine("Warning! " + line + " was added twice under " + FOLDER[mode] + "! This is redundant.");
                                    
                                    transList[mode, 0].Add(line);
                                }
                                else // Translation
                                {
                                    // Check that the translation contains all valid characters.
                                    if (!isValid(line))
                                        Console.WriteLine("Warning! Invalid character found in translation entry " + line + "!");
                                    if (mode < FOLDERCOUNT && !isValidFilename(line))
                                        Console.WriteLine("Warning! " + line + " is not a valid filename! It will not be properly renamed.");
                                    
                                    // Notify the user if an entry appears twice.
                                    if (transList[mode, 1].Contains(line))
                                    {
                                        if (mode < FOLDERCOUNT) // In a folder mode, meaning same-filename is possible.
                                            Console.WriteLine("Warning! " + line + " was added twice under " + FOLDER[mode] + "! This may cause filename overlap.");
                                        else // In a message mode, meaning it could be intentional.
                                            Console.WriteLine(line + " was added twice under " + FOLDER[mode] + ".");
                                    }
                                    
                                    transList[mode, 1].Add(line);
                                }
                                transToggle = !transToggle;
                            }
                        }
                    }
                }
                // Check for mismatch once more at end.
                if (transToggle)
                    Console.WriteLine("Warning! Translations do not appear to be properly paired.");
                
                Console.WriteLine(filename + ".txt loaded as replacement list.");
            }
            else
                Console.WriteLine(filename + ".txt not found. No replacement list loaded.");
            
            enterToContinue();
        }
        
        // Loads user settings file. Option settings are only loaded once on startup, while others can be updated mid-program.
        static void loadUserSettings(bool loadAllSettings = false)
        {
            if (userSettings == null)
                userSettings = new Dictionary<string, int>();
            if (userSettingsStr == null)
                userSettingsStr = new Dictionary<string, string>();
            if (detailSettings == null)
                detailSettings = new List<string>();
            if (extraneousSettings == null)
                extraneousSettings = new List<string>();
            
            // True user preferences.
            userSettings["CommandIndents"] = 0;
            userSettings["WordWrap"] = 0;
            userSettings["WrapLineLimits"] = 1;
            userSettings["WrapStyle"] = 1;
            userSettingsStr["LogFilename"] = "log";
            userSettings["SuperVerboseStrings"] = 0;
            userSettings["StringScriptDetails"] = 0;
            userSettings["StringScriptExtraneous"] = 0;
            userSettings["IgnoreLengthLimits"] = 0;
            userSettings["ForceEngineVersion"] = 0;
            userSettings["NoteUnchanged"] = 0;
            
            // True user preferences should be reloaded frequently.
            List<string> alwaysReload = new List<string>();
            alwaysReload.Add("CommandIndents");
            alwaysReload.Add("WordWrap");
            alwaysReload.Add("WrapLineLimits");
            alwaysReload.Add("WrapStyle");
            alwaysReload.Add("LogFilename");
            alwaysReload.Add("SuperVerboseStrings");
            alwaysReload.Add("StringScriptDetails");
            alwaysReload.Add("StringScriptExtraneous");
            alwaysReload.Add("IgnoreLengthLimits");
            alwaysReload.Add("ForceEngineVersion");
            alwaysReload.Add("NoteUnchanged");
            
            // Program options that can have their defaults set; only get into these on initial load.
            if (loadAllSettings)
            {
                userSettings["Mode"] = 1;
                userSettings["FileScope"] = 1;
                userSettings["ExtractIncludeMessages"] = 1;
                userSettings["ExtractIncludeActions"] = 1;
                userSettings["ExtractUseDataNames"] = 0;
                userSettings["ExtractRewriteStrings"] = 0;
                userSettings["ExtractBlankOutMessages"] = 1;
                userSettings["CheckMessageValidity"] = 1;
                userSettings["CheckFileReferences"] = 1;
                userSettings["CheckUnusedFiles"] = 0;
                userSettings["CheckUnusedDataEntries"] = 0;
                userSettings["CheckLineLengths"] = 0;
                userSettings["SpecialGameMode"] = 0;
                userSettingsStr["ReplacementInputFile"] = "input";
                userSettings["MainReadEncoding"] = 932;
                userSettings["MainWriteEncoding"] = 932;
                userSettings["MiscReadEncoding"] = 932;
                userSettings["MiscWriteEncoding"] = 932;
                userSettings["FilenameReadEncoding"] = 932;
                userSettings["FilenameWriteEncoding"] = 932;
            }
            
            // Load settings from file, but after initial load, skip over non-reloaded ones.
            if (File.Exists("UserSettings.txt"))
            {
                using (StreamReader input = new StreamReader("UserSettings.txt"))
                {
                    string sectionMode = "";
                    string line;
                    while ((line = input.ReadLine()) != null)
                    {
                        if (sectionMode != "")
                        {
                            if (line.StartsWith("##"))
                                sectionMode = "";
                            else
                            {
                                int equals = line.IndexOf("=");
                                string settingName = line.Substring(0, equals).Trim();
                                string settingStr = line.Substring(equals + 1).Trim();
                                
                                int setting;
                                if (int.TryParse(settingStr, out setting))
                                    if (setting == 1)
                                        (sectionMode == "Details"? detailSettings : extraneousSettings).Add(settingName);
                            }
                        }
                        else if (line.StartsWith("@"))
                        {
                            int settingStart = line.IndexOf("@") + 1;
                            int equals = line.IndexOf("=");
                            string settingName = line.Substring(settingStart, equals - 1).Trim();
                            string settingStr = line.Substring(equals + 1).Trim();
                            
                            if (!loadAllSettings && !alwaysReload.Contains(settingName))
                                continue;
                            
                            if (userSettingsStr.ContainsKey(settingName)) // String
                                userSettingsStr[settingName] = settingStr;
                            else // Number
                            {
                                int setting;
                                if (int.TryParse(settingStr, out setting))
                                    userSettings[settingName] = setting;
                            }
                        }
                        else if (line.Contains("##IncludeInDetailedMode##"))
                            sectionMode = "Details";
                        else if (line.Contains("##ExtraneousStrings##"))
                            sectionMode = "Extraneous";
                    }
                }
            }
            
            // True user preferences.
            validateUserSetting("CommandIndents", 0, 1, 0);
            validateUserSetting("WordWrap", 0, 1, 0);
            validateUserSetting("WrapLineLimits", 1, 3, 1);
            validateUserSetting("WrapStyle", 1, 2, 1);
            validateUserSettingFilename("LogFilename", "log");
            validateUserSetting("SuperVerboseStrings", 0, 1, 0);
            validateUserSetting("StringScriptDetails", 0, 1, 0);
            validateUserSetting("StringScriptExtraneous", 0, 1, 0);
            validateUserSetting("IgnoreLengthLimits", 0, 2, 0);
            validateUserSetting("ForceEngineVersion", 0, 3, 0);
            validateUserSetting("NoteUnchanged", 0, 1, 0);
            
            // Program options that can have their defaults set.
            validateUserSetting("Mode", 1, 3, 1);
            validateUserSetting("FileScope", 0, 1, 1);
            validateUserSetting("ExtractIncludeMessages", 0, 1, 1);
            validateUserSetting("ExtractIncludeActions", 0, 1, 1);
            validateUserSetting("ExtractUseDataNames", 0, 1, 0);
            validateUserSetting("ExtractRewriteStrings", 0, 1, 0);
            validateUserSetting("ExtractBlankOutMessages", 0, 1, 1);
            validateUserSetting("CheckMessageValidity", 0, 1, 1);
            validateUserSetting("CheckFileReferences", 0, 1, 1);
            validateUserSetting("CheckUnusedFiles", 0, 1, 0);
            validateUserSetting("CheckUnusedDataEntries", 0, 1, 0);
            validateUserSetting("CheckLineLengths", 0, 1, 0);
            validateUserSetting("SpecialGameMode", 0, 3, 0);
            validateUserSettingFilename("ReplacementInputFile", "input");
            
            // Update dedicated variables based on userSettings.
            superVerboseStrings = userSettings["SuperVerboseStrings"] == 1;
            ignoreLengthLimits = userSettings["IgnoreLengthLimits"];
            forceEngineVersion = userSettings["ForceEngineVersion"];
            
            // On first load, set program options to the specified defaults.
            if (loadAllSettings)
            {
                switch (userSettings["Mode"])
                {
                    case 1: globalMode = "Extracting"; break;
                    case 2: globalMode = "Rewriting"; break;
                    case 3: globalMode = "Checking"; break;
                }
                allFiles = userSettings["FileScope"] == 1;
                
                if (globalMode == "Checking")
                {
                    includeMessages = userSettings["CheckMessageValidity"] == 1;
                    includeActions = userSettings["CheckFileReferences"] == 1;
                }
                else
                {
                    includeMessages = userSettings["ExtractIncludeMessages"] == 1;
                    includeActions = userSettings["ExtractIncludeActions"] == 1;
                }
                includeDataNames = userSettings["ExtractUseDataNames"] == 1;
                useRewrittenStrings = userSettings["ExtractRewriteStrings"] == 1;
                blankOutMessages = userSettings["ExtractBlankOutMessages"] == 1;
                checkUnusedFiles = userSettings["CheckUnusedFiles"] == 1;
                checkUnusedData = userSettings["CheckUnusedDataEntries"] == 1;
                checkLineLength = userSettings["CheckLineLengths"] == 1;
                
                ibMode = false;
                starUnknownMode = false;
                if (userSettings["SpecialGameMode"] == 1)
                    ibMode = true;
                else if (userSettings["SpecialGameMode"] == 2)
                    starUnknownMode = true;
                else if (userSettings["SpecialGameMode"] == 3)
                    hanoiMode = true;
                
                string[] prefixes = { "Main", "Misc", "Filename" };
                for (int i = 0; i < 3; i++)
                {
                    string prefix = prefixes[i];
                    tryToSetReadEncoding(userSettings[prefix + "ReadEncoding"], i);
                    tryToSetWriteEncoding(userSettings[prefix + "WriteEncoding"], i);
                }
            }
        }
        static void validateUserSetting(string settingName, int min, int max, int def)
        {
            if (userSettings[settingName] < min || userSettings[settingName] > max)
                userSettings[settingName] = def;
        }
        static void validateUserSettingFilename(string settingName, string def)
        {
            if (userSettingsStr[settingName] == "" || !isValidFilename(userSettingsStr[settingName]))
                userSettingsStr[settingName] = def;
        }
        
        // Returns whether StringScriptDetails is enabled as well as the specific setting.
        public static bool getDetailSetting(string setting)
        {
            return userSettings["StringScriptDetails"] == 1 && detailSettings.Contains(setting);
        }
        
        // Returns whether StringScriptExtraneous is enabled as well as the specific setting.
        public static bool getExtraneousSetting(string setting)
        {
            return userSettings["StringScriptExtraneous"] == 1 && extraneousSettings.Contains(setting);
        }
        
        // When exporting StirngScripts with original string setting enabled, either gets original string from database, or adds string from data to database.
        public static string getOriginalString(string commandName, string dataString, bool database = false)
        {
            string myPage = !database? currentPage : currentLine;
            
            if (generatingOriginalStringDB)
            {
                if (originalStringDB == null)
                    originalStringDB = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>();
                if (!originalStringDB.ContainsKey(currentFile))
                    originalStringDB[currentFile] = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
                if (!originalStringDB[currentFile].ContainsKey(currentEvent))
                    originalStringDB[currentFile][currentEvent] = new Dictionary<string, Dictionary<string, List<string>>>();
                if (!originalStringDB[currentFile][currentEvent].ContainsKey(myPage))
                    originalStringDB[currentFile][currentEvent][myPage] = new Dictionary<string, List<string>>();
                if (!originalStringDB[currentFile][currentEvent][myPage].ContainsKey(commandName))
                    originalStringDB[currentFile][currentEvent][myPage][commandName] = new List<string>();
                
                string databaseString = "";
                if (dataString != null)
                    databaseString = dataString.Replace("\r\n", "\n");
                originalStringDB[currentFile][currentEvent][myPage][commandName].Add(databaseString);
                
                return dataString;
            }
            else
            {
                if (originalStringDB == null
                 || !originalStringDB.ContainsKey(currentFile)
                 || !originalStringDB[currentFile].ContainsKey(currentEvent)
                 || !originalStringDB[currentFile][currentEvent].ContainsKey(myPage)
                 || !originalStringDB[currentFile][currentEvent][myPage].ContainsKey(commandName)
                 || originalStringDB[currentFile][currentEvent][myPage][commandName].Count == 0)
                    return "<Original string not found>";
                
                string dbString = originalStringDB[currentFile][currentEvent][myPage][commandName][0].Replace("\r\n", "\n").Replace("\n", "\r\n");
                originalStringDB[currentFile][currentEvent][myPage][commandName].RemoveAt(0);
                return dbString;
            }
        }
        
        // Tries to set the encoding, defaulting to Shift-JIS if invalid.
        static void tryToSetReadEncoding(int num, int strType)
        {
            try
            {
                readEncodings[strType] = Encoding.GetEncoding(num);
                readEncodingIDs[strType] = num;
            }
            catch (Exception)
            {
                Console.WriteLine(num + " is not a valid codepage number. Defaulting to Shift-JIS.");
                readEncodings[strType] = Encoding.GetEncoding(932);
                readEncodingIDs[strType] = 932;
                enterToContinue();
            }
        }
        static void tryToSetWriteEncoding(int num, int strType)
        {
            try
            {
                writeEncodings[strType] = Encoding.GetEncoding(num);
                writeEncodingIDs[strType] = num;
            }
            catch (Exception)
            {
                Console.WriteLine(num + " is not a valid codepage number. Defaulting to Shift-JIS.");
                readEncodings[strType] = Encoding.GetEncoding(932);
                readEncodingIDs[strType] = 932;
                enterToContinue();
            }
        }
        
        // In Extracting mode, quickly reads the database just to determine if it's 2003.
        static void getLDBVersion()
        {
            if (globalMode == "Extracting")
            {
                gettingLDBVersion = true;
                
                if (File.Exists(gamePath + "\\RPG_RT.ldb"))
                    processDatabase(gamePath + "\\RPG_RT.ldb", false);
                else
                    Console.WriteLine("RPG_RT.ldb not found. Assuming version as 2000.");
                
                gettingLDBVersion = false;
            }
        }
        
        // In Extracting mode, quickly reads the database and map tree just to load in map and data names.
        static void readInDataNames(bool forceRead = false)
        {
            if ((globalMode == "Extracting" && includeDataNames) || forceRead)
            {
                readingDataNames = true;
                
                if (File.Exists(gamePath + "\\RPG_RT.ldb"))
                    processDatabase(gamePath + "\\RPG_RT.ldb", false);
                else
                    Console.WriteLine("RPG_RT.ldb not found. Data names could not be loaded.");
                
                if (File.Exists(gamePath + "\\RPG_RT.lmt"))
                    processMapTree(gamePath + "\\RPG_RT.lmt", false);
                else
                    Console.WriteLine("RPG_RT.lmt not found. Map names could not be loaded.");
                
                readingDataNames = false;
            }
        }
        
        // Generates a list of files in the resource folders to help prepare an input file for Rewriting.
        static bool generateFilenames(string filepath, bool nonASCIIOnly = true)
        {
            string dir = Path.GetDirectoryName(filepath);
            
            if (File.Exists("filelist.txt"))
                File.Delete("filelist.txt");
            using (StreamWriter f = new StreamWriter("filelist.txt", false, UNICODE))
            {
                // Go through every folder.
                for (int i = 0; i < FOLDERCOUNT; i++)
                {
                    if (Directory.Exists(dir + "\\" + FOLDER[i]))
                    {
                        bool headerWritten = false;
                        IEnumerable<string> fileList = Directory.EnumerateFiles(dir + "\\" + FOLDER[i], "*.*");
                        foreach (string file in fileList)
                        {
                            string filename = Path.GetFileNameWithoutExtension(file);
                            if (!nonASCIIOnly || !isValid(filename))
                            {
                                if (!headerWritten)
                                {
                                    f.WriteLine("***" + FOLDER[i].ToUpper());
                                    headerWritten = true;
                                }
                                f.WriteLine(filename);
                                f.WriteLine("___");
                            }
                        }
                        if (headerWritten)
                            f.WriteLine();
                    }
                }
                f.Close();
            }
            
            return true;
        }
        
        // Renames files in all subfolders according to the loaded input file. Returns true if anything was actually renamed.
        static bool translateFilenames(string filepath)
        {
            string dir = Path.GetDirectoryName(filepath);
            bool anyRenameExists = false;
            
            Console.WriteLine("Renaming files...");
            
            // Go through every folder.
            for (int i = 0; i < FOLDERCOUNT; i++)
            {
                if (Directory.Exists(dir + "\\" + FOLDER[i]))
                {
                    bool renameExists = false;
                    IEnumerable<string> fileList = Directory.EnumerateFiles(dir + "\\" + FOLDER[i], "*.*");
                    foreach (string file in fileList)
                    {
                        // If original list contains the filename, rename the file to the translation.
                        // The lists contain only names, so remove the path and keep the extension separate.
                        // Also, don't try to rename to an existing file - it'll throw an actual error if it tries.
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        string fileExt = Path.GetExtension(file);
                        if (transList[i, 0].Contains(fileName))
                        {
                            if (!renameExists)
                            {
                                renameExists = true;
                                anyRenameExists = true;
                                Console.WriteLine("Renaming files in " + FOLDER[i] + " folder...");
                            }
                            string renamedName = transList[i, 1][transList[i, 0].IndexOf(fileName)];
                            string destination = dir + "\\" + FOLDER[i] + "\\" + renamedName + fileExt;
                            if (!File.Exists(destination))
                                File.Move(file, destination);
                            else
                                Console.WriteLine("Cannot rename to " + renamedName + ". A file with that name already exists.");
                        }
                    }
                }
            }
            
            return anyRenameExists;
        }
        
        // Creates unusedFiles list with all resource files, which can then be removed as references are found.
        static void initializeUnusedFiles(string filepath)
        {
            unusedFiles = new Dictionary<string, List<string>>();
            unusedFilesLowercase = new Dictionary<string, List<string>>();
            sameNameFiles = new Dictionary<string, List<string>>();
            sameNameFilesLowercase = new Dictionary<string, List<string>>();
            
            string dir = Path.GetDirectoryName(filepath);
            
            // Go through every folder.
            for (int i = 0; i < FOLDERCOUNT; i++)
            {
                unusedFiles[FOLDER[i]] = new List<string>();
                sameNameFiles[FOLDER[i]] = new List<string>();
                unusedFilesLowercase[FOLDER[i]] = new List<string>();
                sameNameFilesLowercase[FOLDER[i]] = new List<string>();
                
                if (Directory.Exists(dir + "\\" + FOLDER[i]))
                {
                    IEnumerable<string> fileList = Directory.EnumerateFiles(dir + "\\" + FOLDER[i], "*.*");
                    foreach (string file in fileList)
                    {
                        if (Path.GetFileName(file).Equals("Thumbs.db"))
                            continue;
                        
                        string filename = Path.GetFileNameWithoutExtension(file);
                        string toLower = filename.ToLower();
                        
                        if (!unusedFilesLowercase[FOLDER[i]].Contains(toLower))
                        {
                            unusedFiles[FOLDER[i]].Add(filename);
                            unusedFilesLowercase[FOLDER[i]].Add(toLower);
                        }
                        else if (!sameNameFilesLowercase[FOLDER[i]].Contains(toLower))
                        {
                            sameNameFiles[FOLDER[i]].Add(filename);
                            sameNameFilesLowercase[FOLDER[i]].Add(toLower);
                        }
                    }
                }
            }
        }
        
        // Writes filename issues from Checking mode to log, once either the single file or all files have been read.
        static void writeFileIssues()
        {
            if (checkIssueReference)
            {
                foreach (string key in FOLDER)
                {
                    if (missingFiles.ContainsKey(key))
                    {
                        missingFiles[key].Sort();
                        foreach (string file in missingFiles[key])
                            logMessage(key + " file not found: " + file, true);
                    }
                }
                
                logMessage("");
            }
            
            if (allFiles) // Unused stuff only makes sense for All Files, because whether a single file doesn't use everything is irrelevant
            {
                if (checkUnusedFiles)
                {
                    foreach (string key in FOLDER)
                    {
                        string priority = "";
                        if (key.Equals("Music"))
                            priority = "priority is .mid > .wav > .mp3";
                        else if (key.Equals("Sound"))
                            priority = "only .wav supported";
                        else if (key.Equals("Movie"))
                            priority = "priority is .avi > .mpg";
                        else // Image
                            priority = "priority is .bmp > .png > .xyz";
                        
                        if (unusedFiles.ContainsKey(key))
                        {
                            if (unusedFiles[key].Count > 0)
                            {
                                checkIssueUnusedFile = true;
                                unusedFiles[key].Sort();
                                foreach (string file in unusedFiles[key])
                                    logMessage(key + " file never used: " + file, true);
                            }
                        }
                        if (sameNameFiles.ContainsKey(key))
                        {
                            if (sameNameFiles[key].Count > 0)
                            {
                                checkIssueUnusedFile = true;
                                sameNameFiles[key].Sort();
                                foreach (string file in sameNameFiles[key])
                                    logMessage(key + " filename overlap (" + priority + "): " + file, true);
                            }
                        }
                    }
                    
                    logMessage("");
                }
                
                if (checkUnusedData)
                {
                    foreach (string key in unusedDataEntries.Keys)
                    {
                        List<int> entries = unusedDataEntries[key];
                        
                        string[] tabNames = key.Equals("Heroes")? heroNames
                                          : key.Equals("Skills")? skillNames
                                          : key.Equals("Items")? itemNames
                                          : key.Equals("Monsters")? monsterNames
                                          : key.Equals("Troops")? troopNames
                                          : key.Equals("Attributes")? attributeNames
                                          : key.Equals("Conditions")? conditionNames
                                          : key.Equals("Animations")? animationNames
                                          : key.Equals("Terrains")? terrainNames
                                          : key.Equals("ChipSets")? chipSetNames
                                          : key.Equals("Common Events")? commonNames
                                          : key.Equals("Classes")? classNames
                                          : key.Equals("Battler Animations")? battlerAnimSetNames
                                          : new string[] { "" };
                        
                        bool uncertain = false;
                        
                        if (entries.Count > 0)
                        {
                            checkIssueUnusedDatabase = true;
                            if (variableReferencedDatabases.Contains(key))
                            {
                                logMessage("[" + key + " database was referred to by variable at least once.]");
                                logMessage("[As such, whether the following entries are truly unused is uncertain.]");
                                uncertain = true;
                            }
                        }
                        
                        foreach (int entry in entries)
                            logMessage(key + " entry #" + entry + " (" + tabNames[entry - 1] + ") " + (uncertain? "may or may not be" : "is not") + " used.");
                    }
                }
            }
        }
        
        // Prompts whether to move unused resources to separate subfolders.
        static void promptIsolateUnusedFiles()
        {
            if (!checkUnusedFiles || !checkIssueUnusedFile)
                return;
            
            bool doIsolate = false;
            if (!commandLineMode)
            {
                Console.WriteLine("Move unused files into \"Unused\" subfolders? (Y/N)");
                doIsolate = yesNoPrompt(true);
            }
            else
                doIsolate = cmdIsolateUnusedFiles;
            
            if (doIsolate)
            {
                foreach (string key in FOLDER)
                {
                    List<string> possibleExtensions;
                    if (key.Equals("Music"))
                        possibleExtensions = new List<string>(new string[] { ".mid", ".wav", ".mp3" });
                    else if (key.Equals("Sound"))
                        possibleExtensions = new List<string>(new string[] { ".wav" });
                    else if (key.Equals("Movie"))
                        possibleExtensions = new List<string>(new string[] { ".avi", ".mpg" });
                    else // Image
                        possibleExtensions = new List<string>(new string[] { ".bmp", ".png", ".xyz" });
                    
                    string resourceFolder = gamePath + "\\" + key;
                    string unusedFolder = resourceFolder + "\\Unused";
                    
                    if (sameNameFiles.ContainsKey(key))
                    {
                        if (sameNameFiles[key].Count > 0)
                        {
                            Directory.CreateDirectory(unusedFolder);
                            
                            foreach (string file in sameNameFiles[key])
                            {
                                // Determine which extension has priority (the first one in the list that exists), and move any other extensions.
                                string priorityExt = "";
                                for (int i = 0; i < possibleExtensions.Count; i++)
                                {
                                    string ext = possibleExtensions[i];
                                    if (File.Exists(resourceFolder + "\\" + file + ext))
                                    {
                                        priorityExt = ext;
                                        break;
                                    }
                                }
                                
                                foreach (string ext in possibleExtensions)
                                {
                                    string sourceFile = resourceFolder + "\\" + file + ext;
                                    if (File.Exists(sourceFile) && ext != priorityExt)
                                    {
                                        string destinationFile = unusedFolder + "\\" + file + ext;
                                        if (File.Exists(destinationFile))
                                            File.Delete(destinationFile);
                                        File.Move(sourceFile, destinationFile);
                                    }
                                }
                            }
                        }
                    }
                    
                    if (unusedFiles.ContainsKey(key))
                    {
                        if (unusedFiles[key].Count > 0)
                        {
                            Directory.CreateDirectory(unusedFolder);
                            
                            foreach (string file in unusedFiles[key])
                            {
                                foreach (string ext in possibleExtensions)
                                {
                                    string sourceFile = resourceFolder + "\\" + file + ext;
                                    if (File.Exists(sourceFile))
                                    {
                                        string destinationFile = unusedFolder + "\\" + file + ext;
                                        if (File.Exists(destinationFile))
                                            File.Delete(destinationFile);
                                        File.Move(sourceFile, destinationFile);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        // If using data names, these return the stored name of that data index. Otherwise, they return a generic identifier like "Hero #1."
        public static string getDataHero(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Heroes"))
                unusedDataEntries["Heroes"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < heroNames.Length? heroNames[i - 1] : "(undefined)")
                : "Hero #" + i);
        }
        public static string getDataSkill(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Skills"))
                unusedDataEntries["Skills"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < skillNames.Length? skillNames[i - 1] : "(undefined)")
                : "Skill #" + i);
        }
        public static string getDataItem(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Items"))
                unusedDataEntries["Items"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < itemNames.Length? itemNames[i - 1] : "(undefined)")
                : "Item #" + i);
        }
        public static string getDataMonster(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Monsters"))
                unusedDataEntries["Monsters"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < monsterNames.Length? monsterNames[i - 1] : "(undefined)")
                : "Monster #" + i);
        }
        public static string getDataTroop(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Troops"))
                unusedDataEntries["Troops"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < troopNames.Length? troopNames[i - 1] : "(undefined)")
                : "Troop #" + i);
        }
        public static string getDataAttribute(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Attributes"))
                unusedDataEntries["Attributes"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < attributeNames.Length? attributeNames[i - 1] : "(undefined)")
                : "Attribute #" + i);
        }
        public static string getDataCondition(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Conditions"))
                unusedDataEntries["Conditions"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < conditionNames.Length? conditionNames[i - 1] : "(undefined)")
                : "Condition #" + i);
        }
        public static string getDataAnimation(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Animations"))
                unusedDataEntries["Animations"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < animationNames.Length? animationNames[i - 1] : "(undefined)")
                : "Animation #" + i);
        }
        public static string getDataTerrain(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Terrains"))
                unusedDataEntries["Terrains"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < terrainNames.Length? terrainNames[i - 1] : "(undefined)")
                : "Terrain #" + i);
        }
        public static string getDataChipSet(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("ChipSets"))
                unusedDataEntries["ChipSets"].Remove(i);
            
            return (includeDataNames? (i > 0 && i - 1 < chipSetNames.Length? chipSetNames[i - 1] : "(undefined)")
                : "ChipSet #" + i);
        }
        public static string getDataCommon(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Common Events"))
                unusedDataEntries["Common Events"].Remove(i);
            
            return (includeDataNames? (i + ": " + (i > 0 && i - 1 < commonNames.Length? commonNames[i - 1] : "(undefined)"))
                : "Common #" + i);
        }
        public static string getDataMap(int i)
        {
            return (includeDataNames? (i.ToString("D4") + ": " + (i > 0 && i - 1 < mapNames.Length && i - 1 >= 0? mapNames[i - 1] : "(undefined)"))
                : "Map #" + i);
        }
        public static string getDataSwitch(int i)
        {
            return (includeDataNames? ("[" + i.ToString("D4") + ": "
                + (i > 0 && i - 1 < switchNames.Length? switchNames[i - 1] : "(undefined)") + "]")
                : "[" + i.ToString("D4") + "]");
        }
        public static string getDataVariable(int i)
        {
            return (includeDataNames? ("[" + i.ToString("D4") + ": "
                + (i > 0 && i - 1 < variableNames.Length? variableNames[i - 1] : "(undefined)") + "]")
                : "[" + i.ToString("D4") + "]");
        }
        public static string getDataBattleCommand(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Battle Commands"))
                unusedDataEntries["Battle Commands"].Remove(i);
            
            return (includeDataNames? ((i + 1) + ": " + (i > 0 && i - 1 < battleCommandNames.Length? battleCommandNames[i - 1] : "(undefined)"))
                : "Battle Command #" + i);
        }
        public static string getDataClass(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Classes"))
                unusedDataEntries["Classes"].Remove(i);
            
            return (includeDataNames? ((i + 1) + ": " + (i > 0 && i - 1 < classNames.Length? classNames[i - 1] : "(undefined)"))
                : "Class #" + i);
        }
        public static string getDataBattleAnimSet(int i)
        {
            if (checkUnusedData && unusedDataEntries.ContainsKey("Battler Animations"))
                unusedDataEntries["Battler Animations"].Remove(i);
            
            return (includeDataNames? ((i + 1) + ": " + (i > 0 && i - 1 < battlerAnimSetNames.Length? battlerAnimSetNames[i - 1] : "(undefined)"))
                : "Battler Animation Set #" + i);
        }
        public static string getDataBattlerPose(int animSet, int i)
        {
            if (i < 12) // Default poses
            {
                return new string[] { "Idle Stance", "Right Hand", "Left Hand", "Skill Use", "Dead",
                                      "Damage", "Bad Status", "Defending", "Walking Left", "Walking Right",
                                      "Victory", "Item" }[i];
            }
            
            // Determining which animation set is proper gets way too tricky for me to bother, so no data names for now.
            /*if (includeDataNames)
            {
                string str = "(undefined)";
                if (animSet < battlerPoseNames.Length)
                    if (i > 0 && i - 1 < battlerPoseNames[animSet].Length)
                        str = battlerPoseNames[animSet][i - 1];
                return (i + 1) + ": " + str;
            }
            else*/
            return "Battler Pose #" + (animSet != 0? (animSet + "-") : "") + (i + 1);
        }
        public static string getDataBattlerPose(int i)
        {
            return getDataBattlerPose(0, i);
        }
        public static string getDataWeaponAnimation(int animSet, int i)
        {
            // Determining which animation set is proper gets way too tricky for me to bother, so no data names for now.
            /*if (includeDataNames)
            {
                string str = "(undefined)";
                if (animSet < weaponAnimationNames.Length)
                    if (i > 0 && i - 1 < weaponAnimationNames[animSet].Length)
                        str = weaponAnimationNames[animSet][i - 1];
                return (i + 1) + ": " + str;
            }
            else*/
            return "Weapon Animation #" + (animSet != 0? (animSet + "-") : "") + (i + 1);
        }
        public static string getDataWeaponAnimation(int i)
        {
            return getDataWeaponAnimation(0, i);
        }
        
        /* STRING SHORTCUTS */
        
        // Returns the name of a "target" for Hero-oriented commands.
        public static string getHeroTarget(int target, int value)
        {
            if (target == 0) // All
                return "Whole Party";
            else if (target == 1) // Fixed
                return getDataHero(value);
            else if (target == 2) // Variable
                return "Variable " + getDataVariable(value);
            return "";
        }
        
        // Returns the name of a "target" for event-oriented commands.
        public static string getTargetEvent(int target)
        {
            if (target == 10001)
                return "Hero";
            else if (target == 10002)
                return "Boat";
            else if (target == 10003)
                return "Ship";
            else if (target == 10004)
                return "Airship";
            else if (target == 10005)
                return "This Event";
            return "Event #" + target;
        }
        
        // Returns a string of the volume, tempo, and balance for a song/sound.
        public static string getSoundVTB(int volume, int tempo, int balance)
        {
            string balanceStr = (balance - 50).ToString();
            if (balance > 50)
                balanceStr = "+" + balanceStr;
            
            return "Volume " + volume + "%, "
                + "Tempo " + tempo + "%, "
                + "Balance " + balanceStr;
        }
        
        // Returns the corresponding screen-erase transition effect.
        public static string getEraseEffects(int effect)
        {
            string[] effects = { "Fade-Out", "Whole Random Blocks", "Random Blocks Up", "Random Blocks Down",
                                 "Blinds Close", "Hi-Low Stripe", "Left-Right Stripe", "Outside-Inside", "Inside-Outside",
                                 "Scroll Down", "Scroll Up", "Scroll Right", "Scroll Left", "Hi-Low Divide", "Left-Right Divide",
                                 "Hi-Low Left-Right", "Zoom In", "Mosaic", "Roster Scroll", "Instant Erase" };
            
            if (effect >= 0 && effect < effects.Length)
                return effects[effect];
            else
                return "Create Removing Place";
        }
        
        // Returns the corresponding screen-show transition effect.
        public static string getShowEffects(int effect)
        {
            string[] effects = { "Fade-In", "Whole Random Blocks", "Random Blocks Up", "Random Blocks Down",
                                 "Blinds Open", "Hi-Low Stripe", "Left-Right Stripe", "Outside-Inside", "Inside-Outside",
                                 "Scroll Down", "Scroll Up", "Scroll Right", "Scroll Left", "Hi-Low Combine", "Left-Right Combine",
                                 "Hi-Low Left-Right", "Zoom Out", "Mosaic", "Roster Scroll", "Instant Display" };
            
            if (effect >= 0 && effect < effects.Length)
                return effects[effect];
            else
                return "Create Removing Place";
        }
        
        /* BYTE-READING */
        
        // Reads a single byte.
        public static byte readByte(FileStream f)
        {
            return (byte)f.ReadByte();
        }
        
        // Reads and returns the value of a int16 (two bytes).
        public static int readTwoBytes(FileStream f)
        {
            int total = readByte(f);
            total += readByte(f) * 256;
            return total;
        }
        
        // Reads and returns the value of a int32 (four bytes).
        public static long readFourBytes(FileStream f)
        {
            int total = readByte(f);
            total += readByte(f) * 256;
            total += readByte(f) * 65536;
            total += readByte(f) * 16777216;
            return total;
        }
        
        // Reads and returns the value of a "multibyte," used for values over 127 (0x7f).
        public static int readMultibyte(FileStream f, int sum = 0)
        {
            byte b = readByte(f);
            if (b < 128)
                return b + sum;
            else
                return readMultibyte(f, (b - 128 + sum) * 128);
        }
        
        // Reads a byte length (always 1), then a 0 or 1.
        public static bool readLengthBool(FileStream f)
        {
            byteCheck(f, 0x01);
            return (readByte(f) == 1);
        }
        
        // Reads a byte length, then a number of bytes dictated by the length.
        public static int readLengthBytes(FileStream f)
        {
            int length = readMultibyte(f);
            if (length == 1)
                return readByte(f);
            else if (length == 2)
                return readTwoBytes(f);
            else
                return readMultibyte(f);
        }
        
        // Reads a byte length, then a multibyte.
        public static int readLengthMultibyte(FileStream f)
        {
            readMultibyte(f);
            return readMultibyte(f);
        }
        
        // Reads a byte length (always 8), then an eight-byte double.
        public static double readLengthDouble(FileStream f)
        {
            byteCheck(f, 0x08);
            BinaryReader reader = new BinaryReader(f);
            return reader.ReadDouble();
        }
        
        // Reads a byte length, then a byte or two, which is translated into bools based on the bits.
        public static bool[] readLengthFlags(FileStream f)
        {
            int length = readByte(f);
            int flagCount = length * 8;
            int bits = length == 1? readByte(f) : readTwoBytes(f);
            
            bool[] flags = new bool[flagCount];
            for (int i = 0; i < flagCount; i++)
                flags[i] = (bits & (int)Math.Pow(2, i)) != 0;
            
            return flags;
        }
        
        // Reads a byte length, then an array of bytes.
        public static int[] readByteArray(FileStream f)
        {
            int length = readMultibyte(f);
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
                array[i] = readByte(f);
            return array;
        }
        
        // Reads a byte length, then an array of two-byte integers.
        public static int[] readTwoByteArray(FileStream f)
        {
            int length = readMultibyte(f);
            if (length < 2) // Length can be 0 or 1 when "empty"
            {
                int[] array = new int[length];
                for (int i = 0; i < length; i++)
                    array[i] = readByte(f);
                return array;
            }
            else
            {
                int[] array = new int[length / 2];
                for (int i = 0; i < length / 2; i++)
                    array[i] = readTwoBytes(f);
                return array;
            }
        }
        
        // Reads a byte length, then an array of four-byte integers.
        public static long[] readFourByteArray(FileStream f)
        {
            int length = readMultibyte(f);
            long[] array = new long[length / 4];
            for (int i = 0; i < length / 4; i++)
                array[i] = readFourBytes(f);
            return array;
        }
        
        // Reads a count, then an array of multibytes.
        public static int[] readMultibyteArray(FileStream f)
        {
            int count = readMultibyte(f);
            int[] array = new int[count];
            for (int i = 0; i < count; i++)
                array[i] = readMultibyte(f);
            return array;
        }
        
        // Reads a byte length, then an 2D array of two-byte integers. (Must provide dimension sizes, but size2 can be guessed from length and size1.)
        public static int[][] readTwoByteArray2D(FileStream f, int size1, int size2 = -1)
        {
            int length = readMultibyte(f);
            if (size2 == -1)
                size2 = (length / size1) / 2;
            
            int[][] array = new int[size1][];
            for (int i = 0; i < size1; i++)
            {
                array[i] = new int[size2];
                for (int j = 0; j < size2; j++)
                    array[i][j] = readTwoBytes(f);
            }
            return array;
        }
        
        // Reads a byte length, then an array of bools (0s or 1s).
        public static bool[] readBoolArray(FileStream f)
        {
            int length = readMultibyte(f);
            bool[] array = new bool[length];
            for (int i = 0; i < length; i++)
                array[i] = readByte(f) == 1;
            return array;
        }
        
        // Reads a byte length, then the string of that length following it.
        public static string readString(FileStream f, int strType)
        {
            int length = readMultibyte(f);
            
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
                bytes[i] = (byte)readByte(f);
            
            char[] chars = new char[readEncodings[strType].GetCharCount(bytes, 0, bytes.Length)];
            readEncodings[strType].GetChars(bytes, 0, bytes.Length, chars, 0);
            return new string(chars);
        }
        
        // Special version of readString for move routes that subtracts from lengthTemp, and in move commands, uses a different style of multibyte.
        public static string readStringMove(FileStream f, ref int lengthTemp, int strType, string source)
        {
            int length = readMultibyte(f);
            lengthTemp--;
            
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)readByte(f);
                lengthTemp--;
                if (bytes[i] >= 0x80)
                {
                    if (source == "Custom")
                    {
                        i++;
                        if (i < bytes.Length)
                            bytes[i] = (byte)readByte(f);
                        lengthTemp--;
                    }
                    else // Yep, it's different for non-custom routes. Whyyyyy
                        bytes[i] += (byte)(readByte(f) - 1);
                }
            }
            
            char[] chars = new char[readEncodings[strType].GetCharCount(bytes, 0, bytes.Length)];
            readEncodings[strType].GetChars(bytes, 0, bytes.Length, chars, 0);
            return new string(chars);
        }
        
        // Reads a string, may rewrite or check it depending on global mode, then returns the (possibly rewritten) string.
        public static string readStringAndRewrite(FileStream f, int mode, int strType, string source = "Event")
        {
            string readStr = readString(f, strType);
            return readStringAndRewriteShared(readStr, mode, strType, source);
        }
        
        // Special version of readAndRewrite for move events that subtracts from lengthTemp.
        public static string readStringMoveAndRewrite(FileStream f, int mode, ref int lengthTemp, int strType, string source)
        {
            string readStr = readStringMove(f, ref lengthTemp, strType, source);
            return readStringAndRewriteShared(readStr, mode, strType, source);
        }
        
        // The critical part shared by both versions of readStringAndRewrite.
        static string readStringAndRewriteShared(string str, int mode, int strType, string source)
        {
            if (!stringScriptImportMode // Don't want replacements taking effect at the same time as import
             && !readingDataNames // No reason to replace strings when just reading data names
             && !currentFile.EndsWith(".lsd") && saveRef == null) // Don't auto-rewrite filenames in save files, it's offered as a separate option
            {
                if (globalMode == "Rewriting" // Changing filenames in data
                 || (globalMode == "Extracting" && useRewrittenStrings) // Changing filenaems for scripts
                 || (completenessMode && completenessReplacement)) // Considering replacements for sake of determining completeness
                {
                    str = rewriteString(mode, str);
                    if (mode == M_MESSAGEALL) // When called for Message-All, do Message-Part and Message-Start too.
                    {
                        str = rewriteString(M_MESSAGESTART, str);
                        str = rewriteString(M_MESSAGEPART, str);
                    }
                }
            }
            
            checkStringValidForMode(str, mode);
            
            return str;
        }
        
        // In Checking mode, check if a file exists where it should, or if the string is "valid," based on the mode.
        public static void checkStringValidForMode(string str, int mode)
        {
            if (globalMode == "Checking")
            {
                if (mode < FOLDERCOUNT) // For filenames, check that they exist.
                {
                    if (includeActions) // Only check file validity if actions are included.
                    {
                        if (str != "" && str != "(OFF)") // Not a file, used to turn off the music.
                        {
                            DirectoryInfo root = new DirectoryInfo(gamePath + "\\" + FOLDER[mode]);
                            
                            // Folder doesn't exist, or no file matches the filename with any extension.
                            if (!root.Exists || root.GetFiles(str + ".*").Length == 0)
                            {
                                if (!missingFiles.ContainsKey(FOLDER[mode]))
                                    missingFiles[FOLDER[mode]] = new List<string>();
                                
                                if (!missingFiles[FOLDER[mode]].Contains(str))
                                    missingFiles[FOLDER[mode]].Add(str);
                                checkIssueReference = true;
                            }
                        }
                    }
                    
                    if (checkUnusedFiles)
                    {
                        if (unusedFilesLowercase.ContainsKey(FOLDER[mode]))
                        {
                            if (unusedFilesLowercase[FOLDER[mode]].Contains(str.ToLower()))
                            {
                                DirectoryInfo root = new DirectoryInfo(gamePath + "\\" + FOLDER[mode]);
                                
                                // Folder exists, and a file matches the filename with any extension.
                                if (root.Exists && root.GetFiles(str + ".*").Length > 0)
                                {
                                    int index = unusedFilesLowercase[FOLDER[mode]].IndexOf(str.ToLower());
                                    unusedFiles[FOLDER[mode]].RemoveAt(index);
                                    unusedFilesLowercase[FOLDER[mode]].RemoveAt(index);
                                }
                            }
                        }
                    }
                }
                else if (mode != M_COMMENT)
                {
                    if (includeMessages) // Only check message validity if messages are included.
                    {
                        if (!isValid(str))
                        {
                            string type = "[" + currentPosition() + "] ";
                            if (mode == M_MESSAGEALL || mode == M_MESSAGEPART || mode == M_MESSAGESTART)
                                type += "Message";
                            else if (mode == M_OPTION)
                                type += "Choice";
                            else if (mode == M_NAME || mode == M_NICKNAME)
                                type += "Name";
                            logMessage(type + " invalid: " + str);
                            checkIssueMessage = true;
                        }
                    }
                }
            }
        }
        
        // Applies translations to the string from the given mode's input list, then returns it.
        public static string rewriteString(int mode, string str)
        {
            // Don't do Message-Alls or Options in completeness checking, since that results in false positives.
            if ((mode == M_MESSAGEALL || mode == M_OPTION) && completenessMode)
                return str;
            
            string originalStr = str;
            
            List<string> mySourceList = transList[mode, 0];
            List<string> myTransList = transList[mode, 1];
            
            if (mode == M_MESSAGEALL || mode == M_MESSAGEPART || mode == M_OPTION)
            {
                for (int i = 0; i < mySourceList.Count; i++)
                    if (str.Contains(mySourceList[i])) // Does string contain instance(s) of original entry?
                        str = str.Replace(mySourceList[i], myTransList[i]); // Replace with corresponding translation.
            }
            else if (mode == M_MESSAGESTART)
            {
                for (int i = 0; i < mySourceList.Count; i++)
                    if (str.StartsWith(mySourceList[i])) // Does string start with original entry?
                        str = str.Replace(mySourceList[i], myTransList[i]); // Replace with corresponding translation.
            }
            else // Must match string exactly
            {
                if (mySourceList.Contains(str))
                    str = myTransList[mySourceList.IndexOf(str)];
                else
                {
                    if (mode < FOLDERCOUNT) // Only a concern for filenames.
                    {
                        if (!isValid(str)) // Invalid filename which has no replacement.
                            logMessage(FOLDER[mode] + " replacement not found: " + str, true);
                    }
                }
            }
            
            if (globalMode == "Rewriting" && !str.Equals(originalStr))
                changesMade = true;
            
            return str;
        }
        
        // Reads a database entry name string, and if in an appropriate mode, puts it into the given data names array.
        public static string readStringDataName(FileStream f, int id, ref string[] dataNames, int strType)
        {
            string dataName = readString(f, strType);
            if ((readingDataNames || makingDataEntryLists) && id >= 1 && id < dataNames.Length)
                dataNames[id - 1] = dataName;
            return dataName;
        }
        
        // Reads a byte length, a count, then a list of objects.
        public static List<T> readList<T>(FileStream f, string mode = "", int arg = 0) where T : RPGByteData, new()
        {
            readMultibyte(f);
            int count = readMultibyte(f);
            List<T> list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                if (mode.Equals("Page"))
                {
                    currentPage = "Page " + (i + 1);
                    currentPageNum = i + 1;
                }
                else if (mode.Equals("Pose"))
                    battlerPoseNames[arg - 1] = new string[count];
                else if (mode.Equals("Weapon"))
                    weaponAnimationNames[arg - 1] = new string[count];
                
                T obj = new T();
                obj.load(f);
                list.Add(obj);
            }
            return list;
        }
        
        // Reads a count, then a list of one-byte objects.
        public static List<T> readListNoLength<T>(FileStream f) where T : RPGData, new()
        {
            int count = readMultibyte(f);
            List<T> list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T obj = new T();
                obj.load(f);
                list.Add(obj);
            }
            return list;
        }
        
        // Reads a byte length, then a list of commands, stopping at the end command.
        public static List<Command> readCommandList(FileStream f)
        {
            readMultibyte(f); // Length
            List<Command> list = new List<Command>();
            int line = 1;
            while (true)
            {
                currentLine = "Line " + line++;
                
                Command command = new Command(f);
                list.Add(command);
                
                if (command.isEndCommand())
                    break;
            }
            return list;
        }
        
        // Reads a list of database entries.
        public static List<T> readDatabaseList<T>(FileStream f, string plural, string singular, ref string[] dataNames) where T : RPGDatabaseEntry, new()
        {
            currentEvent = plural;
            currentPage = "";
            currentEventNum = 0;
            currentPageNum = 0;
            
            int tabCount = readMultibyte(f);
            if (readingDataNames || makingDataEntryLists)
                dataNames = new string[tabCount];
            
            if (plural.Equals("Battler Animations"))
            {
                battlerPoseNames = new string[tabCount][];
                weaponAnimationNames = new string[tabCount][];
            }
            
            List<T> entries = new List<T>();
            for (int i = 0; i < tabCount; i++)
            {
                if (plural.Equals("Common Events") || plural.Equals("Troops"))
                {
                    currentEvent = singular + " " + (i + 1);
                    currentEventNum = i + 1;
                }
                else
                {
                    currentLine = singular + " " + (i + 1);
                    if (plural.Equals("Battler Animations"))
                        currentBattlerAnimation = i;
                }
                
                T entry = new T();
                entry.load(f);
                entries.Add(entry);
            }
            
            if (makingDataEntryLists)
            {
                if (!plural.Equals("Switches") && !plural.Equals("Variables"))
                {
                    string n = plural;
                    unusedDataEntries[n] = new List<int>();
                    for (int i = 0; i < tabCount; i++)
                        if (!entries[i].isBlank())
                            unusedDataEntries[n].Add(i + 1);
                }
            }
            
            return entries;
        }
        
        // Reads a byte that should be guaranteed to have the given value. If it doesn't, something went wrong, so an exception is thrown.
        public static void byteCheck(FileStream f, byte b)
        {
            byte bRead = readByte(f);
            if (b != bRead)
            {
                Console.WriteLine("Warning! A byte check failed.\n"
                                + "(At position " + hexParen(f.Position - 1) + ", read " + hexParen(bRead, 2) + ", should be " + hexParen(b, 2) + ".)\n"
                                + "Possible corrupt file or program bug.");
                throw new Exception();
            }
        }
        
        // Peeks at the next byte in the file, keeping the same position.
        public static byte bytePeek(FileStream f)
        {
            byte b = readByte(f);
            f.Seek(-1, SeekOrigin.Current);
            return b;
        }
        
        // Reads a string that should be guaranteed to have the given value using byteChecks.
        public static void stringCheck(FileStream f, string str)
        {
            byteCheck(f, (byte)stringByteLength(str));
            foreach (char c in str)
                byteCheck(f, (byte)c);
        }
        
        // Skips through a chunk if the given opcode is found. Returns an array of the bytes skipped.
        public static byte[] skipChunk(FileStream f, byte b, bool printBytes = false)
        {
            long startPosition = f.Position;
            
            byte readB = readByte(f);
            if (readB >= 128)
            {
                f.Seek(-1, SeekOrigin.Current);
                readB = (byte)M.readMultibyte(f);
            }
            
            if (b == readB)
            {
                List<byte> bytesRead = new List<byte>();
                bytesRead.Add(b);
                
                int length = readMultibyte(f);
                bytesRead.AddRange(getMultibytesForValue(length));
                
                bytesRead.AddRange(skipBytes(f, length));
                
                if (printBytes)
                {
                    Console.Write("Skipped chunk " + M.hex(b, 2) + " at " + M.hex(startPosition) + ": ");
                    foreach (byte read in bytesRead)
                        Console.Write(M.hex(read, 2) + " ");
                    Console.WriteLine();
                }
                
                return bytesRead.ToArray();
            }
            else
            {
                f.Seek(-M.countMultibyte(readB), SeekOrigin.Current);
                return new byte[0];
            }
        }
        
        // Runs skipChunk on a range of bytes in ascending order. Returns an array of the bytes skipped.
        public static byte[] skipChunkRange(FileStream f, byte start, byte end)
        {
            List<byte> bytesRead = new List<byte>();
            for (byte i = start; i <= end; i++)
                bytesRead.AddRange(skipChunk(f, i));
            return bytesRead.ToArray();
        }
        
        // Reads through the specified number of bytes. Returns an array of the bytes skipped.
        public static byte[] skipBytes(FileStream f, int n)
        {
            byte[] bytesRead = new byte[n];
            for (int i = 0; i < n; i++)
                bytesRead[i] = readByte(f);
            return bytesRead;
        }
        
        // Reads a byte length, then skips through that many bytes. Returns an array of the bytes skipped.
        public static byte[] skipLengthBytes(FileStream f)
        {
            int length = readMultibyte(f);
            return skipBytes(f, length);
        }
        
        /* BYTE-WRITING */
        
        // Writes a single byte to the target.
        public static void writeByte(int b)
        {
            targetWriter.Write((byte)b);
        }
        
        // Writes a value to the target as a multibyte.
        public static void writeMultibyte(int n)
        {
            writeByteArrayNoLength(getMultibytesForValue(n));
        }
        
        // Returns the bytes that should be written for this value.
        public static byte[] getMultibytesForValue(int value)
        {
            BinaryWriter bytes = new BinaryWriter(new MemoryStream());
            
            long n = (long)value;
            if (n < 0)
                n = (long)Math.Pow(2, 32) + n;
            
            if (n < 128)
                bytes.Write((byte)n);
            else
            {
                if (div(n, 128) + 128 <= 255) // Only one higher order byte will be needed.
                    bytes.Write((byte)(div(n, 128) + 128));
                else // Two or more will be needed.
                    getMultibytesForValue2(bytes, div(n, 128) + 128);
                bytes.Write((byte)(n % 128));
            }
            
            bytes.Close();
            return (bytes.BaseStream as MemoryStream).ToArray();
        }
        static void getMultibytesForValue2(BinaryWriter bytes, long n)
        {
            if (n < 128)
                bytes.Write((byte)(n + 128));
            else
            {
                if (div(n, 128) + 127 <= 255) // Only one higher order byte will be needed.
                    bytes.Write((byte)(div(n, 128) + 127));
                else // Two or more will be needed.
                    getMultibytesForValue2(bytes, div(n, 128) + 127);
                bytes.Write((byte)((n % 128) + 128));
            }
        }
        
        // Counts how many bytes would be used to write the given value as a multibyte.
        public static int countMultibyte(int n)
        {
            if (n < 128 && n >= 0)
                return 1;
            return getMultibytesForValue(n).Length;
        }
        
        // Writes a value as an int16 (two bytes).
        public static void writeTwoBytes(int value)
        {
            writeByte(value % 256);
            writeByte((value >> 8) % 256);
        }
        
        // Writes a value as an int32 (four bytes).
        public static void writeFourBytes(long value)
        {
            writeByte((int)(value % 256));
            writeByte((int)((value >> 8) % 256));
            writeByte((int)((value >> 16) % 256));
            writeByte((int)((value >> 24) % 256));
        }
        
        // Writes a byte length (always 1), then true (1) or false (0).
        public static void writeLengthBool(bool b)
        {
            writeByte(0x01);
            writeByte(b? (byte)0x01 : (byte)0x00);
        }
        
        // Writes a byte length, then a number of bytes dictated by the length.
        public static void writeLengthBytes(int value)
        {
            int length = value > 255? 2 : 1;
            writeByte(length);
            if (length == 1)
                writeByte(value);
            else
                writeTwoBytes(value);
        }
        
        // Writes a byte length, then a multibyte.
        public static void writeLengthMultibyte(int n)
        {
            writeMultibyte(countMultibyte(n));
            writeMultibyte(n);
        }
        
        // Counts how many bytes would be used to write a length (which can itself take up multiple bytes) followed by a multibyte.
        public static int countLengthMultibyte(int n)
        {
            return countMultibyte(countMultibyte(n)) + countMultibyte(n);
        }
        
        // Writes a byte length (always 8), then an eight-byte double.
        public static void writeLengthDouble(double value)
        {
            writeByte(0x08);
            targetWriter.Write(value);
        }
        
        // Writes a byte length, then a byte or two, with bits based on an array of bools.
        public static void writeLengthFlags(bool[] flags)
        {
            int total = 0;
            for (int i = 0; i < flags.Length; i++)
                if (flags[i])
                    total += (int)Math.Pow(2, i);
            
            int byteLength = flags.Length < 8? 1 : 2;
            writeByte(byteLength);
            if (byteLength == 1)
                writeByte(total);
            else
                writeTwoBytes(total);
        }
        
        // Writes a byte length, then an array of bytes.
        public static void writeByteArray(int[] bytes)
        {
            writeByte(bytes.Length);
            foreach (byte b in bytes)
                writeByte(b);
        }
        
        // Writes an array of bytes.
        public static void writeByteArrayNoLength(byte[] bytes)
        {
            foreach (byte b in bytes)
                writeByte(b);
        }
        
        // Writes a byte length, then an array of two-byte integers.
        public static void writeTwoByteArray(int[] array)
        {
            if (array.Length < 1)
            {
                writeByteArray(array);
                return;
            }
            
            writeMultibyte(array.Length * 2);
            for (int i = 0; i < array.Length; i++)
                writeTwoBytes(array[i]);
        }
        
        // Writes a byte length, then an array of four-byte integers.
        public static void writeFourByteArray(long[] array)
        {
            writeMultibyte(array.Length * 4);
            for (int i = 0; i < array.Length; i++)
                writeFourBytes(array[i]);
        }
        
        // Writes a count, then an array of multibytes.
        public static void writeMultibyteArray(int[] array)
        {
            writeMultibyte(array.Length);
            for (int i = 0; i < array.Length; i++)
                writeMultibyte(array[i]);
        }
        
        // Writes a byte length, then an 2D array of two-byte integers.
        public static void writeTwoByteArray2D(int[][] array)
        {
            writeMultibyte(array.Length * array[0].Length * 2);
            for (int i = 0; i < array.Length; i++)
                for (int j = 0; j < array[0].Length; j++)
                    writeTwoBytes(array[i][j]);
        }
        
        // Writes a byte length, then an array of bools (0s or 1s).
        public static void writeBoolArray(bool[] array)
        {
            writeMultibyte(array.Length);
            for (int i = 0; i < array.Length; i++)
                writeByte(array[i]? 1 : 0);
        }
        
        // Writes the byte length of the string followed by the string to the target. Returns lengthMinus for special multibyte reads in move commands.
        public static int writeString(string str, int strType, string source = "")
        {
            byte[] bytes = writeEncodings[strType].GetBytes(str);
            writeMultibyte(bytes.Length);
            
            int lengthMinus = 0;
            
            for (int i = 0; i < bytes.Length; i++)
            {
                if (source == "Move" && bytes[i] >= 0x80) // Non-custom move event.
                {
                    writeByte(0x81);
                    writeByte(bytes[i] - 0x80);
                    lengthMinus++;
                }
                else // Everything else.
                    writeByte(bytes[i]);
            }
            
            return lengthMinus;
        }
        
        // Writes a byte length, a count, then a list of objects.
        public static void writeList<T>(List<T> list) where T : RPGByteData, new()
        {
            int byteLength = 0;
            byteLength += countMultibyte(list.Count);
            foreach (T obj in list)
                byteLength += obj.getLength();
            
            writeMultibyte(byteLength);
            writeMultibyte(list.Count);
            for (int i = 0; i < list.Count; i++)
                list[i].write();
        }
        
        // Writes a count, then a list of objects.
        public static void writeListNoLength<T>(List<T> list) where T : RPGData, new()
        {
            writeMultibyte(list.Count);
            for (int i = 0; i < list.Count; i++)
                list[i].write();
        }
        
        // Writes a byte length, then a list of Commands.
        public static void writeCommandList(List<Command> list)
        {
            int byteLength = 0;
            foreach (Command obj in list)
                byteLength += obj.getLength();
            
            writeMultibyte(byteLength);
            foreach (Command command in list)
                command.write();
        }
        
        /* GENERAL HELPER FUNCTIONS */
        
        // Creates a text file to which the text string is written.
        public static void writeToNewFile(string filename, string text)
        {
            string dir = Path.GetDirectoryName(filename);
            if (dir != "" && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            using (StreamWriter fWrite = new StreamWriter(filename, false, UNICODE))
            {
                fWrite.Write(text);
                fWrite.Close();
            }
        }
        
        // Checks if the file is in use.
        public static bool fileInUse(string filepath)
        {
            FileStream stream = null;
            
            try
            {
                FileInfo f = new FileInfo(filepath);
                stream = f.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            
            return false;
        }
        
        // Shows a general file dialog and returns filename, or "" if canceled.
        static string fileDialog(string type = "", bool secondProject = false)
        {
            if (commandLineMode)
            {
                string cmdFile = !secondProject? projectFile1 : projectFile2;
                
                if (Directory.Exists(cmdFile)) // Inputting a directory isn't normally allowed, so assume they want the .lmt inside
                    cmdFile = cmdFile + "\\RPG_RT.lmt";
                
                if (!File.Exists(cmdFile)) // You wouldn't OpenFileDialog a non-existent file
                {
                    Console.WriteLine("File path \"" + (!secondProject? projectFile1 : projectFile2) + "\" not valid.");
                    return "";
                }
                
                return cmdFile;
            }
            
            OpenFileDialog dialog = new OpenFileDialog();
            
            if (type == "") // Go! function
            {
                if (allFiles)
                {
                    dialog.Filter = "Map Tree File|*.lmt";
                    dialog.Title = "Select Project";
                }
                else
                {
                    string all = globalMode != "Checking"? ";*.lsd" : "";
                    string lsd = globalMode != "Checking"? "|Save File|*.lsd" : "";
                    dialog.Filter = "All Files|*.lmu;*.lmt;*.ldb" + all + "|Map File|*.lmu|Map Tree File|*.lmt|Database File|*.ldb" + lsd;
                    dialog.Title = "Select File to Process";
                }
            }
            else if (type == "lmt") // .lmt only
            {
                dialog.Filter = "Map Tree File|*.lmt";
                dialog.Title = "Select Project";
            }
            else if (type == "compare" || type == "combo") // Compare two projects, extract two-in-one message scripts
            {
                dialog.Filter = "Map Tree File|*.lmt";
                dialog.Title = !secondProject? "Select Original Project" : "Select Translated Project";
            }
            else if (type == "copy") // Copy from one project to another
            {
                dialog.Filter = "Map Tree File|*.lmt";
                dialog.Title = !secondProject? "Select Source Project" : "Select Destination Project";
            }
            
            if (dialog.ShowDialog() == DialogResult.OK)
                return dialog.FileName;
            return "";
        }
        
        // Shows "press Enter to continue" and waits for input, unless running from command line.
        public static void enterToContinue()
        {
            if (!commandLineMode)
            {
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
            }
        }
        
        // Lists all files in a directory (excluding Readme.txt and UserSettings.txt).
        static void printFileList(string path = "", string pattern = "*")
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + path, pattern);
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                string ext = Path.GetExtension(file);
                if (ext.Equals(".txt") && (filename.Equals("Readme") || filename.Equals("UserSettings")))
                    continue;
                Console.WriteLine("- " + filename);
            }
        }
        
        // Prompts user to input Y or N and returns answer.
        public static bool yesNoPrompt(bool anyNo = false)
        {
            string input;
            do
            {
                input = Console.ReadLine();
                if (input.ToUpper() == "Y")
                    return true;
                else if (anyNo || input.ToUpper() == "N")
                    return false;
            } while (input.ToUpper() != "Y" && input.ToUpper() != "N");
            
            return false;
        }
        
        // Puts text on clipboard normally, or writes to log file in command line mode, and displays message.
        public static void clipboardSetText(string str, string messageBase = "")
        {
            if (!commandLineMode)
            {
                Clipboard.SetText(str);
                if (messageBase != "")
                    Console.WriteLine(messageBase + " and put on clipboard.");
            }
            else
            {
                writeToNewFile(userSettingsStr["LogFilename"] + ".txt", str);
                if (messageBase != "")
                    Console.WriteLine(messageBase + " and written to " + userSettingsStr["LogFilename"] + ".txt.");
            }
        }
        
        // Converts byte array for multibyte to value.
        public static int convertFromMultibyte(byte[] bytes, int sum = 0)
        {
            byte b = bytes[0];
            if (b < 128)
                return b + sum;
            else
            {
                byte[] nextBytes = new byte[bytes.Length - 1];
                for (int i = 1; i < bytes.Length; i++)
                    nextBytes[i - 1] = bytes[i];
                return convertFromMultibyte(nextBytes, (b - 128 + sum) * 128);
            }
        }
        
        // Returns the integer division of two numbers.
        public static int div(int n1, int n2)
        {
            return (int)Math.Floor((double)n1 / n2);
        }
        public static long div(long n1, long n2)
        {
            return (long)Math.Floor((double)n1 / n2);
        }
        
        // Returns the value as a hex string.
        public static string hex(int value, int digits = 1)
        {
            string length = "";
            if (digits != 1)
                length = digits.ToString();
            return string.Format("{0:X" + length + "}", value);
        }
        public static string hex(long value, int digits = 1)
        {
            return hex((int)value);
        }
        
        // Returns the value with the hex string after it in parentheses (0x00).
        public static string hexParen(int value, int digits = 1)
        {
            return value + " (0x" + hex(value, digits) + ")";
        }
        public static string hexParen(long value, int digits = 1)
        {
            return hexParen((int)value, digits);
        }
        
        // Changes string to [Default] if it is \x01, which weirdly used as a default in save files.
        public static string x01Default(string str)
        {
            if (str.Equals("\x01"))
                return "[Default]";
            return str;
        }
        
        // Formats frame count (60 FPS) as hours:minutes:seconds.milliseconds.
        public static string framesToTime(int frames)
        {
            return secondsToTime(frames / 60) + "." + string.Format("{0:00}", (int)Math.Round(((frames % 60) / 60f) * 100f));
        }
        
        // Formats seconds to hours:minutes:seconds.
        public static string secondsToTime(int seconds)
        {
            string str = "";
            
            int hours = 0;
            if (seconds >= 3600)
            {
                hours = seconds / 3600;
                seconds -= hours * 3600;
                str += hours + ":";
            }
            
            int minutes = seconds / 60;
            seconds -= minutes * 60;
            
            str += string.Format(hours > 0? "{0:00}" : "{0}", minutes) + ":" + string.Format("{0:00}", seconds);
            return str;
        }
        
        // Translates bool to "on" or "off" string.
        public static string onOff(bool value)
        {
            return value? "On" : "Off";
        }
        
        // Returns value with preceding + or -.
        public static string withPlusMinus(int value)
        {
            return value < 0? value.ToString() : "+" + value;
        }
        
        // Returns whether the string can be reasonably interpreted as true or false.
        public static bool validTrueFalse(string value)
        {
            value = value.ToLower().Trim();
            if (value.Equals("y") || value.Equals("n")
             || value.Equals("yes") || value.Equals("no")
             || value.Equals("1") || value.Equals("0")
             || value.Equals("true") || value.Equals("false"))
                return true;
            return false;
        }
        
        // Parses a string as a bool.
        public static bool boolString(string value)
        {
            value = value.ToLower().Trim();
            if (value.Equals("y") || value.Equals("yes") || value.Equals("1") || value.Equals("true"))
                return true;
            return false;
        }
        
        // Returns whether a string contains all valid ASCII characters.
        public static bool isValid(string str)
        {
            str = str.Replace(Environment.NewLine, "");
            foreach (char c in str)
            {
                if ((int)c < (int)' ' || (int)c > (int)'~') // Outside ASCII range
                    if (c != '\r' && c != '\n') // Allow linebreaks
                        return false;
            }
            return true;
        }
        
        // Returns whether a string would be a valid filename.
        public static bool isValidFilename(string str)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach(char c in invalidChars)
            {
                if (str.IndexOf(c) != -1)
                    return false;
            }
            return true;
        }
        
        // Returns a string describing the current position of file processing.
        public static string currentPosition(bool includeLine = true)
        {
            return currentFile + " " + currentEvent + (currentPage != ""? " " + currentPage : "") + (includeLine? " " + currentLine : "");
        }
        
        // Sets current position variables as appropriate before exporting a database entry for StringScripts.
        public static void setCurrentDatabaseEntry(string databaseName, int id)
        {
            M.currentEvent = databaseName;
            M.currentEventNum = 0;
            M.currentPage = "";
            M.currentPageNum = 0;
            M.currentLine = id.ToString();
        }
        
        // Adds a string to the current comparison list.
        public static void addComparisonString(string str)
        {
            if (readingOriginal)
            {
                if (!origStringList.ContainsKey(currentFile))
                    origStringList[currentFile] = new Dictionary<int, Dictionary<int, List<string>>>();
                if (!origStringList[currentFile].ContainsKey(currentEventNum))
                    origStringList[currentFile][currentEventNum] = new Dictionary<int, List<string>>();
                if (!origStringList[currentFile][currentEventNum].ContainsKey(currentPageNum))
                    origStringList[currentFile][currentEventNum][currentPageNum] = new List<string>();
                origStringList[currentFile][currentEventNum][currentPageNum].Add(str);
            }
            else
            {
                if (!transStringList.ContainsKey(currentFile))
                    transStringList[currentFile] = new Dictionary<int, Dictionary<int, List<string>>>();
                if (!transStringList[currentFile].ContainsKey(currentEventNum))
                    transStringList[currentFile][currentEventNum] = new Dictionary<int, List<string>>();
                if (!transStringList[currentFile][currentEventNum].ContainsKey(currentPageNum))
                    transStringList[currentFile][currentEventNum][currentPageNum] = new List<string>();
                transStringList[currentFile][currentEventNum][currentPageNum].Add(str);
            }
            
            if (!sourceLocationString.ContainsKey(currentFile))
                sourceLocationString[currentFile] = new Dictionary<int, Dictionary<int, string>>();
            if (!sourceLocationString[currentFile].ContainsKey(currentEventNum))
                sourceLocationString[currentFile][currentEventNum] = new Dictionary<int, string>();
            
            sourceLocationString[currentFile][currentEventNum][currentPageNum] = currentPosition(false);
        }
        
        // Checks if each command in the given event and page of importingStringArgs is empty - if not, warn that the input has too many commands.
        public static void checkIfImportingStringsExhausted(int ev, int page = 0, bool troop = false)
        {
            if (!stringScriptImportMode)
                return;
            
            string localStr = troop? "Troop " + ev + " Page " + page
                            : page != 0? currentFile + " Event " + ev + " Page " + page
                                       : "Common " + ev;
            
            if (importingStringArgs.ContainsKey(ev))
            {
                if (importingStringArgs[ev].ContainsKey(page))
                {
                    foreach (string cmd in importingStringArgs[ev][page].Keys)
                    {
                        if (cmd.Equals("commoneventname")) // Extraneous string, not command
                            continue;
                        
                        if (importingStringArgs[ev][page][cmd].Count > 0)
                        {
                            int count = importingStringArgs[ev][page][cmd].Count;
                            string capsCommand = "";
                            switch (cmd)
                            {
                                case "message": capsCommand = "Message"; break;
                                case "choice": capsCommand = "Choice"; break;
                                case "namechange": capsCommand = "NameChange"; break;
                                case "titlechange": capsCommand = "TitleChange"; break;
                                case "namefork": capsCommand = "NameFork"; break;
                            }
                            
                            Console.WriteLine(localStr + " " + capsCommand + ": " + count + " more command" + (count != 1? "s" : "") + " in input than in data.");
                        }
                    }
                }
            }
        }
        
        // Adds a string to the current double-extraction list.
        public static void addExtractDoubleString(string str)
        {
            if (readingOriginal)
            {
                if (!origStringList.ContainsKey(currentFile))
                    origStringList[currentFile] = new Dictionary<int, Dictionary<int, List<string>>>();
                if (!origStringList[currentFile].ContainsKey(currentEventNum))
                    origStringList[currentFile][currentEventNum] = new Dictionary<int, List<string>>();
                if (!origStringList[currentFile][currentEventNum].ContainsKey(currentPageNum))
                    origStringList[currentFile][currentEventNum][currentPageNum] = new List<string>();
                origStringList[currentFile][currentEventNum][currentPageNum].Add(str);
            }
            else
            {
                if (!transStringList.ContainsKey(currentFile))
                    transStringList[currentFile] = new Dictionary<int, Dictionary<int, List<string>>>();
                if (!transStringList[currentFile].ContainsKey(currentEventNum))
                    transStringList[currentFile][currentEventNum] = new Dictionary<int, List<string>>();
                if (!transStringList[currentFile][currentEventNum].ContainsKey(currentPageNum))
                    transStringList[currentFile][currentEventNum][currentPageNum] = new List<string>();
                transStringList[currentFile][currentEventNum][currentPageNum].Add(str);
            }
        }
        
        // For comparison and unique/duplicate-compiling modes, add messages to lists as appropriate.
        public static void addMessageForSpecialFunctions(string str)
        {
            if (comparisonMode)
                addComparisonString(str);
            else if (extractDoubleMode)
                addExtractDoubleString(str);
            else if (compilingUnique)
            {
                if (!uniqueList.Contains(str))
                {
                    uniqueList.Add(str);
                    uniqueListSources.Add(new List<string>());
                }
                uniqueListSources[uniqueList.IndexOf(str)].Add(currentPosition());
            }
            else if (compilingDuplicates)
            {
                if (!duplicateMessageList.ContainsKey(str))
                    duplicateMessageList[str] = 1;
                else
                {
                    duplicateMessageList[str]++;
                    if (duplicateMessageList[str] > duplicateMessageHighest)
                        duplicateMessageHighest = duplicateMessageList[str];
                }
            }
        }
        
        // Copy command args from source project to dictionary, or paste them into destination project.
        public static void copyPasteCommandValues(int opcode, ref int[] args)
        {
            if (readingOriginal)
            {
                if (!actionCommandValues.ContainsKey(currentFile))
                    actionCommandValues[currentFile] = new Dictionary<int, Dictionary<int, Dictionary<int, List<int[]>>>>();
                if (!actionCommandValues[currentFile].ContainsKey(currentEventNum))
                    actionCommandValues[currentFile][currentEventNum] = new Dictionary<int, Dictionary<int, List<int[]>>>();
                if (!actionCommandValues[currentFile][currentEventNum].ContainsKey(currentPageNum))
                    actionCommandValues[currentFile][currentEventNum][currentPageNum] = new Dictionary<int, List<int[]>>();
                if (!actionCommandValues[currentFile][currentEventNum][currentPageNum].ContainsKey(opcode))
                    actionCommandValues[currentFile][currentEventNum][currentPageNum][opcode] = new List<int[]>();
                actionCommandValues[currentFile][currentEventNum][currentPageNum][opcode].Add(args);
            }
            else
            {
                if (actionCommandValues != null)
                {
                    if (actionCommandValues.ContainsKey(currentFile))
                    {
                        if (actionCommandValues[currentFile].ContainsKey(currentEventNum))
                        {
                            string evPageStr = "Event " + currentEventNum + " Page " + currentPageNum;
                            
                            if (actionCommandValues[currentFile][currentEventNum].ContainsKey(currentPageNum))
                            {
                                string commandName = "";
                                switch (opcode)
                                {
                                    case Command.C_WAIT: commandName = "Wait"; break;
                                    case Command.C_SCREENTONE: commandName = "Screen Tone"; break;
                                    case Command.C_SCREENFLASH: commandName = "Screen Flash"; break;
                                    case Command.C_SCREENSHAKE: commandName = "Screen Shake"; break;
                                    case Command.C_PANSCREEN: commandName = "Pan Screen"; break;
                                    case Command.C_FADEMUSIC: commandName = "Fade Out BGM"; break;
                                }
                                
                                if (actionCommandValues[currentFile][currentEventNum][currentPageNum].ContainsKey(opcode))
                                {
                                    string localStr = currentFile + " " + evPageStr + (currentLine != ""? " " + currentLine : "") + " " + commandName;
                                    
                                    if (actionCommandValues[currentFile][currentEventNum][currentPageNum][opcode].Count > 0)
                                    {
                                        int[] importArgs = actionCommandValues[currentFile][currentEventNum][currentPageNum][opcode][0];
                                        actionCommandValues[currentFile][currentEventNum][currentPageNum][opcode].RemoveAt(0);
                                        
                                        if (importArgs.Length != args.Length)
                                        {
                                            args = importArgs;
                                            changesMade = true;
                                            commandValueChangesMade = true;
                                        }
                                        else
                                        {
                                            bool different = false;
                                            for (int i = 0; i < args.Length && i < importArgs.Length; i++)
                                            {
                                                if (args[i] != importArgs[i])
                                                {
                                                    different = true;
                                                    break;
                                                }
                                            }
                                            
                                            if (different)
                                            {
                                                args = importArgs;
                                                changesMade = true;
                                                commandValueChangesMade = true;
                                            }
                                        }
                                    }
                                    else
                                        Console.WriteLine(localStr + ": Extra command in destination.");
                                }
                                else
                                    Console.WriteLine(currentFile + " " + evPageStr + " " + commandName + ": Command not in source.");
                            }
                        }
                    }
                }
            }
        }
        
        // Checks if each command in the given event and page of actionCommandValues is empty - if not, warn that the source has more commands.
        public static void checkIfCommandValuesExhausted(int ev, int page = 0, bool troop = false)
        {
            if (!copyingCommandValues)
                return;
            
            string localStr = troop? "Troop " + ev + " Page " + page
                            : page != 0? currentFile + " Event " + ev + " Page " + page
                                       : "Common " + ev;
            
            if (actionCommandValues.ContainsKey(currentFile))
            {
                if (actionCommandValues[currentFile].ContainsKey(ev))
                {
                    if (actionCommandValues[currentFile][ev].ContainsKey(page))
                    {
                        foreach (int opcode in actionCommandValues[currentFile][ev][page].Keys)
                        {
                            if (actionCommandValues[currentFile][ev][page][opcode].Count > 0)
                            {
                                int count = actionCommandValues[currentFile][ev][page][opcode].Count;
                                string commandName = "";
                                switch (opcode)
                                {
                                    case Command.C_WAIT: commandName = "Wait"; break;
                                    case Command.C_SCREENTONE: commandName = "Screen Tone"; break;
                                    case Command.C_SCREENFLASH: commandName = "Screen Flash"; break;
                                    case Command.C_SCREENSHAKE: commandName = "Screen Shake"; break;
                                    case Command.C_PANSCREEN: commandName = "Pan Screen"; break;
                                    case Command.C_FADEMUSIC: commandName = "Fade Out BGM"; break;
                                }
                                
                                Console.WriteLine(localStr + " " + commandName + ": " + count + "more command" + (count != 1? "s" : "") + " in input than in data.");
                            }
                        }
                    }
                }
            }
        }
        
        // Takes out escape codes in a message string for "displayed length" checking.
        public static string removeEscapeCodes(string str)
        {
            while (str.Contains("\\"))
            {
                int index = str.IndexOf('\\');
                if (index + 1 < str.Length)
                {
                    char codeChar = str[index + 1];
                    if (codeChar.ToString().ToLower().Equals("c") || codeChar.ToString().ToLower().Equals("s")) // Remove color and speed changes
                    {
                        int rightBracket = str.IndexOf(']', index + 1);
                        str = str.Remove(index, rightBracket - index + 1);
                    }
                    else if (codeChar.ToString().ToLower().Equals("v") || codeChar.ToString().ToLower().Equals("n")) // Variable/name references become "0"
                    {
                        int rightBracket = str.IndexOf(']', index + 1);
                        str = str.Replace(str.Substring(index, rightBracket - index + 1), "0");
                    }
                    else // Single character
                        str = str.Remove(index, 2);
                }
            }
            return str;
        }
        
        // Returns a generic format for single-line strings in editable scripts, particularly database ones.
        public static string databaseExportString(string header, string str, string comment = "", bool terminate = false)
        {
            string lb = Environment.NewLine;
            string terminator = lb + (terminate? "##" + lb + lb : "");
            
            string originalString = "";
            if (getDetailSetting("OriginalDatabaseStrings"))
                originalString = "//" + getOriginalString(header, str, true) + lb;
            else if (generatingOriginalStringDB) // Call method regardless to add string to database
                getOriginalString(header, str, true);
            
            return originalString + "#" + header + "#" + (comment != ""? " " + comment : "") + lb + str + terminator;
        }
        
        // A generic import method for command lists, for use by Page, TroopPage, and CommonEvent.
        public static void importPageCommands(ref List<Command> commands, int pageNum)
        {
            currentPage = "Page " + pageNum;
            currentPageNum = pageNum;
            
            if (importingStringArgs == null)
                return;
            
            Dictionary<int, List<string>> caseNames = new Dictionary<int, List<string>>();
            
            for (int i = 0; i < commands.Count; i++)
            {
                Command command = commands[i];
                command.updateMessageFaceOn();
                
                if (command.isMessageStart() || command.isChoice()
                 || command.isNameChange() || command.isTitleChange() || command.isNameFork() || command.isStringPicture())
                {
                    if (importingStringArgs.ContainsKey(currentEventNum))
                    {
                        string evPageStr = "Event " + currentEventNum + " Page " + currentPageNum;
                        
                        if (importingStringArgs[currentEventNum].ContainsKey(currentPageNum))
                        {
                            string commandShort = command.isMessageStart()? "Message"
                                                : command.isChoice()? "Choice"
                                                : command.isNameChange()? "NameChange"
                                                : command.isTitleChange()? "TitleChange"
                                                : command.isNameFork()? "NameFork"
                                                : command.isStringPicture()? "StringPicture"
                                                : "";
                            
                            if (importingStringArgs[currentEventNum][currentPageNum].ContainsKey(commandShort))
                            {
                                string localStr = currentFile + " " + evPageStr + " " + currentLine + " " + commandShort;
                                
                                if (importingStringArgs[currentEventNum][currentPageNum][commandShort].Count > 0)
                                {
                                    string importArg = importingStringArgs[currentEventNum][currentPageNum][commandShort][0];
                                    importingStringArgs[currentEventNum][currentPageNum][commandShort].RemoveAt(0);
                                    
                                    if (command.isMessageStart()) // Must add/remove lines as necessary
                                    {
                                        importArg = wordWrap(importArg);
                                        
                                        string[] lines = importArg.Split('\n');
                                        
                                        if (importArg.Trim().ToLower().Equals("<<remove>>") && !command.isBlank()) // Change Message command to blank
                                        {
                                            command = new Command(Command.C_BLANK, command.getIndent(), new int[] { Command.DUMMYMESSAGEARG }, "");
                                            commands[i] = command;
                                            importArg = "";
                                            lines = new string[] { "" };
                                            changesMade = true;
                                        }
                                        else if (!importArg.Trim().ToLower().Equals("<<remove>>") && command.isBlank()) // Change blank command to Message
                                        {
                                            command = new Command(Command.C_MESSAGE, command.getIndent(), new int[] { }, "");
                                            commands[i] = command;
                                            changesMade = true;
                                        }
                                        
                                        List<string> oldLines = new List<string>();
                                        oldLines.Add(command.getStringArg());
                                        
                                        if (command.setStringArg(lines[0]))
                                            changesMade = true;
                                        
                                        // First, remove all Message Follows after this Message.
                                        if (i + 1 < commands.Count)
                                        {
                                            do
                                            {
                                                Command commandForRemoval = commands[i + 1];
                                                if (commandForRemoval.isMessageFollowOrExtraBox())
                                                {
                                                    string oldLine = commandForRemoval.getStringArg();
                                                    oldLines.Add(oldLine);
                                                    commands.RemoveAt(i + 1);
                                                }
                                                else
                                                    break;
                                            } while (true);
                                        }
                                        
                                        // Then add the remaining lines as either Message Follows or, every 4 lines, an "extra" Message to make a new box.
                                        int linesLength = lines.Length;
                                        for (int j = 1; j < linesLength; j++)
                                        {
                                            if (j % 4 == 0) // New box with a dummy argument, so program can detect it and treat it like a single message
                                            {
                                                Command newBox = new Command(Command.C_MESSAGE, command.getIndent(), new int[] { Command.DUMMYMESSAGEARG }, lines[j]);
                                                commands.Insert((i++) + 1, newBox);
                                            }
                                            else // Following line
                                            {
                                                Command newLine = new Command(Command.C_MESSAGEFOLLOW, command.getIndent(), new int[] { }, lines[j]);
                                                commands.Insert((i++) + 1, newLine);
                                            }
                                        }
                                        
                                        // If necessary, compare line strings to determine if the message changed at all.
                                        if (!changesMade)
                                        {
                                            if (oldLines.Count != linesLength) // If line count differs, definitely changed
                                                changesMade = true;
                                            else
                                            {
                                                for (int j = 1; j < linesLength; j++)
                                                {
                                                    if (!oldLines[j].Equals(lines[j]))
                                                    {
                                                        changesMade = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (command.isChoice()) // Replace if choice length matches, then store strings to replace cases later
                                    {
                                        string[] commandChoices = command.getTrueChoices();
                                        string[] importChoices = importArg.Split('\n');
                                        
                                        if (commandChoices.Length == importChoices.Length)
                                        {
                                            // Note: It seems English 2003 may use ", " instead of "/" to separate choices, but / still works, so...
                                            if (command.setStringArg(importArg.Replace('\n', '/')))
                                                changesMade = true;
                                            
                                            int indent = command.getIndent();
                                            caseNames[indent] = new List<string>();
                                            foreach (string choice in importChoices)
                                                caseNames[indent].Add(choice);
                                        }
                                        else
                                            Console.WriteLine(localStr + ": Choice count does not match.");
                                    }
                                    else if (command.isNameChange() || command.isTitleChange()
                                          || command.isNameFork() || command.isStringPicture()) // Just replace string, easy
                                    {
                                        if (command.setStringArg(importArg))
                                            changesMade = true;
                                    }
                                }
                                else
                                    Console.WriteLine(localStr + ": No more input found to insert.");
                            }
                            else
                                Console.WriteLine("[" + currentFile + "] " + evPageStr + " " + commandShort + ": Command input not found.");
                        }
                        else
                            Console.WriteLine("[" + currentFile + "] " + evPageStr + ": Page not found in input file.");
                    }
                    else
                        Console.WriteLine("[" + currentFile + "] Event " + currentEventNum + ": Event not found in input file.");
                }
                else if (command.isChoiceCase()) // Set names given by preceding choice command on same indent level
                {
                    int indent = command.getIndent();
                    if (caseNames.ContainsKey(indent))
                    {
                        if (caseNames[indent].Count > 0)
                        {
                            string caseName = caseNames[indent][0];
                            caseNames[indent].RemoveAt(0);
                            
                            if (command.setStringArg(caseName))
                                changesMade = true;
                        }
                    }
                }
            }
        }
        
        // Checks if importingStringArgs contains this field, and if so, imports it into the string variable.
        public static void importDatabaseString(int tabNum, int entry, string fieldName, ref string dataStr, int maxLength = -1)
        {
            fieldName = fieldName.ToLower();
            
            if (importingStringArgs != null)
            {
                if (importingStringArgs.ContainsKey(tabNum))
                {
                    if (importingStringArgs[tabNum].ContainsKey(entry))
                    {
                        if (importingStringArgs[tabNum][entry].ContainsKey(fieldName))
                        {
                            if (importingStringArgs[tabNum][entry][fieldName].Count > 0)
                            {
                                string newStr = importingStringArgs[tabNum][entry][fieldName][0];
                                string oldStr = dataStr;
                                
                                if (maxLength != -1)
                                    newStr = limitLength(newStr, maxLength);
                                
                                dataStr = newStr;
                                if (!newStr.Equals(oldStr))
                                    changesMade = true;
                            }
                        }
                    }
                }
            }
        }
        
        // After loading a command list, goes through them and sets trueChoices for Show Choice commands, based on their case names.
        public static void updateTrueChoices(List<Command> commands)
        {
            Dictionary<int, List<string>> backwardsCases = new Dictionary<int, List<string>>();
            
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                Command c = commands[i];
                int indent = c.getIndent();
                
                if (c.isEndCase()) // End Case signifies where to start a new list of cases between here and the next Show Choice
                {
                    if (!backwardsCases.ContainsKey(indent))
                        backwardsCases[indent] = new List<string>();
                    else
                        backwardsCases[indent].Clear();
                }
                else if (c.isChoiceCaseNonCancel()) // Add case names to the list in reverse order
                {
                    if (!backwardsCases.ContainsKey(indent))
                        backwardsCases[indent] = new List<string>();
                    backwardsCases[indent].Add(c.getStringArg());
                }
                else if (c.isChoice()) // When a Show Choice is hit, add the accumulated choices on this indent (after reversing)
                {
                    if (!backwardsCases.ContainsKey(indent)) // If somehow no list was made, fall back on splitting by /
                        c.setTrueChoices(c.getStringArg().Split('/'));
                    else
                    {
                        backwardsCases[indent].Reverse();
                        c.setTrueChoices(backwardsCases[indent].ToArray());
                        backwardsCases[indent].Clear();
                    }
                }
            }
        }
        
        // Limits length of string, considering two-byte characters in "to translate" write encoding to be two "units."
        public static string limitLength(string str, int length)
        {
            if (ignoreLengthLimits >= 1)
                return str;
            
            string origStr = str;
            
            if (str.Length > length)
                str = str.Substring(0, length);
            
            do
            {
                int byteLength = stringByteLength(str);
                if (byteLength > length)
                    str = str.Substring(0, str.Length - 1);
                else
                    break;
            } while (true);
            
            if (!str.Equals(origStr))
                Console.WriteLine("Truncated to " + length + ": " + str);
            
            return str;
        }
        
        // Gets byte length of string (using "to translate" write encoding), relevant for character limits.
        public static int stringByteLength(string str)
        {
            return writeEncodings[S_TOTRANSLATE].GetBytes(str).Length;
        }
        
        // Wraps overflowing lines in a message according to word wrap settings.
        public static string wordWrap(string str)
        {
            if (userSettings["WordWrap"] == 0)
                return str;
            
            bool fullBox = userSettings["WrapLineLimits"] == 2 || (userSettings["WrapLineLimits"] == 1 && !messageFaceOn);
            bool faceBox = userSettings["WrapLineLimits"] == 3 || (userSettings["WrapLineLimits"] == 1 && messageFaceOn);
            bool wrapImmediately = userSettings["WrapStyle"] == 2;
            
            int limit = fullBox? 50 : 38;
            string[] lines = str.Split('\n');
            List<string> newLines = new List<string>();
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                
                if (stringByteLength(line) > limit)
                {
                    // In immediate mode, wrap no later than the last possible character.
                    // In even mode, wrap no later than the average of the total character length.
                    int breakCount = 1, maxBreak = 0;
                    if (wrapImmediately)
                        maxBreak = limit;
                    else
                    {
                        maxBreak = line.Length / 2;
                        if (maxBreak > limit)
                        {
                            maxBreak = line.Length / 3;
                            breakCount = 2;
                        }
                        if (maxBreak > limit)
                        {
                            maxBreak = line.Length / 4;
                            breakCount = 3;
                        }
                    }
                    
                    do
                    {
                        for (int j = maxBreak; j > 0; j--)
                        {
                            if (j >= line.Length)
                                continue;
                            
                            if (line[j] == ' ')
                            {
                                string firstLine = line.Substring(0, j);
                                newLines.Add(firstLine);
                                line = line.Substring(j + 1);
                                
                                // In immediate mode, force another loop if length still goes over.
                                if (wrapImmediately && stringByteLength(line) > limit)
                                    breakCount = 2;
                                break;
                            }
                        }
                        breakCount--;
                    } while (breakCount > 0);
                }
                
                // Add either the line that was already under limit, or the remaining broken-off line.
                newLines.Add(line);
            }
            
            str = "";
            for (int i = 0; i < newLines.Count; i++)
                str += newLines[i] + (i < newLines.Count - 1? "\n" : "");
            
            return str;
        }
        
        // Adds a message to the log.
        public static void logMessage(string str, bool noDuplicates = false)
        {
            // Don't write log for Extracting, except when doing project comparison.
            if (globalMode == "Extracting" && !comparisonMode)
                return;
            
            if (!noDuplicates || !logStrings.Contains(str))
            {
                if (showLogMessages)
                    Console.WriteLine(str);
                logStrings.Add(str);
            }
        }
        
        // Prompts whether to save a log file.
        public static void logSave()
        {
            if (logStrings.Count > 0)
            {
                logExists = true;
                string logFile = userSettingsStr["LogFilename"];
                
                if (globalMode == "Rewriting")
                    Console.WriteLine("Translations were not found for some non-ASCII filenames.\nWrite missing translations to " + logFile + ".txt? (Y/N)");
                else if (globalMode == "Checking")
                {
                    string checkContents = "";
                    if (checkIssueReference)
                        checkContents += "- References to non-existent files\n";
                    if (checkIssueUnusedFile)
                        checkContents += "- Unused files in resource folders\n";
                    if (checkIssueUnusedDatabase)
                        checkContents += "- Never-referenced database entries\n";
                    if (checkIssueMessage)
                        checkContents += "- Messages with non-ASCII characters\n";
                    if (checkIssueOverflow)
                        checkContents += "- Message overflow\n";
                    Console.WriteLine("Issues found.\n" + checkContents + "Write issues to " + logFile + ".txt? (Y/N)");
                }
                else if (comparisonMode)
                {
                    if (!completenessMode)
                        Console.WriteLine("Found inconsistencies in string translations.\nWrite to " + logFile + ".txt? (Y/N)");
                    else
                        Console.WriteLine("Found unchanged messages.\nWrite to " + logFile + ".txt? (Y/N)");
                }
                
                if (yesNoPrompt())
                {
                    StringWriter str = new StringWriter(new StringBuilder());
                    foreach (string s in logStrings)
                        str.WriteLine(s);
                    writeToNewFile(logFile + ".txt", str.ToString());
                    Console.WriteLine("Log file written.");
                }
            }
            else
                logExists = false;
        }
        
        // Shows a message if debug mode is enabled.
        public static void debugMessage(string str)
        {
            if (debugMode)
                Console.WriteLine(str);
        }
    }
}
