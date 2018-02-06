using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BomberSoz.Core;
using System.Configuration;


namespace BomberSoz.Model
{

    public interface IMap
    {

     
        Entity[,] Load(string Path);
        int Width { get; }
        int Height { get; }
        int[,] ArrayForCleverMonster { get; set; }
        Entity[,] ArrayMap { get; set; }
        PointInMap FirstPositionBomber { get; set; }
        int BombTimer { get; set; }
     

        void SetUpgradeOnMap(int itemCountUpgradeFar, int itemCountUpgradeRemote, int itemCountUpgradeMultiBomb); //тип 1 - больше взрыв, тип 2- взрыв по нажатию клавиши

    }


}


