using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Versioning;
using FMScrambler.Models;


namespace FMScrambler.helper
{
    public static class Static
    {
        public static bool glitchFusions = false;
        public static bool highID = false;
        public static string versionInfo = "0.1 Alpha";

        public static int cardCount;
        public static Card[] Cards = new Card[722];

        public static void setCardCount(int c)
        {
            cardCount = c;
   
        }

        public static string GameBinPath = string.Empty;
        public static string SLUSPath;
        public static string WAPath;

        public static string[] CharTable { get; private set; } = File.ReadLines("./CharacterTable.txt").ToArray();

        public static Dictionary<byte, char> Dict = new Dictionary<byte, char>();
        public static Dictionary<char, byte> rDict = new Dictionary<char, byte>();

        
    }
}