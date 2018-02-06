using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberSoz.Model
{
    public class Bomb : Entity
    {
        public Bomb(PointInMap position, int timeToBoom)
        {
            this.TimeToBoom = timeToBoom;
            this.Position = position;
        }
        public Bomb()
        {

        }

        public int TimeToBoom { get; set; }
        public int CurrentTime { get; set; }


    }
}
