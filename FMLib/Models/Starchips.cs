using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMLib.Models
{
    /// <summary>
    /// Table of Cost/Password pairs of all cards
    /// </summary>
    public class Starchips
    {
        /// <summary>
        /// Card cost in starchips
        /// </summary>
        public int Cost { get; set; }
        /// <summary>
        /// Password for the card
        /// </summary>
        public int Password { get; set; }
        /// <summary>
        /// Password representation as string
        /// </summary>
        public string PasswordStr { get; set; }
    }
}
