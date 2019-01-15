using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FMLib.Models;
using FMLib.Utility;

namespace FMLib.Randomizer
{
    /// <summary>
    /// 
    /// </summary>
    public class DataScrambler
    {
        private readonly Random _random;
        private readonly int _seed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        public DataScrambler(int seed)
        {

            // Initialize RNG with the Seed
            _random = new Random(seed);
            _seed = seed;
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadDataFromSlus()
        {
            MemoryStream memStream = new MemoryStream(File.ReadAllBytes(Static.SlusPath)) { Position = 1854020L };
            for (int i = 0; i < 722; i++)
            {
                int int32 = memStream.ExtractPiece(0, 4).ExtractInt32();
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
            for (int i = 0; i < 722; i++)
            {
                byte num = memStream.ExtractPiece(0, 1)[0];
                Static.Cards[i].Level = num & 15;
                Static.Cards[i].Attribute = num >> 4 & 15;
            }

            // Card Name
            for (int i = 0; i < 722; i++)
            {
                memStream.Position = 1859586 + i * 2;
                int num = memStream.ExtractPiece(0, 2).ExtractUInt16() & ushort.MaxValue;
                memStream.Position = 1861632 + num - 24576;
                Static.Cards[i].Name = memStream.GetText(Static.Dict);
            }

            // Card Description
            for (int i = 0; i < 722; i++)
            {
                memStream.Position = 1772034 + i * 2;
                int num3 = memStream.ExtractPiece(0, 2).ExtractUInt16();
                memStream.Position = 1774068 + (num3 - 2548);
                Static.Cards[i].Description = memStream.GetText(Static.Dict);
            }

            for (int i = 0; i < 39; i++)
            {
                memStream.Position = 1861202 + i * 2;
                memStream.Position = 1861632 + memStream.ExtractPiece(0, 2).ExtractUInt16() - 24576;
                Static.Duelist[i] = new Duelist(memStream.GetText(Static.Dict));
            }

            memStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadDataFromWaMrg()
        {
            // WA_MRG process
            FileStream fusionStream = new FileStream(Static.WaPath, FileMode.Open);
            MemoryStream memStream = new MemoryStream(fusionStream.ExtractPiece(0, 65536, 12089344));

            // Card Fusions
            for (int i = 0; i < 722; ++i)
            {
                memStream.Position = 2 + i * 2;
                memStream.Position = memStream.ExtractPiece(0, 2).ExtractUInt16() & ushort.MaxValue;
                if (memStream.Position != 0L)
                {
                    int num1 = memStream.ReadByte();
                    if (num1 == 0)
                    {
                        num1 = 511 - memStream.ReadByte();
                    }

                    int num2 = num1;

                    while (num2 > 0)
                    {
                        int num3 = memStream.ReadByte();
                        int num4 = memStream.ReadByte();
                        int num5 = memStream.ReadByte();
                        int num6 = memStream.ReadByte();
                        int num7 = memStream.ReadByte();
                        int num9 = (num3 & 3) << 8 | num4;
                        int num11 = (num3 >> 2 & 3) << 8 | num5;
                        int num13 = (num3 >> 4 & 3) << 8 | num6;
                        int num15 = (num3 >> 6 & 3) << 8 | num7;

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

            memStream = new MemoryStream(fusionStream.ExtractPiece(0, 10240, 12079104));

            while (true)
            {
                int num6 = memStream.ExtractPiece(0, 2).ExtractUInt16();

                if (num6 == 0)
                {
                    break;
                }

                int num7 = memStream.ExtractPiece(0, 2).ExtractUInt16();

                for (int num8 = 0; num8 < num7; num8++)
                {
                    int num9 = memStream.ExtractPiece(0, 2).ExtractUInt16();
                    Static.Cards[num6 - 1].Equips.Add(num9 - 1);
                }
            }

            memStream.Close();

            //Read Starchip Cost/Password pairs
            memStream = new MemoryStream(fusionStream.ExtractPiece(0, 722 * 8, 0xFB9808));
            for (var i = 0; i < 722; ++i)
            {
                Static.Cards[i].Starchip = new Starchips();
                var cost_bytes = new byte[4];
                cost_bytes[0] = (byte)memStream.ReadByte();
                cost_bytes[1] = (byte)memStream.ReadByte();
                cost_bytes[2] = (byte)memStream.ReadByte();
                cost_bytes[3] = (byte)memStream.ReadByte();
                Static.Cards[i].Starchip.Cost = cost_bytes.ExtractInt32();
                var pass_bytes = new byte[4];
                pass_bytes[0] = (byte)memStream.ReadByte();
                pass_bytes[1] = (byte)memStream.ReadByte();
                pass_bytes[2] = (byte)memStream.ReadByte();
                pass_bytes[3] = (byte)memStream.ReadByte();
                var res_pass = "";
                for (var j = 3; j >= 0; --j)
                {
                    var str = pass_bytes[j].ToString("X");
                    if (str.Length == 1) str = str.Insert(0, "0");
                    res_pass += str;
                }
                int.TryParse(res_pass, out int out_pass);
                Static.Cards[i].Starchip.Password = out_pass;
                Static.Cards[i].Starchip.PasswordStr = res_pass;
            }

            memStream.Close();
            fusionStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RandomizeFusions()
        {
            if (Static.RandomFusions)
            {
                for (int i = 0; i < 722; i++)
                {
                    foreach (Fusion t in Static.Cards[i].Fusions)
                    {
                        // FUSION RANDOMIZING
                        t.Cards2 = _random.Next(Static.HighId ? 1 : i, Static.CardCount);
                        t.Result = _random.Next(Static.HighId ? 1 : i, Static.CardCount);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minAtk"></param>
        /// <param name="maxAtk"></param>
        /// <param name="minDef"></param>
        /// <param name="maxDef"></param>
        /// <param name="minCost"></param>
        /// <param name="maxCost"></param>
        public void RandomizeCardInfo(int minAtk = 1000, int maxAtk = 3000, int minDef = 1000, int maxDef = 3000, int minCost = 1, int maxCost = 999999)
        {
            for (int i = 0; i < 722; i++)
            {
                // ATK/DEF RANDOMIZING
                if (Static.RandomAtkdef)
                {
                    Static.Cards[i].Attack = _random.Next(minAtk, maxAtk);

                    Static.Cards[i].Defense = _random.Next(minDef, maxDef);
                }

                if (Static.GlitchAttributes)
                {
                    Static.Cards[i].Attribute = _random.Next(1, 15);
                }

                if (Static.RandomGuardianStars)
                {
                    Static.Cards[i].GuardianStar1 = _random.Next(1, 25);
                    Static.Cards[i].GuardianStar2 = _random.Next(1, 25);
                }

                if (Static.RandomTypes)
                {
                    Static.Cards[i].Type = _random.Next(1, 25);
                }

                if (Static.RandomEquips)
                {
                    for (int j = 0; j < _random.Next(20); j++)
                    {
                        int rando = _random.Next(1, 722);
                        Static.Cards[i].Equips.Add(rando);
                    }
                }

                if (Static.RandomStarchips)
                {
                    foreach (Card card in Static.Cards)
                    {
                        card.Starchip.Cost = _random.Next(minCost, maxCost);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minDropRate"></param>
        /// <param name="maxDropRate"></param>
        public void RandomizeCardDrops(int minDropRate = 1, int maxDropRate = 1)
        {
            if (Static.RandomCardDrops)
            { 
                foreach (Duelist t1 in Static.Duelist)
                {
                    var total_rate = 0;
                    while (true)
                    {
                        var rate = _random.Next(minDropRate, maxDropRate);
                        if (total_rate + rate > 2048)
                        {
                            rate = 2048 - total_rate;
                        }
                        t1.Drop.BcdPow[_random.Next(0, 722)]+= rate;
                        t1.Drop.SaPow[_random.Next(0, 722)] += rate;
                        t1.Drop.SaTec[_random.Next(0, 722)] += rate;
                        total_rate += rate;
                        if (total_rate == 2048) break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RandomizeDuelistDecks()
        {
            if (Static.RandomDecks)
            {
                foreach (Duelist t1 in Static.Duelist)
                {
                    for (int ix = 0; ix < 2048; ix++)
                    {
                        t1.Deck[_random.Next(0, 722)]++;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteChangesToFile()
        {

            // Writing Random Fusions
            if (Static.RandomFusions)
            {
                using (FileStream fileStream = new FileStream(Static.WaPath, FileMode.Open))
                {

                    int[] numArray = {
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
                        short num1 = card.Fusions.Count != 0 ? (short)(memStream2.Position + 1444L) : (short)0;
                        memStream1.Write(num1.Int16ToByteArray(), 0, 2);
                        if (card.Fusions.Count != 0)
                        {
                            if (card.Fusions.Count < 256)
                            {
                                memStream2.WriteByte((byte)card.Fusions.Count);
                            }
                            else
                            {
                                memStream2.WriteByte(0);
                                memStream2.WriteByte((byte)Math.Abs(card.Fusions.Count - 511));
                            }
                            for (int i = 0; i < card.Fusions.Count; ++i)
                            {
                                int num2 = card.Fusions[i].Cards2 + 1 & byte.MaxValue;
                                int num3 = card.Fusions[i].Result + 1 & byte.MaxValue;
                                int num4 = 0;
                                int num5 = 0;
                                int num6 = card.Fusions[i].Cards2 + 1 >> 8 & 3 | (card.Fusions[i].Result + 1 >> 8 & 3) << 2;
                                if (i < card.Fusions.Count - 1)
                                {
                                    num4 = card.Fusions[i + 1].Cards2 + 1 & byte.MaxValue;
                                    num5 = card.Fusions[i + 1].Result + 1 & byte.MaxValue;
                                    num6 |= (card.Fusions[i + 1].Cards2 + 1 >> 8 & 3) << 4 |
                                            (card.Fusions[i + 1].Result + 1 >> 8 & 3) << 6;
                                    ++i;
                                }
                                memStream2.WriteByte((byte)(num6 & byte.MaxValue));
                                memStream2.WriteByte((byte)(num2 & byte.MaxValue));
                                memStream2.WriteByte((byte)(num3 & byte.MaxValue));
                                if (num4 != 0 || num5 != 0)
                                {
                                    memStream2.WriteByte((byte)(num4 & byte.MaxValue));
                                    memStream2.WriteByte((byte)(num5 & byte.MaxValue));
                                }
                            }
                        }
                    }
                    while (memStream2.Position < 64092L)
                    {
                        memStream2.WriteByte(238);
                    }

                    foreach (int num in numArray)
                    {
                        fileStream.Position = num;
                        var mem_arr1 = memStream1.ToArray();
                        var mem_arr2 = memStream2.ToArray();
                        fileStream.Write(mem_arr1, 0, mem_arr1.Length);
                        fileStream.Write(mem_arr2, 0, mem_arr2.Length);
                    }

                    // Close memorystream after use
                    memStream2.Close();
                    memStream1.Close();
                }
            }

            // Randomize ATK/DEF, Guardian Stars, Types, Attributes
            if (Static.RandomAtkdef || Static.RandomGuardianStars || Static.RandomTypes || Static.RandomAttributes)
            {
                using (FileStream fileStreamSl = new FileStream(Static.SlusPath, FileMode.Open))
                {
                    fileStreamSl.Position = 1854020L;
                    using (MemoryStream memoryStream = new MemoryStream(2888))
                    {
                        for (int i = 0; i < 722; ++i)
                        {
                            int value = (Static.Cards[i].Attack / 10 & 511) | (Static.Cards[i].Defense / 10 & 511) << 9 |
                                        (Static.Cards[i].GuardianStar2 & 15) << 18 |
                                        (Static.Cards[i].GuardianStar1 & 15) << 22 | (Static.Cards[i].Type & 31) << 26;
                            memoryStream.Write(value.Int32ToByteArray(), 0, 4);
                        }
                        var arr = memoryStream.ToArray();
                        fileStreamSl.Write(arr, 0, arr.Length);
                    }
                }
            }

            // Randomize Decks and Card Drops
            if (Static.RandomDecks || Static.RandomCardDrops)
            {
                using (FileStream duelistStream = new FileStream(Static.WaPath, FileMode.Open))
                {
                    for (int i = 0; i < 39; i++)
                    {
                        int num = 15314944 + 6144 * i;

                        // Randomize Decks
                        if (Static.RandomDecks)
                        {
                            duelistStream.Position = num;
                            using (MemoryStream memoryStream = new MemoryStream(1444))
                            {
                                int[] array = Static.Duelist[i].Deck;
                                foreach (int t in array)
                                {
                                    short value = (short)t;
                                    memoryStream.Write(value.Int16ToByteArray(), 0, 2);
                                }
                                var arr = memoryStream.ToArray();
                                duelistStream.Write(arr, 0, arr.Length);
                            }
                        }

                        // Randomize Card Drops
                        if (Static.RandomCardDrops)
                        {
                            duelistStream.Position = num + 1460;
                            using (MemoryStream memoryStream2 = new MemoryStream(1444))
                            {
                                int[] array = Static.Duelist[i].Drop.SaPow;
                                foreach (int t in array)
                                {
                                    short value2 = (short)t;
                                    memoryStream2.Write(value2.Int16ToByteArray(), 0, 2);
                                }
                                var arr = memoryStream2.ToArray();
                                duelistStream.Write(arr, 0, arr.Length);
                            }
                            duelistStream.Position = num + 2920;
                            using (MemoryStream memoryStream3 = new MemoryStream(1444))
                            {
                                int[] array = Static.Duelist[i].Drop.BcdPow;
                                foreach (int t in array)
                                {
                                    short value3 = (short)t;
                                    memoryStream3.Write(value3.Int16ToByteArray(), 0, 2);
                                }
                                var arr = memoryStream3.ToArray();
                                duelistStream.Write(arr, 0, arr.Length);
                            }
                            duelistStream.Position = num + 4380;
                            using (MemoryStream memoryStream4 = new MemoryStream(1444))
                            {
                                int[] array = Static.Duelist[i].Drop.SaTec;
                                foreach (int t in array)
                                {
                                    short value4 = (short)t;
                                    memoryStream4.Write(value4.Int16ToByteArray(), 0, 2);
                                }
                                var arr = memoryStream4.ToArray();
                                duelistStream.Write(arr, 0, arr.Length);
                            }
                        }
                    }
                }
            }

            if (Static.RandomStarchips)
            {
                using (FileStream starchipStream = new FileStream(Static.WaPath, FileMode.Open))
                {
                    starchipStream.Position = 0xFB9808;
                    for (var i = 0; i < 722; ++i)
                    {
                        var cost_arr = Static.Cards[i].Starchip.Cost.Int32ToByteArray();
                        var pass_arr = Static.Cards[i].Starchip.PasswordStr.StringToByteArray();
                        var offset = 0;
                        for (var j = cost_arr.Length - 2; j >= 0; --j)
                        {
                            if (cost_arr[j] == 0) offset++;
                            else break;
                        }
                        for (var j = 0; j < cost_arr.Length - offset - 1; ++j)
                        {
                            starchipStream.WriteByte(cost_arr[j]);
                        }
                        for (var j = 0; j < offset; ++j) starchipStream.WriteByte(0);
                        // Advance over unused byte
                        starchipStream.Position += 1;
                        for (var j = pass_arr.Length - 1; j >= 0; --j)
                        {
                            starchipStream.WriteByte(pass_arr[j]);
                        }
                    }
                }
            }
        }

        // TODO: Better Log File Logic + HTML/JSON/XML Format
        /// <summary>
        /// 
        /// </summary>
        public void WriteFusionSpoilerFile()
        {
            if (!File.Exists($@"fusions_spoiler_#{_seed}.log"))
            {
                File.CreateText($"fusions_spoiler_#{_seed}.log").Close();
            }

            StreamWriter logStream = new StreamWriter($@"fusions_spoiler_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Fusion Scrambler Spoiler File ==");
            logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");

            foreach (Card c in Static.Cards)
            {
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"=> #{c.Id} {c.Name} ({c.Attack}/{c.Defense})");
                logStream.WriteLine("=> FUSIONS:");
                foreach (Fusion fus in c.Fusions)
                {
                    logStream.WriteLine($"    => {fus.Cards1} + {fus.Cards2} = {fus.Result}         ({(fus.Cards1 > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards1).Name)} + {(fus.Cards2 > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards2).Name)} = {(fus.Result > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Result).Name)})");
                }
            }

            logStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteStarchipSpoilerFile()
        {
            if (!File.Exists($@"starchip_spoiler_#{_seed}.log"))
            {
                File.CreateText($"starchip_spoiler_#{_seed}.log").Close();
            }

            StreamWriter logStream = new StreamWriter($@"starchip_spoiler_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Starchip Scrambler Spoiler File ==");
            logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");

            foreach (Card c in Static.Cards)
            {
                logStream.WriteLine($"    => #{c.Id} {c.Name}");
                logStream.WriteLine($"        Cost: {c.Starchip.Cost} Password: {c.Starchip.PasswordStr}");
            }

            logStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteDropsSpoilerFile()
        {
            if (!File.Exists($@"drops_spoiler_#{_seed}.log"))
            {
                File.CreateText($"drops_spoiler_#{_seed}.log").Close();
            }

            StreamWriter logStream = new StreamWriter($@"drops_spoiler_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Drops Scrambler Spoiler File ==");
            logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");

            // Get drop map as well as sort by drop rate
            Dictionary<Duelist, List<KeyValuePair<Card, int>>> get_drop_map(DropType dropType)
            {
                var drop_map = new Dictionary<Duelist, List<KeyValuePair<Card, int>>>();
                foreach (Duelist d in Static.Duelist)
                {
                    var sorted_map = new List<KeyValuePair<Card, int>>();
                    foreach (Card c in Static.Cards)
                    {
                        if (d.Drop.GetDropArray(dropType)[c.Id - 1] > 0)
                        {
                            sorted_map.Add(new KeyValuePair<Card, int>(c, d.Drop.GetDropArray(dropType)[c.Id - 1]));
                        }
                    }
                    sorted_map.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                    drop_map.Add(d, sorted_map);
                }
                return drop_map;
            }

            var sap_d_map = get_drop_map(DropType.SAPOW);
            var bcd_d_map = get_drop_map(DropType.BCDPOW);
            var sat_d_map = get_drop_map(DropType.SATEC);

            foreach (Duelist d in Static.Duelist)
            {
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"{d.Name} S/A-Tec drops");
                var drop_map = sat_d_map[d];
                foreach (var p in drop_map)
                {
                    logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                    logStream.WriteLine($"        Rate: {p.Value}/2048");
                }

                logStream.WriteLine();
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"{d.Name} B/C/D drops");
                drop_map = bcd_d_map[d];
                foreach (var p in drop_map)
                {
                    logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                    logStream.WriteLine($"        Rate: {p.Value}/2048");
                }

                logStream.WriteLine();
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"{d.Name} S/A-Pow drops");
                drop_map = sap_d_map[d];
                foreach (var p in drop_map)
                {
                    logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                    logStream.WriteLine($"        Rate: {p.Value}/2048");
                }
                logStream.WriteLine();
            }

            logStream.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        public void WriteHtmlFusionSpoilerFile()
        {
            if (!File.Exists($@"fusions_spoiler_#{_seed}.html"))
            {
                File.CreateText($"fusions_spoiler_#{_seed}.html").Close();
            }

            StreamWriter logStream = new StreamWriter($@"fusions_spoiler_#{_seed}.html");

            string template =
                $@"<!DOCTYPE html>
                <html lang=\""en\""> 
                <head>
                <meta charset=\""UTF-8\"">
                <meta name=\""viewport\"" content=\""width=device-width, initial-scale=1.0\"">
                <meta http-equiv=\""X-UA-Compatible\"" content=\""ie=edge\"">
                <title>{_seed}</title>
                <style>
                    html, body {{ width: 100%; height: auto; margin: 0 auto; }}
                    table {{ width: 90%; margin: 0 auto; border: 1px solid lightgrey; border-spacing: 0; }}
                    thead {{ background-color: #f2ffeb; padding: 2px; }}
                    thead th {{ padding: 3px; text-align: center; }}
                    table td {{ border-bottom: 1px solid lightgrey!important; padding: 2px!important; text-align: center; }}
                    table tr:last-child td {{ border-bottom: 0!important; }}
                    tbody tr td {{ border-right: 1px solid lightgrey; }}
                    tbody tr td:last-child {{ border-right: 0!important; }}
                </style>
                </head>
                <body>
                <div style=\""width: 100 %; text-align: center;\"">";

            string table2Template =
                @"<span style=\""text-align: center; font-size: 120%; font-weight: bold; margin-bottom: 5px;\"">Changed Fusions</span>
                  <table>
                    <thead>
                        <tr>
                    <th style=\""width: 5%;\"">ID 1</th>
                    <th style=\""width: 25%;\"">Name 1</th>
                    <th style=\""width: 5%;\"">ID 2</th>
                    <th style=\""width: 25%;\"">Name 2</th>
                    <th style=\""width: 5%;\"">ResultID</th>
                    <th style=\""width: 25%;\"">Result Name</th>
                </tr>
            </thead>
            <tbody>";
            string tmpFusions = "";

            logStream.WriteLine(template + $" <h1>YU-GI-OH! Forbidden Memories Fusion Scrambler Spoiler File</h1> <h4>Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion}</h4>");

            foreach (Card c in Static.Cards)
            {
                foreach (Fusion fus in c.Fusions)
                {
                    tmpFusions += "<tr>";
                    tmpFusions += $"<td>{fus.Cards1}</td> <td>{(fus.Cards1 > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards1).Name)}</td> <td>{fus.Cards2}</td> <td>{(fus.Cards2 > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards2).Name)}</td> <td>{fus.Result}</td> <td>{(fus.Result > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Result).Name)}</td></tr>";
                }
            }
            //logStream.WriteLine("</tbody></table><br />");
            logStream.WriteLine(table2Template);
            Console.WriteLine("Writing tmpFusions");
            logStream.WriteLine(tmpFusions);
            logStream.WriteLine("</tbody></table></div></body></html>");
            logStream.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="minAtk"></param>
        /// <param name="maxAtk"></param>
        /// <param name="minDef"></param>
        /// <param name="maxDef"></param>
        /// <param name="minCost"></param>
        /// <param name="maxCost"></param>
        /// <returns></returns>
        public bool PerformScrambling(int minAtk = 0, int maxAtk = 0, int minDef = 0, int maxDef = 0, int minCost = 0, int maxCost = 999999, int minDropRate = 1, int maxDropRate = 1)
        {
            LoadDataFromSlus();
            LoadDataFromWaMrg();
            RandomizeFusions();
            RandomizeCardInfo(minAtk, maxAtk, minDef, maxDef, minCost, maxCost);
            RandomizeCardDrops(minDropRate, maxDropRate);
            RandomizeDuelistDecks();
            WriteChangesToFile();
            if (Static.Spoiler)
            {
                if (Static.RandomFusions) { WriteFusionSpoilerFile(); /*WriteHtmlFusionSpoilerFile();*/ }
                if (Static.RandomStarchips) WriteStarchipSpoilerFile();
                if (Static.RandomCardDrops) WriteDropsSpoilerFile();
            }

            return true;
        }
    }
}
