using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
            StringReader strReader = new StringReader(File.ReadAllText(@"./CharacterTable.txt"));

            string input;

            while ((input = strReader.ReadLine()) != null)
            {
                Match match = Regex.Match(input, "^([A-Fa-f0-9]{2})\\=(.*)$");

                if (!match.Success)
                {
                    continue;
                }

                char k1 = Convert.ToChar(match.Groups[2].ToString());
                byte k2 = (byte)int.Parse(match.Groups[1].ToString(), NumberStyles.HexNumber);

                Static.Dict.Add(k2, k1);

                if (!Static.RDict.ContainsKey(k1))
                {
                    Static.RDict.Add(k1, k2);
                }
            }

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
            fusionStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RandomizeFusions()
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minAtk"></param>
        /// <param name="maxAtk"></param>
        /// <param name="minDef"></param>
        /// <param name="maxDef"></param>
        public void RandomizeCardInfo(int minAtk = 1000, int maxAtk = 3000, int minDef = 1000, int maxDef = 3000)
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
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RandomizeCardDrops()
        {
            foreach (Duelist t1 in Static.Duelist)
            {
                t1.Drop.BcdPow[_random.Next(0, 722)]++;
                t1.Drop.SaPow[_random.Next(0, 722)]++;
                t1.Drop.SaTec[_random.Next(0, 722)]++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RandomizeDuelistDecks()
        {
            foreach (Duelist t1 in Static.Duelist)
            {
                for (int ix = 0; ix < 2048; ix++)
                {
                    t1.Deck[_random.Next(0, 722)]++;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteChangesToFile()
        {
            FileStream fileStream = new FileStream(Static.WaPath, FileMode.Open);

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

            // Writing Random Fusions
            if (Static.RandomFusions)
            {
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
            }

            while (memStream2.Position < 64092L)
            {
                memStream2.WriteByte(238);
            }

            foreach (int num in numArray)
            {
                fileStream.Position = num;
                fileStream.Write(memStream1.ToArray(), 0, 1444);
                fileStream.Write(memStream2.ToArray(), 0, 64092);
            }

            // Close file and memorystream after use
            fileStream.Close();
            memStream2.Close();
            memStream1.Close();

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
                        fileStreamSl.Write(memoryStream.ToArray(), 0, 2888);
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
                                duelistStream.Write(memoryStream.ToArray(), 0, 1444);
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
                                duelistStream.Write(memoryStream2.ToArray(), 0, 1444);
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
                                duelistStream.Write(memoryStream3.ToArray(), 0, 1444);
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
                                duelistStream.Write(memoryStream4.ToArray(), 0, 1444);
                            }
                        }
                    }
                }
            }
        }

        // TODO: Better Log File Logic + HTML/JSON/XML Format
        /// <summary>
        /// 
        /// </summary>
        public void WriteLogFile()
        {
            if (!File.Exists($@"scramblelog_#{_seed}.log"))
            {
                File.CreateText($"scramblelog_#{_seed}.log").Close();
            }

            StreamWriter logStream = new StreamWriter($@"scramblelog_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Fusion Scrambler Log Output ==");
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
        public void WriteHtmlLogFile()
        {
            if (!File.Exists($@"scramblelog_#{_seed}.html"))
            {
                File.CreateText($"scramblelog_#{_seed}.html").Close();
            }

            StreamWriter logStream = new StreamWriter($@"scramblelog_#{_seed}.html");

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

            string table1Template =
                @"<span style=\""text-align: center; font-size: 120%; font-weight: bold; margin-bottom: 5px;\"">Changed Cards</span>
                  <table>
                    <thead>
                    <tr>
                        <th style=\""width: 5%;\"">ID</th>
                        <th style=\""width: 25%;\"">Name</th>
                        <th style=\""width: 10%;\"">Attack/Defense</th>
                        <th style=\""width: 15%;\"">Guardian Stars</th>
                        <th style=\""width: 10%;\"">Attribute</th>
                        <th style=\""width: 10%;\"">Type</th>
                   </tr>
                   </thead>
                 <tbody>";

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

            logStream.WriteLine(template + $" <h1>YU-GI-OH! Forbidden Memories Fusion Scrambler Log Output</h1> <h4>Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion}</h4>");
            logStream.WriteLine(table1Template);

            foreach (Card c in Static.Cards)
            {
                logStream.WriteLine($"<tr><td>#{c.Id}</td> <td>{c.Name}</td> <td>{c.Attack}/{c.Defense}</td> <td>{c.GuardianStar1}/{c.GuardianStar2}</td> <td>{c.Attribute}</td> <td>{c.Type}</td></tr>");

                foreach (Fusion fus in c.Fusions)
                {
                    tmpFusions += "<tr>";
                    tmpFusions += $"<td>{fus.Cards1}</td> <td>{(fus.Cards1 > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards1).Name)}</td> <td>{fus.Cards2}</td> <td>{(fus.Cards2 > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards2).Name)}</td> <td>{fus.Result}</td> <td>{(fus.Result > 722 ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Result).Name)}</td></tr>";
                }
            }
            logStream.WriteLine("</tbody></table><br />");
            logStream.WriteLine(table2Template);
            Console.WriteLine("Writing tmpFusions");
            logStream.WriteLineAsync(tmpFusions).RunSynchronously();
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
        /// <param name="log"></param>
        /// <returns></returns>
        public bool PerformScrambling(int minAtk = 0, int maxAtk = 0, int minDef = 0, int maxDef = 0, bool log = true)
        {
            LoadDataFromSlus();
            LoadDataFromWaMrg();
            RandomizeFusions();
            RandomizeCardInfo(minAtk, maxAtk, minDef, maxDef);
            RandomizeCardDrops();
            RandomizeDuelistDecks();
            WriteChangesToFile();
            if (log)
                WriteLogFile();

            return true;
        }
    }
}
