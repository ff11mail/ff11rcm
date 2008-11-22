/* XIACE - Memory Access Provider for FFXI.
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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FFXI.XIACE
{
    public class Inventory
    {
        private PolProcess pol;

        public struct InventoryItem
        {
            public short id;
            public short order;
            public byte count;
        }

        public Inventory(Process proc)
        {
            pol = new PolProcess(proc);
        }

        public Inventory(int pid)
        {
            Process proc = Process.GetProcessById(pid);
            pol = new PolProcess(proc);
        }

        /// <summary>
        /// 装備品のカバンの中での位置
        /// </summary>
        /// <param name="slot">装備スロット</param>
        /// <returns>位置</returns>
        unsafe private byte GetEquipItemPos(eEquipSlot slot)
        {
            byte pos;
            int off = (int)OFFSET.EQUIP_INFO + ((int)slot * 8) + 4;
            MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)((int)pol.BaseAddress + off), &pos, 1, null);
            return pos;
        }

        /// <summary>
        /// アイテム情報の取得
        /// </summary>
        /// <param name="index">位置</param>
        /// <param name="offset">INVENTORY_INFO|SAFEBOX_INFO|STORAGE_INFO|LOCKER_INFO</param>
        /// <returns>InventoryItem 構造体</returns>
        unsafe private InventoryItem _GetInventoryItem (short index, OFFSET offset)
        {
            short id;
            short order;
            byte count;
            int off = (int)offset + index * 0x2c;
            MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)((int)pol.BaseAddress + off), &id, 2, null);
            MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)((int)pol.BaseAddress + off + 2), &order, 2, null);
            MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)((int)pol.BaseAddress + off + 4), &count, 1, null);
            InventoryItem item = new InventoryItem();
            item.id = id;
            item.order = order;
            item.count = count;
            return item;
        }

        /// <summary>
        /// ItemID -> Name をメモリから読む
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        unsafe public string GetItemNameById(short id)
        {
            byte[] name = new byte[32];
            short sid;
            int i = 0;
            while (true)
            {
                // ここの構造体よくわかんないけど1個328バイト,
                // 0バイト目にItemID
                // 104バイト目にアイテム名
                int off = (int)OFFSET.ITEM_INFO + (i++ * 328);
                MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)((int)pol.BaseAddress + off), &sid, 2, null);
                if (sid == 0)
                    break;
                if (sid == id)
                {
                    fixed (byte* buf = name)
                    {
                        MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)((int)pol.BaseAddress + off + 104), buf, 32, null);
                    }
                    return Encoding.Default.GetString(name).Trim('\0');
                }                
            }
            return "";
        }

        /// <summary>
        /// カバンアイテム情報取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public InventoryItem GetInventoryItem(short index)
        {
            return _GetInventoryItem(index, OFFSET.INVENTORY_INFO);
        }

        /// <summary>
        /// 金庫アイテム情報取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public InventoryItem GetSafeboxItem(short index)
        {
            return _GetInventoryItem(index, OFFSET.SAFEBOX_INFO);
        }

        /// <summary>
        /// 収納アイテム取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public InventoryItem GetStorageItem(short index)
        {
            return _GetInventoryItem(index, OFFSET.STORAGE_INFO);
        }

        /// <summary>
        /// ロッカーアイテム取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public InventoryItem GetLockerItem(short index)
        {
            return _GetInventoryItem(index, OFFSET.LOCKER_INFO);
        }

        /// <summary>
        /// 装備アイテム名取得
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public string GetEquippedItemName(eEquipSlot slot)
        {
            byte pos = GetEquipItemPos(slot);
            if ((pos) == 0) return "";
            InventoryItem item = _GetInventoryItem(pos, OFFSET.INVENTORY_INFO);
            if (item.id == 0) return "";
            return GetItemNameById(item.id);
        }

        /// <summary>
        /// 装備してるアイテムの数取得 : 事実上 ammo にしか意味なし
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public byte GetEquippedItemCount(eEquipSlot slot)
        {
            byte pos = GetEquipItemPos(slot);
            if ((pos) == 0) return (byte)0;
            InventoryItem item = _GetInventoryItem(pos, OFFSET.INVENTORY_INFO);
            if (item.id == 0) return (byte)0;
            return (byte)item.count;
        }

        /// <summary>
        /// カバンアイテム数の取得 : リアルタイムじゃない可能性あり
        /// </summary>
        /// <returns></returns>
        unsafe public byte GetInventoryCount()
        {
            int ptr;
            byte count;
            MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)((int)pol.BaseAddress + OFFSET.INVENTORY_COUNT), &ptr, 4, null);
            MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)(ptr + 0x52), &count, 1, null);
            return (byte)(count - 1);
        }

        /// <summary>
        /// カバンにある特定アイテムの数の取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte GetItemCountByIndex(short index)
        {
            InventoryItem item = _GetInventoryItem(index, OFFSET.INVENTORY_INFO);
            return item.count;
        }
    }
}
