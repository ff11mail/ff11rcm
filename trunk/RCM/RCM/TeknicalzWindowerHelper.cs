// Windower Helper Class By: T3knicalD3ath
// Version 1 Beta

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FFXI
{

    public class Windower
    {
        #region "MainFunctions"

        public class MainFunctions
        {
            //Keyboard Helper: (MMF Name = "WindowerMMFKeyboardHandler")
            private  int mKeyboardHelper;
            //Text Helper: (MMF Name = "WindowerMMFTextHandler")
            private  int mTextHelper;
            //Console Helper:
            private  int mConsoleHandler;
            //This Here Is Just A Simple Delay To Know How Long To Hold The Key Down For
            private static int KeyDownDelay = 100;

            public void SetPID(uint PID)
            {
                if (mTextHelper != 0)
                DeleteTextHelper(mTextHelper);
                if(mKeyboardHelper != 0)
                DeleteKeyboardHelper(mKeyboardHelper);
                if(mConsoleHandler != 0)
                DeleteConsoleHelper(mConsoleHandler);
                mTextHelper = CreateTextHelper("WindowerMMFTextHandler_" + PID.ToString());
                mKeyboardHelper = CreateKeyboardHelper("WindowerMMFKeyboardHandler_" + PID.ToString());
                mConsoleHandler = CreateConsoleHelper("WindowerMMFConsoleHandler_" + PID.ToString());
            }

            public void useLegacy()
            {
                DeleteTextHelper(mTextHelper);
                DeleteKeyboardHelper(mKeyboardHelper);
                DeleteConsoleHelper(mConsoleHandler);
                mTextHelper = CreateTextHelper("WindowerMMFTextHandler");
                mKeyboardHelper = CreateKeyboardHelper("WindowerMMFKeyboardHandler");
                mConsoleHandler = CreateConsoleHelper("WindowerMMFConsoleHandler");
            }

            [DllImport("WindowerHelper.dll")]
            private static extern int CreateTextHelper(string name);

            [DllImport("WindowerHelper.dll")]
            private static extern void DeleteTextHelper(int helper);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHCreateTextObject(int helper, string name);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHDeleteTextObject(int helper, string name);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetText(int helper, string name, string text);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetVisibility(int helper, string name, bool visible);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetFont(int helper, string name, ref byte font, short height);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetColor(int helper, string name, byte a, byte r, byte g, byte b);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetLocation(int helper, string name, float x, float y);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetBold(int helper, string name, bool bold);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetItalic(int helper, string name, bool italic);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetBGColor(int helper, string name, byte a, byte r, byte g, byte b);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetBGBorderSize(int helper, string name, float pixels);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetBGVisibility(int helper, string name, bool visible);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHSetRightJustified(int helper, string name, bool justified);

            [DllImport("WindowerHelper.dll")]
            private static extern void CTHFlushCommands(int helper);

            [DllImport("WindowerHelper.dll")]
            private static extern int CreateKeyboardHelper(string name);

            [DllImport("WindowerHelper.dll")]
            private static extern void DeleteKeyboardHelper(int helper);

            [DllImport("WindowerHelper.dll")]
            private static extern void CKHSetKey(int helper, byte key, bool down);

            [DllImport("WindowerHelper.dll")]
            private static extern void CKHBlockInput(int helper, bool block);

            [DllImport("WindowerHelper.dll")]
            private static extern void CKHSendString(int helper, string data);

            [DllImport("WindowerHelper.dll")]
            private static extern int CreateConsoleHelper(string name);

            [DllImport("WindowerHelper.dll")]
            private static extern void DeleteConsoleHelper(int helper);

            [DllImport("WindowerHelper.dll")]
            private static extern bool CCHIsNewCommand(int helper);

            [DllImport("WindowerHelper.dll")]
            private static extern short CCHGetArgCount(int helper);
            public  short ConsoleGetArgCount()
            {
                return CCHGetArgCount(mConsoleHandler);
            }

            [DllImport("WindowerHelper.dll")]
            private static extern void CCHGetArg(int helper, short index, byte[] buffer);

            public  string ConsoleGetArg(short index)
            {
                byte[] buffer = new byte[256];
                // text = String.Format("{0:255}", " ");
                CCHGetArg(mConsoleHandler, index, buffer);
                var len = 0;
                for (var i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] == 0)
                    {
                        len = i;
                        break;
                    }
                }
                return Encoding.Default.GetString(buffer, 0, len);
            }

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~The Main Functions That Make Life Easy Start Here~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            public  void SendText(string text)
            {
                CKHSendString(mKeyboardHelper, text);
            }

            public  void KeyPress(KeyCode KeyCode)
            {
                CKHSetKey(mKeyboardHelper, Convert.ToByte(KeyCode), true);
                System.Threading.Thread.Sleep(KeyDownDelay);
                CKHSetKey(mKeyboardHelper, Convert.ToByte(KeyCode), false);
            }

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~~~~~~~~~~~~~~Works But I Don't Use It~~~~~~~~~~~~~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            public  void KeyDown(KeyCode KeyCode, bool down) //to use set private to public
            {
                CKHSetKey(mKeyboardHelper, Convert.ToByte(KeyCode), down);
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        }

        #endregion

        #region "KeyCodes"

        public enum KeyCode
        {
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~~~~~~~~These Here Are The Most Important FFXI Keys~~~~~~~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            EscapeKey = 1,

            EnterKey = 28,
            TabKey = 15,

            UpArrow = 200,
            LeftArrow = 203,
            RightArrow = 205,
            DownArrow = 208,
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~~~~~~These Here Are The NumPad Keys On Your Keyboard~~~~~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            MainNumlockKey = 69,

            NP_Number0 = 82,
            NP_Number1 = 79,
            NP_Number2 = 80,
            NP_Number3 = 81,
            NP_Number4 = 75,
            NP_Number5 = 76,
            NP_Number6 = 77,
            NP_Number7 = 71,
            NP_Number8 = 72,
            NP_Number9 = 73,

            NP_Forwardslash = 181,
            NP_MultiplyKey = 55,
            NP_MinusKey = 74,
            NP_AdditionKey = 78,
            NP_EnterKey = 156,
            NP_PeriodKey = 83,
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~~These Here Are The Letters From A to Z On Your Keyboard~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            LetterA = 30,
            LetterB = 48,
            LetterC = 46,
            LetterD = 32,
            LetterE = 18,
            LetterF = 33,
            LetterG = 34,
            LetterH = 35,
            LetterI = 23,
            LetterJ = 36,
            LetterK = 37,
            LetterL = 38,
            LetterM = 50,
            LetterN = 49,
            LetterO = 24,
            LetterP = 25,
            LetterQ = 16,
            LetterR = 19,
            LetterS = 31,
            LetterT = 20,
            LetterU = 22,
            LetterV = 47,
            LetterW = 17,
            LetterX = 45,
            LetterY = 21,
            LetterZ = 44,
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~~These Here Are The Numbers From 0 to 9 On Your Keyboard~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            Number1 = 2,
            Number2 = 3,
            Number3 = 4,
            Number4 = 5,
            Number5 = 6,
            Number6 = 7,
            Number7 = 8,
            Number8 = 9,
            Number9 = 10,
            Number0 = 11,
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~These Here Are The F Keys From F1 to F12 On Your Keyboard~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            F1Key = 59,
            F2Key = 60,
            F3Key = 61,
            F4Key = 62,
            F5Key = 63,
            F6Key = 64,
            F7Key = 65,
            F8Key = 66,
            F9Key = 67,
            F10Key = 68,
            F11Key = 87,
            F12Key = 88,
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~~~These Here Are Ones That You Should Not Need But Here~~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //MinusKey = 12,
            //EqualsKey = 13,
            //BackspaceKey = 14,
            //LeftBracket = 26,
            //RightBracket = 27,
            //LeftCtrlKey = 29,
            //Semicolon = 39,
            //Apostrophe = 40,
            //Accentgrave = 41,
            //LeftShift = 42,
            //Backslash = 43,
            //CommaKey = 51,
            //PeriodKey = 52,
            //ForwardslashKEy = 53,
            //RightShift = 54,
            //ScrollLock = 70,
            //LeftAltKey = 56,
            //Spacebar = 57,
            //CapsLock = 58,
            //RightControlKey = 157,
            //RightAltKey = 184,
            //InsertKey = 210,
            //DeleteKey = 211,
            //LeftWindowKey = 219,
            //RightWindowKey = 220

            //Calculator = &HA1

            //MuteKey = &HA0
            //PlayNPauseKey = &HA2
            //StopMedia = &HA
            //VolumeDown = &HAE
            //VolumeUp = &HB0
            //NextMediaTrack = &HED
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        }

        #endregion
    }
}
