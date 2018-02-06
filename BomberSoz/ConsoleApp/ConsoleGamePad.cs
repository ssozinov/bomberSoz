using System;
using BomberSoz.NewFolder1;

namespace BomberSoz.ConsoleApp
{
    public class ConsoleGamePad : IGamePad
    {
        public Action PressLeft { get; set; }
        public Action PressRight { get; set; }
        public Action PressUp { get; set; }
        public Action PressDown { get; set; }


        public Action PressSpace { get; set; }
        public Action PressR { get; set; }

        public Action PressP { get; set; }
        public void Start()
        {
            ConsoleKeyInfo key;
            while (true)
            {

                key = Console.ReadKey();

                switch (key.Key)
                {

                    case ConsoleKey.LeftArrow:
                        var tempLeft = PressLeft;
                        tempLeft.Invoke();
                        break;
                    case ConsoleKey.RightArrow:
                        var tempRight = PressRight;
                        tempRight.Invoke();
                        break;
                    case ConsoleKey.DownArrow:
                        var temp = PressDown;
                        if (temp != null)
                        {
                            temp(); // temp.Invoke(); одинаково читай про делегаты
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        var tempUp = PressUp;
                        if (tempUp != null)
                        {
                            tempUp();
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        var tempSpace = PressSpace;
                        if (tempSpace != null)
                        {
                            tempSpace(); // temp.Invoke(); 
                        }
                        break;
                    case ConsoleKey.R:
                        var tempR = PressR;
                        if (tempR != null)
                        {
                            tempR(); // temp.Invoke(); 
                        }
                        break;
                 
                }
              
                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
               

            }
        }
    }
}
