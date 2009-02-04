/* 
 * JPIDConv ItemName converter for SpellCast.
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
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace JPIDConv
{
    public class Converter
    {
        private string path;

        /// <summary>
        /// DAT読み込み
        /// </summary>
        /// <param name="table">出力ハッシュテーブル</param>
        /// <param name="rom">ROM番号</param>
        /// <param name="file_id">ファイル番号</param>
        /// <param name="increv">逆引きテーブルを追加するかどうか</param>
        private void loadDat(Hashtable table, byte rom, byte file_id, bool increv)
        {
            string cache_name = path + String.Format("\\cache_{0}_{1}.txt",  rom, file_id);
            string fileName = PlayOnline.FFXI.FFXI.GetFilePath(0, rom, file_id); 
            StreamReader sr;
            // POLUtils のバグ対策
            Regex regWrong = new Regex(@"([帶煟舆矓绡楰])");
            Match m;
            Hashtable fixTable = new Hashtable();
            //fixTable.Add("吤", "判");
            //fixTable.Add("厇", "劇");
            //fixTable.Add("囓", "図");
            fixTable.Add("帶", "帷");
            //fixTable.Add("久", "幅");
            fixTable.Add("煟", "影");
            //fixTable.Add("闭", "拭");
            fixTable.Add("舆", "爆");
            fixTable.Add("矓", "石");
            fixTable.Add("绡", "縞");
            //fixTable.Add("耎", "茎");
            //fixTable.Add("稈", "計");
            fixTable.Add("楰", "陰");
            // fixTable.Add("", "飲");
            //fixTable.Add("諄", "髄");
            Console.WriteLine("データベース読み込み中... RomID: {0}, FileID: {1}", rom, file_id);

            if (File.GetLastWriteTime(cache_name) < File.GetLastWriteTime(fileName))
            {
                PlayOnline.FFXI.FileTypes.ItemData itemdata = new PlayOnline.FFXI.FileTypes.ItemData();
                PlayOnline.FFXI.ThingList things = itemdata.Load(fileName);
                StreamWriter sw = new StreamWriter(cache_name);
                foreach (PlayOnline.FFXI.Things.Item item in things)
                {                    
                    string name = item.GetFieldText("name");
                    m = regWrong.Match(name);
                    if (m.Success)
                    {
                        string old = m.Groups[1].Value;
                        string rep = (string)fixTable[old];                        
                        Console.WriteLine("  Wrong char deteced in \"{0}\" : {1} -> {2}", name, old, rep);
                        name = name.Replace(old, rep);
                    }

                    uint id = (uint)item.GetFieldValue("id");
                    if (name == ".") continue;
                    sw.WriteLine(name + "\t" + id);
                }
                sw.Flush();
                sw.Close();
            }
            sr = new StreamReader(cache_name);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] data = line.Split('\t');
                int id = Int32.Parse(data[1]);
                string str = string.Format("{0:X}", id);

                // 二重追加防止
                if (table.Contains(data[0]))
                    continue;

                // null ID対応
                if (str.Substring(2) == "00")
                {
                    str = @"\x" + str.Substring(0, 2) + @"\xFF";
                    table.Add(data[0], @"\xFD\x0A\x01" + str + @"\xFD");
                    if (increv)
                    {
                        table.Add(@"\xFD\x0A\x01" + str + @"\xFD", data[0]);
                    }
                }
                else
                {
                    str = @"\x" + str.Substring(0, 2) + @"\x" + str.Substring(2, 2);
                    table.Add(data[0], @"\xFD\x07\x01" + str + @"\xFD");
                    if (increv)
                    {
                        table.Add(@"\xFD\x07\x01" + str + @"\xFD", data[0]);
                    }
                }

            }
            sr.Close();
        }

        private void ConvertXML(string input, string output, Hashtable convTable, bool ignoreNull)
        {
            StreamReader sr = new StreamReader(input);
            StreamWriter sw = new StreamWriter(output);
            Console.WriteLine("変換: {0} -> {1}", Path.GetFileName(input), Path.GetFileName(output));
            Regex reg = new Regex("(>[^<]+<|\"[^<]+\")");
            Match m;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                m = reg.Match(line);
                while (m.Success)
                {
                    string name = m.Groups[1].Value.Substring(1, m.Groups[1].Value.Length - 2);
                    string idstr = string.Empty;
                    if (convTable.Contains(name))
                    {
                        idstr = (string)convTable[name];
                        if (ignoreNull && idstr.Contains(@"\x00"))
                        {
                            Console.WriteLine(" W: '{0}' の変換 '{1}' は無視されます.", name, idstr);
                        }
                        else
                        {
                            string rep = m.Groups[1].Value[0] + idstr + m.Groups[1].Value[m.Groups[1].Value.Length - 1];
                            line = line.Replace(m.Groups[1].Value, rep);
                        }
                    }
                    m = m.NextMatch();
                }
                sw.WriteLine(line);
            }
            sr.Close();
            sw.Flush();
            sw.Close();            
        }

        public Converter()
        {
            path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            // 必要 (初期化？)
            PlayOnline.Core.POL.GetApplicationPath(PlayOnline.Core.AppID.FFXI,
                        PlayOnline.Core.POL.Region.Japan);
        }

        public void Run()
        {
            string[] xmls;

            // JP Name->ID
            Hashtable NameId = new Hashtable();
            loadDat(NameId, 0, 6, true);
            loadDat(NameId, 0, 7, true);

            // JP to ID
            foreach (string xml in Directory.GetFiles(path, "*_jp.xml"))
            {
                string dst = xml.Replace("_jp.xml", ".xml");
                if (File.Exists(dst) && File.GetLastWriteTime(dst) > File.GetLastWriteTime(xml))
                {
                    Console.WriteLine("{0}の変換をスキップします: {1} > {2}", Path.GetFileName(xml), File.GetLastWriteTime(dst), File.GetLastWriteTime(xml));
                    continue;
                }
                ConvertXML(xml, dst, NameId, true);
            }

            // EN to JP
            xmls = Directory.GetFiles(path, "*_en.xml");
            if (xmls.Length > 0)
            {
                Hashtable NameIdEn = new Hashtable();
                Hashtable convtable = new Hashtable();
                loadDat(NameIdEn, 118, 108, false);
                loadDat(NameIdEn, 118, 109, false);

                foreach (string key in NameIdEn.Keys)
                {
                    string value = (string)NameIdEn[key];
                    if (NameId.Contains(value))
                    {
                        convtable[key] = NameId[value];
                    }
                }

                foreach (string xml in xmls)
                {
                    string dst = xml.Replace("_en.xml", "_en2jp.xml");
                    if (File.Exists(dst) && File.GetLastWriteTime(dst) > File.GetLastWriteTime(xml))
                    {
                        Console.WriteLine("{0}の変換をスキップします: {1} > {2}", Path.GetFileName(xml), File.GetLastWriteTime(dst), File.GetLastWriteTime(xml));
                        continue;
                    }
                    ConvertXML(xml, dst, convtable, false);
                }
            }
            Console.Write("完了しました。何かキーを押すと終了します...");
            Console.ReadKey(); 
        }
    }
}
