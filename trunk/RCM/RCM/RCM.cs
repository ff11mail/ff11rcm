using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using FFXI;

namespace RCM
{
    public partial class RCM : Form
    {
        public SortedList WindowerPids = new SortedList();
        public Thread thread1;
        public Thread thread2;
        public Windower.MainFunctions w1;
        public Windower.MainFunctions w2;

        private RCMNetServer serv = null;
        private RCMNetClient client = null;
        
        public RCM()
        {
            InitializeComponent();
            ListWindowerProcess();
            w1 = new Windower.MainFunctions();
            w2 = new Windower.MainFunctions();
        }

        public void ListWindowerProcess()
        {
            WindowerPids.Clear();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("pol"))
            {
                WindowerPids.Add(p.MainWindowTitle, p);
                comboBox1.Items.Add(p.MainWindowTitle);
                comboBox2.Items.Add(p.MainWindowTitle);
                comboBox3.Items.Add(p.MainWindowTitle);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = comboBox2.Items.Count > 1 ? 1 : 0;
            comboBox3.SelectedIndex = 0;
        }

        private string comRead(Windower.MainFunctions w)
        {
            string text = "";
            short count;
            string ret = string.Empty;
            count = w.ConsoleGetArgCount();
            text = w.ConsoleGetArg(0);
            if (text == "rcm")
            {
                string[] args = new string[count - 1];
                for (short i = 1; i < count; i++)
                {
                    args[i - 1] = w.ConsoleGetArg(i);
                }
                w.SendText("//rcmnop"); // send a dummy command (Required!!) // IsNewCommand がバグってるので
                ret = String.Join(" ", args);
            }
            return ret;
        }

        /// <summary>
        /// w1 から読み取ってw2に送る
        /// ださいのでクラスつくったほうがいい
        /// </summary>
        public void comThread1()
        {
            string command;
            while (true)
            {
                command = comRead(w1);
                if (!String.IsNullOrEmpty(command))
                {
                    w2.SendText(command); // send command to w2
                }
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// w2 から読み取ってw1 に送る
        /// </summary>
        public void comThread2()
        {
            string command;
            while (true)
            {
                command = comRead(w2);
                if (!String.IsNullOrEmpty(command))
                {
                    w1.SendText(command); // send command to w2
                }
                Thread.Sleep(50);
            }
        }

        public void RunP2()
        {
            System.Diagnostics.Process p;

            p = (System.Diagnostics.Process)WindowerPids[comboBox1.SelectedItem];
            w1.SetPID((uint)p.Id);
            p = (System.Diagnostics.Process)WindowerPids[comboBox2.SelectedItem];
            w2.SetPID((uint)p.Id);

            ThreadStart t1 = new ThreadStart(comThread1);
            thread1 = new Thread(t1);
            thread1.IsBackground = true;
            thread1.Start();
            ThreadStart t2 = new ThreadStart(comThread2);
            thread2 = new Thread(t2);
            thread2.IsBackground = true;
            thread2.Start();
        }

        public void StopP2()
        {
            thread1.Abort();
            thread2.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "スタート")
            {
                button1.Text = "停止";
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;                
                groupBox2.Enabled = false;
                RunP2();
            }
            else
            {
                StopP2();
                button1.Text = "スタート";
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                groupBox2.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (button1.Text == "停止")
                StopP2();
            this.Close();
        }

        // Network Issues
        // public functions.
        public string getPass()
        {
            return textBoxPassword.Text;
        }

        public int getPort()
        {
            return (int)numericUpDown1.Value;
        }

        public string getAddress()
        {
            return textBoxAddress.Text;
        }

        public void WindowerSendText(string text)
        {
            // w1 をつかおう ownWindower とかにかえたほうがよさげ
            w1.SendText(text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "スタート")
            {
                System.Diagnostics.Process p;
                p = (System.Diagnostics.Process)WindowerPids[comboBox3.SelectedItem];
                w1.SetPID((uint)p.Id);
                button2.Text = "停止";
                comboBox3.Enabled = false;
                groupBox1.Enabled = false;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                textBoxAddress.Enabled = false;
                textBoxPassword.Enabled = false;
                numericUpDown1.Enabled = false;
                if (radioButton1.Checked)
                {
                    serv = new RCMNetServer(this);
                    serv.Start();
                    thread1 = new Thread(new ThreadStart(comNetThread));
                    thread1.IsBackground = true;
                    thread1.Start();
                }
                else
                {
                    client = new RCMNetClient(this);
                    if (!client.Start())
                    {
                        MessageBox.Show(this, "接続に失敗しました。");
                        enableNetGui();
                    }
                    else
                    {
                        thread1 = new Thread(new ThreadStart(comNetThread));
                        thread1.IsBackground = true;
                        thread1.Start();
                    }
                }
            }
            else
            {
                if (serv != null)
                {
                    serv.Stop();
                    serv = null;
                }
                else
                {
                    client.Stop();
                    client = null;
                }
                if (thread1 != null && thread1.IsAlive)
                    thread1.Abort();
                thread1 = null;
                button2.Text = "スタート";
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                textBoxAddress.Enabled = true;
                textBoxPassword.Enabled = true;
                numericUpDown1.Enabled = true;
                groupBox1.Enabled = true;
            }
        }

        delegate void ArglessDelegateFunc();
        private void enableNetGui()
        {
            if (InvokeRequired) {
                Invoke(new ArglessDelegateFunc(enableNetGui));
                return;
            }
            button2.Text = "スタート";
            comboBox3.Enabled = true;
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            textBoxAddress.Enabled = true;
            textBoxPassword.Enabled = true;
            numericUpDown1.Enabled = true;
            groupBox1.Enabled = true;     
        }

        public void clientDead()
        {
            if (thread1 != null && thread1.IsAlive)
                thread1.Abort();
            enableNetGui();
        }

        private void comNetThread()
        {
            string command;
            while (true)
            {
                command = comRead(w1);
                if (!string.IsNullOrEmpty(command))
                {
                    if (serv != null)
                        serv.Enqueue(command); // <-- ?  SendText でいい気がする
                    else if (client != null)
                        client.SendText(command);
                    w1.SendText("//rcmnop"); // IsNewCommand がバグってるので
                }
                Thread.Sleep(100);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void updateListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListWindowerProcess();
        }
    }
}
