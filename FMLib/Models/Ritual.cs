namespace FMLib.Models
{
    /// <summary>
    /// Ritual Model Class
    /// </summary>
    public class Ritual
    {
        /// <summary>
        /// Card ID of the Ritual Equip as Integer
        /// </summary>
        public int RitualCard { set; get; }

        /// <summary>
        /// IDs of Cards required for the Ritual as Integer Array
        /// </summary>
        public int[] Cards { set; get; }

        /// <summary>
        /// Resulting Ritual Card ID
        /// </summary>
        public int Result { set; get; }
    }
}
