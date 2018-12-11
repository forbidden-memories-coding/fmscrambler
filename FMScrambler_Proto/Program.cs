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
                    if (!Static.RDict.ContainsKey(k1))
                    {
                        Static.RDict.Add(k1, k2);
                    }
                }
            }

            Console.WriteLine("Start Position? ");
            var startPos = Int64.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
            Console.WriteLine("Length to read? ");
            var readLen = Int64.Parse(Console.ReadLine());


            var memStream = new MemoryStream(File.ReadAllBytes(@"SLUS_014.11")) { Position = startPos };
            String aText = String.Empty;
            while (memStream.Position < (startPos+readLen))
            {
                aText += memStream.GetText(Static.Dict);                
                memStream.Position += 1L;
            }

            string replace = aText.Replace((char) 0xFE, (char)0x0A);

            Console.WriteLine(replace);
            //Console.WriteLine(bText.Length);
        }

    }
}
