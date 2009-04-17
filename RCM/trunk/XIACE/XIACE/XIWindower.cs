/* XIACE - Memory Access Provider for FFXI.
 * Copyright (C) 2009 FFXI RCM Project <ff11rcm@gmail.com>
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
