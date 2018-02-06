using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BomberSoz.Model
{
    public class Monster : Entity
    {

        public Monster(bool can, byte typeOfMonster)
        {
            this.CanFly = can;
            this.TypeMonsters = typeOfMonster;
            Random rand = new Random();

            this.Direction = rand.Next(1, 4);

        }

        public bool IsCrash { get; set; } = false;
        public bool CanFly { get; set; } = false;

        public int Direction;//1- вверх 2 вниз 3-влево 4-вправо


    }
}
