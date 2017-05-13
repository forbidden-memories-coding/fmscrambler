using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using FMScrambler.Models;

namespace FMScrambler.helper
{
    public class FileHandler
    {

        public void LoadSlus (string filepath)
        {
            StringReader strReader = new StringReader(File.ReadAllText(@"./CharacterTable.txt"));
            
            string input;

            while ((input = strReader.ReadLine()) != null)
            {
                Match match = Regex.Match(input, "^([A-Fa-f0-9]{2})\\=(.*)$");

                if (match.Success)
                {
                    char k1 = Convert.ToChar(match.Groups[2].ToString());
                    byte k2 = (byte) int.Parse(match.Groups[1].ToString(), NumberStyles.HexNumber);
                    Static.Dict.Add(k2, k1);
                    if (!Static.rDict.ContainsKey(k1))
                        Static.rDict.Add(k1, k2);
                }
            }

            MemoryStream memStream = new MemoryStream(File.ReadAllBytes(Static.SLUSPath)) {Position = 1854020L};
            
            for (int i = 0; i < 722; ++i)
            {
                int int32 = memStream.extractPiece(0, 4, -1).extractInt32(0);
                Static.Cards[i] = new Card();
                Static.Cards[i].Id = i + 2;

            }
            for (int j = 0; j < 722; ++j)
            {
                memStream.Position = (long)(1859586 + j * 2);
                int num = (int)memStream.extractPiece(0, 2, -1).extractUInt16(0) & (int)ushort.MaxValue;
                memStream.Position = (long)(1861632 + num - 24576);
                Static.Cards[j].Name = memStream.GetText(Static.Dict);
            }

            FileStream fusionStream = new FileStream(Static.WAPath, FileMode.Open);
            MemoryStream memStream2 = new MemoryStream(fusionStream.extractPiece(0, 65536, 12089344));

            for (int i = 0; i < 722; ++i)
            {
                memStream2.Position = (long) (2 + i * 2);
                memStream2.Position = (long)((int)memStream2.extractPiece(0, 2, -1).extractUInt16(0) & (int)ushort.MaxValue);
                if (memStream2.Position != 0L)
                {
                    int num1 = memStream2.ReadByte();
                    if (num1 == 0) num1 = 511 - memStream2.ReadByte();
                    int num2 = num1;

                    while (num2 > 0)
                    {
                        int num3 = memStream2.ReadByte();
                        int num4 = memStream2.ReadByte();
                        int num5 = memStream2.ReadByte();
                        int num6 = memStream2.ReadByte();
                        int num7 = memStream2.ReadByte();
                        int num8 = 3;
                        int num9 = (num3 & num8) << 8 | num4;
                        int num10 = 2;
                        int num11 = (num3 >> num10 & 3) << 8 | num5;
                        int num12 = 4;
                        int num13 = (num3 >> num12 & 3) << 8 | num6;
                        int num14 = 6;
                        int num15 = (num3 >> num14 & 3) << 8 | num7;

                        Static.Cards[i].Fusions.Add(new Fusion(i+1, num9 - 1, num11 - 1));
                        --num2;

                        if (num2 <= 0) continue;
                        Static.Cards[i].Fusions.Add(new Fusion(i+1, num13-1, num15-1));
                        --num2;
                    }
                }
            }
            memStream.Close();
            memStream2.Close();
            fusionStream.Close();
        }

        public void ScrambleFusions(int seed)
        {
            
            Random rand = new Random(seed);

            if (!File.Exists(@"scramblelog_#" + seed + ".log")) File.CreateText("scramblelog_#" + seed + ".log").Close();
            StreamWriter logStream = new StreamWriter(@"scramblelog_#"+seed+".log");
            logStream.WriteLine("@@ YU-GI-OH! Forbidden Memories Fusion Scrambler Log Output @@");
            logStream.WriteLine("@@ Version "+Static.versionInfo+" @@");
            logStream.WriteLine("@@ Seed: #" + seed+" @@");
            logStream.WriteLine("@@ Start time: " +DateTime.Now+" @@");

            var mainWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;

            for (int i = 0; i < 722; i++)
            {
                
                if (mainWin != null) mainWin.lbl_status.Content = "Scrambling Card #" + i + 1 + " of 722.";
                for (int j = 0; j < Static.Cards[i].Fusions.Count; j++)
               {
                   Static.Cards[i].Fusions[j].Cards2 = rand.Next((Static.highID) ? 1 : i, Static.cardCount);
                   Static.Cards[i].Fusions[j].Result = rand.Next((Static.highID) ? 1 : i, Static.cardCount);
                   logStream.WriteLine("=> " +
                       (Static.Cards[i].Fusions[j].Cards1 > 721 ? "(!) GLITCHCARD (!)" : Static.Cards[Static.Cards[i].Fusions[j].Cards1].Name)
                       +" ("+
                       (Static.Cards[i].Fusions[j].Cards1+1)
                       +") + " +
                       (Static.Cards[i].Fusions[j].Cards2 > 721 ? "(!) GLITCHCARD (!)" : Static.Cards[Static.Cards[i].Fusions[j].Cards2].Name)
                       + " ("+ 
                       Static.Cards[i].Fusions[j].Cards2
                       + ") = " +
                       (Static.Cards[i].Fusions[j].Result > 721 ? "(!) GLITCHCARD (!)" : Static.Cards[Static.Cards[i].Fusions[j].Result].Name)
                       + " ("+ 
                       Static.Cards[i].Fusions[j].Result
                       +")");
               }
               
            }

            FileStream fileStream = new FileStream(Static.WAPath, FileMode.Open);
            int[] numArray = new int[7]
            {
                12089344,
                12570624,
                13051904,
                13533184,
                14014464,
                14495744,
                14977024
            };

            MemoryStream memStream1 = new MemoryStream(1444);
            MemoryStream memStream2 = new MemoryStream(64092);

            memStream1.Position = 2L;
            memStream2.Position = 2L;

            foreach (Card card in Static.Cards)
            {
                short num1 = card.Fusions.Count != 0 ? (short) (memStream2.Position + 1444L) : (short) 0;
                memStream1.Write(num1.int16ToByteArray(), 0, 2);

                if (card.Fusions.Count != 0)
                {
                    if (card.Fusions.Count < 256)
                        memStream2.WriteByte((byte) card.Fusions.Count);
                    else
                    {
                        memStream2.WriteByte((byte) 0);
                        memStream2.WriteByte((byte) Math.Abs(card.Fusions.Count - 511));
                    }
                    for (int i = 0; i < card.Fusions.Count; ++i)
                    {
                        int num2 = card.Fusions[i].Cards2 + 1 & (int)byte.MaxValue;
                        int num3 = card.Fusions[i].Result + 1 & (int)byte.MaxValue;
                        int num4 = 0;
                        int num5 = 0;
                        int num6 = card.Fusions[i].Cards2 + 1 >> 8 & 3 | (card.Fusions[i].Result + 1 >> 8 & 3) << 2;
                        if (i < card.Fusions.Count - 1)
                        {
                            num4 = card.Fusions[i + 1].Cards2 + 1 & (int)byte.MaxValue;
                            num5 = card.Fusions[i + 1].Result + 1 & (int)byte.MaxValue;
                            num6 |= (card.Fusions[i + 1].Cards2 + 1 >> 8 & 3) << 4 | (card.Fusions[i + 1].Result + 1 >> 8 & 3) << 6;
                            ++i;
                        }
                        memStream2.WriteByte((byte)(num6 & (int)byte.MaxValue));
                        memStream2.WriteByte((byte)(num2 & (int)byte.MaxValue));
                        memStream2.WriteByte((byte)(num3 & (int)byte.MaxValue));
                        if (num4 != 0 || num5 != 0)
                        {
                            memStream2.WriteByte((byte)(num4 & (int)byte.MaxValue));
                            memStream2.WriteByte((byte)(num5 & (int)byte.MaxValue));
                        }
                    }
                }
            }

            while (memStream2.Position < 64092L)
                memStream2.WriteByte((byte) 238);
            foreach (int num in numArray)
            {
                fileStream.Position = (long) num;
                fileStream.Write(memStream1.ToArray(), 0, 1444);
                fileStream.Write(memStream2.ToArray(), 0, 64092);
            }
            fileStream.Close();
            memStream2.Close();
            memStream1.Close();

            logStream.WriteLine("@@ Done scrambling at " + DateTime.Now+" @@");
            logStream.Close();
            if (mainWin != null) mainWin.lbl_status.Content = "Done scrambling!";
        }
    }
}