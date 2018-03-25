using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMLib.Models
{
    public class Rank
    {
        public int[] SAPow { get; set; } = new int[722];
        public int[] SATec { get; set; } = new int[722];
        public int[] BCDPow { get; set; } = new int[722];
    }
}
