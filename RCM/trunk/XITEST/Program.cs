using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using FFXI.XIACE;

namespace XITEST
{
    class Program
    {
        static Process CheckFFXIRunning()
        {
            Process p = Process.GetProcessesByName("pol")[0];
            if (p == null) return null;
            foreach (ProcessModule m in p.Modules)
            {
                if (m.ModuleName.Equals("FFXiMain.dll", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("POL.EXE Pid: {0}, Title: {1}, {2}: 0x{3:X}", p.Id, p.MainWindowTitle, m.ModuleName, (int)m.BaseAddress);
                    return p;
                }
            }
            return null;
        }

        static void Finish(string message)
        {
            Console.WriteLine(message);
            Console.Write("何かキーを押すと終了します...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        static void Pause()
        {
            ConsoleKeyInfo cki;
            Console.Write("何かキーを押すと続けます... (ESCで終了)");
            cki = Console.ReadKey();
            if (cki.Key == ConsoleKey.Escape)
                Environment.Exit(0);
            Console.WriteLine();
        }

        static void CheckPlayer(FFXI.XIACE.Player player)
        {
            Console.WriteLine("== Checking Player Class ==");
            Console.WriteLine("Name: {0}", player.Name);
            Console.WriteLine("Job: {0}{1}/{2}{3}", player.MainJob, player.MainJobLevel, player.SubJob, player.SubJobLevel);
            Console.WriteLine("Exp: {0:N0}/{1:N0}", player.Exp, player.ExpNext);
            Console.WriteLine("HP: {0}/{1} ({2}%)", player.HP, player.HPMax, player.HPP);
            Console.WriteLine("MP: {0}/{1} ({2}%)", player.MP, player.MPMax, player.MPP);
            Console.WriteLine("TP: {0}%", player.TP);
            Console.WriteLine("Area: {0} (ID:0x{1:X})", player.AreaName, (int)player.Area);
            Console.WriteLine("Activity: {0} (ID: 0x{1:X})", player.Activity, (int)player.Activity);
            Console.WriteLine();
;
        }

        static void CheckEquipment(FFXI.XIACE.Inventory inventory)
        {
            Console.WriteLine("== Checking Inventory Class : Equipment ==");
            Console.WriteLine("Main: {0}", inventory.GetEquippedItemName(eEquipSlot.Main));
            Console.WriteLine("Sub: {0}", inventory.GetEquippedItemName(eEquipSlot.Sub));
            Console.WriteLine("Range: {0}", inventory.GetEquippedItemName(eEquipSlot.Range));
            Console.WriteLine("Ammo: {0}", inventory.GetEquippedItemName(eEquipSlot.Ammo));
            Console.WriteLine("Head: {0}", inventory.GetEquippedItemName(eEquipSlot.Head));
            Console.WriteLine("Body: {0}", inventory.GetEquippedItemName(eEquipSlot.Body));
            Console.WriteLine("Hands: {0}", inventory.GetEquippedItemName(eEquipSlot.Hands));
            Console.WriteLine("Legs: {0}", inventory.GetEquippedItemName(eEquipSlot.Legs));
            Console.WriteLine("Feet: {0}", inventory.GetEquippedItemName(eEquipSlot.Feet));
            Console.WriteLine("Waist: {0}", inventory.GetEquippedItemName(eEquipSlot.Waist));
            Console.WriteLine("Neck: {0}", inventory.GetEquippedItemName(eEquipSlot.Neck));
            Console.WriteLine("Ear Left: {0}", inventory.GetEquippedItemName(eEquipSlot.EarLeft));
            Console.WriteLine("Ear Right: {0}", inventory.GetEquippedItemName(eEquipSlot.EarRight));
            Console.WriteLine("Ring Left: {0}", inventory.GetEquippedItemName(eEquipSlot.RingLeft));
            Console.WriteLine("Ring Right: {0}", inventory.GetEquippedItemName(eEquipSlot.RingRight));
            Console.WriteLine("Back: {0}", inventory.GetEquippedItemName(eEquipSlot.Back));
            Console.WriteLine();
        }

        static void CheckInventory(FFXI.XIACE.Inventory inventory)
        {
            Console.WriteLine("== Checking Inventory Class ==");
            Console.WriteLine("Inventory: {0}/{1}", inventory.GetInventoryCount(), inventory.GetInventoryMax());
            for (short i = 0; i < inventory.GetInventoryMax(); i++)
            {
                string name;
                Inventory.InventoryItem item = inventory.GetInventoryItem(i);
                name = inventory.GetItemNameById(item.id);
                Console.WriteLine(" {0,2:D2} {1:X4} {2} x{3}", i, (int)item.id, String.IsNullOrEmpty(name) ? "EMPTY" : name, item.count);
            }
            Pause();
            Console.WriteLine("Safebox: {0}", inventory.GetSafeboxMax());
            for (short i = 0; i < inventory.GetSafeboxMax(); i++)
            {
                string name;
                Inventory.InventoryItem item = inventory.GetSafeboxItem(i);
                name = inventory.GetItemNameById(item.id);
                Console.WriteLine(" {0,2:D2}: {1:X4} {2} x{3}", i, (int)item.id, String.IsNullOrEmpty(name) ? "EMPTY" : name, item.count);
            }
            Pause();
            Console.WriteLine("Storage: {0}", inventory.GetStorageMax());
            for (short i = 0; i < inventory.GetStorageMax(); i++)
            {
                string name;
                Inventory.InventoryItem item = inventory.GetStorageItem(i);
                name = inventory.GetItemNameById(item.id);
                Console.WriteLine(" {0,2:D2}: {1:X4} {2} x{3}", i, (int)item.id, String.IsNullOrEmpty(name) ? "EMPTY" : name, item.count);
            }
            Pause();
            Console.WriteLine("Locker: {0}", inventory.GetLockerMax());
            for (short i = 0; i < inventory.GetLockerMax(); i++)
            {
                string name;
                Inventory.InventoryItem item = inventory.GetLockerItem(i);
                name = inventory.GetItemNameById(item.id);
                Console.WriteLine(" {0,2:D2}: {1:X4} {2} x{3}", i, (int)item.id, String.IsNullOrEmpty(name) ? "EMPTY" : name, item.count);
            }
            Console.WriteLine();
        }

        static void CheckBuffs(Player player)
        {
            int c = 0;
            Console.WriteLine("== Checking Buffs ==");
            Console.Write("List Buffs:");
            foreach (eBuff b in player.ListBuffs())
            {
                if (b == eBuff.Undefined) break;
                Console.Write("{0} ", b);
                c++;
            }
            Console.WriteLine("({0} buffs)", c);
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Process p = CheckFFXIRunning();
            XIWindower xiw = null;

            if (p == null)
            {
                Finish("FFXi is not running.");
            }
            try
            {
                xiw = new XIWindower(p.Id);
            }
            catch
            {
                Finish("WindowerHelper.dll がないかも");
            }
            CheckPlayer(xiw.Player);
            Pause();
            CheckBuffs(xiw.Player);
            Pause();
            CheckEquipment(xiw.Inventory);
            Pause();
            CheckInventory(xiw.Inventory);

            Finish("== End of TEST ==");
        }
    }
}
