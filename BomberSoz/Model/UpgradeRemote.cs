using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberSoz.Model
{
   public class UpgradeRemote : Entity
    {
        public int TimeOnMap { get; set; } = 0;
        public bool NeedClear { get; set; } = false;
    }
}
