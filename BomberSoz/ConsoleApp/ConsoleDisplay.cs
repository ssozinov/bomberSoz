using System;
using BomberSoz.Core;
using BomberSoz.Model;
using BomberSoz.NewFolder1;
using System.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BomberSoz.ConsoleApp
{
    public class ConsoleDisplay : IDisplay
    {
        private ConsoleMap _map;
        private ConsoleMap _prevMap;
        public ConsoleDisplay(ConsoleMap map)
        {
            this._map = map;
        }


        // Должен отрисовывать только изменения карты
        public void DrawMap()
        {


            Logging.WriteDebbug("Я рисую");
            if (_prevMap == null)
            {
                for (int rowCnt = 0; rowCnt <= _map.Height; rowCnt++)
                {
                    for (int colCnt = 0; colCnt <= _map.Width; colCnt++)
                    {
                        var item = _map.ArrayMap[rowCnt, colCnt];




                        var wall = item as Wall;
                        if (wall != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write(ConsoleConsts.SYMBOL_WALL);
                            Console.ResetColor();
                            continue;
                        }
                        var wallUpgradeFar = item as WallWithUpgradeFar;
                        if (wallUpgradeFar != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write(ConsoleConsts.SYMBOL_WALL);
                            Console.ResetColor();
                            continue;
                        }
                        var wallUpgradeRemote = item as WallWithUpgradeRemote;
                        if (wallUpgradeRemote != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write(ConsoleConsts.SYMBOL_WALL);
                            Console.ResetColor();
                            continue;
                        }
                        var wallUpgradeMultiBomb = item as WallWithUpgradeMultiBomb;
                        if (wallUpgradeMultiBomb != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write(ConsoleConsts.SYMBOL_WALL);
                            Console.ResetColor();
                            continue;
                        }
                        var upgradeFar = item as UpgradeFar;
                        if (upgradeFar != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(ConsoleConsts.SYMBOL_UPGRADE_FAR);
                            Console.ResetColor();
                            continue;
                        }
                        var upgradeRemote = item as UpgradeRemote;
                        if (upgradeRemote != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(ConsoleConsts.SYMBOL_UPGRADE_REMOTE);
                            Console.ResetColor();
                            continue;
                        }
                        var upgradeMultiBomb = item as UpgradeMultiBomb;
                        if (upgradeMultiBomb != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(ConsoleConsts.SYMBOL_UPGRADE_MULTI_BOMB);
                            Console.ResetColor();
                            continue;
                        }
                        var space = item as Space;
                        if (space != null)
                        {
                            Console.Write(ConsoleConsts.SYMBOL_SPACE);
                            continue;
                        }

                        var monster_fly = item as MonsterFly;
                        if (monster_fly != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ConsoleConsts.SYMBOL_MONSTER_ARE_FLY);
                            Console.ResetColor();
                            continue;
                        }
                        var monster_not_fly = item as MonsterNotFly;
                        if (monster_not_fly != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ConsoleConsts.SYMBOL_MONSTER_NOT_FLY);
                            Console.ResetColor();
                            continue;
                        }
                        var monsterClever = item as MonsterClever;
                        if (monsterClever != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ConsoleConsts.SYMBOL_MONSTER_CLEVER);
                            Console.ResetColor();
                            continue;
                        }
                        var bomber = item as Bomber;
                        if (bomber != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.Write(ConsoleConsts.SYMBOL_BOMBER);
                            Console.ResetColor();
                            continue;
                        }
                        var wallAngle = item as WallAngle;
                        if (wallAngle != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write(ConsoleConsts.SYMBOL_MAP_ANGLE);
                            Console.ResetColor();
                            continue;
                        }
                        var wallVert = item as WallVerticale;
                        if (wallVert != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write(ConsoleConsts.SYMBOL_MAP_WALL_VERTICAL);
                            Console.ResetColor();
                            continue;
                        }
                        var wallHoriz = item as WallHorizontal;
                        if (wallHoriz != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write(ConsoleConsts.SYMBOL_MAP_WALL_HORIZONTAL);
                            Console.ResetColor();
                            continue;
                        }





                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Entity item;

                for (int colCnt = 0; colCnt <= _map.Height; colCnt++)
                {
                    for (int rowCnt = 0; rowCnt <= _map.Width; rowCnt++)
                    {
                        if (!_map.ArrayMap[colCnt, rowCnt].Equals(_prevMap.ArrayMap[colCnt, rowCnt]))
                        {
                            WriteToPosition(_map.ArrayMap[colCnt, rowCnt], rowCnt, colCnt);
                        }



                    }
                }

            }



            // Запоминаем прошлую карту
            _prevMap = (ConsoleMap)_map.Clone();
            Console.WindowTop = 0;
            Console.WindowLeft = 0;
            Logging.WriteDebbug("Я дорисовал");
        }

        public void CleanMap()
        {
            Console.Clear();
        }

       
        /// <summary>
        /// Перемещает курсор на указанную позицию и печатает строку
        /// </summary>
        /// <param name="s">символ выводимый на печать</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void WriteToPosition(Entity item, int x, int y)
        {
            try
            {
                Console.SetCursorPosition(x, y);


                if (item is Wall)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(ConsoleConsts.SYMBOL_WALL);
                    Console.ResetColor();
                    return;
                }

                if (item is WallWithUpgradeFar)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(ConsoleConsts.SYMBOL_WALL);
                    Console.ResetColor();
                    return;
                }
                if (item is WallWithUpgradeRemote)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(ConsoleConsts.SYMBOL_WALL);
                    Console.ResetColor();
                    return;
                }
                if (item is WallWithUpgradeMultiBomb)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(ConsoleConsts.SYMBOL_WALL);
                    Console.ResetColor();
                    return;
                }
                if (item is UpgradeRemote)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(ConsoleConsts.SYMBOL_UPGRADE_REMOTE);
                    Console.ResetColor();
                    return;
                }
                if (item is UpgradeFar)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(ConsoleConsts.SYMBOL_UPGRADE_FAR);
                    Console.ResetColor();
                    return;
                }
                if (item is UpgradeMultiBomb)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(ConsoleConsts.SYMBOL_UPGRADE_MULTI_BOMB);
                    Console.ResetColor();
                    return;
                }
                if (item is Space)
                {
                    Console.Write(ConsoleConsts.SYMBOL_SPACE);
                    return;
                }

                if (item is Bomber)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(ConsoleConsts.SYMBOL_BOMBER);
                    Console.ResetColor();
                    return;
                }

                if (item is MonsterFly)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(ConsoleConsts.SYMBOL_MONSTER_ARE_FLY);
                    Console.ResetColor();
                    return;
                }
                if (item is MonsterNotFly)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(ConsoleConsts.SYMBOL_MONSTER_NOT_FLY);
                    Console.ResetColor();
                    return;
                }
                if (item is MonsterClever)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(ConsoleConsts.SYMBOL_MONSTER_CLEVER);
                    Console.ResetColor();
                    return;
                }
                if (item is BomberOnBomb)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(ConsoleConsts.SYMBOL_BOMB);
                    Console.ResetColor();
                    return;
                }
                if (item is Bomb)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(ConsoleConsts.SYMBOL_BOMB);
                    Console.ResetColor();
                    return;
                }
                if (item is BombBoom)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(ConsoleConsts.SYMBOL_BOMB_BANG);
                    Console.ResetColor();
                    return;
                }
                if (item is WallAngle)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(ConsoleConsts.SYMBOL_MAP_ANGLE);
                    Console.ResetColor();
                    return;
                }
                if (item is WallHorizontal)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(ConsoleConsts.SYMBOL_MAP_WALL_HORIZONTAL);
                    Console.ResetColor();

                    return;
                }
                if (item is WallVerticale)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(ConsoleConsts.SYMBOL_MAP_WALL_VERTICAL);
                    Console.ResetColor();
                    return;
                }
                Console.SetCursorPosition((_map.Height) + 3, (_map.Width) + 3);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        public void ShowYouWin()
        {

            Console.Clear();
            Console.Write("YOU WIN");
            Console.ReadKey();

        }

        /// <summary>
        /// для вывода жизек и геймовера
        /// </summary>
        /// <param name="s"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void WriteServiceInformation(string s, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(s);
        //    Console.SetCursorPosition(_map.Height + 3, _map.Width + 3);
        }
        public void ShowLive(int liveCount)
        {
            WriteServiceInformation("Осталось жизней: " + liveCount, 0, _map.Height + 2);
        }

        /// <summary>
        /// считывает  нажатие клавиш и вызывает метод из геймпада соответствующий нажатию
        /// </summary>
        /// <param name="map"></param>
        public void ShowGameOver()

        {
            ConsoleKeyInfo key= new ConsoleKeyInfo();
            Console.WindowLeft = 0;
            Console.WindowTop = 0;
            WriteServiceInformation("GAME OVER", 10,10);
            while (key.Key != ConsoleKey.Enter)
            {
                key = Console.ReadKey();
            }
            
        }

      
        public void SignalOnDead()
        {
            Console.Beep();
        }

        public void SignalOnBombBoom()
        {
          
        }

        public void SignalOnGetBonus()
        {
          
        }
    }
}
