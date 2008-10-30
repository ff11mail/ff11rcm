/* WindowerHelper wrapper class.
 * Copyright (C) 2008 FFXI RCM Project <ff11rcm@gmail.com>
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FFXI
{
    public class Windower
    {
        // Private
        private int ConsoleHelper;
        private int TextHelper;
        private int KeyboardHelper;

        // キーストロークのDOWN UPの間隔 (SendKey用)
        private int KeyPressDelay = 100;

        // Public
        public uint mPid;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pid">対象プロセスID</param>
        public Windower(uint pid)
        {
            mPid = pid;

            ConsoleHelper = WindowerHelper.CreateConsoleHelper("WindowerMMFConsoleHandler_" + pid.ToString());
            TextHelper = WindowerHelper.CreateTextHelper("WindowerMMFTextHandler_" + pid.ToString());
            KeyboardHelper = WindowerHelper.CreateKeyboardHelper("WindowerMMFKeyboardHandler_" + pid.ToString());
        }

        ~Windower()
        {
            Dispose();
        }

        public void Dispose()
        {
            WindowerHelper.DeleteConsoleHelper(ConsoleHelper);
            WindowerHelper.DeleteTextHelper(TextHelper);
            WindowerHelper.DeleteKeyboardHelper(KeyboardHelper);
            mPid = 0;
        }

        /// <summary>
        /// 新規コマンドかどうか (バグってるの常にtrue (つまり使えない))
        /// </summary>
        public bool IsNewCommand
        {
            get { return WindowerHelper.CCHIsNewCommand(ConsoleHelper); }
        }

        /// <summary>
        /// コマンド引数の数
        /// </summary>
        public short ArgCount
        {
            get { return WindowerHelper.CCHGetArgCount(ConsoleHelper); }
        }

        /// <summary>
        /// FFXIチャットラインへテキストを送信 (コマンド可)
        /// </summary>
        /// <param name="text">テキスト</param>
        public void SendText(string text)
        {
            WindowerHelper.CKHSendString(KeyboardHelper, text);
        }

        /// <summary>
        /// キーストロークを送信
        /// </summary>
        /// <param name="code">キーコード</param>
        public void SendKey(WindowerHelper.KeyCode code)
        {
            WindowerHelper.CKHSetKey(KeyboardHelper, Convert.ToByte(code), true);
            Thread.Sleep(KeyPressDelay);
            WindowerHelper.CKHSetKey(KeyPressDelay, Convert.ToByte(code), false);
        }

        /// <summary>
        /// コンソールコマンドの文字列を取得
        /// </summary>
        /// <param name="index">argc index</param>
        /// <returns>文字列</returns>
        public string ConsoleGetArg(short index)
        {
            byte[] buffer = new byte[256]; // いまのところ256バイトのみ
            // text = String.Format("{0:255}", " ");
            WindowerHelper.CCHGetArg(ConsoleHelper, index, buffer);
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
    }
}
