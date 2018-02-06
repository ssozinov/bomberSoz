using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberSoz.Model
{
    public class PointInMap
    {
        public PointInMap()
        {

        }
        public PointInMap(int x,int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }

        public int Y { get; set; }


    }
}
