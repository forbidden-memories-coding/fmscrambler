using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMLib.Models
{
    public class Rank
    {
        public int[] SaPow { get; set; } = new int[722];
        public int[] SaTec { get; set; } = new int[722];
        public int[] BcdPow { get; set; } = new int[722];
    }
}
