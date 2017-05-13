using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMScrambler.Models
{
    public class Fusion
    {
        public Fusion(int card1, int card2, int result)
        {
            this.Cards1 = card1;
            this.Cards2 = card2;
            this.Result = result;
        }

        public int Cards1 { set; get; }
        public int Cards2 { set; get; }
        public int Result { set; get; }

    }
}
