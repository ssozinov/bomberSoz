using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberSoz.Model
{
    public class Entity
    {

        public PointInMap Position = new PointInMap();

        public byte TypeMonsters;// 1- игрок, 2 - противник 1, 3 - противник 2
        public void SetPosition(int x, int y)
        {
            Position.X = x;
            Position.Y = y;
        }

    }
}
