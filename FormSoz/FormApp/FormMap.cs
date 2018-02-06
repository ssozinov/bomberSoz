using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomberSoz.Manager;
using BomberSoz.Model;
using BomberSoz.Core;
using System.IO;

namespace FormSoz.FormApp
{
   public class FormMap : IMap
    {
        public int Width { get; private set; }
        public int BombTimer { get; set; }
        public int Height { get; set; }
        public Entity[,] ArrayMap { get; set; }
        public int[,] ArrayForCleverMonster { get; set; }


        public PointInMap FirstPositionBomber { get; set; }


        private Random _rand = new Random((int)DateTime.Now.Ticks);

        public FormMap(int width, int height, int bombTimer)
        {
            Width = width + 1; // +1 для нерушимых стенок
            Height = height + 1;

            BombTimer = bombTimer;
            FirstPositionBomber = new PointInMap();
            ArrayMap = new Entity[height + 2, width + 2];
            ArrayForCleverMonster = new int[height + 2, width + 2];
        }

        public Entity[,] Load(string Path)
        {
            {



                for (int rowCount = 0; rowCount <= Width; rowCount++)
                {
                    for (int colCnt = 0; colCnt <= Height; colCnt++)
                    {
                        if (((rowCount == 0) && (colCnt == 0)) || ((colCnt == Height) && (rowCount == Width)) || ((colCnt == Height) && (rowCount == 0)) || ((colCnt == 0) && (rowCount == Width)))
                        {

                            ArrayMap[colCnt, rowCount] = ParseMapToCurrentDisplay(ConsoleConsts.SYMBOL_MAP_ANGLE);
                            continue;
                        }
                        if ((rowCount == 0) || (rowCount == Width))
                        {

                            ArrayMap[colCnt, rowCount] = ParseMapToCurrentDisplay(ConsoleConsts.SYMBOL_MAP_WALL_VERTICAL);
                            continue;
                        }
                        if ((colCnt == 0) || (colCnt == Height))
                        {

                            ArrayMap[colCnt, rowCount] = ParseMapToCurrentDisplay(ConsoleConsts.SYMBOL_MAP_WALL_HORIZONTAL);
                            continue;
                        }
                    }

                }

            }
            StreamReader streamReader = null;
            try
            {
                streamReader = new StreamReader(Path);
            }

            catch
            {
                Logging.WriteInfo("Файл для загрузки карты не найден");
            }

            int heightCnt = 0;
            int rowCnt = 0;
            while (!streamReader.EndOfStream)
            {

                string buferString = "";
                heightCnt++; //считаем количество строк в файле
                if (heightCnt > Height)
                {
                    Logging.WriteInfo("Чисило строк превышает указанное в конфигурации");

                    break;
                }
                buferString += streamReader.ReadLine();
                for (int i = 0; i < buferString.Length - 1; i++)
                {
                    if (buferString.Length + 1 != Width)
                    {
                        Logging.WriteInfo("Ширина не соответствует высотой в конфиге");
                        break;
                    }
                    if ((!buferString[i].Equals(ConsoleConsts.SYMBOL_WALL)) && (!buferString[i].Equals(ConsoleConsts.SYMBOL_SPACE)) &&
                        (!buferString[i].Equals(ConsoleConsts.SYMBOL_BOMBER)))
                    {
                        Logging.WriteInfo("На карте используются символы #");

                        break;

                    }


                    for (int colCnt = 1; colCnt <= Width - 1; colCnt++)
                    {

                        if ((buferString[colCnt - 1].Equals(ConsoleConsts.SYMBOL_BOMBER)))
                        {

                            FirstPositionBomber.X = rowCnt;
                            FirstPositionBomber.Y = colCnt;
                        }
                        this.ArrayMap[rowCnt + 1, colCnt] = ParseMapToCurrentDisplay(buferString[colCnt - 1]);
                    }

                }

                rowCnt++;



            }
            Logging.WriteInfo("Карта считана");
            return ArrayMap;
        }

        public object Clone()
        {
            // делаем дубликат карты
            var ret = (FormMap)this.MemberwiseClone();
            ret.ArrayMap = new Entity[Height + 1, Width + 1];
            Array.Copy(ArrayMap, ret.ArrayMap, (Height) * (Width));
            ret.FirstPositionBomber = new PointInMap()
            {
                X = this.FirstPositionBomber.X,
                Y = this.FirstPositionBomber.Y
            };
            return ret;
        }

        public Entity ParseMapToCurrentDisplay(char symbolFromMap)
        {
            Entity result = new Entity();
            switch (symbolFromMap)
            {
                case ConsoleConsts.SYMBOL_BOMBER:
                    result = new Bomber();
                    break;
                case ConsoleConsts.SYMBOL_SPACE:
                    result = new Space();
                    break;
                case ConsoleConsts.SYMBOL_WALL:
                    result = new Wall();
                    break;
                case (char)4:
                    result = new WallAngle();
                    break;
                case ConsoleConsts.SYMBOL_MAP_WALL_VERTICAL:
                    result = new WallVerticale();
                    break;
                case ConsoleConsts.SYMBOL_MAP_WALL_HORIZONTAL:
                    result = new WallHorizontal();
                    break;


            }
            return result;
        }

        /// <summary>
        /// Размещает на карте апгрейды для бомбера
        /// </summary>
        /// <param name="itemCountFar">количество апгрейдов на дальность</param>
        /// <param name="itemCountDistance">количество агрейдов на дистанционное управление</param>
        public void SetUpgradeOnMap(int itemCountFar = 0, int itemCountDistance = 0, int itemCountMiltiBomb = 0)
        {
            int randomIndex = 0;
            bool isNewIndex = false;
            List<int> alrearyUseIndex = new List<int>();
            List<PointInMap> listWithWall = new List<PointInMap>();
            for (int i = 1; i < Height; i++)
            {
                for (int j = 1; j < Width; j++)
                {
                    var tempPoint = new PointInMap();
                    var item = ArrayMap[i, j];
                    var tempWall = item as Wall;

                    if (tempWall != null)
                    {
                        tempPoint.X = i;
                        tempPoint.Y = j;
                        listWithWall.Add(tempPoint);
                    }
                }
            }
            for (int itemCnt = 0; itemCnt < itemCountFar + itemCountDistance + itemCountMiltiBomb; itemCnt++)
            {
                isNewIndex = false;
                while (!isNewIndex)
                {
                    randomIndex = _rand.Next(listWithWall.Count);
                    if (alrearyUseIndex.Where(n => n == randomIndex).Count() == 0)
                    {
                        isNewIndex = true;
                    }
                }
                alrearyUseIndex.Add(randomIndex);

                if (itemCnt < itemCountFar)
                {

                    ArrayMap[listWithWall[randomIndex].X, listWithWall[randomIndex].Y] = new WallWithUpgradeFar();
                    continue;
                }


                if ((itemCnt >= itemCountDistance) && (itemCnt <= itemCountDistance))
                {
                    ArrayMap[listWithWall[randomIndex].X, listWithWall[randomIndex].Y] = new WallWithUpgradeRemote();
                    continue;

                }
                if (itemCnt >= (itemCountDistance + itemCountFar))
                {
                    ArrayMap[listWithWall[randomIndex].X, listWithWall[randomIndex].Y] = new WallWithUpgradeMultiBomb();
                    continue;
                }

            }

        }
    }
}
