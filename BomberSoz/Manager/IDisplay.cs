using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomberSoz.Model;

namespace BomberSoz.NewFolder1
{
    public interface IDisplay
    {
        void DrawMap();
        void CleanMap();

        void ShowLive(int liveCount);
        void ShowGameOver();

        void ShowYouWin();
        void SignalOnDead();
        void SignalOnBombBoom();
        void SignalOnGetBonus();

        
    }
}
