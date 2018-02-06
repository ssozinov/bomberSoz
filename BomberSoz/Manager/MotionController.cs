using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomberSoz.Model;
using BomberSoz.Core;
using BomberSoz.Manager;
using System.Diagnostics;

namespace BomberSoz.NewFolder1
{

    public class MotionController
    {

        private bool isPaused = false;
        private Random _rand = new Random((int)DateTime.Now.Ticks);
        private IMap _map;
        private readonly IDisplay _display;
        private readonly IGamePad _gamePad;
        private readonly MapManager _mapManager;
        private Model.Bomber _bomber;
        private object _lock = new object();
        public List<Bomb> ListWithBomb = new List<Bomb>();
        public List<PointInMap> ListWithPointToCrash;
        public PointInMap nextPoint;
        public List<Monster> ListWithMonster = new List<Monster>();
        public bool isWantDrow = false;
        public bool isSecondNow = false;
        public bool isDrawNow = false;



        public MotionController(IDisplay display, IGamePad gamePad, IMap map, Bomber bomber, MapManager mapManager)
        {
            _display = display;
            _gamePad = gamePad;
            _map = map;
            _bomber = bomber;
            _mapManager = mapManager;
        }



        // Запускает игру
        public void Start()
        {


            int now = DateTime.Now.Second;

            _gamePad.PressDown = () => MoveBomber(1, 0);

            _gamePad.PressLeft = () => MoveBomber(0, -1);
            _gamePad.PressRight = () => MoveBomber(0, 1);

            _gamePad.PressUp = () => MoveBomber(-1, 0);

            _gamePad.PressSpace = () => OnPressSpace();

            _gamePad.PressR = () => RemoteBombBoom();
            _gamePad.PressP = () => PressPause(isPaused);



            while (true)
            {

               if (ListWithMonster.Count == 0)
                {
                if (_mapManager.CheckAvailableMap())
                 {
                       GetNextLevel();
                    }
                  else
                    {
                        _display.ShowYouWin();

                        break;
                    }
                }
              







                if (_bomber.IsDead) //если бомбера убили, то рисуем его на первочальной позиции
                {
                    Logging.WriteDebbug("Пытаюсь помереть");
                    _display.SignalOnDead();
                    _bomber.IsHaveFarUpgrate = false;
                    _bomber.IsHaveRemoteUpgrate = false;
                    _bomber.IsHaveMultiBombUpgrade = false;
                    _bomber.LiveCount -= 1;
                    _display.ShowLive(_bomber.LiveCount);

                    _bomber.IsDead = false;
                    _map.ArrayMap[_map.FirstPositionBomber.X, _map.FirstPositionBomber.Y] = _bomber;
                    isWantDrow = true;

                }

                if (_bomber.LiveCount <= 0) //если нет жизней ,то заканчиваем игру
                {
                    Logging.WriteTrace("Конец игры");
                    _display.CleanMap();

                    isWantDrow = true;
                    _display.ShowGameOver();
                    break;

                }

                if (_bomber.IsOnBomb)
                {
                    var currentPosition = new PointInMap();
                    currentPosition = GetCurrentPosition(_map);
                    if (!_bomber.NeedChangeImage)
                    {
                        _bomber.NeedChangeImage = true;
                        _map.ArrayMap[currentPosition.X, currentPosition.Y] = new BomberOnBomb();
                    }
                    else
                    {
                        _bomber.NeedChangeImage = false;
                        _map.ArrayMap[currentPosition.X, currentPosition.Y] = _bomber;
                    }
                    isWantDrow = true;
                    System.Threading.Thread.Sleep(50);
                }
                if (now != DateTime.Now.Second)
                {

                    Logging.WriteInfo("прошла секунда, шагаю");

                    now = DateTime.Now.Second;

                    UpgradeUpTime();
                    for (int i = 0; i < ListWithMonster.Count; i++)
                    {
                        SetNewPositionMonster(ListWithMonster[i]);
                    }

                    int bombCount = 0;
                    while (bombCount != ListWithBomb.Count)
                    {
                        var bomb = ListWithBomb[bombCount];
                        bomb.CurrentTime += 1;
                        if (bomb.CurrentTime == bomb.TimeToBoom)
                        {

                            if (!_bomber.IsHaveRemoteUpgrate)
                            {
                                OnBombBoom(bomb);
                                ListWithBomb.Remove(bomb);
                                bombCount--;
                            }
                        }
                        bombCount++;
                    }
                    isSecondNow = true;
                     } 
                    if (!isDrawNow)
                    {
                        if (((isWantDrow) || (isSecondNow)) || ((isSecondNow) && (isWantDrow)))
                        {
                            isWantDrow = false;
                            isSecondNow = false;
                            isDrawNow = true;
                            lock (_lock)
                            {
                                System.Diagnostics.Stopwatch sw = new Stopwatch();
                                sw.Start();
                                _display.DrawMap();
                                sw.Stop();
                                Logging.WriteTrace("Время выполнения" + (sw.ElapsedMilliseconds / 100.0).ToString());
                            }
                            isDrawNow = false;
                        }

                    }
                }
            }
        




        public void RemoteBombBoom()
        {

            if ((ListWithBomb.Count != 0) && (_bomber.IsHaveRemoteUpgrate))
            {
                int bombCount = 0;
                while (bombCount != ListWithBomb.Count)
                {
                    OnBombBoom(ListWithBomb[bombCount]);
                    ListWithBomb.Remove(ListWithBomb[bombCount]);
                }
            }
        }
        public void OnBombBoom(Bomb bomb)
        {
            var currentPosition = bomb.Position;


            if (((bomb.Position.X == _bomber.Position.X) && (bomb.Position.Y == _bomber.Position.Y)) || (BuildMapAfterBoom(GetPointAroundBomb(bomb.Position, _bomber.IsHaveFarUpgrate))))
            {

                _bomber.IsOnBomb = false;
                _bomber.IsDead = true;
            }
            _map.ArrayMap[bomb.Position.X, bomb.Position.Y] = new Space();

        }

        public int GetMonsterForPosition(PointInMap point)
        {
            int cnt = 0;
            foreach (var item in ListWithMonster)
            {

                if ((item.Position.X == point.X) && (item.Position.Y == point.Y))
                {
                    break;

                }
                cnt++;
            }
            return cnt;
        }

        public bool BuildMapAfterBoom(List<PointInMap> listWithPoint) /// если бомбера зацепило,возвращает True . Рисует пробелы на месте взрыва
        {
            bool result = false;


            foreach (var point in listWithPoint)
            {
                var item = _map.ArrayMap[point.X, point.Y];




                var wall = item as Wall;
                if (wall != null)
                {

                    continue;
                }
                var wallUpgradeFar = item as WallWithUpgradeFar;
                if (wallUpgradeFar != null)
                {

                    continue;
                }
                var wallUpgradeMultiBomb = item as WallWithUpgradeMultiBomb;
                if (wallUpgradeMultiBomb != null)
                {

                    continue;
                }
                var wallUpgradeRemote = item as WallWithUpgradeRemote;
                if (wallUpgradeRemote != null)
                {

                    continue;
                }
                var space = item as Space;
                if (space != null)
                {

                    continue;
                }

                var monster_fly = item as MonsterFly;
                if (monster_fly != null)
                {

                    ListWithMonster.RemoveAt(GetMonsterForPosition(point));
                    continue;

                }

                var monster_not_fly = item as MonsterNotFly;
                if (monster_not_fly != null)
                {

                    ListWithMonster.RemoveAt(GetMonsterForPosition(point));

                    continue;
                }
                var monsterClever = item as MonsterClever;
                if (monsterClever != null)
                {

                    ListWithMonster.RemoveAt(GetMonsterForPosition(point));

                    continue;
                }
                var bomber = item as Bomber;
                if (bomber != null)
                {

                    result = true;
                    continue;
                }

            }

            ShowBombBang(listWithPoint);
           
            return result;
        }
        public void ShowBombBang(List<PointInMap> listWithPointToDraw)
        {
            var dictionaryWithPrevPointMap = new Dictionary<Entity, PointInMap>();
            foreach (var item in listWithPointToDraw)
            {
                dictionaryWithPrevPointMap.Add((Entity)_map.ArrayMap[item.X, item.Y], item);
                _map.ArrayMap[item.X, item.Y] = new BombBoom();
            }

            _display.DrawMap();
            _display.SignalOnBombBoom();
            System.Threading.Thread.Sleep(300);

            changeImageBombBoom(dictionaryWithPrevPointMap);
        }
        public void changeImageBombBoom(Dictionary<Entity, PointInMap> dictionaryWithPointToDraw)
        {

            foreach (KeyValuePair<Entity, PointInMap> entry in dictionaryWithPointToDraw)
            {
                var item = entry.Key;
                var wall = item as Wall;
                if (wall != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new Space();
                }
                var wallUpgradeFar = item as WallWithUpgradeFar;
                if (wallUpgradeFar != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new UpgradeFar();
                }
                var wallUpgradeRemote = item as WallWithUpgradeRemote;
                if (wallUpgradeRemote != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new UpgradeRemote();
                }
                var wallUpgradeMultiBomb = item as WallWithUpgradeMultiBomb;
                if (wallUpgradeMultiBomb != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new UpgradeMultiBomb();
                }
                var itemSpace = item as Space;
                if (itemSpace != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new Space();
                }
                var monsterFly = item as MonsterFly;
                if (monsterFly != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new Space();
                }
                var monsterNotFly = item as MonsterNotFly;
                if (monsterNotFly != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new Space();
                }
                var monsterClever = item as MonsterClever;
                if (monsterClever != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new Space();
                }
                var bomber = item as Bomber;
                if (bomber != null)
                {
                    _map.ArrayMap[entry.Value.X, entry.Value.Y] = new Space();
                }
            }

        }
        public void OnPressSpace()
        {

            if (ListWithBomb.Count == 0)
            {
                _bomber.IsOnBomb = true;
                ListWithBomb.Add(new Bomb(GetCurrentPosition(_map), _map.BombTimer));
            }
            else
            {
                if (_bomber.IsHaveMultiBombUpgrade)
                {
                    _bomber.IsOnBomb = true;
                    ListWithBomb.Add(new Bomb(GetCurrentPosition(_map), _map.BombTimer));
                }
            }

        }

        /// <summary>
        /// Здесь ищем будущую позицию для монстра 
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public PointInMap SearchNextPositionToCurrentMonster(Monster monster)
        {

            var freePositionForMonster = new Dictionary<int, PointInMap>();
            int nextDirection = 0;
            //  long tryUseRandom;
            bool positionIsFind = false;

            while (!positionIsFind)
            {

                nextDirection = _rand.Next(1, 5);

                switch (monster.Direction)
                {

                    case 1:
                        nextPoint = new PointInMap();
                        nextPoint.X = monster.Position.X - 1;
                        nextPoint.Y = monster.Position.Y;
                        Logging.WriteInfo("Текущая точка для проверки " + nextPoint.X + "   " + nextPoint.Y);
                        Logging.WriteInfo("Текущая позиция монстра " + monster.Position.X + "   " + monster.Position.Y);
                        if (CheckPosition(nextPoint, monster, _bomber))
                        {

                            positionIsFind = true;

                            return nextPoint;
                        }
                        else
                        {
                            Logging.WriteInfo("не смог шагнуть по вертикали " + nextPoint.X + " по горизонтали  " + nextPoint.Y + " " + " следующая попытка  в напрвалении" + nextDirection);
                            monster.Direction = nextDirection;
                        }
                        break;
                    case 2:
                        nextPoint = new PointInMap();
                        nextPoint.X = monster.Position.X + 1;
                        nextPoint.Y = monster.Position.Y;
                        Logging.WriteInfo("Текущая точка для проверки " + nextPoint.X + "   " + nextPoint.Y);
                        Logging.WriteInfo("Текущая позиция монстра " + monster.Position.X + "   " + monster.Position.Y);
                        if (CheckPosition(nextPoint, monster, _bomber))
                        {
                            positionIsFind = true;

                            return nextPoint;
                        }
                        else
                        {
                            Logging.WriteInfo("не смог шагнуть по вертикали " + nextPoint.X + " по горизонтали  " + nextPoint.Y + " " + " следующая попытка  в напрвалении" + nextDirection);
                            monster.Direction = nextDirection;
                        }
                        break;
                    case 3:
                        nextPoint = new PointInMap();
                        nextPoint.X = monster.Position.X;
                        nextPoint.Y = monster.Position.Y - 1;
                        Logging.WriteInfo("Текущая точка для проверки " + nextPoint.X + "   " + nextPoint.Y);
                        Logging.WriteInfo("Текущая позиция монстра " + monster.Position.X + "   " + monster.Position.Y);
                        if (CheckPosition(nextPoint, monster, _bomber))
                        {

                            positionIsFind = true;

                            return nextPoint;
                        }
                        else
                        {
                            Logging.WriteInfo("не смог шагнуть по вертикали " + nextPoint.X + " по горизонтали  " + nextPoint.Y + " " + " следующая попытка  в напрвалении" + nextDirection);
                            monster.Direction = nextDirection;
                        }
                        break;
                    case 4:
                        nextPoint = new PointInMap();
                        nextPoint.X = monster.Position.X;
                        nextPoint.Y = monster.Position.Y + 1;
                        Logging.WriteInfo("Текущая точка для проверки " + nextPoint.X + "   " + nextPoint.Y);
                        Logging.WriteInfo("Текущая позиция монстра " + monster.Position.X + "   " + monster.Position.Y);
                        if (CheckPosition(nextPoint, monster, _bomber))
                        {
                            positionIsFind = true;
                            return nextPoint;
                        }
                        else
                        {
                            Logging.WriteInfo("не смог шагнуть на " + nextPoint.X + " " + nextPoint.Y + " " + " следующая попытка " + nextDirection);
                            monster.Direction = nextDirection;
                        }
                        break;

                }

            }

            return nextPoint;
        }
        public int GetDirectionForPoint(PointInMap prevPoint, PointInMap nextPoint)
        {
            if (nextPoint.X < prevPoint.X)
            {
                return 1;
            }
            if (nextPoint.X > prevPoint.X)
            {
                return 2;
            }
            if (nextPoint.Y < prevPoint.Y)
            {
                return 3;
            }
            if (nextPoint.Y > prevPoint.Y)
            {
                return 4;
            }
            return 1;

        }
        /// <summary>
        /// ищем новую позицию для монстра и рисуем монстра
        /// </summary>
        /// <param name="monster"></param>


        /// <summary>
        /// выбор начальной позиции для существ  
        /// </summary>
        /// <param name="typeMonster">тип монстра 2-не переходит через бомбу, 3-проъодит через бомбу</param>
        /// <param name="countMonster">количество монстров,которые надо расположить на карте</param>
        /// <returns>карту</returns>
        /// 
        public Entity[,] SetPositionForPerson(byte typeMonster, byte countMonster)
        {

            var pointList = new List<PointInMap>();

            PointInMap point;
            for (byte rowCnt = 1; rowCnt < _map.Height; rowCnt++)
            {

                for (byte colCnt = 1; colCnt < _map.Width; colCnt++)
                {

                    if (_map.ArrayMap[rowCnt, colCnt] is Space)
                    {
                        point = new PointInMap();
                        point.X = rowCnt;
                        point.Y = colCnt;
                        pointList.Add(point);

                    }

                }

            }

            for (int monsterCount = 0; monsterCount < countMonster; monsterCount++)
            {
                point = new PointInMap();
                point = pointList[_rand.Next(pointList.Count)];
                if (typeMonster == 2)
                {

                    var currentMonster = new Monster(false, 2);
                    _map.ArrayMap[point.X, point.Y] = new MonsterNotFly();
                    currentMonster.SetPosition(point.X, point.Y);
                    ListWithMonster.Add(currentMonster);

                }
                if (typeMonster == 3)
                {
                    var currentMonster = new Monster(true, 3);
                    _map.ArrayMap[point.X, point.Y] = new MonsterFly();
                    currentMonster.SetPosition(point.X, point.Y);
                    ListWithMonster.Add(currentMonster);
                }
                if (typeMonster == 4)
                {
                    var currentMonster = new Monster(true, 4);
                    _map.ArrayMap[point.X, point.Y] = new MonsterClever();
                    currentMonster.SetPosition(point.X, point.Y);
                    ListWithMonster.Add(currentMonster);
                }
                pointList.Remove(point);
            }



            return _map.ArrayMap;
        }

        /// <summary>
        /// вовзращает массив монстров с координатами
        /// </summary>
        public List<Monster> ReturnListOfMonster()
        {
            return ListWithMonster;
        }

        /// <summary>
        /// проверяет пустая ли указанная клетка
        /// </summary>
        /// <param name="point">точка на карте</param>
        /// <returns></returns>
        public bool CheckPosition(PointInMap point, Monster monster)
        {
            if ((point.X < 0) || (point.X >= _map.Height) || (point.Y < 0) || (point.Y >= _map.Width))
            {
                Logging.WriteInfo("Попытка выхода за пределы карты!");
                return false;
            }


            if (_map.ArrayMap[point.X, point.Y] is Space)
            {

                return true;

            }
            else
            {
                if ((monster.TypeMonsters == 3) && (_map.ArrayMap[point.X, point.Y] is Bomb))
                {
                    return true;
                }
                else
                {
                    Logging.WriteInfo("Не может шагнуть,упирается в припятствие");
                    return false;
                }
            }

        }
        public bool CheckPositionForBomber(PointInMap point)
        {
            if ((point.X < 0) || (point.X >= _map.Height) || (point.Y < 0) || (point.Y >= _map.Width))
            {
                Logging.WriteInfo("Попытка выхода за пределы карты!");
                return false;
            }


            if (_map.ArrayMap[point.X, point.Y] is Space)
            {

                return true;

            }
            if (_map.ArrayMap[point.X, point.Y] is UpgradeFar)
            {
                _bomber.IsHaveFarUpgrate = true;
                _display.SignalOnGetBonus();
                return true;

            }
            if (_map.ArrayMap[point.X, point.Y] is UpgradeRemote)
            {
                _bomber.IsHaveRemoteUpgrate = true;
                _display.SignalOnGetBonus();
                return true;

            }
            if (_map.ArrayMap[point.X, point.Y] is UpgradeMultiBomb)
            {
                _bomber.IsHaveMultiBombUpgrade = true;
                _display.SignalOnGetBonus();

                return true;

            }
            else
            {
                Logging.WriteInfo("Не может шагнуть,упирается в припятствие");
                return false;
            }

        }


        public bool CheckPosition(PointInMap point, Monster monster, Bomber bomber)
        {
            bool isNotOutRange = ((point.X < 0) || (point.X >= _map.Height) || (point.Y < 0) || (point.Y >= _map.Width));
            if ((!CheckPosition(point, monster)) && (!isNotOutRange))
            {
                if (_map.ArrayMap[point.X, point.Y] is Bomber)
                {
                    // _display.DrawMap();
                    bomber.IsDead = true;
                    return true;

                }
                if (_map.ArrayMap[point.X, point.Y] is Bomb)
                {

                    return false;
                }
                if ((_map.ArrayMap[point.X, point.Y] is Bomb) && (monster.TypeMonsters == 3))
                {
                    switch (monster.Direction)
                    {
                        case 1:
                            nextPoint.X = monster.Position.X - 2;
                            nextPoint.Y = monster.Position.Y;
                            return CheckPosition(nextPoint, monster);

                        case 2:
                            nextPoint.X = monster.Position.X + 2;
                            nextPoint.Y = monster.Position.Y;
                            return CheckPosition(nextPoint, monster);

                        case 3:
                            nextPoint.X = monster.Position.X;
                            nextPoint.Y = monster.Position.Y - 2;
                            return CheckPosition(nextPoint, monster);

                        case 4:
                            nextPoint.X = monster.Position.X;
                            nextPoint.Y = monster.Position.Y + 2;
                            return CheckPosition(nextPoint, monster);
                    }
                }
                if ((_map.ArrayMap[point.X, point.Y] is MonsterNotFly) || (_map.ArrayMap[point.X, point.Y] is MonsterFly))
                {
                    switch (monster.Direction)
                    {
                        case 1:
                            nextPoint.X = monster.Position.X - 2;
                            nextPoint.Y = monster.Position.Y;
                            return CheckPosition(nextPoint, monster);

                        case 2:
                            nextPoint.X = monster.Position.X + 2;
                            nextPoint.Y = monster.Position.Y;
                            return CheckPosition(nextPoint, monster);

                        case 3:
                            nextPoint.X = monster.Position.X;
                            nextPoint.Y = monster.Position.Y - 2;
                            return CheckPosition(nextPoint, monster);

                        case 4:
                            nextPoint.X = monster.Position.X;
                            nextPoint.Y = monster.Position.Y + 2;
                            return CheckPosition(nextPoint, monster);
                    }
                }
                return false;
            }
            else
            {
                if (!isNotOutRange)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Возвращает позицию где стоит бомбермен
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public PointInMap GetCurrentPosition(IMap map)
        {
            var point = new PointInMap();
            for (byte rowCnt = 1; rowCnt < _map.Height; rowCnt++)
            {

                for (byte colCnt = 1; colCnt < _map.Width; colCnt++)
                {

                    if ((_map.ArrayMap[rowCnt, colCnt] is Bomber) || (_map.ArrayMap[rowCnt, colCnt] is BomberOnBomb))
                    {

                        point.X = rowCnt;
                        point.Y = colCnt;
                    }

                }
            }
            return point;
        }

        /// <summary>
        /// перестраивает и перересовывает карту после нажатия одной из клавиш управления
        /// </summary>
        /// <param name="x">смещение по оси X</param>
        /// <param name="y">смещение по оси Y</param>
        public bool MoveBomber(int x = 0, int y = 0)
        {
            lock (_lock)
            {
                var newPosition = new PointInMap();
                var beforePosition = this.GetCurrentPosition(_map);
                newPosition.X = beforePosition.X + x;
                newPosition.Y = beforePosition.Y + y;
                if ((newPosition.X >= 1) && (newPosition.Y >= 1) && (newPosition.X < _map.Height) &&
                    (newPosition.Y < _map.Width))
                {
                    if ((_map.ArrayMap[newPosition.X, newPosition.Y] is MonsterFly) ||
                        (_map.ArrayMap[newPosition.X, newPosition.Y] is MonsterNotFly) || (_map.ArrayMap[newPosition.X, newPosition.Y] is MonsterClever))
                    {

                        _map.ArrayMap[beforePosition.X, beforePosition.Y] = new Space();
                        _bomber.Position = _map.FirstPositionBomber;
                        _bomber.IsDead = true;

                        return false;
                    }
                    else
                    {
                        if (CheckPositionForBomber(newPosition))
                        {
                            if (_bomber.IsOnBomb)
                            {
                                _map.ArrayMap[beforePosition.X, beforePosition.Y] = new Bomb();
                            }
                            else
                            {
                                _map.ArrayMap[beforePosition.X, beforePosition.Y] = new Space();
                            }

                            _map.ArrayMap[newPosition.X, newPosition.Y] = _bomber;
                            _bomber.Position.X = newPosition.X;
                            _bomber.Position.Y = newPosition.Y;
                            isWantDrow = true;
                        }
                    }
                }
                _bomber.IsOnBomb = false;

                //  _display.DrawMap();

                return true;

            }
        }

        public List<PointInMap> GetPointAroundBomb(PointInMap currentPoint, bool isDoubleBoom) // возвращаем список клеток ,которые заденут бомбу - шаг взрыва 1
        {
            PointInMap point = new PointInMap();
            List<PointInMap> returnList = new List<PointInMap>();
            int startI = 1;
            int startJ = 1;
            if (isDoubleBoom)
            {
                startI = 2;
                startJ = 2;
            }
            for (var i = -(startI); i <= (startJ); i++)
            {

                for (var j = -(startJ); j <= startJ; j++)
                {
                    if ((Math.Abs(i) != Math.Abs(j)) && (Math.Abs(i) + Math.Abs(j) != 3))
                    {
                        point = new PointInMap();
                        point.X = currentPoint.X + i;
                        point.Y = currentPoint.Y + j;
                        if ((point.X >= 1) && (point.X < _map.Height) && (point.Y >= 1) && (point.Y < _map.Width) && (!(_map.ArrayMap[point.X, point.Y] is Bomb)))
                        {
                            returnList.Add(point);
                        }

                    }

                }

            }
            returnList.Add(currentPoint);
            return returnList;
        }

        /// <summary>
        /// Считает количество секунд проведенных апгрейдами на карте 
        /// и уничтожает их, после истечения промении
        /// </summary>
        public void UpgradeUpTime()
        {
            for (int rowCnt = 0; rowCnt <= _map.Height; rowCnt++)
            {
                for (int colCnt = 0; colCnt <= _map.Width; colCnt++)
                {
                    var item = _map.ArrayMap[rowCnt, colCnt];
                    var farUpgrate = item as UpgradeFar;
                    if (farUpgrate != null)
                    {
                        farUpgrate.TimeOnMap += 1;
                        if (farUpgrate.TimeOnMap == 5)
                        {
                            farUpgrate.NeedClear = true;
                            _map.ArrayMap[rowCnt, colCnt] = new Space();
                        }
                        else
                        {
                            _map.ArrayMap[rowCnt, colCnt] = farUpgrate;
                        }
                    }
                    var remoteUpgrate = item as UpgradeRemote;
                    if (remoteUpgrate != null)
                    {
                        remoteUpgrate.TimeOnMap += 1;
                        if (remoteUpgrate.TimeOnMap == 5)
                        {
                            remoteUpgrate.NeedClear = true;
                            _map.ArrayMap[rowCnt, colCnt] = new Space();
                        }
                        else
                        {
                            _map.ArrayMap[rowCnt, colCnt] = remoteUpgrate;
                        }
                        continue;
                    }
                    var multiBobmUpgrate = item as UpgradeMultiBomb;
                    if (multiBobmUpgrate != null)
                    {
                        multiBobmUpgrate.TimeOnMap += 1;
                        if (multiBobmUpgrate.TimeOnMap == 5)
                        {
                            multiBobmUpgrate.NeedClear = true;
                            _map.ArrayMap[rowCnt, colCnt] = new Space();
                        }
                        else
                        {
                            _map.ArrayMap[rowCnt, colCnt] = multiBobmUpgrate;
                        }
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// формирует следующий уровень
        /// </summary>
        public void GetNextLevel()
        {

            _map.Load(_mapManager.GetNextMap());
            _map.SetUpgradeOnMap(10, 10, 10);
            _bomber.IsHaveFarUpgrate = false;
            _bomber.IsHaveMultiBombUpgrade = false;
            _bomber.IsHaveRemoteUpgrate = false;
            ListWithMonster = new List<Monster>();
            ListWithBomb = new List<Bomb>();
            SetPositionForPerson(2, 2);
            SetPositionForPerson(4, 2);
            _display.DrawMap();





        }

        /// <summary>
        /// ищем новую позицию для монстра и рисуем монстра
        /// </summary>
        /// <param name="monster"></param>
        public void SetNewPositionMonster(Monster monster)
        {
            PointInMap point = new PointInMap();
            _map.ArrayMap[monster.Position.X, monster.Position.Y] = new Space();


            if (monster.TypeMonsters != 4)
            {
                monster.Position = SearchNextPositionToCurrentMonster(monster);

            }
            else
            {
                point = SearchPositionToCleverMonster(monster);
                if (point != null)
                {
                    monster.Position = point;
                }

            }

            if (monster.TypeMonsters == 2) //не летающий 
            {

                _map.ArrayMap[monster.Position.X, monster.Position.Y] = new MonsterNotFly();
            }
            if (monster.TypeMonsters == 3)
            {
                _map.ArrayMap[monster.Position.X, monster.Position.Y] = new MonsterFly();
            }
            if (monster.TypeMonsters == 4)
            {

                _map.ArrayMap[monster.Position.X, monster.Position.Y] = new MonsterClever();
            }

        }
        public PointInMap SearchPositionToCleverMonster(Monster monster)
        {

            var target = new PointInMap(monster.Position.X - 1, monster.Position.Y - 1);
            int stepNumberToTargetPosition = 0;
            int stepNumber = 1;
            int emptyCell = 0;
            List<PointInMap> pointCollection;
            PointInMap resultPoint = new PointInMap();

            List<PointInMap> currentPosition;
            int[,] mapInInt = ParseEntityToInt(_map.ArrayMap);

            while (true)
            {
                while (true)
                {
                    if (_bomber.IsDead) //если бомбер мертв,следуюий умный монстр не сможет шагнуть,так как бомбера фактически нет на карте
                    {
                        break;
                    }
                    currentPosition = new List<PointInMap>();
                    emptyCell = 0;

                    for (int colCnt = 0; colCnt <= _map.Height - 2; colCnt++)
                    {

                        for (int rowCnt = 0; rowCnt <= _map.Width - 2; rowCnt++)
                        {

                            if (mapInInt[colCnt, rowCnt] == stepNumber)
                            {
                                currentPosition.Add(new PointInMap(colCnt, rowCnt));
                                continue;
                            }
                            if (mapInInt[colCnt, rowCnt] != Const.SPACE_CLEVER)
                            {
                                emptyCell++;
                                continue;
                            }
                        }

                    }
                    if (emptyCell == (_map.Height - 1) * (_map.Width - 1))
                    {

                        break;
                    }
                    stepNumber++;
                    foreach (var c in currentPosition)
                    {
                        pointCollection = new List<PointInMap>();
                        pointCollection.Add(new PointInMap(c.X + 1, c.Y));
                        pointCollection.Add(new PointInMap(c.X + -1, c.Y));
                        pointCollection.Add(new PointInMap(c.X, c.Y + 1));
                        pointCollection.Add(new PointInMap(c.X, c.Y - 1));

                        foreach (var p in pointCollection)
                        {
                            if ((p.X >= 0) && (p.X < _map.Height - 1) && (p.Y >= 0) && (p.Y < _map.Width - 1))
                            {
                                if (mapInInt[p.X, p.Y] == Const.SPACE_CLEVER)
                                {
                                    mapInInt[p.X, p.Y] = stepNumber;
                                }
                            }
                        }
                    }

                }
                if (_bomber.IsDead)
                {
                    break;
                }
                stepNumberToTargetPosition = mapInInt[target.X, target.Y];



                pointCollection = new List<PointInMap>();
                pointCollection.Add(new PointInMap(target.X + 1, target.Y));
                pointCollection.Add(new PointInMap(target.X + -1, target.Y));
                pointCollection.Add(new PointInMap(target.X, target.Y + 1));
                pointCollection.Add(new PointInMap(target.X, target.Y - 1));
                foreach (var p in pointCollection)
                {
                    if ((p.X >= 0) && (p.X < _map.Height - 1) && (p.Y >= 0) && (p.Y < _map.Width - 1))
                    {
                        if (mapInInt[p.X, p.Y] == stepNumberToTargetPosition - 1)
                        {
                            resultPoint = new PointInMap();
                            resultPoint.X = p.X + 1;
                            resultPoint.Y = p.Y + 1;
                            break;
                        }
                    }

                }
                if (_map.ArrayMap[resultPoint.X, resultPoint.Y] is Bomber)
                {
                    _bomber.IsDead = true;
                }


                return resultPoint;
             //   break;
            }
            return null;
        }



        public int[,] ParseEntityToInt(Entity[,] mapWithEntity)
        {
            int[,] result = new int[_map.Height - 1, _map.Width - 1];
            for (int colCnt = 1; colCnt <= _map.Height - 1; colCnt++)
            {
                for (int rowCnt = 1; rowCnt <= _map.Width - 1; rowCnt++)
                {
                    var item = mapWithEntity[colCnt, rowCnt];
                    if ((item is Space) || (item is MonsterFly) || (item is MonsterClever) || (item is MonsterNotFly) || (item is Bomber) || (item is Bomb) || (item is BomberOnBomb))
                    {
                        if ((!(item is Bomber)) && (!(item is BomberOnBomb)))
                        {
                            result[colCnt - 1, rowCnt - 1] = Const.SPACE_CLEVER;
                            continue;
                        }
                        else
                        {
                            result[colCnt - 1, rowCnt - 1] = Const.BOMBER_CLEVER;
                        }
                    }

                    else
                    {
                        result[colCnt - 1, rowCnt - 1] = Const.WALL_CLEVER;
                    }

                }
            }
            return result;
        }
        public void PressPause(bool isPaused)
        {
            if (!isPaused)
            {
                isPaused = true;
            }
            else
            {
                isPaused = false;
            }
            while (isPaused)
            {
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}





