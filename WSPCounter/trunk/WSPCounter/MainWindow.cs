using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FFXI.XIACE;

namespace WSPCounter
{
    public struct Weapon
    {
        private uint _Id;
        private string _JName;
        private string _EName;
        private ushort _Unlock;

        public Weapon(uint id, string ja, string en, ushort unlock)
        {
            _Id = id;
            _JName = ja;
            _EName = en;
            _Unlock = unlock;
        }

        public uint ID { get { return _Id; } }
        public string JName { get { return _JName; } }
        public string EName { get { return _EName; } }
        public string Unlock { get { return _Unlock;  } }
    }

    public partial class MainWindow : Form
    {
        private XIWindower xiw;
        private PolProcess[] pols;
        private FFXI.TextObject to;
        private SortedList Weapons = new SortedList();

        public MainWindow()
        {
            InitializeComponent();
            InitTable();
            InitWindower();
            textSample.ForeColor = Color.FromArgb(255, 255, 255);
            textSample.BackColor = Color.FromArgb(0, 0, 0);
            timer1.Start();
        }

        private void InitWindower()
        {
            pols = XIACE.ListPolProcess();
            if (pols.Length == 0)
            {
                MessageBox.Show("FFXIが起動してないようです");
                Environment.Exit(0);
                return;
            }
            xiw = new XIWindower(pols[0].Pid);
            to = xiw.CreateTextObject("WSPCounter");
            to.SetBGColor(128, 0, 0, 0);
            to.SetBGVisibilitiy(true);
            to.SetFontColor(255, 255, 255, 255);
            to.SetLocation(50, 50);
            to.Flush();
        }

        private void InitTable()
        {
            Weapons.Add(17742, new Weapon(17742, "シャープソード", "Vorpal Sword", 250));
            Weapons.Add(17743, new Weapon(17743, "ワイトスレイヤー", "Wightslayer", 250));
            Weapons.Add(17744, new Weapon(17744, "ブレイブブレイド", "Brave Blade", 250));
            Weapons.Add(17956, new Weapon(17956, "ダブルハーケン", "Double Axe", 250));
            Weapons.Add(18003, new Weapon(18003, "ソードブレイカー", "Swordbreaker", 250));
            Weapons.Add(18034, new Weapon(18034, "ダンシングダガー", "Dancing Dagger", 250));
            Weapons.Add(18120, new Weapon(18120, "グローランス", "Radiant Lance", 250));
            Weapons.Add(18426, new Weapon(18426, "佐助の刀", "Sasuke Katana", 250));
            Weapons.Add(18443, new Weapon(18443, "風切りの刃", "Windslicer", 250));
            Weapons.Add(18492, new Weapon(18492, "魔神の斧", "Sturdy Axe", 250));
            Weapons.Add(18589, new Weapon(18589, "魔道士の杖", "Mage's Staff", 250));
            Weapons.Add(18590, new Weapon(18590, "錫杖", "Scepter Staff", 250));
            Weapons.Add(18592, new Weapon(18592, "長老の杖", "Elder Staff", 250));
            Weapons.Add(18719, new Weapon(18719, "キラーボウ", "Killer Bow", 250));
            Weapons.Add(18720, new Weapon(18720, "クイックシルバー", "Quicksilver", 250));
            Weapons.Add(18753, new Weapon(18753, "バーニンナックル", "Burning Fists", 250));
            Weapons.Add(18754, new Weapon(18754, "地獄の爪", "Inferno Claws", 250));
            Weapons.Add(18851, new Weapon(18851, "ウェアバスター", "Werebuster", 250));
            Weapons.Add(18944, new Weapon(18944, "デスシックル", "Death Sickle", 250));
            Weapons.Add(19102, new Weapon(19102, "マインゴーシュ", "Main Gauche", 250));

	        Weapons.Add(16735, new Weapon(16735, "トライアルアクス", "Axe of Trials", 300));
            Weapons.Add(16793, new Weapon(16793, "トライアルサイズ", "Scythe of Trials", 300));
            Weapons.Add(16892, new Weapon(16892, "トライアルスピア", "Spear of Trials", 300));
            Weapons.Add(16952, new Weapon(16952, "トライアルソード", "Sword of Trials", 300));
            Weapons.Add(17456, new Weapon(17456, "トライアルクラブ", "Club of Trials", 300));
            Weapons.Add(17507, new Weapon(17507, "トライアルナックル", "Knuckles of Trials", 300));
            Weapons.Add(17527, new Weapon(17527, "トライアルポール", "Pole of Trials", 300));
            Weapons.Add(17616, new Weapon(17616, "トライアルダガー", "Dagger of Trials", 300));
            Weapons.Add(17654, new Weapon(17654, "トライアルサパラ", "Sapara of Trials", 300));
            Weapons.Add(17773, new Weapon(17773, "試練の小太刀", "Kodachi of Trials", 300));
            Weapons.Add(17815, new Weapon(17815, "試練の太刀", "Tachi of Trials", 300));
            Weapons.Add(17933, new Weapon(17933, "トライアルピック", "Pick of Trials", 300));
            Weapons.Add(18144, new Weapon(18144, "トライアルボウ", "Bow of Trials", 300));
            Weapons.Add(18146, new Weapon(18146, "トライアルガン", "Gun of Trials", 300));

            Weapons.Add(17207, new Weapon(17207, "エクスパンジャー", "Expunger", 500));
            Weapons.Add(17275, new Weapon(17275, "コフィンメーカー", "Coffinmaker", 500));
            Weapons.Add(17451, new Weapon(17451, "モルゲンステルン", "Morgenstern", 500));
            Weapons.Add(17509, new Weapon(17509, "デストロイヤー", "Destroyers", 500));
            Weapons.Add(17589, new Weapon(17589, "チュルソスシュタプ", "Thyrsusstab", 500));
            Weapons.Add(17699, new Weapon(17699, "ダイセクター", "Dissector", 500));
            Weapons.Add(17793, new Weapon(17793, "千手院力王", "Senjuinrikio", 500));
            Weapons.Add(17827, new Weapon(17827, "道芝の露", "Michishiba", 500));
            Weapons.Add(17944, new Weapon(17944, "レトリビューター", "Retributor", 500));
            Weapons.Add(18005, new Weapon(18005, "ハートスナッチャー", "Heart Snatcher", 500));
            Weapons.Add(18053, new Weapon(18053, "グレーブディガー", "Gravedigger", 500));
            Weapons.Add(18097, new Weapon(18097, "権藤鎮教", "Gondo-Shizunori", 500));
            Weapons.Add(18217, new Weapon(18217, "ランページャー", "Rampager", 500));
            Weapons.Add(18378, new Weapon(18378, "サブドゥア", "Subduer", 500));
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            to.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult ret = colorDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                textSample.BackColor = colorDialog1.Color;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult ret = colorDialog1.ShowDialog();
            if (ret == DialogResult.OK)
            {
                textSample.ForeColor = colorDialog1.Color;
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {

            to.SetFontColor((byte)numTextAlpha.Value,
                (byte)textSample.ForeColor.R,
                (byte)textSample.ForeColor.G,
                (byte)textSample.ForeColor.B);
            to.SetBGColor((byte)numBackAlpha.Value,
                (byte)textSample.BackColor.R,
                (byte)textSample.BackColor.G,
                (byte)textSample.BackColor.B);
            to.SetLocation((float)numX.Value, (float)numY.Value);
            to.Flush();
        }

        private void UpdateInfo()
        {
            ArrayList ar = new ArrayList();
            for (short i = 0; i < xiw.Inventory.GetInventoryMax(); i++)
            {
                Inventory.InventoryItem item = xiw.Inventory.GetInventoryItem(i);
                if (Weapons.Contains((int)item.id))
                {
                    Weapon w = (Weapon)Weapons[(int)item.id];
                    ar.Add(String.Format("{0} {1}:{2}/{3}", w.ID,w.EName,item.extraCount, w.Unlock));
                }
            }
            to.SetText(String.Join("\r\n", (String[])ar.ToArray(typeof(String))));
            to.Flush();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (xiw.Player.IsLoggedIn())
            {
                UpdateInfo();
            }
        }
    }
}
