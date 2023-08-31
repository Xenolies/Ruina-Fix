using System;
using System.Text;
using System.IO;

namespace RPGRewriter
{
    class Command : RPGByteData
    {
        int opcode = 0;
        int indent = 0;
        string stringArg = "";
        int[] args;
        string[] trueChoices;
        
        int moveRouteTarget;
        int moveRouteFreq;
        bool moveRouteRepeat;
        bool moveRouteSkip;
        MoveRoute moveRoute;
        
        #region // Command Codes //
        
        public const int C_BLANK = 10;
        public const int C_BATTLECALLEVENT = 1005;
        public const int C_FORCEFLEE = 1006;
        public const int C_ENABLECOMBO = 1007;
        public const int C_CHANGECLASS = 1008;
        public const int C_CHANGEBATTLECOMMANDS = 1009;
        public const int C_MANIACS3005 = 3005;
        public const int C_SHOWSTRINGPICTURE = 3007;
        public const int C_MANIACS3008 = 3008;
        public const int C_MANIACS3013 = 3013;
        public const int C_MANIACS3014 = 3014;
        public const int C_MANIACS3016 = 3016;
        public const int C_MANIACS3018 = 3018;
        public const int C_MANIACS3021 = 3021;
        public const int C_MANIACS3025 = 3025;
        public const int C_MANIACS3026 = 3026;
        public const int C_CALLLOAD = 5001;
        public const int C_EXITGAME = 5002;
        public const int C_TOGGLEATBMODE = 5003;
        public const int C_TOGGLEFULLSCREEN = 5004;
        public const int C_OPENVIDEOOPTIONS = 5005;
        public const int C_MESSAGE = 10110;
        public const int C_MESSAGEFOLLOW = 20110;
        public const int C_MESSAGEOPTIONS = 10120;
        public const int C_CHANGEFACE = 10130;
        public const int C_CHOICE = 10140;
        public const int C_CHOICECASE = 20140;
        public const int C_ENDCASE = 20141;
        public const int C_INPUTNUMBER = 10150;
        public const int C_SWITCH = 10210;
        public const int C_VARIABLE = 10220;
        public const int C_TIMER = 10230;
        public const int C_CHANGEMONEY = 10310;
        public const int C_CHANGEITEMS = 10320;
        public const int C_CHANGEPARTY = 10330;
        public const int C_CHANGEEXP = 10410;
        public const int C_CHANGELEVEL = 10420;
        public const int C_CHANGESTATS = 10430;
        public const int C_CHANGESKILLS = 10440;
        public const int C_CHANGEEQUIP = 10450;
        public const int C_CHANGEHP = 10460;
        public const int C_CHANGEMP = 10470;
        public const int C_CHANGESTATE = 10480;
        public const int C_FULLRECOVER = 10490;
        public const int C_DAMAGE = 10500;
        public const int C_NAMECHANGE = 10610;
        public const int C_TITLECHANGE = 10620;
        public const int C_CHARSETCHANGE = 10630;
        public const int C_FACESETCHANGE = 10640;
        public const int C_VEHICLEGRAPHIC = 10650;
        public const int C_SYSTEMBGMCHANGE = 10660;
        public const int C_SYSTEMSOUNDCHANGE = 10670;
        public const int C_SYSTEMGRAPHICSCHANGE = 10680;
        public const int C_TRANSITIONCHANGE = 10690;
        public const int C_BATTLESTART = 10710;
        public const int C_BATTLEWINCASE = 20710;
        public const int C_BATTLEFLEECASE = 20711;
        public const int C_BATTLELOSECASE = 20712;
        public const int C_BATTLEENDCASE = 20713;
        public const int C_OPENSHOP = 10720;
        public const int C_SHOPSALECASE = 20720;
        public const int C_SHOPCANCELCASE = 20721;
        public const int C_SHOPENDCASE = 20722;
        public const int C_CALLINN = 10730;
        public const int C_INNRESTCASE = 20730;
        public const int C_INNCANCELCASE = 20731;
        public const int C_INNENDCASE = 20732;
        public const int C_NAMEENTRY = 10740;
        public const int C_TELEPORT = 10810;
        public const int C_REMEMBERPLACE = 10820;
        public const int C_RESTOREPLACE = 10830;
        public const int C_RIDEVEHICLE = 10840;
        public const int C_PLACEVEHICLE = 10850;
        public const int C_PLACEEVENT = 10860;
        public const int C_SWAPEVENTS = 10870;
        public const int C_GETTERRAIN = 10910;
        public const int C_GETEVENTID = 10920;
        public const int C_ERASESCREEN = 11010;
        public const int C_SHOWSCREEN = 11020;
        public const int C_SCREENTONE = 11030;
        public const int C_SCREENFLASH = 11040;
        public const int C_SCREENSHAKE = 11050;
        public const int C_PANSCREEN = 11060;
        public const int C_WEATHER = 11070;
        public const int C_PICTURE = 11110;
        public const int C_MOVEPICTURE = 11120;
        public const int C_ERASEPICTURE = 11130;
        public const int C_SHOWANIMATION = 11210;
        public const int C_SETOPACITY = 11310;
        public const int C_FLASHEVENT = 11320;
        public const int C_MOVEEVENT = 11330;
        public const int C_MOVEALL = 11340;
        public const int C_STOPALL = 11350;
        public const int C_WAIT = 11410;
        public const int C_MUSIC = 11510;
        public const int C_FADEMUSIC = 11520;
        public const int C_REMEMBERMUSIC = 11530;
        public const int C_RESTOREMUSIC = 11540;
        public const int C_SOUND = 11550;
        public const int C_MOVIE = 11560;
        public const int C_KEYINPUT = 11610;
        public const int C_CHANGECHIPSET = 11710;
        public const int C_CHANGEPARALLAX = 11720;
        public const int C_CHANGEENCOUNTER = 11740;
        public const int C_CHANGECHIP = 11750;
        public const int C_SETTELEPORT = 11810;
        public const int C_DISABLETELEPORT = 11820;
        public const int C_SETESCAPE = 11830;
        public const int C_DISABLEESCAPE = 11840;
        public const int C_CALLSAVE = 11910;
        public const int C_DISABLESAVE = 11930;
        public const int C_CALLMENU = 11950;
        public const int C_DISABLEMENU = 11960;
        public const int C_FORK = 12010;
        public const int C_ELSE = 22010;
        public const int C_FORKEND = 22011;
        public const int C_LABEL = 12110;
        public const int C_LABELJUMP = 12120;
        public const int C_CYCLE = 12210;
        public const int C_CYCLEBREAK = 12220;
        public const int C_CYCLEEND = 22210;
        public const int C_STOPPARALLEL = 12310;
        public const int C_ERASEEVENT = 12320;
        public const int C_CALLEVENT = 12330;
        public const int C_COMMENT = 12410;
        public const int C_COMMENTFOLLOW = 22410;
        public const int C_GAMEOVER = 12420;
        public const int C_TITLE = 12510;
        public const int C_CHANGEENEMYHP = 13110;
        public const int C_CHANGEENEMYMP = 13120;
        public const int C_CHANGEENEMYSTATE = 13130;
        public const int C_ENEMYAPPEAR = 13150;
        public const int C_CHANGEBATTLEBG = 13210;
        public const int C_BATTLEANIMATION = 13260;
        public const int C_BATTLEFORK = 13310;
        public const int C_BATTLEELSE = 23310;
        public const int C_BATTLEFORKEND = 23311;
        public const int C_STOPBATTLE = 13410;
        
        public const int DUMMYMESSAGEARG = 1374;
        #endregion
        
        #region // String Constants //
        
        static string[] operations = { "=", "+=", "-=", "*=", "/=", "%=" };
        static string[] comparisons = { "==", ">=", "<=", ">", "<", "!=" };
        static string[] messagePositions = { "Top", "Middle", "Bottom" };
        static string[] basicStats = { "Max HP", "Max MP", "Attack", "Defense", "Mind", "Agility" };
        static string[] enemyStats = { "HP", "MP", "Max HP", "Max MP", "Attack", "Defense", "Mind", "Agility" };
        static string[] heroQualities = { "Level", "EXP", "HP", "MP", "Max HP", "Max MP", "Attack", "Defense", "Mind", "Agility",
                                          "Weapon #", "Shield #", "Armor #", "Helment #", "Accessory #" };
        static string[] eventQualities = { "Map ID", "X Coord.", "Y Coord.", "Direction", "Screen X", "Screen Y" };
        static string[] miscData = { "Money", "Timer Seconds", "Party Size", "Saves",
                                     "Battles", "Victories", "Defeats", "Escapes", "MIDI Ticks", "Timer 2 Seconds" };
        static string[] equipTypes = { "Arms", "Shield", "Armor", "Helm", "Other", "All" };
        static string[] systemBGMs = { "Battle", "Battle End", "Inn", "Boat", "Ship", "Airship", "Game Over" };
        static string[] systemSounds = { "Cursor Move", "Confirm", "Cancel", "Buzz", "Battle Start", "Escape",
                                         "Enemy Attack", "Damage Enemy", "Damage Ally", "Evade", "Enemy Die", "Use Item" };
        static string[] transitions = { "Teleport Erase", "Teleport Show", "Battle Start Erase", "Battle Start Show",
                                        "Battle End Erase", "Battle End Show" };
        static string[] specialBattleTypes = { "Normal", "Initiative", "Surrounded", "Back Attack", "Pincer Attack" };
        static string[] weathers = { "None", "Rain", "Snow", "Fog", "Sandstorm" };
        static string[] classChangeLevel = { "Retain Level", "Become Level 1" };
        static string[] classChangeSkills = { "Retain Skills", "Add New Skills Deleting Old", "Add New Skills Keeping Old" };
        static string[] classChangeStats = { "Retain Stats", "Halve Stats", "Set Stats to Level 1", "Set Stats Per New Class"};
        #endregion
        
        public Command(FileStream f)
        {
            load(f);
        }
        public Command(int opcode, int indent, int[] args, string stringArg = "")
        {
            this.opcode = opcode;
            this.indent = indent;
            this.args = args;
            this.stringArg = stringArg;
        }
        public Command()
        {
        }
        
        // Loads a single command within a page.
        override public void load(FileStream f)
        {
            opcode = M.readMultibyte(f);
            indent = M.readMultibyte(f);
            
            int mode = getMode();
            
            if (opcode != C_MOVEEVENT)
            {
                int strType = mode != -1 && mode < M.FOLDERCOUNT? M.S_FILENAME : M.S_TOTRANSLATE;
                stringArg = mode != -1? M.readStringAndRewrite(f, mode, strType) : M.readString(f, strType);
                int argCount = M.readMultibyte(f);
                args = new int[argCount];
                for (int i = 0; i < argCount; i++)
                    args[i] = M.readMultibyte(f);
            }
            else
            {
                M.byteCheck(f, 0x00);
                
                int moveLength = M.readMultibyte(f);
                int lengthTemp = moveLength;
                
                moveRouteTarget = M.readMultibyte(f);
                moveRouteFreq = M.readByte(f);
                moveRouteRepeat = (M.readByte(f) == 1);
                moveRouteSkip = (M.readByte(f) == 1);
                lengthTemp -= 4;
                
                moveRoute = new MoveRoute(f, lengthTemp, "Move");
            }
            
            // After reading data, handle various tasks for special modes.
            M.messagePreceded = opcode == C_MESSAGE || opcode == C_MESSAGEFOLLOW;
            
            updateMessageFaceOn();
            
            if (M.comparisonMode || M.extractDoubleMode || M.compilingUnique || M.compilingDuplicates)
            {
                if (isMessageStart()) // Starting new message
                {
                    if (M.completeMessage == null) // First message in page: just start new string for it
                        M.completeMessage = new StringWriter(new StringBuilder());
                    else if (M.completeMessage.ToString() != "") // Existing message: add that first, then start new
                    {
                        M.addMessageForSpecialFunctions(M.completeMessage.ToString());
                        M.completeMessage = new StringWriter(new StringBuilder());
                    }
                    M.completeMessage.WriteLine(stringArg); // Write this line to new message string
                }
                else if (isMessageFollowOrExtraBox()) // Continuing message, add to current string
                    M.completeMessage.WriteLine(stringArg);
                else if (M.completeMessage != null && M.completeMessage.ToString() != "") // Non-message command, add fully-assembled message
                {
                    M.addMessageForSpecialFunctions(M.completeMessage.ToString());
                    M.completeMessage = new StringWriter(new StringBuilder());
                }
                
                if (opcode == C_CHOICE || opcode == C_NAMECHANGE || opcode == C_SHOWSTRINGPICTURE)
                    M.addMessageForSpecialFunctions(stringArg + Environment.NewLine);
            }
            else if (M.copyingCommandValues)
            {
                if (opcode == C_WAIT || opcode == C_SCREENTONE || opcode == C_SCREENFLASH || opcode == C_SCREENSHAKE || opcode == C_PANSCREEN || opcode == C_FADEMUSIC)
                    M.copyPasteCommandValues(opcode, ref args);
            }
            else
            {
                if (opcode == C_MESSAGE || opcode == C_MESSAGEFOLLOW)
                {
                    if (M.globalMode == "Checking" && M.checkLineLength)
                    {
                        string testStr = M.removeEscapeCodes(stringArg);
                        if ((!M.messageFaceOn && testStr.Length > 50)
                         || (M.messageFaceOn && testStr.Length > 38))
                        {
                            M.logMessage("[" + M.currentPosition() + "] Over line limit: " + stringArg);
                            M.checkIssueOverflow = true;
                        }
                    }
                }
            }
        }
        
        // Default version.
        override public string getString()
        {
            return getString(false, false, false);
        }
        
        // Returns command string.
        public string getString(bool lastLineOfMessage = false, bool includeIndent = false, bool youAreHereMarker = false)
        {
            if (M.userSettings["CommandIndents"] == 1)
                includeIndent = true;
            
            string marker = "", indentStr = "";
            if (includeIndent)
                for (int i = 0; i < indent; i++)
                    indentStr += "\t";
            if (youAreHereMarker)
                marker = "[>>] ";
            
            if (M.checkUnusedData && (opcode == C_MESSAGE || opcode == C_MESSAGEFOLLOW)) // \Ns in messages count as references to hero database
            {
                if (stringArg.ToLower().Contains("\\n["))
                {
                    int index = stringArg.ToLower().IndexOf("\\n[");
                    int rightBracket = stringArg.IndexOf(']', index + 3);
                    string contents = stringArg.Substring(index + 3, rightBracket - index - 3);
                    int entry;
                    if (int.TryParse(contents, out entry))
                        M.unusedDataEntries["Heroes"].Remove(entry);
                }
            }
            
            if (!M.includeMessages && M.blankOutMessages && isMessageStart()) // If blanking is on, return blank for (start of) each message
                return indentStr + marker + "___";
            
            if (!M.includeMessages // If omitting messages, omit messages and comments
             && (opcode == C_MESSAGE || opcode == C_MESSAGEFOLLOW
              || opcode == C_COMMENT || opcode == C_COMMENTFOLLOW))
                return "";
            
            if (!M.includeActions // If omitting actions, omit non-messages/choices
             && !M.ibMode && !M.stringScriptExportMode && !M.checkUnusedData // All of these modes have reason not to skip actions
             && opcode != C_MESSAGE && opcode != C_MESSAGEFOLLOW
             && opcode != C_CHOICE && opcode != C_CHOICECASE
             && opcode != C_SHOWSTRINGPICTURE)
                return "";
            
            string result = "";
            
            if (opcode == C_BLANK)
                result = command10Blank();
            else if (opcode == C_BATTLECALLEVENT)
                result = command1005BattleCallEvent();
            else if (opcode == C_FORCEFLEE)
                result = command1006ForceFlee();
            else if (opcode == C_ENABLECOMBO)
                result = command1007EnableCombo();
            else if (opcode == C_CHANGECLASS)
                result = command1008ChangeClass();
            else if (opcode == C_CHANGEBATTLECOMMANDS)
                result = command1009ChangeBattleCommands();
            else if (opcode == C_MANIACS3005)
                result = command3005ManiacsUnknown();
            else if (opcode == C_SHOWSTRINGPICTURE)
                result = command3007ShowStringPicture();
            else if (opcode == C_MANIACS3008)
                result = command3008ManiacsUnknown();
            else if (opcode == C_MANIACS3013)
                result = command3013ManiacsUnknown();
            else if (opcode == C_MANIACS3014)
                result = command3014ManiacsUnknown();
            else if (opcode == C_MANIACS3016)
                result = command3016ManiacsUnknown();
            else if (opcode == C_MANIACS3018)
                result = command3018ManiacsUnknown();
            else if (opcode == C_MANIACS3021)
                result = command3021ManiacsUnknown();
            else if (opcode == C_MANIACS3025)
                result = command3025ManiacsUnknown();
            else if (opcode == C_MANIACS3026)
                result = command3026ManiacsUnknown();
            else if (opcode == C_CALLLOAD)
                result = command5001CallLoad();
            else if (opcode == C_EXITGAME)
                result = command5002ExitGame();
            else if (opcode == C_TOGGLEATBMODE)
                result = command5003ToggleATBMode();
            else if (opcode == C_TOGGLEFULLSCREEN)
                result = command5004ToggleFullscreen();
            else if (opcode == C_OPENVIDEOOPTIONS)
                result = command5005OpenVideoOptions();
            else if (opcode == C_MESSAGE)
                result = command10110Message();
            else if (opcode == C_MESSAGEFOLLOW)
                result = command20110MessageFollow();
            else if (opcode == C_MESSAGEOPTIONS)
                result = command10120MessageOptions();
            else if (opcode == C_CHANGEFACE)
                result = command10130ChangeFace();
            else if (opcode == C_CHOICE)
                result = command10140Choice();
            else if (opcode == C_CHOICECASE)
                result = command20140ChoiceCase();
            else if (opcode == C_ENDCASE)
                result = command20141EndCase();
            else if (opcode == C_INPUTNUMBER)
                result = command10150InputNumber();
            else if (opcode == C_SWITCH)
                result = command10210Switch();
            else if (opcode == C_VARIABLE)
                result = command10220Variable();
            else if (opcode == C_TIMER)
                result = command10230Timer();
            else if (opcode == C_CHANGEMONEY)
                result = command10310ChangeMoney();
            else if (opcode == C_CHANGEITEMS)
                result = command10320ChangeItems();
            else if (opcode == C_CHANGEPARTY)
                result = command10330ChangeParty();
            else if (opcode == C_CHANGEEXP)
                result = command10410ChangeEXP();
            else if (opcode == C_CHANGELEVEL)
                result = command10420ChangeLevel();
            else if (opcode == C_CHANGESTATS)
                result = command10430ChangeStats();
            else if (opcode == C_CHANGESKILLS)
                result = command10440ChangeSkills();
            else if (opcode == C_CHANGEEQUIP)
                result = command10450ChangeEquip();
            else if (opcode == C_CHANGEHP)
                result = command10460ChangeHP();
            else if (opcode == C_CHANGEMP)
                result = command10470ChangeMP();
            else if (opcode == C_CHANGESTATE)
                result = command10480ChangeState();
            else if (opcode == C_FULLRECOVER)
                result = command10490FullRecover();
            else if (opcode == C_DAMAGE)
                result = command10500Damage();
            else if (opcode == C_NAMECHANGE)
                result = command10610NameChange();
            else if (opcode == C_TITLECHANGE)
                result = command10620TitleChange();
            else if (opcode == C_CHARSETCHANGE)
                result = command10630CharSetChange();
            else if (opcode == C_FACESETCHANGE)
                result = command10640FaceSetChange();
            else if (opcode == C_VEHICLEGRAPHIC)
                result = command10650VehicleGraphic();
            else if (opcode == C_SYSTEMBGMCHANGE)
                result = command10660SystemBGMChange();
            else if (opcode == C_SYSTEMSOUNDCHANGE)
                result = command10670SystemSoundChange();
            else if (opcode == C_SYSTEMGRAPHICSCHANGE)
                result = command10680SystemGraphicsChange();
            else if (opcode == C_TRANSITIONCHANGE)
                result = command10690TransitionChange();
            else if (opcode == C_BATTLESTART)
                result = command10710BattleStart();
            else if (opcode == C_BATTLEWINCASE)
                result = command20710BattleWinCase();
            else if (opcode == C_BATTLEFLEECASE)
                result = command20711BattleFleeCase();
            else if (opcode == C_BATTLELOSECASE)
                result = command20712BattleLoseCase();
            else if (opcode == C_BATTLEENDCASE)
                result = command20713BattleEndCase();
            else if (opcode == C_OPENSHOP)
                result = command10720OpenShop();
            else if (opcode == C_SHOPSALECASE)
                result = command20720ShopSaleCase();
            else if (opcode == C_SHOPCANCELCASE)
                result = command20721ShopCancelCase();
            else if (opcode == C_SHOPENDCASE)
                result = command20722ShopEndCase();
            else if (opcode == C_CALLINN)
                result = command10730CallInn();
            else if (opcode == C_INNRESTCASE)
                result = command20730InnRestCase();
            else if (opcode == C_INNCANCELCASE)
                result = command20731InnCancelCase();
            else if (opcode == C_INNENDCASE)
                result = command20732InnEndCase();
            else if (opcode == C_NAMEENTRY)
                result = command10740NameEntry();
            else if (opcode == C_TELEPORT)
                result = command10810Teleport();
            else if (opcode == C_REMEMBERPLACE)
                result = command10820RememberPlace();
            else if (opcode == C_RESTOREPLACE)
                result = command10830RestorePlace();
            else if (opcode == C_RIDEVEHICLE)
                result = command10840RideVehicle();
            else if (opcode == C_PLACEVEHICLE)
                result = command10850PlaceVehicle();
            else if (opcode == C_PLACEEVENT)
                result = command10860PlaceEvent();
            else if (opcode == C_SWAPEVENTS)
                result = command10870SwapEvents();
            else if (opcode == C_GETTERRAIN)
                result = command10910GetTerrainID();
            else if (opcode == C_GETEVENTID)
                result = command10920GetEventID();
            else if (opcode == C_ERASESCREEN)
                result = command11010EraseScreen();
            else if (opcode == C_SHOWSCREEN)
                result = command11020ShowScreen();
            else if (opcode == C_SCREENTONE)
                result = command11030ScreenTone();
            else if (opcode == C_SCREENFLASH)
                result = command11040ScreenFlash();
            else if (opcode == C_SCREENSHAKE)
                result = command11050ScreenShake();
            else if (opcode == C_PANSCREEN)
                result = command11060PanScreen();
            else if (opcode == C_WEATHER)
                result = command11070Weather();
            else if (opcode == C_PICTURE)
                result = command11110Picture();
            else if (opcode == C_MOVEPICTURE)
                result = command11120MovePicture();
            else if (opcode == C_ERASEPICTURE)
                result = command11130ErasePicture();
            else if (opcode == C_SHOWANIMATION)
                result = command11210ShowAnimation();
            else if (opcode == C_SETOPACITY)
                result = command11310SetOpacity();
            else if (opcode == C_FLASHEVENT)
                result = command11320FlashEvent();
            else if (opcode == C_MOVEEVENT)
                result = command11330MoveEvent();
            else if (opcode == C_MOVEALL)
                result = command11340MoveAll();
            else if (opcode == C_STOPALL)
                result = command11350StopAll();
            else if (opcode == C_WAIT)
                result = command11410Wait();
            else if (opcode == C_MUSIC)
                result = command11510Music();
            else if (opcode == C_FADEMUSIC)
                result = command11520FadeMusic();
            else if (opcode == C_REMEMBERMUSIC)
                result = command11530RememberMusic();
            else if (opcode == C_RESTOREMUSIC)
                result = command11540RestoreMusic();
            else if (opcode == C_SOUND)
                result = command11550Sound();
            else if (opcode == C_MOVIE)
                result = command11560Movie();
            else if (opcode == C_KEYINPUT)
                result = command11610KeyInput();
            else if (opcode == C_CHANGECHIPSET)
                result = command11710ChangeChipSet();
            else if (opcode == C_CHANGEPARALLAX)
                result = command11720ChangeParallax();
            else if (opcode == C_CHANGEENCOUNTER)
                result = command11740ChangeEncounter();
            else if (opcode == C_CHANGECHIP)
                result = command11750ChangeChip();
            else if (opcode == C_SETTELEPORT)
                result = command11810SetTeleport();
            else if (opcode == C_DISABLETELEPORT)
                result = command11820DisableTeleport();
            else if (opcode == C_SETESCAPE)
                result = command11830SetEscape();
            else if (opcode == C_DISABLEESCAPE)
                result = command11840DisableEscape();
            else if (opcode == C_CALLSAVE)
                result = command11910CallSave();
            else if (opcode == C_DISABLESAVE)
                result = command11930DisableSave();
            else if (opcode == C_CALLMENU)
                result = command11950CallMenu();
            else if (opcode == C_DISABLEMENU)
                result = command11960DisableMenu();
            else if (opcode == C_FORK)
                result = command12010Fork();
            else if (opcode == C_ELSE)
                result = command22010Else();
            else if (opcode == C_FORKEND)
                result = command22011ForkEnd();
            else if (opcode == C_LABEL)
                result = command12110Label();
            else if (opcode == C_LABELJUMP)
                result = command12120LabelJump();
            else if (opcode == C_CYCLE)
                result = command12210Cycle();
            else if (opcode == C_CYCLEBREAK)
                result = command12220CycleBreak();
            else if (opcode == C_CYCLEEND)
                result = command22210CycleEnd();
            else if (opcode == C_STOPPARALLEL)
                result = command12310StopParallel();
            else if (opcode == C_ERASEEVENT)
                result = command12320EraseEvent();
            else if (opcode == C_CALLEVENT)
                result = command12330CallEvent();
            else if (opcode == C_COMMENT)
                result = command12410Comment();
            else if (opcode == C_COMMENTFOLLOW)
                result = command22410CommentFollow();
            else if (opcode == C_GAMEOVER)
                result = command12420GameOver();
            else if (opcode == C_TITLE)
                result = command12510Title();
            else if (opcode == C_CHANGEENEMYHP)
                result = command13110ChangeEnemyHP();
            else if (opcode == C_CHANGEENEMYMP)
                result = command13120ChangeEnemyMP();
            else if (opcode == C_CHANGEENEMYSTATE)
                result = command13130ChangeEnemyState();
            else if (opcode == C_ENEMYAPPEAR)
                result = command13150EnemyAppear();
            else if (opcode == C_CHANGEBATTLEBG)
                result = command13210ChangeBattleBG();
            else if (opcode == C_BATTLEANIMATION)
                result = command13260BattleAnimation();
            else if (opcode == C_BATTLEFORK)
                result = command13310BattleFork();
            else if (opcode == C_BATTLEELSE)
                result = command23310BattleElse();
            else if (opcode == C_BATTLEFORKEND)
                result = command23311BattleForkEnd();
            else if (opcode == C_STOPBATTLE)
                result = command13410StopBattle();
            else if (opcode != 0)
            {
                Console.WriteLine("UNSUPPORTED OPCODE " + opcode + " " + M.currentPosition());
                Console.WriteLine(stringArg);
                for (int i = 0; i < args.Length; i++)
                    Console.WriteLine("args " + i + ": " + args[i]);
            }
            
            // When exporting to string script, leave out commands without strings unless some are desired, and put a header/terminator around strings.
            if (M.stringScriptExportMode)
            {
                string commandName = "";
                bool nameFork = isNameFork();
                if (opcode == C_MESSAGE || isRemovedMessage())
                    commandName = "Message";
                else if (opcode == C_CHOICE)
                    commandName = "Choice";
                else if (opcode == C_NAMECHANGE)
                    commandName = "NameChange";
                else if (opcode == C_TITLECHANGE)
                    commandName = "TitleChange";
                else if (opcode == C_SHOWSTRINGPICTURE)
                    commandName = "StringPicture";
                else if (nameFork)
                    commandName = "NameFork";
                
                string lb = Environment.NewLine;
                string lineNum = M.currentLine.Replace("Line ", "");
                string header = "#" + commandName + "#" + (M.getDetailSetting("LineNumber")? " (" + lineNum + ")" : "");
                string terminator = "##" + lb + lb;
                
                if (opcode == C_NAMECHANGE || opcode == C_TITLECHANGE)
                    header += " [Hero #" + args[0] + "]";
                if (nameFork)
                    header += " [Hero #" + args[1] + "]";
                
                header += lb;
                
                if (opcode == C_MESSAGE || opcode == C_MESSAGEFOLLOW || isRemovedMessage()) // Add to full message, write complete command if this is last line
                {
                    if (isMessageStart()) // If this is starting command, initialize message and store header for complete command
                    {
                        M.fullMessageHeader = header;
                        M.fullMessageContent = "";
                        if (isRemovedMessage())
                            M.fullMessageContent = "<<remove>>";
                    }
                    M.fullMessageContent += stringArg + lb;
                    
                    if (lastLineOfMessage)
                    {
                        if (M.getDetailSetting("OriginalCommandStrings") || M.generatingOriginalStringDB) // Call method regardless to add string to database
                        {
                            string messageLines = M.fullMessageContent.Substring(0, M.fullMessageContent.Length - lb.Length); // Remove last lb
                            messageLines = M.getOriginalString("Message", messageLines);
                            if (M.getDetailSetting("OriginalCommandStrings"))
                                M.fullMessageHeader = "//" + messageLines.Replace("\n", "\n//") + lb + M.fullMessageHeader;
                        }
                        
                        string str = M.fullMessageHeader + M.fullMessageContent + terminator;
                        M.fullMessageHeader = "";
                        M.fullMessageContent = "";
                        M.wroteStringInPage = true;
                        return str;
                    }
                    else
                        return "";
                }
                else if (opcode == C_CHOICE) // Write each choice on its own line
                {
                    M.wroteStringInPage = true;
                    string choices = "";
                    for (int i = 0; i < trueChoices.Length; i++)
                        choices += trueChoices[i] + (i < trueChoices.Length - 1? Environment.NewLine : "");
                    
                    if (M.getDetailSetting("OriginalCommandStrings"))
                        header = "//" + M.getOriginalString(commandName, choices).Replace("\n", "\n//") + lb + header;
                    else if (M.generatingOriginalStringDB) // Call method regardless to add string to database
                        M.getOriginalString(commandName, choices);
                    
                    return header + choices + lb + terminator;
                }
                else if (opcode == C_NAMECHANGE || opcode == C_TITLECHANGE || opcode == C_SHOWSTRINGPICTURE || nameFork) // Always just write string and terminator
                {
                    M.wroteStringInPage = true;
                    
                    string exportStringArg = getStringArg(); // Pre-processes stringArg for special commands with unwanted parts
                    
                    if (M.getDetailSetting("OriginalCommandStrings"))
                        header = "//" + M.getOriginalString(commandName, exportStringArg).Replace("\n", "\n//") + lb + header;
                    else if (M.generatingOriginalStringDB) // Call method regardless to add string to database
                        M.getOriginalString(commandName, exportStringArg);
                    
                    return header + exportStringArg + lb + terminator;
                }
                else if (M.userSettings["StringScriptDetails"] == 1) // Include other commands if user wants them
                {
                    string settingName = "";
                    string bracketType = "{";
                    switch (opcode)
                    {
                        case C_FORK:
                        case C_BATTLEFORK:
                            settingName = "CommandBranch"; break;
                        case C_CHOICECASE:
                        case C_ELSE:
                        case C_BATTLEELSE:
                        case C_BATTLEWINCASE:
                        case C_BATTLEFLEECASE:
                        case C_BATTLELOSECASE:
                        case C_SHOPSALECASE:
                        case C_SHOPCANCELCASE:
                        case C_INNRESTCASE:
                        case C_INNCANCELCASE:
                            settingName = "CommandBranchCases"; break;
                        case C_CHANGEFACE:
                            settingName = "CommandMessageFace"; break;
                        case C_TELEPORT:
                            settingName = "CommandTeleport"; break;
                        case C_CHANGEITEMS:
                            settingName = "CommandChangeItems"; break;
                        case C_SWITCH:
                            settingName = "CommandChangeSwitch"; bracketType = "["; break;
                        case C_VARIABLE:
                            settingName = "CommandChangeVariable"; bracketType = "["; break;
                        case C_CALLEVENT:
                        case C_BATTLECALLEVENT:
                            settingName = "CommandCallEvent"; break;
                        case C_PICTURE:
                            settingName = "CommandShowPicture"; break;
                        case C_MOVEPICTURE:
                            settingName = "CommandMovePicture"; break;
                        case C_ERASEPICTURE:
                            settingName = "CommandErasePicture"; break;
                        case C_MUSIC:
                            settingName = "CommandPlayMusic"; break;
                        case C_FADEMUSIC:
                        case C_REMEMBERMUSIC:
                        case C_RESTOREMUSIC:
                            settingName = "CommandMusicControls"; break;
                        case C_SOUND:
                            settingName = "CommandSound"; break;
                        case C_MOVIE:
                            settingName = "CommandMovie"; break;
                        case C_WAIT:
                            settingName = "CommandWait"; break;
                        case C_MOVEEVENT:
                            settingName = "CommandMoveEvent"; break;
                        case C_MOVEALL:
                        case C_STOPALL:
                            settingName = "CommandMoveEventControls"; break;
                        case C_GETTERRAIN:
                        case C_GETEVENTID:
                            settingName = "CommandGetTileTerrainOrEventID"; break;
                        case C_MESSAGEOPTIONS:
                            settingName = "CommandMessageOptions"; break;
                        case C_SHOWANIMATION:
                        case C_BATTLEANIMATION:
                            settingName = "CommandShowAnimation"; break;
                        case C_ERASESCREEN:
                        case C_SHOWSCREEN:
                        case C_SCREENTONE:
                        case C_SCREENFLASH:
                        case C_SCREENSHAKE:
                        case C_PANSCREEN:
                        case C_WEATHER:
                            settingName = "CommandScreenEffects"; break;
                        case C_TRANSITIONCHANGE:
                            settingName = "CommandSetScreenTransition"; break;
                        case C_SETOPACITY:
                        case C_FLASHEVENT:
                            settingName = "CommandEventEffects"; break;
                        case C_FULLRECOVER:
                            settingName = "CommandFullRecovery"; break;
                        case C_DAMAGE:
                            settingName = "CommandDamage"; break;
                        case C_CHANGEMONEY:
                            settingName = "CommandChangeMoney"; break;
                        case C_CHANGEPARTY:
                        case C_CHANGEEXP:
                        case C_CHANGELEVEL:
                        case C_CHANGESTATS:
                        case C_CHANGESKILLS:
                        case C_CHANGEEQUIP:
                        case C_CHANGEHP:
                        case C_CHANGEMP:
                        case C_CHANGESTATE:
                        case C_CHANGECLASS:
                        case C_CHANGEBATTLECOMMANDS:
                            settingName = "CommandChangePartyParams"; break;
                        case C_CHARSETCHANGE:
                        case C_FACESETCHANGE:
                            settingName = "CommandChangePartyGraphics"; break;
                        case C_VEHICLEGRAPHIC:
                        case C_SYSTEMBGMCHANGE:
                        case C_SYSTEMSOUNDCHANGE:
                        case C_SYSTEMGRAPHICSCHANGE:
                            settingName = "CommandChangeVariousSystemFiles"; break;
                        case C_CHANGEPARALLAX:
                        case C_CHANGEENCOUNTER:
                            settingName = "CommandChangeMapSettings"; break;
                        case C_CHANGECHIP:
                            settingName = "CommandChangeMapTile"; break;
                        case C_INPUTNUMBER:
                            settingName = "CommandInputNumber"; break;
                        case C_KEYINPUT:
                            settingName = "CommandKeyInput"; break;
                        case C_TIMER:
                            settingName = "CommandTimer"; break;
                        case C_NAMEENTRY:
                            settingName = "CommandCallNameEntry"; break;
                        case C_CALLSAVE:
                        case C_CALLLOAD:
                        case C_CALLMENU:
                            settingName = "CommandCallMenus"; break;
                        case C_OPENSHOP:
                        case C_CALLINN:
                            settingName = "CommandOpenShopOrInn"; break;
                        case C_BATTLESTART:
                            settingName = "CommandBattleStart"; break;
                        case C_CHANGEENEMYHP:
                        case C_CHANGEENEMYMP:
                        case C_CHANGEENEMYSTATE:
                        case C_ENEMYAPPEAR:
                        case C_FORCEFLEE:
                        case C_ENABLECOMBO:
                        case C_CHANGEBATTLEBG:
                        case C_STOPBATTLE:
                            settingName = "CommandMidBattleControls"; break;
                        case C_TITLE:
                        case C_GAMEOVER:
                        case C_EXITGAME:
                            settingName = "CommandEndOrExitGame"; break;
                        case C_OPENVIDEOOPTIONS:
                            settingName = "CommandOpenVideoOptions"; break;
                        case C_CYCLE:
                        case C_CYCLEBREAK:
                            settingName = "CommandLoop"; break;
                        case C_STOPPARALLEL:
                            settingName = "CommandStopParallelEvents"; break;
                        case C_ERASEEVENT:
                            settingName = "CommandEraseEvent"; break;
                        case C_PLACEEVENT:
                        case C_SWAPEVENTS:
                            settingName = "CommandPlaceEventOrSwapEvents"; break;
                        case C_RIDEVEHICLE:
                        case C_PLACEVEHICLE:
                            settingName = "CommandVehicleControls"; break;
                        case C_REMEMBERPLACE:
                        case C_RESTOREPLACE:
                            settingName = "CommandRememberRestorePlace"; break;
                        case C_DISABLETELEPORT:
                        case C_DISABLEESCAPE:
                        case C_DISABLESAVE:
                        case C_DISABLEMENU:
                            settingName = "CommandEnableDisableFunctions"; break;
                        case C_SETTELEPORT:
                        case C_SETESCAPE:
                            settingName = "CommandSetTeleportOrEscapeDestination"; break;
                        case C_TOGGLEATBMODE:
                            settingName = "CommandToggleATBMode"; break;
                        case C_TOGGLEFULLSCREEN:
                            settingName = "CommandToggleFullscreen"; break;
                        case C_COMMENT:
                        case C_COMMENTFOLLOW:
                            settingName = "CommandComment"; break;
                        case C_LABEL:
                        case C_LABELJUMP:
                            settingName = "CommandLabel"; break;
                    }
                    
                    if (settingName != "" && M.getDetailSetting(settingName))
                    {
                        if (bracketType == "{")
                            return "{{{{{{{{{{{{{{{{{{{{ " + result + " }}}}}}}}}}}}}}}}}}}}" + Environment.NewLine;
                        else
                            return "[[[[[[[[[[[[[[[[[[[[ " + result + " ]]]]]]]]]]]]]]]]]]]]" + Environment.NewLine;
                    }
                }
                return "";
            }
            
            if (M.ibMode)
            {
                // Accept "speaker" indicators from picture commands, and choices/choice cases, but otherwise ignore the standard result string
                if (opcode != C_PICTURE && opcode != C_CHOICE && opcode != C_CHOICECASE)
                    result = "";
                
                if (opcode == C_MESSAGE) // Starting new message
                {
                    if (M.completeMessage == null) // First message in page: just start new string for it
                    {
                        M.completeMessage = new StringWriter(new StringBuilder());
                        if (stringArg != "")
                            M.completeMessage.WriteLine(stringArg);
                    }
                    else // Add any existing message first, then start new
                    {
                        string message = M.completeMessage.ToString();
                        M.completeMessage = new StringWriter(new StringBuilder());
                        if (stringArg != "")
                            M.completeMessage.WriteLine(stringArg);
                        return message + (message != ""? Environment.NewLine : "") + result; // Write completed message and this command to script
                    }
                }
                else if (opcode == C_MESSAGEFOLLOW) // Continuing message, add to current string
                {
                    if (stringArg != "")
                        M.completeMessage.WriteLine(stringArg);
                }
                else if (M.completeMessage != null && M.completeMessage.ToString() != "") // Non-message command, add fully-assembled message
                {
                    string message = M.completeMessage.ToString();
                    M.completeMessage = new StringWriter(new StringBuilder());
                    return message + Environment.NewLine + result; // Write completed message and this command to script
                }
                
                if (opcode == 0 && M.completeMessage != null) // At end of page, write what was left to script
                    return M.completeMessage.ToString();
            }
            
            // In MessageScripts mode, if there was a message written before this and not a page header or something, put an extra line between the two.
            if (M.includeMessages && !M.includeActions && result != "")
            {
                if (M.lastWroteAMessage && !M.ibMode
                 && (isMessageStart() || opcode == C_CHOICE || opcode == C_CHOICECASE || opcode == C_SHOWSTRINGPICTURE))
                    result = Environment.NewLine + result;
                
                if (opcode == C_MESSAGE || opcode == C_MESSAGEFOLLOW || opcode == C_CHOICE || opcode == C_CHOICECASE || opcode == C_SHOWSTRINGPICTURE)
                    M.lastWroteAMessage = true;
                else
                    M.lastWroteAMessage = false;
            }
            
            if (includeIndent)
                result = result.Replace(Environment.NewLine, Environment.NewLine + indentStr);
            
            return result != ""? (indentStr + marker + result) : result;
        }
        
        string command10Blank() // 0a
        {
            return "";
        }
        
        string command1005BattleCallEvent() // 87 6d
        {
            int ev = args[0];
            
            return "Call Event: " + M.getDataCommon(ev);
        }
        
        string command1006ForceFlee() // 87 6e
        {
            int scope = args[0];
            int target = args[0];
            bool ignoreIfSurrounded = (args[2] == 1);
            
            string targetStr = "";
            if (scope == 0)
                targetStr = "Party";
            else if (scope == 1)
                targetStr = "All Enemies";
            else if (scope == 2)
                targetStr = "Enemy " + target;
            
            return "Force Flee: " + targetStr + (ignoreIfSurrounded? " (Ignore If Target Surrounded)" : "");
        }
        
        string command1007EnableCombo() // 87 6f
        {
            int hero = args[0];
            int battleCommand = args[1];
            int repeats = args[2];
            
            return "Enable Combo: " + M.getDataHero(hero) + ", " + M.getDataBattleCommand(battleCommand) + ", Repeat x" + repeats;
        }
        
        string command1008ChangeClass() // 87 70
        {
            int unknown = args[0];
            int hero = args[1];
            int classID = args[2];
            int levelBehavior = args[3];
            int skillBehavior = args[4];
            int statBehavior = args[5];
            bool showLevelUp = (args[6] == 1);
            
            return "Change Class: " + M.getDataHero(hero) + " to " + M.getDataClass(classID) + ", "
                + classChangeLevel[levelBehavior] + ", "
                + classChangeSkills[skillBehavior] + ", "
                + classChangeStats[statBehavior]
                + (showLevelUp? " (Show Level Up)" : "");
        }
        
        string command1009ChangeBattleCommands() // 87 71
        {
            int unknown = args[0];
            int hero = args[1];
            int battleCommand = args[2];
            bool remove = (args[3] == 0);
            
            return "Change Battle Commands: " + M.getDataHero(hero) + ", "
                + (remove? "Remove" : "Add") + " " + M.getDataBattleCommand(battleCommand);
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3005ManiacsUnknown()
        {
            int unknown1 = args[0];
            int unknown2 = args[1];
            
            return "Unknown Command (3005): " + unknown1 + ", " + unknown2;
        }
        
        // todo: 2003 Maniacs patch command; only includes string for now, none of the arguments
        string command3007ShowStringPicture()
        {
            string displayString = stringArg.Substring(1, stringArg.Length - 3); // Strings start with 0x01 and end with two 0x01s
            int unknown1 = args.Length > 0? args[0] : -1;
            int unknown2 = args.Length > 1? args[1] : -1;
            int unknown3 = args.Length > 2? args[2] : -1;
            int unknown4 = args.Length > 3? args[3] : -1;
            int unknown5 = args.Length > 4? args[4] : -1;
            int unknown6 = args.Length > 5? args[5] : -1;
            int unknown7 = args.Length > 6? args[6] : -1;
            int unknown8 = args.Length > 7? args[7] : -1;
            int unknown9 = args.Length > 8? args[8] : -1;
            int unknown10 = args.Length > 9? args[9] : -1;
            int unknown11 = args.Length > 10? args[10] : -1;
            int unknown12 = args.Length > 11? args[11] : -1;
            int unknown13 = args.Length > 12? args[12] : -1;
            int unknown14 = args.Length > 13? args[13] : -1;
            int unknown15 = args.Length > 14? args[14] : -1;
            int unknown16 = args.Length > 15? args[15] : -1;
            int unknown17 = args.Length > 16? args[16] : -1;
            int unknown18 = args.Length > 17? args[17] : -1;
            int unknown19 = args.Length > 18? args[18] : -1;
            int unknown20 = args.Length > 19? args[19] : -1;
            int unknown21 = args.Length > 20? args[20] : -1;
            int unknown22 = args.Length > 21? args[21] : -1;
            
            return "Show String Picture: '" + displayString + "'";
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3008ManiacsUnknown()
        {
            int unknown1 = args[0];
            int unknown2 = args[1];
            int unknown3 = args[2];
            int unknown4 = args[3];
            int unknown5 = args[4];
            int unknown6 = args[5];
            int unknown7 = args[6];
            int unknown8 = args[7];
            
            return "Unknown Command (3008): " + unknown1 + ", " + unknown2 + ", " + unknown3 + ", " + unknown4 + ", " + unknown5 + ", " + unknown6 + ", " + unknown7 + ", " + unknown8;
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3013ManiacsUnknown()
        {
            int unknown1 = args[0];
            int unknown2 = args[1];
            int unknown3 = args[2];
            int unknown4 = args[3];
            int unknown5 = args[4];
            
            return "Unknown Command (3013): " + unknown1 + ", " + unknown2 + ", " + unknown3 + ", " + unknown4 + ", " + unknown5;
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3014ManiacsUnknown()
        {
            int unknown1 = args[0];
            int unknown2 = args[1];
            int unknown3 = args[2];
            int unknown4 = args[3];
            
            return "Unknown Command (3014): " + unknown1 + ", " + unknown2 + ", " + unknown3 + ", " + unknown4;
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3016ManiacsUnknown()
        {
            int unknown1 = args.Length > 0? args[0] : -1;
            int unknown2 = args.Length > 1? args[1] : -1;
            int unknown3 = args.Length > 2? args[2] : -1;
            int unknown4 = args.Length > 3? args[3] : -1;
            int unknown5 = args.Length > 4? args[4] : -1;
            int unknown6 = args.Length > 5? args[5] : -1;
            
            return "Unknown Command (3016): " + unknown1 + ", " + unknown2 + ", " + unknown3 + ", " + unknown4 + ", " + unknown5 + ", " + unknown6;
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3018ManiacsUnknown()
        {
            int unknown1 = args[0];
            int unknown2 = args[1];
            int unknown3 = args[2];
            int unknown4 = args[3];
            
            return "Unknown Command (3018): " + unknown1 + ", " + unknown2 + ", " + unknown3 + ", " + unknown4;
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3021ManiacsUnknown()
        {
            int unknown1 = args[0];
            int unknown2 = args[1];
            int unknown3 = args[2];
            int unknown4 = args[3];
            int unknown5 = args[4];
            int unknown6 = args[5];
            int unknown7 = args[6];
            int unknown8 = args[7];
            
            return "Unknown Command (3021): " + unknown1 + ", " + unknown2 + ", " + unknown3 + ", " + unknown4 + ", " + unknown5 + ", " + unknown6 + ", " + unknown7 + ", " + unknown8;
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3025ManiacsUnknown()
        {
            int unknown1 = args[0];
            int unknown2 = args[1];
            int unknown3 = args[2];
            int unknown4 = args[3];
            int unknown5 = args[4];
            int unknown6 = args[5];
            int unknown7 = args[6];
            
            return "Unknown Command (3025): " + unknown1 + ", " + unknown2 + ", " + unknown3 + ", " + unknown4 + ", " + unknown5 + ", " + unknown6 + ", " + unknown7;
        }
        
        // todo: unknown 2003 Maniacs patch command
        string command3026ManiacsUnknown()
        {
            string unknownString = stringArg;
            int unknown1 = args[0];
            int unknown2 = args[1];
            int unknown3 = args[2];
            
            return "Unknown Command (3026): " + unknownString + ", " + unknown1 + ", " + unknown2 + ", " + unknown3;
        }
        
        string command5001CallLoad() // a7 09
        {
            return "Call Load Menu";
        }
        
        string command5002ExitGame() // a7 0a
        {
            return "Exit Game";
        }
        
        string command5003ToggleATBMode() // a7 0b
        {
            return "Toggle ATB Mode";
        }
        
        string command5004ToggleFullscreen() // a7 0c
        {
            return "Toggle Fullscreen";
        }
        
        string command5005OpenVideoOptions() // a7 0d
        {
            return "Open Video Options";
        }
        
        string command10110Message() // ce 7e
        {
            return stringArg;
        }
        
        string command20110MessageFollow() // 81 9d 0e
        {
            return stringArg;
        }
        
        string command10120MessageOptions() // cf 08
        {
            bool transparent = (args[0] == 1);
            int position = args[1];
            bool dontCoverHero = (args[2] == 1);
            bool continueEvents = (args[3] == 1);
            
            return "Message Options: "
                + (!transparent? "Normal" : "Transparent") + ", "
                + messagePositions[position]
                + (dontCoverHero? ", Avoid Covering Hero" : "")
                + (continueEvents? ", Let Events Continue" : ", Stop Events");
        }
        
        string command10130ChangeFace() // cf 12
        {
            int index = args[0];
            bool rightSide = (args[1] == 1);
            bool flip = (args[2] == 1);
            
            if (M.starUnknownMode)
            {
                string description = "ERASE";
                
                if (stringArg.Equals("なまえ") || stringArg.Equals("Names"))
                    description = new string[] { "Fukuro", "Eddie", "Ophelia", "???",
                                                 "Fukuro Sprite", "Eddie Sprite", "Sooticci", "Kid",
                                                 "Badoh", "Fungelite", "Nopass", "Citizen",
                                                 "Portley", "Sooticci (C)", "Corme", "Huemin" }[index];
                else if (stringArg.Equals("なまえ2") || stringArg.Equals("Names2"))
                    description = new string[] { "Giera Toph", "Giera Rius", "Daios", "Bienz",
                                                 "Commander", "Soldier", "Sooticci", "ReveR",
                                                 "Yurika", "Maid", "Star Goddess", "Navi-Robo",
                                                 "Soupirit", "Maid (C)", "Oedo", "Bienz (C)" }[index];
                else if (stringArg.Equals("なまえ3") || stringArg.Equals("Names3"))
                    description = new string[] { "Giera Toph (C)", "Townsperson (C)", "", "Corme (C)",
                                                 "Badoh (C)", "Ophelia (C)", "Yurika (C)", "Scholar",
                                                 "Waystern", "Desmond", "Cookteria", "Commander (C)",
                                                 "Star Goddess (C)", "Hyottoko", "Okame", "Midorino" }[index];
                else if (stringArg.Equals("なまえ4") || stringArg.Equals("Names4"))
                    description = new string[] { "Sprite", "Mockingbird", "Toss-R-Us", "Buddhuu",
                                                 "Kochka", "Baws", "Earlybird", "Guntram",
                                                 "Twelam", "Mockingbird (C)", "Guntram (C)", "Twelam (C)",
                                                 "Salmi", "Akki", "Licoris", "Oshino" }[index];
                else if (stringArg.Equals("なまえ5") || stringArg.Equals("Names5"))
                    description = new string[] { "Day 1", "Night 1", "Morning 2", "Day 2",
                                                 "Grudge", "Clown", "Ringleader", "Narrator",
                                                 "Night 2", "Night 3", "Morning 3", "Day 3",
                                                 "Clerk", "Miner", "Evenezer", "Scholar (C)" }[index];
                else if (stringArg.Equals("なまえ6") || stringArg.Equals("Names6"))
                    description = new string[] { "Miner (C)", "Natalia", "Boyf (C)", "Girlf",
                                                 "Boyf", "Avenly", "Failure", "Natalia (C)",
                                                 "Oshino (C)", "Florina", "Leader", "Leader (C)",
                                                 "Grandsprite", "Pumpkin", "Mermaid", "Hakata" }[index];
                else if (stringArg.Equals("なまえ7") || stringArg.Equals("Names7"))
                    description = new string[] { "Hakata (C)", "Sprite?", "Ghost?", "Cultney",
                                                 "Morning 4", "Bonus", "Fukuro Pose", "Eddie Pose",
                                                 "", "", "", "",
                                                 "", "", "", "" }[index];
                
                return description;
            }
            else if (M.hanoiMode)
            {
                string description = "ERASE";
                
                if (stringArg.Equals("なまえ") || stringArg.Equals("Name"))
                    description = new string[] { "Coral", "???", "No-Name", "Merritica",
                                                 "Adams", "Researcher", "01", "Mira",
                                                 "Employee", "Cindy", "Adams", "Mira",
                                                 "Cindy", "Coral", "Merritica", "No-Name" }[index];
                else if (stringArg.Equals("なまえ2") || stringArg.Equals("Name2"))
                    description = new string[] { "Roland", "Tochuu", "Georges", "Noroi Kimon",
                                                 "Cleaner", "Cameron", "Crayon", "DHer",
                                                 "Guard", "HANOI", "Roland", "Georges",
                                                 "Noroi Kimon", "Cameron", "Crayon", "Inspector" }[index];
                else if (stringArg.Equals("なまえ3") || stringArg.Equals("Name3"))
                    description = new string[] { "Sunya", "Yoshida", "Clerk", "Dissolved",
                                                 "Chief", "IV", "Cleaner II", "Mira",
                                                 "IV", "Cleaner II", "Guest (Patient)", "Worker (Nurse)",
                                                 "Mascot (Doctor)", "Virus", "Guest (Mouse)", "Mouse" }[index];
                else if (stringArg.Equals("なまえ5") || stringArg.Equals("Name5"))
                    description = new string[] { "Sunya", "Cleaner", "Cleaner II", "IV",
                                                 "Outlaw", "Spider Director", "CLOCK", "Nurse",
                                                 "Doctor", "Patient", "Reaper", "Regret",
                                                 "Crew", "Bayonetta", "Tedious Woman", "Idiotic Man" }[index];
                else if (stringArg.Equals("なまえ6") || stringArg.Equals("Name6"))
                    description = new string[] { "Li'l Sally", "Protective Father", "Protective Mother", "Cleaner",
                                                 "Rosemary", "Yellowbird of Dreams", "Dream-Eating Machine", "\"\"\"Nurse\"\"\"",
                                                 "Bayonetta", "Captain", "Sister (Patient)", "Elder",
                                                 "Worker", "Old Television", "DHer", "01" }[index];
                else if (stringArg.Equals("なまえ7") || stringArg.Equals("Name7"))
                    description = new string[] { "Coral (Unsure)", "Coral", "Coral (Ex-Inspector)", "Coral (Team Sunya)",
                                                 "Counselor", "Hotel Staff", "Guest", "Detective",
                                                 "Dangerous Phone", "Batty", "Bug-Eating Dog", "Dog",
                                                 "IV", "Cornelia", "Electroo", "" }[index];
                else if (stringArg.Equals("なまえ8") || stringArg.Equals("Name8"))
                    description = new string[] { "Masses", "???-Type HANOI", "Child", "Shopkeep",
                                                 "(Corrupted)", "Staff", "Believer (Sister/Priest/Patient)", "(Blank)",
                                                 "Child", "Shopkeep", "Child", "Shopkeep",
                                                 "Mascot (Doctor)", "Wirus", "BONUS", "" }[index];
                else if (stringArg.Equals("なまえ9") || stringArg.Equals("Name9"))
                    description = new string[] { "Merritica (Bonus)", "No-Name (Bonus)", "Adams (Bonus)", "Mira (Bonus)",
                                                 "Cindy (Bonus)", "Roland (Bonus)", "Georges (Bonus)", "Noroi Kimon (Bonus)",
                                                 "Cameron (Bonus)", "Crayon (Bonus)", "Sunya (Bonus)", "Cleaner (Bonus)",
                                                 "Cleaner II (Bonus)", "IV (Bonus)", "Thieving Virus", "", }[index];
                
                return description;
            }
            
            if (stringArg != "")
                return "Select Face Graphic: " + stringArg + ", " + (index + 1) + ", "
                    + (rightSide? "Right" : "Left") + ", "
                    + (flip? "Flip Horizontal" : "Normal");
            else
                return "Select Face Graphic: Erase";
        }
        
        string command10140Choice() // cf 1c
        {
            int cancelType = args[0];
            
            if (!M.includeActions)
                return "<" + stringArg + ">";
            
            return "Choice: " + (M.includeMessages? stringArg : "___") + " (Cancel: "
                + (cancelType == 0? "Not Allowed" : cancelType == 5? "Separate Case" : "Case " + cancelType) + ")";
        }
        
        string command20140ChoiceCase() // 81 9d 2c
        {
            int number = args[0];
            
            bool includeString = M.includeMessages || (M.stringScriptExportMode && M.userSettings["StringScriptDetails"] == 1);
            
            if (number != 4)
            {
                if (includeString)
                    return "[" + stringArg + "]" + (M.includeActions? " Case (" + (number + 1) + ")" : "");
                else
                    return "Case " + (number + 1);
            }
            else
                return "Cancel Case";
        }
        
        string command20141EndCase() // 81 9d 2d
        {
            return "End Choice";
        }
        
        string command10150InputNumber() // cf 26
        {
            int digits = args[0];
            int variableID = args[1];
            
            return "Input Number: " + digits + " digit" + (digits != 1? "s" : "") + ", Variable " + M.getDataVariable(variableID);
        }
        
        string command10210Switch() // cf 62
        {
            int type = args[0];
            int switch1 = args[1];
            int switch2 = args[2];
            int setType = args[3];
            
            string switchName = "";
            if (type == 0) // One
                switchName = "Switch " + M.getDataSwitch(switch1);
            else if (type == 1) // Range
                switchName = "Switch " + M.getDataSwitch(switch1) + " through " + M.getDataSwitch(switch2);
            else // Variable
                switchName = "Switch at Variable " + M.getDataVariable(switch1);
            
            return "Change Switch: " + switchName + " "
                + (setType == 0? "On" : setType == 1? "Off" : "Toggle");
        }
        
        string command10220Variable() // cf 6c
        {
            int type = args[0];
            int var1 = args[1];
            int var2 = args[2];
            int operation = args[3];
            int operand = args[4];
            int value = args[5];
            int value2 = args.Length > 6? args[6] : 0;
            
            string variableName = "";
            if (type == 0) // One
                variableName = "Variable " + M.getDataVariable(var1);
            else if (type == 1) // One
                variableName = "Variable " + M.getDataVariable(var1) + " through " + M.getDataVariable(var2);
            else // Variable
                variableName = "Variable at Variable " + M.getDataVariable(var1);
            
            string rightSide = "";
            if (operand == 0) // Fixed
                rightSide = value.ToString();
            else if (operand == 1) // Variable
                rightSide = "Variable " + M.getDataVariable(value);
            else if (operand == 2) // Variable At
                rightSide = "Variable at Variable " + M.getDataVariable(value);
            else if (operand == 3) // Random
                rightSide = "Random from " + value + " to " + value2;
            else if (operand == 4) // Item
                rightSide = M.getDataItem(value) + " Quantity " + (value2 == 0? "(Owned)" : "(Equipped)");
            else if (operand == 5) // Hero
                rightSide = M.getDataHero(value) + "'s " + heroQualities[value2];
            else if (operand == 6) // Event
                rightSide = M.getTargetEvent(value) + "'s " + eventQualities[value2];
            else if (operand == 7) // Other
                rightSide = miscData[value];
            else if (operand == 8) // Enemy (In Battle)
                rightSide = "Enemy " + (value + 1) + "'s " + enemyStats[value2];
            
            return "Change Variable: " + variableName + " " + operations[operation] + " " + rightSide;
        }
        
        string command10230Timer() // cf 76
        {
            int operation = args[0];
            bool byVariable = (args[1] == 1);
            int seconds = args[2];
            bool display = (args[3] == 1);
            bool inBattle = (args[4] == 1);
            int whichTimer = args.Length > 5? args[5] : 0; // 2003
            
            string setStart = "";
            if (operation == 0) // Set
            {
                setStart = ", " + (!byVariable? ((seconds / 60) + " min " + (seconds % 60) + " sec")
                    : "Variable " + M.getDataVariable(seconds));
            }
            else if (operation == 1) // Start
            {
                setStart = (display? ", Display" : "")
                    + (inBattle? ", Runs In Battle" : "");
            }
            
            return "Timer" + (whichTimer == 1? " 2" : "") + ": "
                + (operation == 0? "Set" : operation == 1? "Start" : "Stop") + setStart;
        }
        
        string command10310ChangeMoney() // d0 46
        {
            bool remove = (args[0] == 1);
            bool byVariable = (args[1] == 1);
            int value = args[2];
            
            return "Change Money: "
                + (!remove? "Add" : "Subtract") + " "
                + (!byVariable? value.ToString() : "Variable " + M.getDataVariable(value));
        }
        
        string command10320ChangeItems() // d0 50
        {
            bool drop = (args[0] == 1);
            bool byVariableLeft = (args[1] == 1);
            int value = args[2];
            bool byVariableRight = (args[3] == 1);
            int value2 = args[4];
            
            if (M.checkUnusedData && byVariableLeft) // Variable reference to Items
                M.variableReferencedDatabases.Add("Items");
            
            return "Change Items: "
                + (drop? "Drop" : "Add") + " "
                + (!byVariableLeft? M.getDataItem(value) : "Item at Variable " + M.getDataVariable(value))
                + " x " + (!byVariableRight? value2.ToString() : "Variable " + M.getDataVariable(value2));
        }
        
        string command10330ChangeParty() // d0 5a
        {
            bool remove = (args[0] == 1);
            bool byVariable = (args[1] == 1);
            int value = args[2];
            
            if (M.checkUnusedData && byVariable) // Variable reference to Heroes
                M.variableReferencedDatabases.Add("Heroes");
            
            return "Change Party: "
                + (remove? "Remove" : "Add") + " "
                + (!byVariable? M.getDataHero(value) : "Hero at Variable " + M.getDataVariable(value));
        }
        
        string command10410ChangeEXP() // d1 2a
        {
            int target = args[0];
            int value = args[1];
            bool subtract = (args[2] == 1);
            bool byVariable = (args[3] == 1);
            int value2 = args[4];
            bool showLevelUp = (args[5] == 1);
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Change EXP: "
                + targetName + ", "
                + (subtract? "Subtract" : "Add") + " "
                + (!byVariable? value2.ToString() : "Variable " + M.getDataVariable(value2))
                + (showLevelUp? ", Show Level Up" : "");
        }
        
        string command10420ChangeLevel() // d1 34
        {
            int target = args[0];
            int value = args[1];
            bool subtract = (args[2] == 1);
            bool byVariable = (args[3] == 1);
            int value2 = args[4];
            bool showLevelUp = (args[5] == 1);
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Change Level: "
                + targetName + ", "
                + (subtract? "Subtract" : "Add") + " "
                + (!byVariable? value2.ToString() : "Variable " + M.getDataVariable(value2))
                + (showLevelUp? ", Show Level Up" : "");
        }
        
        string command10430ChangeStats() // d1 3e
        {
            int target = args[0];
            int value = args[1];
            bool minus = (args[2] == 1);
            int stat = args[3];
            bool byVariable = (args[4] == 1);
            int value2 = args[5];
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Change Stats: "
                + targetName + ", "
                + basicStats[stat] + " "
                + (minus? "-" : "+") + " "
                + (!byVariable? value2.ToString() : "Variable " + M.getDataVariable(value2));
        }
        
        string command10440ChangeSkills() // d1 48
        {
            int target = args[0];
            int value = args[1];
            bool unlearn = (args[2] == 1);
            bool byVariable = (args[3] == 1);
            int value2 = args[4];
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Change Skills: "
                + targetName + ", "
                + (unlearn? "Unlearn" : "Learn") + " "
                + (!byVariable? M.getDataSkill(value2) : "Skill at Variable " + M.getDataVariable(value2));
        }
        
        string command10450ChangeEquip() // d1 52
        {
            int target = args[0];
            int value = args[1];
            bool unequip = args[2] == 1;
            int type = args[3];
            int value2 = args[4];
            
            string targetName = M.getHeroTarget(target, value);
            
            string equipmentOrSlot = "";
            if (!unequip)
            {
                if (M.checkUnusedData && type != 0) // Variable reference to Items
                    M.variableReferencedDatabases.Add("Items");
                
                equipmentOrSlot = (type == 0? M.getDataItem(value2) : "Item at Variable " + M.getDataVariable(value2));
            }
            else
                equipmentOrSlot = equipTypes[type];
            
            return "Change Equipment: "
                + targetName + ", "
                + (unequip? "Unequip" : "Equip") + " "
                + equipmentOrSlot;
        }
        
        string command10460ChangeHP() // d1 5c
        {
            int target = args[0];
            int value = args[1];
            bool subtract = (args[2] == 1);
            bool byVariable = (args[3] == 1);
            int value2 = args[4];
            bool canDie = (args[5] == 1);
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Change HP: "
                + targetName + ", "
                + (subtract? "Subtract" : "Add") + " "
                + (!byVariable? value2.ToString() : "Variable " + M.getDataVariable(value2))
                + (subtract && canDie? ", Can Cause Death" : "");
        }
        
        string command10470ChangeMP() // d1 66
        {
            int target = args[0];
            int value = args[1];
            bool subtract = (args[2] == 1);
            bool byVariable = (args[3] == 1);
            int value2 = args[4];
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Change MP: "
                + targetName + ", "
                + (subtract? "Subtract" : "Add") + " "
                + (!byVariable? value2.ToString() : "Variable " + M.getDataVariable(value2));
        }
        
        string command10480ChangeState() // d1 70
        {
            int target = args[0];
            int value = args[1];
            bool cure = (args[2] == 1);
            int conditionID = args[3];
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Change State: "
                + targetName + ", "
                + (cure? "Cure" : "Inflict") + " "
                + M.getDataCondition(conditionID);
        }
        
        string command10490FullRecover() // d1 7a
        {
            int target = args[0];
            int value = args[1];
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Full Recovery: " + targetName;
        }
        
        string command10500Damage() // d2 04
        {
            int target = args[0];
            int value = args[1];
            int attack = args[2];
            int defensePercent = args[3];
            int mindPercent = args[4];
            int variance = args[5];
            bool useVariable = (args[6] == 1);
            int variableID = args[7];
            
            string targetName = M.getHeroTarget(target, value);
            
            return "Take Damage: "
                + targetName + ", "
                + "Attack " + attack + ", "
                + "Defense " + defensePercent + "%, "
                + "Mind " + mindPercent + "%, "
                + "Variance " + variance
                + (useVariable? ", Consult Variable " + M.getDataVariable(variableID) : "");
        }
        
        string command10610NameChange() // d2 72
        {
            int hero = args[0];
            
            return "Change Name: " + M.getDataHero(hero) + ", " + (M.includeMessages? stringArg : "___");
        }
        
        string command10620TitleChange() // d2 7c
        {
            int hero = args[0];
            
            return "Change Title: " + M.getDataHero(hero) + ", " + (M.includeMessages? stringArg : "___");
        }
        
        string command10630CharSetChange() // d3 06
        {
            int hero = args[0];
            int index = args[1];
            bool transparent = (args[2] == 1);
            
            return "Change Walk Graphic: " + M.getDataHero(hero) + ", "
                + stringArg + ", Index " + (index + 1)
                + (transparent? " (Transparent)" : "");
        }
        
        string command10640FaceSetChange() // d3 10
        {
            int hero = args[0];
            int index = args[1];
            
            return "Change Face: " + M.getDataHero(hero) + ", "
                + stringArg + ", Index " + (index + 1);
        }
        
        string command10650VehicleGraphic() // d3 1a
        {
            int vehicle = args[0];
            int index = args[1];
            
            return "Change Vehicle Graphic: "
                + (vehicle == 0? "Boat" : vehicle == 1? "Ship" : "Airship") + ", "
                + stringArg + ", Index " + (index + 1);
        }
        
        string command10660SystemBGMChange() // d3 24
        {
            int song = args[0];
            int fade = args[1];
            int volume = args[2];
            int tempo = args[3];
            int balance = args[4];
            
            return "Change System BGM: " + systemBGMs[song] + ", " + stringArg
                + ", Fade " + (fade / 1000) + " sec, "
                + M.getSoundVTB(volume, tempo, balance);
        }
        
        string command10670SystemSoundChange() // d3 2e
        {
            int sound = args[0];
            int volume = args[1];
            int tempo = args[2];
            int balance = args[3];
            
            return "Change System Sound: " + systemSounds[sound] + ", " + stringArg + ", "
                + M.getSoundVTB(volume, tempo, balance);
        }
        
        string command10680SystemGraphicsChange() // d3 38
        {
            bool tile = (args[0] == 1);
            int font = args[1];
            
            return "Change System Graphics: " + stringArg + ", "
                + (tile? "Tile" : "Stretch") + ", "
                + "Font " + (font + 1);
        }
        
        string command10690TransitionChange() // d3 42
        {
            int transition = args[0];
            int effect = args[1];
            
            string effectStr = "";
            if (transition == 0 || transition == 2 || transition == 4)
                effectStr = M.getEraseEffects(effect);
            else if (transition == 1 || transition == 3 || transition == 5)
                effectStr = M.getShowEffects(effect);
            
            return "Change Transition: " + transitions[transition] + ", " + effectStr;
        }
        
        string command10710BattleStart() // d3 56
        {
            bool byVariable = (args[0] == 1);
            int troop = args[1];
            int bgType = args[2];
            int escape = args[3];
            bool defeatCase = (args[4] == 1);
            bool firstStrike = (args[5] == 1);
            int specialBattleType = args.Length > 6? args[6] : 0; // 2003
            int looseTight = args.Length > 7? args[7] : 0; // 2003
            int terrain = args.Length > 8? args[8] : 0; // 2003
            
            bool extended = args.Length > 6;
            
            string specialConditionStr = "", bgString = "";
            if (extended)
            {
                specialConditionStr = specialBattleTypes[specialBattleType] + ", ";
                if (bgType == 0)
                    bgString = "Default";
                else if (bgType == 1)
                    bgString = stringArg + " (" + (looseTight == 0? "Loose Formation" : "Tight Formation") + ")";
                else if (bgType == 2)
                    bgString = "Associated With " + M.getDataTerrain(terrain);
            }
            else
                bgString = (bgType == 0? "Default" : stringArg);
            
            if (M.checkUnusedData && byVariable) // Variable reference to Troops
                M.variableReferencedDatabases.Add("Troops");
            
            return "Battle Start: "
                + specialConditionStr
                + (!byVariable? M.getDataTroop(troop) : "Troop at Variable " + M.getDataVariable(troop)) + ", "
                + "Background " + bgString + ", "
                + "Escape " + (escape == 0? "Allowed" : escape == 1? "Forbidden" : "Case") + ", "
                + "Defeat " + (defeatCase? "Case" : "Game Over")
                + (firstStrike? ", First Strike" : "");
        }
        
        string command20710BattleWinCase() // 81 a1 66
        {
            return "Victory Case";
        }
        
        string command20711BattleFleeCase() // 81 a1 67
        {
            return "Escape Case";
        }
        
        string command20712BattleLoseCase() // 81 a1 68
        {
            return "Defeat Case";
        }
        
        string command20713BattleEndCase() // 81 a1 69
        {
            return "End Battle Case";
        }
        
        string command10720OpenShop() // d3 60
        {
            int shopType = args[0];
            int message = args[1];
            bool successBranch = (args[2] == 1);
            // args[3] might be item list length
            
            string shopList = "";
            for (int i = 4; i < args.Length; i++)
            {
                shopList += M.getDataItem(args[i]);
                if (i + 1 < args.Length)
                    shopList += ", ";
            }
            
            return "Open Shop: "
                + (shopType == 0? "Normal": shopType == 1? "No Sell" : "Sell Only") + ", "
                + "Message Set " + (message == 0? "A" : message == 1? "B" : "C") + "; "
                + shopList
                + (successBranch? " (Sale Branch)" : "");
        }
        
        string command20720ShopSaleCase() // 81 a1 72
        {
            return "Transaction Case";
        }
        
        string command20721ShopCancelCase() // 81 a1 73
        {
            return "No Transaction Case";
        }
        
        string command20722ShopEndCase() // 81 a1 74
        {
            return "End Shop Case";
        }
        
        string command10730CallInn() // d3 6a
        {
            int message = args[0];
            int price = args[1];
            bool successBranch = (args[2] == 1);
            
            return "Call Inn: "
                + "Message Set " + (message == 0? "A" : "B") + ", "
                + "Price " + price
                + (successBranch? " (Rest Branch)" : "");
        }
        
        string command20730InnRestCase() // 81 a1 7a
        {
            return "Rest Case";
        }
        
        string command20731InnCancelCase() // 81 a1 7b
        {
            return "Inn Cancel Case";
        }
        
        string command20732InnEndCase() // 81 a1 7c
        {
            return "End Inn Case";
        }
        
        string command10740NameEntry() // d3 74
        {
            int hero = args[0];
            int inputPage = args[1];
            bool showDefault = (args[2] == 1);
            
            return "Enter Hero Name: " + M.getDataHero(hero) + ", "
                + (inputPage == 0? "Page 1 (Hiragana)" : "Page 2 (Katakana)") + " Input"
                + (showDefault? ", Show Default" : "");
        }
        
        string command10810Teleport() // d3 3a
        {
            int map = args[0];
            int x = args[1];
            int y = args[2];
            int dir = args.Length > 3? args[3] - 1 : -1; // 2003
            
            return "Teleport: " + M.getDataMap(map) + " (" + x + "," + y + ")" + (dir != -1? (", Face " + Page.charDirs[dir]) : "");
        }
        
        string command10820RememberPlace() // d4 44
        {
            int map = args[0];
            int x = args[1];
            int y = args[2];
            
            return "Remember Location: "
                + "Map to " + M.getDataVariable(map) + ", X to " + M.getDataVariable(x) + ", Y to " + M.getDataVariable(y);
        }
        
        string command10830RestorePlace() // d4 4e
        {
            int map = args[0];
            int x = args[1];
            int y = args[2];
            
            return "Restore Location: "
                + "Map from " + M.getDataVariable(map) + ", X from " + M.getDataVariable(x) + ", Y from " + M.getDataVariable(y);
        }
        
        string command10840RideVehicle() // d4 58
        {
            return "Ride Vehicle";
        }
        
        string command10850PlaceVehicle() // d4 64
        {
            int vehicle = args[0];
            bool byVariable = (args[1] == 1);
            int map = args[2];
            int x = args[3];
            int y = args[4];
            
            string positionStr = "";
            if (!byVariable)
                positionStr = M.getDataMap(map) + " (" + x + "," + y + ")";
            else
                positionStr = "Map from " + M.getDataVariable(map) + ", X from " + M.getDataVariable(x) + ", Y from " + M.getDataVariable(y);
            
            return "Place Vehicle: "
                + (vehicle == 0? "Boat" : vehicle == 1? "Ship" : "Airship") + ", "
                + positionStr;
        }
        
        string command10860PlaceEvent() // d4 6c
        {
            int ev = args[0];
            bool byVariable = (args[1] == 1);
            int x = args[2];
            int y = args[3];
            int dir = args.Length > 4? args[4] - 1 : -1; // 2003
            
            string positionStr = "";
            if (!byVariable)
                positionStr = "(" + x + "," + y + ")";
            else
                positionStr = "(" + M.getDataVariable(x) + "," + M.getDataVariable(y) + ")";
            
            return "Place Event: "
                + (ev == 10005? "This Event" : "Event #" + ev) + ", "
                + positionStr
                + (dir != -1? ", Face " + Page.charDirs[dir] : "");
        }
        
        string command10870SwapEvents() // d4 76
        {
            int ev = args[0];
            int ev2 = args[1];
            
            return "Swap Events: "
                + (ev == 10005? "This Event" : "Event #" + ev) + ", "
                + (ev2 == 10005? "This Event" : "Event #" + ev2);
        }
        
        string command10910GetTerrainID() // d5 1e
        {
            bool byVariable = (args[0] == 1);
            int x = args[1];
            int y = args[2];
            int destinationVar = args[3];
            
            string positionStr = "";
            if (!byVariable)
                positionStr = "(" + x + "," + y + ")";
            else
                positionStr = "(" + M.getDataVariable(x) + "," + M.getDataVariable(y) + ")";
            
            return "Get Terrain ID: "
                + "Variable " + M.getDataVariable(destinationVar) + " = Terrain ID at " + positionStr;
        }
        
        string command10920GetEventID() // d5 28
        {
            bool byVariable = (args[0] == 1);
            int x = args[1];
            int y = args[2];
            int destinationVar = args[3];
            
            string positionStr = "";
            if (!byVariable)
                positionStr = "(" + x + "," + y + ")";
            else
                positionStr = "(" + M.getDataVariable(x) + "," + M.getDataVariable(y) + ")";
            
            return "Get Event ID: "
                + "Variable " + M.getDataVariable(destinationVar) + " = Event ID at " + positionStr;
        }
        
        string command11010EraseScreen() // d6 02
        {
            int effect = args[0];
            
            return "Erase Screen: " + M.getEraseEffects(effect);
        }
        
        string command11020ShowScreen() // d6 0c
        {
            int effect = args[0];
            
            return "Show Screen: " + M.getShowEffects(effect);
        }
        
        string command11030ScreenTone() // d6 16
        {
            int red = args[0];
            int green = args[1];
            int blue = args[2];
            int chroma = args[3];
            int time = args[4];
            bool wait = (args[5] == 1);
            
            float sec = time / 10f;
            
            return "Set Screen Tone: "
                + "R" + red + " G" + green + " B" + blue + " S" + chroma + ", "
                + sec + " sec"
                + (wait? ", Wait" : "");
        }
        
        string command11040ScreenFlash() // d6 20
        {
            int red = args[0];
            int green = args[1];
            int blue = args[2];
            int power = args[3];
            int time = args[4];
            bool wait = (args[5] == 1);
            int function = args.Length > 6? args[6] : 0; // 2003
            
            float sec = time / 10f;
            
            string flashStr = "";
            if (function == 0) // Flash Once
                flashStr = "R" + red + " G" + green + " B" + blue + " V" + power + ", "
                    + sec + " sec"
                    + (wait? ", Wait" : "");
            else if (function == 1) // Begin Flash (2003)
                flashStr = "Begin, R" + red + " G" + green + " B" + blue + " V" + power + ", "
                    + sec + " sec";
            else if (function == 2) // End Flash (2003)
                flashStr = "Stop";
            
            return "Flash Screen: " + flashStr;
        }
        
        string command11050ScreenShake() // d6 2a
        {
            int power = args[0];
            int speed = args[1];
            int time = args[2];
            bool wait = (args[3] == 1);
            int function = args.Length > 4? args[4] : 0; // 2003
            
            float sec = time / 10f;
            
            string shakeStr = "";
            if (function == 0) // Shake Once
                shakeStr = "Power " + power + ", Speed " + speed + ", " + sec + " sec"
                    + (wait ? ", Wait" : "");
            else if (function == 1) // Begin Shake (2003)
                shakeStr = "Begin, Power " + power + ", Speed " + speed + ", " + sec + " sec";
            else if (function == 2) // End Shake (2003)
                shakeStr = "Stop";
            
            return "Shake Screen: " + shakeStr;
        }
        
        string command11060PanScreen() // d6 34
        {
            int type = args[0];
            int dir = args[1];
            int chips = args[2];
            int speed = args[3];
            bool wait = (args[4] == 1);
            
            string moveStr = "";
            if (type == 0)
                moveStr = "Fix";
            else if (type == 1)
                moveStr = "Undo Fix";
            else if (type == 2)
                moveStr = "Move " + Page.charDirs[dir] + " " + chips + " chip" + (chips != 1? "s" : "") + ", Speed " + speed;
            else if (type == 3)
                moveStr = "Return, Speed " + speed;
            if (wait)
                moveStr += ", Wait";
            
            return "Pan Screen: " + moveStr;
        }
        
        string command11070Weather() // d6 3e
        {
            int weather = args[0];
            int power = args[1];
            
            return "Call Weather: "
                + weathers[weather]
                + (weather != 0? ", " + (power == 0? "Low" : power == 1? "Medium" : "High") : "");
        }
        
        // todo: most of the 2003 English Picture extensions are not noted
        string command11110Picture() // d6 66
        {
            int picNum = args[0];
            bool positionByVariable = (args[1] == 1);
            int x = args[2];
            int y = args[3];
            bool followMap = (args[4] == 1);
            int zoom = args[5];
            int topTrans = args[6];
            int transColor = args[7];
            int red = args[8];
            int green = args[9];
            int blue = args[10];
            int chroma = args[11];
            int effect = args[12];
            int effectValue = args[13];
            int bottomTrans = args.Length > 14? args[14] : 0; // 2003
            // 15 and 16 not used in Show Picture
            bool picNumByVariable = args.Length > 17? (args[17] == 1) : false; // 2003 English
            int picPointerDigits = args.Length > 18? args[18] : 0; // 2003 English, not noted
            int picPointerVariableID = args.Length > 19? args[19] : 0; // 2003 English, not noted
            bool zoomByVariable = args.Length > 20? (args[20] == 1) : false; // 2003 English
            bool topTransByVariable = args.Length > 21? (args[21] == 1) : false; // 2003 English
            int spritesheetColumns = args.Length > 22? args[22] : 0; // 2003 English, not noted
            int spritesheetRows = args.Length > 23? args[23] : 0; // 2003 English, not noted
            int spritesheetAnimate = args.Length > 24? args[24] : 0; // 2003 English, not noted
            int spritesheetFrameOrSpeed = args.Length > 25? args[25] : 0; // 2003 English, not noted
            int spritesheetLoop = args.Length > 26? args[26] : 0; // 2003 English, not noted
            int mapLayer = args.Length > 27? args[27] : 0; // 2003 English, not noted
            int battleLayer = args.Length > 28? args[28] : 0; // 2003 English, not noted
            int flags = args.Length > 29? args[29] : 0; // 2003 English, not noted
            
            bool extended = args.Length > 14;
            
            if (M.ibMode)
            {
                if (stringArg.StartsWith("icon_"))
                {
                    if (stringArg.StartsWith("icon_g"))
                        M.currentSpeaker = "Garry";
                    else if (stringArg.StartsWith("icon_ibm"))
                        M.currentSpeaker = "Ib's Mom";
                    else if (stringArg.StartsWith("icon_ibp"))
                        M.currentSpeaker = "Ib's Dad";
                    else if (stringArg.StartsWith("icon_marvel"))
                        M.currentSpeaker = "Marvelous Night";
                    else if (stringArg.StartsWith("icon_m"))
                        M.currentSpeaker = "Mary";
                    
                    return "[" + M.currentSpeaker + "]";
                }
                else
                    return "";
            }
            
            string picStr = "";
            if (!picNumByVariable)
                picStr = picNum.ToString();
            else
                picStr = "Variable " + M.getDataVariable(picNum);
            
            string positionStr = "";
            if (!positionByVariable)
                positionStr = "(" + x + "," + y + ")";
            else
                positionStr = "Variable " + M.getDataVariable(x) + ", Variable " + M.getDataVariable(y);
            
            string zoomStr = "";
            if (!zoomByVariable)
                zoomStr = zoom.ToString();
            else
                zoomStr = "Variable " + M.getDataVariable(zoom);
            
            string effectStr = "";
            if (effect == 1)
                effectStr = ", Rotate Speed " + effectValue;
            else if (effect == 2)
                effectStr = ", Ripple Power " + effectValue;
            
            string transparencyStr = "";
            if (!extended)
                transparencyStr = "Transparency " + topTrans + "%";
            else
            {
                string topTransStr = !topTransByVariable? topTrans.ToString() : "Variable " + M.getDataVariable(topTrans);
                string bottomTransStr = !topTransByVariable? bottomTrans.ToString() : "Variable " + M.getDataVariable(bottomTrans);
                if (!topTransStr.Equals(bottomTransStr))
                    transparencyStr = "Top Transparency " + topTransStr + "%, Bottom Transparency " + bottomTransStr + "%";
                else
                    transparencyStr = "Transparency " + topTransStr + "%";
            }
            
            return "Show Picture: "
                + picStr + ", "
                + stringArg + ", "
                + positionStr + ", "
                + (followMap? "Follow Map, " : "")
                + "Zoom " + zoomStr + "%, "
                + transparencyStr + ", "
                + (transColor == 1? "Stir" : "No") + " Trans. Color, "
                + "R" + red + " G" + green + " B" + blue + " S" + chroma
                + effectStr;
        }
        
        string command11120MovePicture() // d6 70
        {
            int picNum = args[0];
            bool positionByVariable = (args[1] == 1);
            int x = args[2];
            int y = args[3];
            bool followMap = (args[4] == 1);
            int zoom = args[5];
            int topTrans = args[6];
            int transColor = args[7];
            int red = args[8];
            int green = args[9];
            int blue = args[10];
            int chroma = args[11];
            int effect = args[12];
            int effectValue = args[13];
            int duration = args[14];
            bool wait = (args[15] == 1);
            int bottomTrans = args.Length > 14? args[14] : 0; // 2003
            bool picNumByVariable = args.Length > 17? (args[17] == 1) : false; // 2003 English
            // 18 and 19 not used in Move Picture
            bool zoomByVariable = args.Length > 20? (args[20] == 1) : false; // 2003 English
            bool topTransByVariable = args.Length > 21? (args[21] == 1) : false; // 2003 English
            
            bool extended = args.Length > 14;
            
            string picStr = "";
            if (!picNumByVariable)
                picStr = picNum.ToString();
            else
                picStr = "Variable " + M.getDataVariable(picNum);
            
            string positionStr = "";
            if (!positionByVariable)
                positionStr = "(" + x + "," + y + ")";
            else
                positionStr = "Variable " + M.getDataVariable(x) + ", Variable " + M.getDataVariable(y);
            
            string zoomStr = "";
            if (!zoomByVariable)
                zoomStr = zoom.ToString();
            else
                zoomStr = "Variable " + M.getDataVariable(zoom);
            
            string effectStr = "";
            if (effect == 1)
                effectStr = ", Rotate Speed " + effectValue;
            else if (effect == 2)
                effectStr = ", Ripple Power " + effectValue;
            
            string transparencyStr = "";
            if (!extended)
                transparencyStr = "Transparency " + topTrans + "%";
            else
            {
                string topTransStr = !topTransByVariable? topTrans.ToString() : "Variable " + M.getDataVariable(topTrans);
                string bottomTransStr = !topTransByVariable? bottomTrans.ToString() : "Variable " + M.getDataVariable(bottomTrans);
                if (!topTransStr.Equals(bottomTransStr))
                    transparencyStr = "Top Transparency " + topTransStr + "%, Bottom Transparency " + bottomTransStr + "%";
                else
                    transparencyStr = "Transparency " + topTransStr + "%";
            }
            
            float sec = duration / 10f;
            
            return "Move Picture: "
                + picStr + ", "
                + positionStr + ", "
                + (followMap? "Follow Map, " : "")
                + "Zoom " + zoomStr + "%, "
                + transparencyStr + ", "
                + (transColor == 1? "Stir" : "No") + " Trans. Color, "
                + "R" + red + " G" + green + " B" + blue + " S" + chroma
                + effectStr + ", "
                + sec + " sec"
                + (wait? ", Wait" : "");
        }
        
        string command11130ErasePicture() // d6 7a
        {
            int picNum = args[0];
            int idType = args.Length > 1? args[1] : 0; // 2003 English
            int max = args.Length > 2? args[2] : 0; // 2003 English
            
            return "Erase Picture: " + picNum;
        }
        
        string command11210ShowAnimation() // d7 4a
        {
            int anim = args[0];
            int target = args[1];
            bool wait = (args[2] == 1);
            bool fullScreen = (args[3] == 1);
            
            string targetName = M.getTargetEvent(target);
            
            return "Show Animation: " + M.getDataAnimation(anim) + ", "
                + targetName
                + (wait? ", Wait" : "")
                + (fullScreen? ", Full Screen" : "");
        }
        
        string command11310SetOpacity() // d8 2e
        {
            int which = args[0];
            
            return "Set Hero Opacity: " + (which == 0? "Transparent" : "Normal");
        }
        
        string command11320FlashEvent() // d8 38
        {
            int target = args[0];
            int red = args[1];
            int green = args[2];
            int blue = args[3];
            int power = args[4];
            int time = args[5];
            bool wait = (args[6] == 1);
            
            string targetName = M.getTargetEvent(target);
            
            float sec = time / 10f;
            
            return "Flash Event: "
                + targetName + ", "
                + "R" + red + " G" + green + " B" + blue + ", "
                + "Power " + power + ", "
                + sec + " sec"
                + (wait? ", Wait" : "");
        }
        
        string command11330MoveEvent() // d8 42
        {
            string targetName = M.getTargetEvent(moveRouteTarget);
            string allMovesStr = moveRoute.getString();
            
            return "Move Event: "
                + targetName + ", "
                + "Frequency " + moveRouteFreq
                + (moveRouteRepeat? ", Repeat" : "")
                + (moveRouteSkip? ", Skip Impossible" : "")
                + (allMovesStr != ""? Environment.NewLine + allMovesStr : "");
        }
        
        string command11340MoveAll() // d8 4c
        {
            return "Move All";
        }
        
        string command11350StopAll() // d8 56
        {
            return "Stop All";
        }
        
        string command11410Wait() // d9 12
        {
            int time = args[0];
            bool waitForKey = args.Length > 1? (args[1] == 1) : false; // 2003
            
            float sec = time / 10f;
            
            if (!waitForKey)
                return "Wait: " + sec + " sec";
            else
                return "Wait for Key Press";
        }
        
        string command11510Music() // d9 76
        {
            int fade = args[0];
            int volume = args[1];
            int tempo = args[2];
            int balance = args[3];
            
            return "Play Music: " + stringArg + ", Fade " + (fade / 1000) + " sec, "
                + M.getSoundVTB(volume, tempo, balance);
        }
        
        string command11520FadeMusic() // da 00
        {
            int fade = args[0];
            
            return "Fade Out BGM: " + (fade / 1000) + " sec";
        }
        
        string command11530RememberMusic() // da 0a
        {
            return "Remember BGM";
        }
        
        string command11540RestoreMusic() // da 14
        {
            return "Restore BGM";
        }
        
        string command11550Sound() // da 1e
        {
            int volume = args[0];
            int tempo = args[1];
            int balance = args[2];
            
            return "Play Sound: " + stringArg + ", " + M.getSoundVTB(volume, tempo, balance);
        }
        
        string command11560Movie() // da 28
        {
            int posType = args[0];
            int x = args[1];
            int y = args[2];
            int width = args[3];
            int height = args[4];
            
            string posString = "";
            if (posType == 0)
                posString = "(" + x + "," + y + ")";
            else if (posType == 1)
                posString = "Variable " + M.getDataVariable(x) + ", Variable " + M.getDataVariable(y);
            
            return "Play Movie: " + stringArg + ", "
                + posString + ", " + width + "x" + height;
        }
        
        string command11610KeyInput() // da 5a
        {
            int variable = args[0];
            bool wait = (args[1] == 1);
            bool dir = (args[2] == 1);
            bool confirm = (args[3] == 1);
            bool cancel = (args[4] == 1);
            bool numKeys = args.Length > 5? (args[5] == 1) : false; // 2003
            bool opKeys = args.Length > 6? (args[6] == 1) : false; // 2003
            int timeVariable = args.Length > 7? args[7] : 1; // 2003
            bool useTimeVariable = args.Length > 8? (args[8] == 1) : false; // 2003
            bool shift = args.Length > 9? (args[9] == 1) : false; // 2003
            bool down = args.Length > 10? (args[10] == 1) : dir; // 2003
            bool left = args.Length > 11? (args[11] == 1) : dir; // 2003
            bool right = args.Length > 12? (args[12] == 1) : dir; // 2003
            bool up = args.Length > 13? (args[13] == 1) : dir; // 2003
            
            bool extended = args.Length > 5;
            
            if (!extended)
                return "Key Input: Variable " + M.getDataVariable(variable)
                    + (wait? ", Wait" : "")
                    + (dir? ", Allow Dirs (1-4)" : "")
                    + (confirm? ", Confirm (5)" : "")
                    + (cancel? ", Cancel (6)" : "");
            else
                return "Key Input: Variable " + M.getDataVariable(variable)
                    + (useTimeVariable? ", Time To Press in Variable " + M.getDataVariable(timeVariable) : "")
                    + (wait? ", Wait" : "")
                    + (down? ", Down (1)" : "")
                    + (left? ", Left (2)" : "")
                    + (right? ", Right (3)" : "")
                    + (up? ", Up (4)" : "")
                    + (confirm? ", Confirm (5)" : "")
                    + (cancel? ", Cancel (6)" : "")
                    + (shift? ", Shift (7)" : "")
                    + (numKeys ? ", Numbers 0-9 (10-19)" : "")
                    + (opKeys ? ", +-*/. Keys (20-24)" : "");
        }
        
        string command11710ChangeChipSet() // db 3e
        {
            int chipset = args[0];
            
            return "Change ChipSet: " + M.getDataChipSet(chipset);
        }
        
        string command11720ChangeParallax() // db 48
        {
            bool horz = (args[0] == 1);
            bool vert = (args[1] == 1);
            bool horzAuto = (args[2] == 1);
            int horzSpeed = args[3];
            bool vertAuto = (args[4] == 1);
            int vertSpeed = args[5];
            
            return "Change Parallax: " + stringArg
                + (horz? (", Horz. Loop" + (horzAuto? " (" + horzSpeed + ")" : "")) : "")
                + (vert? (", Vert. Loop" + (vertAuto? " (" + vertSpeed + ")" : "")) : "");
        }
        
        string command11740ChangeEncounter() // db 5c
        {
            int steps = args[0];
            
            return "Change Encounter Rate: " + steps;
        }
        
        string command11750ChangeChip() // db 66
        {
            int layer = args[0];
            int chip1 = args[1];
            int chip2 = args[2];
            
            return "Change Chip: "
                + (layer == 0? "Lower" : "Upper") + ", " + (chip1 + 1) + ", " + (chip2 + 1);
        }
        
        string command11810SetTeleport() // dc 22
        {
            bool delete = (args[0] == 1);
            int map = args[1];
            int x = args[2];
            int y = args[3];
            bool useSwitch = (args[4] == 1);
            int switchID = args[5];
            
            string place = "";
            if (!delete)
                place = "Add Map " + M.getDataMap(map) + " (" + x + "," + y + ")";
            else
                place = "Delete Map " + M.getDataMap(map);
            
            return "Set Teleport: " + place
                + (useSwitch? ", Switch " + M.getDataSwitch(switchID) + " On" : "");
        }
        
        string command11820DisableTeleport() // dc 2c
        {
            bool enable = (args[0] == 1);
            
            return "Allow Teleport: " + (enable? "Enable" : "Disable");
        }
        
        string command11830SetEscape() // dc 36
        {
            int map = args[0];
            int x = args[1];
            int y = args[2];
            bool useSwitch = (args[3] == 1);
            int switchID = args[4];
            
            return "Set Escape: " + M.getDataMap(map) + " (" + x + "," + y + ")"
                + (useSwitch? ", Switch " + M.getDataSwitch(switchID) + " On" : "");
        }
        
        string command11840DisableEscape() // dc 40
        {
            bool enable = (args[0] == 1);
            
            return "Allow Escape: " + (enable? "Enable" : "Disable");
        }
        
        string command11910CallSave() // dd 06
        {
            return "Call Save Menu";
        }
        
        string command11930DisableSave() // dd 1a
        {
            bool enable = (args[0] == 1);
            
            return "Allow Save: " + (enable? "Enable" : "Disable");
        }
        
        string command11950CallMenu() // dd 2e
        {
            return "Call System Menu";
        }
        
        string command11960DisableMenu() // dd 38
        {
            bool enable = (args[0] == 1);
            
            return "Allow System Menu: " + (enable? "Enable" : "Disable");
        }
        
        string command12010Fork() // dd 6a
        {
            int type = args[0];
            int v1 = args[1];
            int v2 = args[2];
            int v3 = args[3];
            int v4 = args[4];
            bool elseCase = (args[5] == 1);
            
            string condition = "";
            if (type == 0) // Switch
            {
                // v1: Switch, v2: On/Off
                condition = "Switch " + M.getDataSwitch(v1) + " is " + (v2 == 0? "On" : "Off");
            }
            else if (type == 1) // Variable
            {
                // v1: Variable 1, v2: Fixed or Variable?, v3: Variable 2/Value, v4: Comparison
                string rightSide = (v2 == 0? v3.ToString() : "Variable " + M.getDataVariable(v3));
                condition = "Variable " + M.getDataVariable(v1) + " " + comparisons[v4] + " " + rightSide;
            }
            else if (type == 2) // Timer
            {
                // v1: Time, v2: Above/Below
                condition = "Timer " + (v2 == 0? "Above" : "Below") + " " + (v1 / 60) + " min " + (v1 % 60) + " sec";
            }
            else if (type == 3) // Money
            {
                // v1: Value, v2: Above/Below
                condition = "Money " + (v2 == 0? "Above" : "Below") + " " + v1;
            }
            else if (type == 4) // Item
            {
                // v1: Item, v2: Owned/Not
                condition = M.getDataItem(v1) + " " + (v2 == 0? "Owned" : "Not Owned");
            }
            else if (type == 5) // Hero
            {
                // v1: Hero, v2: Subcondition, v3: Subcondition Value
                condition = M.getDataHero(v1);
                if (v2 == 0)
                    condition += " is In The Party";
                else if (v2 == 1)
                    condition += " Name = " + (M.includeMessages? stringArg : "___");
                else if (v2 == 2)
                    condition += " Level " + v3 + " or More";
                else if (v2 == 3)
                    condition += " " + v3 + " HP or More";
                else if (v2 == 4)
                    condition += " Knows " + M.getDataSkill(v3);
                else if (v2 == 5)
                    condition += " Equipped With " + M.getDataItem(v3);
                else if (v2 == 6)
                    condition += " Has " + M.getDataCondition(v3);
            }
            else if (type == 6) // Event
            {
                // v1: Event, v2: Direction
                condition = M.getTargetEvent(v1) + " is Facing " + Page.charDirs[v2];
            }
            else if (type == 7) // Vehicle
            {
                // v1: Vehicle
                condition = "Riding " + (v1 == 0? "Boat" : v1 == 1? "Ship" : "Airship");
            }
            else if (type == 8)
                condition = "Started by Action Key";
            else if (type == 9)
                condition = "BGM Played Once";
            else if (type == 10) // Timer 2 (2003)
            {
                // v1: Time, v2: Above/Below
                condition = "Timer 2 " + (v2 == 0? "Above" : "Below") + " " + (v1 / 60) + " min " + (v1 % 60) + " sec";
            }
            else if (type == 11) // Miscellaneous (2003 English)
            {
                // v1: Quality
                condition = v1 == 0? "Save Available" : v1 == 1? "In Test Play" : v1 == 2? "ATB Wait On" : v1 == 3? "In Fullscreen" : "";
            }
            
            return "Condition: " + condition + (elseCase? " (With Else)" : "");
        }
        
        string command22010Else() // 81 ab 7a
        {
            return "Else";
        }
        
        string command22011ForkEnd() // 81 ab 7b
        {
            return "Condition End";
        }
        
        string command12110Label() // de 4e
        {
            int label = args[0];
            
            return "Label: " + label;
        }
        
        string command12120LabelJump() // de 58
        {
            int label = args[0];
            
            return "Go To Label: " + label;
        }
        
        string command12210Cycle() // df 32
        {
            return "Cycle Start";
        }
        
        string command12220CycleBreak() // df 3c
        {
            return "Break Cycle";
        }
        
        string command22210CycleEnd() // 81 ad 42
        {
            return "Cycle End";
        }
        
        string command12310StopParallel() // e0 16
        {
            return "Stop Parallel Events";
        }
        
        string command12320EraseEvent() // e0 20
        {
            return "Erase Event";
        }
        
        string command12330CallEvent() // e0 2a
        {
            int type = args[0];
            int value1 = args[1];
            int value2 = args[2];
            
            string eventStr = "";
            if (type == 0) // Common
                eventStr = M.getDataCommon(value1);
            else if (type == 1) // Fixed Event
                eventStr = M.getTargetEvent(value1) + " Page " + value2;
            else if (type == 2) // Variable Event
                eventStr = "Event in Variable " + M.getDataVariable(value1) + ", Page in Variable " + M.getDataVariable(value2);
            
            return "Call Event: " + eventStr;
        }
        
        string command12410Comment() // e0 7a
        {
            return "Comment: " + stringArg;
        }
        
        string command22410CommentFollow() // 81 af 0a
        {
            return "(Comment): " + stringArg;
        }
        
        string command12420GameOver() // e1 04
        {
            return "Game Over";
        }
        
        string command12510Title() // e1 5e
        {
            return "Return To Title";
        }
        
        string command13110ChangeEnemyHP() // e6 36
        {
            int target = args[0];
            bool subtract = (args[1] == 1);
            bool byVariable = (args[2] == 1);
            int value = args[3];
            bool canDie = (args[4] == 1);
            
            return "Change Enemy HP: "
                + "Enemy " + (target + 1) + ", "
                + (subtract? "Subtract" : "Add") + " "
                + (!byVariable? value.ToString() : "Variable " + M.getDataVariable(value))
                + (subtract && canDie? ", Can Cause Death" : "");
        }
        
        string command13120ChangeEnemyMP() // e6 40
        {
            int target = args[0];
            bool subtract = (args[1] == 1);
            bool byVariable = (args[2] == 1);
            int value = args[3];
            
            return "Change Enemy MP: "
                + "Enemy " + (target + 1) + ", "
                + (subtract? "Subtract" : "Add") + " "
                + (!byVariable? value.ToString() : "Variable " + M.getDataVariable(value));
        }
        
        string command13130ChangeEnemyState() // e6 4a
        {
            int target = args[0];
            bool cure = (args[1] == 1);
            int condition = args[2];
            
            return "Change State: "
                + "Enemy " + (target + 1) + ", "
                + (cure? "Cure" : "Inflict") + " "
                + M.getDataCondition(condition);
        }
        
        string command13150EnemyAppear() // e6 5e
        {
            int target = args[0];
            
            return "Enemy Appear: Enemy " + (target + 1);
        }
        
        string command13210ChangeBattleBG() // e7 1a
        {
            return "Change Battle BG: " + stringArg;
        }
        
        string command13260BattleAnimation() // e7 4c
        {
            int animation = args[0];
            int target = args[1];
            bool wait = (args[2] == 1);
            
            return "Show Battle Animation: " + M.getDataAnimation(animation) + ", "
                + (target == -1? "All Enemies" : "Enemy " + (target + 1)
                + (wait? ", Wait" : ""));
        }
        
        string command13310BattleFork() // e7 7e
        {
            int type = args[0];
            int v1 = args[1];
            int v2 = args[2];
            int v3 = args[3];
            int v4 = args[4];
            bool elseCase = (args[5] == 1);
            
            string condition = "";
            if (type == 0) // Switch
            {
                // v1: Switch, v2: On/Off
                condition = "Switch " + M.getDataSwitch(v1) + " is " + (v2 == 0? "On" : "Off");
            }
            else if (type == 1) // Variable
            {
                // v1: Variable 1, v2: Fixed or Variable?, v3: Variable 2/Value, v4: Comparison
                string rightSide = (v2 == 0? v3.ToString() : "Variable " + M.getDataVariable(v3));
                condition = "Variable " + M.getDataVariable(v1) + " " + comparisons[v4] + " " + rightSide;
            }
            else if (type == 2) // Hero Can Act
            {
                // v1: Hero
                condition = M.getDataHero(v1) + " Can Act";
            }
            else if (type == 3) // Monster Can Act
            {
                // v1: Monster
                condition = "Enemy " + (v1 + 1) + " Can Act";
            }
            else if (type == 4) // Monster Is Target
            {
                // v1: Monster
                condition = "Enemy " + (v1 + 1) + " is Current Target";
            }
            else if (type == 5) // Hero Uses Command
            {
                // v1: Hero, v2: Command
                condition = M.getDataHero(v1) + " Uses Command " + M.getDataBattleCommand(v2);
            }
            
            return "Condition: " + condition + (elseCase? " (With Else)" : "");
        }
        
        string command23310BattleElse() // 81 b6 0e
        {
            return "Else";
        }
        
        string command23311BattleForkEnd() // 81 b6 0f
        {
            return "Condition End";
        }
        
        string command13410StopBattle() // e8 62
        {
            return "Stop Battle";
        }
        
        // Replaces stringArg. Returns whether the new one differs from the original.
        public bool setStringArg(string str)
        {
            if (opcode == C_SHOWSTRINGPICTURE) // Account for 0x01s at start and end
                str = (char)0x01 + str.Replace("\r\n", "\n") + (char)0x01 + (char)0x01;
            
            bool changed = !stringArg.Equals(str);
            stringArg = str;
            return changed;
        }
        
        // Sets trueChoices after determining them from cases.
        public void setTrueChoices(string[] choices)
        {
            trueChoices = choices;
        }
        
        // Writes command data.
        override protected void myWrite()
        {
            M.writeMultibyte(opcode);
            M.writeMultibyte(indent);
            
            if (opcode != C_MOVEEVENT)
            {
                int mode = getMode();
                int strType = mode != -1 && mode < M.FOLDERCOUNT? M.S_FILENAME : M.S_TOTRANSLATE;
                
                M.writeString(stringArg, strType);
                M.writeMultibyte(args.Length);
                for (int i = 0; i < args.Length; i++)
                    M.writeMultibyte(args[i]);
            }
            else
            {
                M.writeByte(0x00);
                
                int totalMoveLength = moveRoute.getLength()
                                    + 1 // Target length, but it's always 1 regardless, I think...
                                    + 3; // Frequency, repeat, skip
                
                M.writeMultibyte(totalMoveLength);
                
                M.writeMultibyte(moveRouteTarget);
                M.writeByte(moveRouteFreq);
                M.writeByte(moveRouteRepeat? 1 : 0);
                M.writeByte(moveRouteSkip? 1 : 0);
                
                moveRoute.write();
            }
        }
        
        // Updates the state of messageFaceOn depending on the opcode of this command.
        public void updateMessageFaceOn()
        {
            if (opcode == C_CHANGEFACE)
                M.messageFaceOn = stringArg != "";
            else if (opcode == C_ELSE)
                M.messageFaceOn = false;
        }
        
        // Replaces stringArg if it references a file.
        public void replaceFilenames()
        {
            int mode = getMode();
            if (mode != -1 && mode < M.FOLDERCOUNT) // Filename
                stringArg = M.rewriteString(mode, stringArg);
        }
        
        // Returns appropriate string handling mode for stringArg based on opcode.
        int getMode()
        {
            int mode = -1;
            
            if (isMessageStart() || (isMessageFollowOrExtraBox() && M.messagePreceded) // Don't rewrite MessageFollows left "hanging" with no Message
             || opcode == C_SHOWSTRINGPICTURE)
                mode = M.M_MESSAGEALL;
            else if (opcode == C_COMMENT || opcode == C_COMMENTFOLLOW)
                mode = M.M_COMMENT;
            else if (opcode == C_CHARSETCHANGE || opcode == C_VEHICLEGRAPHIC)
                mode = M.M_CHARSET;
            else if (opcode == C_FACESETCHANGE || opcode == C_CHANGEFACE)
                mode = M.M_FACESET;
            else if (opcode == C_MUSIC || opcode == C_SYSTEMBGMCHANGE)
                mode = M.M_MUSIC;
            else if (opcode == C_SOUND || opcode == C_SYSTEMSOUNDCHANGE)
                mode = M.M_SOUND;
            else if (opcode == C_SYSTEMGRAPHICSCHANGE)
                mode = M.M_SYSTEM;
            else if (opcode == C_PICTURE)
                mode = M.M_PICTURE;
            else if (opcode == C_MOVIE)
                mode = M.M_MOVIE;
            else if (opcode == C_CHANGEPARALLAX)
                mode = M.M_PANORAMA;
            else if (opcode == C_BATTLESTART || opcode == C_CHANGEBATTLEBG)
                mode = M.M_BACKDROP;
            else if (opcode == C_CHOICE || opcode == C_CHOICECASE)
                mode = M.M_OPTION;
            else if (opcode == C_NAMECHANGE || opcode == C_FORK)
                mode = M.M_NAME;
            else if (opcode == C_TITLECHANGE)
                mode = M.M_NICKNAME;
            
            return mode;
        }
        
        // Returns the indent.
        public int getIndent()
        {
            return indent;
        }
        
        // Returns the stringArg.
        public string getStringArg()
        {
            string returnString = stringArg;
            if (opcode == C_SHOWSTRINGPICTURE) // Account for 0x01s at start and end
                returnString = stringArg.Substring(1, stringArg.Length - 3).Replace("\r\n", "\n").Replace("\n", "\r\n");
            return returnString;
        }
        
        // Returns trueChoices array.
        public string[] getTrueChoices()
        {
            return trueChoices;
        }
        
        // Returns whether the command is the ending command (opcode 0).
        public bool isEndCommand()
        {
            return opcode == 0;
        }
        
        // Returns whether the command is a textbox-starting Message command, not a Message Follow or an extra box with a dummy argument added by the program.
        public bool isMessageStart()
        {
            return (opcode == C_MESSAGE && (args == null || args.Length == 0 || args[0] != DUMMYMESSAGEARG)) // Message command not marked as "extra box"
                || isRemovedMessage(); // Blank command marked as removed message
        }
        
        // Returns whether the command is a Message Follow command, or an extra box added by the program.
        public bool isMessageFollowOrExtraBox()
        {
            return opcode == C_MESSAGEFOLLOW || (opcode == C_MESSAGE && args != null && args.Length > 0 && args[0] == DUMMYMESSAGEARG);
        }
        
        // Returns whether the command is a blank command marked as a removed Message.
        public bool isRemovedMessage()
        {
            return opcode == C_BLANK && args != null && args.Length > 0 && args[0] == DUMMYMESSAGEARG;
        }
        
        // Returns whether the command is a Choice command.
        public bool isChoice()
        {
            return opcode == C_CHOICE;
        }
        
        // Returns whether the command is a Choice Case command.
        public bool isChoiceCase()
        {
            return opcode == C_CHOICECASE;
        }
        
        // Returns whether the command is a Choice Case, but not the Cancel Case.
        public bool isChoiceCaseNonCancel()
        {
            return opcode == C_CHOICECASE && args[0] != 4;
        }
        
        // Returns whether the command is an End Case command.
        public bool isEndCase()
        {
            return opcode == C_ENDCASE;
        }
        
        // Returns whether the command is a Name Change command.
        public bool isNameChange()
        {
            return opcode == C_NAMECHANGE;
        }
        
        // Returns whether the command is a Title Change command.
        public bool isTitleChange()
        {
            return opcode == C_TITLECHANGE;
        }
        
        // Returns whether the command is a Fork command that checks a hero's name.
        public bool isNameFork()
        {
            if (opcode == C_FORK && args.Length > 2)
                return args[0] == 5 && args[2] == 1;
            else
                return false;
        }
        
        // Returns whether the command is a Show String Picture command.
        public bool isStringPicture()
        {
            return opcode == C_SHOWSTRINGPICTURE;
        }
        
        // Returns whether the command is a blank command (with or without a marking argument).
        public bool isBlank()
        {
            return opcode == C_BLANK;
        }
    }
}
