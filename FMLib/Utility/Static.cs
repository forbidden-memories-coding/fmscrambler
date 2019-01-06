using System;
using System.Collections.Generic;
using FMLib.Models;

namespace FMLib.Utility
{
    /// <summary>
    /// Static Variables for application wide useage
    /// </summary>
    public class Static
    {
        /// <summary>
        /// Options (true / false)
        /// </summary>

        public static bool RandomFusions = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool GlitchFusions = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomAttributes = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomGuardianStars = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomCardDrops = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomTypes = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomStarchips = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool GlitchGuardianStars = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomDecks = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool GlitchAttributes = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomEquips = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool HighId = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomAtkdef = false;

        /// <summary>
        /// Count of Cards as Integer
        /// </summary>
        public static int CardCount;

        /// <summary>
        /// Card Array for all the 722 Cards in the game
        /// </summary>
        public static Card[] Cards = new Card[722];

        /// <summary>
        /// 
        /// </summary>
        public static Duelist[] Duelist = new Duelist[39];

        /// <summary>
        /// Method to set the Card Count
        /// </summary>
        /// <param name="c">Card Count as Integer</param>
        public static void SetCardCount(int c)
        {
            CardCount = c;
        }

        /// <summary>
        /// Path to the Game Folder
        /// </summary>
        public static string GameBinPath = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public static string SlusPath;
        /// <summary>
        /// 
        /// </summary>
        public static string WaPath;
        /// <summary>
        /// 
        /// </summary>
        public static string IsoPath;

        /// <summary>
        /// 
        /// </summary>
        public static bool UsedIso = false;
        /// <summary>
        /// 
        /// </summary>
        public static string RandomizerFileName;

        /// <summary>
        /// Helper - Dictionary to map chars from data to readable chars
        /// </summary>
        public static Dictionary<byte, char> Dict = new Dictionary<byte, char>();
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<char, byte> RDict = new Dictionary<char, byte>();
    }
}