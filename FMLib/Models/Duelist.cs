using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMLib.Models
{
    public class Duelist
    {
        public Duelist(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public int[] Deck { get; set; } = new int[722];
        public Rank Drop { get; set; } = new Rank();
    }
}
