using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FFXI.XIACE
{
    internal struct FishingState
    {
        internal int rodPos;
        internal int unknown0;
        internal int unknown1;
        internal int unknown2;
        internal int unknown3;
        internal int unknownPtr;
        internal int fishPtr;
    }

    internal struct Fish
    {
        internal int MaxHP;
        internal int HP;
        internal int ID3;
        internal int unknown0;
        internal int ID1;
        internal int ID2;
        internal int ID4;
        internal int timeout;
        internal int unknown1;
        internal int unknown2;
        internal int rodPos;
    }

    public class Fishing
    {
        private PolProcess pol;
        private FishingState fishingState;
        private Fish fish;

        public Fishing(Process proc)
        {
            pol = new PolProcess(proc);
        }

        public Fishing(int pid)
        {
            Process proc = Process.GetProcessById(pid);
            pol = new PolProcess(proc);
        }

        unsafe private void Read()
        {
            FishingState fs = new FishingState();
            MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)((int)pol.BaseAddress + OFFSET.FISH_INFO), &fs, (uint)Marshal.SizeOf(fs), null);
            fishingState = fs;
            Fish f = new Fish();
            MemoryProvider.ReadProcessMemory(pol.Handle, (IntPtr)(fs.fishPtr), &f, (uint)Marshal.SizeOf(f), null);
            fish = f;
        }

        public int GetFishMaxHP()
        {
            return fish.MaxHP;
        }

        public int GetFishHP()
        {
            Read();
            return fish.HP;
        }

        public int GetFishTimeout()
        {
            Read();
            return fish.timeout;
        }

        public int GetFishID1()
        {
            Read();
            return fish.ID1;
        }

        public int GetFishID2()
        {
            Read();
            return fish.ID2;
        }

        public int GetFishID3()
        {
            Read();
            return fish.ID3;
        }

        public int GetFishID4()
        {
            Read();
            return fish.ID4;
        }

        public int GetFishOnLineTime()
        {
            ///FIXME: still not implemented.
            return 0;
        }

        /// <summary>
        /// FIXME: untested.
        /// </summary>
        /// <returns></returns>
        public bool FishOnLine()
        {
            Read();
            if (fishingState.rodPos < 1)
                return false;
            else
                return true;
        }

        public eRodPosition GetRodposition()
        {
            Read();
            if (fishingState.rodPos == 1)
                return eRodPosition.Center;
            if (fishingState.rodPos == 2 && fish.rodPos == 0)
                return eRodPosition.Left;
            if (fishingState.rodPos == 2 && fish.rodPos == 1)
                return eRodPosition.Right;
            return eRodPosition.Error;
        }
    }
}
