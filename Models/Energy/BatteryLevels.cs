using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSDN.Energy
{
    public class BatRange
    {
        public int ID { get; set; }
        public int[] Rang = new int[2] { 0, 0 };
        public bool isUpdated { get; set; }
    }

   
}
