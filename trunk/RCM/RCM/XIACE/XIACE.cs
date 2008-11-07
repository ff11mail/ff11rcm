﻿/* XIACE - Memory Access Provider for FFXI.
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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace FFXI.XIACE
{
    /// <summary>
    /// 各種オフセット
    /// </summary>
    internal enum OFFSET
    {
        TARGET_INFO = 0x4d622c,
        PLAYER_INFO = 0x8ccb6c, // 名前,HPMPTP,エリアなど
        ACTIVITY_INFO = 0x536790  // 行動状態 enum Activity を参照
    }

    /// <summary>
    /// 行動状態
    /// </summary>
    public enum eActivity : byte
    {
        Standing = 0,
        Fighting = 1,
        Dead1 = 2,
        Dead2 = 3,
        CutScene = 4,
        Chocobo = 5,
        Fishing = 6,
        Healing = 33,
        FishBite = 38,
        Obtained = 39,
        RodBreak = 40,
        LineBreak = 41,
        LostCatch = 43,
        CatchMonster = 42,
        Synthing = 44,
        Sitting = 47
    }

    /// <summary>
    /// POL プロセス情報
    /// </summary>
    internal class PolProcess
    {
        private Process _Process;
        private IntPtr _BaseAddress;

        public IntPtr Handle { get { return _Process.Handle; } }
        public IntPtr BaseAddress { get { return _BaseAddress; } }

        public PolProcess(Process Proc)
        {
            _Process = Proc;
            foreach (ProcessModule m in Proc.Modules)
            {
                if (m.ModuleName == "FFXiMain.dll")
                {
                    _BaseAddress = m.BaseAddress;
                    break;
                }
            }
        }        
    }

    /// <summary>
    /// 実際にメモリにアクセスするスタティッククラス
    /// </summary>
    internal static class MemoryProvider
    {

        /// <summary>
        /// ポインタ型をつかうunsafeメソッド : ポインタから配列に突っ込む(memcpy的な)ときにfixed ステートメントが必要
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        /// <param name="nBufferSize"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        unsafe public extern static bool ReadProcessMemory(IntPtr handle, IntPtr address,
            void* buffer, uint nBufferSize, int* len);

        /// <summary>
        /// ポインタ型を使わないメソッド : Marshal.PtrTo*をつかってがんばれ
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="addr"></param>
        /// <param name="OutputBuffer"></param>
        /// <param name="nBufferSize"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public extern static bool ReadProcessMemorySafe(IntPtr handle, IntPtr addr, IntPtr OutputBuffer, UIntPtr nBufferSize, out UIntPtr lpNumberOfBytesRead);


        public static IntPtr ReadProcessMemorySafe(IntPtr Handle, IntPtr Address, uint nBytesToRead)
        {
            IntPtr Buffer = Marshal.AllocHGlobal((int)nBytesToRead);
            UIntPtr BytesRead = UIntPtr.Zero;
            UIntPtr BytesToRead = (UIntPtr)nBytesToRead;
            if (!ReadProcessMemorySafe(Handle, Address, Buffer, BytesToRead, out BytesRead))
            {
                return IntPtr.Zero;
            }
            return Buffer;
        }

        public static string ReadMemoryString(IntPtr handle, IntPtr addr, uint size)
        {
            IntPtr Buffer = IntPtr.Zero;
            string str = string.Empty;
            Buffer = ReadProcessMemorySafe(handle, addr, size);
            try
            {
                str = Marshal.PtrToStringAnsi(Buffer, (int)size);
                str = str.Trim('\0');
            }
            finally
            {
                Marshal.FreeHGlobal(Buffer);
            }
            return str;
        }

        public static int ReadMemoryInt32(IntPtr handle, IntPtr addr)
        {
            IntPtr Buffer = IntPtr.Zero;
            int i = 0;
            Buffer = ReadProcessMemorySafe(handle, addr, 4);
            try
            {
                i = Marshal.ReadInt32(Buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(Buffer);
            }
            return i;
        }

        public static short ReadMemoryInt16(IntPtr handle, IntPtr addr)
        {
            IntPtr Buffer = IntPtr.Zero;
            short i = 0;
            Buffer = ReadProcessMemorySafe(handle, addr, 2);
            try
            {
                i = Marshal.ReadInt16(Buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(Buffer);
            }
            return i;
        }

        public static byte ReadMemoryByte(IntPtr handle, IntPtr addr)
        {
            IntPtr Buffer = IntPtr.Zero;
            byte ret;
            Buffer = ReadProcessMemorySafe(handle, addr, 1);
            try
            {
                ret = Marshal.ReadByte(Buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(Buffer);
            }
            return ret;
        }
    }
}
