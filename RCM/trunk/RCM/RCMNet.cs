/* RCM - Remote Command Manager for FFXI.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;

namespace RCM
{
    /// <summary>
    /// Client/Server のベースクラス
    /// </summary>
    public abstract class RCMNet
    {
        public NetworkStream ns;
        public TcpClient client;

        /// <summary>
        /// TCP socket から 指定サイズ 読むまでブロック
        /// </summary>
        /// <param name="rBuf">書き出すバッファ</param>
        /// <param name="offset">書き込み位置</param>
        /// <param name="size">読み取るサイズ</param>
        /// <returns>-1: エラー : 読み取ったサイズ</returns>
        public int TCPReadBlock(ref byte[] rBuf, int offset, int size)
        {
            int len = 0;
            while (len < size)
            {
                int rv = 0;
                try
                {
                    rv = ns.Read(rBuf, offset, size - len);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return -1;
                }
                if (rv == 0)
                    return len;
                if (rv < 0)
                {   // Error
                    return rv;
                }
                len += rv;
                offset += rv;
                //Thread.Sleep(100);
            }
            return len;
        }

        /// <summary>
        /// ビッグエンディアン 2バイト 読み取り
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public short ReadBE2(byte[] buf, int offset)
        {
            return (short)((buf[offset] << 8) +
                (buf[offset + 1]));
        }

        /// <summary>
        /// ビッグエンディアン 4バイト読み取り
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int ReadBE4(byte[] buf, int offset)
        {
            return (int)(
                (buf[offset] << 24) +
                (buf[offset + 1] << 16) +
                (buf[offset + 2] << 8) +
                (buf[offset + 3]));
        }

        /// <summary>
        /// ビッグエンディアン 2バイト書き込み
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        public void WriteBE2(short val, ref byte[] buf, ref int offset)
        {
            buf[offset] = (byte)((val >> 8) & 0xff);
            buf[offset + 1] = (byte)(val & 0xff);
            offset += 2;
        }

        /// <summary>
        /// ビッグエンディアン 4バイト書き込み
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        public void WriteBE4(int val, ref byte[] buf, ref int offset)
        {
            buf[offset] = (byte)((val >> 24) & 0xff);
            buf[offset + 1] = (byte)((val >> 16) & 0xff);
            buf[offset + 2] = (byte)((val >> 8) & 0xff);
            buf[offset + 3] = (byte)(val & 0xff);
            offset += 4;
        }

        /// <summary>
        /// パケットタイプ
        /// </summary>
        public enum PacketType
        {
            NOP = 0,
            AUTH,
            SENDSTRING,
            PACKET_TYPE_MAX
        };

        /// <summary>
        /// 認証フェーズ
        /// </summary>
        public enum AuthType
        {
            CHALLENGE = 0,
            PASSWORD,
            SUCCESS,
            FAIL,
            AUTH_TYPE_MAX
        };

        /// <summary>
        /// ストリームに書き込み
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public void StreamWrite(byte[] buf, int offset, int size)
        {
            try
            {
                if (ns.CanWrite)
                {
                    ns.Write(buf, offset, size);
                    ns.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        /// <summary>
        /// ピアにテキスト送信
        /// </summary>
        /// <param name="ns">NetworkStream</param>
        /// <param name="text">text to send</param>
        public void TCPSendText(string text)
        {
            int offset = 0;
            byte[] buf = System.Text.Encoding.GetEncoding(932).GetBytes(text);
            byte[] sendbuf = new byte[2 + 4 + buf.Length];
            WriteBE2((short)PacketType.SENDSTRING, ref sendbuf, ref offset);
            WriteBE4(buf.Length, ref sendbuf, ref offset);
            Array.Copy(buf, 0, sendbuf, offset, buf.Length);
            StreamWrite(sendbuf, 0, sendbuf.Length);
        }

        /// <summary>
        /// ピアからテキストを受信
        /// </summary>
        /// <returns></returns>
        public string TCPReceiveText()
        {
            byte[] buf = new byte[256];
            int rv;
            int rSize;
            string text;
            rv = TCPReadBlock(ref buf, 0, 4); // ペイロードサイズ
            if (rv < 0)
            {
                // エラー
                return string.Empty;
            }
            rSize = ReadBE4(buf, 0);
            rv = TCPReadBlock(ref buf, 0, rSize);
            if (rv < 0)
            {
                // エラー
                return string.Empty;
            }
            text = Encoding.GetEncoding(932).GetString(buf, 0, rSize);
            return text;
        }
    }

    /// <summary>
    /// クライアントセッションクラス
    /// </summary>
    class RCMNetSession : RCMNet
    {
        private RCMNetServer serv;

        private Queue queue;
        private Queue safeQueue;

        private Thread sessThread;
        private Thread writeThread;

        public bool isReady;

        private string challenge;
        private RCM rcm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="c">TCPClient</param>
        /// <param name="s">Server</param>
        /// <param name="r">RCM</param>
        public RCMNetSession(TcpClient c, RCMNetServer s, RCM r)
        {
            isReady = false;
            serv = s;
            client = c;
            ns = client.GetStream();
            rcm = r;
            challenge = SendChallenge(); // C & R 認証
        }

        /// <summary>
        /// セッションスレッド開始
        /// </summary>
        public void Start()
        {
            queue = new Queue();
            safeQueue = Queue.Synchronized(queue);

            sessThread = new Thread(new ThreadStart(this.SessThreadProc));
            sessThread.IsBackground = true;
            sessThread.Start();
        }

        /// <summary>
        /// クローズ
        /// </summary>
        public void Close()
        {
            isReady = false;
            serv.DeleteSession(this);
            if (writeThread != null && writeThread.IsAlive)
                writeThread.Abort();
            sessThread.Abort();
            client.Client.Close();
            client.Close(); // ?
        }

        /// <summary>
        /// テキスト送信
        /// </summary>
        /// <param name="text"></param>
        public void Enqueue(string text)
        {
            safeQueue.Enqueue(text);
        }

        /// <summary>
        /// 認証チャレンジ文字列をクライアントに送信
        /// </summary>
        private string SendChallenge()
        {
            DateTime now = DateTime.Now;
            challenge= String.Format("RCM.{0}{1:00}{2:00}{3:00}{4:00}{5:00}",
                now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second); 
            byte[] b = Encoding.ASCII.GetBytes(challenge);
            byte[] buf = new byte[b.Length + 8]; // MAX
            int off = 0;
            // HEADER: PACKETTYPE(2):PAYLOADSIZE(4): PAYLOAD(2+A)
            // PAYLOAD: AUTHTYPE(2):PAYLOAD(A)
            WriteBE2((short)1, ref buf, ref off);     // PACKETTYE 1 : AUTH
            WriteBE4(2 + b.Length, ref buf, ref off); // PAYLOADSIZE 2 + b.length
            WriteBE2((short)0, ref buf, ref off);     // AUTHTYPE 0 : GREET
            Array.Copy(b, 0, buf, off, b.Length);     // PAYLOAD
            StreamWrite(buf, 0, off + b.Length);

            // create password hash
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(challenge + rcm.getPass()));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            challenge = sb.ToString();
            return challenge;
        }

        /// <summary>
        /// 書き出しスレッド
        /// </summary>
        private void writeThreadProc()
        {
            try
            {
                while (true)
                {
                    while (queue.Count > 0)
                    {
                        string text;
                        text = (string)safeQueue.Dequeue();
                        TCPSendText(text);
                    }
                    Thread.Sleep(200);
                }
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        /// <summary>
        /// セッションスレッド
        /// </summary>
        private void SessThreadProc()
        {
            byte[] rBuf = new byte[256];
            RCMNet.PacketType pkttype;
            int ps; // payload size
            while (true)
            {
                // まず2byte 読む
                int rv;
                rv = TCPReadBlock(ref rBuf, 0, 2);
                if (rv <= 0)
                {
                    Close();
                    break;
                }
                pkttype = (PacketType)ReadBE2(rBuf, 0);
                switch (pkttype)
                {
                    case PacketType.AUTH:
                        AuthType at;
                        rv = TCPReadBlock(ref rBuf, 0, 4); // 4 byte 読む
                        if (rv < 0)
                        {
                            // error
                            Close();
                            break;
                        }
                        ps = ReadBE4(rBuf, 0);
                        rv = TCPReadBlock(ref rBuf, 0, ps); // 指定サイズ分だけ読む
                        if (rv < 0)
                        {
                            // error
                            Close();
                            break;
                        }
                        at = (AuthType)ReadBE2(rBuf, 0);

                        switch (at)
                        {
                            case AuthType.PASSWORD:
                                string pass = Encoding.Default.GetString(rBuf, 2, ps - 2);
                                if (pass == challenge)
                                {
                                    isReady = true;
                                    SendAuth(true);
                                    writeThread = new Thread(new ThreadStart(writeThreadProc));
                                    writeThread.IsBackground = true;
                                    writeThread.Start();
                                }
                                else
                                {
                                    SendAuth(false);
                                    Close();
                                }
                                break;
                            default: // Unexpected AUTH Packet
                                break; // FIXME
                        }
                        break;
                    case PacketType.SENDSTRING:
                        string text = TCPReceiveText();
                        if (!string.IsNullOrEmpty(text))
                            rcm.WindowerSendText(text);
                        break;
                    default:
                        break;
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 認証のレスポンスを送信
        /// </summary>
        /// <param name="auth">成功/失敗</param>
        private void SendAuth(bool auth)
        {
            byte[] sendbuf = new byte[2 + 4 + 2];
            int offset = 0;
            WriteBE2((short)1, ref sendbuf, ref offset); // AUTH
            WriteBE4((int)2, ref sendbuf, ref offset); // SIZE: 2
            if (auth)
                WriteBE2((short)2, ref sendbuf, ref offset); // SUCCESS
            else
                WriteBE2((short)3, ref sendbuf, ref offset); // FAILED
            StreamWrite(sendbuf, 0, sendbuf.Length);
        }
    }

    /// <summary>
    /// サーバクラス : listen して client session クラスを保持する(複数)
    /// </summary>
    class RCMNetServer : RCMNet
    {
        private TcpListener serv;

        private int port;
        private string pass;
        private Thread servThread;
        private ArrayList sessions;
        private ArrayList safeSessions;
        private RCM rcm;

        public RCMNetServer(RCM r)
        {
            rcm = r;
            port = rcm.getPort();
            pass = rcm.getPass();
            sessions = new ArrayList();
            safeSessions = ArrayList.Synchronized(sessions);
        }

        private void ServThreadProc()
        {
            try
            {
                while (true)
                {
                    TcpClient cl = serv.AcceptTcpClient();
                    RCMNetSession sess = new RCMNetSession(cl, this, rcm);
                    safeSessions.Add(sess);
                    sess.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public void DeleteSession(RCMNetSession sess)
        {
            safeSessions.Remove(sess);
        }

        public void Enqueue(string text)
        {
            foreach (RCMNetSession sess in safeSessions)
            {
                if (sess.isReady)
                    sess.Enqueue(text);
            }
        }

        public void Start()
        {
            serv = new TcpListener(IPAddress.Any, port);
            serv.Start();

            ThreadStart ts = new ThreadStart(ServThreadProc);
            servThread = new Thread(ts);
            servThread.IsBackground = true;
            servThread.Start();
        }

        public void Stop()
        {
            ArrayList temp = (ArrayList)safeSessions.Clone();
            foreach (RCMNetSession sess in temp)
            {
                sess.Close();
            }

            if (servThread != null && servThread.IsAlive)
            {
                servThread.Abort();
            }
            serv.Stop();
        }

    }

    /// <summary>
    /// クライアントクラス
    /// </summary>
    class RCMNetClient : RCMNet
    {
        private RCM rcm;
        private Thread clientThread;

        public RCMNetClient(RCM r)
        {
            rcm = r;
        }

        private void Close()
        {
            if (clientThread != null && clientThread.IsAlive)
                clientThread.Abort();
            clientThread = null;
            client.Client.Close();
            client.Close(); // <-- ?
            rcm.clientDead();
        }

        /// <summary>
        /// 通信開始
        /// </summary>
        /// <returns>成功 : true</returns>
        public bool Start()
        {
            RCMNet.PacketType pkttype;
            byte[] buf = new byte[256];

            client = new TcpClient();
            try
            {
                client.Connect(rcm.getAddress(), rcm.getPort());
            }
            catch
            {
                return false;
            }
            ns = client.GetStream();
            // まず2byte 読む (パケットタイプ)
            int rv;
            rv = TCPReadBlock(ref buf, 0, 2);
            if (rv < 0)
            {
                Close();
                return false;
            }

            // 認証
            pkttype = (PacketType)ReadBE2(buf, 0);
            if (pkttype != PacketType.AUTH)
            {
                Close();
                return false;
            }
            int ps; // ペイロードサイズ
            AuthType at;
            TCPReadBlock(ref buf, 0, 4); // 4 byte 読む(ペイロードサイズ)
            ps = ReadBE4(buf, 0);
            TCPReadBlock(ref buf,0, 2); // 2Byte読む (AUTH Type)
            at = (AuthType)ReadBE2(buf, 0);
            if (at != AuthType.CHALLENGE)
            {
                // なんかおかしい
                Close();
                return false;
            }

            // チャレンジ文字列
            TCPReadBlock(ref buf, 0, ps - 2); // 指定サイズ分-2だけ読む
            string challenge = Encoding.Default.GetString(buf,0, ps - 2);
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(challenge + rcm.getPass()));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            string passhash = sb.ToString();            
            
            // ぱしわーど送信
            SendAuthString(passhash);
            TCPReadBlock(ref buf, 0, 2);
            // れしぽんす
            pkttype = (PacketType)ReadBE2(buf, 0);
            if (pkttype != PacketType.AUTH)
            {
                Close();
                return false;
            }
            TCPReadBlock(ref buf, 0, 4); // 4 byte 読む(ペイロードサイズ)
            ps = ReadBE4(buf, 0);
            TCPReadBlock(ref buf, 0, 2); // 2Byte読む (AUTH Type)
            at = (AuthType)ReadBE2(buf, 0);
            if (at != AuthType.SUCCESS)
            {
                // なんかおかしい
                Close();
                return false;
            }
            clientThread = new Thread(new ThreadStart(clientThreadProc));
            clientThread.IsBackground = true;
            clientThread.Start();
            // 成功
            return true;
        }

        /// <summary>
        /// 通信やめる
        /// </summary>
        public void Stop()
        {
            Close();
        }

        /// <summary>
        /// 認証文字列を送信
        /// </summary>
        /// <param name="auth"></param>
        private void SendAuthString(string text)
        {
            byte[] textbuf = Encoding.Default.GetBytes(text); 
            byte[] sendbuf = new byte[2 + 4 + 2 + textbuf.Length]; // pkt(2) ps(4) at(2) string.size 
            int offset = 0;
            WriteBE2((short)1, ref sendbuf, ref offset); // AUTH
            WriteBE4((int)(2 + textbuf.Length), ref sendbuf, ref offset);
            WriteBE2((short)AuthType.PASSWORD, ref sendbuf, ref offset);
            Array.Copy(textbuf, 0, sendbuf, offset, textbuf.Length);
            StreamWrite(sendbuf, 0, sendbuf.Length);
        }

        public void SendText(string text)
        {
            TCPSendText(text);
        }
 
        /// <summary>
        /// サーバから送られてきた文字列を 受け取ってWindowerに渡す
        /// </summary>
        private void clientThreadProc()
        {
            int rv;
            byte[] buf = new byte[256];
            int offset = 0;

            PacketType pt;

            while (true)
            {
                // パケットタイプの読み取りがまずある               
                rv = TCPReadBlock(ref buf, offset, 2);
                if (rv < 0)
                {
                    // エラー
                    Close();
                }
                pt = (PacketType)ReadBE2(buf, 0);
                switch (pt)
                {
                    case PacketType.NOP:
                        //なんもしない
                        break;
                    case PacketType.AUTH:
                        // ? 二重認証はエラーだな
                        break;
                    case PacketType.SENDSTRING:
                        // ここしかこないはず
                        string text = TCPReceiveText();
                        if (!string.IsNullOrEmpty(text))
                            rcm.WindowerSendText(text);
                        break;
                    default:
                        // エラー
                        break;
                }
            }
        }
    }
}
