using System;
using System.Threading.Tasks;
using BomberSoz.ConsoleApp;
using BomberSoz.NewFolder1;
using System.Configuration;
using BomberSoz.Core;
using System.Collections.Generic;
using BomberSoz.Manager;

namespace BomberSoz
{

    class MainClass
    {


        static void Main(string[] args)
        {
            

            var mapManager = new MapManager(ConfigurationManager.AppSettings["pathToMapDirectory"],ConfigurationManager.AppSettings["pathToMapFile"]);
            var map = new ConsoleMap(int.Parse(ConfigurationManager.AppSettings["widthField"]), int.Parse(ConfigurationManager.AppSettings["heightField"]), int.Parse(ConfigurationManager.AppSettings["timeToBoom"]));
            var Bomber = new Model.Bomber(int.Parse(ConfigurationManager.AppSettings["liveCount"]),map.FirstPositionBomber);

            map.Load(mapManager.GetFirstMap());
         
            map.SetUpgradeOnMap(10,10,10);
            var displayConsole = new ConsoleDisplay(map);


            var gamepadConsole = new ConsoleGamePad();
            Console.CursorVisible = false;
            Task.Run(() => gamepadConsole.Start());


            var mC = new MotionController(displayConsole, gamepadConsole, map, Bomber,mapManager);
            mC.SetPositionForPerson(4, 2);
            mC.SetPositionForPerson(3, 2);
            mC.SetPositionForPerson(2, 1);
            displayConsole.DrawMap();
       
          //  mC.SetPositionForPerson(3, 3);
            displayConsole.ShowLive(Bomber.LiveCount);
            Console.WindowLeft = 0;
            Console.WindowTop = 0;

            mC.Start();

         //   Console.ReadKey();
        }


    }
}
