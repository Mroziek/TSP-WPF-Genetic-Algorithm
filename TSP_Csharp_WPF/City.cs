using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Csharp_WPF
{
    public class City
    {
        public int X { get; set; }
        public int Y { get; set; }

        public City(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
