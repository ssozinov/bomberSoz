using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberSoz.Model
{
    public class Bomber : Entity
    {
        public Bomber(int live,PointInMap posisition)
        {
            this.LiveCount = live;
        }
        public Bomber()
        {

        }

        public int LiveCount { get; set; }
        public bool IsDead = false;
        public bool IsOnBomb = false;
        public bool NeedChangeImage = false;
        public bool IsHaveRemoteUpgrate = false;
        public bool IsHaveFarUpgrate = false;
        public bool IsHaveMultiBombUpgrade = false;
        public PointInMap Position = new PointInMap();
    }
}
