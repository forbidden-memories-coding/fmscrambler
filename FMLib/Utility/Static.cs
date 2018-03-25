using System;
using System.Collections.Generic;
using FMLib.Models;

namespace FMLib.Utility
{
    /// <summary>
    /// Static Variables for application wide useage
    /// </summary>
    public static class Static
    {
        /// <summary>
        /// Encoded as Base64 for simplicity, auto-decode to string array
        /// </summary>
        public static string[] CharTable = Convert.FromBase64String("MTg9QQ0KMkQ9Qg0KMkI9Qw0KMjA9RA0KMjU9RQ0KMzE9Rg0KMjk9Rw0KMjM9SA0KMUE9SQ0KM0I9Sg0KMzM9Sw0KMkE9TA0KMUU9TQ0KM" +
            "kM9Tg0KMjE9Tw0KMkY9UA0KM0U9UQ0KMjY9Ug0KMUQ9Uw0KMUM9VA0KMzU9VQ0KMzk9Vg0KMjI9Vw0KNDY9WA0KMjQ9WQ0KM0Y9Wg0KMDM9YQ0KMTU9Yg0KMEY9Yw0KMEM9ZA0KMDE9ZQ" +
            "0KMTM9Zg0KMTA9Zw0KMDk9aA0KMDU9aQ0KMzQ9ag0KMTY9aw0KMEE9bA0KMEU9bQ0KMDY9bg0KMDQ9bw0KMTQ9cA0KMzc9cQ0KMDg9cg0KMDc9cw0KMDI9dA0KMEQ9dQ0KMTk9dg0KMTI9d" +
            "w0KMzY9eA0KMTE9eQ0KMzI9eg0KMzg9MA0KM0Q9MQ0KM0E9Mg0KNDE9Mw0KNEE9NA0KNDI9NQ0KNEU9Ng0KNDU9Nw0KNTc9OA0KNTk9OQ0KMDA9IA0KMzA9LQ0KM0M9Iw0KNDM9Jg0KMEI9L" +
            "g0KMUY9LA0KNTU9YQ0KMTc9IQ0KMUI9Jw0KMjc9PA0KMjg9Pg0KMkU9Pw0KNDQ9Lw0KNDg9Og0KNEI9KQ0KNEM9KA0KNEY9JA0KNTA9Kg0KNTE9Pg0KNTQ9PA0KNDA9Ig0KNTY9Kw0KNUI9JQ==")
            .ToString().Split(Environment.NewLine.ToCharArray());

        /// <summary>
        /// Option - Glitchy Fusions allowed? (true / false)
        /// </summary>
        public static bool glitchFusions = false;

        /// <summary>
        /// Option - Card ID higher than Card1 allowed? (true / false)
        /// </summary>
        public static bool highID = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool randomATKDEF = false;
        /// <summary>
        /// Count of Cards as Integer
        /// </summary>
        public static int cardCount;

        /// <summary>
        /// Card Array for all the 722 Cards in the game
        /// </summary>
        public static Card[] Cards = new Card[722];

        public static Duelist[] Duelist = new Duelist[39];
        /// <summary>
        /// Method to set the Card Count
        /// </summary>
        /// <param name="c">Card Count as Integer</param>
        public static void setCardCount(int c) { cardCount = c; }

        /// <summary>
        /// Path to the Game Folder
        /// </summary>
        public static string GameBinPath = string.Empty;

        /// <summary>
        /// File - Executable - Path to the SLUS Binary file of the game
        /// </summary>
        public static string SLUSPath;

        /// <summary>
        /// File - Data - Path to the WA_MRG.MRG data file of the game
        /// </summary>
        public static string WAPath;

        /// <summary>
        /// Helper - Dictionary to map chars from data to readable chars
        /// </summary>
        public static Dictionary<byte, char> Dict = new Dictionary<byte, char>();

        /// <summary>
        /// Helper - Dictionary to map chars from data to readable chars
        /// </summary>
        public static Dictionary<char, byte> rDict = new Dictionary<char, byte>();

        
    }
}