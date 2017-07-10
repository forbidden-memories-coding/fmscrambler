namespace FMLib.Models
{
    /// <summary>
    /// Fusion Model Class
    /// </summary>
    public class Fusion
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="card1">First Card Material</param>
        /// <param name="card2">Second Card Material</param>
        /// <param name="result">Resulting Fusion</param>
        public Fusion(int card1, int card2, int result)
        {
            this.Cards1 = card1;
            this.Cards2 = card2;
            this.Result = result;
        }

        /// <summary>
        /// First Card Material ID as Integer
        /// </summary>
        public int Cards1 { set; get; }

        /// <summary>
        /// Second Card Material ID as Integer
        /// </summary>
        public int Cards2 { set; get; }

        /// <summary>
        /// Resulting Fusion ID as Integer
        /// </summary>
        public int Result { set; get; }

    }
}
