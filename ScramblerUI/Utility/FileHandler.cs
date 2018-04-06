using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using FMLib.Models;
using FMLib.Utility;
using Troschuetz.Random.Generators;

namespace FMScrambler.Utility
{
    public class FileHandler
    {
        public void LoadSlus(string filepath)
        {
            var strReader = new StringReader(File.ReadAllText(@"./CharacterTable.txt"));

            string input;

            while ((input = strReader.ReadLine()) != null)
            {
                var match = Regex.Match(input, "^([A-Fa-f0-9]{2})\\=(.*)$");

                if (!match.Success)
                {
                    continue;
                }

                var k1 = Convert.ToChar(match.Groups[2].ToString());
                var k2 = (byte) int.Parse(match.Groups[1].ToString(), NumberStyles.HexNumber);

                Static.Dict.Add(k2, k1);

                if (!Static.rDict.ContainsKey(k1))
                {
                    Static.rDict.Add(k1, k2);
                }
            }

            // Card ID, GuardianStar1 and 2, Type
            var memStream = new MemoryStream(File.ReadAllBytes(Static.SLUSPath)) {Position = 1854020L};
            for (var i = 0; i < 722; i++)
            {
                int int32 = memStream.extractPiece(0, 4).extractInt32();
                Static.Cards[i] = new Card
                {
                    Id = i + 1,
                    Attack = (int32 & 511) * 10,
                    Defense = (int32 >> 9 & 511) * 10,
                    GuardianStar2 = int32 >> 18 & 15,
                    GuardianStar1 = int32 >> 22 & 15,
                    Type = int32 >> 26 & 31
                };
            }

            // Card Level and Attribute
            memStream.Position = 1858355L;
            for (var i = 0; i < 722; i++)
            {
                var num = memStream.extractPiece(0, 1)[0];
                Static.Cards[i].Level = num & 15;
                Static.Cards[i].Attribute = num >> 4 & 15;
            }

            // Card Name
            for (var i = 0; i < 722; i++)
            {
                memStream.Position = 1859586 + i * 2;
                var num = memStream.extractPiece(0, 2).extractUInt16() & ushort.MaxValue;
                memStream.Position = 1861632 + num - 24576;
                Static.Cards[i].Name = memStream.GetText(Static.Dict);
            }

            // Card Description
            for (var i = 0; i < 722; i++)
            {
                memStream.Position = 1772034 + i * 2;
                int num3 = memStream.extractPiece(0, 2).extractUInt16();
                memStream.Position = 1774068 + (num3 - 2548);
                Static.Cards[i].Description = memStream.GetText(Static.Dict);
            }

            // Duelist Name
            for (var i = 0; i < 39; i++)
            {
                memStream.Position = 1861202 + i * 2;
                memStream.Position = 1861632 + memStream.extractPiece(0, 2).extractUInt16() - 24576;
                Static.Duelist[i] = new Duelist(memStream.GetText(Static.Dict));
            }

            memStream.Close();
            // WA_MRG process
            var fusionStream = new FileStream(Static.WAPath, FileMode.Open);
            memStream = new MemoryStream(fusionStream.extractPiece(0, 65536, 12089344));

            // Card Fusions
            for (var i = 0; i < 722; ++i)
            {
                memStream.Position = 2 + i * 2;
                memStream.Position = memStream.extractPiece(0, 2).extractUInt16() & ushort.MaxValue;
                if (memStream.Position != 0L)
                {
                    var num1 = memStream.ReadByte();
                    if (num1 == 0)
                    {
                        num1 = 511 - memStream.ReadByte();
                    }

                    var num2 = num1;

                    while (num2 > 0)
                    {
                        var num3 = memStream.ReadByte();
                        var num4 = memStream.ReadByte();
                        var num5 = memStream.ReadByte();
                        var num6 = memStream.ReadByte();
                        var num7 = memStream.ReadByte();
                        var num9 = (num3 & 3) << 8 | num4;
                        var num11 = (num3 >> 2 & 3) << 8 | num5;
                        var num13 = (num3 >> 4 & 3) << 8 | num6;
                        var num15 = (num3 >> 6 & 3) << 8 | num7;

                        Static.Cards[i].Fusions.Add(new Fusion(i + 1, num9 - 1, num11 - 1));
                        --num2;

                        if (num2 <= 0)
                        {
                            continue;
                        }

                        Static.Cards[i].Fusions.Add(new Fusion(i + 1, num13 - 1, num15 - 1));
                        --num2;
                    }
                }
            }
            memStream.Close();
            memStream = new MemoryStream(fusionStream.extractPiece(0, 10240, 12079104));

            while (true)
            {
                var num6 = (int) memStream.extractPiece(0, 2).extractUInt16();

                if (num6 == 0)
                {
                    break;
                }

                int num7 = memStream.extractPiece(0, 2).extractUInt16();

                for (int num8 = 0; num8 < num7; num8++)
                {
                    int num9 = memStream.extractPiece(0, 2).extractUInt16();
                    Static.Cards[num6-1].Equips.Add(num9-1);
                }
            }

            memStream.Close();
            fusionStream.Close();
        }

        public void ScrambleFusions(int seed)
        {
            var randFusion = new Random(seed);
            var randVal = new Random(seed);

            var mainWin =
                Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;

            mainWin.pgr_back.Visibility = Visibility.Visible;
            for (var i = 0; i < 722; i++)
            {
                foreach (var t in Static.Cards[i].Fusions)
                {
                    if (mainWin != null && Static.randomATKDEF)
                    {
                        // ATK/DEF RANDOMIZING
                        Static.Cards[i].Attack = randVal.Next(Convert.ToInt32(mainWin.rs_atk.LowerValue),
                            Convert.ToInt32(mainWin.rs_atk.UpperValue));

                        Static.Cards[i].Defense = randVal.Next(Convert.ToInt32(mainWin.rs_def.LowerValue),
                            Convert.ToInt32(mainWin.rs_def.UpperValue));

                        Static.Cards[i].Attribute = randVal.Next(1, 15);
                        Static.Cards[i].Level = randVal.Next(1, 12);
                        Static.Cards[i].Description = "Trolololo";
                        Static.Cards[i].GuardianStar1 = randVal.Next(1, 25);
                        Static.Cards[i].GuardianStar2 = randVal.Next(1, 25);
                        Static.Cards[i].Type = randVal.Next(1, 25);
                        //logStream.WriteLine($"=> {Static.Cards[i].Name} {Static.Cards[i].Attack}/{Static.Cards[i].Defense}");

                        for (int j = 0; j < randVal.Next(20); j++)
                        {
                            var rando = randVal.Next(1, 722);
                            //Console.WriteLine($"rando: {rando}");
                            Static.Cards[i].Equips.Add(rando);
                            //logStream.WriteLine($"=> {Static.Cards[i].Name} can use as Equip: {Static.Cards[rando].Name}");
                        }
                        
                    }


                    // FUSION RANDOMIZING
                    t.Cards2 = randFusion.Next(Static.highID ? 1 : i, Static.cardCount);
                    t.Result = randFusion.Next(Static.highID ? 1 : i, Static.cardCount);

                }
            }
            var drand = new NR3Generator();
            foreach (Duelist t1 in Static.Duelist)
            {
                for (int ix = 0; ix < 2048; ix++)
                {
                    t1.Deck[drand.Next(0, 722)]++;
                    t1.Drop.BCDPow[drand.Next(0, 722)]++;
                    t1.Drop.SAPow[drand.Next(0, 722)]++;
                    t1.Drop.SATec[drand.Next(0, 722)]++;
                    drand.Seed = (uint)DateTime.Now.Ticks;
                }
            }

            var fileStream = new FileStream(Static.WAPath, FileMode.Open);

            var numArray = new int[7]
            {
                12089344,
                12570624,
                13051904,
                13533184,
                14014464,
                14495744,
                14977024
            };

            var memStream1 = new MemoryStream(1444);
            var memStream2 = new MemoryStream(64092);

            memStream1.Position = 2L;
            memStream2.Position = 2L;

            foreach (var card in Static.Cards)
            {
                var num1 = card.Fusions.Count != 0 ? (short) (memStream2.Position + 1444L) : (short) 0;
                memStream1.Write(num1.int16ToByteArray(), 0, 2);

                if (card.Fusions.Count != 0)
                {
                    if (card.Fusions.Count < 256)
                    {
                        memStream2.WriteByte((byte) card.Fusions.Count);
                    }
                    else
                    {
                        memStream2.WriteByte(0);
                        memStream2.WriteByte((byte) Math.Abs(card.Fusions.Count - 511));
                    }
                    for (var i = 0; i < card.Fusions.Count; ++i)
                    {
                        var num2 = card.Fusions[i].Cards2 + 1 & byte.MaxValue;
                        var num3 = card.Fusions[i].Result + 1 & byte.MaxValue;
                        var num4 = 0;
                        var num5 = 0;
                        var num6 = card.Fusions[i].Cards2 + 1 >> 8 & 3 | (card.Fusions[i].Result + 1 >> 8 & 3) << 2;
                        if (i < card.Fusions.Count - 1)
                        {
                            num4 = card.Fusions[i + 1].Cards2 + 1 & byte.MaxValue;
                            num5 = card.Fusions[i + 1].Result + 1 & byte.MaxValue;
                            num6 |= (card.Fusions[i + 1].Cards2 + 1 >> 8 & 3) << 4 |
                                    (card.Fusions[i + 1].Result + 1 >> 8 & 3) << 6;
                            ++i;
                        }
                        memStream2.WriteByte((byte) (num6 & byte.MaxValue));
                        memStream2.WriteByte((byte) (num2 & byte.MaxValue));
                        memStream2.WriteByte((byte) (num3 & byte.MaxValue));
                        if (num4 != 0 || num5 != 0)
                        {
                            memStream2.WriteByte((byte) (num4 & byte.MaxValue));
                            memStream2.WriteByte((byte) (num5 & byte.MaxValue));
                        }
                    }
                }
            }

            while (memStream2.Position < 64092L)
            {
                memStream2.WriteByte(238);
            }

            foreach (var num in numArray)
            {
                fileStream.Position = num;
                fileStream.Write(memStream1.ToArray(), 0, 1444);
                fileStream.Write(memStream2.ToArray(), 0, 64092);
            }
            fileStream.Close();
            memStream2.Close();
            memStream1.Close();

            using (var fileStreamSl = new FileStream(Static.SLUSPath, FileMode.Open))
            {
                fileStreamSl.Position = 1854020L;
                using (var memoryStream = new MemoryStream(2888))
                {
                    for (var i = 0; i < 722; ++i)
                    {
                        var value = (Static.Cards[i].Attack / 10 & 511) | (Static.Cards[i].Defense / 10 & 511) << 9 |
                                    (Static.Cards[i].GuardianStar2 & 15) << 18 |
                                    (Static.Cards[i].GuardianStar1 & 15) << 22 | (Static.Cards[i].Type & 31) << 26;
                        memoryStream.Write(value.int32ToByteArray(), 0, 4);
                    }
                    fileStreamSl.Write(memoryStream.ToArray(), 0, 2888);
                }
            }

            using (FileStream duelistStream = new FileStream(Static.WAPath, FileMode.Open))
            {
                for (int i = 0; i < 39; i++)
                {
                    int num = 15314944 + 6144 * i;
                    duelistStream.Position = num;
                    using (MemoryStream memoryStream = new MemoryStream(1444))
                    {
                        int[] array = Static.Duelist[i].Deck;
                        foreach (var t in array)
                        {
                            short value = (short)t;
                            memoryStream.Write(value.int16ToByteArray(), 0, 2);
                        }
                        duelistStream.Write(memoryStream.ToArray(), 0, 1444);
                    }
                    duelistStream.Position = num + 1460;
                    using (MemoryStream memoryStream2 = new MemoryStream(1444))
                    {
                        int[] array = Static.Duelist[i].Drop.SAPow;
                        foreach (var t in array)
                        {
                            short value2 = (short)t;
                            memoryStream2.Write(value2.int16ToByteArray(), 0, 2);
                        }
                        duelistStream.Write(memoryStream2.ToArray(), 0, 1444);
                    }
                    duelistStream.Position = num + 2920;
                    using (MemoryStream memoryStream3 = new MemoryStream(1444))
                    {
                        int[] array = Static.Duelist[i].Drop.BCDPow;
                        foreach (var t in array)
                        {
                            short value3 = (short)t;
                            memoryStream3.Write(value3.int16ToByteArray(), 0, 2);
                        }
                        duelistStream.Write(memoryStream3.ToArray(), 0, 1444);
                    }
                    duelistStream.Position = num + 4380;
                    using (MemoryStream memoryStream4 = new MemoryStream(1444))
                    {
                        int[] array = Static.Duelist[i].Drop.SATec;
                        foreach (var t in array)
                        {
                            short value4 = (short)t;
                            memoryStream4.Write(value4.int16ToByteArray(), 0, 2);
                        }
                        duelistStream.Write(memoryStream4.ToArray(), 0, 1444);
                    }
                }
            }

            if (!File.Exists($@"scramblelog_#{seed}.log"))
            {
                File.CreateText($"scramblelog_#{seed}.log").Close();
            }

            StreamWriter logStream = new StreamWriter($@"scramblelog_#{seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Fusion Scrambler Log Output ==");
            logStream.WriteLine($"== Version {Meta.majorVersion}.{Meta.minorVersion}.{Meta.patchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");
            string glitchPlace = "Glitch!";
            foreach (var c in Static.Cards)
            {
                logStream.WriteLine($"====================================================================");
                logStream.WriteLine($"=> #{c.Id} {c.Name} ({c.Attack}/{c.Defense})");
                logStream.WriteLine($"=> FUSIONS:");
                foreach (var fus in c.Fusions)
                {
                    logStream.WriteLine($"    => {fus.Cards1} + {fus.Cards2} = {fus.Result}         ({(fus.Cards1 > 722 ? glitchPlace : Static.Cards.Single(card => card.Id == fus.Cards1).Name)} + {(fus.Cards2 > 722 ? glitchPlace : Static.Cards.Single(card => card.Id == fus.Cards2).Name)} = {(fus.Result > 722 ? glitchPlace : Static.Cards.Single(card => card.Id == fus.Result).Name)})");
                }
            }

            logStream.Close();
            if (mainWin != null)
            {
                mainWin.lbl_status.Content = "Done scrambling!";
                mainWin.pgr_back.Visibility = Visibility.Hidden;
            }
        }
    }
}