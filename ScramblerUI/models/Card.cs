using System.Collections.Generic;


namespace FMScrambler.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Attribute { get; set; }
        public int Level { get; set; }
        public int Type { get; set; }

        public int[] GuardianStar { get; set; }

        public List<Fusion> Fusions { get; set; } = new List<Fusion>();
        public List<int> Equips { get; set; }
        public Ritual Rituals { get; set; }

        public int Starchips { get; set; }
        public string Code { get; set; }



    }
}