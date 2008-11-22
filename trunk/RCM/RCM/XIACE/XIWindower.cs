using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FFXI.XIACE
{
    public class XIWindower : FFXI.Windower
    {
        public Player Player;
        public Inventory Inventory;
        public Fishing Fishing;

        public XIWindower(int pid): base (pid)
        {
            Process p = Process.GetProcessById((int)pid);
            Player = new Player(p);
            Inventory = new Inventory(p);
            Fishing = new Fishing(p);
        }
    }
}
