using System.Collections.Generic;

namespace FMLib.Models
{
    /// <summary>
    /// Model Class for a Card
    /// </summary>
    public class Card
    {
        /// <summary>
        /// ID of the Card as Integer
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the Card as String
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Card as String
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Attack Value of the Card as Integer
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Defense Value of the Card as Integer
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// Attribute of the Card as Integer
        /// </summary>
        public int Attribute { get; set; }

        /// <summary>
        /// Level of the Card as Integer
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Type of the Card as Integer
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Guardian Star of the Card as Integer Array of size 2
        /// </summary>
        public int GuardianStar1 { get; set; }
        public int GuardianStar2 { get; set; }
        /// <summary>
        /// Fusions of the Card as a List
        /// </summary>
        public List<Fusion> Fusions { get; set; } = new List<Fusion>();

        /// <summary>
        /// Equips of the Card as a List
        /// </summary>
        public List<int> Equips { get; set; }

        /// <summary>
        /// Ritual of the Card as Ritual
        /// </summary>
        public Ritual Rituals { get; set; }

        /// <summary>
        /// Starchip cost of the Card as Integer
        /// </summary>
        public int Starchips { get; set; }

        /// <summary>
        /// Password of the Card as String
        /// </summary>
        public string Code { get; set; }
    }
}