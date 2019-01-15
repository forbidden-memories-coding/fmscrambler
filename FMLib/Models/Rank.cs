using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMLib.Models
{
    /// <summary>
    /// 
    /// </summary>
    public enum DropType
    {
        /// <summary>
        /// 
        /// </summary>
        SAPOW,
        /// <summary>
        /// 
        /// </summary>
        SATEC,
        /// <summary>
        /// 
        /// </summary>
        BCDPOW
    }
    /// <summary>
    /// 
    /// </summary>
    public class Rank
    {
        /// <summary>
        /// 
        /// </summary>
        public int[] SaPow { get; set; } = new int[722];
        /// <summary>
        /// 
        /// </summary>
        public int[] SaTec { get; set; } = new int[722];
        /// <summary>
        /// 
        /// </summary>
        public int[] BcdPow { get; set; } = new int[722];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public int[] GetDropArray(DropType type)
        {
            switch (type)
            {
                case DropType.SAPOW:
                    return SaPow;
                case DropType.SATEC:
                    return SaTec;
                case DropType.BCDPOW:
                    return BcdPow;
                default:
                    throw new ApplicationException("Attempting unexisting drop type");
            }
        }
    }
}
