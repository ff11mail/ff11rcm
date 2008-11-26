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
            Console.Write("何かキーを押すと続けます...");
            Console.ReadKey();
        }

        static void CheckPlayer(FFXI.XIACE.Player player)
        {
            Console.WriteLine("== Checking Player Class ==");
            Console.WriteLine("Name: {0}", player.Name);
            Console.WriteLine("HP: {0}/{1} ({2}%)", player.HP, player.HPMax, player.HPP);
            Console.WriteLine("MP: {0}/{1} ({2}%)", player.MP, player.MPMax, player.MPP);
            Console.WriteLine("TP: {0}%", player.TP);
            Console.WriteLine("Area: {0} (ID:0x{1:X})", player.AreaName, (int)player.Area);
            Console.WriteLine("Activity: {0} (ID: 0x{1:X})", player.Activity, (int)player.Activity);
            Console.WriteLine();
;
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
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Process p = CheckFFXIRunning();
            XIWindower xiw;

            if (p == null)
            {
                Finish("FFXi is not running.");
            }
            
            xiw = new XIWindower(p.Id);
            CheckPlayer(xiw.Player);
            Pause();
            CheckInventory(xiw.Inventory);

            Finish("== End of TEST ==");
        }
    }
}
