using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FMLib.Utility;

namespace FMScrambler_Proto
{
    class Program
    {
        static void Main(string[] args)
        {
            var strReader = new StringReader(File.ReadAllText(@"./CharacterTable.txt"));

            string input;

            while ((input = strReader.ReadLine()) != null)
            {
                var match = Regex.Match(input, "^([A-Fa-f0-9]{2})\\=(.*)$");

                if (match.Success)
                {
                    var k1 = Convert.ToChar(match.Groups[2].ToString());
                    var k2 = (byte)int.Parse(match.Groups[1].ToString(), NumberStyles.HexNumber);
                    Static.Dict.Add(k2, k1);
                    if (!Static.rDict.ContainsKey(k1))
                    {
                        Static.rDict.Add(k1, k2);
                    }
                }
            }

            var memStream = new MemoryStream(File.ReadAllBytes(@"SLUS_014.11")) { Position = 1710735L };
            String aText = String.Empty;
            while (memStream.Position < 1770321L)
            {
                //Console.WriteLine(memStream.extractPiece(0, 2, -1).extractUInt16(0));
                //Console.WriteLine($"{memStream.Position}");
                //foreach (var x in memStream.extractPiece(0, 1))
                //{
                //    Console.WriteLine($">> {x}");
                //}
                aText += memStream.GetText(Static.Dict);
                //Console.WriteLine($"Current Pos: {memStream.Position}");
                memStream.Position += 1L;
            }

            Console.WriteLine(aText);
            char[] heS = new char[] { (char)250, (char)251, (char)254, (char)255, (char)11 };
            var bText = aText.Split(heS, StringSplitOptions.None);
            Console.WriteLine(bText.Length);
            for (var i = 0; i < bText.Length; i++)
            {
            //    Console.WriteLine($"[{i}] >> {bText[i]}");
            }
        }

    }
}
